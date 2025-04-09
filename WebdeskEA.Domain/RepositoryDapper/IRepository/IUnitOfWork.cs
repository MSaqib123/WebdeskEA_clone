using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Domain.RepositoryDapper.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        //IProductRepository Products { get; }
        //ICompanyRepository Companies { get; }
        //ICompanyUserRepository CompanyUsers { get; }
        //ICOARepository Coas { get; }
        //IModuleRepository Modules { get; }
        //IUserRightsRepository UserRights { get; }
        Task<int> CompleteAsync();
    }
}
