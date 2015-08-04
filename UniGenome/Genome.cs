using System;
using System.Collections.Generic;

namespace UniGenome
{
    public class Genome
    {
        public NodePointer[] NumberOutputNodes { get; private set; }
        public NodePointer[] BoolOutputNodes { get; private set; }
        public NodePointer[] DoubleOutputNodes { get; private set; }

        public List<long> NumberConstants { get; private set; }
        public List<bool> BoolConstants { get; private set; }
        public List<double> DoubleConstants { get; private set; }

        public List<OperatorNode<long>> NumberOperators { get; private set; }
        public List<OperatorNode<bool>> BoolOperators { get; private set; }
        public List<OperatorNode<double>> DoubleOperators { get; private set; }

        public bool InputsPushed { get; private set; }

        private long[] NumberInputsValues;
        private bool[] BoolInputsValues;
        private double[] DoubleInputsValues;

        public readonly GenomeFormat Format;

        private Random R;

        public Genome(Random r, GenomeFormat format)
        {
            if (format != null)
            {
                this.Format = format;
            }
            else
            {
                throw new NullReferenceException("Format argument equals null.");
            }
            if (r != null)
            {
                this.R = r;
            }
            else
            {
                throw new NullReferenceException("R argument equals null.");
            }
            this.NumberOutputNodes = new NodePointer[this.Format.NumberOutputs];
            this.BoolOutputNodes = new NodePointer[this.Format.BoolOutputs];
            this.DoubleOutputNodes = new NodePointer[this.Format.DoubleOutputs];
            this.NumberConstants = new List<long>();
            this.BoolConstants = new List<bool>();
            this.DoubleConstants = new List<double>();
            this.NumberOperators = new List<OperatorNode<long>>();
            this.BoolOperators = new List<OperatorNode<bool>>();
            this.DoubleOperators = new List<OperatorNode<double>>();
            for (int i = 0; i < this.Format.NumberOutputs; i++)
            {
                this.NumberOutputNodes[i] = this.GetNode(false, NodePointer.Empty, ValueType.Number);
            }
            for (int i = 0; i < this.Format.BoolOutputs; i++)
            {
                this.BoolOutputNodes[i] = this.GetNode(false, NodePointer.Empty, ValueType.Bool);
            }
            for (int i = 0; i < this.Format.DoubleOutputs; i++)
            {
                this.DoubleOutputNodes[i] = this.GetNode(false, NodePointer.Empty, ValueType.Double);
            }
        }

        private Genome(GenomeFormat format)
        {
            this.Format = format;
        }

        public Genome Clone()
        {
            Genome clone = new Genome(this.Format);
            clone.NumberOutputNodes = this.NumberOutputNodes.Clone<NodePointer>();
            clone.BoolOutputNodes = this.BoolOutputNodes.Clone<NodePointer>();
            clone.DoubleOutputNodes = this.DoubleOutputNodes.Clone<NodePointer>();
            clone.NumberConstants = this.NumberConstants.Clone();
            clone.BoolConstants = this.BoolConstants.Clone();
            clone.DoubleConstants = this.DoubleConstants.Clone();
            clone.NumberOperators = this.NumberOperators.Clone();
            clone.BoolOperators = this.BoolOperators.Clone();
            clone.DoubleOperators = this.DoubleOperators.Clone();
            clone.R = this.R;
            return clone;
        }

        private List<NodePointer> GetDependencies(NodePointer node)
        {
            List<NodePointer> dependencies = new List<NodePointer>();
            if (node.Type == NodeType.Operator)
            {
                List<NodePointer> inputValues = new List<NodePointer>();
                switch (node.ValueType)
                {
                    case ValueType.Number:
                        inputValues.AddRange(this.NumberOperators[node.Index].NumberInputs);
                        inputValues.AddRange(this.NumberOperators[node.Index].BoolInputs);
                        inputValues.AddRange(this.NumberOperators[node.Index].DoubleInputs);
                        break;
                    case ValueType.Bool:
                        inputValues.AddRange(this.BoolOperators[node.Index].NumberInputs);
                        inputValues.AddRange(this.BoolOperators[node.Index].BoolInputs);
                        inputValues.AddRange(this.BoolOperators[node.Index].DoubleInputs);
                        break;
                    case ValueType.Double:
                        inputValues.AddRange(this.DoubleOperators[node.Index].NumberInputs);
                        inputValues.AddRange(this.DoubleOperators[node.Index].BoolInputs);
                        inputValues.AddRange(this.DoubleOperators[node.Index].DoubleInputs);
                        break;
                }
                foreach (NodePointer inputValue in inputValues)
                {
                    dependencies.AddRange(this.GetDependencies(inputValue));
                }
            }
            dependencies.Add(node);
            return dependencies;
        }

