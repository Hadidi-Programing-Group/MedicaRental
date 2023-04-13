using MedicaRental.DAL.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Dtos.Validations;

public class UserRoleValidationAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (int.TryParse(value?.ToString(), out int role))
            return Enum.IsDefined(typeof(UserRoles), role);
        return 
            false;
    }
}
