﻿using System.Collections.Generic;
using OpenTK.Mathematics;

using ProceduralCity.Renderer;

namespace ProceduralCity.GameObjects
{
    class StreetLightStrip : IRenderable
    {
        private readonly List<Mesh> _meshes = [];

        public IEnumerable<Mesh> Meshes
        {
            get
            {
                return _meshes;
            }
        }

        public StreetLightStrip(IEnumerable<Vector3> vertices, IEnumerable<Vector2> uvs, Shader shader)
        {
            _meshes.Add(new Mesh(vertices, uvs, shader));
        }
    }
}
