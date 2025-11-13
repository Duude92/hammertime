using Sledge.FileSystem;
using Sledge.Formats.Model.Goldsource;
using Sledge.Formats.Model.Source;
using Sledge.Providers.Model;
using Sledge.Providers.Model.Mdl10;
using Sledge.Rendering.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MdlFile = Sledge.Formats.Model.Source.MdlFile;

namespace HammerTime.Source.Providers.Model.Mdl44
{
	[Export(typeof(IModelProvider))]
	public class SourceModelProvider : IModelProvider
	{
		public bool CanLoadModel(IFile file)
		{
			if (!file.Exists || file.Extension != "mdl") return false;

			try
			{
				using (var s = file.Open())
				{
					using (var br = new BinaryReader(s))
					{
						var id = (ID)br.ReadInt32();
						var version = (Versions)br.ReadInt32();
						return id == ID.Idst && KnownVersions.Contains(version);
					}
				}
			}
			catch
			{
				return false;
			}
		}

		public IModelRenderable CreateRenderable(IModel model)
		{
			return new SourceModelRenderable((MdlModel)model);

		}

		public bool IsProvider(IModel model)
		{
			return model is MdlModel;
		}

		public async Task<IModel> LoadModel(IFile file)
		{
			return await Task.Factory.StartNew(() =>
			{
				var mdl = new MdlFile(file.Open());
				var vtxFiles = file.Parent.GetFiles($"{file.NameWithoutExtension}.((sw)|(dx\\d0))").Where(x => x.Extension == "vtx")
				.ToList();
				var vtxFile = vtxFiles.FirstOrDefault(x => x.NameWithoutExtension.EndsWith("dx90"));
				if (vtxFile == null)
				{
					vtxFiles.Sort((prev, next) => prev.NameWithoutExtension.CompareTo(next.NameWithoutExtension));
					vtxFile = vtxFiles.FirstOrDefault();
				}
				var vvdFile = file.Parent.GetFile(file.NameWithoutExtension + ".vvd");
				var vvd = new VvdFile(vvdFile?.Open());
				var vtx = new VtxFile(vtxFile?.Open());
				mdl.VvdFile = vvd;
				mdl.VtxFile = vtx;
				return new MdlModel(mdl);
			});
		}
		private static readonly HashSet<Versions> KnownVersions = Enum.GetValues(typeof(Versions)).OfType<Versions>().ToHashSet();

		private enum Versions
		{
			V44 = 44
		}
	}
}
