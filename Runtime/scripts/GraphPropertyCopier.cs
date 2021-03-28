using RealityFlow.Plugin.Scripts;

namespace Packages.realityflow_package.Runtime.scripts
{
    /// <summary>
    /// The purpose of this class is to copy the properties of one object into another
    /// </summary>
    /// <typeparam name="TParent">The type of the parent object (The source information)</typeparam>
    /// <typeparam name="TChild">The type of the child object (The destination, these properties will get overwritten with those values that are in the parent)</typeparam>
    public class GraphPropertyCopier<TParent, TChild> where TParent : class
                                            where TChild : class
    {
        /// <summary>
        /// Copy the properties of a parent object into the corresponsing property of the child
        /// </summary>
        /// <param name="parent">Source information object</param>
        /// <param name="child">Destination object (These properties get overwritten)</param>
        public static void Copy(TParent parent, TChild child)
        {
            var parentFields = parent.GetType().GetFields();
            var childFields = child.GetType().GetFields();

            //var childName = child.GetType().GetProperty("Name");
            var childSerializedNodes = child.GetType().GetField("serializedNodes");
            var parentSerializedNodes = parent.GetType().GetField("serializedNodes");
            childSerializedNodes.SetValue(child, parentSerializedNodes.GetValue(parent));
            // var childEdges = child.GetType().GetProperty("edges");
            // var childGroups = child.GetType().GetProperty("groups");
            // var childStackNodes = child.GetType().GetProperty("stackNodes");
            // var childPinnedElements = child.GetType().GetProperty("pinnedElements");
            // var childExposedParameters = child.GetType().GetProperty("exposedParameters");
            // var childStickyNotes = child.GetType().GetProperty("stickyNotes");
            // var childPosition = child.GetType().GetProperty("position");
            // var childScale = child.GetType().GetProperty("scale");

            // var parentName = parent.GetType().GetProperty("Name");
            // var parentSerializedNodes = parent.GetType().GetProperty("serializedNodes");
            // var parentEdges = parent.GetType().GetProperty("edges");
            // var parentGroups = parent.GetType().GetProperty("groups");
            // var parentStackNodes = parent.GetType().GetProperty("stackNodes");
            // var parentPinnedElements = parent.GetType().GetProperty("pinnedElements");
            // var parentExposedParameters = parent.GetType().GetProperty("exposedParameters");
            // var parentStickyNotes = parent.GetType().GetProperty("stickyNotes");
            // var parentPosition = parent.GetType().GetProperty("position");
            // var parentScale = parent.GetType().GetProperty("scale");

            // childName.SetValue(child, parentName.GetValue(parent));
            // childSerializedNodes.SetValue(child, parentSerializedNodes.GetValue(parent));
            // childEdges.SetValue(child, parentEdges.GetValue(parent));
            // childGroups.SetValue(child, parentGroups.GetValue(parent));
            // childStackNodes.SetValue(child, parentStackNodes.GetValue(parent));
            // childPinnedElements.SetValue(child, parentPinnedElements.GetValue(parent));
            // childExposedParameters.SetValue(child, parentExposedParameters.GetValue(parent));
            // childStickyNotes.SetValue(child, parentStickyNotes.GetValue(parent));
            // childPosition.SetValue(child, parentPosition.GetValue(parent));
            // childScale.SetValue(child, parentScale.GetValue(parent));

            // childProperties. = input.name;
            // serializedNodes = input.serializedNodes;
            // edges = input.edges;
            // groups = input.groups;
            // stackNodes = input.stackNodes;
            // pinnedElements = input.pinnedElements;
            // exposedParameters = input.exposedParameters;
            // stickyNotes = input.stickyNotes;
            // position = input.position;
            // scale= input.scale;

            // foreach (var parentProperty in parentProperties)
            // {
            //     foreach (var childProperty in childProperties)
            //     {
            //         if (parentProperty.Name == childProperty.Name && parentProperty.PropertyType == childProperty.PropertyType)
            //         {
            //             childProperty.SetValue(child, parentProperty.GetValue(parent));
            //             break;
            //         }
            //     }
            // }
        }
    }
}