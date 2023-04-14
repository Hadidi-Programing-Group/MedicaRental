using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Dtos;

public record CategoryDto (int Id,  string Name, byte[]? Icon);
