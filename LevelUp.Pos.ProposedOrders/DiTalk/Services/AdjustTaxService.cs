using System;
using LevelUp.Pos.ProposedOrders.DiTalk.Models;

namespace LevelUp.Pos.ProposedOrders.DiTalk.Services
{
    internal interface IAdjustTaxService
    {
        int CalculateAdjustedTaxAmount(CheckModel checkModel);
    }

    internal class AdjustTaxService : IAdjustTaxService
    {
        public interface IDependencies
        {
            INoNegativesService NoNegativesService { get; set; }
        }

        private readonly IDependencies _d;

        public AdjustTaxService(IDependencies dependencies) => _d = dependencies;

        public int CalculateAdjustedTaxAmount(CheckModel checkModel)
        {
            if (checkModel.TaxAmount > checkModel.OutstandingAmount)
            {
                throw new Exception("Tax amount cannot be greater than total outstanding amount.");
            }

            bool isTaxFullyPaid = checkModel.PaymentAmount >= checkModel.OutstandingAmount;
            if (isTaxFullyPaid)
            {
                return checkModel.TaxAmount;
            }

            return _d.NoNegativesService.ZeroIfNegative(checkModel.PaymentAmount - checkModel.PreTaxSubtotal);
        }
    }
}
