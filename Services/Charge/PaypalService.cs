﻿using PayPal.Api;
using Microsoft.Extensions.Configuration;
using DAO;
using System.Globalization;

namespace Services.Charge
{
    public class PaypalService : IPaypalService
    {

        private readonly IConfiguration _configuration;
        private readonly DIAMOND_DBContext _context;
        private  readonly string clientId;
        private  readonly string clientSecret;


        public PaypalService(IConfiguration configuration, DIAMOND_DBContext context)
        {
            _configuration = configuration;
            _context = context;
            clientId = _configuration["Paypal:ClientId"];
            clientSecret = _configuration["Paypal:ClientSecret"];
        }



        public async Task<string> CreatePaymentAsync(Model.Models.Order order, string returnUrl, string cancelUrl)
        {

            try {
                var apiContext = new APIContext(new OAuthTokenCredential(clientId, clientSecret).GetAccessToken());
                var amount = order.TotalAmount;
                Console.WriteLine("OrderId: "+ order.OrderId + " Amount : " + amount);
                var payment = new Payment();
                payment.intent = "sale";
                payment.payer = new Payer { payment_method = "paypal" };
                payment.transactions = new List<Transaction>
                {
                    new Transaction
                    {
                        description = "Transaction description.",
                        invoice_number = order.OrderId.ToString(),
                        amount = new Amount
                        {
                            currency = "USD",
                            total = order.TotalAmount.ToString("F2", CultureInfo.InvariantCulture),
                        },
                    }
                };
                payment.redirect_urls = new RedirectUrls
                {
                    return_url = returnUrl,
                    cancel_url = cancelUrl,
                };

                /*var createdPayment = payment.Create(apiContext);*/
                var createPayment = payment.Create(apiContext);
                return createPayment.links.FirstOrDefault
                    (link => link.rel == "approval_url")?.href;
                /*return createdPayment.links.FirstOrDefault(link => link.rel == "approval_url")?.href;*/

            }
            catch (Exception ex)
            {
                // Log the exception message and stack trace
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                return null;
            }
            
        }

        public async Task<bool> ExecutePaymentAsync(string paymentId, string payerId)
        {
            if (string.IsNullOrEmpty(paymentId) || string.IsNullOrEmpty(payerId))
            {
                Console.WriteLine("Invalid paymentId or payerId.");
                return false;
            }

            try
            {
                var apiContext = new APIContext(new OAuthTokenCredential(clientId, clientSecret).GetAccessToken());
                var paymentExecution = new PaymentExecution { payer_id = payerId };
                var payment = new Payment { id = paymentId };
                var executedPayment = payment.Execute(apiContext, paymentExecution);
                return executedPayment.state.ToLower() == "approved";
            }
            catch (Exception ex)
            {
                // Log the exception message and stack trace
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                return false;
            }
        }
    }
}
