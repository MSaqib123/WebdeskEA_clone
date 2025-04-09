using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebdeskEA.Domain.RepositoryDapper.IRepository;
using WebdeskEA.Domain.RepositoryEntity.IRepository;
using WebdeskEA.Domain.Service;
using WebdeskEA.Models.MappingModel;
using WebdeskEA.Models.Utility;

namespace CRM.Areas.CompanySites.Controllers
{
    [Area("CompanySites")]
    //[Authorize(Roles = "SuperAdmin,Admin,AuthUser")]
    public class CompanyUserController : Controller
    {
        //=================== Constructor =====================
        #region Constructor
        private readonly ICompanyUserRepository _companyUserRepository;
        private readonly ICompanyRepository _CompanyRepository;
        private readonly IErrorLogRepository _errorLogRepository;

        public CompanyUserController(ICompanyUserRepository companyUserRepository,
            IErrorLogRepository errorLogRepository,
            ICompanyRepository companyRepository)
        {
            _companyUserRepository = companyUserRepository;
            _errorLogRepository = errorLogRepository ?? throw new ArgumentNullException(nameof(errorLogRepository));
            _CompanyRepository = companyRepository;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewBag.NameOfForm = "CompanyUser";
            base.OnActionExecuting(context);
        }
        #endregion

        //=================== Model_Binding =====================
        #region ModelBinding
        //=================== Create =====================
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var companies = await _CompanyRepository.GetAllAsync();
                return View(companies);
            }
            catch (Exception ex)
            {
                // Log the error
                await LogError(ex, "Index");
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }

        //=================== Detail =====================
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var companyUsers = await _companyUserRepository.GetAllCompanyUserByCompanyIdAsync(id);
                if (companyUsers == null || !companyUsers.Any())
                {
                    return NotFound();
                }

                var companyDetail = companyUsers.FirstOrDefault();

                ViewBag.CompanyId = companyDetail?.CompanyId;
                ViewBag.CompanyName = companyDetail?.CompanyName;

                return View(companyUsers);
            }
            catch (Exception ex)
            {
                await LogError(ex, "DetailGet");
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }

        //=================== Create =====================
        [HttpGet]
        public async Task<IActionResult> Create(int id)
        {
            try
            {
                CompanyUserDto dto = new CompanyUserDto();
                CompanyDto company = await _CompanyRepository.GetByIdAsync(id);
                dto.CompanyId = company.Id;
                dto.CompanyName = company.Name;
                dto.UserList = await _companyUserRepository.GetAllUsersNEICUAsync();
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex, "CreatePost");
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CompanyUserDto companyUserDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var companyUsers = companyUserDto.UserIds.Select(userId => new CompanyUserDto
                    {
                        CompanyId = companyUserDto.CompanyId,
                        UserId = userId
                    }).ToList();

                    await _companyUserRepository.BulkAddCompanyUserAsync(companyUsers);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    companyUserDto.CompanyList = await _CompanyRepository.GetAllAsync();
                    companyUserDto.UserList = await _companyUserRepository.GetAllUsersNEICUAsync();
                    return View(companyUserDto);
                }
            }
            catch (Exception ex)
            {
                await LogError(ex, "CreatePost");
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }


        //==================== Edit ======================
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var companyUser = await _companyUserRepository.GetCompanyUserWithUpdateBaseAsync(id);
                var company = await _CompanyRepository.GetByIdAsync(id);
                companyUser.CompanyName = company?.Name ?? "Company Not Found";
                companyUser.CompanyId = id;
                if (company == null)
                {
                    return NotFound();
                }
                return View(companyUser);
            }
            catch (Exception ex)
            {
                await LogError(ex, "EditGet");
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CompanyUserDto companyUserDto)
        {
            try
            {
                if (id != companyUserDto.CompanyId)
                {
                    return BadRequest("Company ID mismatch");
                }

                var companyUsers = companyUserDto.UserIds.Select(userId => new CompanyUserDto
                {
                    CompanyId = companyUserDto.CompanyId,
                    UserId = userId
                }).ToList();

                await _companyUserRepository.BulkUpdateCompanyUserAsync(id, companyUsers);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await LogError(ex, "EditPost");
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }


        //==================== Delete ======================
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var companyUsers = await _companyUserRepository.GetAllCompanyUserByCompanyIdAsync(id);
                if (companyUsers == null || !companyUsers.Any())
                {
                    return NotFound();
                }

                var companyDetail = companyUsers.FirstOrDefault();

                ViewBag.CompanyId = companyDetail?.CompanyId;
                ViewBag.CompanyName = companyDetail?.CompanyName;

                return View(companyUsers);
            }
            catch (Exception ex)
            {
                await LogError(ex, "DetailGet");
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _companyUserRepository.DeleteCompanyUserAsync(id);
            if (result == 0)
            {
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = "Empty Users Can not be Deleted" });
            }
            return RedirectToAction(nameof(Index));
        }
        #endregion

        //=================== Partial Binding =====================
        #region PartialBinding

        #endregion

        //=================== Error Handling =====================
        #region ErrorHandling
        //_______ Error Log ______
        private async Task LogError(Exception ex, string action)
        {
            var (errorCode, statusCode) = ErrorUtility.GenerateErrorCodeAndStatus(ex);
            await _errorLogRepository.AddErrorLogAsync(
                area: "Users",
                controller: "UsersController",
                actionName: action,
                formName: $"{action} Form",
                errorShortDescription: ex.Message,
                errorLongDescription: ErrorUtility.GetFullExceptionMessage(ex),
                statusCode: statusCode.ToString(),
                username: User.Identity?.Name
            );
        }
        #endregion

    }
}
