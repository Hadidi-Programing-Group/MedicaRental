using MedicaRental.BLL.Dtos;
using MedicaRental.BLL.Dtos.Transactions;
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
    public class PaymentsController : ControllerBase
    {
        private readonly ITransactionsManager _transactionsManager;
        private readonly UserManager<AppUser> _userManager;

        public PaymentsController(ITransactionsManager transactionsManager, UserManager<AppUser> userManager)
        {
            this._transactionsManager = transactionsManager;
            this._userManager = userManager;
        }
        [Authorize]
        [HttpPost("create-payment-intent")]
        public ActionResult<PaymentIntent> Create([FromBody] payment payment)
        {
            Console.WriteLine(payment.Amount);
            var paymentIntentService = new PaymentIntentService();
            var paymentIntent = paymentIntentService.Create(new PaymentIntentCreateOptions
            {
                Amount = payment.Amount,
                Currency = "usd",
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                },
            });
            Console.WriteLine($"Client Sercre {paymentIntent.ClientSecret}");

            _transactionsManager.InsertTransaction(new TransactionDto()
            {
                Ammount = paymentIntent.Amount / 100,
                PaymentId = paymentIntent.Id,
                UserId = _userManager.GetUserId(User)

            });
            //return Ok(new { client_secret = paymentIntent.ClientSecret });
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
                    _ = _transactionsManager
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
    public class payment
    {
        public int Amount { get; set; }
    }
}

