using AutoMapper;
using WebdeskEA.DataAccess;
using WebdeskEA.Domain.RepositoryEntity.IRepository;
using WebdeskEA.Models.DbModel;
using WebdeskEA.Models.ExternalModel;
using WebdeskEA.Models.MappingModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebdeskEA.DataAccess.DapperFactory;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Design;

namespace WebdeskEA.Domain.RepositoryEntity
{
    public class ApplicationUserRepository : Repository<ApplicationUserDto>, IApplicationUserRepository
    {
        private readonly WebdeskEADBContext _db;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        public ApplicationUserRepository(WebdeskEADBContext db, IMapper mapper, UserManager<ApplicationUser> userManager) : base(db)
        {
            _db = db;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<IdentityResult> AddAsync(ApplicationUserDto model)
        {
            var applicationUser = _mapper.Map<ApplicationUser>(model);

            // Using UserManager to create the user
            applicationUser.Id = Guid.NewGuid().ToString(); // Ensure ApplicationUser.Id is of type string
            var result = await _userManager.CreateAsync(applicationUser, model.Password);

            return result;
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationUserDto model)
        {
            var applicationUser = await _userManager.FindByIdAsync(model.Id.ToString());
            if (applicationUser == null)
            {
                throw new Exception("User not found");
            }

            // Update the properties
            _mapper.Map(model, applicationUser);

            // Using UserManager to update the user
            var result = await _userManager.UpdateAsync(applicationUser);

            return result;
        }

        public async Task<IdentityResult> DeleteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                throw new Exception("User not found");
            }

            // Using UserManager to delete the user
            var result = await _userManager.DeleteAsync(user);

            return result;
        }

        public async Task<ApplicationUserDto> GetByIdAsync(string id)
        {
            var user = await _db.ApplicationUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);

            return _mapper.Map<ApplicationUserDto>(user);
        }
        public async Task<IEnumerable<ApplicationUserDto>> GetAllUsersAsync()
        {
            return await _db.ApplicationUsers
                .AsNoTracking()
                .Select(user => _mapper.Map<ApplicationUserDto>(user))
                .ToListAsync();
        }
        public async Task<IEnumerable<ApplicationUserDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId)
        {
            var users = await _db.ApplicationUsers
                .AsNoTracking()
                .Select(user => _mapper.Map<ApplicationUserDto>(user))
                .ToListAsync();
            return users.Where(x => x.CompanyId == CompanyId && x.TenantId == TenantId);
        }
    }
}
