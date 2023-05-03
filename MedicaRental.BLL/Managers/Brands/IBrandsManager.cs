using MedicaRental.BLL.Dtos;
using MedicaRental.BLL.Dtos.Brand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Managers;

public interface IBrandsManager
{
    Task<DeleteBrandStatusDto> DeleteByIdAsync(Guid id);
    Task<IEnumerable<BrandDto>> GetAllAsyc();
    Task<BrandDto> GetBrandbyId(Guid id);
    Task<InsertBrandStatusDto> InsertNewBrand(InsertBrandDto insertBrandDto);
    Task<UpdateBrandStatusDto> UpdateNewBrand(Guid id, UpdateBrandDto updateBrandDto);
}
