using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packages.realityflow_package.Runtime.scripts.Structures.Actions
{
    public class FlowAction
    {
        public string Id;
        public string ActionType;

        public FlowAction(string id)
        {
            Id = id;
        }

        public FlowAction()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