        private List<NodePointer> GetAvailableNodes(bool dontDepend, NodePointer dontDependOn, ValueType valueType)
        {
            List<NodePointer> possiblePointers = new List<NodePointer>();
            List<NodePointer> availiblePointers = new List<NodePointer>();
            int constantsLength;
            int operatorsLength;
            int inputsLength;
            switch (valueType)
            {
                case ValueType.Number:
                    constantsLength = this.NumberConstants.Count;
                    operatorsLength = this.NumberOperators.Count;
                    inputsLength = this.Format.NumberInputs;
                    break;
                case ValueType.Bool:
                    constantsLength = this.BoolConstants.Count;
                    operatorsLength = this.BoolOperators.Count;
                    inputsLength = this.Format.BoolInputs;
                    break;
                case ValueType.Double:
                    constantsLength = this.DoubleConstants.Count;
                    operatorsLength = this.DoubleOperators.Count;
                    inputsLength = this.Format.DoubleInputs;
                    break;
                default:
                    throw new Exception("Switch overflow.");
            }
            for (int i = 0; i < constantsLength; i++)
            {
                NodePointer possiblePointer = new NodePointer();
                possiblePointer.Index = i;
                possiblePointer.Type = NodeType.Constant;
                possiblePointer.ValueType = valueType;
                possiblePointers.Add(possiblePointer);
            }
            for (int i = 0; i < operatorsLength; i++)
            {
                NodePointer possiblePointer = new NodePointer();
                possiblePointer.Index = i;
                possiblePointer.Type = NodeType.Operator;
                possiblePointer.ValueType = valueType;
                possiblePointers.Add(possiblePointer);
            }
            for (int i = 0; i < inputsLength; i++)
            {
                NodePointer pointer = new NodePointer();
                pointer.Index = i;
                pointer.Type = NodeType.Input;
                pointer.ValueType = valueType;
                availiblePointers.Add(pointer);
            }
            if (dontDepend)
            {
                availiblePointers.AddRange(possiblePointers);
            }
            else
            {
                possiblePointers.ForEach(pointer =>
                {
                    if (!this.GetDependencies(pointer).ContainsValue(dontDependOn))
                    {
                        availiblePointers.Add(pointer);
                    }
                });
            }
            return availiblePointers;
        }

        private OperatorNode<T> CreateOperatorNode<T>(Operator<T> newOperator, bool dontDepend, NodePointer dontDependOn)
        {
            OperatorNode<T> node = new OperatorNode<T>();
            node.Operation = newOperator.Operation;
            node.NumberInputs = new NodePointer[newOperator.NumberInputs];
            node.BoolInputs = new NodePointer[newOperator.BoolInputs];
            node.DoubleInputs = new NodePointer[newOperator.DoubleInputs];
            for (int i = 0; i < newOperator.NumberInputs; i++)
            {
                node.NumberInputs[i] = this.GetNode(dontDepend, dontDependOn, ValueType.Number);
            }
            for (int i = 0; i < newOperator.BoolInputs; i++)
            {
                node.BoolInputs[i] = this.GetNode(dontDepend, dontDependOn, ValueType.Bool);
            }
            for (int i = 0; i < newOperator.DoubleInputs; i++)
            {
                node.DoubleInputs[i] = this.GetNode(dontDepend, dontDependOn, ValueType.Double);
            }
            return node;
        }

