using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAS.SymbolManagement
{
    public class ParameterException : Exception
    {
        public ParameterException(string message) : base(message) { }
        public ParameterException() : base() { }
    }
    public class NativeFunction : IFunction
    {
        public static ParamType GetValueType(ExprNode tree)
        {
            if (tree is AtomNode)
            {
                AtomNode atom = (AtomNode)tree;

                if (atom.Type == AtomType.Numeral)
                    return ParamType.Numeral;
                else
                    return ParamType.Variable;
            }
            else
                return ParamType.Expression;
        }
        public string Name { get; set; }
        public GenericFunction Delegate { get; set; }
        public ParamType ReturnType { get; set; }
        public ParamType[] ParameterTypes { get; set; }

        public bool CheckParameterTypes(object[] parameters)
        {
            if (ParameterTypes[ParameterTypes.Length - 1] == ParamType.Args)
            {
                if (parameters.Length < ParameterTypes.Length - 1)
                    return false;
            }
            else if (ParameterTypes.Length != parameters.Length)
                return false;

            for (int i = 0; i < parameters.Length; i++)
            {
                if (ParameterTypes[i] == ParamType.Args)
                    break;
                if (!(parameters[i] is ExprNode))
                    return false;

                ParamType type = GetValueType((ExprNode)parameters[i]);

                // Variable or Numeral may act as an Expression
                if (ParameterTypes[i] == ParamType.Expression && (type == ParamType.Variable || type == ParamType.Numeral))
                    continue;
                else if (ParameterTypes[i] == type)
                    continue;
                else
                    return false;
            }

            return true;
        }

        public object Call(object[] parameters)
        {
            if (CheckParameterTypes(parameters))
            {
                return Delegate(parameters);
            }
            else
            {
                throw new ParameterException("Parameter mismatch");
            }
        }
    }
    public class NativeFunctionManager : IFunctionManager
    {
        public SessionManager SessionManager { get; set; }
        private Dictionary<string, NativeFunction> functions { get; set; }

        private static object RunOneParamMathFunction(string name, Func<double, double> func, object[] parameters)
        {
            var param0 = (ExprNode)parameters[0];
            ParamType type = NativeFunction.GetValueType((ExprNode)param0);


            if (type == ParamType.Numeral)
            {
                var a = (double)((AtomNode)param0).Data;
                return new AtomNode
                {
                    Data = func(a)
                };
            }
            else return new FunctionNode
            {
                Name = name,
                Params = new List<ExprNode> { (ExprNode)param0 }
            };
        }

        private static object RunTwoParamMathFunction(string name, Func<double, double, double> func, object[] parameters)
        {
            var param0 = (ExprNode)parameters[0];
            var param1 = (ExprNode)parameters[1];
            ParamType type0 = NativeFunction.GetValueType((ExprNode)param0);
            ParamType type1 = NativeFunction.GetValueType((ExprNode)param1);


            if ((type0 == ParamType.Numeral) && (type1 == ParamType.Numeral))
            {
                var a = (double)((AtomNode)param0).Data;
                var b = (double)((AtomNode)param1).Data;
                return new AtomNode
                {
                    Data = func(a, b)
                };
            }
            else return new FunctionNode
            {
                Name = name,
                Params = new List<ExprNode> { (ExprNode)param0, (ExprNode)param1 }
            };
        }

        public static object Sin(object[] parameters)
        {
            return RunOneParamMathFunction("sin", Math.Sin, parameters);
        }

        public static object Cos(object[] parameters)
        {
            return RunOneParamMathFunction("cos", Math.Cos, parameters);
        }

        public static object Tan(object[] parameters)
        {
            return RunOneParamMathFunction("tan", Math.Tan, parameters);
        }
        public static object Asin(object[] parameters)
        {
            return RunOneParamMathFunction("asin", Math.Asin, parameters);
        }

        public static object Acos(object[] parameters)
        {
            return RunOneParamMathFunction("acos", Math.Acos, parameters);
        }
        public static object Atan(object[] parameters)
        {
            return RunOneParamMathFunction("atan", Math.Atan, parameters);
        }
        public static object Sinh(object[] parameters)
        {
            return RunOneParamMathFunction("sinh", Math.Sinh, parameters);
        }
        public static object Cosh(object[] parameters)
        {
            return RunOneParamMathFunction("cosh", Math.Cosh, parameters);
        }
        public static object Tanh(object[] parameters)
        {
            return RunOneParamMathFunction("tanh", Math.Tanh, parameters);
        }
        public static object Asinh(object[] parameters)
        {
            return RunOneParamMathFunction("asinh", ExtendedMath.Asinh, parameters);
        }
        public static object Acosh(object[] parameters)
        {
            return RunOneParamMathFunction("acosh", ExtendedMath.Acosh, parameters);
        }
        public static object Atanh(object[] parameters)
        {
            return RunOneParamMathFunction("atanh", ExtendedMath.Atanh, parameters);
        }
        public static object Pow(object[] parameters)
        {
            return RunTwoParamMathFunction("pow", Math.Pow, parameters);
        }

        public static object Exp(object[] parameters)
        {
            return RunOneParamMathFunction("exp", Math.Exp, parameters);
        }

        public static object Log(object[] parameters)
        {
            return RunOneParamMathFunction("log", Math.Log, parameters);
        }

        public static object Sqrt(object[] parameters)
        {
            return RunOneParamMathFunction("sqrt", Math.Sqrt, parameters);
        }

        public static object Fact(object[] parameters)
        {
            return RunOneParamMathFunction("fact", ExtendedMath.Fact, parameters);
        }
        public static object Exit(object[] parameters)
        {
            Environment.Exit(0);
            return null;
        }

        public static object Subst(object[] parameters)
        {
            ExprNode expression = (ExprNode)parameters[0];
            var substParams = new List<object>();

            for (int i = 1; i < parameters.Length; i += 2)
            {
                var symbol = (AtomNode)parameters[i];
                var value = (ExprNode)parameters[i + 1];
                substParams.Add(symbol.Data);
                substParams.Add(value);
            }

            var result = Evaluator.Substitute(expression, substParams.ToArray());

            var evaluator = new Evaluator(result);
            return evaluator.Evaluate();
        }

        public bool FunctionExists(string name)
        {
            return functions.ContainsKey(name);
        }

        public IFunction GetFunction(string name)
        {
            return functions[name];
        }

        public NativeFunctionManager()
        {
            functions = new Dictionary<string, NativeFunction>();
            functions["sin"] = new NativeFunction
            {
                Name = "sin",
                Delegate = Sin,
                ParameterTypes = new ParamType[] { ParamType.Expression },
                ReturnType = ParamType.Expression
            };
            functions["cos"] = new NativeFunction
            {
                Name = "cos",
                Delegate = Cos,
                ParameterTypes = new ParamType[] { ParamType.Expression },
                ReturnType = ParamType.Expression
            };
            functions["tan"] = new NativeFunction
            {
                Name = "tan",
                Delegate = Tan,
                ParameterTypes = new ParamType[] { ParamType.Expression },
                ReturnType = ParamType.Expression
            };
            functions["asin"] = new NativeFunction
            {
                Name = "asin",
                Delegate = Asin,
                ParameterTypes = new ParamType[] { ParamType.Expression },
                ReturnType = ParamType.Expression
            };
            functions["acos"] = new NativeFunction
            {
                Name = "acos",
                Delegate = Acos,
                ParameterTypes = new ParamType[] { ParamType.Expression },
                ReturnType = ParamType.Expression
            };
            functions["atan"] = new NativeFunction
            {
                Name = "atan",
                Delegate = Atan,
                ParameterTypes = new ParamType[] { ParamType.Expression },
                ReturnType = ParamType.Expression
            };
            functions["sinh"] = new NativeFunction
            {
                Name = "sinh",
                Delegate = Sinh,
                ParameterTypes = new ParamType[] { ParamType.Expression },
                ReturnType = ParamType.Expression
            };
            functions["cosh"] = new NativeFunction
            {
                Name = "cosh",
                Delegate = Cosh,
                ParameterTypes = new ParamType[] { ParamType.Expression },
                ReturnType = ParamType.Expression
            };
            functions["tanh"] = new NativeFunction
            {
                Name = "tanh",
                Delegate = Tanh,
                ParameterTypes = new ParamType[] { ParamType.Expression },
                ReturnType = ParamType.Expression
            };
            functions["asinh"] = new NativeFunction
            {
                Name = "asinh",
                Delegate = Asinh,
                ParameterTypes = new ParamType[] { ParamType.Expression },
                ReturnType = ParamType.Expression
            };
            functions["acosh"] = new NativeFunction
            {
                Name = "acosh",
                Delegate = Acosh,
                ParameterTypes = new ParamType[] { ParamType.Expression },
                ReturnType = ParamType.Expression
            };
            functions["atanh"] = new NativeFunction
            {
                Name = "atanh",
                Delegate = Atanh,
                ParameterTypes = new ParamType[] { ParamType.Expression },
                ReturnType = ParamType.Expression
            };
            functions["pow"] = new NativeFunction
            {
                Name = "pow",
                Delegate = Pow,
                ParameterTypes = new ParamType[] { ParamType.Expression, ParamType.Expression },
                ReturnType = ParamType.Expression
            };
            functions["exp"] = new NativeFunction
            {
                Name = "exp",
                Delegate = Exp,
                ParameterTypes = new ParamType[] { ParamType.Expression },
                ReturnType = ParamType.Expression
            };
            functions["log"] = new NativeFunction
            {
                Name = "log",
                Delegate = Log,
                ParameterTypes = new ParamType[] { ParamType.Expression },
                ReturnType = ParamType.Expression
            };
            functions["ln"] = new NativeFunction
            {
                Name = "ln",
                Delegate = Log,
                ParameterTypes = new ParamType[] { ParamType.Expression },
                ReturnType = ParamType.Expression
            };
            functions["sqrt"] = new NativeFunction
            {
                Name = "sqrt",
                Delegate = Sqrt,
                ParameterTypes = new ParamType[] { ParamType.Expression },
                ReturnType = ParamType.Expression
            };

            functions["fact"] = new NativeFunction
            {
                Name = "fact",
                Delegate = Fact,
                ParameterTypes = new ParamType[] { ParamType.Expression },
                ReturnType = ParamType.Expression
            };

            functions["exit"] = new NativeFunction
            {
                Name = "exit",
                Delegate = Exit,
                ParameterTypes = new ParamType[] { },
                ReturnType = ParamType.None
            };

            functions["subst"] = new NativeFunction
            {
                Name = "subst",
                Delegate = Subst,
                ParameterTypes = new ParamType[] { ParamType.Expression, ParamType.Args },
                ReturnType = ParamType.Expression
            };
        }
    }
}
