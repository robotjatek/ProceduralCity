﻿using System;
using System.Collections.Generic;
using System.Linq;

using OpenTK;

using ProceduralCity.Extensions;

using Serilog;

namespace ProceduralCity.Renderer
{
    class Renderer : IRenderer, IDisposable
    {
        private readonly Dictionary<(int textureId, int shaderId), IBatch> _batches = new Dictionary<(int, int), IBatch>();

        public Action BeforeRender { get; set; }

        public Action AfterRender { get; set; }

        public void RenderScene(Matrix4 projection, Matrix4 view)
        {
            BeforeRender?.Invoke();
            foreach (var batch in _batches.Values)
            {
                batch.Draw(projection, view);
            }
            AfterRender?.Invoke();
        }

        public void AddToScene(IEnumerable<IRenderable> renderables)
        {
            foreach (var renderable in renderables)
            {
                AddToScene(renderable);
            }
        }

        public void AddToScene(IRenderable r)
        {
            foreach (var mesh in r.Meshes)
            {
                if (mesh.Shader == null)
                {
                    Log.Logger.Error($"Shader is null");
                    throw new ArgumentException();
                }

                var textureHash = mesh.Textures.Select(t => t.Id).CalculateHash();
                var shaderId = mesh.Shader.ProgramId;
                var key = (textureHash, shaderId);

                if (_batches.TryGetValue(key, out IBatch batch))
                {
                    batch.AddMesh(mesh);
                }
                else
                {
                    var toAdd = CreateBatch(mesh.IsInstanced, mesh.Shader, mesh.Textures);
                    toAdd.AddMesh(mesh);
                    _batches.Add(key, toAdd);
                }
            }
        }

        public void Clear()
        {
            foreach (var batch in _batches.Values)
            {
                batch.Dispose();
            }
            _batches.Clear();
        }

        public void Dispose()
        {
            foreach (var batch in _batches.Values)
            {
                batch.Dispose();
            }
        }

        private IBatch CreateBatch(bool instanced, Shader shader, IEnumerable<ITexture> textures)
        {
            if (instanced)
            {
                return new InstancedBatch(shader, textures);
            }

            return new ObjectBatch(shader, textures);
        }
    }
}
