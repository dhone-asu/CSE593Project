using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CAS.SymbolManagement
{
    public interface IVariableManager
    {
        IVariable GetVariable(string name);
        bool VariableExists(string name);
    }

    public interface IVariable
    {
        string Name { get; }
        ExprNode Value { get; }
    }

    public class Variable: IVariable
    {
        public string Name { get; set; }
        public ExprNode Value { get; set; }

        public Variable() : base() { }
    }

    public class VariableManager : IVariableManager
    {
        private Dictionary<string, IVariable> variables = new Dictionary<string, IVariable>();

        public IVariable GetVariable(string name)
        {
            return variables[name];
        }

        public bool VariableExists(string name)
        {
            return variables.ContainsKey(name);
        }

        public void SetVariable(string name, IVariable value)
        {
            variables[name] = value;
        }

        public IVariable this[string name]
        {
            get { return GetVariable(name); }
            set { SetVariable(name, value); }
        }
    }
}
