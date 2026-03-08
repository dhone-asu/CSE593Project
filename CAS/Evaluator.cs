using CAS.SymbolManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
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
            { Operator.Eq, "="  },
            { Operator.Neg, "-" },
            { Operator.Assign, ":=" }
        };

        ExprNode tree;
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

        public static string PrintExpression(ExprNode node)
        {
            if (node == null) return "";
            if (node is OpNode)
            {
                var opNode = (OpNode)node;
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

                    if ((leftPrec > 0 && leftPrec < treePrec) ||
                        (leftPrec == treePrec && opNode.Operator == Operator.Pow))
                        leftExpr = $"({leftExpr})";
                }
                else
                    leftExpr = PrintExpression(leftNode);
                if (rightNode is OpNode)
                {
                    rightPrec = Parser.PrecedenceMap[((OpNode)rightNode).Operator];
                    rightExpr = PrintExpression(rightNode);

                    if ((rightPrec > 0 && rightPrec < treePrec) || 
                        (rightPrec == treePrec && opNode.Operator == Operator.Minus) ||
                        (rightPrec == treePrec && opNode.Operator == Operator.Div))
                        rightExpr = $"({rightExpr})";
                }
                else if (rightNode != null)
                    rightExpr = PrintExpression(rightNode);



                if (!string.IsNullOrEmpty(rightExpr))
                {
                    string s = null;
                    if (opNode.Operator == Operator.Plus || opNode.Operator == Operator.Minus)
                        s = $"{leftExpr} {OperatorToStringMap[opNode.Operator]} {rightExpr}";
                    else
                        s = $"{leftExpr}{OperatorToStringMap[opNode.Operator]}{rightExpr}";
                    return s;
                }
                else
                {
                    string s = $"{OperatorToStringMap[opNode.Operator]}{leftExpr}";
                    return s;
                }
            }
            else if (node is AtomNode)
            {
                var atomNode = (AtomNode)node;
                string s = atomNode.Data.ToString();
                return s;
            }
            else if (node is FunctionNode)
            {
                var funcNode = (FunctionNode)node;
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

        public static double Compute(Operator op, double a, double b)
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
                case Operator.Pow:
                    return Math.Pow(a, b);
                case Operator.Neg:
                    return -a;
                default:
                    throw new Exception($"Operator {op} not supported for evaluation");
            }
        }



        private bool ParametersAreIdentifiers(FunctionNode node)
        {
            if (node == null) return false;

            foreach (var item in node.Params)
            {
                if (!(item is AtomNode && ((AtomNode)item).Type == AtomType.Identifier))
                    return false;
            }

            return true;
        }

        private string[] GetParamNames(FunctionNode node)
        {
            var list = new List<string>();

            foreach (var item in node.Params)
            {
                if (!(item is AtomNode && ((AtomNode)item).Type == AtomType.Identifier))
                    throw new EvaluateException("Parameters must be identifier to create user function");
                var atomNode = (AtomNode)item;
                list.Add((string)atomNode.Data);
            }

            return list.ToArray();
        }

        public static ExprNode Evaluate(ExprNode expression)
        {
            var evaluator = new Evaluator(expression);
            return evaluator.Evaluate();
        }

        public ExprNode Evaluate()
        {
            if ((tree is OpNode) && ((OpNode)tree).Operator == Operator.Assign)
            {
                OpNode node = (OpNode)tree;

                Assign(node.Left, node.Right);
                return tree;
            }
            /*else if ((tree is OpNode) && ((OpNode)tree).Operator == Operator.Eq)
            {
                OpNode node = (OpNode)tree;
                var lhs = node.Left;
                var rhs = node.Right;

                return new OpNode
                {
                    Operator = Operator.Eq,
                    Left = Evaluate(lhs),
                    Right = Evaluate(rhs)
                };
            }*/
            else
            {
                return EvaluateNode(tree);
            }
        }

        public static ExprNode Substitute(ExprNode expression, params object[] args)
        {
            if (expression == null) return null;
            expression = (ExprNode)expression.Clone();
            var varNames = new List<string>();
            var substitutions = new List<ExprNode>();

            if (args.Length % 2 != 0)
                throw new EvaluateException("Parameters must be paired");

            for (int i = 0; i < args.Length; i += 2)
            {
                if (!(args[i] is string && args[i + 1] is ExprNode))
                    throw new EvaluateException("Parameters must be alternating string and ExprNode");
                varNames.Add((string)args[i]);
                substitutions.Add((ExprNode)args[i + 1]);
            }

            if (expression is OpNode)
            {
                OpNode opNode = (OpNode)expression;

                opNode.Left = Substitute(opNode.Left, args);
                opNode.Right = Substitute(opNode.Right, args);

                return opNode;
            }
            else if (expression is FunctionNode)
            {
                FunctionNode functionNode = (FunctionNode)expression;

                if (SessionManager.Default.FunctionExists(functionNode.Name))
                {
                    var function = SessionManager.Default.GetFunction(functionNode.Name);

                    for (int i = 0; i < functionNode.Params.Count; i++)
                    {
                        if (function.ParameterTypes[i] == ParamType.Args)
                            break;
                        if (function.ParameterTypes[i] == ParamType.Expression)
                            functionNode.Params[i] = Substitute(functionNode.Params[i], args);
                    }
                    //var function = SessionManager.Default.
                }

                return functionNode;
            }
            else if (expression is AtomNode)
            {
                AtomNode atomNode = (AtomNode)expression;

                if ((atomNode.Type == AtomType.Identifier) && (varNames.Contains((string)atomNode.Data)))
                {
                    int i = varNames.IndexOf((string)atomNode.Data);
                    return substitutions[i];
                }
                else
                    return expression;
            }
            else
                throw new EvaluatorException("Unknown expression node type");
        }



        public void Assign(ExprNode left,  ExprNode right)
        {
            if (left is AtomNode && ((AtomNode)left).Type == AtomType.Identifier)
            {
                SessionManager.Default.SetVariable((string)((AtomNode)left).Data, right);
            }
            else if (left is FunctionNode && ParametersAreIdentifiers((FunctionNode)left))
            {
                UserFunction userFunction = new UserFunction();
                userFunction.Expression = right;
                userFunction.ParameterNames = GetParamNames((FunctionNode)left);
                var parameters = from name in userFunction.ParameterNames select ParamType.Expression;
                userFunction.ParameterTypes = parameters.ToArray();
                userFunction.ReturnType = ParamType.Expression;
                SessionManager.Default.SetFunction((string)((FunctionNode)left).Name, userFunction);
            }
            else
                throw new EvaluatorException("Left-hand side must be variable or function identifier");
        }


        private ExprNode EvaluateNode(ExprNode node)
        {
            if (node == null) return null;
            if (node is OpNode)
            {
                var opNode = (OpNode)node;
                var leftNode = opNode.Left;
                var rightNode = opNode.Right;

                if (rightNode != null)
                {
                    ExprNode leftResult = EvaluateNode(leftNode);
                    ExprNode rightResult = EvaluateNode(rightNode);
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
                    ExprNode leftResult = EvaluateNode(leftNode);

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
            else if (node is AtomNode)
            {
                AtomNode atomNode = (AtomNode)node;

                if (atomNode.Type == AtomType.Identifier)
                {
                    var variable = SessionManager.Default.GetVariable((string)atomNode.Data);

                    if (variable != null)
                    {
                        return variable.Value;
                    }
                    else
                        return atomNode;
                }
                else
                    return atomNode;
            }
            else if (node is FunctionNode)
            {
                FunctionNode functionNode = (FunctionNode)node;
                object[] parameters = functionNode.Params.ToArray();

                if (SessionManager.Default.FunctionExists(functionNode.Name))
                {
                    IFunction function = SessionManager.Default.GetFunction(functionNode.Name);

                    for (int i = 0; i < parameters.Length; i++)
                    {
                        if (function.ParameterTypes[i] == ParamType.Args)
                            break;
                        if (function.ParameterTypes[i] == ParamType.Expression)
                            parameters[i] = Evaluate((ExprNode)parameters[i]);
                    }

                    object result = function.Call(parameters);

                    return (ExprNode)result;
                }
                else
                {
                    return new FunctionNode
                    {
                        Name = functionNode.Name,
                        Params = new List<ExprNode>((ExprNode[])parameters)
                    };
                }
            }
            else
                throw new Exception($"Node type '{node.GetType()}' not supported for evaluation");
        }

        public override string ToString()
        {
            return PrintExpression(tree);
        }
    }
}
