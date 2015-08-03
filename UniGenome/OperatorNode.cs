using System;

namespace UniGenome
{
    public struct OperatorNode<T>
    {
        public Operator<T> Operator{ get; set; }
        public NodePointer[] InputValues { get; set; }
    }
}