        private NodePointer GetNode(bool dontDepend, NodePointer dontDependOn, ValueType valueType)
        {
            if (this.R.Next(8) != 0)
            {
                List<NodePointer> availibleNodes = this.GetAvailableNodes(dontDepend, dontDependOn, valueType);
                if (availibleNodes.Count > 0)
                {
                    int selectedPointerIndex = R.Next(availibleNodes.Count);
                    return availibleNodes[selectedPointerIndex];
                }
            }
            int numberofOperators = this.Format.GetNumberOfOperators(valueType);
            if (this.R.Next(4) != 0 && numberofOperators != 0)
            {
                int operatorIndex = this.R.Next(numberofOperators);
                NodePointer operatorNodePointer = new NodePointer();
                operatorNodePointer.Type = NodeType.Operator;
                operatorNodePointer.ValueType = valueType;
                switch (valueType)
                {
                    case ValueType.Number:
                        operatorNodePointer.Index = this.NumberOperators.Count;
                        this.NumberOperators.Add(CreateOperatorNode(this.Format.NumberOperators[operatorIndex], dontDepend, dontDependOn));
                        break;
                    case ValueType.Bool:
                        operatorNodePointer.Index = this.BoolOperators.Count;
                        this.BoolOperators.Add(CreateOperatorNode(this.Format.BoolOperators[operatorIndex], dontDepend, dontDependOn));
                        break;
                    case ValueType.Double:
                        operatorNodePointer.Index = this.DoubleOperators.Count;
                        this.DoubleOperators.Add(CreateOperatorNode(this.Format.DoubleOperators[operatorIndex], dontDepend, dontDependOn));
                        break;
                }
                return operatorNodePointer;
            }
            NodePointer pointer = new NodePointer();
            switch (valueType)
            {
                case ValueType.Number:
                    pointer.Index = this.NumberConstants.Count;
                    this.NumberConstants.Add(this.R.NextLong());
                    break;
                case ValueType.Bool:
                    pointer.Index = this.BoolConstants.Count;
                    this.BoolConstants.Add(this.R.Next(2) == 0);
                    break;
                case ValueType.Double:
                    pointer.Index = this.DoubleConstants.Count;
                    this.DoubleConstants.Add(this.R.NextFullDouble());
                    break;
            }
            pointer.Type = NodeType.Constant;
            pointer.ValueType = valueType;
            return pointer;
        }

        public void PushInputs(bool[] boolInputs, long[] numberInputs, double[] doubleInputs)
        {
            if (this.Format.NumberInputs == numberInputs.Length && this.Format.BoolInputs == boolInputs.Length && this.Format.DoubleInputs == doubleInputs.Length)
            {
                this.BoolInputsValues = boolInputs;
                this.NumberInputsValues = numberInputs;
                this.DoubleInputsValues = doubleInputs;
                this.InputsPushed = true;
            }
            else
            {
                throw new Exception("Lenght of the input arrays doesn't match Genome format.");
            }
        }

        private void MutateInputArray(NodePointer[] inputs, NodePointer parentNode)
        {
            for (int i = 0; i < inputs.Length; i++)
            {
                if (this.R.Next(50) == 0)
                {
                    inputs[i] = this.GetNode(true, parentNode, inputs[0].ValueType);
                }
                else
                {
                    this.MutateNode(inputs[i]);
                }
            }
        }

        private void MutateNode(NodePointer node)
        {
            if (node.Type == NodeType.Operator)
            {
                NodePointer[] numberInputs;
                NodePointer[] boolInputs;
                NodePointer[] doubleInputs;
                switch (node.ValueType)
                {
                    case ValueType.Number:
                        numberInputs = this.NumberOperators[node.Index].NumberInputs;
                        boolInputs = this.NumberOperators[node.Index].BoolInputs;
                        doubleInputs = this.NumberOperators[node.Index].DoubleInputs;
                        break;
                    case ValueType.Bool:
                        numberInputs = this.BoolOperators[node.Index].NumberInputs;
                        boolInputs = this.BoolOperators[node.Index].BoolInputs;
                        doubleInputs = this.BoolOperators[node.Index].DoubleInputs;
                        break;
                    case ValueType.Double:
                        numberInputs = this.DoubleOperators[node.Index].NumberInputs;
                        boolInputs = this.DoubleOperators[node.Index].BoolInputs;
                        doubleInputs = this.DoubleOperators[node.Index].DoubleInputs;
                        break;
                    default:
                        throw new Exception("Switch overflow.");
                }
                this.MutateInputArray(numberInputs, node);
                this.MutateInputArray(boolInputs, node);
                this.MutateInputArray(doubleInputs, node);
            }
        }

        public void MutateOutputNodes(NodePointer[] outputs)
        {
            for (int i = 0; i < outputs.Length; i++)
            {
                if (this.R.Next(50) == 0)
                {
                    outputs[i] = this.GetNode(false, NodePointer.Empty, outputs[0].ValueType);
                }
                else
                {
                    this.MutateNode(outputs[i]);
                }
            }
        }

