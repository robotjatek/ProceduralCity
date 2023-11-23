using System.Collections.Generic;
using OpenTK.Mathematics;

using ProceduralCity.Renderer;
using ProceduralCity.Utils;

namespace ProceduralCity.GameObjects
{
    class TrafficLight : IRenderable // TODO: inferface for bounding volume?
    {
        private readonly float _speed;
        private static readonly Vector3 UP = new(0, 1, 0);
        private Vector3 _position;
        private Waypoint _target;
        private readonly Shader _headlightShader; //TODO: disable backface culling on this and use 1 mesh only with 1 shader
        private readonly Shader _rearLightShader;
        private readonly List<Mesh> _meshes = [];

        public Matrix4 Model { get; private set; } = Matrix4.Identity;

        public IEnumerable<Mesh> Meshes => _meshes;

        public Vector3 Position { get { return _position; } }

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
            // PrimitiveUtils.CreateBacksideVertices was originally meant to create 3D objects in view space. Here it is abused to create a 2D object in model space
            var vertices = PrimitiveUtils.CreateBacksideVertices( 
                position: new Vector3(0), // Initial vertex position does not matter when constructing the object, because we handle this object as if it is in model space (as every other should be)
                area: new Vector2(2, 0), // First parameter of the area is the WIDTH. As this function is a 3D object creator function, the second argument of the "area" has no meaining here.
                height: 1); 
            var uvs = PrimitiveUtils.CreateBackUVs(); // TODO: kellenek UV-k? Empty array miért nem jó? => empty array-jel nem rendereli ki...

            var mesh = new Mesh(vertices, uvs, _headlightShader)
            {
                IsInstanced = true
            };
            _meshes.Add(mesh);
        }

        private void CreateBackLight()
        {
            var vertices = PrimitiveUtils.CreateFrontVertices(new Vector3(0), new Vector2(2, 0), 1);
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
