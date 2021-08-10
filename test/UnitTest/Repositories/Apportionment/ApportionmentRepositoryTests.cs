using Microsoft.VisualStudio.TestTools.UnitTesting;
using YDH.Port.DBContext.Repositories.Apportionment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTest;
using System.Diagnostics;

namespace YDH.Port.DBContext.Repositories.Apportionment.Tests
{
    [TestClass()]
    public class ApportionmentRepositoryTests
    {
        [TestMethod()]
        public async Task GetOperatorBsIdListAsyncTestAsync()
        {
            //var repository = TestHost.GetService<IApportionmentRepository>();
            //var watch = Stopwatch.StartNew();
            //var list = await repository.GetOperatorBsIdListAsync(DateTime.Now.AddMonths(-1));
            //var e = watch.Elapsed;
        }

        [TestMethod()]
        public async Task GetIndirectCostApportionmentItemListAsyncTestAsync()
        {
            //var repository = TestHost.GetService<IApportionmentRepository>();
            //var watch = Stopwatch.StartNew();
            //var list = await repository.GetIndirectCostApportionmentItemListAsync(DateTime.Now.AddMonths(-1));
            //var e = watch.Elapsed;
        }

        [TestMethod()]
        public async Task GetIndirectCostApportionmentItemCountAsyncTestAsync()
        {
            //var repository = TestHost.GetService<IApportionmentRepository>();
            //var count = await repository.GetIndirectCostApportionmentItemCountAsync(DateTime.Now.AddMonths(-1));
        }

        [TestMethod()]
        public async Task GetIndirectCostAdjustMaxBsidTestAsync()
        {
            //var repository = TestHost.GetService<IApportionmentRepository>();
            //var bsid = await repository.GetIndirectCostAdjustMaxBsid(DateTime.Now.AddMonths(-1), 5000);

        }

        [TestMethod()]
        public async Task GetCustomsApportionmentItemListAsyncTestAsync()
        {
            //var repository = TestHost.GetService<IApportionmentRepository>();

            //DateTime dt = new DateTime(2021, 1, 1, 0, 0, 0);
            //await repository.GetCustomsApportionmentItemListAsync(1,1);
           
        }
    }
}