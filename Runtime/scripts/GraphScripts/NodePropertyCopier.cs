using RealityFlow.Plugin.Scripts;

namespace Packages.realityflow_package.Runtime.scripts
{
    /// <summary>
    /// The purpose of this class is to copy the properties of one object into another
    /// </summary>
    /// <typeparam name="TParent">The type of the parent object (The source information)</typeparam>
    /// <typeparam name="TChild">The type of the child object (The destination, these properties will get overwritten with those values that are in the parent)</typeparam>
    public class NodePropertyCopier<TParent, TChild> where TParent : class
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

            var childPosition = child.GetType().GetField("localPos");

            var parentPosition = parent.GetType().GetField("localPos");

            childPosition.SetValue(child, parentPosition.GetValue(parent));

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