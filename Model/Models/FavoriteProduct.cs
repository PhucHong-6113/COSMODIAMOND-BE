﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Model.Models;

public partial class FavoriteProduct
{
    public int FavoriteId { get; set; }

    public int ProductId { get; set; }

    public virtual Favorite Favorite { get; set; }

    public virtual Product Product { get; set; }
}