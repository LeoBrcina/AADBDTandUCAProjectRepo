using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace PicGramWebApp.Services.ImageProcessing
{
    public class ContrastStrategy : IImageProcessingStrategy
    {
        private readonly float _value;

        public ContrastStrategy(float value)
        {
            _value = value;
        }

        public void Apply(Image image)
        {
            image.Mutate(x => x.Contrast(_value));
        }
    }
}