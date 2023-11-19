using OpenTK.Mathematics;

using System;

namespace ProceduralCity.Generators
{
    public class ColorGenerator
    {
        private readonly Random _random = new();

        // RGB colors
        private readonly Color4[] _cloudColors = new[]
        {
            new Color4(0, 1.0f, 0, 1), //green
            new Color4(0, 0, 1.0f, 1), //blue
            new Color4(1, 0, 0, 1), //red
            new Color4(0.59f, 0.29f, 0, 1), //brown
            new Color4(1.0f, 0.50f, 0, 1), //orange
            new Color4(0.6f, 0, 0.6f, 1), //purple
            new Color4(1.0f, 1.0f, 0.1f, 1), //yellow
            new Color4(1.0f, 0.2f, 0.33f, 1), //"radical red"
        };

        public Color4 Primary { get; private set; } = Color4.Black;
        public Color4 Secondary { get; private set; } = Color4.Black;

        private static Color4 MixedColor(Color4 c1, Color4 c2)
        {
            var color1AsVector = new Vector3(c1.R, c1.G, c1.B);
            var color2AsVector = new Vector3(c2.R, c2.G, c2.B);

            /*
             * This is a totally arbitrary value for the mixed colors, which happens to be correct 95% of the times.
             * In the remaining times the vector values are so low, that the mixed color is black, resulting in
             * essentially a cloudless night. I am still unsure if this should be considered a bug or a feature.
             */
            var mixed = Vector3.Clamp((color1AsVector + color2AsVector) * 0.3f, new Vector3(0f), new Vector3(1f));
            return new Color4(mixed.X, mixed.Y, mixed.Z, 1f);
        }

        public delegate void ColorChangedEvent();

        public event ColorChangedEvent OnColorChanged;

        public Color4 Mixed
        {
            get => MixedColor(Primary, Secondary);
        }
        
        public ColorGenerator()
        {
            GenerateColors();
        }

        public void GenerateColors()
        {
            var primaryIndex = _random.Next(0, _cloudColors.Length);
            var secondaryIndex = _random.Next(0, _cloudColors.Length);

            Primary = _cloudColors[primaryIndex];
            Secondary =  _cloudColors[secondaryIndex];
            OnColorChanged?.Invoke();
        }
    }
}
