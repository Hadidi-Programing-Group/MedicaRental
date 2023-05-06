using MedicaRental.BLL.Dtos.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Dtos.Brand;

public record InsertBrandDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string CountryOfOrigin { get; set; } = string.Empty;

    [Base64StringImageValidation]
    public string Image { get; set; } = string.Empty;
}
