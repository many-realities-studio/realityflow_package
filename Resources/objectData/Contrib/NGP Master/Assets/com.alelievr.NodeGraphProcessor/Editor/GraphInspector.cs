using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System;

namespace RealityFlow.Plugin.Contrib
{
    public class GraphInspector : Editor
    {
        protected VisualElement root;
        protected BaseGraph     graph;

        VisualElement   parameterContainer;

        protected virtual void OnEnable()
        {
            graph = target as BaseGraph;
            graph.onExposedParameterListChanged += UpdateExposedParameters;
            graph.onExposedParameterModified += UpdateExposedParameters;
        }

        protected virtual void OnDisable()
        {
            graph.onExposedParameterListChanged -= UpdateExposedParameters;
            graph.onExposedParameterModified -= UpdateExposedParameters;
        }

        public sealed override VisualElement CreateInspectorGUI()
        {
            root = new VisualElement();
            CreateInspector();
            return root;
        }

        protected virtual void CreateInspector()
        {
            parameterContainer = new VisualElement{
                name = "ExposedParameters"
            };
            FillExposedParameters(parameterContainer);

            root.Add(parameterContainer);
        }

        protected void FillExposedParameters(VisualElement parameterContainer)
        {
            if (graph.exposedParameters.Count != 0)
                parameterContainer.Add(new Label("Exposed Parameters:"));

            foreach (var param in graph.exposedParameters)
            {
                if(param.settings.isHidden)
                    continue;
                
                VisualElement prop = new VisualElement();
                prop.style.display = DisplayStyle.Flex;
                Type paramType = Type.GetType(param.type);
                var field = FieldFactory.CreateField(paramType, param.serializedValue.value, (newValue) => {
					Undo.RegisterCompleteObjectUndo(graph, "Changed Parameter " + param.name + " to " + newValue);
                    param.serializedValue.value = newValue;
                }, param.name);
                prop.Add(field);
                parameterContainer.Add(prop);
            }
        }

        void UpdateExposedParameters(string guid) => UpdateExposedParameters();
		
        void UpdateExposedParameters()
        {
            parameterContainer.Clear();
            FillExposedParameters(parameterContainer);
        }

        // Don't use ImGUI
        public sealed override void OnInspectorGUI() {}

    }
}