using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Packages.realityflow_package.Runtime.scripts.Messages.UserMessages
{
    public class RegisterUser_Received : ConfirmationMessage_Received
    {
        public RegisterUser_Received(bool wasSuccessful) : base(wasSuccessful)
        {
            this.MessageType = "CreateUser";
            this.WasSuccessful = wasSuccessful;
        }
    }
}
