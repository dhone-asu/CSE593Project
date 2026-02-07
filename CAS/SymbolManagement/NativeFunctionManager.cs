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
        public static ValueType GetValueType(ExprNode tree)
        {
            if (tree is AtomNode)
            {
                AtomNode atom = (AtomNode)tree;

                if (atom.Type == AtomType.Numeral)
                    return ValueType.Numeral;
                else
                    return ValueType.Variable;
            }
            else
                return ValueType.Expression;
        }
        public string Name { get; set; }
        public GenericFunction Delegate { get; set; }
        public ValueType ReturnValue { get; set; }
        public ValueType[] Parameters { get; set; }

        public bool CheckParameterTypes(object[] parameters)
        {
            if (Parameters.Length != parameters.Length)
                return false;

            for (int i = 0; i < parameters.Length; i++)
            {
                if (!(parameters[i] is ExprNode))
                    return false;

                ValueType type = GetValueType((ExprNode)parameters[i]);

                // Variable or Numeral may act as an Expression
                if (Parameters[i] == ValueType.Expression && (type == ValueType.Variable || type == ValueType.Numeral))
                    continue;
                else if (Parameters[i] == type)
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
            ValueType type = NativeFunction.GetValueType((ExprNode)param0);


            if (type == ValueType.Numeral)
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
            ValueType type0 = NativeFunction.GetValueType((ExprNode)param0);
            ValueType type1 = NativeFunction.GetValueType((ExprNode)param1);


            if ((type0 == ValueType.Numeral) && (type1 == ValueType.Numeral))
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
        public static object Exit(object[] parameters)
        {
            Environment.Exit(0);
            return null;
        }

        public object Subst(object[] parameters)
        {
            ExprNode expression = (ExprNode)parameters[0];
            string symbol = (string)((AtomNode)parameters[1]).Data;
            ExprNode value = (ExprNode)parameters[2];

            var result = Evaluator.Substitute(expression, symbol, value);

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
                Parameters = new ValueType[] { ValueType.Expression },
                ReturnValue = ValueType.Expression
            };
            functions["cos"] = new NativeFunction
            {
                Name = "cos",
                Delegate = Cos,
                Parameters = new ValueType[] { ValueType.Expression },
                ReturnValue = ValueType.Expression
            };
            functions["tan"] = new NativeFunction
            {
                Name = "tan",
                Delegate = Tan,
                Parameters = new ValueType[] { ValueType.Expression },
                ReturnValue = ValueType.Expression
            };
            functions["asin"] = new NativeFunction
            {
                Name = "asin",
                Delegate = Asin,
                Parameters = new ValueType[] { ValueType.Expression },
                ReturnValue = ValueType.Expression
            };
            functions["acos"] = new NativeFunction
            {
                Name = "acos",
                Delegate = Acos,
                Parameters = new ValueType[] { ValueType.Expression },
                ReturnValue = ValueType.Expression
            };
            functions["atan"] = new NativeFunction
            {
                Name = "atan",
                Delegate = Atan,
                Parameters = new ValueType[] { ValueType.Expression },
                ReturnValue = ValueType.Expression
            };
            functions["sinh"] = new NativeFunction
            {
                Name = "sinh",
                Delegate = Sinh,
                Parameters = new ValueType[] { ValueType.Expression },
                ReturnValue = ValueType.Expression
            };
            functions["cosh"] = new NativeFunction
            {
                Name = "cosh",
                Delegate = Cosh,
                Parameters = new ValueType[] { ValueType.Expression },
                ReturnValue = ValueType.Expression
            };
            functions["tanh"] = new NativeFunction
            {
                Name = "tanh",
                Delegate = Tanh,
                Parameters = new ValueType[] { ValueType.Expression },
                ReturnValue = ValueType.Expression
            };
            functions["asinh"] = new NativeFunction
            {
                Name = "asinh",
                Delegate = Asinh,
                Parameters = new ValueType[] { ValueType.Expression },
                ReturnValue = ValueType.Expression
            };
            functions["acosh"] = new NativeFunction
            {
                Name = "acosh",
                Delegate = Acosh,
                Parameters = new ValueType[] { ValueType.Expression },
                ReturnValue = ValueType.Expression
            };
            functions["atanh"] = new NativeFunction
            {
                Name = "atanh",
                Delegate = Atanh,
                Parameters = new ValueType[] { ValueType.Expression },
                ReturnValue = ValueType.Expression
            };
            functions["pow"] = new NativeFunction
            {
                Name = "pow",
                Delegate = Pow,
                Parameters = new ValueType[] { ValueType.Expression, ValueType.Expression },
                ReturnValue = ValueType.Expression
            };
            functions["exp"] = new NativeFunction
            {
                Name = "exp",
                Delegate = Exp,
                Parameters = new ValueType[] { ValueType.Expression },
                ReturnValue = ValueType.Expression
            };
            functions["log"] = new NativeFunction
            {
                Name = "log",
                Delegate = Log,
                Parameters = new ValueType[] { ValueType.Expression },
                ReturnValue = ValueType.Expression
            };
            functions["ln"] = new NativeFunction
            {
                Name = "ln",
                Delegate = Log,
                Parameters = new ValueType[] { ValueType.Expression },
                ReturnValue = ValueType.Expression
            };
            functions["sqrt"] = new NativeFunction
            {
                Name = "sqrt",
                Delegate = Sqrt,
                Parameters = new ValueType[] { ValueType.Expression },
                ReturnValue = ValueType.Expression
            };

            functions["exit"] = new NativeFunction
            {
                Name = "exit",
                Delegate = Exit,
                Parameters = new ValueType[] { },
                ReturnValue = ValueType.None
            };

            functions["subst"] = new NativeFunction
            {
                Name = "subst",
                Delegate = Subst,
                Parameters = new ValueType[] { ValueType.Expression, ValueType.Variable, ValueType.Expression },
                ReturnValue = ValueType.Expression
            };
        }
    }
}
