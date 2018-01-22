using System;

namespace LevelUp.Pos.ProposedOrders.DiTalk.Wrappers
{
    internal interface IMathWrapper
    {
        int Max(int a, int b);

        int Min(int a, int b);

        int Abs(int value);
    }

    internal class MathWrapper : IMathWrapper
    {
        public int Max(int a, int b) => Math.Max(a, b);

        public int Min(int a, int b) => Math.Min(a, b);

        public int Abs(int value) => Math.Abs(value);
    }
}
