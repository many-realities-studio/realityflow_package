using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packages.realityflow_package.Runtime.scripts.Messages
{
    public abstract class ReceivedMessage : BaseMessage
    {
        public abstract void RaiseEvent();
    }
}
