﻿using DiamondShopSystem.API.DTO;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.pipeline.html;
using iTextSharp.tool.xml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model.Models;
using Services.EmailServices;
using Services.ShippingService;
using Services.Users;
using static Repository.Shippings.ShippingRepository;
using iTextSharp.text;

namespace DiamondShopSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssignController : ControllerBase
    {
        private readonly IShippingService _shippingService;
        private readonly IManagerService _managerService;
        private readonly IOrderService _orderService;
        private readonly IEmailService _emailService;

        public AssignController(IShippingService shippingService, IManagerService managerService, IOrderService orderService,IEmailService e)
        {
            _shippingService = shippingService;
            _managerService = managerService;
            _orderService = orderService;
            _emailService = e;
        }
        //[HttpPost("SendPdf")]
        //public IActionResult GeneratePdf([FromBody] HtmlContentModel model)
        //{
        //    byte[] pdfBytes;
        //    using (var ms = new MemoryStream())
        //    {
        //        var document = new Document();
        //        PdfWriter writer = PdfWriter.GetInstance(document, ms);
        //        document.Open();
        //        using (var strReader = new StringReader(model.HtmlContent))
        //        {
        //            HtmlPipelineContext htmlContext = new HtmlPipelineContext(null);
        //            htmlContext.SetTagFactory(Tags.GetHtmlTagProcessorFactory());

        //            ICSSResolver cssResolver = XMLWorkerHelper.GetInstance().GetDefaultCssResolver(false);
        //            // Add CSS if necessary
        //            // cssResolver.AddCssFile("path_to_css_file", true);

        //            IPipeline pipeline = new CssResolverPipeline(cssResolver, new HtmlPipeline(htmlContext, new PdfWriterPipeline(document, writer)));
        //            var worker = new XMLWorker(pipeline, true);
        //            var xmlParse = new XMLParser(true, worker);
        //            xmlParse.Parse(strReader);
        //            xmlParse.Flush();
        //        }
        //        document.Close();
        //        pdfBytes = ms.ToArray();
        //    }
        //    return Ok(pdfBytes);
        //    //return File(pdfBytes, "application/pdf", "generated.pdf");
        //}
        [HttpGet("getAllOrdersFromShipping")]
        public async Task<ActionResult<List<OrderAssigned>>> GetAllOrdersFromShipping()
        {
            try
            {
                var orders = await _shippingService.GetAllOrdersAsync();

                if (orders == null || !orders.Any())
                {
                    return NotFound(new { Message = "No orders found for the given status." });
                }

                return Ok(orders);
            }
            catch (Exception ex)
            {
                // Log the exception (you can use a logging framework for this)
                return StatusCode(500, new { Message = "An error occurred while processing your request.", Details = ex.Message });
            }
        }
        [HttpGet("Shippings")]
        public async Task<ActionResult<List<Shipping>>> GetAllShipping()
        {
            var shippingList = await _shippingService.GetAllShippingAsync();

            if (shippingList == null || shippingList.Count == 0)
            {
                return NotFound();
            }

            return Ok(shippingList);
        }

        [HttpGet("shippingsByStatus/{status}")]
        public async Task<ActionResult<List<Shipping>>> GetShippingByStatus(string status)
        {
            var shippingList = await _shippingService.GetShippingByStatusAsync(status);

            if (shippingList == null || shippingList.Count == 0)
            {
                return NotFound();
            }

            return Ok(shippingList);
        }

        [HttpGet("shippingById/{shippingId}")]
        public async Task<ActionResult<Shipping>> GetShippingById(int shippingId)
        {
            var shipping = await _shippingService.GetShippingByIdAsync(shippingId);
            if (shipping == null)
            {
                return NotFound("There is no shipping match this Id:" + shippingId);
            }
            return Ok(shipping);
        }
        //Get the list of saleStaff base on the managerId
        [HttpGet("saleStaffListByManagerId/{managerId}")]
        public IActionResult GetSaleStaffByManagerId(int managerId)
        {
            var saleStaffs = _shippingService.GetSaleStaffByManagerId(managerId);

            if (_managerService.GetManagerById(managerId) == null)
            {
                return BadRequest("No sales staff found for the given manager ID.");
            }

            if (saleStaffs == null || !saleStaffs.Any())
            {
                return BadRequest("No sales staff found for the given manager ID.");
            }

            var saleStaffRequests = saleStaffs.Select(s => new SaleStaffRequest
            {
                Count = s.Count,
                Status = s.Status,
                SStaffId = s.SaleStaff.SStaffId,
                Name = s.SaleStaff.Name,
                Phone = s.SaleStaff.Phone,
                Email = s.SaleStaff.Email,
                ManagerId = s.SaleStaff.ManagerId
            }).ToList();

            return Ok(saleStaffRequests);
        }

        [HttpGet("deliveryStaffListByManagerId/{managerId}")]
        public IActionResult GetDeliveryStaffBySaleStaffId(int managerId)
        {
            var deliveryStaffs = _shippingService.GetDeliveryStaffByManagerId(managerId);

            if (_managerService.GetManagerById(managerId) == null)
            {
                return BadRequest("No sales staff found for the given manager ID.");
            }

            if (deliveryStaffs == null || !deliveryStaffs.Any())
            {
                return BadRequest("No sales staff found for the given manager ID.");
            }

            var deliveryStaffRequests = deliveryStaffs.Select(s => new DeliveryStaffRequest
            {
                Count = s.Count,
                Status = s.Status,
                DStaffId = s.DeliveryStaff.DStaffId,
                Name = s.DeliveryStaff.Name,
                Phone = s.DeliveryStaff.Phone,
                ManagerId = s.DeliveryStaff.ManagerId
            }).ToList();

            return Ok(deliveryStaffRequests);
        }

        [HttpGet("getAllSaleStaff")]
        public IActionResult GetAllSaleStaff()
        {
            var saleStaffs = _shippingService.GetAllSaleStaff();

            if (saleStaffs == null || !saleStaffs.Any())
            {
                return BadRequest("No sales staff found for the given manager ID.");
            }

            var saleStaffRequests = saleStaffs.Select(s => new SaleStaffRequest
            {
                Count = s.Count,
                Status = s.Status,
                SStaffId = s.SaleStaff.SStaffId,
                Name = s.SaleStaff.Name,
                Phone = s.SaleStaff.Phone,
                Email = s.SaleStaff.Email,
                ManagerId = s.SaleStaff.ManagerId
            }).ToList();

            return Ok(saleStaffRequests);
        }

        [HttpGet("getAllDeliveryStaff")]
        public IActionResult GetAllDeliveryStaff()
        {
            var deliveryStaffs = _shippingService.GetAllDeliveryStaff();

            if (deliveryStaffs == null || !deliveryStaffs.Any())
            {
                return BadRequest("No sales staff found for the given manager ID.");
            }

            var deliveryStaffRequests = deliveryStaffs.Select(s => new DeliveryStaffRequest
            {
                Count = s.Count,
                Status = s.Status,
                DStaffId = s.DeliveryStaff.DStaffId,
                Name = s.DeliveryStaff.Name,
                Phone = s.DeliveryStaff.Phone,
                ManagerId = s.DeliveryStaff.ManagerId
            }).ToList();

            return Ok(deliveryStaffRequests);
        }

        
        //Assign staff the order and make the shipping
        //Get orders in the shipping table base on the saleStaffId, let the staff see what are their Orders
        [HttpGet("ordersFromSaleStaffId/{saleStaffId}")]
        public async Task<ActionResult<List<OrderAssigned>>> GetOrdersBySaleStaffId(int saleStaffId)
        {
            try
            {
                var orders = await _shippingService.GetOrdersBySaleStaffIdAsync(saleStaffId);

                if (orders == null || orders.Count == 0)
                {
                    return NotFound("There are no orders matching your given data.");
                }
                return Ok(orders);
            }
            catch (Exception ex)
            {
                // Log the exception (you can use a logging framework for this)
                return StatusCode(500, new { Message = "An error occurred while processing your request.", Details = ex.Message });
            }
        }

        //Get orders in the shipping table base on the saleStaffId, let the staff see what are their Orders
        [HttpGet("ordersFromSaleStaffIdAndStatus/{saleStaffId}/{status}")]
        public async Task<ActionResult<List<OrderAssigned>>> GetOrdersBySaleStaffIdAndStatus(int saleStaffId, string status)
        {
            try
            {
                var orders = await _shippingService.GetOrdersBySaleStaffIdAndStatusAsync(saleStaffId, status);

                if (orders == null || orders.Count == 0)
                {
                    return NotFound("There are no orders matching your given data.");
                }

                return Ok(orders);
            }
            catch (Exception ex)
            {
                // Log the exception (you can use a logging framework for this)
                return StatusCode(500, new { Message = "An error occurred while processing your request.", Details = ex.Message });
            }
        }


        [HttpGet("getOrdersByDeliveryStaffId/{deliveryStaffId}/{status}")]
        public async Task<IActionResult> GetOrdersByDeliveryStaffId(int deliveryStaffId, string status = "Shipping")
        {
            try
            {
                var orders = await _shippingService.GetOrdersByDeliveryStaffIdAsync(deliveryStaffId, status);
                if (orders == null)
                {
                    return NotFound($"No orders found for delivery staff ID {deliveryStaffId}");
                }

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("assignStaff")]
        public async Task<IActionResult> CreateShipping(int orderId, int saleStaffId, string status = "Approve")
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var shipping = await _shippingService.AssignOrderAsync(status, orderId, saleStaffId);
                return Ok("1");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: ");
            }
        }

        //Assgin to deliveryStaff and change the status
        [HttpPost("assignDelivery")]
        public async Task<IActionResult> AssignShippingToDelivery(int orderId, int deliveryStaffId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var order = await _shippingService.AssignShippingToDeliveryAsync(orderId, deliveryStaffId);
                return Ok(order.OrderId);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("confirmFinishOrder/{orderId}")]
        public async Task<IActionResult> IsConfirmFinishOrder(int orderId)
        {
            var result = await _shippingService.IsConfirmFinishShippingAsync(orderId);
            if (!result)
            {
                return NotFound(new { Message = "Shipping not found." });
            }

            return Ok(new { Message = "Order and Shipping statuses updated successfully." });
        }

        
    }

}
