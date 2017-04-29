# LevelUp-Point Of Sale Check Calculator

- [Introduction](#introduction)
- [Usage](#usage)
- [Overview](#order-flow)
- [Links](#links)

## Introduction
For point-of-sale and point-of-sale-integration developers working with LevelUp, this library simplifies what a developer needs to know to create orders for customers.

This single method takes known data from your check, such as:

- The current total outstanding amount (including tax) due on the check.
- The total tax amount on the check.
- The total amount of items exempt from earning loyalty (e.g. tobacco, alcohol).
- The current amount your customer wants to pay.

...and gives you the values needed to either "[create a Proposed Order](http://developer.thelevelup.com/api-reference/v15/orders-create-proposed/)" or "[complete an Order](http://developer.thelevelup.com/api-reference/v15/orders-create-completed/)".

- The `spend_amount`
- The `tax_amount`
- The `exemption_amount`

## Usage
Using this library is simple. There is one method and it always returns one object with three properties.
```
class Program
{
    static void Main(string[] args)
    {
        // $12.00 is owed, $1.00 of that is tax. $3.00 of that is tobacco/alcohol, and the customer wants to pay
        // $10.00 towards the check
        int totalOutstandingAmount = 1200;
        int totalTaxAmount = 100;
        int totalExemptionAmount = 300;
        int customerPaymentAmount = 1000;

        var adjustedCheckValues = LevelUp.Pos.ProposedOrderCalculator.Calculator.CalculateAdjustedCheckValues(
            totalOutstandingAmount,
            totalTaxAmount,
            totalExemptionAmount,
            customerPaymentAmount
        );

        Console.WriteLine($"The spend_amount is {adjustedCheckValues.SpendAmount}.");           // 1000
        Console.WriteLine($"The tax_amount is {adjustedCheckValues.TaxAmount}.");               // 0
        Console.WriteLine($"The exemption_amount is {adjustedCheckValues.ExemptionAmount}.");   // 200
    }
}
```

## Order Flow
Below is how you might expect an order flow to work with this library to create an order for a customer on LevelUp using proposed orders:

```
// ...
// Retreive the inputs to this library from the check (described in the introduction)
// ...

var adjustedCheckValues = LevelUp.Pos.ProposedOrderCalculator.Calculator.CalculateCreateProposedOrderValues(
    totalOutstandingAmount,
    totalTaxAmount,
    totalExemptionAmount,
    customerPaymentAmount
);

// -> API Call: LevelUp Create Proposed Order
// <- If successful, apply the customers discount credit

// Retrieve the inputs to this library again (the tax, for instance, has likely changed)

var adjustedCheckValues = LevelUp.Pos.ProposedOrderCalculator.Calculator.CalculateCompleteOrderValues(
    totalOutstandingAmount,
    totalTaxAmount,
    totalExemptionAmount,
    customerPaymentAmount,
    appliedDiscountAmount
);

// -> API Call: LevelUp Complete Order
// <- If successful, the process of placing a LevelUp order is finished
```

## Links
- [developer.thelevelup.com](https://developer.thelevelup.com)
  - [Create Proposed Order](http://developer.thelevelup.com/api-reference/v15/orders-create-proposed/)
  - [Complete Order](http://developer.thelevelup.com/api-reference/v15/orders-create-completed/)