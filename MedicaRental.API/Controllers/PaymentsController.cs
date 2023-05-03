using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
        public PaymentsController()
        {
        }
        [HttpPost("create-payment-intent")]
        public ActionResult<PaymentIntent> Create([FromBody] PaymentIntent intent)
        {
            Console.WriteLine(intent.Amount);
            var paymentIntentService = new PaymentIntentService();
            var paymentIntent = paymentIntentService.Create(new PaymentIntentCreateOptions
            {
                Amount = 5000,
                Currency = "usd",
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                },
            });
            Console.WriteLine($"Client Sercre {paymentIntent.ClientSecret}");
            //return Ok(new { client_secret = paymentIntent.ClientSecret });
            return Ok(paymentIntent.ToJson());

        }

    }

}