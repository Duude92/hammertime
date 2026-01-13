using System;
using System.Drawing;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Documents;

namespace Sledge.Common.Shell.Components
{
    /// <summary>
    /// A tool which is hosted in the tool bar of the shell.
    /// </summary>
    public interface ITool : IContextAware
    {
        /// <summary>
        /// The tool's icon
        /// </summary>
        Image Icon { get; }

        /// <summary>
        /// The tool's name
        /// </summary>
        string Name { get; }
        /// <summary>
        /// The tool's id to filter tools
        /// </summary>
        Capability ToolCapability { get; }
    }
}
