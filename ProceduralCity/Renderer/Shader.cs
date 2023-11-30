using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using OpenTK.Graphics.OpenGL;

using ProceduralCity.Extensions;
using ProceduralCity.Renderer.Uniform;
using Serilog;

namespace ProceduralCity.Renderer
{
    public class Shader : IDisposable
    {
        private readonly Dictionary<string, int> _uniformLocations = [];
        private readonly UniformHandler _uniformHandler = new();

        public int ProgramId
        {
            get;
            private set;
        }

        public Shader(string vertexShader, string fragmentShader) : this(new[] { vertexShader }, new[] { fragmentShader }) { }

        public Shader(IEnumerable<string> vertexShaders, string fragmentShader) : this(vertexShaders, new[] { fragmentShader }) { }

        public Shader(string vertexShader, IEnumerable<string> fragmentShaders) : this(new[] { vertexShader }, fragmentShaders) { }

        public Shader(IEnumerable<string> vertexShaders, IEnumerable<string> fragmentShaders)
        {
            var vertexFiles = vertexShaders.Select(LoadTextFile);
            var fragmentFiles = fragmentShaders.Select(LoadTextFile);

            var vertexHandles = vertexFiles.Select(vertexHandle => CompileShader(vertexHandle, ShaderType.VertexShader)).ToArray();
            var fragmentHandles = fragmentFiles.Select(fragmentHandle => CompileShader(fragmentHandle, ShaderType.FragmentShader));
            int[] shaderHandles = [..vertexHandles, ..fragmentHandles];
                
            ProgramId = LinkShaders(shaderHandles);

            shaderHandles.ForEach(GL.DeleteProgram);
        }

        public void SetUniformValue<T>(string uniformName, T value) where T : IUniformValue
        {
            this.Use();
            if (_uniformLocations.TryGetValue(uniformName, out int location) == false)
            {
                location = GL.GetUniformLocation(ProgramId, uniformName);
                _uniformLocations.Add(uniformName, location);
            }

            if (location != -1)
            {
                value.Visit(location, _uniformHandler);
            }
            else
            {
                Log.Logger.Warning("Could not find uniform {uniformName}", uniformName);
            }

            Unbind();
        }

        public void Use()
        {
            GL.UseProgram(ProgramId);
        }

        public static void Unbind()
        {
            GL.UseProgram(0);
        }

        private static int LinkShaders(IEnumerable<int> shaderHandles)
        {
            var programId = GL.CreateProgram();
            shaderHandles.ForEach(shaderHandle =>
            {
                GL.AttachShader(programId, shaderHandle);
            });
            
            GL.LinkProgram(programId);
            return programId;
        }

        private static int CompileShader(string shaderFile, ShaderType type)
        {
            var shaderHandle = GL.CreateShader(type);
            GL.ShaderSource(shaderHandle, shaderFile);
            GL.CompileShader(shaderHandle);
            CheckForCompilationErrors(shaderHandle);
            return shaderHandle;
        }

        private static void CheckForCompilationErrors(int shaderId)
        {
            GL.GetShader(shaderId, ShaderParameter.CompileStatus, out int result);
            if (result != 1)
            {
                var log = GL.GetShaderInfoLog(shaderId);
                Log.Error(log);
            }
            else
            {
                Log.Information("Compiled shader {shaderId} successfully", shaderId);
            }
        }

        private static string LoadTextFile(string filename)
        {
            return File.ReadAllText($"Shaders/{filename}");
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
