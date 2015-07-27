using System;

namespace UniGenome
{

    public enum BoolOperatorType
    {
        AND, NOT, OR, XOR, BiggerThan, Equals
    }

    public class BoolOperator : ICloneable
    {
        public BoolOperatorType Type { get; set; }
        public NodePointer[] InputValues { get; set; }

        public object Clone()
        {
            BoolOperator clone = new BoolOperator();
            clone.Type = this.Type;
            clone.InputValues = new NodePointer[this.InputValues.Length];
            for (int i = 0; i < this.InputValues.Length; i++)
            {
                clone.InputValues[i] = this.InputValues[i];
            }
            return clone;
        }
    }
}
