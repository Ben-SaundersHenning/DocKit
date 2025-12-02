using DocumentFormat.OpenXml.Packaging;
using SkiaSharp;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;
using DocumentFormat.OpenXml.Wordprocessing;

namespace DocKit.Images;

public class Image
{

    internal string FilePath { get; set; }
    
    private int Width { get; set; }
    
    private int WidthInEMUs { get; set; }
    
    private int Height { get; set; }
    
    private int HeightInEMUs { get; set; }
    
    private int DpiX { get; set; }
    
    private int DpiY { get; set; }
    
    private const int EMUsPerInch = 914400;
    
    //public Image(string filePath)
    //{

    //    if (File.Exists(filePath) == false)
    //        throw new FileNotFoundException($"File not found: '{filePath}'");
    //    
    //    Path = filePath;
    //    
    //    using var image = SKImage.FromEncodedData(filePath);
    //    Width = (int)Math.Round((decimal)image.Width * 9525);
    //    Height = (int)Math.Round((decimal)image.Height * 9525);
    //}

    public Image(string filePath)
    {
        if (File.Exists(filePath) == false)
            throw new FileNotFoundException($"File not found: '{filePath}'");
        
        FilePath = filePath;

        String fileExtension = Path.GetExtension(filePath).ToLowerInvariant();
        
        using SKBitmap bitmap = SKBitmap.Decode(filePath);
        Width = bitmap.Width;
        Height = bitmap.Height;
        
        // Note: one day, could read the DPI information from the image itself
        // Defaulting to 96 DPI for now.
        //DpiX = 96;
        //DpiY = 96;
        
        WidthInEMUs = (int)Math.Round((decimal)Width * 9525);
        HeightInEMUs = (int)Math.Round((decimal)Height * 9525);

    }
    
    public void FitToBounds(double maxWidthInches, double maxHeightInches)
    {
        
        // Convert current size from EMUs to inches
        double currentWidthInches = (double)WidthInEMUs / EMUsPerInch;
        double currentHeightInches = (double)HeightInEMUs / EMUsPerInch;
    
        // Calculate scale factors needed to fit each dimension
        double widthScale = maxWidthInches / currentWidthInches;
        double heightScale = maxHeightInches / currentHeightInches;
    
        // Use the smaller scale to ensure it fits both constraints
        double scale = Math.Min(widthScale, heightScale);
    
        // Only scale down, not up
        //if (scale > 1.0)
        //    scale = 1.0;
    
        // Scale the size
        WidthInEMUs = (int)(currentWidthInches * scale * EMUsPerInch);
        HeightInEMUs = (int)(currentHeightInches * scale * EMUsPerInch);

    } 
    
    internal Drawing CreateDrawingElement(uint imageId, string relationshipId)
    {
        
        string imageName = $"Image{imageId}";
        
        // A drawing is an inline or anchor container
        // inline: drawing flows with text
        // anchor: drawing flows around text
        var drawing = new Drawing(
            
            // Inline option
            new DW.Inline(
                
                // Size of the image as it will appear, in EMU
                // (english metric units)
                new DW.Extent
                { 
                    Cx = WidthInEMUs, 
                    Cy = HeightInEMUs 
                },
                
                // Shadow/effect margins
                new DW.EffectExtent
                { 
                    LeftEdge = 0L, 
                    TopEdge = 0L,
                    RightEdge = 0L, 
                    BottomEdge = 0L 
                },
                
                // Metadata about the drawing
                new DW.DocProperties
                { 
                    Id = imageId,
                    Name = imageName,
                    // Description gives a comment on the image
                    //Description = imageId.ToString()
                },
                
                // Drawing properties (lock aspect ratio to prevent distortion)
                new DW.NonVisualGraphicFrameDrawingProperties(
                    new A.GraphicFrameLocks { NoChangeAspect = true }
                ),
                
                // The actual image graphic
                new A.Graphic(
                    new A.GraphicData(
                        new PIC.Picture(
                            
                            // Non-visual properties (IDs, names)
                            new PIC.NonVisualPictureProperties(
                                new PIC.NonVisualDrawingProperties
                                { 
                                    Id = 0U,
                                    Name = imageName // could reference acutal image data here
                                },
                                new PIC.NonVisualPictureDrawingProperties()
                            ),
                            
                            // Image fill (how the image fills the shape)
                            new PIC.BlipFill(
                                
                                // Reference to the actual image data
                                new A.Blip
                                { 
                                    Embed = relationshipId, // references the actual image
                                    CompressionState = A.BlipCompressionValues.Print
                                },
                                // Stretch to fill the shape
                                new A.Stretch(new A.FillRectangle())
                            ),
                            
                            // Shape properties (position and size)
                            new PIC.ShapeProperties(
                                // 2D transformation (position and size)
                                new A.Transform2D(
                                    new A.Offset { X = 0L, Y = 0L },
                                    new A.Extents
                                    { 
                                        Cx = WidthInEMUs, 
                                        Cy = HeightInEMUs 
                                    }
                                ),
                                // Shape is a rectangle
                                new A.PresetGeometry(new A.AdjustValueList()) 
                                { 
                                    Preset = A.ShapeTypeValues.Rectangle 
                                }
                            )
                        )
                    ) 
                    { 
                        Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" 
                    }
                )
            )
            {
                // Distance from surrounding text (0 for inline images)
                // These are margins
                DistanceFromTop = 0U,
                DistanceFromBottom = 0U,
                DistanceFromLeft = 0U,
                DistanceFromRight = 0U
            }
        );
        
        return drawing;
        
    }
    
}
