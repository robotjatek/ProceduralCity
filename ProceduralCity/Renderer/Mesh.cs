﻿using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Mathematics;

namespace ProceduralCity.Renderer
{
    public class Mesh
    {
        public IEnumerable<Vector3> Vertices { get; private set; }
        public IEnumerable<Vector2> UVs { get; private set; }
        public IEnumerable<ITexture> Textures { get; private set; } = Enumerable.Empty<ITexture>();
        public Shader Shader { get; private set; }

        public Mesh(IEnumerable<Vector3> vertices, IEnumerable<Vector2> uvs, Shader shader)
        {
            Vertices = vertices;
            UVs = uvs;
            Shader = shader;
        }

        public Mesh(IEnumerable<Vector3> vertices, IEnumerable<Vector2> uvs, IEnumerable<ITexture> textures, Shader shader)
        {
            Vertices = vertices;
            UVs = uvs;
            Textures = textures ?? throw new ArgumentNullException(nameof(textures), "Textures cannot be null!");
            Shader = shader;
        }
    }
}
