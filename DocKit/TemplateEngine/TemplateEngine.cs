using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Wordprocessing;

namespace DocKit.TemplateEngine;

public partial class TemplateEngine
{

   [GeneratedRegex(@"<<(?<tagtype>if|/if|image|doc|logic||) *\[(?<operand>[ \w\[\]\\/._-]{3,})\](?<flags>[ \w\[\]\\/.:_-]*)>>|<<(?<tagtype>/if)>>", RegexOptions.Compiled)]
   private static partial Regex MyRegex();
   
   private Regex? Matcher { get; set; }

   public void RunEngine(Document document, Func<string, string>? getReplacementString)
   {
      
      // getReplacementString takes in a string and returns a string
      
      Matcher = MyRegex();
      
      // TODO: this should be done prior
      // 1. Isolate all the matches
      MatchIsolator isolator = new MatchIsolator(document.Body, Matcher);
      isolator.IsolateAllTags();

      // 2. Call the tag processor, process the tags

   }

}