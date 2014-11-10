using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNFRuleParser {
    class grammar : Grammar {
        public grammar() {
            var statements = new NonTerminal("Statements");
            var bnfStatement = new NonTerminal("BnfStatement");
            var nodeDefinition = new NonTerminal("NodeDefinition");
            var ruleDefinition = new NonTerminal("RuleDefinition");
            var bnfRules = new NonTerminal("BnfRules");
            var bnfRule = new NonTerminal("BNFRule");
            var identifier = TerminalFactory.CreateCSharpIdentifier("identifier");
            var number = TerminalFactory.CreateCSharpNumber("number");
            var descriptor = new NonTerminal("Descriptor");
            var qualifiedIdentifier = new NonTerminal("QualifiedIdentifier");
            var qualification = new NonTerminal("Qualification");
            var eq = ToTerm("=");   
            var or = ToTerm("|");
            var and = ToTerm("+");

            var q1 = ToTerm("*");
            var q2 = ToTerm("^");
            var sem = ToTerm(";");

            var b1 = ToTerm("[");
            var b2 = ToTerm("]");


            var binOp = new NonTerminal("binOp");

            descriptor.Rule = q1 | q2;
            bnfStatement.Rule = nodeDefinition | ruleDefinition;
            nodeDefinition.Rule = identifier + sem;
            ruleDefinition.Rule = identifier + eq + bnfRules + sem;
            bnfRules.Rule = MakePlusRule(bnfRules, null, bnfRule);
            binOp.Rule = and | or;
            RegisterOperators(1, "+", "|");
            this.MarkPunctuation(sem, b1, b2, eq);

            bnfRule.Rule = bnfRule + binOp + bnfRule | qualifiedIdentifier;

            qualifiedIdentifier.Rule = identifier + b1 + qualification + b2 | identifier;
            qualification.Rule = number | q1 | q2 | and;

            statements.Rule = MakePlusRule(statements, null, bnfStatement);
            this.Root = statements;

        }
    }
}
