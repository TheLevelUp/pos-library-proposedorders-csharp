using System;
using FluentAssertions;
using LevelUp.Pos.CheckCalculators.Tests.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace LevelUp.Pos.ProposedOrders.Tests
{
    [TestClass]
    public class DecimalConversionTests
    {
        [TestMethod]
        public void SameCompleteOrderResultsForDecimalAndInt()
        {
            var calculatedWithDecimal =
                ProposedOrderCalculator.CalculateCompleteOrderValuesFromDollars(9m, 1m, 1m, 8.5m, 0.5m);
            var calculatedWithInt =
                ProposedOrderCalculator.CalculateCompleteOrderValues(900, 100, 100, 850, 50);

            calculatedWithDecimal.ShouldBeEquivalentTo(calculatedWithInt);
        }

        [TestMethod]
        public void SameProposedOrderResultsForDecimalAndInt()
        {
            var calculatedWithDecimal =
                ProposedOrderCalculator.CalculateCreateProposedOrderValuesFromDollars(9m, 1m, 1m, 8.5m);
            var calculatedWithInt =
                ProposedOrderCalculator.CalculateCreateProposedOrderValues(900, 100, 100, 850);

            calculatedWithDecimal.ShouldBeEquivalentTo(calculatedWithInt);
        }

        [TestMethod]
        public void AdjustedCheckDollarCentsConversion()
        {
            var adjustedCheck =
                ProposedOrderCalculator.CalculateCreateProposedOrderValues(500, 100, 100, 100);

            adjustedCheck.ShouldBeEquivalentTo(adjustedCheck.ToDollars().ToCents());
        }
    }
}
