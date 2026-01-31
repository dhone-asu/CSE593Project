using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAS.SymbolManagement
{
    public class Constant : IVariable
    {
        string name;
        ExprNode value;
        public string Name { get { return name; } }
        public ExprNode Value { get { return value; } }

        public Constant(string name, ExprNode value)
        {
            this.name = name;
            this.value = value;
        }
    }
    public class ConstantManager : IVariableManager
    {
        private Dictionary<string, Constant> constants = new Dictionary<string, Constant>();

        public IVariable GetVariable(string name)
        {
            return constants[name];
        }

        public bool VariableExists(string name)
        {
            return constants.ContainsKey(name);
        }

        public ConstantManager()
        {
            constants["pi"] = new Constant("pi", new AtomNode { Data = Math.PI, Type = AtomType.Numeral });
            constants["e"] = new Constant("e", new AtomNode { Data = Math.E, Type = AtomType.Numeral });
            constants["inf"] = new Constant("inf", new AtomNode { Data = double.PositiveInfinity, Type = AtomType.Numeral });
            constants["gamma"] =
                new Constant("gamma", new AtomNode { Data = ExtendedMath.EulerMascheroni, Type = AtomType.Numeral });
        }
    }
}
