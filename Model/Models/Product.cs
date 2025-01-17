﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Model.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string ProductName { get; set; }

    public decimal? UnitPrice { get; set; }

    public int DiamondId { get; set; }

    public int CoverId { get; set; }

    public int MetaltypeId { get; set; }

    public int SizeId { get; set; }

    public string Pp { get; set; }

    public string Status { get; set; }

    public virtual ICollection<CartProduct> CartProducts { get; set; } = new List<CartProduct>();

    public virtual Cover Cover { get; set; }

    public virtual Diamond Diamond { get; set; }

    public virtual ICollection<FavoriteProduct> FavoriteProducts { get; set; } = new List<FavoriteProduct>();

    public virtual Metaltype Metaltype { get; set; }

    public virtual ICollection<ProductOrder> ProductOrders { get; set; } = new List<ProductOrder>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual Size Size { get; set; }
}