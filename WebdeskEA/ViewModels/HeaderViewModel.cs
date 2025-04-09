using WebdeskEA.Models.MappingModel;

namespace WebdeskEA.ViewModels
{
    public class HeaderViewModel
    {
        public HeaderViewModel()
        {
            moduleDto = new ModuleDto();
            CompanyDtos = new List<CompanyDto>();
            FinancialYearDtos = new List<FinancialYearDto>();
        }
        #region Properties
        public string ProfileImageUrl { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        #endregion

        #region List and Modules
        public ModuleDto moduleDto { get; set; }
        public IEnumerable<CompanyDto> CompanyDtos { get; set; }
        public IEnumerable<FinancialYearDto> FinancialYearDtos { get; set; }
        #endregion
    }
}
