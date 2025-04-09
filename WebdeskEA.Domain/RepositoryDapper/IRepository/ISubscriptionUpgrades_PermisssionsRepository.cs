using WebdeskEA.Models.MappingModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.Models.MappingSingleModel;

namespace WebdeskEA.Domain.RepositoryDapper.IRepository
{
    public interface ISubscriptionUpgrades_PermisssionsRepository
    {
        Task<IEnumerable<SubscriptionUpgrades_PermisssionsDto>> GetByUserIdAsync(string id);
    }
}
