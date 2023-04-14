using MedicaRental.BLL.Dtos.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Dtos;

public record UpdateCategoryDto
{
    [Required(ErrorMessage = "Category name is required")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Category image is required.")]
    [Base64StringImageValidation]
    public string Icon { get; set; } = string.Empty;
}
