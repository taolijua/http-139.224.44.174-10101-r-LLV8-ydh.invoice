using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using YDH.Finace.Data.Interfaces;
using YDH.Finace.Data.Repositories;
using YDH.Finace.Entities.Base;
using YDH.Finace.Entities.Management;

namespace YDH.Finace.DBContext.Repositories.Management
{
    public class InvoiceRepository : Repository, IInvoiceRepository
    {
        private readonly ILogger<InvoiceRepository> _logger = null;
        private readonly IMySqlContext _mySql = null;
        // 获取依赖注入
        private readonly IServiceScope scope = null;
        public InvoiceRepository(IServiceProvider serviceProvider, ILogger<InvoiceRepository> logger, IMySqlContext mySql)
        {
            scope = serviceProvider.CreateScope();
            _logger = logger;
            _mySql = mySql;
        }

        /// <summary>
        /// 获取报销人员列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<ResponseRep<SysReimbursementApplicant>> QueryBaoXiaoNameListAsync(PageRequestDto<SysReimbursementApplicant> entity)
        {
            ResponseRep<SysReimbursementApplicant> pd = new ResponseRep<SysReimbursementApplicant>();

            string where = " 1=1 ";
            var @params = new List<MySqlParameter>();
            //按姓名查询
            if (!string.IsNullOrWhiteSpace(entity.Dto?.BXName))
            {
                where += " and name =   @name ";
                @params.Add(new MySqlParameter("name", entity.Dto?.BXName));
            }


            var sql = $"select count(1) from cw_reimbursement_applicant where {where} ;";

            pd.Count = await _mySql.ExecuteScalarAsync<int>(sql, @params.ToArray() ?? null);
            if (pd.Count < 1) return null;

            sql = "   SELECT  " +
                "  id " +
                " ,name" +
               "   FROM cw_reimbursement_applicant   ";

            if (entity.Page > 0)
            {
                sql += $"   limit {(entity.Page < 1 ? 0 : entity.Page - 1) * entity.limit},{entity.limit};";
            }

            pd.Data = await _mySql.ListAsync<SysReimbursementApplicant>(sql, @params.ToArray() ?? null);

            return pd;
        }


        /// <summary>
        /// 获取报销公司信息（公司名称及纳税人识别号）
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<ResponseRep<SysCompany>> QueryCompanyListAsync(PageRequestDto<SysCompany> entity)
        {
            ResponseRep<SysCompany> pd = new ResponseRep<SysCompany>();

            string where = " 1=1 ";
            var @params = new List<MySqlParameter>();
            //按名称查询
            if (!string.IsNullOrWhiteSpace(entity.Dto?.CompanyName))
            {
                where += " and company_name =   @CompanyName ";
                @params.Add(new MySqlParameter("CompanyName", entity.Dto?.CompanyName));
            }
            //纳税人识别号姓名查询
            if (!string.IsNullOrWhiteSpace(entity.Dto?.IdentificationNumber))
            {
                where += " and identification_number =   @IdentificationNumber ";
                @params.Add(new MySqlParameter("IdentificationNumber", entity.Dto?.IdentificationNumber));
            }

            var sql = $"select count(1) from cw_company_info where {where} ;";

            pd.Count = await _mySql.ExecuteScalarAsync<int>(sql, @params.ToArray() ?? null);
            if (pd.Count < 1) return null;

            sql = $"  select * from cw_company_info where {where} ";

            if (entity.Page > 0)
            {
                sql += $"   limit {(entity.Page < 1 ? 0 : entity.Page - 1) * entity.limit},{entity.limit};";
            }

            pd.Data = await _mySql.ListAsync<SysCompany>(sql, @params.ToArray() ?? null);

            return pd;
        }

