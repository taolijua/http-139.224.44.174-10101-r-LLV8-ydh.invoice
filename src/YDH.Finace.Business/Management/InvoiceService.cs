using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using YDH.Finace.Common;
using YDH.Finace.Common.AllExceptions;
using YDH.Finace.Common.BusinessServices;
using YDH.Finace.DBContext.Repositories.Management;
using YDH.Finace.Entities.Base;
using YDH.Finace.Entities.Management;
using YDH.Finace.Web.FilterAttributes;

namespace YDH.Finace.Business.Management
{
    /// <summary>
    /// 
    /// </summary>
   public  class InvoiceService: BusinessService, IInvoiceService
    {
        // 日记
        private readonly ILogger<LoginService> _logger = null;
        // 获取依赖注入
        private readonly IServiceScope scope = null;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public InvoiceService(IServiceProvider serviceProvider, ILogger<LoginService> logger)
        {
            scope = serviceProvider.CreateScope();
            _logger = logger;
        }

        /// <summary>
        /// 获取报销人员列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [SkipResultFilter]
        public async Task<ResponseRep<SysReimbursementApplicant>> QueryBaoXiaoNameListAsync(PageRequestDto<SysReimbursementApplicant> entity)
        {
            //if (entity == null)
            //{
            //    throw new FriendlyException("参数不能为空！");
            //}
            //if (entity.limit < 1) entity.limit = 50;// 一页默认50行

            // 权限管理实例
            var InvoiceRepository = scope.ServiceProvider.GetRequiredService<IInvoiceRepository>();

            //
            return await InvoiceRepository.QueryBaoXiaoNameListAsync(entity);
        }

        /// <summary>
        /// 获取发票列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<ResponseRep<SysInvoice>> QueryInvoiceListAsync(PageRequestDto<SysInvoice> entity)
        {
            if (entity == null)
            {
                throw new FriendlyException("参数不能为空！");
            }
            if (entity.limit < 1) entity.limit = 50;// 一页默认50行

            // 权限管理实例
            var InvoiceRepository = scope.ServiceProvider.GetRequiredService<IInvoiceRepository>();

            //
            return await InvoiceRepository.QueryInvoiceListAsync(entity);
        }

        /// <summary>
        /// 获取录入人员列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
         public async Task<ResponseRep<SysUser>> QueryInputNameListAsync(PageRequestDto<SysUser> entity)
        {
            // 权限管理实例
            var InvoiceRepository = scope.ServiceProvider.GetRequiredService<IInvoiceRepository>();

            return await InvoiceRepository.QueryInputNameListAsync(entity);
        }


        /// <summary>
        /// 获取报销公司信息（公司名称及纳税人识别号）
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<ResponseRep<SysCompany>> QueryCompanyListAsync(PageRequestDto<SysCompany> entity)
        {
            // 权限管理实例
            var InvoiceRepository = scope.ServiceProvider.GetRequiredService<IInvoiceRepository>();

            return await InvoiceRepository.QueryCompanyListAsync(entity);
        }


        /// <summary>
        /// 手动添加发票信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<ReturnDataDto<SysInvoice>> SaveInvoiceInfoAsync(SysInvoice entity)
        {
            ReturnDataDto<SysInvoice> rt = new ReturnDataDto<SysInvoice>();


            // 权限管理实例
            var InvoiceRepository = scope.ServiceProvider.GetRequiredService<IInvoiceRepository>();
            //先判断该发票是否已报销
            rt = await InvoiceRepository.QueryIsBaoXiaoAsync(entity);
            if (rt.Code == 2)
            {
                return rt;
            }
            //如果不存在已报销的情况，则直接保存发票信息
            rt = await InvoiceRepository.SaveInvoiceInfoAsync(entity);

            return rt;
        }

