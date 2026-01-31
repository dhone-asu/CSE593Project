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
}