        /// <summary>
        /// 查询发票列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<ResponseRep<SysInvoice>> QueryInvoiceListAsync(PageRequestDto<SysInvoice> entity)
        {
            ResponseRep<SysInvoice> pd = new ResponseRep<SysInvoice>();

            string where = " 1=1 ";
            var @params = new List<MySqlParameter>();
            #region 查询条件
            //按发票代码模糊查询
            if (!string.IsNullOrWhiteSpace(entity.Dto?.InvoiceCode))
            {
                where += " and cd.invoice_code like   @InvoiceCode ";
                @params.Add(new MySqlParameter("InvoiceCode", entity.Dto?.InvoiceCode + "%"));
            }
            //按发票号码模糊查询
            if (!string.IsNullOrWhiteSpace(entity.Dto?.InvoiceNumber))
            {
                where += " and cd.invoice_number like   @InvoiceNumber ";
                @params.Add(new MySqlParameter("InvoiceNumber", entity.Dto?.InvoiceNumber + "%"));
            }
            //按开票日期查询
            if (!string.IsNullOrWhiteSpace(entity.Dto?.InvoiceDateStar.ToString()))
            {
                where += " and cd.invoice_date >=   @InvoiceDateStar ";
                @params.Add(new MySqlParameter("InvoiceDateStar", entity.Dto?.InvoiceDateStar ));
            }
            if (!string.IsNullOrWhiteSpace(entity.Dto?.InvoiceDateEnd.ToString()))
            {
                where += " and cd.invoice_date <=  @InvoiceDateEnd ";
                @params.Add(new MySqlParameter("InvoiceDateEnd", entity.Dto?.InvoiceDateEnd ));
            }
            //按录入日期查询
            if (!string.IsNullOrWhiteSpace(entity.Dto?.InputTime.ToString()))
            {
                where += " and cd.input_time >=   @InputTimeStar ";
                @params.Add(new MySqlParameter("InputTimeStar", entity.Dto?.InputTimeStar));
            }
            if (!string.IsNullOrWhiteSpace(entity.Dto?.InputTimeEnd.ToString()))
            {
                where += " and cd.input_time <=   @InputTimeEnd ";
                @params.Add(new MySqlParameter("InputTimeEnd", entity.Dto?.InputTimeEnd));
            }
            //按报销人员ID查询
            if (!string.IsNullOrWhiteSpace(entity.Dto?.BXId.ToString()))
            {
                where += " and cd.bx_id =   @BXId ";
                @params.Add(new MySqlParameter("BXId", entity.Dto?.BXId));
            }
            //按录入人员ID查询
            if (!string.IsNullOrWhiteSpace(entity.Dto?.InputId.ToString()))
            {
                where += " and cd.input_id =   @InputId ";
                @params.Add(new MySqlParameter("InputId", entity.Dto?.InputId));
            }
            #endregion


            var sql = $"select count(*) from cw_claim_detail ;";
            pd.Count = await _mySql.ExecuteScalarAsync<int>(sql, @params.ToArray() ?? null);
            if (pd.Count < 1) return null;

            sql = "   SELECT  " +
               "  cd.id  " +
               "  ,cd.invoice_code " +
               "  ,cd.invoice_number  " +
               "  ,cd.invoice_date   " +
               "  ,cd.check_code_six  " +
               "  ,cd.money  " +
               "  ,cd.input_time   " +
               "  ,cd.note   " +
               "  ,cd.check_result  " +
               "  , (select nickname from cw_sys_user where cd.last_modify_by=id) as last_modify_by" +
               "  ,cd.last_modify_time" +
               "  ,su.nickname  " +
               "  ,ra.name  " +
               "  ,ci.identification_number   " +
               "  ,ci.company_name  " +
              "   FROM cw_claim_detail cd  " +
              "   left outer join cw_reimbursement_applicant ra  " +
              "   on cd.bx_id=ra.id  " +
              "   left outer join cw_sys_user su on  cd.input_id=su.id  " +
              "   left outer join cw_company_info ci on cd.company_id=ci.id  " +
              "   WHERE " +
              $"   {where}    " +
              $"   limit {(entity.Page < 1 ? 0 : entity.Page - 1) * entity.limit},{entity.limit};";


            pd.Data = await _mySql.ListAsync<SysInvoice>(sql, @params.ToArray() ?? null);

            return pd;
        }

        /// <summary>
        /// 查询录入人员列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<ResponseRep<SysUser>> QueryInputNameListAsync(PageRequestDto<SysUser> entity)
        {
            ResponseRep<SysUser> pd = new ResponseRep<SysUser>();

            string where = " 1=1 ";
            var @params = new List<MySqlParameter>();
            var sql = $"select count(*) from cw_sys_user ";
            pd.Count = await _mySql.ExecuteScalarAsync<int>(sql, @params.ToArray() ?? null);
            if (pd.Count < 1) return null;

            sql = "  select   " +
                "    id       " +
                "    ,nickname   " +
                "    from   cw_sys_user ";
            pd.Data = await _mySql.ListAsync<SysUser>(sql, @params.ToArray() ?? null);
            return pd;
        }

