﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Model.Models;

public partial class ShippingMethod
{
    public int ShippingMethodId { get; set; }

    public string MethodName { get; set; }

    public decimal Cost { get; set; }

    public string Description { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}