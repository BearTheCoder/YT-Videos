using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace RandomNumbers
{
    class Program
    {
        static List<ulong> ResultList = new List<ulong>();
        static ulong Xnp1 = 0;
        static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            ulong a = 1416;
            ulong c = 1;
            ulong Seed = 14189;

            // With every increase by a factor of 10 to the modulus the time it takes increases by a factor of 100.
            // A modulus of 1,000,000 takes 1313.68 seconds, a modulus of 1,000,000,000 would take 41 years.
            ulong m = 51981;

            /*// The following will produce an iterative equation chain and produce 100% efficiency
            ulong a = 1;
            ulong c = 1;
            ulong Seed = 0;
            // With every increase by a factor of 10 to the modulus the time it takes increases by a factor of 100.
            // A modulus of 1,000,000 takes 1313.68 seconds, a modulus of 1,000,000,000 would take 41 years.
            ulong m = 1000000;*/
            while (true)
            {
                ulong Result = LCG(a, c, Seed, m);
                if (ResultList.Contains(Result) && ResultList != null)
                {
                    Console.WriteLine("  ");
                    Console.WriteLine("  Equation Xnp1 = ((a * Xn) + c) mod m");
                    Console.WriteLine("  Variable 'a': " + a);
                    Console.WriteLine("  Variable 'c': " + c);
                    Console.WriteLine("  Variable 'Xn': " + Seed);
                    Console.WriteLine("  Variable 'm': " + m);
                    Console.WriteLine("  Count of unique numbers: " + ResultList.Count);
                    Console.WriteLine("  Efficiency check complete: "
                        + ((decimal)ResultList.Count / m) * 100 + "% efficient");
                    break;
                }
                else
                {
                    //Console.WriteLine(Result); // Uncomment to do value checks with lower numbers
                    ResultList.Add(Result);
                }
            }
            sw.Stop();
            Console.WriteLine("  Operation complete.");
            Console.WriteLine("  Elapsed Seconds: " + ((float)sw.ElapsedMilliseconds /*/ 1000*/));
            Console.WriteLine("  Press any button to close...");
            Console.ReadLine();
        }
        static ulong LCG(ulong a, ulong c, ulong Xn, ulong m)
        {
            if (Xnp1 == 0)
            {
                Xnp1 = ((a * Xn) + c) % m;
                return Xnp1;
            }
            else
            {
                Xnp1 = ((a * Xnp1) + c) % m;
                return Xnp1;
            }
        }
    }
}
