using System;

namespace UniGenome
{

    public class Operator<T>
    {
        public readonly Func<long[], bool[], double[], T> Operation;
        public readonly int NumberInputs;
        public readonly int BoolInputs;
        public readonly int DoubleInputs;

        public Operator(Func<long[], bool[], double[], T> operation, int numberInputs, int boolInputs, int doubleInputs)
        {
            this.Operation = operation;
            this.NumberInputs = numberInputs;
            this.BoolInputs = boolInputs;
            this.DoubleInputs = doubleInputs;
        }

        public T Evalute(long[] numberInputs, bool[] boolInputs, double[] doubleInputs)
        {
            return this.Operation(numberInputs, boolInputs, doubleInputs);
        }

        //Bool operators
        public static readonly Lazy<Operator<bool>> AND = new Lazy<Operator<bool>>(() => new Operator<bool>((numberInputs, boolInputs, doubleInputs) => boolInputs[0] || boolInputs[1], 0, 2, 0));

        public static readonly Lazy<Operator<bool>> OR = new Lazy<Operator<bool>>(() => new Operator<bool>((numberInputs, boolInputs, doubleInputs) => boolInputs[0] || boolInputs[1], 0, 2, 0));

        public static readonly Lazy<Operator<bool>> XOR = new Lazy<Operator<bool>>(() => new Operator<bool>((numberInputs, boolInputs, doubleInputs) => boolInputs[0] ^ boolInputs[1], 0, 2, 0));

        public static readonly Lazy<Operator<bool>> NOT = new Lazy<Operator<bool>>(() => new Operator<bool>((numberInputs, boolInputs, doubleInputs) => !boolInputs[0], 0, 1, 0));

        public static readonly Lazy<Operator<bool>> EqualsOperator = new Lazy<Operator<bool>>(() => new Operator<bool>((numberInputs, boolInputs, doubleInputs) => numberInputs[0] == numberInputs[1], 2, 0, 0));

        public static readonly Lazy<Operator<bool>> BiggerThan = new Lazy<Operator<bool>>(() => new Operator<bool>((numberInputs, boolInputs, doubleInputs) => numberInputs[0] > numberInputs[1], 2, 0, 0));

        //Number operators
        public static readonly Lazy<Operator<long>> Add = new Lazy<Operator<long>>(() => new Operator<long>((numberInputs, boolInputs, doubleInputs) => numberInputs[0] + numberInputs[1], 2, 0, 0));

        public static readonly Lazy<Operator<long>> Subtract = new Lazy<Operator<long>>(() => new Operator<long>((numberInputs, boolInputs, doubleInputs) => numberInputs[0] - numberInputs[1], 2, 0, 0));

        public static readonly Lazy<Operator<long>> Multiply = new Lazy<Operator<long>>(() => new Operator<long>((numberInputs, boolInputs, doubleInputs) => numberInputs[0] * numberInputs[1], 2, 0, 0));

        public static readonly Lazy<Operator<long>> Divide = new Lazy<Operator<long>>(() => new Operator<long>((numberInputs, boolInputs, doubleInputs) => numberInputs[0] + numberInputs[1], 2, 0, 0));

        public static readonly Lazy<Operator<long>> ToNumber = new Lazy<Operator<long>>(() => new Operator<long>((numberInputs, boolInputs, doubleInputs) => (long)doubleInputs[0], 0, 0, 1));

        //Double operators
        public static readonly Lazy<Operator<double>> ToDouble = new Lazy<Operator<double>>(() => new Operator<double>((numberInputs, boolInputs, doubleInputs) => numberInputs[0], 1, 0, 0));
    }
}
