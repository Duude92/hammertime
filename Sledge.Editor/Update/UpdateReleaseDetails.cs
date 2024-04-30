using System;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sledge.Editor.Update
{
    public class UpdateReleaseDetails
    {
        public string Tag { get; }
        public string Name { get; }
        public string Changelog { get; }
        public string FileName { get; }
        public string DownloadUrl { get; }
        public DateTime PublishDate { get; }

        public bool Exists => Tag != null;

        public UpdateReleaseDetails(string jsonString, string tag)
        {
            var obj = JsonConvert.DeserializeObject(jsonString) as JArray;
            if (obj == null || obj.Count < 1) return;


			var rel = obj.Select(x => x as JObject).Where(x => { var xTag = x.GetValue("tag_name").ToString(); return xTag.Equals("latest"); }).FirstOrDefault();


            var assets = rel?.GetValue("assets") as JArray;
            if (assets == null || assets.Count < 1) return;

            var exeAsset = assets.FirstOrDefault(x => x is JObject o && o.GetValue("name").ToString().EndsWith(".exe")) as JObject;
            if (exeAsset == null) return;

            Tag = rel.GetValue("tag_name").ToString();
            Name = rel.GetValue("name").ToString();
            Changelog = rel.GetValue("body").ToString();
            FileName = exeAsset.GetValue("name").ToString();
            DownloadUrl = exeAsset.GetValue("url").ToString();
			PublishDate = exeAsset.GetValue("created_at").Value<DateTime>();
		}
    }
}
