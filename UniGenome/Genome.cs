using System;
using System.Collections.Generic;

namespace UniGenome
{
    public class Genome : ICloneable
    {
        public NodePointer[] NumberOutputNodes { get; private set; }
        public NodePointer[] BoolOutputNodes { get; private set; }
        public NodePointer[] DoubleOutputsNodes { get; private set; }

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
            this.DoubleOutputsNodes = new NodePointer[this.Format.DoubleOutputs];
            this.NumberConstants = new List<long>();
            this.BoolConstants = new List<bool>();
            this.DoubleConstants = new List<double>();
            this.NumberOperators = new List<OperatorNode<long>>();
            this.BoolOperators = new List<OperatorNode<bool>>();
            this.DoubleOperators = new List<OperatorNode<double>>();
            for (int i = 0; i < this.Format.NumberOutputs; i++)
            {
                NumberOutputNodes[i] = this.GetNumberNode(false, NodePointer.Empty);
            }
            for (int i = 0; i < this.Format.BoolOutputs; i++)
            {
                BoolOutputNodes[i] = this.GetBoolNode(false, NodePointer.Empty);
            }
        }

        private Genome()
        {

        }

        public object Clone()
        {
            Genome clone = new Genome();
            clone.NumberOutputNodes = this.NumberOutputNodes.Clone<NodePointer>();
            clone.BoolOutputNodes = this.BoolOutputNodes.Clone<NodePointer>();
            clone.NumberOfNumberInputs = this.NumberOfNumberInputs;
            clone.NumberOfBoolInputs = this.NumberOfBoolInputs;
            clone.NumberConstants = this.NumberConstants.Clone();
            clone.BoolConstants = this.BoolConstants.Clone();
            clone.NumberOperators = this.NumberOperators.Clone();
            clone.BoolOperators = this.BoolOperators.Clone();
            clone.R = this.R;
            return clone;
        }

        private List<NodePointer> GetDependencies(NodePointer node)
        {
            List<NodePointer> dependencies = new List<NodePointer>();
            if (node.Type == NodeType.Operator)
            {
                NodePointer[] inputValues;
                if (node.IsNumber)
                {
                    inputValues = this.NumberOperators[node.Index].InputValues;
                }
                else
                {
                    inputValues = this.BoolOperators[node.Index].InputValues;
                }
                foreach (NodePointer inputValue in inputValues)
                {
                    dependencies.AddRange(this.GetDependencies(inputValue));
                }
            }
            dependencies.Add(node);
            return dependencies;
        }

        private List<NodePointer> GetAvalibleNumberNodes(bool dontDepend, NodePointer dontDependOn)
        {
            List<NodePointer> availiblePointers = new List<NodePointer>();
            for (int i = 0; i < this.NumberConstants.Count; i++)
            {
                NodePointer possibleNodePointer = new NodePointer();
                possibleNodePointer.Index = i;
                possibleNodePointer.Type = NodeType.Constant;
                possibleNodePointer.IsNumber = true;
                if (dontDepend)
                {
                    if (!this.GetDependencies(possibleNodePointer).ContainsValue(dontDependOn))
                    {
                        availiblePointers.Add(possibleNodePointer);
                    }
                }
                else
                {
                    availiblePointers.Add(possibleNodePointer);
                }
            }
            for (int i = 0; i < this.NumberOperators.Count; i++)
            {
                NodePointer possibleNodePointer = new NodePointer();
                possibleNodePointer.Index = i;
                possibleNodePointer.Type = NodeType.Operator;
                possibleNodePointer.IsNumber = true;
                if (dontDepend)
                {
                    if (!this.GetDependencies(possibleNodePointer).ContainsValue(dontDependOn))
                    {
                        availiblePointers.Add(possibleNodePointer);
                    }
                }
                else
                {
                    availiblePointers.Add(possibleNodePointer);
                }
            }
            for (int i = 0; i < this.NumberOfNumberInputs; i++)
            {
                NodePointer nodePointer = new NodePointer();
                nodePointer.Index = i;
                nodePointer.Type = NodeType.Input;
                nodePointer.IsNumber = true;
                availiblePointers.Add(nodePointer);
            }
            return availiblePointers;
        }

