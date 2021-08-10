using Microsoft.VisualStudio.TestTools.UnitTesting;
using YDH.Port.Business.Apportionment;
using System;
using System.Collections.Generic;

namespace YDH.Port.Business.Apportionment.Tests
{
    [TestClass()]
    public class ApportionmentCalculatorTests
    {
        [TestMethod()]
        public void CalculationByWeight()
        {
           // Assert.IsTrue(TotalChargeList.All(i => CalculationByWeightTest(i)));
        }
       // public bool CalculationByWeightTest(decimal totalCharge)
        //{
            //var rnd = new Random();
            //var list = new List<WeightApportionCalcModel>();
            //for (var i = 0; i < 3121; i++)
            //{
            //    list.Add(new WeightApportionCalcModel { Id = i, Weight = (rnd.Next() % 10) + (decimal)rnd.NextDouble().RoundDown(3) });
            //}
            //var result = ApportionmentCalculator.CalculationByWeight(list, totalCharge);
            //var totalCharge2 = result.Sum(i => i.Charge);
            //return totalCharge2 == totalCharge;
      //  }

        //[TestMethod]
       // public void CalculationByQuantity()
        //{
        //    Assert.IsTrue(TotalChargeList.All(i => CalculationByQuantityTest(i)));
       // }
       // private bool CalculationByQuantityTest(decimal totalCharge)
       // {
            //var rnd = new Random();
            //var list = new List<QuantityApportionCalcModel>();
            //for (var i = 0; i < 3121; i++)
            //{
            //    list.Add(new QuantityApportionCalcModel { Id = i });
            //}
            //var result = ApportionmentCalculator.CalculationByQuantity(list, totalCharge);
            //var totalCharge2 = result.Sum(i => i.Charge);
            //return totalCharge2 == totalCharge;
       // }

        private static IEnumerable<decimal> TotalChargeList => new List<decimal>
        { 
            0.01M, 
            2, 
            200,
            2000,
            3132.19M,
            5566.63M,
            99557454.12M
        };
    }
}