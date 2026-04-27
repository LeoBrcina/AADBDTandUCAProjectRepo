using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace PicGramWebApp.Services.ImageProcessing
{
    public class BlurStrategy : IImageProcessingStrategy
    {
        public void Apply(Image image)
        {
            image.Mutate(x => x.GaussianBlur());
        }
    }
}