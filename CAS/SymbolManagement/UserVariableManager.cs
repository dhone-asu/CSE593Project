using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAS.SymbolManagement
{
    public class UserVariable : IVariable
    {
        public string Name { get; set; }
        public ExprNode Value { get; set; }

        public UserVariable() : base() { }
    }

    public class UserVariableManager : IVariableManager
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
