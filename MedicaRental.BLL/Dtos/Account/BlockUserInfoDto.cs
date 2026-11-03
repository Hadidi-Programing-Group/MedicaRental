using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Dtos;

public record BlockUserInfoDto
{

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [RegularExpression(@"^[a-zA-Z0-9+_.-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Email format is not valid it must be in the form name@example.com")]
    public string Email { get; set; } = string.Empty;

    public DateTime EndDate { get; set; } = DateTime.Now.AddYears(20);
}
