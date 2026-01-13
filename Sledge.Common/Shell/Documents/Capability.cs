using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sledge.Common.Shell.Documents
{
	public sealed class Capability
	{
		public string Name { get; }

		private Capability(string name)
		{
			Name = name;
		}
		public static Capability Create(string name)
		{
			if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
			return new Capability(name);
		}
	}
}