        /// <summary>
        /// 扫描添加发票信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<ReturnDataDto<SysInvoice>> ScanSaveInvoiceInfoAsync(SysInvoice entity)
        {
            ReturnDataDto<SysInvoice> rt = new ReturnDataDto<SysInvoice>();
            //处理扫描的信息
            //传统发票以01开头，深圳电子普通发票以http开头，分开处理
            if (entity.ScanInfo.StartsWith("01"))//二维码的通用格式均以01开头
            {
                rt = AnalyzeInvoiceWithOriginal(entity.ScanInfo);
            }
            else if (entity.ScanInfo.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                rt = AnalyzeBlockchainInvoiceWithUrl(entity.ScanInfo);
            }
            else
            {
                rt.Code = -1;
                rt.Message = "不是正确的发票信息";
                return rt;
            }
            if (rt.Code == -1)
            {
                return rt;
            }
            entity.InvoiceCode = rt.Dto.InvoiceCode;
            entity.InvoiceNumber = rt.Dto.InvoiceNumber;
            entity.InvoiceDate = rt.Dto.InvoiceDate;
            entity.Money = rt.Dto.Money;
            if (!string.IsNullOrWhiteSpace(rt.Dto.IdentificationNumber))
            {
                entity.IdentificationNumber = rt.Dto.IdentificationNumber;
                //通过扫描出来的纳税人识别号，找到报销公司ID
                ReturnDataDto<SysCompany> company = new ReturnDataDto<SysCompany>();
                // 权限管理实例
                var InvoiceRepository = scope.ServiceProvider.GetRequiredService<IInvoiceRepository>();
                company =await InvoiceRepository.QueryConpanyByNumbersync(entity.IdentificationNumber);
                if (company.Dto != null && company.Dto.CompanyId != 0)
                {
                    entity.CompanyId = company.Dto.CompanyId;
                }
                else
                {
                    rt.Code = -1;
                    rt.Message = "扫描出来的纳税人识别号不存在，请先维护报销公司信息！";
                }

            }
            if (!string.IsNullOrWhiteSpace(rt.Dto.CheckCode))
            {
                entity.CheckCode = rt.Dto.CheckCode;
            }
            //处理信息后正常添加发票信息
            rt= await SaveInvoiceInfoAsync(entity);

            return rt;
        }

