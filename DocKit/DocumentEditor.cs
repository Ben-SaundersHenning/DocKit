using DocumentFormat.OpenXml.Drawing;

namespace DocKit;

public partial class Document
{
    
    public void InsertText(string text)
    {
        Body.AppendChild(new Paragraph(new Run(new Text(text))));
    }

    public void EditCheckbox(string identifier, bool value)
    {
        
        //CheckBox checkBox = CheckBox.Find(Body, identifier);
        
    }
    
}