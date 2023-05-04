using MedicaRental.DAL.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace MedicaRental.BLL.Dtos
{
    public record AddItemDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Serial { get; set; }
        public string? Model { get; set; }       
        public int Stock { get; set; }
        public decimal Price { get; set; }
        public string? Image { get; set; }
  
        public bool IsListed { get; set; }
        public Guid BrandId { get; set; }
        public Guid CategoryId { get; set; }
        public Guid SubCategoryId { get; set; }

        public string? SellerId { get; set; }
    };
}
