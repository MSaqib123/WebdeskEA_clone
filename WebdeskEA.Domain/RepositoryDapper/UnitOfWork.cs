using AutoMapper;
using WebdeskEA.DataAccess.DapperFactory;
using WebdeskEA.Domain.RepositoryDapper.IRepository;
using WebdeskEA.Domain.RepositoryEntity.IRepository;
using WebdeskEA.Models.MappingModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Domain.RepositoryDapper
{
    public class UnitOfWork : RepositoryDapper.IRepository.IUnitOfWork
    {
        private readonly IDapperDbConnectionFactory _dbConnectionFactory;
        private readonly IApplicationUserRepository _ApplicationUserRepository;
        private IDbConnection _dbConnection;
        private IDbTransaction _dbTransaction;
        private IProductRepository _productRepository;
        private ICompanyRepository _companyRepository;
        private ICompanyUserRepository _companyUserRepository;
        private ICOARepository _CoaRepository;
        private IModuleRepository _ModuleRepository;
        private IUserRightsRepository _UserRightsRepository;
        private readonly IMapper _mapper;

        public UnitOfWork(IDapperDbConnectionFactory dbConnectionFactory, IMapper mapper)
        {
            _dbConnectionFactory = dbConnectionFactory;
            _mapper = mapper;
        }

        //public IProductRepository Products
        //    => _productRepository ??= new ProductRepository(_dbConnectionFactory, _mapper);
        public ICompanyRepository Companies
            => _companyRepository ??= new CompanyRepository(_dbConnectionFactory, _mapper);

        public ICompanyUserRepository CompanyUsers
            => _companyUserRepository ??= new CompanyUserRepository(_dbConnectionFactory, _mapper,_ApplicationUserRepository);
        
        public ICOARepository Coas
            => _CoaRepository ??= new COARepository(_dbConnectionFactory, _mapper);

        public IModuleRepository Modules
            => _ModuleRepository ??= new ModuleRepository(_dbConnectionFactory, _mapper);

        public IUserRightsRepository UserRights
            => _UserRightsRepository ??= new UserRightsRepository(_dbConnectionFactory, _mapper);

        public async Task<int> CompleteAsync()
        {
            try
            {
                return await Task.FromResult(1); // Placeholder for actual transaction commit.
            }
            catch (Exception)
            {
                // Handle exceptions and rollback if needed.
                throw;
            }
        }

        public void Dispose()
        {
            _dbTransaction?.Dispose();
            _dbConnection?.Dispose();
        }
    }
}
