using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CAS
{
    public delegate object GenericFunction(object[] parameters);
    public enum ValueType { None, Numeral, Variable, Expression };

    public class ParameterException: Exception
    {
        public ParameterException(string message) : base(message) { }
        public ParameterException(): base() { }
    }

    public interface IFunction
    {
        string Name { get; set; }
        GenericFunction Delegate { get; set; }
        ValueType ReturnValue { get; set; }
        ValueType[] Parameters {  get; set; }
        bool CheckParameterTypes(object[] parameters);
        object Call(object[] parameters);
    }

    public interface IFunctionManager
    {
        IFunction GetFunction(string name);
        bool FunctionExists(string name);
    }

    public class Function : IFunction
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

    public class NativeFunctionManager: IFunctionManager
    {
        private Dictionary<string, Function> functions { get; set; }

        private static object RunOneParamMathFunction(string name, Func<double,double> func, object[] parameters)
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
                    Data = func(a,b)
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
