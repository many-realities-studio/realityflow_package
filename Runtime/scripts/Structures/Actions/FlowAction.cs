using Newtonsoft.Json;
using Packages.realityflow_package.Runtime.scripts.Messages;
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

        public FlowAction(bool noAction)
        {
            Id = Guid.NewGuid().ToString();
            if(noAction == true)
            {
                ActionType = "NoAction";
            }
        }

        [JsonConstructor]
        public FlowAction()
        {
            Id = Guid.NewGuid().ToString();
        }

        public static dynamic ConvertToChildClass(string JsonObject, string actionType)
        {
            switch(actionType)
            {
                case "Teleport":
                    return MessageSerializer.DesearializeObject<TeleportAction>(JsonObject);
                case "SnapZone":
                    return MessageSerializer.DesearializeObject<TeleportAction>(JsonObject);
                case "Enable":
                    return MessageSerializer.DesearializeObject<FlowAction>(JsonObject);
                case "Disable":
                    return MessageSerializer.DesearializeObject<FlowAction>(JsonObject);
                default:
                    return new FlowAction(true);
            }
        }
    }
}
