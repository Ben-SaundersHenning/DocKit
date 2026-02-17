using DocKit.Images;

namespace DocKit.Test;

using DocKit;
using DocKit.TemplateEngine;

public class DocumentTest
{

    [Fact]
    public void Create_ValidDocument()
    {

        Document doc = Document.Create("/tmp/test_doc.docx");

        Assert.NotNull(doc);

    }
    
    [Fact]
    public void Save_Document()
    {
        
        File.Delete("/tmp/test_doc.docx"); 

        Document doc = Document.Create("/tmp/test_doc.docx");
        
        Assert.NotNull(doc);
        
        doc.Save();
        
        Assert.True(File.Exists(Path.GetFullPath("/tmp/test_doc.docx")));

    }
    
    [Fact]
    public void Save_NotEmptyDocument()
    {
        
        File.Delete("/tmp/test_doc.docx"); 

        Document doc = Document.Create("/tmp/test_doc.docx");
        
        Assert.NotNull(doc);
        
        doc.AppendText("Hello World!");
        
        doc.Save();
        
        Assert.True(File.Exists(Path.GetFullPath("/tmp/test_doc.docx")));

    }
    
    [Fact]
    public void Save_WithImage()
    {
        
        File.Delete("/tmp/test_doc.docx"); 

        Document doc = Document.Create("/tmp/test_doc.docx");
        
        Assert.NotNull(doc);
        
        doc.AppendImage(Path.GetFullPath("../../../../Documents_Testing/image.png"));
        
        doc.Save();
        
        Assert.True(File.Exists(Path.GetFullPath("/tmp/test_doc.docx")));

    }
    
    [Fact]
    public void Save_WithScaledImage()
    {
        
        File.Delete("/tmp/test_doc.docx"); 

        Document doc = Document.Create("/tmp/test_doc.docx");
        
        Assert.NotNull(doc);

        Image image = new Image(Path.GetFullPath("../../../../Documents_Testing/image.png"));
        image.FitToBounds(2, 2);
        
        doc.AppendImage(image);
        
        doc.Save();
        
        Assert.True(File.Exists(Path.GetFullPath("/tmp/test_doc.docx")));

    }

    [Fact]
    public void IsolateTag()
    {
        
        File.Delete("/tmp/test_doc.docx"); 

        Document doc = Document.Open("/home/ben/Projects/dockit/Documents_Testing/template_engine/tags.docx");
        
        Assert.NotNull(doc);

        TemplateEngine eng = new TemplateEngine();
        eng.RunEngine(doc, null);
        
        string savePath = "/tmp/test_doc.docx";
        
        doc.SaveAs(savePath);
        
        Assert.True(File.Exists(Path.GetFullPath("/tmp/test_doc.docx")));
        
    }

    [Fact]
    public void SimpleReplacementProcessing()
    {
        
        File.Delete("/tmp/test_doc.docx"); 

        Document doc = Document.Open("/home/ben/Projects/dockit/Documents_Testing/template_engine/tags.docx");
        
        Assert.NotNull(doc);

        TemplateEngine eng = new TemplateEngine();
        eng.RunEngine(doc, ReplaceFunc);
        
        string savePath = "/tmp/test_doc.docx";
        
        doc.SaveAs(savePath);
        
        Assert.True(File.Exists(Path.GetFullPath("/tmp/test_doc.docx")));
        
    }
    
    [Fact]
    public void Create_Null()
    {

        var exception = Assert.Throws<ArgumentNullException>(() => Document.Create(null));
        Assert.Equal("Value cannot be null. (Parameter 'path')", exception.Message);

    }
    
    [Fact]
    public void Create_BadPath()
    {

        var exception = Assert.Throws<DirectoryNotFoundException>(() => Document.Create("~/Blah/Blah/Blah"));
        Assert.Equal("Directory not found: '~/Blah/Blah'", exception.Message);

    }
    
    [Fact]
    public void Create_InvalidPath()
    {

        var exception = Assert.Throws<DirectoryNotFoundException>(() => Document.Create("blah blah blah"));
        Assert.Equal("Directory not found: ''", exception.Message);

    }
    
    [Fact]
    public void Create_AlreadyExists()
    {

        var exception = Assert.Throws<IOException>(() => Document.Create("/home/ben/Projects/dockit/DocKit.Test/documents/test.docx"));
        Assert.Equal("File already exists: '/home/ben/Projects/dockit/DocKit.Test/documents/test.docx'", exception.Message);

    }


    [Fact]
    public void Open_ValidDocument()
    {

        Document doc = Document.Open("/home/ben/Projects/dockit/DocKit.Test/documents/test.docx");
        Assert.NotNull(doc);
        
    }
    
    [Fact]
    public void Open_ValidTemplate()
    {

        Document doc = Document.Open("/home/ben/Projects/dockit/DocKit.Test/documents/test.dotx");
        Assert.NotNull(doc);
        
    }
    
    [Fact]
    public void Open_InvalidFileType()
    {

        var exception = Assert.Throws<NotSupportedException>(() => Document.Open("/home/ben/Projects/dockit/DocKit.Test/documents/test.ppt"));
        Assert.Equal("File type '.ppt' is not supported", exception.Message);
        
    }
    
    [Fact]
    public void Open_InvalidDocument()
    {

        var exception = Assert.Throws<FileNotFoundException>(() => Document.Open("/home/ben/Projects/dockit/DocKit.Test/documents/DONOTCREATE.docx"));
        Assert.Equal("File not found: '/home/ben/Projects/dockit/DocKit.Test/documents/DONOTCREATE.docx'", exception.Message);
        
    }

    private string ReplaceFunc(string operand)
    {
        return "THIS IS A REPLACEMENT";
    }
        
}