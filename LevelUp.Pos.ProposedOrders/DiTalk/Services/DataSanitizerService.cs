using LevelUp.Pos.ProposedOrders.DiTalk.Models;

namespace LevelUp.Pos.ProposedOrders.DiTalk.Services
{
    internal interface IDataSanitizerService
    {
        CheckModel SanitizeData(CheckModel checkModel);
    }

    internal class DataSanitizerService : IDataSanitizerService
    {
        public interface IDependencies
        {
            INoNegativesService NoNegativesService { get; }
        }

        private readonly IDependencies _d;

        public DataSanitizerService(IDependencies dependencies) => _d = dependencies;

        public CheckModel SanitizeData(CheckModel checkModel)
        {
            checkModel.PaymentAmount = PaymentAmountCannotBeGreaterThanOutstandingAmount(
                checkModel.OutstandingAmount,
                checkModel.PaymentAmount);

            checkModel.TaxAmount = TaxAmountCannotBeGreaterThanOutstandingAmount(
                checkModel.OutstandingAmount,
                checkModel.TaxAmount);

            checkModel.ExemptionAmount = ExemptionAmountCannotBeGreaterThanPreTaxSubtotal(
                checkModel.ExemptionAmount,
                checkModel.PreTaxSubtotal);

            return checkModel;
        }

        private int PaymentAmountCannotBeGreaterThanOutstandingAmount(
            int outstandingAmount,
            int paymentAmount)
            => _d.NoNegativesService.SmallerOrZeroIfNegative(outstandingAmount, paymentAmount);

        private int TaxAmountCannotBeGreaterThanOutstandingAmount(int outstandingAmount, int taxAmount)
            => _d.NoNegativesService.SmallerOrZeroIfNegative(taxAmount, outstandingAmount);

        private int ExemptionAmountCannotBeGreaterThanPreTaxSubtotal(
            int exemptAmount,
            int outstandingAmount)
            => _d.NoNegativesService.SmallerOrZeroIfNegative(exemptAmount, outstandingAmount);
    }
}
