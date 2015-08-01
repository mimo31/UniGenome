using System;

namespace UniGenome
{

    public enum NumberOperatorType
    {
        Add, Subtract, Multiply, Divide, If
    }

    public struct NumberOperator : ICloneable
    {
        public NumberOperatorType Type{ get; set; }
        public NodePointer[] InputValues { get; set; }

        public object Clone()
        {
            NumberOperator clone = new NumberOperator();
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
