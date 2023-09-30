using System.Collections.Generic;
using OpenTK.Mathematics;

using ProceduralCity.Renderer;
using ProceduralCity.Utils;

namespace ProceduralCity.GameObjects
{
    class TrafficLight : IRenderable
    {
        private readonly float _speed;
        private static readonly Vector3 UP = new(0, 1, 0);
        private Vector3 _position;
        private Waypoint _target;
        private readonly Shader _headlightShader; //TODO: disable backface culling on this and use 1 mesh only with 1 shader
        private readonly Shader _rearLightShader;
        private readonly List<Mesh> _meshes = new();

        public Matrix4 Model { get; private set; } = Matrix4.Identity;

        public IEnumerable<Mesh> Meshes
        {
            get { return _meshes; }
        }

        public TrafficLight(Vector3 position, Waypoint target, Shader headLightShader, Shader rearLightShader, float speed)
        {
            _speed = speed;
            _position = position;
            _target = target;
            _headlightShader = headLightShader;
            _rearLightShader = rearLightShader;

            CreateHeadLight();
            CreateBackLight();
        }

        private void CreateHeadLight()
        {
            var vertices = PrimitiveUtils.CreateBacksideVertices(new Vector3(0), new Vector2(2), 1);
            var uvs = PrimitiveUtils.CreateBackUVs();

            var mesh = new Mesh(vertices, uvs, _headlightShader)
            {
                IsInstanced = true
            };
            _meshes.Add(mesh);
        }

        private void CreateBackLight()
        {
            var vertices = PrimitiveUtils.CreateFrontVertices(new Vector3(0), new Vector2(2), 1);
            var uvs = PrimitiveUtils.CreateFrontUvs();

            var mesh = new Mesh(vertices, uvs, _rearLightShader)
            {
                IsInstanced = true
            };
            _meshes.Add(mesh);
        }

        public void Move(float elapsedTime)
        {
            CalculateTarget();
            TransformObject(elapsedTime);

            _meshes.ForEach(a => a.Model.Value = Model);
        }

        private void TransformObject(float elapsedTime)
        {
            var direction = _target.Position - _position;
            direction.Normalize();
            var look = Matrix4.LookAt(_position, _target.Position, UP).Inverted();
            _position += direction * _speed * elapsedTime;
            Model = look;
        }

        private void CalculateTarget()
        {
            var distance = Vector3.Distance(_position, _target.Position);
            if (distance < 0.1f)
            {
                _target = _target.Next;
            }
        }
    }
}
