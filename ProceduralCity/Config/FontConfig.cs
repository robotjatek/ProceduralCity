using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web.Script.Serialization;

namespace ProceduralCity.Config
{
    class CharProperties
    {
        public int X { get; set; }

        public int Y { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int OriginX { get; set; }

        public int OriginY { get; set; }

        public int Advance { get; set; }
    }

    class FontConfig
    {
        public FontConfig(string fontName)
        {
            //TODO: this is some nasty hack down there. Unfortunately ConfigurationBuilder does not support case-insensitive keys in JSON
            var serializer = new JavaScriptSerializer();
            var json = File.ReadAllText($"Fonts/{fontName}/font.json");
            var dictionary = serializer.Deserialize<Dictionary<string, object>>(json);
            Bind(dictionary);
            var charactersSubDictionary = serializer.Serialize(dictionary["characters"]);
            Characters = serializer.Deserialize<Dictionary<string, CharProperties>>(charactersSubDictionary);
        }

        private void Bind(Dictionary<string, object> from)
        {
            foreach (var item in from)
            {
                if (!item.Key.Equals("characters", System.StringComparison.OrdinalIgnoreCase))
                {
                    this.GetType().GetProperty(item.Key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).SetValue(this, item.Value);
                }
            }
        }

        public string Name { get; set; }

        public int Size { get; set; }

        public bool Bold { get; set; }

        public bool Italic { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public Dictionary<string, CharProperties> Characters { get; set; } = new Dictionary<string, CharProperties>();
    }
}
