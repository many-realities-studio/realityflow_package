using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Packages.realityflow_package.Runtime.scripts.Messages.UserMessages
{
    public class LogoutUser_Received : ConfirmationMessage_Received
    {
        public LogoutUser_Received(bool wasSuccessful) : base(wasSuccessful)
        {
            this.MessageType = "LogoutUser";
            this.WasSuccessful = wasSuccessful;
        }
    }
}
