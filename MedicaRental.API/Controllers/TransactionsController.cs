using MedicaRental.BLL.Dtos;
using MedicaRental.BLL.Dtos.CartItem;
using MedicaRental.BLL.Managers;
using MedicaRental.DAL.Context;
using MedicaRental.DAL.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Stripe;
using System;
using System.Collections.Generic;

namespace MedicaRental.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionsManager _transactionsManager;
        private readonly ITransactionItemsManager _transactionItemsManager;
        private readonly IItemsManager _itemsManager;
        private readonly UserManager<AppUser> _userManager;

        public TransactionsController(ITransactionsManager transactionsManager, ITransactionItemsManager transactionItemsManager, IItemsManager itemsManager, UserManager<AppUser> userManager)
        {
            _transactionsManager = transactionsManager;
            _transactionItemsManager = transactionItemsManager;
            _userManager = userManager;
            _itemsManager = itemsManager;
        }

        [Authorize]
        [HttpPost("create-payment-intent")]
        public async Task<ActionResult<PaymentIntent>> CreateStripePaymentIntent(IEnumerable<CartItemMinimalDto> items)
        {
            var amount = _itemsManager.GetTotalPrice(items.Select(i => i.ItemId));

            var paymentIntentService = new PaymentIntentService();
            var paymentIntent = paymentIntentService.Create(new PaymentIntentCreateOptions
            {
                Amount = decimal.ToInt64(amount),
                Currency = "egp",
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                },
            });

            if (paymentIntent is null) return StatusCode(StatusCodes.Status503ServiceUnavailable);

            Console.WriteLine($"Client Sercre {paymentIntent.ClientSecret}");

            var transactionId = await _transactionsManager.InsertTransaction(new()
            {
                Ammount = paymentIntent.Amount / 100,
                PaymentId = paymentIntent.Id,
                ClientId = _userManager.GetUserId(User)
            });

            if (transactionId is null) 
                return StatusCode(StatusCodes.Status500InternalServerError);

            var inserted = await _transactionItemsManager.InsertTransactionItems(items, (Guid)transactionId);

            if(!inserted)
                return StatusCode(StatusCodes.Status500InternalServerError);


            return Ok(paymentIntent.ToJson());
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> Index()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            const string endpointSecret = "whsec_26839dec5f0f476ef9b48676d3fb0fbff6953e35133eb5e20b6eab9d9e310e54";
            try
            {
                var stripeEvent = EventUtility.ParseEvent(json);
                var signatureHeader = Request.Headers["Stripe-Signature"];

                stripeEvent = EventUtility.ConstructEvent(json,
                        signatureHeader, endpointSecret);

                if (stripeEvent.Type == Events.PaymentIntentSucceeded)
                {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    Console.WriteLine("A successful payment for {0} was made.", paymentIntent.Amount);
                    TransactionDto? t = await _transactionsManager.GetByPaymentIdAsync(paymentIntent.Id);
                    if (t is null)
                    {
                        await Console.Out.WriteLineAsync("payment was not saved to DB");
                    }
                    _ = await _transactionsManager
                         .UpdateTransaction(new UpdateTransactionStatusDto(paymentIntent.Id, TransactionStatus.Success));
                }
                else if (stripeEvent.Type == Events.PaymentMethodAttached)
                {
                    var paymentMethod = stripeEvent.Data.Object as PaymentMethod;
                    // Then define and call a method to handle the successful attachment of a PaymentMethod.
                    // handlePaymentMethodAttached(paymentMethod);
                }
                else
                {
                    Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
                }
                return Ok();
            }
            catch (StripeException e)
            {
                Console.WriteLine("Error: {0}", e.Message);
                return BadRequest();
            }
            catch (Exception e)
            {
                return StatusCode(500);
            }

        }
    }
}

