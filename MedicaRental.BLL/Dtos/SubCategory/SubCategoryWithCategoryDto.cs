using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Dtos;
public record SubCategoryWithCategoryDto(Guid Id, string Name, string? Icon, Guid? CategoryId, string? CategoryName);
