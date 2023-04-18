using MedicaRental.DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Dtos.Report;


public record InsertReportDtos(string Name,string Statement,string ReportedId,string ReporteeId,
   Guid? MessageId, Guid? ReviewId,Guid? ItemId);