        private List<NodePointer> GetAvalibleBoolNodes(bool dontDepend, NodePointer dontDependOn)
        {
            List<NodePointer> availiblePointers = new List<NodePointer>();
            for (int i = 0; i < this.BoolConstants.Count; i++)
            {
                NodePointer possibleNodePointer = new NodePointer();
                possibleNodePointer.Index = i;
                possibleNodePointer.Type = NodeType.Constant;
                possibleNodePointer.IsNumber = false;
                if (dontDepend)
                {
                    if (!this.GetDependencies(possibleNodePointer).ContainsValue(dontDependOn))
                    {
                        availiblePointers.Add(possibleNodePointer);
                    }
                }
                else
                {
                    availiblePointers.Add(possibleNodePointer);
                }
            }
            for (int i = 0; i < this.BoolOperators.Count; i++)
            {
                NodePointer possibleNodePointer = new NodePointer();
                possibleNodePointer.Index = i;
                possibleNodePointer.Type = NodeType.Operator;
                possibleNodePointer.IsNumber = false;
                if (dontDepend)
                {
                    if (!this.GetDependencies(possibleNodePointer).ContainsValue(dontDependOn))
                    {
                        availiblePointers.Add(possibleNodePointer);
                    }
                }
                else
                {
                    availiblePointers.Add(possibleNodePointer);
                }
            }
            for (int i = 0; i < this.NumberOfBoolInputs; i++)
            {
                NodePointer nodePointer = new NodePointer();
                nodePointer.Index = i;
                nodePointer.Type = NodeType.Input;
                nodePointer.IsNumber = false;
                availiblePointers.Add(nodePointer);
            }
            return availiblePointers;
        }

        private NodePointer GetNumberNode(bool dontDepend, NodePointer dontDependOn)
        {
            if (this.R.Next(8) != 0)
            {
                List<NodePointer> availibleNodes = this.GetAvalibleNumberNodes(dontDepend, dontDependOn);
                if (availibleNodes.Count > 0)
                {
                    int selectedPointerIndex = R.Next(availibleNodes.Count);
                    return availibleNodes[selectedPointerIndex];
                }
            }
            if (this.R.Next(4) == 0)
            {
                this.NumberConstants.Add(R.NextLong());
                NodePointer pointer = new NodePointer();
                pointer.Type = NodeType.Constant;
                pointer.IsNumber = true;
                pointer.Index = this.NumberConstants.Count - 1;
                return pointer;
            }
            else
            {
                NumberOperatorNode newOperator = new NumberOperatorNode();
                int randomNumber = this.R.Next(5);
                switch (randomNumber)
                {
                    case (int)NumberOperatorType.Add:
                        newOperator.InputValues = new NodePointer[2];
                        newOperator.InputValues[0] = this.GetNumberNode(dontDepend, dontDependOn);
                        newOperator.InputValues[1] = this.GetNumberNode(dontDepend, dontDependOn);
                        break;
                    case (int)NumberOperatorType.Subtract:
                        newOperator.InputValues = new NodePointer[2];
                        newOperator.InputValues[0] = this.GetNumberNode(dontDepend, dontDependOn);
                        newOperator.InputValues[1] = this.GetNumberNode(dontDepend, dontDependOn);
                        break;
                    case (int)NumberOperatorType.Multiply:
                        newOperator.InputValues = new NodePointer[2];
                        newOperator.InputValues[0] = this.GetNumberNode(dontDepend, dontDependOn);
                        newOperator.InputValues[1] = this.GetNumberNode(dontDepend, dontDependOn);
                        break;
                    case (int)NumberOperatorType.Divide:
                        newOperator.InputValues = new NodePointer[2];
                        newOperator.InputValues[0] = this.GetNumberNode(dontDepend, dontDependOn);
                        newOperator.InputValues[1] = this.GetNumberNode(dontDepend, dontDependOn);
                        break;
                    case (int)NumberOperatorType.If:
                        newOperator.InputValues = new NodePointer[3];
                        newOperator.InputValues[0] = this.GetBoolNode(dontDepend, dontDependOn);
                        newOperator.InputValues[1] = this.GetNumberNode(dontDepend, dontDependOn);
                        newOperator.InputValues[2] = this.GetNumberNode(dontDepend, dontDependOn);
                        break;
                }
                newOperator.Type = (NumberOperatorType)randomNumber;
                this.NumberOperators.Add(newOperator);
                NodePointer pointer = new NodePointer();
                pointer.Index = this.NumberOperators.Count - 1;
                pointer.Type = NodeType.Operator;
                pointer.IsNumber = true;
                return pointer;
            }
        }

