using System.Collections.Generic;
using System.Text.RegularExpressions;
using GenericRepository.Mongo.Pluralizer.Rules;

namespace GenericRepository.Mongo.Pluralizer
{
    internal class Pluralizer
    {
        private readonly IList<ReplaceRule> _pluralRules = PluralRules.GetRules();
        private readonly ICollection<string> _uncountables = Uncountables.GetUncountables();
        private readonly IDictionary<string, string> _irregularPlurals = IrregularRules.GetIrregularPlurals();
        private readonly IDictionary<string, string> _irregularSingles = IrregularRules.GetIrregularSingulars();

        private static readonly Regex _replacementRegex = new Regex("\\$(\\d{1,2})");

        public string Pluralize(string word)
        {
            return Transform(word, _irregularSingles, _irregularPlurals, _pluralRules);
        }

        private string RestoreCase(string originalWord, string newWord)
        {
            // Tokens are an exact match.
            if (originalWord == newWord)
                return newWord;

            // Lower cased words. E.g. "hello".
            if (originalWord == originalWord.ToLower())
                return newWord.ToLower();

            // Upper cased words. E.g. "HELLO".
            if (originalWord == originalWord.ToUpper())
                return newWord.ToUpper();

            // Title cased words. E.g. "Title".
            if (originalWord[0] == char.ToUpper(originalWord[0]))
                return char.ToUpper(newWord[0]) + newWord.Substring(1);

            // Lower cased words. E.g. "test".
            return newWord.ToLower();
        }

        private string ApplyRules(string token, string originalWord, IList<ReplaceRule> rules)
        {
            // Empty string or doesn't need fixing.
            if (string.IsNullOrEmpty(token) || _uncountables.Contains(token))
                return originalWord;


            // Iterate over the sanitization rules and use the first one to match.
            // Iterate backwards since specific/custom rules can be appended
            for (var i = rules.Count - 1; i >= 0; i--)
            {
                var rule = rules[i];

                // If the rule passes, return the replacement.
                if (rule.Condition.IsMatch(originalWord))
                {
                    var match = rule.Condition.Match(originalWord);
                    var matchString = match.Groups[0].Value;
                    if (string.IsNullOrWhiteSpace(matchString))
                        return rule.Condition.Replace(originalWord, GetReplaceMethod(originalWord[match.Index - 1].ToString(), rule.ReplaceWith), 1);
                    return rule.Condition.Replace(originalWord, GetReplaceMethod(matchString, rule.ReplaceWith), 1);
                }
            }

            return originalWord;
        }

        private MatchEvaluator GetReplaceMethod(string originalWord, string replacement)
        {
            return match =>
            {
                return RestoreCase(originalWord, _replacementRegex.Replace(replacement, m => match.Groups[int.Parse(m.Groups[1].Value)].Value));
            };
        }

        private string Transform(string word, IDictionary<string, string> replacables,
            IDictionary<string, string> keepables, IList<ReplaceRule> rules)
        {
            if (keepables.ContainsKey(word)) return word;
            if (replacables.TryGetValue(word, out string token)) return RestoreCase(word, token);
            return ApplyRules(word, word, rules);
        }
    }
}
