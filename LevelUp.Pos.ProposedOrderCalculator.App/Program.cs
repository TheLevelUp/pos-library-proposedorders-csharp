using System;

namespace LevelUp.Pos.ProposedOrderCalculator.App
{
    class Program
    {
        static void Main(string[] args)
        {
            // $12.00 is owed, $1.00 of that is tax. $3.00 of that is tobacco/alcohol, and they want to pay $10.00
            // towards the check
            int totalOutstandingAmount = 1200;
            int totalTaxAmount = 100;
            int totalExemptionAmount = 300;
            int spendAmount = 1000;

            var adjustedCheckValues = Calculator.CalculateAdjustedCheckValues(
                totalOutstandingAmount,
                totalTaxAmount,
                totalExemptionAmount,
                spendAmount
            );

            Console.WriteLine($"The spend_amount is {adjustedCheckValues.SpendAmount}.");           // 1000
            Console.WriteLine($"The tax_amount is {adjustedCheckValues.TaxAmount}.");               // 0
            Console.WriteLine($"The exemption_amount is {adjustedCheckValues.ExemptionAmount}.");   // 200
        }
    }
}
