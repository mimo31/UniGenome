using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniGenome;

namespace TestingUniGenome
{
    static class Program
    {
        static Random R = new Random();

        static void Main(string[] args)
        {
            while (1 == 1)
            {
                Console.WriteLine("NEW");
                Genome g = new Genome(2, 2, 5, 3, R);
                Console.WriteLine("GET MUTATION");
                g = g.GetMutation();
                long[] numberInputs = new long[] { 9600, 32, 930, 66, 2 };
                bool[] boolInputs = new bool[] { false, false };
                Console.WriteLine("PUSH INPUTS");
                g.PushInputs(boolInputs, numberInputs);
                Console.WriteLine("GET OUTPUT");
                g.GetNumberOutput(2);
            }
        }
    }
}