        /// <summary>
        /// 处理值税专票、普票、电子普通发票的二维码信息
        /// </summary>
        /// <param name="scan">扫描二维码得到的信息</param>
        /// <returns></returns>
        public ReturnDataDto<SysInvoice> AnalyzeInvoiceWithOriginal(string scan)
        {
            ReturnDataDto<SysInvoice> rt = new ReturnDataDto<SysInvoice>();
            SysInvoice invoice = new SysInvoice();

            //data = "01,10,144031909110,01034285,91440300708437873H,5.45,20190320,0053e0982be37e9763314b0c8a58b7c121e0b8b59b03bf3cae346ea8c7d2cbd7b4";
            string [] arr;
            if (scan.Contains(','))
            {
                 arr = scan.Split(',');
            }
            else 
            {
                 arr = scan.Split('，');
            }
          
          
                var valid = NumberValidators.Invoices.Validators.VATCodeValidatorHelper.Validate(arr[2]);
                if (valid.IsValid)
                {
                    //区块链电子发票
                    if (valid.Category == NumberValidators.Invoices.VATKind.Blockchain)
                    {
                    //                        Console.WriteLine(@"区块链电子普通发票二维码原始格式结果
                    //发票代码：{0}
                    //发票号码：{1}
                    //开票时间：{2}
                    //不含税金额：{3}`
                    //销售方纳税人识别号：{4}", arr[2], arr[3], arr[6], arr[5], arr[4]);
                    invoice.InvoiceCode = arr[2];//发票代码
                    invoice.InvoiceNumber = arr[3];//发票号码
                    invoice.InvoiceDate =Convert.ToDateTime(arr[6]) ;//开票时间
                    invoice.Money = arr[5];//不含税金额
                    invoice.IdentificationNumber = arr[4];//销售方纳税人识别号
                    }
                    else
                    {
                    //                        Console.WriteLine(@"增值税发票二维码原始格式结果
                    //发票代码：{0}
                    //发票号码：{1}
                    //开票时间：{2}
                    //不含税金额：{3}
                    //发票类型：{4}", arr[2], arr[3], arr[5], arr[4], valid.Category);
                    DateTime time = DateTime.ParseExact(arr[5], "yyyyMMdd", null);
                  
                    invoice.InvoiceCode = arr[2];//发票代码
                    invoice.InvoiceNumber = arr[3];//发票号码
                    invoice.InvoiceDate = time;//开票时间
                    invoice.Money = arr[4];//不含税金额
                    invoice.CheckCode = arr[6];//校验码
                    }
                }
            rt.Dto = invoice;
            return rt;
        }
        /// <summary>
        /// 处理深圳电子普通发票的Url格式
        /// </summary>
        /// <param name="scan"></param>
        /// <returns></returns>
        public ReturnDataDto<SysInvoice> AnalyzeBlockchainInvoiceWithUrl(string scan)
        {
            ReturnDataDto<SysInvoice> rt = new ReturnDataDto<SysInvoice>();
            SysInvoice invoice = new SysInvoice();
            //qrUrl = @"https://bcfp.shenzhen.chinatax.gov.cn/verify/scan?hash=007547b34bf2fa4dfadf9dd789f9b017b59bee8c563e1185232d66bc58b7c90a04&bill_num=05830164&total_amount=500";
            var qrUri = new Uri(scan);
            if (!qrUri.Host.Equals("bcfp.shenzhen.chinatax.gov.cn", StringComparison.OrdinalIgnoreCase))
            {
                rt.Code = -1;
                rt.Message = "不是深圳电子普通发票的域名";
                return rt;
            }
            var collection = HttpUtility.ParseQueryString(qrUri.Query);
            var url = $"{qrUri.Scheme}://{qrUri.Host}/dzswj/bers_ep_web/query_bill_detail";
            var restClient = new RestClient(url);
            var restRequest = new RestRequest(Method.POST);
            restRequest.AddParameter("bill_num", collection["bill_num"]);
            restRequest.AddParameter("total_amount", collection["total_amount"]);
            restRequest.AddParameter("tx_hash", collection["hash"]);
            var restResponse = restClient.Execute(restRequest);
            //Console.WriteLine("区块链电子普通发票二维码Url格式服务端响应内容：");
            //Console.WriteLine(restResponse.Content);

            var json = (JObject)JsonConvert.DeserializeObject(restResponse.Content);
            if (json["retcode"].ToString() == "0")
            {
                var record = json["bill_record"];
                var time = record.Value<long>("time");
                var dt = DateTimeOffset.FromUnixTimeSeconds(time).LocalDateTime;
                //                Console.WriteLine(@"区块链电子普通发票Url格式结果
                //发票代码：{0}
                //发票号码：{1}
                //开票时间：{2:yyyy-MM-dd HH:mm:ss}
                //不含税金额：{3}
                //销售方纳税人识别号：{4}", record["bill_code"], record["bill_num"], dt, record.Value<int>("amount") * 0.01m, record["seller_taxpayer_id"]);
                invoice.InvoiceCode = record["bill_code"].ToString();//发票代码
                invoice.InvoiceNumber = record["bill_num"].ToString();//发票号码
                invoice.InvoiceDate = dt;//开票时间
                invoice.Money =Convert.ToString( record.Value<int>("amount") * 0.01m);//不含税金额
                invoice.IdentificationNumber = record["seller_taxpayer_id"].ToString();//销售方纳税人识别号
            }
            else
            {
                rt.Code = -1;
                rt.Message = $"服务端响应错误：{json["retmsg"].ToString()}";
                return rt;
                //Console.WriteLine("服务端响应错误：{0}", json["retmsg"]);
            }
            rt.Dto = invoice;
            return rt;
        }

