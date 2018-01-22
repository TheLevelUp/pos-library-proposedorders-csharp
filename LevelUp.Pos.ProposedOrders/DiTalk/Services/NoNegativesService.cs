using LevelUp.Pos.ProposedOrders.DiTalk.Wrappers;

namespace LevelUp.Pos.ProposedOrders.DiTalk.Services
{
    internal interface INoNegativesService
    {
        int SmallerOrZeroIfNegative(int a, int b);

        int ZeroIfNegative(int val);
    }

    internal class NoNegativesService : INoNegativesService
    {
        public interface IDependencies
        {
            IMathWrapper MathWrapper { get; }
        }

        private readonly IDependencies _d;

        public NoNegativesService(IDependencies dependencies) => _d = dependencies;

        public int SmallerOrZeroIfNegative(int a, int b)
            => ZeroIfNegative(_d.MathWrapper.Min(a, b));

        public int ZeroIfNegative(int val) => _d.MathWrapper.Max(0, val);
    }
}
