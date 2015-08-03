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
        public Operator<double>[] DoubleOperatos { get; set; }
    }
}
