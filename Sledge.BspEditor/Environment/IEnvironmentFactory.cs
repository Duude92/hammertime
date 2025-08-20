using System;
using System.Collections.Generic;
using System.Globalization;

namespace Sledge.BspEditor.Environment
{
    public interface IEnvironmentFactory
    {
        Type Type { get; }
        string TypeName { get; }
        string Description { get; }

        IEnvironment Deserialise(SerialisedEnvironment environment);
        SerialisedEnvironment Serialise(IEnvironment environment);

        IEnvironment CreateEnvironment();
        IEnvironmentEditor CreateEditor();
    }
    public static class EnvironmentHelper
    {
		public static T GetVal<T>(Dictionary<string, string> dictionary, string key, T def = default(T))
		{
			if (dictionary.TryGetValue(key, out var val))
			{
				try
				{
					return (T)Convert.ChangeType(val, typeof(T), CultureInfo.InvariantCulture);
				}
				catch
				{
					//
				}
			}
			return def;
		}
	}
}