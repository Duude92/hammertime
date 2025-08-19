using Sledge.BspEditor.Documents;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Sledge.BspEditor.Compile
{
	internal class CommandProcess : BatchProcess
	{
		public CommandProcess(BatchStepType stepType, string process) : base(stepType, RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "cmd" : "", $"/c {process}")
		{
		}

		public override async Task Run(Batch batch, MapDocument document)
		{
			await RunInternal(batch, document);
		}

	}
}