        public Genome GetMutation()
        {
            Genome mutatedClone = this.Clone();
            mutatedClone.MutateOutputNodes(mutatedClone.NumberOutputNodes);
            mutatedClone.MutateOutputNodes(mutatedClone.BoolOutputNodes);
            mutatedClone.MutateOutputNodes(mutatedClone.DoubleOutputNodes);
            mutatedClone.RemoveUnusedNodes();
            return mutatedClone;
        }

        private void CheckIndex(int index, int upperBound)
        {
            if (index >= upperBound)
            {
                throw new ArgumentOutOfRangeException("Passed index must be smaller than number of Genome outputs.");
            }
            else if (index < 0)
            {
                throw new ArgumentOutOfRangeException("Passed index must be positive.");
            }
        }
 
        private void CheckPushedInputs()
        {
            if (!InputsPushed)
            {
                throw new Exception("Genome must contain pushed inputs before getting any output.");
            }
        }

        public long GetNumberOutput(int index)
        {
            CheckPushedInputs();
            CheckIndex(index, this.NumberOutputNodes.Length);
            return this.GetNumberNodeValue(this.NumberOutputNodes[index]);
        }

        public bool GetBoolOutput(int index)
        {
            CheckPushedInputs();
            CheckIndex(index, this.BoolOutputNodes.Length);
            return this.GetBoolNodeValue(this.BoolOutputNodes[index]);
        }

        public double GetDoubleOutput(int index)
        {
            CheckPushedInputs();
            CheckIndex(index, this.DoubleOutputNodes.Length);
            return this.GetDoubleNodeValue(this.DoubleOutputNodes[index]);
        }

        private T GetOperatorValue<T>(OperatorNode<T> operatorNode)
        {
            long[] numberInputs = new long[operatorNode.NumberInputs.Length];
            bool[] boolInputs = new bool[operatorNode.BoolInputs.Length];
            double[] doubleInputs = new double[operatorNode.DoubleInputs.Length];
            for (int i = 0; i < numberInputs.Length; i++)
            {
                numberInputs[i] = this.GetNumberNodeValue(operatorNode.NumberInputs[i]);
            }
            for (int i = 0; i < boolInputs.Length; i++)
            {
                boolInputs[i] = this.GetBoolNodeValue(operatorNode.BoolInputs[i]);
            }
            for (int i = 0; i < doubleInputs.Length; i++)
            {
                doubleInputs[i] = this.GetDoubleNodeValue(operatorNode.DoubleInputs[i]);
            }
            return operatorNode.Evalute(numberInputs, boolInputs, doubleInputs);
        }

        private long GetNumberNodeValue(NodePointer node)
        {
            if (node.Type == NodeType.Constant)
            {
                return this.NumberConstants[node.Index];
            }
            else if (node.Type == NodeType.Input)
            {
                return this.NumberInputsValues[node.Index];
            }
            else
            {
                return this.GetOperatorValue(this.NumberOperators[node.Index]);
            }
        }

        private bool GetBoolNodeValue(NodePointer node)
        {
            if (node.Type == NodeType.Constant)
            {
                return this.BoolConstants[node.Index];
            }
            else if (node.Type == NodeType.Input)
            {
                return this.BoolInputsValues[node.Index];
            }
            else
            {
                return this.GetOperatorValue(this.BoolOperators[node.Index]);
            }
        }

        private double GetDoubleNodeValue(NodePointer node)
        {
            if (node.Type == NodeType.Constant)
            {
                return this.DoubleConstants[node.Index];
            }
            else if (node.Type == NodeType.Input)
            {
                return this.DoubleInputsValues[node.Index];
            }
            else
            {
                return this.GetOperatorValue(this.DoubleOperators[node.Index]);
            }
        }

        private void RemoveUnusedNodes()
        {
            List<NodePointer> usefulNodes = new List<NodePointer>();
            foreach (NodePointer outputNode in this.NumberOutputNodes)
            {
                usefulNodes.AddRange(this.GetDependencies(outputNode));
            }
            foreach (NodePointer outputNode in this.BoolOutputNodes)
            {
                usefulNodes.AddRange(this.GetDependencies(outputNode));
            }
            this.RemoveUnusedNumberConstants(usefulNodes);
            this.RemoveUnusedNumberOperators(usefulNodes);
            this.RemoveUnusedBoolConstants(usefulNodes);
            this.RemoveUnusedBoolOperators(usefulNodes);
        }

