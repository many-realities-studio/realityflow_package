﻿using Newtonsoft.Json;
using RealityFlow.Plugin.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Packages.realityflow_package.Runtime.scripts.Messages.UserMessages
{
    /// <summary>
    /// Send a login request message format
    /// Response: <see cref="LoginUser_Received"/>
    /// </summary>
    public class Login_SendToServer : BaseMessage
    {
        [JsonProperty("FlowUser")]
        public FlowUser flowUser { get; set; }


        public Login_SendToServer(FlowUser flowUser)
        {
            this.flowUser = flowUser;
            this.MessageType = "LoginUser";
        }
    }

    /// <summary>
    /// logout request message format 
    /// Response: <see cref="LogoutUser_Received"/>
    /// </summary>
    public class Logout_SendToServer : BaseMessage
    {
        [JsonProperty("FlowUser")]
        FlowUser flowUser { get; set; }

        public Logout_SendToServer(FlowUser flowUser)
        {
            this.flowUser = flowUser;
            this.MessageType = "LogoutUser";
        }
    }

    /// <summary>
    /// Register user request message format 
    /// Response: <see cref="RegisterUser_Received"/>
    /// </summary>
    public class RegisterUser_SendToServer : BaseMessage
    {
        [JsonProperty("FlowUser")]
        FlowUser flowUser { get; set; }

        public RegisterUser_SendToServer(FlowUser flowUser)
        {
            this.flowUser = flowUser;
            this.MessageType = "CreateUser";
        }
    }
}
