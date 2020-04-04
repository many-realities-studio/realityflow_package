using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packages.realityflow_package.Runtime.scripts.Messages.BehaviourMessages
{
    public class FlowAction_Teleport : BaseFlowAction
    {
        TeleportCoordinates _TeleportCoordinates;

        public FlowAction_Teleport(TeleportCoordinates TeleportCoordinates) : base()
        {
            _TeleportCoordinates = TeleportCoordinates;
            ActionType = "Teleport";
        }
    }
}
