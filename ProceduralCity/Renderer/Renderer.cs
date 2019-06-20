using System;
using System.Collections.Generic;
using OpenTK;

namespace ProceduralCity.Renderer
{
    class Renderer : IDisposable
    {
        private readonly Dictionary<(int textureId, int shaderId), ObjectBatch> _batches = new Dictionary<(int, int), ObjectBatch>();

        public void RenderScene(Matrix4 projection, Matrix4 view, Matrix4 model)
        {
            foreach (var batch in _batches.Values)
            {
                batch.Draw(projection, view, model);
            }
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
            var textureId = r.Texture != null ? r.Texture.Id : 0;
            var shaderId = r.Shader.ProgramId;
            var key = (textureId, shaderId);

            if (_batches.TryGetValue(key, out ObjectBatch batch))
            {
                batch.AddRenderable(r);
            }
            else
            {
                var toAdd = new ObjectBatch(r.Shader, r.Texture);
                toAdd.AddRenderable(r);
                _batches.Add(key, toAdd);
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
