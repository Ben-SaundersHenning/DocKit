using System.Text.RegularExpressions;

namespace DocKit.TemplateEngine;

internal static class FlagParser
{
    
    
    [GeneratedRegex(@"(?<key>[a-z]+)=(?<value>[a-z]+)", RegexOptions.Compiled)]
    private static partial Regex MyRegex();

    // TODO: parse the flags
    //:upper
    //:lower
    //:f=yyyy-mm-d    
    //:l=2    
    //:w=2    
    internal static Dictionary<string, string> ParseFlags(string flagsString)
    {

        // 1. split the string by spaces, trim each piece
        var opts = flagsString.Split(' ');
        
        for (int i = 0; i < flagsString.Length; i++)
        {
            var opt = opts[i].ToLowerInvariant().Trim();

            string regex = "(?<key>[a-z]+)=(?<value>[a-z]+)";
            Match m = Regex.Match(opt, regex, RegexOptions.IgnoreCase); 
            string key = m.Groups["key"].Value;
            string value = m.Groups["value"].Value;
        }
        
        // 2. Values should be in the form "options=value"
        // 3. Place option as key, value as value
        // 4. Return dictionary
        
        throw new NotImplementedException("FlagParser.ParseFlags() not implemented");
        
    }
    
}