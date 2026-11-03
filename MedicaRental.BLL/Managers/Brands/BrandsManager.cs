using MedicaRental.DAL.UnitOfWork;
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
    }
}