        /// <summary>
        /// 添加发票前查询该发票是否已报销
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<ReturnDataDto<SysInvoice>> QueryIsBaoXiaoAsync(SysInvoice entity)
        {
            ReturnDataDto<SysInvoice> rt = new ReturnDataDto<SysInvoice>();
            string sql = " select count(1)  from  cw_claim_detail " +
                         "  where invoice_code = @InvoiceCode  and invoice_number = @InvoiceNumber";
            var @params = new MySqlParameter[]
              {
                new MySqlParameter("InvoiceCode",entity.InvoiceCode),
                new MySqlParameter("InvoiceNumber", entity.InvoiceNumber)
              };
            int count= await _mySql.ExecuteScalarAsync<int>(sql, @params.ToArray() ?? null);
            if (count < 1)
            {
                rt.Code = 1;
                rt.Message = "该发票之前未报销过。";
            }
            else {
                rt.Code = 2;
                rt.Message = "该发票已报销。";

                //SysInvoice Dto = new SysInvoice();
                //Dto.InvoiceCode = entity.Dto.InvoiceCode;
                // Dto.InvoiceNumber = entity.Dto.InvoiceNumber;

                ReturnDataDto<SysInvoice> Invoice = await QueryInvoiceInfoAsync(new SysInvoice { InvoiceCode = entity.InvoiceCode,InvoiceNumber = entity.InvoiceNumber }) ;
                rt.Dto = Invoice.Dto;
            }
            return rt;
        }

        /// <summary>
        /// 根据发票代码和发票号码查询已报销的发票信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async  Task<ReturnDataDto<SysInvoice>> QueryInvoiceInfoAsync(SysInvoice dto)
        {
            ReturnDataDto<SysInvoice> rt = new ReturnDataDto<SysInvoice>();
            string sql = "   SELECT  " +
               "  cd.id  " +
               "  ,cd.invoice_code " +
               "  ,cd.invoice_number  " +
               "  ,cd.invoice_date   " +
               "  ,cd.check_code_six  " +
               "  ,cd.money  " +
               "  ,cd.input_time   " +
               "  ,cd.note   " +
               "  ,su.nickname  " +
               "  ,ra.name  " +
               "  ,cd.identification_number   " +
               "  ,cd.company_name  " +
              "   FROM cw_claim_detail cd  " +
              "   left outer join cw_reimbursement_applicant ra  " +
              "   on cd.bx_id=ra.id  " +
              "   left outer join cw_sys_user su on  cd.input_id=su.id" +
              "   WHERE " +
              "   invoice_code = @InvoiceCode  and invoice_number = @InvoiceNumber ";

            var @params = new MySqlParameter[]
             {
                new MySqlParameter("InvoiceCode",dto.InvoiceCode),
                new MySqlParameter("InvoiceNumber", dto.InvoiceNumber)
             };

            rt.Dto = await _mySql.EntityAsync<SysInvoice>(sql, @params.ToArray() ?? null);

            return rt;
        }