        private void RemoveUnusedNumberConstants(List<NodePointer> usefulNodes)
        {
            for (int i = 0; i < this.NumberConstants.Count; i++)
            {
                NodePointer pointer = new NodePointer();
                pointer.Index = i;
                pointer.IsNumber = true;
                pointer.Type = NodeType.Constant;
                if (!usefulNodes.ContainsValue(pointer))
                {
                    this.NumberConstants.RemoveAt(i);
                    for (int j = 0; j < this.NumberOutputNodes.Length; j++)
                    {
                        if (this.NumberOutputNodes[j].Index > i && this.NumberOutputNodes[j].Type == NodeType.Constant)
                        {
                            this.NumberOutputNodes[j].Index--;
                        }
                    }
                    for (int j = 0; j < this.NumberOperators.Count; j++)
                    {
                        for (int k = 0; k < this.NumberOperators[j].InputValues.Length; k++)
                        {
                            NodePointer inputValue = this.NumberOperators[j].InputValues[k];
                            if (inputValue.Index > i && inputValue.Type == NodeType.Constant && inputValue.IsNumber)
                            {
                                this.NumberOperators[j].InputValues[k].Index--;
                            }
                        }
                    }
                    for (int j = 0; j < this.BoolOperators.Count; j++)
                    {
                        for (int k = 0; k < this.BoolOperators[j].InputValues.Length; k++)
                        {
                            NodePointer inputValue = this.BoolOperators[j].InputValues[k];
                            if (inputValue.Index > i && inputValue.Type == NodeType.Constant && inputValue.IsNumber)
                            {
                                this.BoolOperators[j].InputValues[k].Index--;
                            }
                        }
                    }
                    for (int j = 0; j < usefulNodes.Count; j++)
                    {
                        if (usefulNodes[j].Index > i && usefulNodes[j].Type == NodeType.Constant && usefulNodes[j].IsNumber)
                        {
                            NodePointer newPointer = usefulNodes[j];
                            newPointer.Index--;
                            usefulNodes[j] = newPointer;
                        }
                    }
                    i--;
                }
            }
        }

        private void RemoveUnusedNumberOperators(List<NodePointer> usefulNodes)
        {
            for (int i = 0; i < this.NumberOperators.Count; i++)
            {
                NodePointer pointer = new NodePointer();
                pointer.Index = i;
                pointer.IsNumber = true;
                pointer.Type = NodeType.Operator;
                if (!usefulNodes.ContainsValue(pointer))
                {
                    this.NumberOperators.RemoveAt(i);
                    for (int j = 0; j < this.NumberOutputNodes.Length; j++)
                    {
                        if (this.NumberOutputNodes[j].Index > i && this.NumberOutputNodes[j].Type == NodeType.Operator)
                        {
                            this.NumberOutputNodes[j].Index--;
                        }
                    }
                    for (int j = 0; j < this.NumberOperators.Count; j++)
                    {
                        for (int k = 0; k < this.NumberOperators[j].InputValues.Length; k++)
                        {
                            NodePointer inputValue = this.NumberOperators[j].InputValues[k];
                            if (inputValue.Index > i && inputValue.Type == NodeType.Operator && inputValue.IsNumber)
                            {
                                this.NumberOperators[j].InputValues[k].Index--;
                            }
                        }
                    }
                    for (int j = 0; j < this.BoolOperators.Count; j++)
                    {
                        for (int k = 0; k < this.BoolOperators[j].InputValues.Length; k++)
                        {
                            NodePointer inputValue = this.BoolOperators[j].InputValues[k];
                            if (inputValue.Index > i && inputValue.Type == NodeType.Operator && inputValue.IsNumber)
                            {
                                this.BoolOperators[j].InputValues[k].Index--;
                            }
                        }
                    }
                    for (int j = 0; j < usefulNodes.Count; j++)
                    {
                        if (usefulNodes[j].Index > i && usefulNodes[j].Type == NodeType.Operator && usefulNodes[j].IsNumber)
                        {
                            NodePointer newPointer = usefulNodes[j];
                            newPointer.Index--;
                            usefulNodes[j] = newPointer;
                        }
                    }
                    i--;
                }
            }
        }

