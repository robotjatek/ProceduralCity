using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Mathematics;

using ProceduralCity.Renderer.Uniform;
using Serilog;

namespace ProceduralCity.Renderer
{
    class ProceduralSkybox : ISkybox
    {
        private readonly ILogger _logger;
        private readonly Random _random = new Random();
        private readonly List<Mesh> _meshes = new List<Mesh>();
        private readonly Shader _shader;
        private readonly Vector3[] _cloudColors = new[]
        {
            new Vector3(0, 1.0f, 0), //green
            new Vector3(0, 0, 1.0f), //blue
            new Vector3(1, 0, 0), //red
            new Vector3(0.59f, 0.29f, 0), //brown
            new Vector3(1.0f, 0.50f, 0), //orange
            new Vector3(0.6f, 0, 0.6f), //purple
            new Vector3(1.0f, 1.0f, 0.1f), //yellow
            new Vector3(1.0f, 0.2f, 0.33f), //"radical red"
        };

        public IEnumerable<Mesh> Meshes
        {
            get
            {
                return _meshes;
            }
        }

        public ProceduralSkybox(ILogger logger)
        {
            _logger = logger;
            _shader = new Shader("skybox/skybox.vert", "skybox/proceduralSkybox.frag");
            GenerateSky();

            var vertices = CreateVertices();
            var uvs = Enumerable.Empty<Vector2>();

            _meshes.Add(new Mesh(vertices, uvs, _shader));
        }

        public void Update()
        {
            GenerateSky();
        }

        private void GenerateSky()
        {
            var (color1, color2) = GenerateCloudColors();
            var skyColor = Vector3.Clamp(MixCloudColors(color1, color2), new Vector3(0), new Vector3(1));
            SetSkyColor(skyColor, skyColor * 0.15f);
            SetCloudCutoffValue();
            SetSeeds();
        }

        private void SetSeeds()
        {
            var x = (float)(_random.NextDouble() * 1000f);
            var y = (float)(_random.NextDouble() * 1000f);
            var z = (float)(_random.NextDouble() * 1000f);
            var scale = (float)(_random.NextDouble() * 10000f);
            _logger.Information($"Setting sky seed values: x:{x}, y:{y}, z:{z}, scale:{scale}");

            _shader.SetUniformValue("u_seed_x", new FloatUniform
            {
                Value = x
            });

            _shader.SetUniformValue("u_seed_y", new FloatUniform
            {
                Value = y
            });

            _shader.SetUniformValue("u_seed_z", new FloatUniform
            {
                Value = z
            });

            _shader.SetUniformValue("u_seed_scale", new FloatUniform
            {
                Value = scale
            });
        }

        private Vector3 MixCloudColors(Vector3 color1, Vector3 color2)
        {
            return (color1 + color2) * 0.3f;
        }

        private void SetSkyColor(Vector3 bottomColor, Vector3 topColor)
        {
            _shader.SetUniformValue("u_sky_bottom_color", new Vector3Uniform
            {
                Value = bottomColor
            });
            _shader.SetUniformValue("u_sky_top_color", new Vector3Uniform
            {
                Value = topColor
            });

            _logger.Information($"Sky top color set to: {topColor}");
            _logger.Information($"Sky bottom color set to: {bottomColor}");
        }

        private void SetCloudCutoffValue()
        {
            var cutoff = _random.Next(450, 650) / 1000.0f;
            _shader.SetUniformValue("u_cloud_cutoff", new FloatUniform
            {
                Value = cutoff
            });
            _logger.Information($"Cloud cutoff value set to: {cutoff}");
        }

        private (Vector3 color1, Vector3 color2) GenerateCloudColors()
        {
            var cloudColor1 = _cloudColors[_random.Next(0, _cloudColors.Length)];
            _shader.SetUniformValue("u_cloud_color_1", new Vector3Uniform
            {
                Value = cloudColor1
            });

            var cloudColor2 = _cloudColors[_random.Next(0, _cloudColors.Length)];
            _shader.SetUniformValue("u_cloud_color_2", new Vector3Uniform
            {
                Value = cloudColor2
            });

            _logger.Information($"Generated cloud colors: {cloudColor1}, {cloudColor2}");
            return (cloudColor1, cloudColor2);
        }

        private IEnumerable<Vector3> CreateVertices()
        {
            return new[]
            {
                new Vector3(-1.0f,  1.0f, -1.0f),
                new Vector3(-1.0f, -1.0f, -1.0f),
                new Vector3(1.0f, -1.0f, -1.0f),
                new Vector3( 1.0f, -1.0f, -1.0f),
                new Vector3(1.0f,  1.0f, -1.0f),
                new Vector3(-1.0f,  1.0f, -1.0f),

                new Vector3(-1.0f, -1.0f,  1.0f),
                new Vector3(-1.0f, -1.0f, -1.0f),
                new Vector3( -1.0f,  1.0f, -1.0f),
                new Vector3(-1.0f,  1.0f, -1.0f),
                new Vector3(-1.0f,  1.0f,  1.0f),
                new Vector3(-1.0f, -1.0f,  1.0f),

                new Vector3(1.0f, -1.0f, -1.0f),
                new Vector3(1.0f, -1.0f,  1.0f),
                new Vector3(1.0f,  1.0f,  1.0f),
                new Vector3(1.0f,  1.0f,  1.0f),
                new Vector3(1.0f,  1.0f, -1.0f),
                new Vector3(1.0f, -1.0f, -1.0f),

                new Vector3(-1.0f, -1.0f,  1.0f),
                new Vector3(-1.0f,  1.0f,  1.0f),
                new Vector3(1.0f,  1.0f,  1.0f),
                new Vector3(1.0f,  1.0f,  1.0f),
                new Vector3(1.0f, -1.0f,  1.0f),
                new Vector3(-1.0f, -1.0f,  1.0f),

                new Vector3(-1.0f,  1.0f, -1.0f),
                new Vector3(1.0f,  1.0f, -1.0f),
                new Vector3(1.0f,  1.0f,  1.0f),
                new Vector3(1.0f,  1.0f,  1.0f),
                new Vector3(-1.0f,  1.0f,  1.0f),
                new Vector3(-1.0f,  1.0f, -1.0f),

                new Vector3(-1.0f, -1.0f, -1.0f),
                new Vector3(-1.0f, -1.0f,  1.0f),
                new Vector3(1.0f, -1.0f, -1.0f),
                new Vector3(1.0f, -1.0f, -1.0f),
                new Vector3(-1.0f, -1.0f,  1.0f),
                new Vector3(1.0f, -1.0f,  1.0f)
            };
        }

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _shader.Dispose();
                }

                disposedValue = true;
            }
        }

        ~ProceduralSkybox()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
