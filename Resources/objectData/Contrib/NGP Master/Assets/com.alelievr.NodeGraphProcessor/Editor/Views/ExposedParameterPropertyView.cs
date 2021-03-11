using UnityEngine.UIElements;

namespace RealityFlow.Plugin.Contrib
{
	public class ExposedParameterPropertyView : VisualElement
	{
		protected BaseGraphView baseGraphView;

		public ExposedParameter parameter { get; private set; }

		public Toggle     hideInInspector { get; private set; }

		public ExposedParameterPropertyView(BaseGraphView graphView, ExposedParameter param)
		{
			baseGraphView = graphView;
			parameter      = param;

			hideInInspector = new Toggle
			{
				text  = "Hide in Inspector",
				value = parameter.settings.isHidden
			};
			hideInInspector.RegisterValueChangedCallback(e =>
			{
				baseGraphView.graph.UpdateExposedParameterVisibility(parameter, e.newValue);
			});

			Add(hideInInspector);
		}
	}
} 