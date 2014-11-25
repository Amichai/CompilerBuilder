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
            var ruleDefinition = new NonTerminal("RuleDefinition");
            var bnfRules = new NonTerminal("BnfRules");
            var bnfRule = new NonTerminal("BNFRule");
            var identifier = TerminalFactory.CreateCSharpIdentifier("identifier");
            var stringLiteral = new StringLiteral("identifier", "\"");
            //var identifier = TerminalFactory.CreateCSharpString("identifier");
            var number = TerminalFactory.CreateCSharpNumber("number");
            var descriptor = new NonTerminal("Descriptor");
            var qualifiedIdentifier = new NonTerminal("QualifiedIdentifier");
            var qualification = new NonTerminal("Qualification");
            var specialType = new NonTerminal("SpecialType");
            var eq = ToTerm("=");   
            var or = ToTerm("|");
            var and = ToTerm("+");

            var q1 = ToTerm("*");
            var q2 = ToTerm("^");
            var sem = ToTerm(";");

            var b1 = ToTerm("[");
            var b2 = ToTerm("]");

            var lb = ToTerm("<");
            var rb = ToTerm(">");

            var binOp = new NonTerminal("binOp");

            var nonTerminal = new NonTerminal("NonTerminal");
            specialType.Rule = lb + identifier + rb;
            descriptor.Rule = q1 | q2;
            bnfStatement.Rule = ruleDefinition;
            ruleDefinition.Rule = identifier + eq + bnfRules + sem | identifier + sem;
            bnfRules.Rule = MakePlusRule(bnfRules, null, bnfRule);
            binOp.Rule = and | or;
            RegisterOperators(1, "+", "|");
            this.MarkPunctuation(sem, b1, b2, eq, rb, lb);

            bnfRule.Rule = bnfRule + binOp + bnfRule | qualifiedIdentifier;
            nonTerminal.Rule = identifier | stringLiteral | specialType;
            qualifiedIdentifier.Rule = 
                nonTerminal + b1 + qualification + b2 | nonTerminal;
            qualification.Rule = number | q1 | q2 | and;

            statements.Rule = MakePlusRule(statements, null, bnfStatement);
            this.Root = statements;

        }
    }
}
