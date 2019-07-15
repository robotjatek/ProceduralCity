using System;
using OpenTK;

namespace ProceduralCity.Renderer
{
    public interface ISkybox : IDisposable
    {
        void Render(Matrix4 proj, Matrix4 view);
    }
}
