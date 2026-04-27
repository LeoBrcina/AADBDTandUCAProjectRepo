using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace PicGramWebApp.Services.ImageProcessing
{
    public class SharpenStrategy : IImageProcessingStrategy
    {
        private readonly float _amount;

        public SharpenStrategy(float amount)
        {
            _amount = amount;
        }

        public void Apply(Image image)
        {
            image.Mutate(x => x.GaussianSharpen(_amount));
        }
    }
}