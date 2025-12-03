using System.Text.RegularExpressions;

namespace DocKit.TemplateEngine;

public class TemplateEngine
{
   public TemplateEngine(Regex matcher)
   {
      Matcher = matcher;
   }

   private Regex Matcher { get; set; }

   public void RunEngine(Document document, Func<string, string>? getReplacementString)
   {
      
      // getReplacementString takes in a string and returns a string
      
      Matcher = new Regex(
         @"<<(?<tagtype>if|/if|image|doc|logic||) *\[(?<operand>[ \w\[\]\\/._-]{3,})\](?<flags>[ \w\[\]\\/.:_-]*)>>|<<(?<tagtype>/if)>>",
         RegexOptions.Compiled);
      
      // 1. Isolate all the matches
      
   } 
    
}