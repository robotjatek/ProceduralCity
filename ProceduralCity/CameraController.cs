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

In progress:

To Do:
- Select a random movement and move the camera
    - Strafe left or right
    - Strafe left or right while looking at the city center
    - If the bounds are reached teleport to a new position
- Fade out-Fade-in effect when teleporting
- Toggle between flyby and user camera
 */
using OpenTK.Mathematics;

using ProceduralCity.Config;

using System;

namespace ProceduralCity
{
    internal class CameraController
    {
        private readonly ICamera _camera;
        private readonly IAppConfig _configuration;
        private readonly Vector3 _cityCenterPosition;

        private float _elapsedTimeSinceLastTeleport = 0;

        public CameraController(ICamera camera, IAppConfig configuration)
        {
            _camera = camera;
            _configuration = configuration;
            _cityCenterPosition = new Vector3(_configuration.WorldSize / 2, _configuration.MaxBuildingHeight, _configuration.WorldSize / 2);
        }

        public void Update(float deltaTime)
        {
            _elapsedTimeSinceLastTeleport += deltaTime;
            if(_elapsedTimeSinceLastTeleport > 10)
            {
                TeleportToNewPosition();
                _elapsedTimeSinceLastTeleport = 0;
            }
            _camera.MoveForward(deltaTime);
        }

        //TODO: make this private
        public void TeleportToNewPosition()
        {
            var rnd = new Random();
            // x, height, y;
            var height = _configuration.MaxBuildingHeight + 10.0f;
            var nx = (float) rnd.NextDouble() * _configuration.WorldSize;
            var ny = (float) rnd.NextDouble() * _configuration.WorldSize;

            var pos = new Vector3(nx, height, ny);
            _camera.SetPosition(pos);
            LookAtCityCenter();
        }

        private void LookAtCityCenter()
        {
            _camera.LookAt(_cityCenterPosition);
        }
    }
}
