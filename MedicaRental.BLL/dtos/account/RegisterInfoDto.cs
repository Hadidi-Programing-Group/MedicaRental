using MedicaRental.BLL.Dtos.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Dtos;

public record RegisterInfoDto
{
    [Required(ErrorMessage = "First Name is required")]
    [MinLength(3, ErrorMessage = "Name can't be less that three characters")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last Name is required")]
    [MinLength(3, ErrorMessage = "Name can't be less that three characters")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [RegularExpression(@"^[a-zA-Z0-9+_.-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Email format is not valid it must be in the form name@example.com")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;

    [RegularExpression(@"^(010|011|012|015)\d{8}$", ErrorMessage = "Invalid Egyptian phone number.")]
    public string PhoneNumber { get; set; } = string.Empty;

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
