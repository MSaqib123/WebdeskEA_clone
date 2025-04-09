using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using WebdeskEA.Domain.RepositoryDapper;
using WebdeskEA.Models.MappingModel;
using WebdeskEA.Models.MappingSingleModel;

namespace WebdeskEA.ViewModels
{
    public class RegistrationViewModel
    {
        public RegistrationViewModel()
        {
            TenantDto = new TenantDto();
            ApplicationUserDto = new ApplicationUserDto();
            CompanyDto = new CompanyDto();

            SubscriptionUpgrades_PermisssionsDto = new List<SubscriptionUpgrades_PermisssionsDto>();
            CountryDtoList = new List<CountryDto>(); ;
            StateProvinceDtoList = new List<StateProvinceDto>(); ;
            CityDtoList = new List<CityDto>();
        }


        #region View_Models
        public TenantDto TenantDto { get; set; }
        public ApplicationUserDto ApplicationUserDto { get; set; }
        public CompanyDto CompanyDto { get; set; }
        #endregion

        #region List_Record
        [ValidateNever]
        public IEnumerable<SubscriptionUpgrades_PermisssionsDto> SubscriptionUpgrades_PermisssionsDto { get; set; }
        [ValidateNever]
        public IEnumerable<CountryDto> CountryDtoList { get; set; }
        [ValidateNever]
        public IEnumerable<StateProvinceDto> StateProvinceDtoList { get; set; }
        [ValidateNever]
        public IEnumerable<CityDto> CityDtoList { get; set; }
        #endregion

    }




}
