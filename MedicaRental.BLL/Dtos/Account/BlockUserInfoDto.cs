using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Dtos;

public record BlockUserInfoDto
{
    public string Id { get; set; } = string.Empty;

    public DateTime EndDate { get; set; } = DateTime.UtcNow.AddYears(20);

    public Guid? ReportId { get; set; }
}
