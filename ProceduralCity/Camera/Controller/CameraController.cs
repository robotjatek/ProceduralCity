/*
CameraController class

Known inputs
- size of the world
- buildings max height
- coordinates of the city center can be calculated

High level logic v1:

Done:
Select a random position between the city borders
Look in the direction of the city center
After some time teleport to a new position
Move forward towards the city center
If the city center is close move backwards, if far move forward
Rotate around the city center
Fade out-Fade-in effect when teleporting
Toggle between flyby and user camera
Command pattern for movement commands
Move on a plane while looking down at a certain angle
Strafe with vertical angle parameter set

In progress:
- Follow a random path with street level shots too (curves?)
    -- Inject IWorld instance
    -- IWorld should contain the sites BSP tree
    -- Based on the BSP tree calculate a random street coordinate on the street level
    -- Create a path on the street level
    -- Move the camera along the path


To Do:
- Select a random movement and move the camera
    - Rotate around an arbitrary position/object
 */
using OpenTK.Mathematics;

using ProceduralCity.Camera.Controller.Movements;
using ProceduralCity.Config;
using ProceduralCity.Utils;

using System;
using System.Linq;

namespace ProceduralCity.Camera.Controller
{

    internal class CameraController : IMovementHandler
    {
        private const int MAX_ELAPSED_TIME_BEFORE_STATE_CHANGE = 10;
        private const float START_FADEOUTBEFORETELEPORT = 1.5f;
        private const float END_FADEINAFTERTELEPORT = 1.5f;
        private const float FADEOUT_LENGTH = MAX_ELAPSED_TIME_BEFORE_STATE_CHANGE - START_FADEOUTBEFORETELEPORT;
        private const float FADEIN_LENGTH = END_FADEINAFTERTELEPORT;

        private readonly ICamera _camera;
        private readonly IAppConfig _configuration;
        private readonly Vector3 _cityCenterPosition;
        private readonly RandomService _randomService;

        private float _elapsedTimeSinceLastStateChange = 0;
        private readonly double _maxDistance;
        private IMovement _chosenMovement = new StandMovement();
        private bool _enabled = true;
        private readonly IWorld _world;

        public delegate void SetFadeoutDelegate(float fadeoutFactor);
        public SetFadeoutDelegate SetFadeout { get; set; }


        public CameraController(ICamera camera, IAppConfig configuration, RandomService randomService, IWorld world)
        {
            _camera = camera;
            _configuration = configuration;
            _randomService = randomService;
            _world = world;

            _cityCenterPosition = new Vector3(_configuration.WorldSize / 2, _configuration.MaxBuildingHeight, _configuration.WorldSize / 2);
            _maxDistance = _configuration.WorldSize * MathHelper.Sqrt(2) * 0.3f;
        }

        public void Update(float deltaTime)
        {
            if (_enabled)
            {
                _elapsedTimeSinceLastStateChange += deltaTime;

                if (_chosenMovement is not PathMovement)
                {
                    FadeOut();
                    FadeIn();
                }

                if (_elapsedTimeSinceLastStateChange > MAX_ELAPSED_TIME_BEFORE_STATE_CHANGE
                    && _chosenMovement is not PathMovement) // TODO: remove this hack and let every movementtype to trigger a change event by themselves
                {
                    TeleportToNewPosition();
                }

                _chosenMovement.Handle(this, deltaTime);
            }
        }

        public void HandleStraightMovement(StraightMovement movement, float deltaTime)
        {
            if (movement.Direction == MovementDirection.A)
            {
                _camera.MoveForward(deltaTime);
            }
            else if (movement.Direction == MovementDirection.B)
            {
                _camera.MoveBackward(deltaTime);
            }
        }

        public void HandleRotateMovement(RotateMovement movement, float deltaTime)
        {
            if (movement.Direction == MovementDirection.A)
            {
                _camera.StrafeLeft(deltaTime);
            }
            else if (movement.Direction == MovementDirection.B)
            {
                _camera.StrafeRight(deltaTime);
            }
        }

        public void HandlePlaneStrafeMovement(PlaneStrafeMovement movement, float deltaTime)
        {
            _camera.SetVerticalInstant(MathHelper.DegreesToRadians(movement.VerticalAngle));
            if (movement.Direction == MovementDirection.A)
            {
                _camera.StrafeLeft(deltaTime);
            }
            else if (movement.Direction == MovementDirection.B)
            {
                _camera.StrafeRight(deltaTime);
            }
        }

