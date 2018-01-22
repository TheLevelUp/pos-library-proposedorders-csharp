namespace LevelUp.Pos.ProposedOrders.DiTalk.Models
{
    public class AdjustedCheckValuesModel
    {
        public int SpendAmount { get; }
        public int TaxAmount { get; }
        public int ExemptionAmount { get; }

        internal AdjustedCheckValuesModel(int spendAmount, int taxAmount, int exemptionAmount)
        {
            SpendAmount = spendAmount;
            TaxAmount = taxAmount;
            ExemptionAmount = exemptionAmount;
        }

        public override string ToString()
        {
            return $"SpendAmount={SpendAmount};TaxAmount={TaxAmount};ExemptionAmount={ExemptionAmount};";
        }
    }
}
