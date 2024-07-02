﻿using Model.Models;
using Services.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.OrdersManagement
{
    public interface IShippingService
    {
        Task<Shipping> GetShippingByIdAsync(int shippingId);
        Task<Shipping> AssignOrderAsync(string status, int orderId, int saleStaffId);
        Task<List<Order>> GetOrdersBySaleStaffIdAndStatusAsync(int saleStaffId, string status);
        Task<Order> GetOrderByOrderIdAsync(int orderId);
        Task AssignOrderToDeliveryAsync(int orderId, int deliveryStaffId);
        //Task UpdateShippingAsync(Shipping shipping);
        //Task DeleteShippingAsync(int shippingId);
    }

}
