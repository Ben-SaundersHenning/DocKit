using System.Text.RegularExpressions;

namespace DocKit.Utility;

public static class MatchExtensions
{

   extension(Match match)
   {
      
      public int EndIndex => match.Index + match.Length - 1;
      
   }
   
}
