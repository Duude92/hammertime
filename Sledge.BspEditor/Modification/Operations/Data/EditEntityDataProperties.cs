using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification.Operations.Mutation;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Transport;

namespace Sledge.BspEditor.Modification.Operations.Data
{
    public class EditEntityDataProperties : IOperation
    {
        private readonly long _id;
        private readonly Dictionary<string, string> _valuesToSet;
        private SerialisedObject _beforeState;
        public bool Trivial => false;

        public EditEntityDataProperties(long id, Dictionary<string, string> valuesToSet)
        {
            _id = id;
            _valuesToSet = valuesToSet;
        }

        public async Task<Change> Perform(MapDocument document)
        {
            var ch = new Change(document);

            var obj = document.Map.Root.FindByID(_id);
            var data = obj?.Data.GetOne<EntityData>();
            if (data != null)
            {
                _beforeState = data.ToSerialisedObject();
                foreach (var kv in _valuesToSet)
                {
                    if (kv.Value == null) data.Properties.Remove(kv.Key);
                    else if (kv.Key == "Location")
                    {
                        var split = kv.Value.Split(' ');
                        if (float.TryParse(split[0], out var x) &&
                            float.TryParse(split[1], out var y) &&
                            float.TryParse(split[2], out var z))
                        {
                            var vec = new Vector3(x, y, z) - obj.Data.GetOne<Origin>().Location;
							var translation = Matrix4x4.CreateTranslation(vec);

                            var tr = new Transaction(new Transform(translation, new[] { obj }));
                            MapDocumentOperation.Perform(document, tr);
                        }
                    }
                    else data.Properties[kv.Key] = kv.Value;
                }
                ch.Update(obj);
            }

            return ch;
        }

        public async Task<Change> Reverse(MapDocument document)
        {
            var ch = new Change(document);

            var obj = document.Map.Root.FindByID(_id);
            if (obj != null && _beforeState != null)
            {
                var ed = new EntityData(_beforeState);
                obj.Data.Replace(ed);
                ch.Update(obj);
            }

            return ch;
        }
    }
}
