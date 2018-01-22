using System;
using LevelUp.Pos.ProposedOrders.DiTalk.Models;

namespace LevelUp.Pos.ProposedOrders.DiTalk.Services
{
    internal interface IAdjustExemptService
    {
        int CalculateAdjustedExemptionAmount(CheckModel checkModel);
    }

    internal class AdjustExemptService : IAdjustExemptService
    {
        public interface IDependencies
        {
            INoNegativesService NoNegativesService { get; }
        }

        private readonly IDependencies _d;

        public AdjustExemptService(IDependencies dependencies) => _d = dependencies;

        public int CalculateAdjustedExemptionAmount(CheckModel checkModel)
        {
            if (checkModel.ExemptionAmount > checkModel.PreTaxSubtotal)
            {
                throw new Exception("Exemption amount cannot be greater that the pre-tax total on the check.");
            }

            bool isExemptFullyPaid = checkModel.PaymentAmount >= checkModel.PreTaxSubtotal;
            if (isExemptFullyPaid)
            {
                return checkModel.ExemptionAmount;
            }

            return _d.NoNegativesService.ZeroIfNegative(checkModel.PaymentAmount - checkModel.NonExemptSubtotal);
        }
    }
}
