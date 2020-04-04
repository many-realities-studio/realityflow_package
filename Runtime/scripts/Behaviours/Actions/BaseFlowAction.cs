using UnityEditor;

namespace Packages.realityflow_package.Runtime.scripts.Messages.BehaviourMessages
{
    public class BaseFlowAction
    {
        public GUID Id;
        public string ActionType;

        public BaseFlowAction(GUID id)
        {
            Id = id;
        }

        public BaseFlowAction()
        {
            Id = GUID.Generate();
        }
    }
}