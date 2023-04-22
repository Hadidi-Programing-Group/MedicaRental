using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Dtos;

public record UserProfileInfoDto(string Name, string FirstName, string LastName, string PhoneNumber, string Address, string Email, bool IsGrantedRent);

public record UpdateProfileInfoDto(string FirstName, string LastName, string PhoneNumber, string Address, string Email);

public record UserApprovalInfoDto(string NationalId, byte[] NationalImage, byte[] UnionImage);
