using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Packages.realityflow_package.Runtime.scripts.Messages.ProjectMessages
{
    public class DeleteProject_Received : ConfirmationMessage_Received
    {
        public DeleteProject_Received(bool wasSuccessful) : base(wasSuccessful)
        {
            this.MessageType = "DeleteProject";
        }
    }
}
