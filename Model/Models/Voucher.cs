﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Model.Models;

public partial class Voucher
{
    public int VoucherId { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public DateOnly ExpDate { get; set; }

    public int? Quantity { get; set; }

    public int? Rate { get; set; }

    public int? CusId { get; set; }

    public virtual Customer CusNavigation { get; set; }

    public virtual ICollection<Customer> Cus { get; set; } = new List<Customer>();
}