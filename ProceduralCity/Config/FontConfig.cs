using Newtonsoft.Json;

using System.Collections.Generic;
using System.IO;
using System.Reflection;

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
            var json = File.ReadAllText($"Fonts/{fontName}/font.json");

            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            Bind(dictionary);
            var charactersSubDictionary = JsonConvert.SerializeObject(dictionary["characters"]);
            Characters = JsonConvert.DeserializeObject<Dictionary<string, CharProperties>>(charactersSubDictionary);
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

        public long Size { get; set; }

        public bool Bold { get; set; }

        public bool Italic { get; set; }

        public long Width { get; set; }

        public long Height { get; set; }

        public Dictionary<string, CharProperties> Characters { get; set; } = new Dictionary<string, CharProperties>();
    }
}
