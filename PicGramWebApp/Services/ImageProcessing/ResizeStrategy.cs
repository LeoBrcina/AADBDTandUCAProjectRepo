using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace PicGramWebApp.Services.ImageProcessing
{
    public class ResizeStrategy : IImageProcessingStrategy
    {
        private readonly int _width;
        private readonly int _height;

        public ResizeStrategy(int width, int height)
        {
            _width = width;
            _height = height;
        }

        public void Apply(Image image)
        {
            image.Mutate(x => x.Resize(_width, _height));
        }
    }
}