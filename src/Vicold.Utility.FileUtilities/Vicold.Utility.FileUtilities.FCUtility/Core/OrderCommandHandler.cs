using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vicold.Utility.FileUtilities.FCUtility.Core
{
    internal class OrderCommandHandler
    {
        private readonly Dictionary<string, Action<object>> _commandDic = new Dictionary<string, Action<object>>();

        public void Register(string command, Action<object> action)
        {
            if (_commandDic.ContainsKey(command))
            {
                //_commandDic[command] = action;
                throw new Exception($"command: {command} has been registered.");
            }
            else
            {
                _commandDic.Add(command, action);
            }
        }

        public void Handle(string command, object args)
        {
            if (_commandDic.ContainsKey(command))
            {
                _commandDic[command]?.Invoke(args);
            }
            else
            {
                throw new Exception($"undefined command: {command}");
            }
        }   
    }
}
