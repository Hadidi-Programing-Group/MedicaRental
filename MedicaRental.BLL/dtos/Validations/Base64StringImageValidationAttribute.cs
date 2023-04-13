using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Dtos.Validations;

public class Base64StringImageValidationAttribute : ValidationAttribute
{

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        RegisterInfoDto? instance = validationContext.ObjectInstance as RegisterInfoDto;

        if (instance is null)
            return new ValidationResult("Object is null");


        if (!TryConvertFromBase64String(value?.ToString()))
        {
            return new ValidationResult("Image is in invalid format");
        }

        return null;
    }

    private bool TryConvertFromBase64String(string? base64String)
    {
        if (base64String is null)
        {
            return false;
        }
        try
        {
            byte[] bytes = Convert.FromBase64String(base64String);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