        private void RemoveUnusedBoolConstants(List<NodePointer> usefulNodes)
        {
            for (int i = 0; i < this.BoolConstants.Count; i++)
            {
                NodePointer pointer = new NodePointer();
                pointer.Index = i;
                pointer.IsNumber = false;
                pointer.Type = NodeType.Constant;
                if (!usefulNodes.ContainsValue(pointer))
                {
                    this.BoolConstants.RemoveAt(i);
                    for (int j = 0; j < this.BoolOutputNodes.Length; j++)
                    {
                        if (this.BoolOutputNodes[j].Index > i && this.BoolOutputNodes[j].Type == NodeType.Constant)
                        {
                            this.BoolOutputNodes[j].Index--;
                        }
                    }
                    for (int j = 0; j < this.NumberOperators.Count; j++)
                    {
                        for (int k = 0; k < this.NumberOperators[j].InputValues.Length; k++)
                        {
                            NodePointer inputValue = this.NumberOperators[j].InputValues[k];
                            if (inputValue.Index > i && inputValue.Type == NodeType.Constant && !inputValue.IsNumber)
                            {
                                this.NumberOperators[j].InputValues[k].Index--;
                            }
                        }
                    }
                    for (int j = 0; j < this.BoolOperators.Count; j++)
                    {
                        for (int k = 0; k < this.BoolOperators[j].InputValues.Length; k++)
                        {
                            NodePointer inputValue = this.BoolOperators[j].InputValues[k];
                            if (inputValue.Index > i && inputValue.Type == NodeType.Constant && !inputValue.IsNumber)
                            {
                                this.BoolOperators[j].InputValues[k].Index--;
                            }
                        }
                    }
                    for (int j = 0; j < usefulNodes.Count; j++)
                    {
                        if (usefulNodes[j].Index > i && usefulNodes[j].Type == NodeType.Constant && !usefulNodes[j].IsNumber)
                        {
                            NodePointer newPointer = usefulNodes[j];
                            newPointer.Index--;
                            usefulNodes[j] = newPointer;
                        }
                    }
                    i--;
                }
            }
        }

        private void RemoveUnusedBoolOperators(List<NodePointer> usefulNodes)
        {
            for (int i = 0; i < this.BoolOperators.Count; i++)
            {
                NodePointer pointer = new NodePointer();
                pointer.Index = i;
                pointer.IsNumber = false;
                pointer.Type = NodeType.Operator;
                if (!usefulNodes.ContainsValue(pointer))
                {
                    this.BoolOperators.RemoveAt(i);
                    for (int j = 0; j < this.BoolOutputNodes.Length; j++)
                    {
                        if (this.BoolOutputNodes[j].Index > i && this.BoolOutputNodes[j].Type == NodeType.Operator)
                        {
                            this.BoolOutputNodes[j].Index--;
                        }
                    }
                    for (int j = 0; j < this.NumberOperators.Count; j++)
                    {
                        for (int k = 0; k < this.NumberOperators[j].InputValues.Length; k++)
                        {
                            NodePointer inputValue = this.NumberOperators[j].InputValues[k];
                            if (inputValue.Index > i && inputValue.Type == NodeType.Operator && !inputValue.IsNumber)
                            {
                                this.NumberOperators[j].InputValues[k].Index--;
                            }
                        }
                    }
                    for (int j = 0; j < this.BoolOperators.Count; j++)
                    {
                        for (int k = 0; k < this.BoolOperators[j].InputValues.Length; k++)
                        {
                            NodePointer inputValue = this.BoolOperators[j].InputValues[k];
                            if (inputValue.Index > i && inputValue.Type == NodeType.Operator && !inputValue.IsNumber)
                            {
                                this.BoolOperators[j].InputValues[k].Index--;
                            }
                        }
                    }
                    for (int j = 0; j < usefulNodes.Count; j++)
                    {
                        if (usefulNodes[j].Index > i && usefulNodes[j].Type == NodeType.Operator && !usefulNodes[j].IsNumber)
                        {
                            NodePointer newPointer = usefulNodes[j];
                            newPointer.Index--;
                            usefulNodes[j] = newPointer;
                        }
                    }
                    i--;
                }
            }
        }
    }
}