        private NodePointer GetBoolNode(bool dontDepend, NodePointer dontDependOn)
        {
            if (this.R.Next(8) != 0)
            {
                List<NodePointer> availibleNodes = this.GetAvalibleBoolNodes(dontDepend, dontDependOn);
                if (availibleNodes.Count > 0)
                {
                    int selectedPointerIndex = R.Next(availibleNodes.Count);
                    return availibleNodes[selectedPointerIndex];
                }
            }
            if (this.R.Next(4) == 0)
            {
                this.BoolConstants.Add(R.Next(2) == 1);
                NodePointer pointer = new NodePointer();
                pointer.Type = NodeType.Constant;
                pointer.IsNumber = false;
                pointer.Index = this.BoolConstants.Count - 1;
                return pointer;
            }
            else
            {
                BoolOperatorNode newOperator = new BoolOperatorNode();
                int randomNumber = this.R.Next(6);
                switch (randomNumber)
                {
                    case (int)BoolOperatorType.AND:
                        newOperator.InputValues = new NodePointer[2];
                        newOperator.InputValues[0] = this.GetBoolNode(dontDepend, dontDependOn);
                        newOperator.InputValues[1] = this.GetBoolNode(dontDepend, dontDependOn);
                        break;
                    case (int)BoolOperatorType.NOT:
                        newOperator.InputValues = new NodePointer[1];
                        newOperator.InputValues[0] = this.GetBoolNode(dontDepend, dontDependOn);
                        break;
                    case (int)BoolOperatorType.OR:
                        newOperator.InputValues = new NodePointer[2];
                        newOperator.InputValues[0] = this.GetBoolNode(dontDepend, dontDependOn);
                        newOperator.InputValues[1] = this.GetBoolNode(dontDepend, dontDependOn);
                        break;
                    case (int)BoolOperatorType.XOR:
                        newOperator.InputValues = new NodePointer[2];
                        newOperator.InputValues[0] = this.GetBoolNode(dontDepend, dontDependOn);
                        newOperator.InputValues[1] = this.GetBoolNode(dontDepend, dontDependOn);
                        break;
                    case (int)BoolOperatorType.BiggerThan:
                        newOperator.InputValues = new NodePointer[2];
                        newOperator.InputValues[0] = this.GetNumberNode(dontDepend, dontDependOn);
                        newOperator.InputValues[1] = this.GetNumberNode(dontDepend, dontDependOn);
                        break;
                    case (int)BoolOperatorType.Equals:
                        newOperator.InputValues = new NodePointer[2];
                        newOperator.InputValues[0] = this.GetNumberNode(dontDepend, dontDependOn);
                        newOperator.InputValues[1] = this.GetNumberNode(dontDepend, dontDependOn);
                        break;
                }
                newOperator.Type = (BoolOperatorType)randomNumber;
                this.BoolOperators.Add(newOperator);
                NodePointer pointer = new NodePointer();
                pointer.Index = this.BoolOperators.Count - 1;
                pointer.Type = NodeType.Operator;
                pointer.IsNumber = false;
                return pointer;
            }
        }

        public void PushInputs(bool[] boolInputs, long[] numberInputs)
        {
            if (this.NumberOfBoolInputs == boolInputs.Length && this.NumberOfNumberInputs == numberInputs.Length)
            {
                this.BoolInputsValues = boolInputs;
                this.NumberInputsValues = numberInputs;
                this.InputsPushed = true;
            }
            else
            {
                throw new Exception("Leght of the input arrays doesn't match number of Genome inputs.");
            }
        }

