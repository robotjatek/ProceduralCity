using System.Collections.Generic;
using OpenTK;
using ProceduralCity.Renderer;

namespace ProceduralCity
{
    class Building : IRenderable
    {
        private readonly List<Vector3> _vertices = new List<Vector3>();
        private readonly List<Vector2> _UVs = new List<Vector2>();

        public Texture Texture
        {
            get; private set;
        }

        public Shader Shader
        {
            get; private set;
        }

        public IEnumerable<Vector3> Vertices
        {
            get
            {
                return _vertices;
            }
        }

        public IEnumerable<Vector2> UVs
        {
            get
            {
                return _UVs;
            }
        }

        public Building(Vector3 position, Vector2 area, Texture texture, Shader shader, float height)
        {
            Texture = texture;
            Shader = shader;
            var maxHeight = height;

            //hátsó oldal
            _UVs.Add(new Vector2(area.X, 0));
            _vertices.Add(new Vector3(position.X + area.X, position.Y, position.Z));
            _UVs.Add(new Vector2(area.X, maxHeight));
            _vertices.Add(new Vector3(position.X + area.X, position.Y + maxHeight, position.Z));
            _UVs.Add(new Vector2(0, maxHeight));
            _vertices.Add(new Vector3(position.X, position.Y + maxHeight, position.Z));
            _UVs.Add(new Vector2(area.X, 0));
            _vertices.Add(new Vector3(position.X + area.X, position.Y, position.Z));
            _UVs.Add(new Vector2(0, maxHeight));
            _vertices.Add(new Vector3(position.X, position.Y + maxHeight, position.Z));
            _UVs.Add(new Vector2(0, 0));
            _vertices.Add(new Vector3(position.X, position.Y, position.Z));

            //első oldal
            _UVs.Add(new Vector2(0, 0));
            _vertices.Add(new Vector3(position.X, position.Y, position.Z + area.Y));
            _UVs.Add(new Vector2(0, maxHeight));
            _vertices.Add(new Vector3(position.X, position.Y + maxHeight, position.Z + area.Y));
            _UVs.Add(new Vector2(area.X, 0));
            _vertices.Add(new Vector3(position.X + area.X, position.Y, position.Z + area.Y));
            _UVs.Add(new Vector2(0, maxHeight));
            _vertices.Add(new Vector3(position.X, position.Y + maxHeight, position.Z + area.Y));
            _UVs.Add(new Vector2(area.X, maxHeight));
            _vertices.Add(new Vector3(position.X + area.X, position.Y + maxHeight, position.Z + area.Y));
            _UVs.Add(new Vector2(area.X, 0));
            _vertices.Add(new Vector3(position.X + area.X, position.Y, position.Z + area.Y));

            //jobb oldal
            _UVs.Add(new Vector2(0, 0));
            _vertices.Add(new Vector3(position.X + area.X, position.Y, position.Z + area.Y));
            _UVs.Add(new Vector2(0, maxHeight));
            _vertices.Add(new Vector3(position.X + area.X, position.Y + maxHeight, position.Z + area.Y));
            _UVs.Add(new Vector2(area.Y, 0));
            _vertices.Add(new Vector3(position.X + area.X, position.Y, position.Z));
            _UVs.Add(new Vector2(0, maxHeight));
            _vertices.Add(new Vector3(position.X + area.X, position.Y + maxHeight, position.Z + area.Y));
            _UVs.Add(new Vector2(area.Y, maxHeight));
            _vertices.Add(new Vector3(position.X + area.X, position.Y + maxHeight, position.Z));
            _UVs.Add(new Vector2(area.Y, 0));
            _vertices.Add(new Vector3(position.X + area.X, position.Y, position.Z));

            //bal oldal
            _UVs.Add(new Vector2(area.Y, 0));
            _vertices.Add(new Vector3(position.X, position.Y, position.Z));
            _UVs.Add(new Vector2(area.Y, maxHeight));
            _vertices.Add(new Vector3(position.X, position.Y + maxHeight, position.Z));
            _UVs.Add(new Vector2(0, maxHeight));
            _vertices.Add(new Vector3(position.X, position.Y + maxHeight, position.Z + area.Y));
            _UVs.Add(new Vector2(area.Y, 0));
            _vertices.Add(new Vector3(position.X, position.Y, position.Z));
            _UVs.Add(new Vector2(0, maxHeight));
            _vertices.Add(new Vector3(position.X, position.Y + maxHeight, position.Z + area.Y));
            _UVs.Add(new Vector2(0, 0));
            _vertices.Add(new Vector3(position.X, position.Y, position.Z + area.Y));

            //felső oldal
            _UVs.Add(new Vector2(0, 0));
            _vertices.Add(new Vector3(position.X, position.Y + maxHeight, position.Z + area.Y));
            _UVs.Add(new Vector2(0, 0));
            _vertices.Add(new Vector3(position.X, position.Y + maxHeight, position.Z));
            _UVs.Add(new Vector2(0, 0));
            _vertices.Add(new Vector3(position.X + area.X, position.Y + maxHeight, position.Z + area.Y));
            _UVs.Add(new Vector2(0, 0));
            _vertices.Add(new Vector3(position.X, position.Y + maxHeight, position.Z));
            _UVs.Add(new Vector2(0, 0));
            _vertices.Add(new Vector3(position.X + area.X, position.Y + maxHeight, position.Z));
            _UVs.Add(new Vector2(0, 0));
            _vertices.Add(new Vector3(position.X + area.X, position.Y + maxHeight, position.Z + area.Y));
        }
    }
}
