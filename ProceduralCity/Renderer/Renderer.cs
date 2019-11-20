using System;
using System.Collections.Generic;
using OpenTK;

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
                var textureId = mesh.Texture != null ? mesh.Texture.Id : 0; //TODO: add different mesh types for textured and untextured
                var shaderId = mesh.Shader.ProgramId;
                var key = (textureId, shaderId);

                if (_batches.TryGetValue(key, out ObjectBatch batch))
                {
                    batch.AddMesh(mesh);
                }
                else
                {
                    var toAdd = new ObjectBatch(mesh.Shader, mesh.Texture);
                    toAdd.AddMesh(mesh);
                    _batches.Add(key, toAdd);
                }
            }
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
