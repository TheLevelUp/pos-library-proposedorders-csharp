# LevelUp-Point Of Sale Check Calculator

- [Introduction](#introduction)
- [Usage & Order Flow](#usage--order-flow)
- [Links](#links)

## Introduction
For point-of-sale and point-of-sale-integration developers working with LevelUp, this library simplifies what a developer needs to know to create orders for customers. Some common inbound questions include, "What happens if a customer only wants to pay part of the bill? What do I send LevelUp?" This tool will take care of that for you.

The `ProposedOrderCalculator` accepts known check values such as:

- The current total outstanding amount (including tax) due on the check.
- The current tax amount on the check.
- The total amount of items exempt from earning loyalty (e.g. tobacco, alcohol).
- The amount your customer wants to pay.

...and gives you the values needed to either "[create a Proposed Order](http://developer.thelevelup.com/api-reference/v15/orders-create-proposed/)" or "[complete an Order](http://developer.thelevelup.com/api-reference/v15/orders-create-completed/)".

- The `spend_amount`
- The `tax_amount`
- The `exemption_amount`

## Usage & Order Flow
Using this library with Proposed Orders is simple.

Before charging a customer with LevelUp, pass values from your check to `ProposedOrderCalculator.CalculateCreateProposedOrderValues(...)` before calling `/v15/proposed_orders`.

After applying any LevelUp discount available (returned in the previous API response), pass values from your check one more time to `ProposedOrderCalculator.CalculateCompleteOrderValues(...)` before calling `/v15/completed_orders`.
```csharp
class Program
{
    static void Main(string[] args)
    {
        // $9.90 is owed, $0.90 of that is tax. $3.00 of that is tobacco/alcohol, and the customer wants to pay
        // $9.00 towards the check
        int itemsSubtotalAmount = 900;
        int taxAmountDue = itemsSubtotalAmount / TAX_RATE;
        int totalOutstandingAmount = itemsSubtotalAmount + taxAmountDue;
        int exemptionAmount = 300;

        int spendAmount = 900;

        // Create propsed order
        AdjustedCheckValues proposedOrderValues = ProposedOrderCalculator.CalculateCreateProposedOrderValues(
            totalOutstandingAmount,
            taxAmountDue,
            exemptionAmount,
            spendAmount);

        // Make LevelUp API call to /v15/proposed_orders using values within proposedOrderValues

        // available discount amount $1
        int availableDiscountAmount = 100;

        // POS applies $1.00 pretax discount to check and updates subtotal and tax
        itemsSubtotalAmount -= availableDiscountAmount;
        taxAmountDue = itemsSubtotalAmount / TAX_RATE;
        totalOutstandingAmount = itemsSubtotalAmount + taxAmountDue;

        // Create completed order
        AdjustedCheckValues completedOrderValues = ProposedOrderCalculator.CalculateCompleteOrderValues(
            totalOutstandingAmount,
            taxAmountDue,
            exemptionAmount,
            spendAmount,
            availableDiscountAmount);

        // Make LevelUp API call to /v15/completed_orders using values within completedOrderValues
    }
}
```

## Links
- [developer.thelevelup.com](https://developer.thelevelup.com)
  - [Create Proposed Order](http://developer.thelevelup.com/api-reference/v15/orders-create-proposed/)
  - [Complete Order](http://developer.thelevelup.com/api-reference/v15/orders-create-completed/)