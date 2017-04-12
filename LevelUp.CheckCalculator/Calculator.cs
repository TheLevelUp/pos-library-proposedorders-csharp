using System;

namespace LevelUp.CheckCalculator
{
    public static class Calculator
    {
        /// <summary>
        /// Determines any adjustments to make to a OrderRequest exemption amount to allow for partial payments;
        /// In the case of a partial-payment scenerio, the exemption amount should not exceed the payment amount 
        /// if a customer is not paying the check in full.
        /// </summary>
        /// <param name="usersPaymentInDollars">The amount of the check that the user has elected to pay.  This 
        /// will typically be equal to the outstanding balance on check, however in split or partial payment scenarios,
        /// it may be less.</param>
        /// <param name="exemptionAmountInDollars">The exemption amount specified in the request</param>
        /// <returns>The adjusted exemption amount when partial payment is taken into account.</returns>
        internal static decimal CalculateAdjustedExemptionAmount(decimal usersPaymentInDollars,
            decimal? exemptionAmountInDollars)
        {
            if (!exemptionAmountInDollars.HasValue)
            {
                return 0;
            }

            decimal adjustedExemptionAmount = exemptionAmountInDollars.Value;
            if (exemptionAmountInDollars.Value > usersPaymentInDollars)
            {
                adjustedExemptionAmount = Math.Max(0, usersPaymentInDollars);
            }

            return adjustedExemptionAmount;
        }

        /// <summary>
        /// Determines any adjustments to make to a OrderRequest spend amount to allow for partial payments.
        /// </summary>
        /// <param name="totalOutstandingAmountOnCheckInDollars">The total outstanding amount that is remaining on 
        /// the check, including the tax.</param>
        /// <param name="paymentRequested">Any custom spend-amount that might be specified in the UI as part of 
        /// a partial-payment scenerio.  If no custom payment amount was specified, and the consumer expects to pay
        /// the balance of the check in full with this payment, then this would be null</param>
        /// <returns>The adjusted spend amount when partial payment is taken into account.</returns>
        internal static decimal CalculateAdjustedSpendAmount(decimal totalOutstandingAmountOnCheckInDollars,
            decimal? paymentRequested)
        {
            if (!paymentRequested.HasValue || paymentRequested.Value < 0)
            {
                return totalOutstandingAmountOnCheckInDollars;
            }

            decimal returnValue = totalOutstandingAmountOnCheckInDollars;

            if (paymentRequested.Value < totalOutstandingAmountOnCheckInDollars)
            {
                returnValue = paymentRequested.Value;
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
        /// <param name="requestedSpendAmountInDollars">The amount of the check that the user has elected to pay.  This 
        /// will typically be equal to the outstanding balance on check, however in split or partial payment scenarios,
        /// it may be less.</param>
        /// <param name="totalOutstandingAmountOnCheckInDollars">The total outstanding amount that is remaining on 
        /// the check, including the tax.</param>
        /// <param name="totalTaxAmountOnCheckInDollars">The total amount of tax on the check</param>
        internal static decimal CalculateAdjustedTax(decimal requestedSpendAmountInDollars,
            decimal totalOutstandingAmountOnCheckInDollars,
            decimal totalTaxAmountOnCheckInDollars)
        {
            // If they're paying the order in full (i.e. no split-payment/two-touch-loyalty), no adjustment is required
            if (decimal.Equals(requestedSpendAmountInDollars, totalOutstandingAmountOnCheckInDollars))
            {
                return totalTaxAmountOnCheckInDollars;
            }

            decimal outstandingAmount = decimal.Subtract(totalOutstandingAmountOnCheckInDollars, requestedSpendAmountInDollars);

            decimal adjustedTaxAmountToPay = decimal.Subtract(totalTaxAmountOnCheckInDollars, outstandingAmount);
            adjustedTaxAmountToPay = Math.Max(adjustedTaxAmountToPay, 0);

            return adjustedTaxAmountToPay;
        }
    }
}