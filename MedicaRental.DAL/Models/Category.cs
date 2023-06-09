﻿namespace MedicaRental.DAL.Models
{
    public class Category
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; } = string.Empty;
        
        public byte[]? Icon { get; set; }

        public ICollection<Item> Items { get; set; } = new List<Item>();
        public ICollection<SubCategory> SubCategories { get; set; } = new List<SubCategory>();
    }
}
