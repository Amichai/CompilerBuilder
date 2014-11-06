using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNFRuleParser {
    public static class BNFParser {
        public static ParseTree Parse(string input) {
            var g = new grammar();
            var parser = new Parser(g);
            var tree = parser.Parse(input);
            if (tree.ParserMessages.Count() > 0) {
                Debug.Print(string.Join(", ", tree.ParserMessages));
            } else {
                var result = tree.ToXml();
                Debug.Print(result);
            }
            return tree;
        }   
    }
}
