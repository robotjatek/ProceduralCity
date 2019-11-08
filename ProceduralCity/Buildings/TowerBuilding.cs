using System.Collections.Generic;
using OpenTK;
using ProceduralCity.Renderer;

namespace ProceduralCity.Buildings
{
    class TowerBuilding : IBuilding
    {
        private readonly List<Vector3> _vertices = new List<Vector3>();

        private readonly List<Vector2> _UVs = new List<Vector2>();

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

        public ITexture Texture { get; private set; }

        public Shader Shader { get; private set; }

        public TowerBuilding(Vector3 position, Vector2 area, Texture texture, Shader shader, float height)
        {
            Texture = texture;
            Shader = shader;

            var blockToppingArea = new Vector2(area.X + 3, area.Y + 3);
            var lastPosition = new Vector3(position);

            while (height - 8 > 0)
            {
                var blockToppingPosition = new Vector3(lastPosition.X - 1.5f, lastPosition.Y, lastPosition.Z - 1.5f);
                blockToppingPosition.Y += 5;

                CreateTexturedCube(lastPosition, area, 5);
                CreateUntexturedCube(blockToppingPosition, blockToppingArea, 3);

                lastPosition.Y += 8;
                height -= 8;
            }
        }

        private void CreateCubeVertices(Vector3 position, Vector2 area, float height)
        {
            //hátsó oldal
            _vertices.Add(new Vector3(position.X + area.X, position.Y, position.Z));
            _vertices.Add(new Vector3(position.X + area.X, position.Y + height, position.Z));
            _vertices.Add(new Vector3(position.X, position.Y + height, position.Z));
            _vertices.Add(new Vector3(position.X + area.X, position.Y, position.Z));
            _vertices.Add(new Vector3(position.X, position.Y + height, position.Z));
            _vertices.Add(new Vector3(position.X, position.Y, position.Z));

            //első oldal
            _vertices.Add(new Vector3(position.X, position.Y, position.Z + area.Y));
            _vertices.Add(new Vector3(position.X, position.Y + height, position.Z + area.Y));
            _vertices.Add(new Vector3(position.X + area.X, position.Y, position.Z + area.Y));
            _vertices.Add(new Vector3(position.X, position.Y + height, position.Z + area.Y));
            _vertices.Add(new Vector3(position.X + area.X, position.Y + height, position.Z + area.Y));
            _vertices.Add(new Vector3(position.X + area.X, position.Y, position.Z + area.Y));

            //jobb oldal
            _vertices.Add(new Vector3(position.X + area.X, position.Y, position.Z + area.Y));
            _vertices.Add(new Vector3(position.X + area.X, position.Y + height, position.Z + area.Y));
            _vertices.Add(new Vector3(position.X + area.X, position.Y, position.Z));
            _vertices.Add(new Vector3(position.X + area.X, position.Y + height, position.Z + area.Y));
            _vertices.Add(new Vector3(position.X + area.X, position.Y + height, position.Z));
            _vertices.Add(new Vector3(position.X + area.X, position.Y, position.Z));

            //bal oldal
            _vertices.Add(new Vector3(position.X, position.Y, position.Z));
            _vertices.Add(new Vector3(position.X, position.Y + height, position.Z));
            _vertices.Add(new Vector3(position.X, position.Y + height, position.Z + area.Y));
            _vertices.Add(new Vector3(position.X, position.Y, position.Z));
            _vertices.Add(new Vector3(position.X, position.Y + height, position.Z + area.Y));
            _vertices.Add(new Vector3(position.X, position.Y, position.Z + area.Y));

            //felső oldal
            _vertices.Add(new Vector3(position.X, position.Y + height, position.Z + area.Y));
            _vertices.Add(new Vector3(position.X, position.Y + height, position.Z));
            _vertices.Add(new Vector3(position.X + area.X, position.Y + height, position.Z + area.Y));
            _vertices.Add(new Vector3(position.X, position.Y + height, position.Z));
            _vertices.Add(new Vector3(position.X + area.X, position.Y + height, position.Z));
            _vertices.Add(new Vector3(position.X + area.X, position.Y + height, position.Z + area.Y));

            //alsó oldal     
            _vertices.Add(new Vector3(position.X + area.X, position.Y, position.Z + area.Y));
            _vertices.Add(new Vector3(position.X + area.X, position.Y, position.Z));
            _vertices.Add(new Vector3(position.X, position.Y, position.Z));
            _vertices.Add(new Vector3(position.X + area.X, position.Y, position.Z + area.Y));
            _vertices.Add(new Vector3(position.X, position.Y, position.Z));
            _vertices.Add(new Vector3(position.X, position.Y, position.Z + area.Y));
        }

