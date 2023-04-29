using MedicaRental.DAL.Context;
using MedicaRental.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.DAL.Repositories;

internal class ReportActionRepo : EntityRepo<ReportAction>, IReportActionRepo
{
    public ReportActionRepo(MedicaRentalDbContext context) : base(context)
    {
    }
}
