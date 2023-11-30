using System;
using System.Collections.Generic;
using OpenTK.Mathematics;

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
        private readonly Shader _lumaEffect = new("vs.vert", "PostProcess/luminance.frag");
        private readonly Shader _blendEffect = new("vs.vert", "PostProcess/blend.frag");
        private readonly Shader _horizontalBlurEffect = new("vs.vert", "PostProcess/blur.frag");
        private readonly Shader _verticalBlurEffect = new("vs.vert", "PostProcess/blur.frag");

        public PostprocessPipeline(ILogger logger, IAppConfig config, Texture inputTexture, Texture outputTexture)
        {
            _logger = logger;
            _inputTexture = inputTexture; // Rendered 3D world
            _outputTexture = outputTexture;

            _postprocess =
            [
                // All three needed for the bloom effect
                new PostProcess(logger, new[] { _inputTexture }, _outputTexture, _lumaEffect),
                new PostProcess(logger, new[] { _outputTexture }, _outputTexture, _horizontalBlurEffect),
                new PostProcess(logger, new[] { _outputTexture }, _outputTexture, _verticalBlurEffect),
                new PostProcess(logger, new[] { _inputTexture, _outputTexture }, _outputTexture, _blendEffect),
            ];

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

            _horizontalBlurEffect.SetUniformValue("u_texture", new IntUniform
            {
                Value = 0
            });
            _horizontalBlurEffect.SetUniformValue("u_Offset", new Vector2Uniform
            {
                Value = new Vector2(1f / _inputTexture.Width, 0)
            });

            _verticalBlurEffect.SetUniformValue("u_texture", new IntUniform
            {
                Value = 0
            });
            _verticalBlurEffect.SetUniformValue("u_Offset", new Vector2Uniform
            {
                Value = new Vector2(0, 1f / _inputTexture.Height)
            });

            _logger.Information("Post-process pipeline has been set up with {postprocess_count} effects", _postprocess.Count);
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
                    _horizontalBlurEffect.Dispose();
                    _verticalBlurEffect.Dispose();
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
