using System;
using System.Collections.Generic;
using ProceduralCity.Config;
using ProceduralCity.Renderer.Uniform;
using Serilog;

namespace ProceduralCity.Renderer.PostProcess
{
    class PostprocessPipeline : IDisposable
    {
        private readonly ILogger _logger;
        private readonly Texture _inputTexture;
        private readonly Texture _outputTexture;

        private readonly List<PostProcess> _postprocess;
        private readonly Shader _lumaEffect = new Shader("vs.vert", "PostProcess/luminance.frag");
        private readonly Shader _blendEffect = new Shader("vs.vert", "PostProcess/blend.frag");

        public PostprocessPipeline(ILogger logger, IAppConfig config, Texture inputTexture, Texture outputTexture)
        {
            _logger = logger;
            _inputTexture = inputTexture;
            _outputTexture = outputTexture;

            _postprocess = new List<PostProcess>()
            {
                new PostProcess(logger, new[] { _inputTexture }, _outputTexture, _lumaEffect),
                new PostProcess(logger, new[] { _inputTexture, _outputTexture }, _outputTexture, _blendEffect)
            };

            _lumaEffect.SetUniformValue("u_LuminanceTreshold", new FloatUniform
            {
                Value = config.BloomTreshold
            });
            _lumaEffect.SetUniformValue("tex", new IntUniform
            {
                Value = 0
            });

            _blendEffect.SetUniformValue("u_texture1", new IntUniform
            {
                Value = 0
            });
            _blendEffect.SetUniformValue("u_texture2", new IntUniform
            {
                Value = 1
            });
        }

        public void RunPipeline()
        {
            foreach (var p in _postprocess)
            {
                p.DoPostProcess();
            }
        }

        public void Resize(int width, int height, float scale)
        {
            foreach (var p in _postprocess)
            {
                p.Resize(width, height, scale);
            }
        }

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _lumaEffect.Dispose();
                    _blendEffect.Dispose();
                    foreach (var p in _postprocess)
                    {
                        p.Dispose();
                    }
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
