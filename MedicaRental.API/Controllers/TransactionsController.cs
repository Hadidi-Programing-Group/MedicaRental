using MedicaRental.BL.MailService;
using MedicaRental.BLL.Dtos;
using MedicaRental.BLL.Dtos.CartItem;
using MedicaRental.BLL.Helpers;
using MedicaRental.BLL.Managers;
using MedicaRental.DAL.Context;
using MedicaRental.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Net;

namespace MedicaRental.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly IEmailSender _emailSender;
        private readonly ITransactionsManager _transactionsManager;
        private readonly ICartItemsManager _cartItemsManager;
        private readonly ITransactionItemsManager _transactionItemsManager;
        private readonly IItemsManager _itemsManager;
        private readonly UserManager<AppUser> _userManager;

        public TransactionsController(IEmailSender emailSender, ITransactionsManager transactionsManager, ITransactionItemsManager transactionItemsManager, IItemsManager itemsManager, ICartItemsManager cartItemsManager, UserManager<AppUser> userManager)
        {
            _emailSender = emailSender;
            _transactionsManager = transactionsManager;
            _transactionItemsManager = transactionItemsManager;
            _userManager = userManager;
            _itemsManager = itemsManager;
            _cartItemsManager = cartItemsManager;
        }

        [Authorize]
        [HttpPost("create-payment-intent")]
        public async Task<ActionResult<PaymentIntent>> CreateStripePaymentIntent()
        {
            var userId = _userManager.GetUserId(User);
            var cartItems = await _cartItemsManager.GetCartItemsAsync(userId);
            decimal amount = await _cartItemsManager.GetTotalPrice(userId);
            var items = cartItems.Select(i => new CartItemMinimalDto(i.Id, i.ItemId, i.NumberOfDays));


            var paymentIntentService = new PaymentIntentService();
            try
            {
                var paymentIntent = paymentIntentService.Create(new PaymentIntentCreateOptions
                {
                    Amount = decimal.ToInt64(amount) * 100,
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
                    ClientId = userId
                });
                if (transactionId is null)
                    return StatusCode(StatusCodes.Status500InternalServerError);


                var inserted = await _transactionItemsManager.InsertTransactionItems(items, (Guid)transactionId);
                if (!inserted)
                    return StatusCode(StatusCodes.Status500InternalServerError);


                return Ok(paymentIntent.ToJson());
            }
            catch
            {
                return BadRequest("Cart is Empty");
            }
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> Index()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            string endpointSecret = Environment.GetEnvironmentVariable("STRIPE_WEB_SEC") ??
                "whsec_26839dec5f0f476ef9b48676d3fb0fbff6953e35133eb5e20b6eab9d9e310e54";
            try
            {
                var stripeEvent = EventUtility.ParseEvent(json);
                var signatureHeader = Request.Headers["Stripe-Signature"];

                stripeEvent = EventUtility.ConstructEvent(json,
                        signatureHeader, endpointSecret);

                if (stripeEvent.Type == Events.PaymentIntentSucceeded)
                {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    TransactionDto? t = await _transactionsManager.GetByPaymentIdAsync(paymentIntent.Id);
                    if (t is null)
                    {
                        await Console.Out.WriteLineAsync("payment was not saved to DB");
                    }
                    _ = await _transactionsManager
                         .UpdateTransaction(
                        new UpdateTransactionStatusDto(paymentIntent.Id, TransactionStatus.Success));

                    StatusDto result = await _itemsManager.changeToAds(paymentIntent.Id);
                    if (result.StatusCode != HttpStatusCode.OK)
                        return StatusCode((int)result.StatusCode, result);

                    var resultDeleteFromCard = await _cartItemsManager.RemoveCartItemsAsync(t.ClientId);
                    if (resultDeleteFromCard.StatusCode == HttpStatusCode.OK)
                        return Ok(paymentIntent.ToJson());

                    var user = await _userManager.FindByIdAsync(t.ClientId); 

                    var callback = EmailHelpers.CreatePaymentConfirmEmail(user.Name, (paymentIntent.Amount / 100).ToString(), paymentIntent.Id, DateTime.Now);

                    var message = new EmailMessage(new string[] { user.Email }, "Payment Confirmation", callback);
                    await _emailSender.SendEmailAsync(message);

                    return StatusCode((int)resultDeleteFromCard.StatusCode, resultDeleteFromCard);

                }
                return Ok();
            }
            catch (StripeException e)
            {
                Console.WriteLine("Error: {0}", e.Message);
                Console.WriteLine("endpointSecret: {0}", endpointSecret);
                return BadRequest();
            }
            catch (Exception e)
            {
                return StatusCode(500);
            }

        }
    }
}

