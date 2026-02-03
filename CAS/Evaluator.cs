using CAS.SymbolManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CAS
{
    public class EvaluatorException : Exception
    {
        public EvaluatorException(string message) : base(message) { }
        public EvaluatorException() : base() { }
    }

    public class Evaluator
    {
        public static readonly Dictionary<Operator, string> OperatorToStringMap = new Dictionary<Operator, string>
        {
            { Operator.None, "" },
            { Operator.Plus, "+" },
            { Operator.Minus, "-" },
            { Operator.Div, "/" },
            { Operator.Mult, "*" },
            { Operator.Pow, "^" },
            { Operator.Mod, "%" },
            { Operator.Eq, "=="  },
            { Operator.Neq, "!=" },
            { Operator.Gt, ">" },
            { Operator.Lt, "<" },
            { Operator.Gte, ">=" },
            { Operator.Lte, "<=" },
            { Operator.Neg, "-" },
            { Operator.Assign, "=" }
        };

        ExprNode tree;
        public SessionManager SessionManager { get; set; }
        public Evaluator(ExprNode expression) 
        {
            tree = expression;
        }

        public Evaluator(string expression)
        {
            StringBuffer stringBuffer = new StringBuffer(expression);
            Tokenizer tokenizer = new Tokenizer(stringBuffer);
            Parser parser = new Parser(tokenizer);
            tree = parser.ParseInput();
        }

        public ExprNode Tree { get { return tree; } }

        private string PrintExpression(ExprNode tree)
        {
            if (tree == null) return "";
            if (tree is OpNode)
            {
                var opNode = (OpNode)tree;
                var leftNode = opNode.Left;
                var rightNode = opNode.Right;
                int treePrec = Parser.PrecedenceMap[opNode.Operator];
                int leftPrec = 0, rightPrec = 0;
                string leftExpr = null;
                string rightExpr = null;

                if (leftNode is OpNode)
                {
                    leftPrec = Parser.PrecedenceMap[((OpNode)leftNode).Operator];

                    leftExpr = PrintExpression(leftNode);

                    if (leftPrec > 0 && leftPrec < treePrec)
                        leftExpr = $"({leftExpr})";
                }
                else
                    leftExpr = PrintExpression(leftNode);
                if (rightNode is OpNode)
                {
                    rightPrec = Parser.PrecedenceMap[((OpNode)rightNode).Operator];
                    rightExpr = PrintExpression(rightNode);

                    if (rightPrec > 0 && rightPrec < treePrec)
                        rightExpr = $"({rightExpr})";
                }
                else if (rightNode != null)
                    rightExpr = PrintExpression(rightNode);



                if (!string.IsNullOrEmpty(rightExpr))
                {
                    string s = $"{leftExpr} {OperatorToStringMap[opNode.Operator]} {rightExpr}";
                    return s;
                }
                else
                {
                    string s = $"{OperatorToStringMap[opNode.Operator]}{leftExpr}";
                    return s;
                }
            }
            else if (tree is AtomNode)
            {
                var atomNode = (AtomNode)tree;
                string s = atomNode.Data.ToString();
                return s;
            }
            else if (tree is FunctionNode)
            {
                var funcNode = (FunctionNode)tree;
                string parameters = "";

                if (funcNode.Params.Any())
                {
                    parameters = $"{PrintExpression(funcNode.Params[0])}";
                    for (int i = 1; i < funcNode.Params.Count; i++)
                    {
                        string param = PrintExpression(funcNode.Params[i]);
                        parameters = $"{parameters}, {param}";
                    }
                }
                string s = $"{funcNode.Name}({parameters})";
                return s;
            }
            else
                throw new EvaluatorException("Node type not supported.");
        }

        private double Compute(Operator op, double a, double b)
        {
            switch (op)
            {
                case Operator.Plus:
                    return a + b;
                case Operator.Minus:
                    return a - b;
                case Operator.Mult:
                    return a * b;
                case Operator.Div:
                    return a / b;
                case Operator.Mod:
                    return a % b;
                case Operator.Pow:
                    return Math.Pow(a, b);
                case Operator.Neg:
                    return -a;
                default:
                    throw new Exception($"Operator {op} not supported for evaluation");
            }
        }

        private IVariable GetVariable(string name)
        {
            if (SessionManager == null)
                return null;

            if (SessionManager.ConstantManager.VariableExists(name))
            {
                return SessionManager.ConstantManager.GetVariable(name);
            }
            else if (SessionManager.UserVariableManager.VariableExists(name))
            {
                return SessionManager.UserVariableManager.GetVariable(name);
            }
            else
                return null;
        }

        private void SetVariable(string name, ExprNode value)
        {
            if (SessionManager == null)
                return;

            if (SessionManager.ConstantManager.VariableExists(name))
                throw new EvaluatorException($"Cannot set variable '{name}' because it refers to a constant");

            var variable = new UserVariable
            {
                Name = name,
                Value = value
            };

            SessionManager.UserVariableManager.SetVariable(name, variable);
        }

        private IFunction GetFunction(string name)
        {
            if (SessionManager == null)
                return null;

            if (SessionManager.NativeFunctionManager.FunctionExists(name))
            {
                return SessionManager.NativeFunctionManager.GetFunction(name);
            }
            else
                return null;
        }

        private void SetFunction(string name, IFunction function)
        {
            if (SessionManager == null)
                return;

            if (SessionManager.NativeFunctionManager.FunctionExists(name))
                throw new EvaluatorException($"Cannot set function '{name}' because it refers to a native function.");

        }

        private bool ParametersAreIdentifiers(FunctionNode node)
        {
            if (node == null) return false;

            foreach (var item in node.Params)
            {
                if (!(item is AtomNode && ((AtomNode)item).Type != AtomType.Identifier))
                    return false;
            }

            return true;
        }


        public ExprNode Evaluate()
        {
            if ((tree is OpNode) && ((OpNode)tree).Operator == Operator.Assign)
            {
                OpNode node = (OpNode)tree;

                Assign(node.Left, node.Right);
                return tree;
            }
            else
            {
                return Eval(tree);
            }

        }

        public void Assign(ExprNode left,  ExprNode right)
        {
            if (left is AtomNode && ((AtomNode)left).Type == AtomType.Identifier)
            {
                SetVariable((string)((AtomNode)left).Data, right);
            }
            else if (left is FunctionNode && ParametersAreIdentifiers((FunctionNode)left))
            {
                UserFunction userFunction = new UserFunction();
                userFunction.Expression = right;
                SetFunction((string)((FunctionNode)left).Name, userFunction);
            }
            else
                throw new EvaluatorException("Left-hand side must be variable or function identifier");
        }


        private ExprNode Eval(ExprNode tree)
        {
            if (tree == null) return null;
            if (tree is OpNode)
            {
                var opNode = (OpNode)tree;
                var leftNode = opNode.Left;
                var rightNode = opNode.Right;

                if (rightNode != null)
                {
                    ExprNode leftResult = Eval(leftNode);
                    ExprNode rightResult = Eval(rightNode);
                    if ((leftResult is AtomNode && ((AtomNode)leftResult).Type == AtomType.Numeral) &&
                        (rightResult is AtomNode && ((AtomNode)rightResult).Type == AtomType.Numeral))
                    {
                        double leftVal = (double)((AtomNode)leftResult).Data, rightVal = (double)((AtomNode)rightResult).Data;
                        return new AtomNode
                        {
                            Type = AtomType.Numeral,
                            Data = Compute(opNode.Operator, leftVal, rightVal)
                        };
                    }
                    else
                        return new OpNode
                        {
                            Operator = opNode.Operator,
                            Left = leftResult,
                            Right = rightResult
                        };
                }
                else
                {
                    ExprNode leftResult = Eval(leftNode);

                    if (leftResult is AtomNode && ((AtomNode)leftResult).Type == AtomType.Numeral)
                    {
                        double leftVal = (double)((AtomNode)leftResult).Data;
                        return new AtomNode
                        {
                            Type = AtomType.Numeral,
                            Data = Compute(opNode.Operator, leftVal, 0)
                        };
                    }
                    else
                        return opNode;
                }
            }
            else if (tree is AtomNode)
            {
                AtomNode node = (AtomNode)tree;

                if (node.Type == AtomType.Identifier)
                {
                    var variable = GetVariable((string)node.Data);

                    if (variable != null)
                    {
                        return variable.Value;
                    }
                    else
                        return tree;
                }
                else
                    return tree;
            }
            else if (tree is FunctionNode)
            {
                FunctionNode functionNode = (FunctionNode)tree;
                object[] parameters = functionNode.Params.ToArray();
                for (int i = 0; i < parameters.Count(); i++)
                {
                    parameters[i] = Eval((ExprNode)parameters[i]);
                }

                IFunction function = GetFunction(functionNode.Name);

                if (function != null)
                {
                    object result = function.Call(parameters);

                    return (ExprNode)result;
                }
                else
                    return new FunctionNode
                    {
                        Name = functionNode.Name,
                        Params = new List<ExprNode>((ExprNode[])parameters)
                    };
            }
            else
                throw new Exception($"Node type '{tree.GetType()}' not supported for evaluation");
        }

        public override string ToString()
        {
            return PrintExpression(tree);
        }
    }
}
