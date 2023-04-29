using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Dtos;

public record DeleteMessageRequestDto(string UserId, Guid MessageId, Guid? ReportId);
