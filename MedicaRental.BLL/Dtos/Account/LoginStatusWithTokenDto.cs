using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Dtos;

public record LoginStatusWithTokenDto(string StatusMessage, HttpStatusCode StatusCode, bool isAuthenticated, string? Token, DateTime? Expiry) : StatusDto (StatusMessage, StatusCode);
