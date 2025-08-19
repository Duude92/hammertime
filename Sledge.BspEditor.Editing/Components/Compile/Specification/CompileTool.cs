using System.Collections.Generic;
using System.Linq;
using Sledge.Common.Transport;

namespace Sledge.BspEditor.Editing.Components.Compile.Specification
{
	public class CompileTool
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public int Order { get; set; }
		public bool Enabled { get; set; }
		public bool Custom { get; set; }
		public List<CompileParameter> Parameters { get; private set; }

		public CompileTool()
		{
			Parameters = new List<CompileParameter>();
		}

		public static CompileTool Parse(SerialisedObject gs)
		{
			var tool = new CompileTool
			{
				Name = gs.Get("Name", ""),
				Description = gs.Get("Description", ""),
				Order = gs.Get("Order", 0),
				Enabled = gs.Get("Enabled", true),
				Custom = gs.Get("Custom", false)
			};
			var parameters = gs.Children.Where(x => x.Name == "Parameter");
			tool.Parameters.AddRange(parameters.Select(CompileParameter.Parse));
			if (tool.Custom) tool.Parameters.Add(new CompileParameter { Name = "Value" });
			return tool;
		}
	}
}