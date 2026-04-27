using SixLabors.ImageSharp;

namespace PicGramWebApp.Services.ImageProcessing
{
    // Strategy pattern: defines a family of image processing algorithms (resize, blur, grayscale, etc.)
    // and allows them to be selected and combined dynamically at runtime without modifying client code.
    public interface IImageProcessingStrategy
    {
        void Apply(Image image);
    }
}