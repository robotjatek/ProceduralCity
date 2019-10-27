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

        private void AddToScene(IRenderable r)
        {
            var textureId = r.Texture != null ? r.Texture.Id : 0;
            var shaderId = r.Shader._programId;
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
