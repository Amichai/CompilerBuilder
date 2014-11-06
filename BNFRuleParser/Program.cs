using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNFRuleParser {
    class Program {
        static void Main(string[] args) {
            var g = new grammar();
            var parser = new Parser(g);
            var tree = parser.Parse(toParse);
            if (tree.ParserMessages.Count() > 0) {
                Debug.Print(string.Join(", ", tree.ParserMessages));
            } else {
                var result = tree.ToXml();
                Debug.Print(result);
            }

        }

        private static string toParse = @"
testing;
program = e[0] | e[0];
program = e[*] + e[+];
program = e[0] | e;
program = e | e | e;
";
//program = e | e;
//expressionList = expression + t
//inclusiveIdentifier.Rule = op | identifier | number;
//identifierList.Rule = MakePlusRule(identifierList, number, inclusiveIdentifier);
//expression.Rule = equation | identifierList | specialChar;
//op.Rule = ToTerm(""+"") | ""-"" | ""*"" | ""/"" | ""**"" | ""="";
//specialChar.Rule = e + identifier;
//equation.Rule = e + identifier + argumentList;
//argument.Rule = l + argumentVal + r;
//argumentList.Rule = MakePlusRule(argumentList, null, argument);
//argumentVal.Rule = expressionList;
//this.Root = program;
////this.RegisterBracePair(""{"", ""}"");
//this.MarkPunctuation(""{"", ""}"", @""\"");
    }
}
