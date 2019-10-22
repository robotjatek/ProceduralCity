using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using OpenTK;
using ProceduralCity.Config;
using ProceduralCity.Renderer;

namespace ProceduralCity
{
    class Textbox : IDisposable
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

        public Textbox(string fontName)
        {
            _fontmap = new Texture($"{fontName}/font.png", "Fonts");
            _fontConfig = new FontConfig(fontName);
            _shader = new Shader("vs.vert", "fs.frag");
        }

        public Textbox WithText(string text)
        {
            _text.Clear();
            var cursorX = 0.0f;
            var maxCharacterHeight = _fontConfig.Characters.Max(c => c.Value.Height);

            foreach (var character in text)
            {
                var characterConfig = _fontConfig.Characters[character.ToString(CultureInfo.InvariantCulture)];
                var renderableChar = new Character(_shader, _fontmap, characterConfig, new Vector2(0 + cursorX, maxCharacterHeight), 1.0f);
                _text.Add(renderableChar);
                cursorX += renderableChar.Advance;
            }

            return this;
        }

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _shader.Dispose();
                    _fontmap.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
