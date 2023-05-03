using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Dtos.Brand;

public record DeleteBrandStatusDto(bool isDeleted, string StatusMessage);

