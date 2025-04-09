using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.Models.ViewModel;

namespace WebdeskEA.Domain.SecurityService
{
    public interface ILoginService
    {
        Task<IActionResult> LoginAsync(LoginInputViewModel input, string returnUrl);
        //Task<IActionResult> SignInSuperAdminAsync(string returnUrl);
        //bool IsSuperAdmin(string email, string password);
    }

}
