using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packages.realityflow_package.Runtime.scripts.Structures.Actions
{
    public class TeleportAction
    {
        TeleportCoordinates teleportCoordinates;

        public TeleportAction(TeleportCoordinates teleportCoordinates)
        {
            this.teleportCoordinates = teleportCoordinates;
        }
    }
}
