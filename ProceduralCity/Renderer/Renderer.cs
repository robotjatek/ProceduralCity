﻿using System;
using System.Collections.Generic;
using OpenTK;

namespace ProceduralCity.Renderer
{
    class Renderer : IDisposable
    {
        private readonly Dictionary<(int textureId, int shaderId), ObjectBatch> _batches = new Dictionary<(int, int), ObjectBatch>();

        public void RenderScene(Matrix4 mvp)
        {
            foreach (var batch in _batches.Values)
            {
                batch.Draw(mvp);
            }
        }

        public void AddToScene(IRenderable r)
        {
            var textureId = r.GetTexture() != null ? r.GetTexture().Id : 0;
            var shaderId = r.GetShader().ProgramId;
            var key = (textureId, shaderId);

            if (_batches.TryGetValue(key, out ObjectBatch batch))
            {
                batch.AddRenderable(r);
            }
            else
            {
                var toAdd = new ObjectBatch(r.GetShader(), r.GetTexture());
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
