using System;

namespace UniGenome
{

    public class GenomeFormat
    {
        public int NumberInputs { get; set; }
        public int BoolInputs { get; set; }
        public int DoubleInputs { get; set; }
        public int NumberOutputs { get; set; }
        public int BoolOutputs { get; set; }
        public int DoubleOutputs { get; set; }
        public Operator<long>[] NumberOperators { get; set; }
        public Operator<bool>[] BoolOperators { get; set; }
        public Operator<double>[] DoubleOperators { get; set; }

        public int GetNumberOfOperators(ValueType valueType)
        {
            switch (valueType)
            {
                case ValueType.Number:
                    return this.NumberOperators.Length;
                case ValueType.Bool:
                    return this.BoolOperators.Length;
                case ValueType.Double:
                    return this.DoubleOperatos.Length;
                default:
                    throw new Exception("Switch overflow.");
            }
        }
    }
}
