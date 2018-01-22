using System;
using LevelUp.Pos.ProposedOrders.DiTalk;

namespace LevelUp.TestCli
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Starting test service.");

            var calc = new OrderCalculator();
            var response = calc.CalculateCompleteOrderValues(10, 1, 1, 6, 0);

            Console.WriteLine(response.ToString());

            Console.WriteLine("Test ending.  Press any key please.");
            Console.ReadKey();
        }
    }
}
