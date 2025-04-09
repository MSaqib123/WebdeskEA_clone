using AutoMapper;
using Dapper;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using WebdeskEA.DataAccess.DapperFactory;
using WebdeskEA.Models.DbModel;
using WebdeskEA.Models.MappingModel;
using WebdeskEA.Domain.RepositoryDapper.IRepository;
using WebdeskEA.Models.ExternalModel;
using System.Data.Common;
using System.Formats.Asn1;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations.Schema;
using WebdeskEA.Domain.CommonMethod;
using Z.Dapper.Plus;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.Design;

namespace WebdeskEA.Domain.RepositoryDapper
{
    public class VoucherRepository : Repository<Voucher>, IVoucherRepository
    {
        private readonly IMapper _mapper;
        private readonly IVoucherDtlRepository _voucherDtlRepository;

        public VoucherRepository(
            IVoucherDtlRepository voucherDtlRepository,
            IDapperDbConnectionFactory dbConnectionFactory,
            IMapper mapper)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
            _voucherDtlRepository = voucherDtlRepository;
        }

        //------ Get -------
        #region Get
        public async Task<IEnumerable<VoucherDto>> GetAllAsync()
        {
            var procedure = "spVoucher_GetAll";
            var result = await _dbConnection.QueryAsync<Voucher>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<VoucherDto>>(result);
        }
        public async Task<VoucherDto> GetByIdAsync(int id)
        {
            var procedure = "spVoucher_GetById";
            var parameters = new { Id = id };
            var result = await _dbConnection.QueryFirstOrDefaultAsync<Voucher>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<VoucherDto>(result);
        }
        public async Task<VoucherDto> GetByIdAsync(int id, int TenantId, int CompanyId)
        {
            var procedure = "spVoucher_GetByTenantIdCompanyId";
            var parameters = new { Id = id, CompanyId = CompanyId, TenantId = TenantId };
            var result = await _dbConnection.QueryFirstOrDefaultAsync<Voucher>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<VoucherDto>(result);
        }

