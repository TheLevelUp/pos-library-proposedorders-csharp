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
        //public static int CalculateDiscountToApply(int checkTotalAmount,
        //                                           int taxAmount,
        //                                           int spendAmount,
        //                                           int discountAvailableAmount,
        //                                           int exemptionAmount = 0)
        //{
        //    int amountDueLessTax = checkTotalAmount - taxAmount;

        //    int paymentRequested = Math.Max(0, Math.Min(spendAmount, amountDueLessTax));

        //    if (paymentRequested < amountDueLessTax)
        //    {
        //        int exemptionAmountToIgnore = amountDueLessTax - paymentRequested;

        //        // if attempting a partial payment, reduce the exemption considered against the payment amount
        //        // by the difference between that payment amount and the actual total due
        //        exemptionAmount = Math.Max(0, exemptionAmount - exemptionAmountToIgnore);
        //    }

        //    return Math.Min(discountAvailableAmount, Math.Max(0, spendAmount - exemptionAmount));
        //}


        /// <summary>
        /// Calculates the LevelUp discount to apply based on the arguments passed
        /// </summary>
        /// <param name="merchantFundedCreditAvailableInDollars">The merchant funded credit amount available in dollars returned from the LevelUp API</param>
        /// <param name="paymentAmountInDollars">The requested payment amount from the cashier/customer in dollars</param>
        /// <param name="amountDueInDollars">The remaining amount due on the check including tax in dollars</param>
        /// <param name="taxAmountDueInDollars">The tax amount due on the check in dollars</param>
        /// <param name="exemptedItemsTotalInDollars">Total cost of all exempted items on the check in dollars</param>
        /// <returns>The LevelUp discount to apply in dollars</returns>
        /// <see cref="http://developer.thelevelup.com/api-reference/v14/merchant-funded-credit/"/>
        public static decimal CalculateDiscountToApply(int merchantFundedCreditAvailableInDollars,
            int paymentAmountInDollars,
            int amountDueInDollars,
            int taxAmountDueInDollars,
            int exemptedItemsTotalInDollars)
        {
            int amountDueLessTax = amountDueInDollars - taxAmountDueInDollars;

            int paymentRequested = Math.Max(0, Math.Min(paymentAmountInDollars, amountDueLessTax));

            if (paymentRequested < amountDueLessTax)
            {
                int exemptionAmountToIgnore = amountDueLessTax - paymentRequested;

                // if attempting a partial payment, reduce the exemption considered against the payment amount
                // by the difference between that payment amount and the actual total due
                exemptedItemsTotalInDollars = Math.Max(0, exemptedItemsTotalInDollars - exemptionAmountToIgnore);
            }

            return Math.Min(merchantFundedCreditAvailableInDollars,
                Math.Max(0, paymentRequested - exemptedItemsTotalInDollars));
        }
    }
}
