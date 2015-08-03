using System;
using System.Diagnostics;

namespace UniGenome
{

    public enum NodeType
    {
        Constant, Input, Operator
    }

    public enum Type
    {
        Number, Bool, Double
    }

    public struct NodePointer : IEquatable<NodePointer>
    {
        public int Index { get; set; }
        public NodeType Type { get; set; }
        public Type ValueType { get; set; }

        public static NodePointer Empty = new NodePointer()
        {
            Index = -1
        };

        public bool Equals(NodePointer pointer)
        {
            return (this.Index == pointer.Index) && (this.Type == pointer.Type) && (this.ValueType == pointer.ValueType);
        }
    }
}
