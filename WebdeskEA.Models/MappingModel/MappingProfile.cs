using AutoMapper;
using WebdeskEA.Models.DbModel;
using WebdeskEA.Models.ExternalModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.Models.BaseEntites;

namespace WebdeskEA.Models.MappingModel;

// Profile for Select Operations
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        //=========== MappingSingleModel =============
        #region MyRegion
        //just  Used without Mapp Simple
        #endregion

        //=========== Mapping DTOS ============
        #region MyRegion
        CreateMap<POSConfigDto, POSConfig>().ReverseMap();
        CreateMap<CategoryDto, Category>().ReverseMap();
        CreateMap<GlobalSettingsDto, GlobalSettings>().ReverseMap();
        CreateMap<SRVATBreakdownDto, SRVATBreakdown>().ReverseMap();
        CreateMap<SRDtlTaxDto, SRDtlTax>().ReverseMap();
        CreateMap<PRDtlTaxDto, PRDtlTax>().ReverseMap();
        CreateMap<PRVATBreakdownDto, PRVATBreakdown>().ReverseMap();
        CreateMap<PODtlTaxDto, PODtlTax>().ReverseMap();
        CreateMap<PIDtlTaxDto, PIDtlTax>().ReverseMap();
        CreateMap<PIVATBreakdownDto, PIVATBreakdown>().ReverseMap();
        CreateMap<POVATBreakdownDto, POVATBreakdown>().ReverseMap();
        CreateMap<SIDtlTaxDto, SIDtlTax>().ReverseMap();
        CreateMap<SIVATBreakdownDto, SIVATBreakdown>().ReverseMap();
        CreateMap<SOVATBreakdownDto, SOVATBreakdown>().ReverseMap();
        CreateMap<SODtlTaxDto, SODtlTax>().ReverseMap();
        CreateMap<PRDtlDto, PRDtl>().ReverseMap();
        CreateMap<PRDto, PR>().ReverseMap();
        CreateMap<SRDto, SR>().ReverseMap();
        CreateMap<SRDtlDto, SRDtl>().ReverseMap();
        CreateMap<VoucherTypeDto, VoucherType>().ReverseMap();
        CreateMap<VoucherDtlDto, VoucherDtl>().ReverseMap();
        CreateMap<VoucherDto, Voucher>().ReverseMap();
        CreateMap<CoaCategoryDto, CoaCategory>().ReverseMap();
        CreateMap<StockLedgerDto, StockLedger>().ReverseMap();
        CreateMap<OSDtlDto, OSDtl>().ReverseMap();
        CreateMap<OSDto, OS>().ReverseMap();
        CreateMap<OBDtlDto, OBDtl>().ReverseMap();
        CreateMap<OBDto, OB>().ReverseMap();
        CreateMap<SIDto, SI>().ReverseMap();
        CreateMap<SIDtlDto, SIDtl>().ReverseMap();
        CreateMap<PIDtlDto, PIDtl>().ReverseMap();
        CreateMap<PIDto, PI>().ReverseMap();
        CreateMap<PODtlDto, PODtl>().ReverseMap();
        CreateMap<PODto, PO>().ReverseMap();
        CreateMap<SODtlDto, SODtl>().ReverseMap();
        CreateMap<SODto, SO>().ReverseMap();
        CreateMap<ProductCOADto, ProductCOA>().ReverseMap();
        CreateMap<TaxMasterDto, TaxMaster>().ReverseMap();
        CreateMap<BankDto, Bank>().ReverseMap();
        CreateMap<CustomerDto, Customer>().ReverseMap();
        CreateMap<SupplierDto, Supplier>().ReverseMap();
        CreateMap<FinancialYearDto, FinancialYear>().ReverseMap();
        CreateMap<PackageDto,Package>().ReverseMap();
        CreateMap<PackageTypeDto,PkgType>().ReverseMap();
        CreateMap<PackagePermissionDto, PackagePermission>().ReverseMap();
        CreateMap<TenantPermissionDto, TenantPermission>().ReverseMap();
        CreateMap<RoleDto,Role>().ReverseMap();
        CreateMap<RolePermissionDto, RolePermission>().ReverseMap();
        CreateMap<CountryDto, Country>().ReverseMap();
        CreateMap<StateProvinceDto, StateProvince>().ReverseMap();
        CreateMap<CityDto, City>().ReverseMap();
        CreateMap<TenantDto, Tenant>().ReverseMap();
        CreateMap<TenantTypeDto, TenantType>().ReverseMap();
        CreateMap<ModuleDto, Module>().ReverseMap();
        CreateMap<UserRightDto, UserRight>().ReverseMap();
        CreateMap<COATypeDto, Coatype>().ReverseMap();
        CreateMap<COADto, Coa>().ReverseMap();
        CreateMap<ProductDto, Product>().ReverseMap();
        CreateMap<BrandDto, Brand>().ReverseMap();
        CreateMap<CompanyDto, Company>().ReverseMap();
        CreateMap<CompanyBusinessCategoryDto, BusinessCategory>().ReverseMap();
        CreateMap<CompanyUserDto, CompanyUser>().ReverseMap();
        CreateMap<ErrorLogDto, ErrorLog>().ReverseMap();
        CreateMap<CompanyUserDto, CompanyUser>().ReverseMap();
        //____ Identity ____
        CreateMap<ApplicationUser, ApplicationUserDto>().ReverseMap();
        #endregion
    }

}