        private void CreateCubeUVs()
        {
            //hátsó oldal
            _UVs.Add(new Vector2(1, 0));
            _UVs.Add(new Vector2(1, 1));
            _UVs.Add(new Vector2(0, 1));
            _UVs.Add(new Vector2(1, 0));
            _UVs.Add(new Vector2(0, 1));
            _UVs.Add(new Vector2(0, 0));

            //első oldal
            _UVs.Add(new Vector2(0, 0));
            _UVs.Add(new Vector2(0, 1));
            _UVs.Add(new Vector2(1, 0));
            _UVs.Add(new Vector2(0, 1));
            _UVs.Add(new Vector2(1, 1));
            _UVs.Add(new Vector2(1, 0));

            //jobb oldal
            _UVs.Add(new Vector2(0, 0));
            _UVs.Add(new Vector2(0, 1));
            _UVs.Add(new Vector2(1, 0));
            _UVs.Add(new Vector2(0, 1));
            _UVs.Add(new Vector2(1, 1));
            _UVs.Add(new Vector2(1, 0));

            //bal oldal
            _UVs.Add(new Vector2(1, 0));
            _UVs.Add(new Vector2(1, 1));
            _UVs.Add(new Vector2(0, 1));
            _UVs.Add(new Vector2(1, 0));
            _UVs.Add(new Vector2(0, 1));
            _UVs.Add(new Vector2(0, 0));

            //felső oldal
            _UVs.Add(new Vector2(0, 0));
            _UVs.Add(new Vector2(0, 0));
            _UVs.Add(new Vector2(0, 0));
            _UVs.Add(new Vector2(0, 0));
            _UVs.Add(new Vector2(0, 0));
            _UVs.Add(new Vector2(0, 0));

            //alsó oldal     
            _UVs.Add(new Vector2(0, 0));
            _UVs.Add(new Vector2(0, 0));
            _UVs.Add(new Vector2(0, 0));
            _UVs.Add(new Vector2(0, 0));
            _UVs.Add(new Vector2(0, 0));
            _UVs.Add(new Vector2(0, 0));
        }

        private void CreateZeroCubeUVs()
        {
            //hátsó oldal
            _UVs.Add(new Vector2(0, 0));
            _UVs.Add(new Vector2(0, 0));
            _UVs.Add(new Vector2(0, 0));
            _UVs.Add(new Vector2(0, 0));
            _UVs.Add(new Vector2(0, 0));
            _UVs.Add(new Vector2(0, 0));

            //első oldal
            _UVs.Add(new Vector2(0, 0));
            _UVs.Add(new Vector2(0, 0));
            _UVs.Add(new Vector2(0, 0));
            _UVs.Add(new Vector2(0, 0));
            _UVs.Add(new Vector2(0, 0));
            _UVs.Add(new Vector2(0, 0));

            //jobb oldal
            _UVs.Add(new Vector2(0, 0));
            _UVs.Add(new Vector2(0, 0));
            _UVs.Add(new Vector2(0, 0));
            _UVs.Add(new Vector2(0, 0));
            _UVs.Add(new Vector2(0, 0));
            _UVs.Add(new Vector2(0, 0));

            //bal oldal
            _UVs.Add(new Vector2(0, 0));
            _UVs.Add(new Vector2(0, 0));
            _UVs.Add(new Vector2(0, 0));
            _UVs.Add(new Vector2(0, 0));
            _UVs.Add(new Vector2(0, 0));
            _UVs.Add(new Vector2(0, 0));

            //felső oldal
            _UVs.Add(new Vector2(0, 0));
            _UVs.Add(new Vector2(0, 0));
            _UVs.Add(new Vector2(0, 0));
            _UVs.Add(new Vector2(0, 0));
            _UVs.Add(new Vector2(0, 0));
            _UVs.Add(new Vector2(0, 0));

            //alsó oldal     
            _UVs.Add(new Vector2(0, 0));
            _UVs.Add(new Vector2(0, 0));
            _UVs.Add(new Vector2(0, 0));
            _UVs.Add(new Vector2(0, 0));
            _UVs.Add(new Vector2(0, 0));
            _UVs.Add(new Vector2(0, 0));
        }

        private void CreateTexturedCube(Vector3 position, Vector2 area, float height)
        {
            CreateCubeUVs();
            CreateCubeVertices(position, area, height);
        }

        private void CreateUntexturedCube(Vector3 position, Vector2 area, float height)
        {
            CreateCubeVertices(position, area, height);
            CreateZeroCubeUVs();
        }
    }
}
