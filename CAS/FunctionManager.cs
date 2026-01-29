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

    public class Function : IFunction
    {
        public string Name { get; set; }
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

                if (Parameters[i] == ValueType.Expression && type == ValueType.Variable)
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
    public interface IFunctionManager
    {
        Function GetFunction(string name);
        bool FunctionExists(string name);
    }

    public class NativeFunctionManager: IFunctionManager
    {
        private Dictionary<string, Function> functions { get; set; }

        public static object Sin(object[] parameters)
        {
            var a = parameters[1];
            ValueType type = Function.GetValueType(a);

            if (type == ValueType.Numeral)
                return Math.Sin((double)((AtomNode)a).Data);
            else
                return a;
        }

        public bool FunctionExists(string name)
        {
            return functions.ContainsKey(name);
        }

        public Function GetFunction(string name)
        {
            return functions[name];
        }
    }
}
