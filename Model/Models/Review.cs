﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Model.Models;

public partial class Review
{
    public int ReviewId { get; set; }

    public string Review1 { get; set; }

    public int? Rating { get; set; }

    public DateOnly? ReviewDate { get; set; }

    public int? CusId { get; set; }

    public virtual Customer Cus { get; set; }
}