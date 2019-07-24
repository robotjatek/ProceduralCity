using System.Collections.Generic;
using ProceduralCity.Config;
using ProceduralCity.Renderer;

namespace ProceduralCity
{
    class Billboard
    {
        private readonly List<IRenderable> _text = new List<IRenderable>();
        private readonly Texture _fontmap;
        private readonly Shader _shader;
        private readonly FontConfig _fontConfig;

        public IEnumerable<IRenderable> Text
        {
            get
            {
                return _text;
            }
        }

        public Billboard(string fontName)
        {
            _fontmap = new Texture($"Fonts/{fontName}/font.png"); //TODO: textures are loaded from a different folder
            _fontConfig = new FontConfig(fontName);
        }

        public Billboard WithText(string text)
        {
            _text.Clear();
            foreach (var character in text)
            {
                var renderableChar = new Character(_shader, _fontmap);
                _text.Add(renderableChar);
            }

            return this;
        }
    }
}
