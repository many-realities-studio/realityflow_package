using RealityFlow.Plugin.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packages.realityflow_package.Runtime.scripts
{
    public static class ConfigurationSingleton
    {
        public static FlowProject CurrentProject { get; set; }
        public static FlowUser CurrentUser { get; set; }
    }
}