        /// <summary>
        /// 导入发票校验结果
        /// </summary>
        /// <param name="invoice"></param>
        /// <returns></returns>
        public async Task<ReturnDataDto<SysInvoice>> UploadCkeckInfoAsync(string invoice)
        {
            ReturnDataDto<SysInvoice> rt = new ReturnDataDto<SysInvoice>();
            //反序列化Json字符串内容为对象
            IList<SysInvoice> jsondata = JsonConvert.DeserializeObject<List<SysInvoice>>(invoice);
            // 文件管理实例
            var InvoiceRepository = scope.ServiceProvider.GetRequiredService<IInvoiceRepository>();
            //判断发票是否存在
            IList<SysInvoice> errorzubao = new List<SysInvoice>();
            errorzubao = await InvoiceRepository.ISExistsInvoiceAsync(jsondata);

            //去掉不符合组包规则的数据
            if (errorzubao != null)
            {
                foreach (SysInvoice item in errorzubao)
                {
                    jsondata.Remove(item);
                }
            }
            //批量更新校验结果
            await InvoiceRepository.UploadCkeckInfoAsync(jsondata);

            rt.Code = 200;
            rt.Message = "导入成功";
            rt.Data = errorzubao;
            return rt;
        }

        /// <summary>
        /// 发票校验真伪
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<ReturnDataDto<SysInvoice>> CheckInvoiceListAsync(SysInvoice entity)
        {
            ReturnDataDto<SysInvoice> rt = new ReturnDataDto<SysInvoice>();
            DateTime dt =Convert.ToDateTime( entity.InvoiceDate);
            string date = dt.ToString("yyyyMMdd");
            string url = "http://inv-veri.com/check?fpdm="+ entity.InvoiceCode+ "&fphm="+entity.InvoiceNumber+ "&date="+ date+ "&code="+entity.CheckCodeSix+ "&channel=yd";
            string result= HttpExtensions.HttpGet(url, "application/json");
            // 文件管理实例
            var InvoiceRepository = scope.ServiceProvider.GetRequiredService<IInvoiceRepository>();

            //处理json结果
            JObject jo = (JObject)JsonConvert.DeserializeObject(result);
            IList<SysInvoice> jsondata = new List<SysInvoice>();
            SysInvoice invoice = new SysInvoice();
            invoice.InvoiceCode = entity.InvoiceCode;
            invoice.InvoiceNumber = entity.InvoiceNumber;
            if (jo != null)
            {
                if (jo["code"].ToString() == "0")
                {


                    if (jo["info"] != null)
                    {
                        //验证成功
                        invoice.CheckResult = "校验成功";
                        jsondata.Add(invoice);

                        await InvoiceRepository.UploadCkeckInfoAsync(jsondata);
                    }
                    else
                    {
                        //验证有问题
                        invoice.CheckResult = jo["data"]["message"].ToString();
                        jsondata.Add(invoice);

                        await InvoiceRepository.UploadCkeckInfoAsync(jsondata);
                    }
                }
                else
                {
                    invoice.CheckResult = jo["message"].ToString();
                    jsondata.Add(invoice);

                    await InvoiceRepository.UploadCkeckInfoAsync(jsondata);
                }
            }
            rt.Code = 1;
            return rt;
        }

