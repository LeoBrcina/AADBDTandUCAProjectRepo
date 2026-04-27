using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace PicGramWebApp.Services.ImageProcessing
{
    public class BrightnessStrategy : IImageProcessingStrategy
    {
        private readonly float _value;

        public BrightnessStrategy(float value)
        {
            _value = value;
        }

        public void Apply(Image image)
        {
            image.Mutate(x => x.Brightness(_value));
        }
    }
}