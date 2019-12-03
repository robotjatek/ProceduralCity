using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using ProceduralCity.Extensions;

namespace ProceduralCity.Renderer
{
    class Renderer : IRenderer, IDisposable
    {
        private readonly Dictionary<(int textureId, int shaderId), ObjectBatch> _batches = new Dictionary<(int, int), ObjectBatch>();

        public Action BeforeRender { get; set; }

        public Action AfterRender { get; set; }

        public void RenderScene(Matrix4 projection, Matrix4 view, Matrix4 model)
        {
            BeforeRender?.Invoke();
            foreach (var batch in _batches.Values)
            {
                batch.Draw(projection, view, model);
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
                var textureHash = mesh.Textures.Select(t => t.Id).CalculateHash();
                var shaderId = mesh.Shader.ProgramId;
                var key = (textureHash, shaderId);

                if (_batches.TryGetValue(key, out ObjectBatch batch))
                {
                    batch.AddMesh(mesh);
                }
                else
                {
                    var toAdd = new ObjectBatch(mesh.Shader, mesh.Textures);
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
    }
}
