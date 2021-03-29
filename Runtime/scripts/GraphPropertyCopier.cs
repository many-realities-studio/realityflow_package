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
        /// Copy the properties of a parent object into the corresponsing Field of the child
        /// </summary>
        /// <param name="parent">Source information object</param>
        /// <param name="child">Destination object (These properties get overwritten)</param>
        public static void Copy(TParent parent, TChild child)
        {
            // var parentFields = parent.GetType().GetFields();
            // var childFields = child.GetType().GetFields();

            var childName = child.GetType().GetField("Name");
            var childSerializedNodes = child.GetType().GetField("serializedNodes");
            var childEdges = child.GetType().GetField("edges");
            var childGroups = child.GetType().GetField("groups");
            var childStackNodes = child.GetType().GetField("stackNodes");
            var childPinnedElements = child.GetType().GetField("pinnedElements");
            var childExposedParameters = child.GetType().GetField("exposedParameters");
            var childStickyNotes = child.GetType().GetField("stickyNotes");
            var childPosition = child.GetType().GetField("position");
            var childScale = child.GetType().GetField("scale");

            var parentName = parent.GetType().GetField("Name");
            var parentSerializedNodes = parent.GetType().GetField("serializedNodes");
            var parentEdges = parent.GetType().GetField("edges");
            var parentGroups = parent.GetType().GetField("groups");
            var parentStackNodes = parent.GetType().GetField("stackNodes");
            var parentPinnedElements = parent.GetType().GetField("pinnedElements");
            var parentExposedParameters = parent.GetType().GetField("exposedParameters");
            var parentStickyNotes = parent.GetType().GetField("stickyNotes");
            var parentPosition = parent.GetType().GetField("position");
            var parentScale = parent.GetType().GetField("scale");

            childName.SetValue(child, parentName.GetValue(parent));
            childSerializedNodes.SetValue(child, parentSerializedNodes.GetValue(parent));
            childEdges.SetValue(child, parentEdges.GetValue(parent));
            childGroups.SetValue(child, parentGroups.GetValue(parent));
            childStackNodes.SetValue(child, parentStackNodes.GetValue(parent));
            childPinnedElements.SetValue(child, parentPinnedElements.GetValue(parent));
            childExposedParameters.SetValue(child, parentExposedParameters.GetValue(parent));
            childStickyNotes.SetValue(child, parentStickyNotes.GetValue(parent));
            childPosition.SetValue(child, parentPosition.GetValue(parent));
            childScale.SetValue(child, parentScale.GetValue(parent));

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

            // foreach (var parentField in parentProperties)
            // {
            //     foreach (var childField in childProperties)
            //     {
            //         if (parentField.Name == childField.Name && parentField.FieldType == childField.FieldType)
            //         {
            //             childField.SetValue(child, parentField.GetValue(parent));
            //             break;
            //         }
            //     }
            // }
        }
    }
}