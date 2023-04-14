using MedicaRental.BLL.Dtos.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Dtos;

public record ClientRegisterInfoDto
{
    public BaseUserRegisterInfoDto BaseUserRegisterInfo { get; set; } = new();

    [RegularExpression(@"^\d{14}$", ErrorMessage = "National ID must be 14 numbers")]
    public string SSN { get; set; } = string.Empty;

    [Required(ErrorMessage = "Address is required")]
    public string Address { get; set; } = string.Empty;

    [Required(ErrorMessage = "National ID image is required.")]
    [Base64StringImageValidation]
    public string NationalIdImage { get; set; } = string.Empty;

    [Required(ErrorMessage = "Union Card image is required.")]
    [Base64StringImageValidation]
    public string UnionCardImage { get; set; } = string.Empty;
}
