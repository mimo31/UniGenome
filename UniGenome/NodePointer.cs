using System;
using System.Diagnostics;

namespace UniGenome
{

    public enum NodeType
    {
        Constant, Input, Operator
    }

    public struct NodePointer : ICloneable, IEquatable<NodePointer>
    {
        public int Index { get; set; }
        public NodeType Type { get; set; }
        public bool IsNumber { get; set; }

        public static NodePointer Empty = new NodePointer()
        {
            Index = -1
        };

        public object Clone()
        {
            NodePointer clone = new NodePointer();
            clone.Index = this.Index;
            clone.Type = this.Type;
            clone.IsNumber = this.IsNumber;
            return clone;
        }

        public bool Equals(NodePointer pointer)
        {
            return (this.Index == pointer.Index) && (this.Type == pointer.Type) && (this.IsNumber == pointer.IsNumber);
        }
    }
}
