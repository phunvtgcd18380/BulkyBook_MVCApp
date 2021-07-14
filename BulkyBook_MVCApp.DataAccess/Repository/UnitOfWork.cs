﻿using BulkyBook_MVCApp.DataAccess.Data;
using BulkyBook_MVCApp.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook_MVCApp.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Category = new CategoryRepository(_db);
            SP_Call = new SP_Call(_db);
            coverType = new CoverTypeRepository(_db);
            Product = new ProductRepository(_db);
        }
        public ICategoryRepository Category { get; private set; }
        public ISP_Call SP_Call { get; private set; }

        public ICoverTypeRepository coverType { get; private set; }
        public IProductRepository Product { get; private set; }

        public void Dispose()
        {
            _db.Dispose();
        }
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}