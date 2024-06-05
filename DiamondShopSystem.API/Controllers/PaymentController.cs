﻿using Microsoft.AspNetCore.Mvc;
using Model.Models;
using Services.Charge;
using Repository.Charge;

namespace DiamondShopSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : Controller
    {
        // payment service VNPAY
        private readonly Ivnpay _vnPayService;
        private readonly IVnPayRepository _vnPayRepository;

        // payment service Stripe
        private readonly IStripeService _stripeService;
        private readonly IStripeRepository _stripeRepository;
        private readonly IConfiguration _configuration;

        // payment service Paypal
        private readonly IPaypalService _paypalService;
        private readonly IPaypalRepository _paypalRepository;

        public PaymentController(Ivnpay vnPayService, IVnPayRepository vnPayRepository, IStripeService stripeService, IStripeRepository stripeRepository,IPaypalRepository paypalRepository, IPaypalService paypalService ,IConfiguration configuration)
        {
            _vnPayService = vnPayService;
            _vnPayRepository = vnPayRepository;
            _stripeService = stripeService;
            _stripeRepository = stripeRepository;
            _configuration = configuration;
            _paypalService = paypalService;
            _paypalRepository = paypalRepository;

        }

        [HttpPost("CreatePayment-VNPAY")]
        public IActionResult CreatePayment(int orderId)
        {
            Order order = _vnPayRepository.GetOrderById(orderId);
            if (order == null || order.TotalAmount <= 0)
            {
                return BadRequest("Invalid order data.");
            }

            try
            {
                // Save the order to the database
                /*                _vnPayRepository.SaveOrder(order);*/

                // Create VNPay payment URL
                /*string returnUrl = Url.Action("PaymentReturn", "Checkout", null, Request.Scheme);*/
                var returnUrl = "https://google.com.vn";
                string paymentUrl = _vnPayService.CreatePaymentUrl(order, returnUrl);

                return Ok(new { url = paymentUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("PaymentReturn-VNPAY")]
        public IActionResult PaymentReturn()
        {
            string queryString = Request.QueryString.Value;
            var vnp_HashSecret = "MEIJ0KIOZC8Z8ZU2A5W28CT7RAC6K9I0";

            // Validate the signature of the payment URL
            if (!string.IsNullOrEmpty(queryString) && _vnPayService.ValidateSignature(queryString, vnp_HashSecret))
            {
                // Retrieve the order ID from the query string
                if (int.TryParse(Request.Query["vnp_TxnRef"], out int orderId))
                {
                    Order order = _vnPayRepository.GetOrderById(orderId);

                    if (order != null)
                    {
                        order.Status = "Paid";
                        _vnPayRepository.SaveOrder(order);
                        return Ok("Payment successful.");
                    }
                }
            }

            return BadRequest("Invalid payment.");
        }

        [HttpPost("create-payment-intent-STRIPE")]
        public async Task<IActionResult> CreatePaymentIntent(int orderId)
        {
            var order = await _stripeRepository.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                return NotFound("Order not found");
            }

            var clientSecret = await _stripeService.CreatePaymentIntent(order.TotalAmount, "usd");
            var publishableKey = _configuration["Stripe:PublishableKey"];
            var returnData = new { clientSecret, publishableKey };
            return Json(returnData);
        }

        [HttpPost("webhook-STRIPE")]
        public async Task<IActionResult> Webhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var isHandled = await _stripeService.HandlePaymentWebhook(json);

            if (isHandled)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet("create-payment-PAYPAL")]
        public async Task<IActionResult> CreatePaymentPAYPAL(int orderId)
        {
            var order = await _paypalRepository.GetOrderByIdAsync(orderId);
            Console.WriteLine("OrderId Controller : " + order.OrderId);
            if (order == null)
            {
                return NotFound("Order not found");
            }

            string returnUrl = Url.Action("ExecutePayment", "Payment", new { orderId = orderId }, Request.Scheme) ?? string.Empty;
            string cancelUrl = Url.Action("CancelPayment", "Payment", new { orderId = orderId }, Request.Scheme) ?? string.Empty;

            if (string.IsNullOrEmpty(returnUrl) || string.IsNullOrEmpty(cancelUrl))
            {
                return BadRequest("Invalid return or cancel URL.");
            }

            var paymentUrl = await _paypalService.CreatePaymentAsync(order, returnUrl, cancelUrl);
            if (string.IsNullOrEmpty(paymentUrl))
            {
                return BadRequest("Failed to create payment URL.");
            }

            return Ok(new { Url = paymentUrl });
        }

        [HttpGet("execute-payment-PAYPAL")]
        public async Task<IActionResult> ExecutePayment(string paymentId, string payerId, int orderId)
        {
            if (await _paypalService.ExecutePaymentAsync(paymentId, payerId))
            {
                await _paypalRepository.UpdateOrderStatusAsync(orderId, "Paid");
                return Redirect("https://google.com");
            }
            else
            {
                await _paypalRepository.UpdateOrderStatusAsync(orderId, "Failed");
                return Redirect("https://youtube.com");
            }
        }

        [HttpGet("cancel-payment-PAYPAL")]
        public async Task<IActionResult> CancelPayment(int orderId)
        {
            await _paypalRepository.UpdateOrderStatusAsync(orderId, "Cancelled");
            return Redirect("https://facebook.com");
        }
    }
}
