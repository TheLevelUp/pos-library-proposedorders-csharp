using System;

namespace LevelUp.CheckCalculator
{
    public class CalculatorHelpers
    {
        /// <summary>
        /// Determines any adjustments to make to a OrderRequest exemption amount to allow for partial payments;
        /// In the case of a partial-payment scenerio, the exemption amount should not exceed the payment amount 
        /// if a customer is not paying the check in full.
        /// </summary>
        /// <param name="spendAmount">The amount of the check that the user has elected to pay.  This 
        /// will typically be equal to the outstanding balance on check, however in split or partial payment scenarios,
        /// it may be less.</param>
        /// <param name="exemptionAmount">The exemption amount specified in the request</param>
        /// <returns>The adjusted exemption amount when partial payment is taken into account.</returns>
        internal static int CalculateAdjustedExemptionAmount(int spendAmount, int? exemptionAmount)
        {
            if (!exemptionAmount.HasValue)
            {
                return 0;
            }

            int adjustedExemptionAmount = exemptionAmount.Value;

            if (exemptionAmount.Value > spendAmount)
            {
                adjustedExemptionAmount = Math.Max(0, spendAmount);
            }

            return adjustedExemptionAmount;
        }

        /// <summary>
        /// Determines any adjustments to make to a OrderRequest spend amount to allow for partial payments.
        /// </summary>
        /// <param name="totalOutstandingAmountOnCheck">The total outstanding amount that is remaining on 
        /// the check, including the tax.</param>
        /// <param name="spendAmount">Any custom spend-amount that might be specified in the UI as part of 
        /// a partial-payment scenerio.  If no custom payment amount was specified, and the consumer expects to pay
        /// the balance of the check in full with this payment, then this would be null</param>
        /// <returns>The adjusted spend amount when partial payment is taken into account.</returns>
        internal static int CalculateAdjustedSpendAmount(int totalOutstandingAmountOnCheck, int? spendAmount)
        {
            if (!spendAmount.HasValue || spendAmount.Value < 0)
            {
                return totalOutstandingAmountOnCheck;
            }

            int returnValue = totalOutstandingAmountOnCheck;

            if (spendAmount.Value < totalOutstandingAmountOnCheck)
            {
                returnValue = spendAmount.Value;
            }

            return returnValue;
        }

        /// <summary>
        /// For proposed orders, specifically in a partial-payment scenario where the customer is not paying the 
        /// check total in full, we must adjust the tax amount that is sent to LevelUp in the request accordingly.
        /// Ultimately the logic here boils down to the idea that the last person to contribute to the check is
        /// responsible for funding the tax portion.  If the tax amount is larger than an individual's contribution, it
        /// will fall upon the last two (etc.) people to apply their payment to the tax portion of the check.  One 
        /// accepted caveat here is that if the last user in a split payment situation has discount credit, they
        /// will not be able to apply any of that credit to the tax portion of the check (where, through a more 
        /// creative ordering, we may have been able to enable them to do so.)  Moreover, if that user does not 
        /// have a linked payment account (two-touch loyalty, etc.) then their payment will be rejected entirely.
        /// </summary>
        /// <param name="spendAmount">The amount of the check that the user has elected to pay.  This 
        /// will typically be equal to the outstanding balance on check, however in split or partial payment scenarios,
        /// it may be less.</param>
        /// <param name="totalOutstandingAmountOnCheck">The total outstanding amount that is remaining on 
        /// the check, including the tax.</param>
        /// <param name="totalTaxAmountOnCheck">The total amount of tax on the check</param>
        internal static int CalculateAdjustedTax(int spendAmount, int totalOutstandingAmountOnCheck, int totalTaxAmountOnCheck)
        {
            // If they're paying the order in full (i.e. no split-payment/two-touch-loyalty), no adjustment is required
            if (decimal.Equals(spendAmount, totalOutstandingAmountOnCheck))
            {
                return totalTaxAmountOnCheck;
            }

            int outstandingAmount = totalOutstandingAmountOnCheck - spendAmount;

            int adjustedTaxAmountToPay = totalTaxAmountOnCheck - outstandingAmount;

            adjustedTaxAmountToPay = Math.Max(adjustedTaxAmountToPay, 0);

            return adjustedTaxAmountToPay;
        }
    }
}
