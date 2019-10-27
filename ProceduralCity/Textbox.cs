using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using OpenTK;
using ProceduralCity.Config;
using ProceduralCity.Extensions;
using ProceduralCity.Renderer;
using ProceduralCity.Renderer.Uniform;

namespace ProceduralCity
{
    class Textbox : IDisposable
    {
        private readonly List<IRenderable> _text = new List<IRenderable>();
        private readonly Texture _fontmap;
        private readonly Shader _shader;
        private readonly FontConfig _fontConfig;
        private float _hue;
        private float _saturation;
        private float _value;

        public IEnumerable<IRenderable> Text
        {
            get
            {
                return _text;
            }
        }

        public float Hue
        {
            get
            {
                return _hue;
            }
            set
            {
                _hue = value.Clamp();

                _shader.SetUniformValue("hue", new FloatUniform
                {
                    Value = _hue
                });
            }
        }
        public float Saturation
        {
            get
            {
                return _saturation;
            }
            set
            {
                _saturation = value.Clamp();

                _shader.SetUniformValue("saturation", new FloatUniform
                {
                    Value = _saturation
                });
            }
        }

        public float Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value.Clamp();
                _shader.SetUniformValue("value", new FloatUniform
                {
                    Value = _value
                });
            }
        }

        public Textbox(string fontName)
        {
            _fontmap = new Texture($"{fontName}/font.png", "Fonts");
            _fontConfig = new FontConfig(fontName);
            _shader = new Shader("vs.vert", "font.frag");
            _shader.Use();
            Hue = 1;
            Saturation = 0;
            Value = 1;
        }

        public Textbox WithText(string text, Vector2 position, float scale)
        {
            _text.Clear();
            var cursorX = 0.0f;
            var maxCharacterHeight = _fontConfig.Characters.Max(c => c.Value.Height);

            foreach (var character in text)
            {
                var characterConfig = _fontConfig.Characters[character.ToString(CultureInfo.InvariantCulture)];
                var renderableChar = new Character(_shader, _fontmap, characterConfig, new Vector2(position.X + cursorX, position.Y + maxCharacterHeight), scale);
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
