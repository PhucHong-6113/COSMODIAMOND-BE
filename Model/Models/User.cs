﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Model.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public string Status { get; set; }

    public string Role { get; set; }

    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();

    public virtual ICollection<DeliveryStaff> DeliveryStaffs { get; set; } = new List<DeliveryStaff>();

    public virtual ICollection<Manager> Managers { get; set; } = new List<Manager>();

    public virtual ICollection<SaleStaff> SaleStaffs { get; set; } = new List<SaleStaff>();
}