using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace LevelUp.CheckCalculator.Tests
{
    [TestClass]
    public class CalculatorTests
    {
        [TestMethod]
        public void DiscountToApply_MiscellaneousTests()
        {
            /* 4 States
             * Compare 'Payment' to 'Due W/Tax' where 'Due W/Tax' is less than, equal, greater than, and 0
             * 
             * 4 States
             * Compare 'MFC' to 'Payment' where 'MFC' is less than, equal, greater than, and 0
             * 
             * 5 States
             * Compare 'Exempt' to 'MFC' or 'Due W/Tax' where 'Exempt' is less than, equal, greater than, and 0
             * 
             * Results in 4 * 4 * 5 test cases.
             */
            decimal[,] values = {
                //MFC, Payment, Due W/Tax, Tax, Exempt, Expected Discount
                {5m, 10m, 10m, 1m, 3m, 5m},
                {5m, 10m, 10m, 1m, 5m, 4m},
                {5m, 10m, 10m, 1m, 10m, 0m},
                {5m, 10m, 10m, 1m, 20m, 0m},
                {5m, 10m, 10m, 1m, 0m, 5m},
                {10m, 10m, 10m, 1m, 3m, 6m},
                {10m, 10m, 10m, 1m, 5m, 4m},
                {10m, 10m, 10m, 1m, 10m, 0m},
                {10m, 10m, 10m, 1m, 20m, 0m},
                {10m, 10m, 10m, 1m, 0m, 9m},
                {20m, 10m, 10m, 1m, 3m, 6m},
                {20m, 10m, 10m, 1m, 5m, 4m},
                {20m, 10m, 10m, 1m, 10m, 0m},
                {20m, 10m, 10m, 1m, 20m, 0m},
                {20m, 10m, 10m, 1m, 0m, 9m},
                {0m, 10m, 10m, 1m, 3m, 0m},
                {0m, 10m, 10m, 1m, 5m, 0m},
                {0m, 10m, 10m, 1m, 10m, 0m},
                {0m, 10m, 10m, 1m, 20m, 0m},
                {0m, 10m, 10m, 1m, 0m, 0m},
                {5m, 10m, 11m, 1m, 3m, 5m},
                {5m, 10m, 11m, 1m, 5m, 5m},
                {5m, 10m, 11m, 1m, 10m, 0m},
                {5m, 10m, 11m, 1m, 20m, 0m},
                {5m, 10m, 11m, 1m, 0m, 5m},
                {10m, 10m, 11m, 1m, 3m, 7m},
                {10m, 10m, 11m, 1m, 5m, 5m},
                {10m, 10m, 11m, 1m, 10m, 0m},
                {10m, 10m, 11m, 1m, 20m, 0m},
                {10m, 10m, 11m, 1m, 0m, 10m},
                {20m, 10m, 11m, 1m, 3m, 7m},
                {20m, 10m, 11m, 1m, 5m, 5m},
                {20m, 10m, 11m, 1m, 10m, 0m},
                {20m, 10m, 11m, 1m, 20m, 0m},
                {20m, 10m, 11m, 1m, 0m, 10m},
                {0m, 10m, 11m, 1m, 3m, 0m},
                {0m, 10m, 11m, 1m, 5m, 0m},
                {0m, 10m, 11m, 1m, 10m, 0m},
                {0m, 10m, 11m, 1m, 20m, 0m},
                {0m, 10m, 11m, 1m, 0m, 0m},
                {5m, 10m, 12m, 1m, 3m, 5m},
                {5m, 10m, 12m, 1m, 5m, 5m},
                {5m, 10m, 12m, 1m, 10m, 1m},
                {5m, 10m, 12m, 1m, 20m, 0m},
                {5m, 10m, 12m, 1m, 0m, 5m},
                {10m, 10m, 12m, 1m, 3m, 8m},
                {10m, 10m, 12m, 1m, 5m, 6m},
                {10m, 10m, 12m, 1m, 10m, 1m},
                {10m, 10m, 12m, 1m, 20m, 0m},
                {10m, 10m, 12m, 1m, 0m, 10m},
                {20m, 10m, 12m, 1m, 3m, 8m},
                {20m, 10m, 12m, 1m, 5m, 6m},
                {20m, 10m, 12m, 1m, 10m, 1m},
                {20m, 10m, 12m, 1m, 20m, 0m},
                {20m, 10m, 12m, 1m, 0m, 10m},
                {0m, 10m, 12m, 1m, 3m, 0m},
                {0m, 10m, 12m, 1m, 5m, 0m},
                {0m, 10m, 12m, 1m, 10m, 0m},
                {0m, 10m, 12m, 1m, 20m, 0m},
                {0m, 10m, 12m, 1m, 0m, 0m},
                {5m, 10m, 0m, 0m, 3m, 0m},
                {5m, 10m, 0m, 0m, 5m, 0m},
                {5m, 10m, 0m, 0m, 10m, 0m},
                {5m, 10m, 0m, 0m, 20m, 0m},
                {5m, 10m, 0m, 0m, 0m, 0m},
                {10m, 10m, 0m, 0m, 3m, 0m},
                {10m, 10m, 0m, 0m, 5m, 0m},
                {10m, 10m, 0m, 0m, 10m, 0m},
                {10m, 10m, 0m, 0m, 20m, 0m},
                {10m, 10m, 0m, 0m, 0m, 0m},
                {20m, 10m, 0m, 0m, 3m, 0m},
                {20m, 10m, 0m, 0m, 5m, 0m},
                {20m, 10m, 0m, 0m, 10m, 0m},
                {20m, 10m, 0m, 0m, 20m, 0m},
                {20m, 10m, 0m, 0m, 0m, 0m},
                {0m, 10m, 0m, 0m, 3m, 0m},
                {0m, 10m, 0m, 0m, 5m, 0m},
                {0m, 10m, 0m, 0m, 10m, 0m},
                {0m, 10m, 0m, 0m, 20m, 0m},
                {0m, 10m, 0m, 0m, 0m, 0m}
            };

            RunTestArray(values);
        }

        private void RunTestArray(decimal[,] values)
        {
            for (int i = 0; i < values.GetLength(0); i++)
            {
                Calculator.CalculateDiscountToApply(values[i, 0],
                    values[i, 1],
                    values[i, 2],
                    values[i, 3],
                    values[i, 4]).Should().Be(values[i, 5], "row={0}", i);
            }
        }
    }
}
