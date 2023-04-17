using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Dtos.Report;

public record DeleteReportStatusDto(bool isDeleted, string StatusMessage);
