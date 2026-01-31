using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAS
{
    public class NativeFunctionManager : IFunctionManager
    {
        private Dictionary<string, Function> functions { get; set; }

        private static object RunOneParamMathFunction(string name, Func<double, double> func, object[] parameters)
        {
            var param0 = parameters[0];
            ValueType type = Function.GetValueType((ExprNode)param0);


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
            var param0 = parameters[0];
            var param1 = parameters[1];
            ValueType type0 = Function.GetValueType((ExprNode)param0);
            ValueType type1 = Function.GetValueType((ExprNode)param1);


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

        public static object Exit(object[] parameters)
        {
            Environment.Exit(0);
            return null;
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
            functions = new Dictionary<string, Function>();
            functions["sin"] = new Function
            {
                Name = "sin",
                Delegate = Sin,
                Parameters = new ValueType[] { ValueType.Expression },
                ReturnValue = ValueType.Expression
            };
            functions["cos"] = new Function
            {
                Name = "cos",
                Delegate = Cos,
                Parameters = new ValueType[] { ValueType.Expression },
                ReturnValue = ValueType.Expression
            };
            functions["tan"] = new Function
            {
                Name = "tan",
                Delegate = Tan,
                Parameters = new ValueType[] { ValueType.Expression },
                ReturnValue = ValueType.Expression
            };
            functions["asin"] = new Function
            {
                Name = "asin",
                Delegate = Asin,
                Parameters = new ValueType[] { ValueType.Expression },
                ReturnValue = ValueType.Expression
            };
            functions["acos"] = new Function
            {
                Name = "acos",
                Delegate = Acos,
                Parameters = new ValueType[] { ValueType.Expression },
                ReturnValue = ValueType.Expression
            };
            functions["atan"] = new Function
            {
                Name = "atan",
                Delegate = Atan,
                Parameters = new ValueType[] { ValueType.Expression },
                ReturnValue = ValueType.Expression
            };
            functions["sinh"] = new Function
            {
                Name = "sinh",
                Delegate = Sinh,
                Parameters = new ValueType[] { ValueType.Expression },
                ReturnValue = ValueType.Expression
            };
            functions["cosh"] = new Function
            {
                Name = "cosh",
                Delegate = Cosh,
                Parameters = new ValueType[] { ValueType.Expression },
                ReturnValue = ValueType.Expression
            };
            functions["tanh"] = new Function
            {
                Name = "tanh",
                Delegate = Tanh,
                Parameters = new ValueType[] { ValueType.Expression },
                ReturnValue = ValueType.Expression
            };
            functions["asinh"] = new Function
            {
                Name = "asinh",
                Delegate = Asinh,
                Parameters = new ValueType[] { ValueType.Expression },
                ReturnValue = ValueType.Expression
            };
            functions["acosh"] = new Function
            {
                Name = "acosh",
                Delegate = Acosh,
                Parameters = new ValueType[] { ValueType.Expression },
                ReturnValue = ValueType.Expression
            };
            functions["atanh"] = new Function
            {
                Name = "atanh",
                Delegate = Atanh,
                Parameters = new ValueType[] { ValueType.Expression },
                ReturnValue = ValueType.Expression
            };
            functions["pow"] = new Function
            {
                Name = "pow",
                Delegate = Pow,
                Parameters = new ValueType[] { ValueType.Expression, ValueType.Expression },
                ReturnValue = ValueType.Expression
            };
            functions["exp"] = new Function
            {
                Name = "exp",
                Delegate = Exp,
                Parameters = new ValueType[] { ValueType.Expression },
                ReturnValue = ValueType.Expression
            };
            functions["log"] = new Function
            {
                Name = "log",
                Delegate = Log,
                Parameters = new ValueType[] { ValueType.Expression },
                ReturnValue = ValueType.Expression
            };
            functions["ln"] = new Function
            {
                Name = "ln",
                Delegate = Log,
                Parameters = new ValueType[] { ValueType.Expression },
                ReturnValue = ValueType.Expression
            };

            functions["exit"] = new Function
            {
                Name = "exit",
                Delegate = Exit,
                Parameters = new ValueType[] { },
                ReturnValue = ValueType.None
            };
        }
    }
}
