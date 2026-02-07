using CAS.SymbolManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAS
{
    public class SessionManager
    {
        public static SessionManager Default { get; private set; } = new SessionManager();
        ConstantManager constantManager = new ConstantManager();
        UserVariableManager userVariableManager = new UserVariableManager();
        NativeFunctionManager nativeFunctionManager = new NativeFunctionManager();
        UserFunctionManager userFunctionManager = new UserFunctionManager();

        public SessionManager()
        {
            nativeFunctionManager.SessionManager = this;
            userFunctionManager.SessionManager = this;
        }

        public ConstantManager ConstantManager { get { return constantManager; } }
        public UserVariableManager UserVariableManager { get {return userVariableManager; } }
        public NativeFunctionManager NativeFunctionManager { get { return nativeFunctionManager; } }
        public UserFunctionManager UserFunctionManager { get { return userFunctionManager; } }
    }
}
