
using System;

using OpenTK.Mathematics;

namespace ProceduralCity.Renderer
{
    interface IBatch : IDisposable
    {
        void AddMesh(Mesh m);

        void Draw(Matrix4 projection, Matrix4 view);
    }
}
