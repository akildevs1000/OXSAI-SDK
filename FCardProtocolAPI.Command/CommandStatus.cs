using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCardProtocolAPI.Command
{
    public enum CommandStatus
    {
        Succeed = 200,
        CommonTimeout = 100,
        PasswordError = 101,
        CommandError = 102,
        ParameterError = 103,
        ConnectionError = 104
    }
}
