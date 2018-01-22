namespace LevelUp.Pos.ProposedOrders.DiTalk.Models
{
    internal class CheckModel
    {
        public int PaymentAmount { get; set; }
        public int TaxAmount { get; set; }
        public int ExemptionAmount { get; set; }
        public int OutstandingAmount { get; set; }
        public int PreTaxSubtotal => OutstandingAmount - TaxAmount;
        public int NonExemptSubtotal => PreTaxSubtotal - ExemptionAmount;
    }
}
