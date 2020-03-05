using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace RealityFlow.Plugin.Scripts
{
    [DataContract]
    public class FlowUser
    {
        [DataMember]
        public string Username { get; set; }
        [DataMember]
        public string Password { get; set; }

        public FlowUser(string username, string password)
        {
            this.Username = username;
            this.Password = password;
        }
    }
}