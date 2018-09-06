using System;

namespace LevelUp.Pos.ProposedOrders.Tests.Mocks
{
    /// <summary>
    /// This class is meant to streamline expected calculations from a typical point of sale for the purpose of 
    /// testing.  The behavior of this class expects a starting total amount due (including tax), and the tax amount.
    /// It handles applying pre-tax discounts and standard payments/tenders.
    /// </summary>
    internal class PointOfSale
    {
        private int StartingTotal { get; }
        private int StartingTax { get; }
        private int StartingSubtotal => StartingTotal - StartingTax;
        private decimal TaxRate => (decimal)StartingTax / (decimal)StartingSubtotal;

        private int TotalDiscounts { get; set; }
        private int TotalTenders { get; set; }

        private int AdjustedSubtotal => StartingSubtotal - TotalDiscounts;
        private decimal AdjustedSubtotalInDollars => AdjustedSubtotal / 100.0M;

        private int AdjustedTax =>
            decimal.ToInt32(Math.Round(AdjustedSubtotalInDollars * TaxRate * 100.0M, MidpointRounding.ToEven));

        /// <summary>
        /// The current amount, in cents, owed by a customer, at any given moment.
        /// </summary>
        public int TotalOutstandingAmount => AdjustedSubtotal + AdjustedTax - TotalTenders;

        /// <summary>
        /// The total amount of tax due, in cents, on a check, at any given moment.
        /// </summary>
        public int TotalTaxAmount => AdjustedTax;

        /// <summary>
        /// By passing in a total amount owed and the tax due on a check, we can simulate a simplistic point of sale
        /// for testing purposes.
        /// </summary>
        /// <param name="total">The intial amount due on the check including tax, in cents.</param>
        /// <param name="tax">The initial tax due on the check, in cents.</param>
        public PointOfSale(int total, int tax)
        {
            if (total == tax)
            {
                throw new Exception($"The total must be greater than tax (a subtotal amount must exist). If the intent " +
                                    $"is to test a partial payment scenario, use the ${nameof(ApplyTender)} method to " +
                                    $"reduce the outstanding balance.");
            }

            StartingTotal = total;
            StartingTax = tax;
        }

        /// <summary>
        /// Applies a pre-tax discount to the check.
        /// </summary>
        /// <param name="discountAmountInCents"></param>
        public void ApplyDiscount(int discountAmountInCents) => TotalDiscounts += discountAmountInCents;

        /// <summary>
        /// Apply a standard tender to the check. e.g. Cash, Credit Card, etc.
        /// </summary>
        /// <param name="tenderAmountInCents"></param>
        public void ApplyTender(int tenderAmountInCents) => TotalTenders += tenderAmountInCents;
    }
}
