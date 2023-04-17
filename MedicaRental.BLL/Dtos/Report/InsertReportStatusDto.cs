using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Dtos.Report;


    public record InsertReportStatusDto(bool isCreated, Guid? Id, string StatusMessage);


