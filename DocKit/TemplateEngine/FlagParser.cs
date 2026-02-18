using System.Text.RegularExpressions;

namespace DocKit.TemplateEngine;

internal static class FlagParser
{
    
    internal static Dictionary<string, string> ParseFlags(string flagsString)
    {

        Dictionary<string, string> flags = new();
        
        // 1. split the string by spaces, trim each piece
        var opts = flagsString.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        
        for (int i = 0; i < opts.Length; i++)
        {
            
            // convert to lower, trim, and strip the leading ':'.
            var opt = opts[i].Trim().Substring(1);

            string key = "";
            string value = "";

            // tags like :f=yyyy, :l=2
            if (opt.Contains('='))
            {
                var split = opt.Split('=');
                key = split[0];
                value = split[1];
            }
            else // tags like :upper, :lower
            {
                key = opt.Trim();
                value = "true";
            }
            
            flags.Add(key, value);


            //string regex = "(?<key>[a-z]+)=(?<value>[a-z]+)";
            //Match m = Regex.Match(opt, regex, RegexOptions.IgnoreCase); 
            //string key = m.Groups["key"].Value;
            //string value = m.Groups["value"].Value;
            //flags.Add(key, value);
            
        }
        
        // 2. Values should be in the form "options=value"
        // 3. Place option as key, value as value
        // 4. Return dictionary

        return flags;
        
    }
    
}