        /// <summary>
        /// 保存发票信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async  Task<ReturnDataDto<SysInvoice>> SaveInvoiceInfoAsync(SysInvoice dto)
        {
            ReturnDataDto<SysInvoice> rt = new ReturnDataDto<SysInvoice>();
            //先将报销人员ID查询出来
           // string sql = " select * from cw_reimbursement_applicant where name =@BXName ";
            //var @params = new MySqlParameter[]
            //{
            //    new MySqlParameter("BXName",dto.BXName)
            //};
            //var bx_id = await _mySql.ExecuteScalarAsync(sql, @params.ToArray() ?? null);
            ////如果报销人员表里不存在则加入该报销用户
            //if (bx_id == null)
            //{
            //     sql = " insert into cw_reimbursement_applicant (name) VALUES( @BXName )";
            //     await _mySql.ExecuteNonQueryAsync(sql, @params.ToArray() ?? null);
            //    sql = " select * from cw_reimbursement_applicant where name =@BXName ";
            //    bx_id = await _mySql.ExecuteScalarAsync(sql, @params.ToArray() ?? null);
            //}

            string sql = "  insert into  cw_claim_detail  " +
                  "   (bx_id , " +
                  "    company_id, " +
                  "    invoice_code, " +
                  "    invoice_number,  " +
                  "    invoice_date, " +
                  "    check_code," +
                  "    check_code_six," +
                  "    money," +
                  "    input_time," +
                  "    input_id," +
                  "    note) " +
                  "    values (  " +
                  "    @bxid ,  " +
                  "    @CompanyId ," +
                  "    @InvoiceCode,  " +
                  "    @InvoiceNumber,  " +
                  "    @InvoiceDate,  " +
                  "    @CheckCode,  " +
                  "    @CheckCodeSix,  " +
                  "    @Money ,  " +
                  "    @InputTime,  " +
                  "    @InputId,  " +
                  "    @Note )  ";
            var context = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();

            long userid = context.HttpContext.User.GetAuthUserId();

            string CheckCodeSix = "";
            if (!string.IsNullOrWhiteSpace(dto.CheckCode))
            {
               CheckCodeSix = dto.CheckCode.Substring(dto.CheckCode.Length - 6); 
            }


            var @params = new MySqlParameter[]
             {
                new MySqlParameter("bxid",dto.BXId),
                new MySqlParameter("CompanyId",dto.CompanyId),
                new MySqlParameter("InvoiceCode",dto.InvoiceCode),
                new MySqlParameter("InvoiceNumber",dto.InvoiceNumber),
                new MySqlParameter("InvoiceDate",dto.InvoiceDate),
                new MySqlParameter("CheckCode",dto.CheckCode),
                new MySqlParameter("CheckCodeSix", CheckCodeSix ),
                new MySqlParameter("Money",dto.Money),
                new MySqlParameter("InputTime",DateTime.Now.ToString("yyyy-MM-dd")),
                new MySqlParameter("InputId",userid),
                new MySqlParameter("Note",dto.Note),
             };

            await _mySql.ExecuteScalarAsync(sql, @params.ToArray() ?? null);
            rt.Code = 1;
            rt.Message = "添加发票信息成功";
            return rt;
        }


        /// <summary>
        /// 导入发票校验结果时判断发票是否存在,返回不存在的发票列表
        /// </summary>
        /// <param name="zubaos"></param>
        /// <returns></returns>
         public async Task<IList<SysInvoice>> ISExistsInvoiceAsync(IList<SysInvoice> invoice)
        {
            var @params = new List<MySqlParameter>();

            IList<SysInvoice> exinvoices = new List<SysInvoice>();
            IList<SysInvoice> errorinvoices = new List<SysInvoice>();
            //判断发票是否存在
            var sql = "select * from cw_claim_detail where invoice_code in(" +
                "{0}" +
                ")  " +
                "   and   invoice_number in (" +
                "{1} " +
                " )";

            var InvoiceCodes = string.Join(",", invoice.Select(i =>
               $"'{i.InvoiceCode}'"
               ));

            var InvoiceNumbers = string.Join(",", invoice.Select(i =>
         $"'{i.InvoiceNumber}'"
         ));

            exinvoices = await _mySql.ListAsync<SysInvoice>(string.Format(sql, InvoiceCodes, InvoiceNumbers), @params.ToArray() ?? null);

            for (int i = invoice.Count - 1; i >= 0; i--)
            {
               var invoices = exinvoices.Select(a =>new { a.InvoiceCode,a.InvoiceNumber}).ToList();
                if (!invoices.Exists(a=>a.InvoiceCode== invoice[i].InvoiceCode && a.InvoiceNumber == invoice[i].InvoiceNumber))
                {
                    invoice[i].status = "失败，该发票不存在系统";
                    errorinvoices.Add(invoice[i]);
                }
            }
            return errorinvoices;
        }

        /// <summary>
        /// 通过扫描出来的纳税人识别号，找到报销公司ID
        /// </summary>
        /// <param name="identification_number"></param>
        /// <returns></returns>
        public async Task<ReturnDataDto<SysCompany>> QueryConpanyByNumbersync(string identification_number)
        {
            ReturnDataDto<SysCompany> rt = new ReturnDataDto<SysCompany>();
            string sql = " select * from cw_company_info where identification_number =@IdentificationNnumber ";
            var @params = new MySqlParameter[]
            {
                new MySqlParameter("IdentificationNnumber",identification_number)
            };
            rt.Dto = await _mySql.EntityAsync<SysCompany>(sql, @params.ToArray() ?? null);

            return rt;
        }

