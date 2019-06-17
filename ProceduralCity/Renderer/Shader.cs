using System;
using System.IO;
using OpenTK.Graphics.OpenGL;

namespace ProceduralCity.Renderer
{
    class Shader : IDisposable
    {
        public int ProgramId
        {
            get;
            private set;
        }

        public Shader(string vertexShader, string fragmentShader)
        {
            string vertexFile = LoadTextFile(vertexShader);
            string fragmentFile = LoadTextFile(fragmentShader);

            var vertexHandle = CompileShader(vertexFile, ShaderType.VertexShader);
            var fragmentHandle = CompileShader(fragmentFile, ShaderType.FragmentShader);

            ProgramId = LinkShaders(vertexHandle, fragmentHandle);

            GL.DeleteShader(vertexHandle);
            GL.DeleteShader(fragmentHandle);
        }

        public void Use()
        {
            GL.UseProgram(ProgramId);
        }

        private int LinkShaders(int vertexHandle, int fragmentHandle)
        {
            var programId = GL.CreateProgram();
            GL.AttachShader(ProgramId, vertexHandle);
            GL.AttachShader(ProgramId, fragmentHandle);
            GL.LinkProgram(ProgramId);
            return programId;
        }

        private int CompileShader(string vertexFile, ShaderType type)
        {
            var vertexHandle = GL.CreateShader(type);
            GL.ShaderSource(vertexHandle, vertexFile);
            GL.CompileShader(vertexHandle);
            CheckForCompilationErrors(vertexHandle);
            return vertexHandle;
        }

        private void CheckForCompilationErrors(int shaderId)
        {
            GL.GetShader(shaderId, ShaderParameter.CompileStatus, out int result);
            if (result != 1)
            {
                var log = GL.GetShaderInfoLog(shaderId);
                Console.WriteLine(log);
            }
            else
            {
                Console.WriteLine($"Compiled shader {shaderId} successfully");
            }
        }

        private string LoadTextFile(string vertexShader)
        {
            return File.ReadAllText($"Shaders/{vertexShader}");
        }

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                //if (disposing)
                //{
                //}

                GL.DeleteProgram(ProgramId);
                disposedValue = true;
            }
        }

        ~Shader()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
