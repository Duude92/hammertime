using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Menu;
using Sledge.Common.Translations;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sledge.BspEditor.Editing.Commands.Prefabs
{
	//[Export(typeof(ICommand))]
	//[CommandID("Tools:NewPrefab")]
	//[MenuItem("Tools", "", "Create new prefab", "B")]
	//[AutoTranslate]
	public class NewPrefab : ICommand
	{
		public string Name => "New prefab";

		public string Details => "Open new prefab window";

		public Task Invoke(IContext context, CommandParameters parameters)
		{
			throw new NotImplementedException();
		}

		public bool IsInContext(IContext context) => true;
	}
}
