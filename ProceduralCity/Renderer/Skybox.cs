﻿using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Mathematics;

using ProceduralCity.Renderer.Uniform;

namespace ProceduralCity.Renderer
{
    class Skybox : ISkybox, IRenderable, IDisposable
    {
        private readonly List<Mesh> _meshes = new();
        private readonly ITexture _texture;
        private readonly Shader _shader;

        public IEnumerable<Mesh> Meshes
        {
            get
            {
                return _meshes;
            }
        }

        public Skybox()
        {
            var filenames = new[]
            {
                "skybox/hq/right.jpg",
                "skybox/hq/left.jpg",
                "skybox/hq/top.jpg",
                "skybox/hq/bottom.jpg",
                "skybox/hq/back.jpg",
                "skybox/hq/front.jpg",
            };

            _texture = new CubemapTexture(filenames.ToList());
            _shader = new Shader("skybox/skybox.vert", "skybox/skybox.frag");
            _shader.SetUniformValue("skybox", new IntUniform
            {
                Value = 0
            });

            var vertices = CreateVertices();
            var uvs = Enumerable.Empty<Vector2>(); //TODO: seems like a hack (edit: yes its kinda a hack. ObjectBatch expects an uv list, null will crash the program. In the skybox shader itself no UVs are used.)

            _meshes.Add(new Mesh(vertices, uvs, new[] { _texture }, _shader));
        }

        private static IEnumerable<Vector3> CreateVertices()
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
                    _texture.Dispose();
                }

                disposedValue = true;
            }
        }

        ~Skybox()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Update()
        {
            //XXX: Could be used to implement day-night cycles, etc.
            throw new NotImplementedException();
        }
    }
}