        public async Task<IEnumerable<VoucherDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId)
        {
            var procedure = "spVoucher_GetAllByParentCompanyAndTenantId";
            var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId };
            var entity = await _dbConnection.QueryAsync<Voucher>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<VoucherDto>>(entity);
        }
        public async Task<IEnumerable<VoucherDto>> GetAllByTenantCompanyIdByVoucherTypeAsync(int TenantId, int CompanyId, string voucherType = null)
        {
            var procedure = "spVoucher_GetAllByTenantCompanyIdByVoucherType";
            var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId, VoucherType = voucherType };
            var entity = await _dbConnection.QueryAsync<Voucher>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<VoucherDto>>(entity);
        }


        public async Task<IEnumerable<VoucherDto>> GetAllByCompanyIdOrAccountTypeAsync(int CompanyId, string AccountType = "")
        {
            var procedure = "spVoucher_GetAllByCompanyIdOrAccountType";
            var parameters = new { ParentCompanyId = CompanyId, AccountType = AccountType };
            var entity = await _dbConnection.QueryAsync<Voucher>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<VoucherDto>>(entity);
        }
        public async Task<IEnumerable<VoucherDto>> GetAllByCompanyIdOrAccountTypeIdAsync(int TenantId, int CompanyId, int VoucherTypeId)
        {
            var procedure = "spVoucher_GetAllByCompanyIdOrAccountTypeId";
            var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId, VoucherTypeId = VoucherTypeId };
            var entity = await _dbConnection.QueryAsync<Voucher>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<VoucherDto>>(entity);
        }
        #endregion

        //------ Add -------
        #region Add

        public async Task<int> AddAsync(VoucherDto Dto)
        {
            try
            {
                var procedure = "spVoucher_Insert";
                var coaEntity = _mapper.Map<Voucher>(Dto);
                var parameters = CommonDapperMethod.GenerateParameters(coaEntity);
                parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
                await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
                var id = parameters.Get<int>("@Id");
                return id;
            }
            catch (SqlException ex) when (ex.Number == 50000 || ex.Number == 50001 || ex.Number == 50002)
            {
                Console.WriteLine("Unique constraint violation: " + ex.Message);
                throw new Exception("A record with the same name, code, or account code already exists.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                throw;
            }
        }

        public async Task<int> AddTransactionAsync(VoucherDto dto)
        {
            try
            {
                //_______ Inserted SO _________
                var id = await AddAsync(dto);

                //_______ Inserted Sale SOCOA _________
                var dtlDtos = dto.VoucherDtlDtos.Distinct().ToList();
                foreach (var Dtl in dtlDtos)
                {
                    VoucherDtlDto DtlDto = new VoucherDtlDto();
                    DtlDto.VoucherId = id;
                    DtlDto.COAId = Dtl.COAId;
                    DtlDto.DbAmount = Dtl.DbAmount;
                    DtlDto.CrAmount = Dtl.CrAmount;
                    DtlDto.AccountNo = Dtl.AccountNo;
                    DtlDto.BankName = Dtl.BankName;
                    DtlDto.ChequeNo = Dtl.ChequeNo;
                    DtlDto.PaymentType = Dtl.PaymentType;
                    DtlDto.PaidInvoiceNo = Dtl.PaidInvoiceNo;
                    DtlDto.PaidInvoiceId = Dtl.PaidInvoiceId;
                    DtlDto.PaidInvoiceType = dto.PaidInvoiceType ?? "";
                    DtlDto.TenantId = dto.TenantId;
                    DtlDto.TenantCompanyId = dto.TenantCompanyId;
                    DtlDto.Remarks = Dtl.Remarks;
                    await _voucherDtlRepository.AddAsync(DtlDto);
                }
                return 1;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<int> BulkAddProcAsync(IEnumerable<VoucherDto> Dtos)
        {
            //var procedure = "spVoucher_BulkInsert";
            var companies = _mapper.Map<IEnumerable<Voucher>>(Dtos);
            using (var connection = new SqlConnection(_dbConnection.ConnectionString))
            {
                await connection.OpenAsync();
                var actionSet = connection.BulkInsert(companies);
                return 1;
            }
        }

        public async Task<int> BulkAddAsync(IEnumerable<VoucherDto> Dtos)
        {
            var procedure = "spVoucher_BulkInsert";
            var companies = _mapper.Map<IEnumerable<Voucher>>(Dtos);
            return await _dbConnection.ExecuteAsync(procedure, new { Items = companies }, commandType: CommandType.StoredProcedure);
        }
        #endregion

        //------ Update -------
        #region Update
        public async Task<int> UpdateAsync(VoucherDto Dto)
        {
            try
            {
                var procedure = "spVoucher_Update";
                var coaEntity = _mapper.Map<Voucher>(Dto);
                var parameters = CommonDapperMethod.GenerateParameters(coaEntity);
                return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw;
            }

            //var id = parameters.Get<int>("@VoucherId");
        }


        public async Task<int> UpdateTransactionAsync(VoucherDto dto)
        {
            try
            {
                //_______ Inserted SO _________
                await UpdateAsync(dto);

                //_______ Inserted Sale SOCOA _________
                var dtlDtos = dto.VoucherDtlDtos.Distinct().ToList();

                //_______ Delete Sale SOCOA _________
                await _voucherDtlRepository.DeleteByVoucherIdAsync(dto.Id);

                foreach (var Dtl in dtlDtos)
                {
                    VoucherDtlDto DtlDto = new VoucherDtlDto();
                    DtlDto.VoucherId = dto.Id;
                    DtlDto.COAId = Dtl.COAId;
                    DtlDto.DbAmount = Dtl.DbAmount;
                    DtlDto.CrAmount = Dtl.CrAmount;
                    DtlDto.AccountNo = Dtl.AccountNo;
                    DtlDto.BankName = Dtl.BankName;
                    DtlDto.ChequeNo = Dtl.ChequeNo;
                    DtlDto.PaymentType = Dtl.PaymentType;
                    DtlDto.PaymentType = Dtl.PaymentType;
                    DtlDto.PaidInvoiceNo = Dtl.PaidInvoiceNo;
                    DtlDto.PaidInvoiceId = Dtl.PaidInvoiceId;
                    DtlDto.PaidInvoiceType = dto.PaidInvoiceType ?? "";
                    DtlDto.TenantId = dto.TenantId;
                    DtlDto.TenantCompanyId = dto.TenantCompanyId;
                    DtlDto.Remarks = Dtl.Remarks;
                    await _voucherDtlRepository.AddAsync(DtlDto);
                }
                return 1;

            }
            catch (Exception ex)
            {
                throw;
            }

        }

        #endregion

        //------ delete -------
        #region Delete
        public async Task<int> DeletesAsync(int id)
        {
            var procedure = "spVoucher_Delete";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        #endregion

        //-------------- not used --------------
        #region Not_used
        public async Task<IEnumerable<VoucherDto>> GetByNameAsync(string name)
        {
            var procedure = "";
            var companies = await _dbConnection.QueryAsync<Voucher>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<VoucherDto>>(companies);
        }

        #endregion
    }
}
