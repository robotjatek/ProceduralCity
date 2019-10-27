using System;
using System.Collections.Generic;
using System.IO;
using OpenTK.Graphics.OpenGL;
using ProceduralCity.Renderer.Uniform;
using Serilog;

namespace ProceduralCity.Renderer
{
    class Shader : IDisposable
    {
        private readonly Dictionary<string, IUniformValue> _uniformValues = new Dictionary<string, IUniformValue>();

        public int _programId
        {
            get;
            private set;
        }

        public IEnumerable<KeyValuePair<string, IUniformValue>> Uniforms
        {
            get
            {
                return _uniformValues;
            }
        }

        public Shader(string vertexShader, string fragmentShader)
        {
            string vertexFile = LoadTextFile(vertexShader);
            string fragmentFile = LoadTextFile(fragmentShader);

            var vertexHandle = CompileShader(vertexFile, ShaderType.VertexShader);
            var fragmentHandle = CompileShader(fragmentFile, ShaderType.FragmentShader);

            _programId = LinkShaders(vertexHandle, fragmentHandle);

            GL.DeleteShader(vertexHandle);
            GL.DeleteShader(fragmentHandle);
        }

        public void SetUniformValue<T>(string uniformName, T value) where T : IUniformValue
        {
            _uniformValues[uniformName] = value;
        }

        public void Use()
        {
            GL.UseProgram(_programId);
        }

        private int LinkShaders(int vertexHandle, int fragmentHandle)
        {
            var programId = GL.CreateProgram();
            GL.AttachShader(programId, vertexHandle);
            GL.AttachShader(programId, fragmentHandle);
            GL.LinkProgram(programId);
            return programId;
        }

        private int CompileShader(string shaderFile, ShaderType type)
        {
            var shaderHandle = GL.CreateShader(type);
            GL.ShaderSource(shaderHandle, shaderFile);
            GL.CompileShader(shaderHandle);
            CheckForCompilationErrors(shaderHandle);
            return shaderHandle;
        }

        private void CheckForCompilationErrors(int shaderId)
        {
            GL.GetShader(shaderId, ShaderParameter.CompileStatus, out int result);
            if (result != 1)
            {
                var log = GL.GetShaderInfoLog(shaderId);
                Log.Error(log);
            }
            else
            {
                Log.Information($"Compiled shader {shaderId} successfully");
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

                GL.DeleteProgram(_programId);
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
