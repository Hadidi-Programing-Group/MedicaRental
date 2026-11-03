using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Dtos;

public record CategoryWithSubCategoriesDto(Guid Id, string Name, byte[]? Icon, List<SubCategoryDto> SubCategories);
