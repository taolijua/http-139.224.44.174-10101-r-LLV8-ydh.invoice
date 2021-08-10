using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YDH.Finace.App.Basics;
using YDH.Finace.Business.Management;
using YDH.Finace.Entities.Base;
using YDH.Finace.Entities.Management;

namespace YDH.Finace.App.Controllers
{
   /// <summary>
   /// 发票管理
   /// </summary>
    public class InvoiceController : ApiController
    {

        private readonly IInvoiceService _InvoiceService = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="InvoiceService"></param>
        public InvoiceController(IInvoiceService InvoiceService)
        {
            _InvoiceService = InvoiceService;
        }

        /// <summary>
        /// 获取报销用户列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseRep<SysReimbursementApplicant>> QueryBaoXiaoNameListAsync (PageRequestDto<SysReimbursementApplicant> entity)
        {
            return await _InvoiceService.QueryBaoXiaoNameListAsync(entity);

        }


        /// <summary>
        /// 添加报销用户信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ReturnDataDto<SysReimbursementApplicant>> AddBXNameInfoAsync(SysReimbursementApplicant entity)
        {
            return await _InvoiceService.AddBXNameInfoAsync(entity);

        }

        /// <summary>
        /// 获取发票列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseRep<SysInvoice>> QueryInvoiceListAsync(PageRequestDto<SysInvoice> entity)
        {
            return await _InvoiceService.QueryInvoiceListAsync(entity);

        }


        /// <summary>
        /// 获取录入人员列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseRep<SysUser>> QueryInputNameListAsync(PageRequestDto<SysUser> entity)
        {
            return await _InvoiceService.QueryInputNameListAsync(entity);
        }

        /// <summary>
        /// 获取报销公司信息列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseRep<SysCompany>> QueryCompanyListAsync(PageRequestDto<SysCompany> entity)
        {
            return await _InvoiceService.QueryCompanyListAsync(entity);
        }

        /// <summary>
        /// 添加公司信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ReturnDataDto<SysCompany>> AddCompanyInfoAsync(SysCompany entity)
        {
            return await _InvoiceService.AddCompanyInfoAsync(entity);

        }

        /// <summary>
        /// 手动添加发票信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ReturnDataDto<SysInvoice>> SaveInvoiceInfoAsync(SysInvoice entity)
        { 
            return await _InvoiceService.SaveInvoiceInfoAsync(entity);
        }
        /// <summary>
        /// 扫描添加发票信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ReturnDataDto<SysInvoice>> ScanSaveInvoiceInfoAsync(SysInvoice entity)
        {
            return await _InvoiceService.ScanSaveInvoiceInfoAsync(entity);
        }

        /// <summary>
        /// 导入发票校验结果
        /// </summary>
        /// <param name="invoice"></param>
        /// <returns></returns>
        [HttpPost]

        public async Task<ReturnDataDto<SysInvoice>> UploadCkeckInfoAsync(string invoice)
        {
            string data = Request.Form["data"];
            return await _InvoiceService.UploadCkeckInfoAsync(data);
        }
        /// <summary>
        /// 发票校验
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]

        public async Task<ReturnDataDto<SysInvoice>> CheckInvoiceListAsync(SysInvoice entity)
        {

            return await _InvoiceService.CheckInvoiceListAsync(entity);
        }
    }
}
