using System;

namespace UniGenome
{
    public struct OperatorNode<T>
    {
        public Func<long[], bool[], double[], T> Operation{ get; set; }
        public NodePointer[] NumberInputs { get; set; }
        public NodePointer[] BoolInputs { get; set; }
        public NodePointer[] DoubleInputs { get; set; }

        public T Evalute(long[] numberInputs, bool[] boolInputs, double[] doubleInputs)
        {
            return this.Operation(numberInputs, boolInputs, doubleInputs);
        }
    }
}
