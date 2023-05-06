using MedicaRental.BLL.Dtos.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Dtos;

public record UserProfileInfoWithIdDto(string Id,string Name, string FirstName, string LastName, string PhoneNumber, string Address, string Email, bool IsGrantedRent);

//public record UpdateProfileInfoDto(string FirstName, string LastName, string PhoneNumber, string Address, string Email);

public record UserApprovalInfoWithIdDto(string Id, string NationalId, string NationalImage, string UnionImage);

//public record UpdateApprovalInfoDto
//{
//    [RegularExpression(@"^\d{14}$", ErrorMessage = "National ID must be 14 numbers")]
//    public string NationalId { get; set; } = string.Empty;

//    [Required(ErrorMessage = "National ID image is required.")]
//    [Base64StringImageValidation]
//    public string NationalImage { get; set; } = string.Empty;

//    [Required(ErrorMessage = "Union Card image is required.")]
//    [Base64StringImageValidation]
//    public string UnionImage { get; set; } = string.Empty;
//}
