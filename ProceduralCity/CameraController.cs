/*
CameraController class

Known inputs
- size of the world
- buildings max heigth
- coordinates of the city center can be calcualted

High level logic v1:

Done:
Select a random position between the city borders
Look in the direction of the city center
After some time teleport to a new position
Move forward towards the city center
If the city center is close move backwards, if far move forward
Rotate around the city center

In progress:
- Fade out-Fade-in effect when teleporting

To Do:
- Select a random movement and move the camera
    - Move while looking down
    - Rotate around an arbitrary position/object
    - Follow a random path (curves?)
- Toggle between flyby and user camera
 */
using OpenTK.Mathematics;

using ProceduralCity.Config;

using System;

namespace ProceduralCity
{
    enum MovementType
    {
        STRAIGHT,
        STAND,
        ROTATE,
    }

    enum Direction
    {
        A,B
    }

    internal class CameraController
    {
        private readonly ICamera _camera;
        private readonly IAppConfig _configuration;
        private readonly Vector3 _cityCenterPosition;
        private readonly Random _random = new();

        private float _elapsedTimeSinceLastTeleport = 0;
        private readonly double _maxDistance;
        private MovementType _chosenMovement;
        private Direction _direction;

        public CameraController(ICamera camera, IAppConfig configuration)
        {
            _camera = camera;
            _configuration = configuration;
            _cityCenterPosition = new Vector3(_configuration.WorldSize / 2, _configuration.MaxBuildingHeight, _configuration.WorldSize / 2);
            _maxDistance = _configuration.WorldSize * MathHelper.Sqrt(2) * 0.3f;
        }

        public void Update(float deltaTime)
        {
            _elapsedTimeSinceLastTeleport += deltaTime;
            if (_elapsedTimeSinceLastTeleport > 10)
            {
                TeleportToNewPosition();
                _elapsedTimeSinceLastTeleport = 0;
            }

            // TODO: command pattern
            if (_chosenMovement == MovementType.STRAIGHT)
            {

                if(_direction == Direction.A)
                {
                    _camera.MoveForward(deltaTime);
                }
                else if (_direction == Direction.B)
                {
                    _camera.MoveBackward(deltaTime);
                }
            }
            else if (_chosenMovement == MovementType.ROTATE)
            {
                if(_direction == Direction.A)
                {
                    _camera.StrafeLeft(deltaTime);
                }
                else
                {
                    _camera.StrafeRight(deltaTime);
                }
                LookAtCityCenter();
            } 

        }

        //TODO: make this private
        public void TeleportToNewPosition()
        {
            var rnd = new Random();
            // x, height, y;
            var height = _configuration.MaxBuildingHeight + 10.0f;
            var nx = (float)rnd.NextDouble() * _configuration.WorldSize;
            var ny = (float)rnd.NextDouble() * _configuration.WorldSize;

            var pos = new Vector3(nx, height, ny);
            _camera.SetPosition(pos);
            LookAtCityCenter();
            PickMovement();

        }

        // Picks a movement mode and direction which the camera will follow until the next state change
        private void PickMovement()
        {
            var distanceToCityCenter = (_cityCenterPosition - _camera.GetPosition()).Length;
            var enumValues = Enum.GetValues(typeof(MovementType));
            var mode = (MovementType)enumValues.GetValue(_random.Next(enumValues.Length));
            _chosenMovement = mode;

            if (mode == MovementType.STRAIGHT)
            {
                _direction = distanceToCityCenter > _maxDistance ? Direction.A : Direction.B;
            }
            else if (mode == MovementType.ROTATE)
            {
                _direction = (Direction) _random.Next(2);
            }
        }

        private void LookAtCityCenter()
        {
            _camera.LookAt(_cityCenterPosition);
        }
    }
}
