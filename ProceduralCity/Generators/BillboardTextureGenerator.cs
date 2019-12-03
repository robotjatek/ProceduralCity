﻿using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using ProceduralCity.Config;
using ProceduralCity.Renderer;
using Serilog;

namespace ProceduralCity.Generators
{
    class BillboardTextureGenerator : IBillboardTextureGenerator
    {
        private readonly Random _random = new Random();
        private readonly ILogger _logger;
        private readonly IAppConfig _config;
        private readonly Matrix4 _projectionMatrix;

        private static readonly string[] _prefixes = new[]
        {
            "i",
            "my",
            "Mega ",
            "Super ",
            "National ",
            "Federal "
        };

        private static readonly string[] _first = new[]
        {
            "Global",
            "Internet",
            "Vortex",
            "Gravity",
            "Space",
            "Cloud",
            "Dream",
            "Karma",
            "Wizard",
            "Apple",
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
        };

        private static readonly string[] _second = new[]
        {
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
        };

        public BillboardTextureGenerator(ILogger logger, IAppConfig config)
        {
            _config = config;
            _logger = logger;
            _projectionMatrix = Matrix4.CreateOrthographicOffCenter(0, _config.BillboardTextureWidth, _config.BillboardTextureHeight, 0, -1, 1);
        }

        public void GenerateBillboardTextures(IEnumerable<Texture> textures)
        {
            _logger.Information($"Generating billboard textures.");

            foreach (var tex in textures)
            {
                var word = GenerateBillboardText();

                using (var text = new Textbox("Consolas")
                    .WithText(word, new Vector2(), 1.5f)
                    .WithHue((float)_random.NextDouble())
                    .WithSaturation((float)_random.NextDouble()))
                using (var renderer = new Renderer.Renderer())
                using (var backbufferRenderer = new BackBufferRenderer(_logger, tex, tex.Width, tex.Height, false))
                {
                    renderer.BeforeRender = () => { GL.Enable(EnableCap.Blend); };
                    renderer.AfterRender = () => { GL.Disable(EnableCap.Blend); };
                    renderer.AddToScene(text.Text);
                    backbufferRenderer.RenderToTexture(renderer, _projectionMatrix, Matrix4.Identity, Matrix4.Identity);

                    if (text.CursorAdvance > tex.Width)
                    {
                        _logger.Warning($"The billboard text is wider than the texture width! ({word})");
                    }
                }
            }
        }

        private string GenerateBillboardText()
        {
            var prefix = _prefixes[_random.Next(_prefixes.Length)];
            var first = _first[_random.Next(_first.Length)];
            var second = _second[_random.Next(_second.Length)];

            var word = _random.Next() % 2 == 0 ? $"{prefix}{first}" : $"{first} {second}";
            _logger.Debug($"Billboard text: {word}");
            return word;
        }
    }
}