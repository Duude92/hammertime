using Sledge.BspEditor.Compile;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.Common.Shell.Documents;
using Sledge.DataStructures.GameData;
using Sledge.FileSystem;
using Sledge.Providers.Texture;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sledge.BspEditor.Environment.Empty
{
    public class EmptyEnvironment : IEnvironment
    {
        public string Engine => "None";
        public string ID => "Empty";
        public string Name => "Empty";
        public IFile Root => null;
        public IEnumerable<string> Directories => new string[0];
		public IReadOnlySet<Capability> Capabilities => new HashSet<Capability>();

		public async Task<TextureCollection> GetTextureCollection()
        {
             return new EmptyTextureCollection(new TexturePackage[0]);
        }

        public async Task<GameData> GetGameData()
        {
            return new GameData();
        }

        public Task UpdateDocumentData(MapDocument document)
        {
            return Task.FromResult(0);
        }

        public void AddData(IEnvironmentData data)
        {

        }

        public IEnumerable<T> GetData<T>() where T : IEnvironmentData
        {
            return null;
        }

        public Task<Batch> CreateBatch(IEnumerable<BatchArgument> arguments, BatchOptions options)
        {
            return Task.FromResult<Batch>(null);
        }

        public IEnumerable<AutomaticVisgroup> GetAutomaticVisgroups()
        {
            yield break;
        }

        public bool IsNullTexture(string name)
        {
            return false;
        }

		public IEnumerable<TexturePackageReference> GetSkyboxes() => Enumerable.Empty<TexturePackageReference>();
		public string DefaultBrushEntity => "";
        public string DefaultPointEntity => "";
        public decimal DefaultTextureScale => 1;
        public float DefaultGridSize => 16;
		public string CordonTexture { get; set; }
		public string[] NonRenderableTextures { get; set; }
        public string BaseDirectory => ".";
	}
}