        private void MutateNode(NodePointer node)
        {
            if (node.Type == NodeType.Operator)
            {
                if (node.IsNumber)
                {
                    for (int i = 0; i < this.NumberOperators[node.Index].InputValues.Length; i++)
                    {
                        if (this.R.Next(50) == 0)
                        {
                            NodePointer newNode;
                            if (this.NumberOperators[node.Index].InputValues[i].IsNumber)
                            {
                                newNode =  this.GetNumberNode(true, node);
                            }
                            else
                            {
                                newNode = this.GetBoolNode(true, node);
                            }
                            this.NumberOperators[node.Index].InputValues[i] = newNode;
                        }
                        else
                        {
                            this.MutateNode(this.NumberOperators[node.Index].InputValues[i]);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < this.BoolOperators[node.Index].InputValues.Length; i++)
                    {
                        if (this.R.Next(50) == 0)
                        {
                            NodePointer newNode;
                            if (this.BoolOperators[node.Index].InputValues[i].IsNumber)
                            {
                                newNode = this.GetNumberNode(true, node);
                            }
                            else
                            {
                                newNode = this.GetBoolNode(true, node);
                            }
                            this.BoolOperators[node.Index].InputValues[i] = newNode;
                        }
                        else
                        {
                            this.MutateNode(this.BoolOperators[node.Index].InputValues[i]);
                        }
                    }
                }
            }
        }

        public Genome GetMutation()
        {
            Genome mutatedClone = (Genome)this.Clone();
            for (int i = 0; i < mutatedClone.NumberOutputNodes.Length; i++)
            {
                if (this.R.Next(50) == 0)
                {
                    mutatedClone.NumberOutputNodes[i] = mutatedClone.GetNumberNode(false, NodePointer.Empty);
                }
                else
                {
                    mutatedClone.MutateNode(mutatedClone.NumberOutputNodes[i]);
                }
            }
            for (int i = 0; i < mutatedClone.BoolOutputNodes.Length; i++)
            {
                if (this.R.Next(50) == 0)
                {
                    mutatedClone.BoolOutputNodes[i] = mutatedClone.GetBoolNode(false, NodePointer.Empty);
                }
                else
                {
                    mutatedClone.MutateNode(mutatedClone.BoolOutputNodes[i]);
                }
            }
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
                BoolOperatorNode boolOperator = this.BoolOperators[node.Index];
                switch (boolOperator.Type)
                {
                    case BoolOperatorType.AND:
                        return this.GetBoolNodeValue(boolOperator.InputValues[0]) && this.GetBoolNodeValue(boolOperator.InputValues[1]);
                    case BoolOperatorType.NOT:
                        return !this.GetBoolNodeValue(boolOperator.InputValues[0]);
                    case BoolOperatorType.OR:
                        return this.GetBoolNodeValue(boolOperator.InputValues[0]) || this.GetBoolNodeValue(boolOperator.InputValues[1]);
                    case BoolOperatorType.XOR:
                        return this.GetBoolNodeValue(boolOperator.InputValues[0]) ^ this.GetBoolNodeValue(boolOperator.InputValues[1]);
                    case BoolOperatorType.BiggerThan:
                        return this.GetNumberNodeValue(boolOperator.InputValues[0]) > this.GetNumberNodeValue(boolOperator.InputValues[1]);
                    case BoolOperatorType.Equals:
                        return this.GetNumberNodeValue(boolOperator.InputValues[0]) == this.GetNumberNodeValue(boolOperator.InputValues[1]);
                    default:
                        throw new ArithmeticException("Switch overflow.");
                }
            }
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
                NumberOperatorNode numberOperator = this.NumberOperators[node.Index];
                switch (numberOperator.Type)
                {
                    case NumberOperatorType.Add:
                        return this.GetNumberNodeValue(numberOperator.InputValues[0]) + this.GetNumberNodeValue(numberOperator.InputValues[1]);
                    case NumberOperatorType.Subtract:
                        return this.GetNumberNodeValue(numberOperator.InputValues[0]) - this.GetNumberNodeValue(numberOperator.InputValues[1]);
                    case NumberOperatorType.Multiply:
                        return this.GetNumberNodeValue(numberOperator.InputValues[0]) * this.GetNumberNodeValue(numberOperator.InputValues[1]);
                    case NumberOperatorType.Divide:
                        long dividend = this.GetNumberNodeValue(numberOperator.InputValues[0]);
                        long divisor = this.GetNumberNodeValue(numberOperator.InputValues[1]);
                        if (divisor == 0)
                        {
                            if (dividend == 0)
                            {
                                return 1;
                            }
                            else
                            {
                                return long.MaxValue;
                            }
                        }
                        else
                        {
                            return dividend / divisor;
                        }
                    case NumberOperatorType.If:
                        if (this.GetBoolNodeValue(numberOperator.InputValues[0]))
                        {
                            return this.GetNumberNodeValue(numberOperator.InputValues[1]);
                        }
                        else
                        {
                            return this.GetNumberNodeValue(numberOperator.InputValues[2]);
                        }
                    default:
                        throw new ArithmeticException("Switch overflow.");
                }
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
