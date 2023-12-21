using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OpenTK.Mathematics;

using ProceduralCity.Generators;
using ProceduralCity.Renderer.Uniform;
using ProceduralCity.Utils;

using Serilog;

namespace ProceduralCity.Renderer
{
    class ProceduralSkybox : ISkybox
    {
        private readonly ILogger _logger;
        private readonly List<Mesh> _meshes = [];
        private readonly Shader _shader;
        private readonly ColorGenerator _colorGenerator;
        private readonly RandomService _randomService;

        public IEnumerable<Mesh> Meshes
        {
            get
            {
                return _meshes;
            }
        }

        public ProceduralSkybox(ILogger logger, ColorGenerator colorGenerator, RandomService randomService)
        {
            _logger = logger;
            _shader = new Shader("skybox/skybox.vert", "skybox/proceduralSkybox.frag");
            _colorGenerator = colorGenerator;
            _randomService = randomService;

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
            var (color1, color2) = (_colorGenerator.Primary, _colorGenerator.Secondary);
            SetCloudColors(color1, color2);

            // This is the gradient color of the sky, without the clouds
            var skyColor = new Vector3(_colorGenerator.Mixed.R, _colorGenerator.Mixed.G, _colorGenerator.Mixed.B);
            SetSkyTransientColor(skyColor, skyColor * 0.15f);

            SetCloudCutoffValue();
            SetSeeds(); // Sets the seeds for the value noise algorithm
        }

        private void SetSeeds()
        {
            var x = (float)(_randomService.NextDouble() * 1000f);
            var y = (float)(_randomService.NextDouble() * 1000f);
            var z = (float)(_randomService.NextDouble() * 1000f);
            var scale = (float)(_randomService.NextDouble() * 10000f);
            _logger.Information("Setting sky seed values: x: {x}, y: {y}, z: {z}, scale: {scale}", x, y, z, scale);

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

        private void SetSkyTransientColor(Vector3 bottomColor, Vector3 topColor)
        {
            _shader.SetUniformValue("u_sky_bottom_color", new Vector3Uniform
            {
                Value = bottomColor
            });
            _shader.SetUniformValue("u_sky_top_color", new Vector3Uniform
            {
                Value = topColor
            });

            _logger.Information("Sky top color set to: {topColor}", topColor);
            _logger.Information("Sky bottom color set to: {bottomColor}", bottomColor);
        }

        private void SetCloudCutoffValue()
        {
            var cutoff = _randomService.Next(450, 650) / 1000.0f;
            _shader.SetUniformValue("u_cloud_cutoff", new FloatUniform
            {
                Value = cutoff
            });
            _logger.Information("Cloud cutoff value set to: {cutoff}", cutoff);
        }

        private void SetCloudColors(Color4 color1, Color4 color2)
        {
            _shader.SetUniformValue("u_cloud_color_1", new Vector3Uniform
            {
                Value = new Vector3(color1.R, color1.G, color1.B)
            });

            _shader.SetUniformValue("u_cloud_color_2", new Vector3Uniform
            {
                Value = new Vector3(color2.R, color2.G, color2.B)
            });

            _logger.Information("Cloud colors set to: {cloudColor1}, {cloudColor2}", color1, color2);
        }

        private static ReadOnlyCollection<Vector3> CreateVertices()
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
            }.AsReadOnly();
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
