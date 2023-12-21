using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

using ProceduralCity.Config;
using ProceduralCity.GameObjects;
using ProceduralCity.Renderer;
using ProceduralCity.Utils;

using Serilog;

namespace ProceduralCity.Generators
{
    class BillboardTextureGenerator : IBillboardTextureGenerator
    {
        private readonly RandomService _randomService;
        private readonly ILogger _logger;
        private readonly IAppConfig _config;
        private readonly Matrix4 _projectionMatrix;

        private static readonly string[] _prefixes =
        [
            "i",
            "my",
            "Mega ",
            "Super ",
            "National ",
            "Federal "
        ];

        private static readonly string[] _first =
        [
            "Global",
            "Internet",
            "Vortex",
            "Gravity",
            "Space",
            "Cloud",
            "Dream",
            "Karma",
            "Wizard",
            "Pear",
            "Micro",
            "Raven",
            "World",
            "Earth",
            "Alpha",
            "Omega",
            "Atlantic",
            "Pacific",
            "European",
            "Asian",
            "Massive",
            "Sun",
            "Wolf"
        ];

        private static readonly string[] _second =
        [
            "Industries",
            "Technologies",
            "Systems",
            "Inc",
            "Corp",
            "Solutions",
            "Electronics",
            "Ltd",
            "Microsystems",
            "Sports",
            "Records",
            "Music",
            "Security",
            "Motors",
            "Automotive",
            "Online",
            "Dynamic",
            "Enterprises",
            "USA",
            "Studios",
            "Bank"
        ];

        public BillboardTextureGenerator(ILogger logger, IAppConfig config, RandomService randomService)
        {
            _config = config;
            _logger = logger;
            _randomService = randomService;
            _projectionMatrix = Matrix4.CreateOrthographicOffCenter(0, _config.BillboardTextureWidth, _config.BillboardTextureHeight, 0, -1, 1);
        }

        public void GenerateBillboardTextures(IEnumerable<Texture> textures)
        {
            _logger.Information($"Generating billboard textures.");

            foreach (var tex in textures)
            {
                var word = GenerateBillboardText();

                using var text = new Textbox("Consolas")
                    .WithText(word, new Vector2(), 1.25f)
                    .WithHue(1.0f)
                    .WithSaturation(0)
                    .WithValue(1.0f);

                using var renderer = new Renderer.Renderer();
                using var backbufferRenderer = new BackBufferRenderer(_logger, tex, tex.Width, tex.Height, false);
                renderer.BeforeRender = () => { GL.Enable(EnableCap.Blend); };
                renderer.AfterRender = () => { GL.Disable(EnableCap.Blend); };
                renderer.AddToScene(text.Text);
                backbufferRenderer.RenderToTexture(renderer, _projectionMatrix, Matrix4.Identity);

                if (text.CursorAdvance > tex.Width)
                {
                    _logger.Warning("The billboard text is wider than the texture width! ({word})", word);
                }
            }
        }

        private string GenerateBillboardText()
        {
            var prefix = _prefixes[_randomService.Next(_prefixes.Length)];
            var first = _first[_randomService.Next(_first.Length)];
            var second = _second[_randomService.Next(_second.Length)];

            var word = _randomService.Next() % 2 == 0 ? $"{prefix}{first}" : $"{first} {second}";
            _logger.Debug("Billboard text: {word}", word);
            return word;
        }
    }
}