        /// <summary>
        /// 添加报销用户
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<ReturnDataDto<SysReimbursementApplicant>> AddBXNameInfoAsync(SysReimbursementApplicant entity)
        {
            ReturnDataDto<SysReimbursementApplicant> rt=new ReturnDataDto<SysReimbursementApplicant>();
            // 权限管理实例
            var InvoiceRepository = scope.ServiceProvider.GetRequiredService<IInvoiceRepository>();
            //先查询该报销人员是否存在
            PageRequestDto<SysReimbursementApplicant> bxuser = new PageRequestDto<SysReimbursementApplicant>();
            bxuser.Dto = entity;

            ResponseRep<SysReimbursementApplicant> user = new ResponseRep<SysReimbursementApplicant>();
            user = await InvoiceRepository.QueryBaoXiaoNameListAsync(bxuser);
            if (user != null && user.Count > 0)
            {
                rt.Code = -1;
                rt.Message = "该报销人员已存在";
                return rt;
            }
            //增加用户
            await InvoiceRepository.AddBXNameInfoAsync(entity);
            rt.Code = 1;
            rt.Message = "添加成功";
            return rt;

        }
        /// <summary>
        /// 添加公司信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<ReturnDataDto<SysCompany>> AddCompanyInfoAsync(SysCompany entity)
        {
            ReturnDataDto<SysCompany> rt = new ReturnDataDto<SysCompany>();
            // 权限管理实例
            var InvoiceRepository = scope.ServiceProvider.GetRequiredService<IInvoiceRepository>();

            //先查询该报销人员是否存在
            PageRequestDto<SysCompany> Company = new PageRequestDto<SysCompany>();
            Company.Dto = entity;

            ResponseRep<SysCompany> Companyresult = new ResponseRep<SysCompany>();
            Companyresult = await InvoiceRepository.QueryCompanyListAsync(Company);
            if (Companyresult != null && Companyresult.Count > 0)
            {
                rt.Code = -1;
                rt.Message = "该公司信息已存在";
                return rt;
            }
            //增加用户
            await InvoiceRepository.AddCompanyInfoAsync(entity);
            rt.Code = 1;
            rt.Message = "添加成功";
            return rt;
        }
    }
    /// <summary>
    /// 发票服务
    /// </summary>
    public interface IInvoiceService : IBusinessService
    {
        /// <summary>
        /// 获取报销人员列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<ResponseRep<SysReimbursementApplicant>> QueryBaoXiaoNameListAsync(PageRequestDto<SysReimbursementApplicant> entity);

        /// <summary>
        /// 增加报销人员信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<ReturnDataDto<SysReimbursementApplicant>> AddBXNameInfoAsync(SysReimbursementApplicant entity);

        /// <summary>
        /// 获取发票列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<ResponseRep<SysInvoice>> QueryInvoiceListAsync(PageRequestDto<SysInvoice> entity);

        /// <summary>
        /// 获取录入人员列表
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
        /// 手动添加发票信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<ReturnDataDto<SysInvoice>> SaveInvoiceInfoAsync(SysInvoice entity);

        /// <summary>
        /// 扫描添加发票信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<ReturnDataDto<SysInvoice>> ScanSaveInvoiceInfoAsync(SysInvoice entity);
        /// <summary>
        /// 处理增值税专票、普票、电子普通发票的二维码信息
        /// </summary>
        /// <param name="scan">扫描二维码得到的信息</param>
        /// <returns></returns>
        ReturnDataDto<SysInvoice> AnalyzeInvoiceWithOriginal(string  scan);
        /// <summary>
        /// 处理深圳电子普通发票的Url格式
        /// </summary>
        /// <param name="scan">扫描二维码得到的信息</param>
        /// <returns></returns>
        ReturnDataDto<SysInvoice> AnalyzeBlockchainInvoiceWithUrl(string scan);

        /// <summary>
        /// 导入发票校验结果
        /// </summary>
        /// <param name="invoice"></param>
        /// <returns></returns>
        Task<ReturnDataDto<SysInvoice>> UploadCkeckInfoAsync(string invoice);
        /// <summary>
        /// 发票校验真伪
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<ReturnDataDto<SysInvoice>> CheckInvoiceListAsync(SysInvoice entity);
        /// <summary>
        /// 添加公司信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<ReturnDataDto<SysCompany>> AddCompanyInfoAsync(SysCompany entity);
    }
}
