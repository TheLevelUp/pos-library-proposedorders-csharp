using LevelUp.Pos.ProposedOrders.DiTalk.Models;
using LevelUp.Pos.ProposedOrders.DiTalk.Wrappers;

namespace LevelUp.Pos.ProposedOrders.DiTalk.Services
{
    internal interface IAdjustCheckService
    {
        AdjustedCheckValuesModel AdjustProposedOrder(
            int outstandingAmount,
            int taxAmount,
            int exemptAmount,
            int paymentAmount);

        AdjustedCheckValuesModel AdjustCompleteOrder(
            int outstandingAmount,
            int taxAmount,
            int exemptAmount,
            int paymentAmount,
            int appliedDiscountAmount);
    }

    internal class AdjustCheckService : IAdjustCheckService
    {
        public interface IDependencies
        {
            IMathWrapper MathWrapper { get; }
            IAdjustExemptService AdjustExemptService { get; }
            IAdjustTaxService AdjustTaxService { get; }
            IDataSanitizerService DataSanitizerService { get; }
        }

        private readonly IDependencies _d;

        public AdjustCheckService(IDependencies dependencies) => _d = dependencies;

        public AdjustedCheckValuesModel AdjustProposedOrder(
            int outstandingAmount,
            int taxAmount,
            int exemptAmount,
            int paymentAmount)
        {
            var checkModel = new CheckModel
            {
                ExemptionAmount = exemptAmount,
                OutstandingAmount = outstandingAmount,
                PaymentAmount = paymentAmount,
                TaxAmount = taxAmount
            };

            return RunAdjustments(checkModel);
        }

        public AdjustedCheckValuesModel AdjustCompleteOrder(
            int outstandingAmount,
            int taxAmount,
            int exemptAmount,
            int paymentAmount,
            int appliedDiscountAmount)
        {
            int outstandingAmountWithDiscount = outstandingAmount + _d.MathWrapper.Abs(appliedDiscountAmount);

            var checkModel = new CheckModel
            {
                ExemptionAmount = exemptAmount,
                OutstandingAmount = outstandingAmountWithDiscount,
                PaymentAmount = paymentAmount,
                TaxAmount = taxAmount
            };

            return RunAdjustments(checkModel);
        }

        internal AdjustedCheckValuesModel RunAdjustments(CheckModel checkModel)
        {
            var sanitizedCheck = _d.DataSanitizerService.SanitizeData(checkModel);
            var adjustedTaxAmount = _d.AdjustTaxService.CalculateAdjustedTaxAmount(sanitizedCheck);
            var adjustedExemptionAmount = _d.AdjustExemptService.CalculateAdjustedExemptionAmount(sanitizedCheck);

            return new AdjustedCheckValuesModel(
                sanitizedCheck.PaymentAmount,
                adjustedTaxAmount,
                adjustedExemptionAmount);
        }
    }
}
