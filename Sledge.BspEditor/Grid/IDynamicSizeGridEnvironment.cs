namespace Sledge.BspEditor.Grid
{
	public interface IDynamicSizeGridEnvironment
	{
		public bool OverrideMapSize { get; }
		public decimal MapSizeLow { get; }
		public decimal MapSizeHigh { get; }
	}
}
