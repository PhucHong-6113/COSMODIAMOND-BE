﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Model.Models;

public partial class DeliveryStaff
{
    public int DStaffId { get; set; }

    public string Name { get; set; }

    public string Phone { get; set; }

    public int? ManagerId { get; set; }

    public virtual User DStaff { get; set; }

    public virtual Manager Manager { get; set; }

    public virtual ICollection<Shipping> Shippings { get; set; } = new List<Shipping>();
}