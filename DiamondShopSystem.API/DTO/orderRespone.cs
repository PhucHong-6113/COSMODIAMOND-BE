﻿namespace DiamondShopSystem.API.DTO
{
    public class orderRespone
    {
        public int id {  get; set; }
        public string status { get; set; }
        public List<orderItem> items { get; set; }  
    }
    public class orderItem
    {
        public int pid { get; set; }
        public string name1 { get; set; }
        public int quantity { get; set; }
        public decimal total {  get; set; }
        public string img { get; set; }
        public string size { get; set; }
        public string metal { get; set; }
    }
}