        /// <summary>
        ///批量更新校验结果
        /// </summary>
        /// <returns></returns>
        public async Task<int> UploadCkeckInfoAsync(IList<SysInvoice> invoice)
        {
            int affect = 0;
            var context = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();

            long userid = context.HttpContext.User.GetAuthUserId();
            foreach (var item in invoice)
            {
            
                var sql = " update cw_claim_detail  set " +
                    "  check_result =@check_result ,last_modify_by = @last_modify_by ,last_modify_time = @last_modify_time " +
                    "  where invoice_code =@invoice_code  " +
                    "  and invoice_number=@invoice_number ";

                var @params = new MySqlParameter[]
                {
                new MySqlParameter("invoice_code",item.InvoiceCode),
                new MySqlParameter("invoice_number", item.InvoiceNumber),
                new MySqlParameter("check_result", item.CheckResult),
                new MySqlParameter("last_modify_by", userid),
                new MySqlParameter("last_modify_time", DateTime.Now)
                };
                affect = await _mySql.ExecuteScalarAsync<int>(sql, @params.ToArray() ?? null);
            }
       
            return affect;
        }


        /// <summary>
        /// 添加报销用户
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> AddBXNameInfoAsync(SysReimbursementApplicant entity)
        {
            int affect = 0;
            string sql = "  insert into cw_reimbursement_applicant (" +
                "  name ) values ( " +
                " @name " +
                " )";

            var @params = new MySqlParameter[]
         {
                new MySqlParameter("name",entity.BXName)
         };

            await _mySql.ExecuteScalarAsync(sql, @params.ToArray() ?? null);

            return affect;
        }
        /// <summary>
        /// 添加报销信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async  Task<int> AddCompanyInfoAsync(SysCompany entity)
        {
            int affect = 0;

            string sql = "  insert into cw_company_info (" +
               "  company_name ,identification_number  ) values ( " +
               " @CompanyName , @IdentificationNumber " +
               " )";

            var @params = new MySqlParameter[]
         {
                new MySqlParameter("CompanyName",entity.CompanyName),
                new MySqlParameter("IdentificationNumber",entity.IdentificationNumber)
         };

            await _mySql.ExecuteScalarAsync(sql, @params.ToArray() ?? null);

            return  affect;
        }

    }

    public interface IInvoiceRepository : IRepository
    {
        /// <summary>
        /// 查询报销人员
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<ResponseRep<SysReimbursementApplicant>> QueryBaoXiaoNameListAsync(PageRequestDto<SysReimbursementApplicant> entity);

        /// <summary>
        /// 查询发票列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<ResponseRep<SysInvoice>> QueryInvoiceListAsync(PageRequestDto<SysInvoice> entity);

        /// <summary>
        /// 查询录入人员列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<ResponseRep<SysUser>> QueryInputNameListAsync(PageRequestDto<SysUser> entity);


        /// <summary>
        /// 获取报销公司信息（公司名称及纳税人识别号）
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<ResponseRep<SysCompany>> QueryCompanyListAsync(PageRequestDto<SysCompany> entity);

        /// <summary>
        /// 添加发票前查询该发票是否已报销
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<ReturnDataDto<SysInvoice>> QueryIsBaoXiaoAsync(SysInvoice entity);

        /// <summary>
        /// 根据发票代码和发票号码查询已报销的发票信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<ReturnDataDto<SysInvoice>> QueryInvoiceInfoAsync(SysInvoice dto);

        /// <summary>
        /// 保存发票信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<ReturnDataDto<SysInvoice>> SaveInvoiceInfoAsync(SysInvoice dto);

      
        /// <summary>
        /// 通过扫描出来的纳税人识别号，找到报销公司ID
        /// </summary>
        /// <param name="identification_number"></param>
        /// <returns></returns>
        Task<ReturnDataDto<SysCompany>> QueryConpanyByNumbersync(string identification_number);

        /// <summary>
        /// 判断发票是否存在
        /// </summary>
        /// <param name="zubaos"></param>
        /// <returns></returns>
        Task<IList<SysInvoice>> ISExistsInvoiceAsync(IList<SysInvoice> invoice);

        /// <summary>
        ///批量更新校验结果
        /// </summary>
        /// <returns></returns>
        Task<int> UploadCkeckInfoAsync(IList<SysInvoice> invoice);

        /// <summary>
        /// 添加报销用户
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> AddBXNameInfoAsync(SysReimbursementApplicant entity);
        /// <summary>
        /// 添加报销信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
         Task<int> AddCompanyInfoAsync(SysCompany entity);

    }
}
