using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CAS.SymbolManagement;

namespace CAS
{
    public enum AtomType { Numeral, Identifier }


    public class ExprNode : ICloneable, IEquatable<ExprNode>
    {
        public static readonly AtomNode One = new AtomNode
        {
            Data = 1.0,
            Type = AtomType.Numeral
        };

        public static readonly AtomNode NegativeOne = new AtomNode
        {
            Data = -1.0,
            Type = AtomType.Numeral
        };

        public static readonly AtomNode Zero = new AtomNode
        {
            Data = 0.0,
            Type = AtomType.Numeral
        };

        public virtual List<string> GetVariableNames()
        {
            return new List<string>();
        }

        public virtual object Clone()
        {
            return new ExprNode();
        }

        public virtual bool Equals(ExprNode a)
        {
            return (object)a != null;
        }

        public static bool operator==(ExprNode a, ExprNode b)
        {
            if ((object)a == null)
                return (object)b == null;
            else
                return a.Equals(b); 
        }

        public static bool operator !=(ExprNode a, ExprNode b)
        {
            if ((object)a == null)
                return (object)b != null;
            else
                return !a.Equals(b);
        }

        public static ExprNode operator ^(ExprNode a, ExprNode b)
        {
            return new OpNode
            {
                Operator = Operator.Pow,
                Left = a,
                Right = b
            };
        }

        public static ExprNode operator -(ExprNode node)
        {
            if (node is AtomNode && ((AtomNode)node).Type == AtomType.Numeral)
            {
                var result = (AtomNode)node.Clone();
                result.Data = -(double)result.Data;
                return result;
            }
            else
                return new OpNode
                {
                    Operator = Operator.Neg,
                    Left = node
                };
        }

        public static ExprNode operator/(ExprNode a, ExprNode b)
        {
            return new OpNode
            {
                Operator = Operator.Div,
                Left = a,
                Right = b
            };
        }

        public static ExprNode operator *(ExprNode a, ExprNode b)
        {
            return new OpNode
            {
                Operator = Operator.Mult,
                Left = a,
                Right = b
            };
        }

        public static ExprNode operator+(ExprNode a, ExprNode b)
        {
            return new OpNode
            {
                Operator = Operator.Plus,
                Left = a,
                Right = b
            };
        }

        public static ExprNode operator-(ExprNode a, ExprNode b)
        {
            return new OpNode
            {
                Operator = Operator.Minus,
                Left = a,
                Right = b
            };
        }

            
    }
    public class OpNode: ExprNode
    {
        public Operator Operator { get; set; }
        public ExprNode Left { get; set; }
        public ExprNode Right { get; set; }

        public override object Clone()
        {
            return new OpNode
            {
                Operator = Operator,
                Left = (ExprNode)Left?.Clone(),
                Right = (ExprNode)Right?.Clone(),
            };
        }

        public override bool Equals(ExprNode a)
        {
            if (a is OpNode)
            {
                var node = (OpNode)a;

                return (Operator == node.Operator && Left.Equals(node.Left) && Right.Equals(node.Right));
            }
            else
                return false;
        }

        public override List<string> GetVariableNames()
        {
            var list = Left.GetVariableNames();
            var rightList = Right?.GetVariableNames();

            if (rightList != null)
                list.AddRange(rightList);
            return list;
        }
    }

    public class FunctionNode : ExprNode
    {
        public string Name { get; set; }
        public List<ExprNode> Params { get; set; } = new List<ExprNode>();

        public override object Clone()
        {
            var newParams = new List<ExprNode>();

            foreach (var param in Params)
            {
                newParams.Add((ExprNode)param.Clone());
            }

            return new FunctionNode
            {
                Name = Name,
                Params = newParams
            };
        }

        public override bool Equals(ExprNode a)
        {
            if (a is FunctionNode)
            {
                var node = (FunctionNode)a;

                if (Name != node.Name || Params.Count != node.Params.Count)
                    return false;

                for (int i = 0; i < Params.Count; i++)
                {
                    if (!Params[i].Equals(node.Params[i]))
                        return false;
                }
                return true;
                
            }
            else return false;
        }

        public override List<string> GetVariableNames()
        {
            var list = new List<string>();

            foreach (var item in Params)
            {
                var paramList = item.GetVariableNames();
                list.AddRange(paramList);
            }

            return list;
        }

        public ExprNode Derivative(string variable)
        {
            if (SessionManager.Default.FunctionExists(Name))
            {
                IFunction function = SessionManager.Default.GetFunction(Name);
                return function.Derivative(this,variable);
            }
            else
                return new FunctionNode
                {
                    Name = "deriv",
                    Params =
                    {
                        this,
                        new AtomNode
                        {
                            Type = AtomType.Identifier,
                            Data = variable
                        }
                    }
                };
        }
    }

    public class AtomNode : ExprNode
    {        
        public object Data { get; set; }
        public AtomType Type { get; set; }

        public override object Clone()
        {
            return new AtomNode
            {
                Data = Data,
                Type = Type
            };
        }

        public override bool Equals(ExprNode a)
        {
            if (a is AtomNode)
            {
                var node = (AtomNode)a;

                if (Type != node.Type) return false;

                if (Type == AtomType.Numeral)
                    return ((double)Data) == ((double)node.Data);
                else
                    return ((string)Data) == ((string)node.Data); 
            }
            else return false;
        }

        public override List<string> GetVariableNames()
        {
            var list = new List<string>();

            if (Type == AtomType.Identifier)
            {
                list.Add((string)Data);
            }

            return list;
        }
    }
}
