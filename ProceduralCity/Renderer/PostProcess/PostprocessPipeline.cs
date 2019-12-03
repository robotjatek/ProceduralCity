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
        private readonly Shader _lumaEffect;

        public PostprocessPipeline(ILogger logger, IAppConfig config, Texture inputTexture, Texture outputTexture)
        {
            _logger = logger;
            _inputTexture = inputTexture;
            _outputTexture = outputTexture;

            _postprocess = new List<PostProcess>()
            {
                new PostProcess(logger, _outputTexture),
            };

            _lumaEffect = new Shader("vs.vert", "PostProcess/luminance.frag");
            _lumaEffect.SetUniformValue("u_LuminanceTreshold", new FloatUniform
            {
                Value = config.BloomTreshold
            });
            _lumaEffect.SetUniformValue("tex", new IntUniform
            {
                Value = 0
            });
        }

        public void RunPipeline()
        {
            Texture input = _inputTexture;
            foreach (var p in _postprocess)
            {
                p.DoPostProcess(input, _lumaEffect);
                input = _outputTexture;
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
