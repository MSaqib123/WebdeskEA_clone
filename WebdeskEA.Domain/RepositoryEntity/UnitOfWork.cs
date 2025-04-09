using AutoMapper;
using WebdeskEA.DataAccess;
using WebdeskEA.Domain.RepositoryEntity.IRepository;
using WebdeskEA.Models.ExternalModel;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Domain.RepositoryEntity
{
    public class UnitOfWork : IUnitOfWork
    {
        private WebdeskEADBContext _db;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public IApplicationUserRepository ApplicationUser { get; private set; }
        //public ICompanyRepository Company { get; private set; }

        public UnitOfWork(WebdeskEADBContext db, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _mapper = mapper;
            _userManager = userManager;

            ApplicationUser = new ApplicationUserRepository(_db, _mapper, _userManager);
            //Company = new CompanyRepository(_db);
        }

        public void SaveChange()
        {
            _db.SaveChanges();
        }
    }
}
