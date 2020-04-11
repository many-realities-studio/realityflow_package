﻿using Packages.realityflow_package.Runtime.scripts.Messages;
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

        public static dynamic ConvertToChildClass(string JsonObject, string actionType)
        {
            switch(actionType)
            {
                case "Teleport":
                    return MessageSerializer.DesearializeObject<TeleportAction>(JsonObject);
                default:
                    return null;
            }
        }
    }
}