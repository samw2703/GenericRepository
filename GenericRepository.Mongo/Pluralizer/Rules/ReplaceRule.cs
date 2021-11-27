using System.Text.RegularExpressions;

namespace GenericRepository.Mongo.Pluralizer.Rules
{
    public class ReplaceRule
    {
        public Regex Condition { get; set; }
        public string ReplaceWith { get; set; }
    }
}
