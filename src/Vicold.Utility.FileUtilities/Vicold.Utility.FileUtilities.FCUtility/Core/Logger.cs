using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vicold.Utility.FileUtilities.FCUtility.Views.Pages;

namespace Vicold.Utility.FileUtilities.FCUtility.Core
{
    internal class Logger
    {
        private Action<string>? _action;

        public Logger()
        {
        }

        public Logger(Action<string> action)
        {
            _action = action;
        }

        public void Binding(Action<string> action)
        {
            _action = action;
        }

        public void Log(IFuncPage page, string log)
        {
            Log(page.FuncTitle, log);
        }

        public void Log(string head, string log)
        {
            _action?.Invoke($"{head}：{log}");
        }
    }
}
