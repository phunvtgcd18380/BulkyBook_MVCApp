using BulkyBook_MVCApp.DataAccess.Data;
using BulkyBook_MVCApp.DataAccess.Repository.IRepository;
using BulkyBook_MVCApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook_MVCApp.DataAccess.Repository
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private readonly ApplicationDbContext _db;
        public CompanyRepository(ApplicationDbContext db) :base(db)
        {
            _db = db;
        }
        public void Update(Company company)
        {
            var objFromDb = _db.Companies.FirstOrDefault(o => o.Id == company.Id);
            if(objFromDb != null)
            {
                objFromDb.Name = company.Name;
                objFromDb.StreetAddress = company.StreetAddress;
                objFromDb.Province = company.Province;
                objFromDb.PhoneNumber = company.PhoneNumber;
                objFromDb.City = company.City;
                objFromDb.PostalCode = company.PostalCode;
                objFromDb.IsAuthorizedCompany = company.IsAuthorizedCompany;
            }
        }
    }
}
