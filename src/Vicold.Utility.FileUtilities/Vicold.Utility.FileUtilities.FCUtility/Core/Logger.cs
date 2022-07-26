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
        private Action<IFuncPage, string> _action;

        public Logger(Action<IFuncPage, string> action)
        {
            _action = action;
        }

        public void Log(IFuncPage page, string message)
        {
            _action(page, message);
        }
    }
}
