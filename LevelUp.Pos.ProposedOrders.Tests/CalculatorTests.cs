#region Copyright (Apache 2.0)
//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
// <copyright file="CalculatorTests.cs" company="SCVNGR, Inc. d/b/a LevelUp">
//   Copyright(c) 2017 SCVNGR, Inc. d/b/a LevelUp. All rights reserved.
// </copyright>
// <license publisher="Apache Software Foundation" date="January 2004" version="2.0">
//   Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except
//   in compliance with the License. You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software distributed under the License
//   is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
//   or implied. See the License for the specific language governing permissions and limitations under
//   the License.
// </license>
//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
#endregion

using System;
using FluentAssertions;
using LevelUp.Pos.CheckCalculators.Tests.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LevelUp.Pos.ProposedOrders.Tests
{
    [TestClass]
    public class CalculatorTests
    {
        [TestMethod]
        public void RunTestBattery()
        {
            RunTestArray(CalculatorTestData.TestBattery);
        }

        private void RunTestArray(int[,] values)
        {
            for (int i = 0; i < values.GetLength(0); i++)
            {
                int totalOutstandingAmount = values[i, 0];
                int totalTaxAmount = values[i, 1];
                int totalExemptionAmount = values[i, 2];
                int spendAmount = values[i, 3];

                int expectedSpendAmount = values[i, 4];
                int expectedTaxAmount = values[i, 5];
                int expectedExemptionAmount = values[i, 6];

                AdjustedCheckValues expectedCheckValues =
                    new AdjustedCheckValues(expectedSpendAmount, expectedTaxAmount, expectedExemptionAmount);

                AdjustedCheckValues actualCheckValues = ProposedOrderCalculator.CalculateCreateProposedOrderValues(
                    totalOutstandingAmount,
                    totalTaxAmount,
                    totalExemptionAmount,
                    spendAmount);

                actualCheckValues.ShouldBeEquivalentTo(expectedCheckValues,
                    "row={0}; Expected:{1}; Actual: {2}",
                    i, expectedCheckValues, actualCheckValues, Environment.NewLine);
            }
        }

        [TestMethod]
        public void TestTest123()
        {
            int totalOutstandingAmount = 1;
            int totalTaxAmount = 10;
            int totalExemptionAmount = 0;
            int spendAmount = 1;

            AdjustedCheckValues expectedCheckValues =
                new AdjustedCheckValues(spendAmount:1, taxAmount:1, exemptionAmount:0);

            AdjustedCheckValues actualCheckValues = ProposedOrderCalculator.CalculateCreateProposedOrderValues(
                totalOutstandingAmount,
                totalTaxAmount,
                totalExemptionAmount,
                spendAmount);

            actualCheckValues.ShouldBeEquivalentTo(expectedCheckValues);
        }
    }
}