        public void HandleStandMovement(StandMovement movement, float deltaTime)
        {
            // Do nothing
        }

        public void HandlePlaneMovement(PlaneMovement movement, float deltaTime)
        {
            _camera.SetVerticalInstant(MathHelper.DegreesToRadians(movement.VerticalAngle));
            if (movement.Direction != MovementDirection.A)
            {
                _camera.MoveBackwardOnAPlane(deltaTime);
            }
            else if (movement.Direction != MovementDirection.B)
            {
                _camera.MoveForwardOnAPlane(deltaTime);
            }
        }

        public void HandlePathMovement(PathMovement movement, float deltaTime)
        {
            /*
             * TODO in the future: use a bezier curve or spline to move between waypoints
             */

            if (movement.FirstTick)
            {
                _camera.Position = movement.Path.First();
                _camera.LookAt(movement.Path.Skip(1).First());
                movement.FirstTick = false;
                movement.CurrentPathIndex = 1;
            }

            if (movement.CurrentPathIndex < movement.Path.Length)
            {
                var currentWaypoint = movement.Path[movement.CurrentPathIndex];
                _camera.LookAt(currentWaypoint);
                _camera.MoveForward(deltaTime);

                if (Vector3.Distance(_camera.Position, currentWaypoint) < 1.5f)
                {
                    movement.CurrentPathIndex++;
                }
            }
        }

        public void ToggleFlyby()
        {
            if (!_enabled)
            {
                TeleportToNewPosition();
            }

            SetFadeout?.Invoke(1.0f);
            _enabled = !_enabled;
        }

        private void FadeIn()
        {
            if (_elapsedTimeSinceLastStateChange < END_FADEINAFTERTELEPORT)
            {
                var fadeTime = _elapsedTimeSinceLastStateChange;
                var factor = Math.Clamp(fadeTime / FADEIN_LENGTH, 0, 1);
                SetFadeout?.Invoke(factor);
            }
        }

        private void FadeOut()
        {
            if (_elapsedTimeSinceLastStateChange > FADEOUT_LENGTH)
            {
                var fadeTime = _elapsedTimeSinceLastStateChange - FADEOUT_LENGTH;
                var fact = Math.Clamp(1.0f - fadeTime / (MAX_ELAPSED_TIME_BEFORE_STATE_CHANGE - FADEOUT_LENGTH), 0, 1);
                SetFadeout?.Invoke(fact);
            }
        }

        //TODO: make this private
        public void TeleportToNewPosition()
        {
            var height = _randomService.Next(_configuration.MaxBuildingHeight + 10, _configuration.MaxBuildingHeight + 200);
            var minDistanceToCenter = 2000f;
            var randomAngle = (float)(_randomService.NextDouble() * 2 * Math.PI);
            var randomDistance = minDistanceToCenter + (float)(_randomService.NextDouble() * (_configuration.WorldSize / 2.0f - minDistanceToCenter));
            var randomPoint = PolarToCartesian(randomDistance, randomAngle);
            var randomCoordinate = new Vector2(_configuration.WorldSize / 2.0f, _configuration.WorldSize / 2.0f) + randomPoint;

            _camera.Position = new Vector3(randomCoordinate.X, height, randomCoordinate.Y);


            LookAtCityCenter();
            PickMovement();
            _elapsedTimeSinceLastStateChange = 0;

        }

        static Vector2 PolarToCartesian(float radius, float angle)
        {
            var x = radius * MathF.Cos(angle);
            var y = radius * MathF.Sin(angle);
            return new Vector2(x, y);
        }

        // Picks a movement mode and direction which the camera will follow until the next state change
        private void PickMovement()
        {
            var buildParams = new MovementParams
            {
                CameraPosition = _camera.Position,
                CityCenterPosition = _cityCenterPosition,
                MaxDistance = _maxDistance,
                World = _world,
            };

            _chosenMovement = MovementBuilder.BuildRandomMovement(buildParams, _randomService);
        }

        private void LookAtCityCenter()
        {
            _camera.LookAt(_cityCenterPosition);
        }
    }
}
