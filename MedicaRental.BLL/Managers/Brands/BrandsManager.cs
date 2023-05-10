using MedicaRental.BLL.Dtos;
using MedicaRental.BLL.Dtos.Brand;
using MedicaRental.BLL.Helpers;
using MedicaRental.DAL.Context;
using MedicaRental.DAL.Models;
using MedicaRental.DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Managers
{
    public class BrandsManager : IBrandsManager
    {
        private readonly IUnitOfWork _unitOfWork;

        public BrandsManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DeleteBrandStatusDto> DeleteByIdAsync(Guid id)
        {
            await _unitOfWork.Brands.DeleteOneById(id);
            try
            {
                _unitOfWork.Save();
                return new DeleteBrandStatusDto(
                    isDeleted: true,
                    StatusMessage: "Brand was deleted successfully"
                    );
            }
            catch
            {
                return new DeleteBrandStatusDto(
                    isDeleted: false,
                    StatusMessage: "Coudn't remove Brand"
                    );
            }
        }

        public async Task<IEnumerable<BrandDto>> GetAllAsyc()
        {
            var brands = await _unitOfWork.Brands.GetAllAsync(
             selector: brand => new BrandDto(
                 brand.Id,
                 brand.Name,
                 brand.CountryOfOrigin,
                 SharedHelper.GetMimeFromBase64(Convert.ToBase64String(brand.Image!))
                )
             
         );

            return brands;
        }

        public async Task<PageDto<BrandDto>> GetAllPagedAsyc(int page, string? searchText)
        {
            var brands = await _unitOfWork.Brands.FindAllAsync(
                 predicate: c => searchText == null ||
                 MedicaRentalDbContext.LevDist(c.Name, searchText, SharedHelper.SearchMaxDistance) <= SharedHelper.SearchMaxDistance,

                 selector: brand => new BrandDto(
                     brand.Id,
                     brand.Name,
                     brand.CountryOfOrigin,
                     SharedHelper.GetMimeFromBase64(Convert.ToBase64String(brand.Image!))
                    ),
                 orderBy: searchText == null ? q => q.OrderBy(c => c.Name) :
                  q => q.OrderBy(c => MedicaRentalDbContext.LevDist(c.Name, searchText, SharedHelper.SearchMaxDistance)).ThenBy(c => c.Name),
                 skip: page > 1 ? (page - 1) * SharedHelper.Take : null,
                 take: SharedHelper.Take
             );


            var count = await _unitOfWork.Brands.GetCountAsync(
                predicate: c => searchText == null ||
                 MedicaRentalDbContext.LevDist(c.Name, searchText, SharedHelper.SearchMaxDistance) <= SharedHelper.SearchMaxDistance

                );

            return new(brands, count);
        }

        public async Task<BrandDto> GetBrandbyId(Guid id)
        {
            var brand = await _unitOfWork.Brands.FindAsync(
            predicate: b => b.Id == id);
            if (brand is null)
                return null;
            return new BrandDto(
                Id: brand.Id,
                Name: brand.Name,
                CountryOfOrigin: brand.CountryOfOrigin,
                Image: SharedHelper.GetMimeFromBase64(Convert.ToBase64String(brand.Image!)));
        }

        public async Task<InsertBrandStatusDto> InsertNewBrand(InsertBrandDto insertBrandDto)
        {
            Brand brand = new()
            {
                Name = insertBrandDto.Name,
                CountryOfOrigin= insertBrandDto.CountryOfOrigin,
                Image = Convert.FromBase64String(insertBrandDto.Image)
            };


            await _unitOfWork.Brands.AddAsync(brand);
            try
            {
                _unitOfWork.Save();
                return new InsertBrandStatusDto(
                    isCreated: true,
                    Id: brand.Id,
                    StatusMessage: "Brand Created Successfully"
                    );
            }
            catch
            {
                return new InsertBrandStatusDto(
                    isCreated: false,
                    Id: null,
                    StatusMessage: "Failed to insert new Brand"
                    );
            }
        }

        public async Task<UpdateBrandStatusDto> UpdateNewBrand(UpdateBrandDto updateBrandDto)
        {
            var brand = await _unitOfWork.Brands.FindAsync(b => b.Id == updateBrandDto.Id);
            if (brand is null)
                return new UpdateBrandStatusDto(
                    isUpdated: false,
                    Id: null,
                    StatusMessage: "Brand Couldn't be found"
                    );

            brand.Name = updateBrandDto.Name;
            brand.CountryOfOrigin = updateBrandDto.CountryOfOrigin;
            brand.Image = Convert.FromBase64String(updateBrandDto.Image);

            _unitOfWork.Brands.Update(brand);

            try
            {
                _unitOfWork.Save();
                return new UpdateBrandStatusDto(
                    isUpdated: true,
                    Id: brand.Id,
                    StatusMessage: "Brand was updated successfully"
                    );
            }
            catch
            {
                return new UpdateBrandStatusDto(
                    isUpdated: false,
                    Id: null,
                    StatusMessage: "Brand Couldn't be updated"
                    );
            }
        }
    }
}
