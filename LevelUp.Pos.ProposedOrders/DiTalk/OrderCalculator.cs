using Autofac;
using LevelUp.Pos.ProposedOrders.DiTalk.Models;
using LevelUp.Pos.ProposedOrders.DiTalk.Services;

namespace LevelUp.Pos.ProposedOrders.DiTalk
{
    public interface IOrderCalculator
    {
        AdjustedCheckValuesModel CalculateCreateProposedOrderValues(
            int outstandingAmount,
            int taxAmount,
            int exemptAmount,
            int paymentAmount
        );

        AdjustedCheckValuesModel CalculateCompleteOrderValues(
            int outstandingAmount,
            int taxAmount,
            int exemptAmount,
            int paymentAmount,
            int appliedDiscountAmount
        );
    }

    public class OrderCalculator
    {
        public AdjustedCheckValuesModel CalculateCreateProposedOrderValues(
            int outstandingAmount,
            int taxAmount,
            int exemptAmount,
            int paymentAmount
        )
        {
            var iocContainer = Framework.Autofac.GetContainer();
            using (var scope = iocContainer.BeginLifetimeScope())
            {
                var adjustCheckService = scope.Resolve<IAdjustCheckService>();
                return adjustCheckService.AdjustProposedOrder(
                    outstandingAmount,
                    taxAmount,
                    exemptAmount,
                    paymentAmount);
            }
        }

        public AdjustedCheckValuesModel CalculateCompleteOrderValues(
            int outstandingAmount,
            int taxAmount,
            int exemptAmount,
            int paymentAmount,
            int appliedDiscountAmount
        )
        {
            var iocContainer = Framework.Autofac.GetContainer();
            using (var scope = iocContainer.BeginLifetimeScope())
            {
                var adjustCheckService = scope.Resolve<IAdjustCheckService>();
                return adjustCheckService.AdjustCompleteOrder(
                    outstandingAmount,
                    taxAmount,
                    exemptAmount,
                    paymentAmount,
                    appliedDiscountAmount);
            }
        }
    }
}
