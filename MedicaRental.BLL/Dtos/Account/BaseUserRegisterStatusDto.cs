using MedicaRental.DAL.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Dtos;

public record BaseUserRegisterStatusDto(bool isCreated, string RegisterMessage, AppUser? NewUser);
