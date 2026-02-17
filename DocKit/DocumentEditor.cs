using DocKit.Images;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace DocKit;

public partial class Document
{

    private uint _imageCounter = 0;
    
    public void AppendText(string text)
    {
        var run = new Run(new Text(text))
        {
            RunProperties = new RunProperties()
        };
        Body.AppendChild(new Paragraph(run));
    }

    private Drawing CreateImagePart(Image image)
    {
        
        // adds an image part to the document
        ImagePart imagePart = MainPart.AddImagePart(DrawingUtils.GetImagePartType(image));
        
        // feed the data into the image part
        using (FileStream stream = new FileStream(image.FilePath, FileMode.Open))
        {
            imagePart.FeedData(stream);
        }
        
        // creates a drawing element
        Drawing drawing = image.CreateDrawingElement(_imageCounter++, MainPart.GetIdOfPart(imagePart));

        return drawing;

    }

    public void AppendImage(Image image)
    {
        
        Drawing drawing = CreateImagePart(image);
        
        // then insert the drawing into the document
        var run = new Run(drawing)
        {
            RunProperties = new RunProperties()
        };
        Body.AppendChild(new Paragraph(run));

    }
    
    public void AppendImage(string filePath)
    {
        Image image = new Image(filePath);
        AppendImage(image);
    }

    internal void AddImage(Image image, Run imageParent)
    {
        
        Drawing drawing = CreateImagePart(image);
        
        imageParent.AppendChild(drawing);
        
    }


    
}