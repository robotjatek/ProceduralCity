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
        private readonly Texture _workTexture; // The work texture where intermediate work gets done

        private readonly List<PostProcess> _postprocessEffects;
        private readonly Shader _lumaEffect = new("vs.vert", "PostProcess/luminance.frag");
        private readonly Shader _blendEffect = new("vs.vert", "PostProcess/blend.frag");
        private readonly Shader _horizontalBlurEffect = new("vs.vert", "PostProcess/blur.frag");
        private readonly Shader _verticalBlurEffect = new("vs.vert", "PostProcess/blur.frag");

        public PostprocessPipeline(ILogger logger, IAppConfig config, Texture inputTexture)
        {
            _logger = logger;
            _inputTexture = inputTexture; // Rendered 3D world
            _workTexture = new Texture(inputTexture.Width, inputTexture.Height);

            _postprocessEffects =
            [
                // All three needed for the bloom effect
                new PostProcess(logger, new[] { _inputTexture }, _workTexture, _lumaEffect), // Extract pixels that are above a given luminance threshold. Pixels below the threshold are black
                new PostProcess(logger, new[] { _workTexture }, _workTexture, _horizontalBlurEffect), // Blur the output of the luminance effect horziontally
                new PostProcess(logger, new[] { _workTexture }, _workTexture, _verticalBlurEffect), // Blur the image further vertically
                // The output after this stage is the pixels that are above the given luminance threshold with blur appied to them. This is a significantly darker picture than the input

                // The last stage blends the output of the previous stage with the input picture
                new PostProcess(logger, new[] { _inputTexture, _workTexture }, _inputTexture, _blendEffect),
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

            _logger.Information("Post-process pipeline has been set up with {postprocess_count} effects", _postprocessEffects.Count);
        }

        public void RunPipeline()
        {
            foreach (var effect in _postprocessEffects)
            {
                effect.DoPostProcess();
            }
        }

        public void Resize(int width, int height, float scale)
        {
            foreach (var effect in _postprocessEffects)
            {
                effect.Resize(width, height, scale);
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
                    _workTexture.Dispose();

                    foreach (var p in _postprocessEffects)
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
