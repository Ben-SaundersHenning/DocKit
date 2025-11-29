using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Office2010.Word;
using DocumentFormat.OpenXml.Wordprocessing;
using Checked = DocumentFormat.OpenXml.Wordprocessing.Checked;

// Structure of legacy:
//FieldChar root; 
//
//FormFieldData checkBox parent;
//
//FormFieldName which is the identifier for the checkbox
//CheckBox checkbox itself;
//
//Checked which is the value of the checkbox;

namespace DocKit.Checkboxes;

public enum CheckboxType
{
    Legacy,
    Modern
}

public class Checkbox
{

    public string Identifier { get; }

    public bool Value
    {
        get;
        set
        {
            field = value;
            SetValue(value);
        }
    }

    public CheckboxType Type { get; }
    
    private CheckBox? _legacyCheckbox;
    
    private SdtElement? _modernCheckBox;

    private Checkbox(string identifier, CheckboxType type, object checkbox)
    {
        
        Identifier = identifier;
        Type = type;
        
        switch (type)
        {
            case CheckboxType.Modern:
                _modernCheckBox = (SdtElement)checkbox;
                break;
            case CheckboxType.Legacy:
                _legacyCheckbox = (CheckBox)checkbox;
                break;
            default:
                throw new ArgumentException($"Checkboxes of type {type} are not supported");
        } 

    }

    public static Checkbox Find(Body body, string identifier)
    {
        
        // To find a modern checkbox:
        //   1. look through all SdtContentCheckBox (w14:checkbox)
        //   2. see if their parent, an SdtProperties (w14:sdtPr) has a child of type Tag (w:tag)
        //   3. If the tag has a value that matches tag, it's the correct checkbox
        SdtElement? modern = body.Descendants<SdtContentCheckBox>()
            .Where(checkbox => checkbox.Parent?.GetFirstChild<Tag>()?.Val == identifier)
            .Select(checkbox => checkbox.Ancestors<SdtElement>().FirstOrDefault())
            .FirstOrDefault();

        if (modern != null)
            return new Checkbox(identifier, CheckboxType.Modern, modern);

        // To find a legacy checkbox:
        //   1. Look for a FormFieldName whose Val.Value is identifier (a string)
        //   2. Return its sibling who is of type CheckBox
        CheckBox? legacy = body.Descendants<FormFieldName>()
            .Where(tag => tag.Val?.Value == identifier)
            .Select(tag => tag.Parent?.GetFirstChild<CheckBox>())
            .FirstOrDefault();
        
        if (legacy != null)
            return new Checkbox(identifier, CheckboxType.Legacy, legacy);
        
        throw new ArgumentException($"Checkboxes with tag '{identifier}' was not found");

    }

    public static Checkbox Create(string identifier, bool value)
    {
        // TODO: create the checkbox (a SdtElement)
        //return new Checkbox(identifier, CheckboxType.Modern, ); 
        throw new NotImplementedException();
    }

    private void SetValue(bool value)
    {
        
        if (Type ==  CheckboxType.Modern && _modernCheckBox != null)
            SetValueModern(value);
        else if (Type ==  CheckboxType.Legacy && _legacyCheckbox != null)
            SetValueLegacy(value);
        
    }

    private void SetValueModern(bool value)
    {
        SdtContentCheckBox? checkbox = _modernCheckBox?.Descendants<SdtContentCheckBox>().FirstOrDefault();
        
        if (checkbox == null)
            // TODO: create the checkbox value
            throw new NotImplementedException();
        
        checkbox.Checked?.Val = value ? OnOffValues.True : OnOffValues.False;
        
        Text? checkboxChar = _modernCheckBox?.Descendants<Text>().FirstOrDefault();
        
        if (checkboxChar == null) 
            // TODO: create the content section
            throw new NotImplementedException();

        int unicode = value switch
        {
            true => int.Parse(checkbox.CheckedState?.Val?.Value ?? "2612", System.Globalization.NumberStyles.HexNumber),
            _ => int.Parse(checkbox.UncheckedState?.Val?.Value ?? "2610", System.Globalization.NumberStyles.HexNumber)
        };

        checkboxChar.Text = unicode.ToString();

    }

    private void SetValueLegacy(bool value)
    {
        
        Checked? state = _legacyCheckbox?.GetFirstChild<Checked>();
        
        if (state == null)
            // TODO: create the checkbox value
            throw new NotImplementedException();

        state.Val = new OnOffValue(value);

    }
    
    //private void EditLegacyCheckbox(string tag, bool newState)
    //{
    //    foreach (CheckBox cb in Body!.Descendants<CheckBox>())
    //    {
    //        FormFieldName cbName = cb.Parent.ChildElements.First<FormFieldName>();
    //        if (cbName.Val.Value == tag)
    //        {
    //            
    //            Checked state = cb.GetFirstChild<Checked>();
    //            
    //            if (state == null)
    //            {
    //                state = new Checked();
    //                cb.AddChild(state);
    //            }
    //            
    //            state.Val = new OnOffValue(newState);
    //            
    //        }
    //    }
    //}

    //private void EditCheckbox(string tag, bool newState)
    //{
    //        
    //    foreach (SdtContentCheckBox cb in Body!.Descendants<SdtContentCheckBox>())
    //    {
    //        

    //        SdtProperties properties = (SdtProperties)cb.Parent;
    //        SdtRun parent = (SdtRun)properties.Parent;
    //        Tag? checkboxTag = properties.Descendants<Tag>().FirstOrDefault();

    //        if (checkboxTag != null && checkboxTag.Val == tag) //found correct checkbox
    //        {
    //            
    //            cb.Checked!.Val = newState ? OnOffValues.True : OnOffValues.False;
    //            
    //            SdtContentRun content = parent.Descendants<SdtContentRun>().FirstOrDefault();
    //            
    //            if (content != null)
    //            {
    //                Text text = content.Descendants<Text>().FirstOrDefault();
    //                if (text != null)
    //                {
    //                    int unicodeChar = int.Parse(cb.CheckedState.Val.Value, System.Globalization.NumberStyles.HexNumber);
    //                    text.Text = ((char)unicodeChar).ToString();
    //                }
    //            }
    //        }
    //    }
    //}
}