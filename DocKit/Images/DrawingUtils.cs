using DocumentFormat.OpenXml.Packaging;

namespace DocKit.Images;

public static class DrawingUtils
{
    
    private static class FileExtensions
    {
        public const string  Png = ".png";
        public const string Jpeg = ".jpeg";
        public const string Gif = ".gif";
        public const string Bmp = ".bmp";
        public const string Tiff = ".tiff";
    }

    internal static PartTypeInfo GetImagePartType(Image image)
    {
        String fileExtension = Path.GetExtension(image.FilePath).ToLowerInvariant();
        return fileExtension switch
        {
            FileExtensions.Png => ImagePartType.Png,
            FileExtensions.Jpeg => ImagePartType.Jpeg,
            FileExtensions.Gif => ImagePartType.Gif,
            FileExtensions.Bmp => ImagePartType.Bmp,
            FileExtensions.Tiff => ImagePartType.Tiff,
            _ => throw new NotSupportedException($"File type '{fileExtension}' is not supported")
        };
    }
    
}