using System.Text.RegularExpressions;
using DocKit.Images;
using DocumentFormat.OpenXml.Wordprocessing;

namespace DocKit.TemplateEngine;

// TODO: this class will process all tags that are not just search and replace.
// Conditionals, documents, etc..

internal class TagProcessor(Document document)
{
    private Body body = document.Body;


    internal void ProcessDocument(Func<string, string> getReplacementString, Regex matcher)
    {
        
        var tags = document.Body.Descendants<Run>().Where(r =>
            r.ExtendedAttributes.Any(attr => attr is 
                { Prefix: "tmplTag", LocalName: "isTag", NamespaceUri: "jstg", Value: "true" }));

        foreach (Run r in tags)
        {
            
            Match match = matcher.Match(r.GetFirstChild<Text>()!.Text);
            Text text = r.GetFirstChild<Text>()!;
            
            string tagType = match.Groups["tagtype"].Value;
            string operand = match.Groups["operand"].Value;
            string flags = match.Groups["flags"].Value;
            
            Dictionary<string, string> parsedFlags = FlagParser.ParseFlags(flags);
                
            switch (tagType.Trim())
            {
                case "if":
                    Console.Error.WriteLine("if not implemented");
                    continue;
                
                case "/if":
                    Console.Error.WriteLine("end if not implemented");
                    continue;
                
                case "image":
                    // TODO: check if operand is a valid image path
                    Image image = new Image(operand);
                    image.FitToBounds(2, 2);
                    document.AddImage(image, r);
                    text.Remove();
                    continue;
                
                case "doc":
                    Console.Error.WriteLine("doc not implemented");
                    continue;
                
                default: // regular tag
                    Console.WriteLine("regular");
                    break;
                
            }
            
            // Regular tag, just a search and replace
            string replacement = getReplacementString(operand);
            text.Text = replacement;

        }
        
    }

    // Evaluates all tags matching the <<[]>> syntax inside the document.
    //public void ProcessDocument(Func<string, string>? getReplacementString, IDocumentLogic? logic)
    //public void ProcessDocumentOld(Func<string, string>? getReplacementString)
    //{
    //    
    //    foreach (Match match in Matcher.Matches(text.Text))
    //    {

    //        string tagType = match.Groups["tagtype"].Value;
    //        string operand = match.Groups["operand"].Value;
    //        string flags = match.Groups["flags"].Value;
    //        
    //        string replacement = getReplacementString!(operand);

    //        switch (tagType.Trim())
    //        {
    //            
    //            // TODO: WHAT IF THE TAGS ARE NOT IN THE SAME PARAGRAPH?
    //           case "if":
    //               
    //               bool? result = logic.GetRule(operand);
    //               int k = j + 1;

    //               // keep the content in the if
    //               // traverse until finding the </if>
    //               if (result is true)
    //               { 
    //                   Text nextText = texts.ElementAt(k);
    //                   while (nextText.Text != "<</if>>" && nextText.Text != $"<</if [{operand}]>>")
    //                   {
    //                       nextText = texts.ElementAt(++k);
    //                   }
    //                   nextText.Remove();
    //               }
    //               else
    //               {
    //                   Text nextText = texts.ElementAt(k);
    //                   while (nextText.Text != "<</if>>" && nextText.Text != $"<</if [{operand}]>>")
    //                   {
    //                       nextText.Remove();
    //                       k = k - 1;
    //                       nextText = texts.ElementAt(++k);
    //                   }

    //                   nextText.Remove();
    //               }

    //               // remove the <<if [...]>>
    //               text.Remove();
    //               
    //               continue;
    //           
    //           case "/if":
    //               continue;
    //           
    //           case "image":
    //               Drawing img = GetImage(new Image(replacement));
    //               text.InsertAfterSelf(img);
    //               text.Remove();
    //               continue;
    //           case "doc":
    //               // relative path to parent doc
    //               string docPath = operand;
    //               string subDoc = $"{DirPath}/{docPath}";
    //                
    //               if (!File.Exists(subDoc))
    //               {
    //                   text.Text = text.Text.Replace(match.Value, $"<<NULL: {docPath} DOES NOT EXIST>>");
    //                   continue;
    //               }

    //               Document toInsert = new Document(subDoc, DocumentType.ExistingDocument);
    //               this.ReplaceTextWithDocument(match.Value, toInsert, getReplacementString, data, logic);
    //               toInsert.Dispose();
    //               continue;
    //           
    //           default: // regular tags
    //               break;
    //        }

    //        DateOnly date;

    //        // this is a date, need to check formatting.
    //        // ex: << [key.date] :f YYYY-mm-dd >>
    //        if (DateOnly.TryParse(replacement, out date) && flags.Contains(":f"))
    //        {
    //            List<string> switchStrings = flags.Split(' ').ToList();
    //            int index = switchStrings.FindIndex(s => s.Contains(":f")) + 1;
    //            string dateFormat = switchStrings[index].Replace('-', ' ');
    //            text.Text = text.Text.Replace(match.Value, date.ToString(dateFormat));
    //        }
    //        
    //        // a pronoun
    //        // ex: << [key.gender] :p0 >>
    //        // ex: << [key.gender] :p0 :upper >>
    //        // p0, p1, p2, p3 are valid switches
    //        if (operand.Contains("gender"))
    //        {
    //            
    //            //temp, isolates the p# switches
    //            flags = Regex.Replace(flags, @"\s+", "");
    //            if(flags.Length > 0 && flags[0] == ':') {flags = flags.Substring(1);}
    //            List<string> allSwitches = flags.Trim().Split(':').ToList(); 
    //            allSwitches.RemoveAll(s => s.Length < 1);
    //            
    //            switch (allSwitches.FindLast(s => s[0] == 'p'))
    //            {
    //               case "p0":
    //                   if (replacement == "Male") { replacement = "mr"; }
    //                   else if (replacement == "Female") { replacement = "ms"; }
    //                   else { replacement = "mx"; }
    //                   break;
    //               case "p1":
    //                   if (replacement == "Male") { replacement = "male"; }
    //                   else if (replacement == "Female") { replacement = "female"; }
    //                   else { replacement = "person"; }
    //                   break;
    //               case "p2":
    //                   if (replacement == "Male") { replacement = "he"; }
    //                   else if (replacement == "Female") { replacement = "she"; }
    //                   else { replacement = "they"; }
    //                   break;
    //               case "p3":
    //                   if (replacement == "Male") { replacement = "his"; }
    //                   else if (replacement == "Female") { replacement = "her"; }
    //                   else { replacement = "their"; }
    //                   break;
    //               case "p4":
    //                   if (replacement == "Male") { replacement = "himself"; }
    //                   else if (replacement == "Female") { replacement = "herself"; }
    //                   else { replacement = "themself"; }
    //                   break;
    //            }
    //        }
    //        
    //        if (flags.Contains(":upper"))
    //        {
    //            text.Text = text.Text.Replace(match.Value,
    //                Utility.ToUpperFirstChar(replacement));
    //        }
    //        else if (flags.Contains(":lower"))
    //        {
    //            text.Text = text.Text.Replace(match.Value,
    //                Utility.ToLowerFirstChar(replacement));
    //        }
    //        else
    //        {
    //            text.Text = text.Text.Replace(match.Value, replacement);
    //        }

    //    }
    //    
    //}
    
}