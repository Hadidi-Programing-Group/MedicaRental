using MedicaRental.BLL.Dtos;
using MedicaRental.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace MedicaRental.BLL.Helpers;

public static class ReportHelper
{
    public static Expression<Func<Report, ReportDto>> ReportListSeletor = r => new ReportDto(
                r.Id,
                r.Name,
                r.IsSolved,
                r.CreatedDate.ToString("o"),
                r.SolveDate == null? "" : ((DateTime)r.SolveDate).ToString("o"),
                r.Reported!.Name,
                r.Reporter!.Name
                );
    public static Func<IQueryable<Report>, IIncludableQueryable<Report, object>> ReportListInclude = source => source.Include(s => s.Reported).ThenInclude(c => c.User).Include(s => s.Reporter).ThenInclude(c => c.User);
}
