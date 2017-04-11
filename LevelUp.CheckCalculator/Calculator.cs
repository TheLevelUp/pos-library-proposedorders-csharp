using System;

namespace LevelUp.CheckCalculator
{
    public static class Calculator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="checkTotalAmount">The full total amount of the check (including tax), in cents.</param>
        /// <param name="taxAmount">The amount of tax currently due on the check, in cents.</param>
        /// <param name="spendAmount">The amount the customer is requesting to pay, in cents.</param>
        /// <param name="discountAvailableAmount">The amount of discount available, in cents.</param>
        /// <param name="exemptionAmount">The total amount of exempted items on the check, in cents.</param>
        /// <returns></returns>
        public static decimal CalculateDiscountToApply(int discountAvailableAmount,
            int spendAmount,
            int checkTotalAmount,
            int taxAmount,
            int exemptionAmount)
        {
            int amountDueLessTax = checkTotalAmount - taxAmount;

            int paymentRequested = Math.Max(0, Math.Min(spendAmount, amountDueLessTax));

            if (paymentRequested < amountDueLessTax)
            {
                int exemptionAmountToIgnore = amountDueLessTax - paymentRequested;

                // if attempting a partial payment, reduce the exemption considered against the payment amount
                // by the difference between that payment amount and the actual total due
                exemptionAmount = Math.Max(0, exemptionAmount - exemptionAmountToIgnore);
            }

            return Math.Min(discountAvailableAmount,
                Math.Max(0, paymentRequested - exemptionAmount));
        }
    }
}
