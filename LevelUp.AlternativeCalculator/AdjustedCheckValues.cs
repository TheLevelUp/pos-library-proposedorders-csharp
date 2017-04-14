namespace LevelUp.AlternativeCalculator
{
    public class AdjustedCheckValues
    {
        public int SpendAmount { get; set; }
        public int TaxAmount { get; set; }
        public int ExemptionAmount { get; set; }

        public AdjustedCheckValues(int spendAmount, int taxAmount, int exemptionAmount)
        {
            SpendAmount = spendAmount;
            TaxAmount = taxAmount;
            ExemptionAmount = exemptionAmount;
        }

        public override bool Equals(object obj)
        {
            var adjustedCheckValues = (AdjustedCheckValues) obj;

            if (adjustedCheckValues != null)
            {
                return adjustedCheckValues.SpendAmount == this.SpendAmount &&
                       adjustedCheckValues.TaxAmount == this.TaxAmount &&
                       adjustedCheckValues.ExemptionAmount == this.ExemptionAmount;
            }

            return base.Equals(obj);
        }
    }
}