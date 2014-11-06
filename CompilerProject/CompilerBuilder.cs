using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TriAxis.RunSharp;

namespace CompilerProject {
    public static class CompilerBuilder {
        /// <summary>
        /// Returns the path to an assembly that can be executed using 
        /// AppDomain.CurrentDomain.ExecuteAssembly(path);
        /// </summary>
        /// <returns></returns>
        public static string Build(ParseTree languageDefinition, string input) {
            var path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Grammar1.exe");
            AssemblyGen ag = new AssemblyGen(path);
            TypeGen grammar = ag.Public.Class("grammar", typeof(Grammar));
            CodeGen g = grammar.Constructor();
            Dictionary<string, Operand> operands = new Dictionary<string, Operand>();

            foreach (var statement in languageDefinition.Root.ChildNodes) {
                var d = statement.ChildNodes.Single();
                var type = d.Term.ToString();
                string name;
                switch(type){
                    case "NodeDefinition":
                        name = d.ChildNodes.Single().Token.Value.ToString();
                        var programLocal = g.Local(Exp.New(typeof(NonTerminal), name));
                        operands[name] = programLocal;
                        break;
                    case "RuleDefinition":
                        name = d.ChildNodes.First().Token.Value.ToString();
                        var o1 = operands[name];
                        var rules = d.ChildNodes[1];
                        foreach (var rule in rules.ChildNodes) {
                            var r = rule.ChildNodes.Single();
                            string ruleType = r.Term.ToString();
                            //g.Assign(o1.Field("Rule", ))
                        }
                        break;
                    default:
                        throw new Exception();
                }
            }
            g.Assign(g.This().Field("Root"), operands["root"]);

            TypeGen p = ag.Public.Class("Program");
            {
                CodeGen g2 = p.Public.Static.Method(typeof(void), "Main").Parameter(typeof(string[]), "args");
                {

                    var localGrammar = g2.Local(Exp.New(grammar));
                    var localParser = g2.Local(Exp.New(typeof(Parser), localGrammar));
                    var tree = localParser.Invoke("Parse", "1");
                    //var tree = localParser.Invoke(this.Input, "1");

                    //var xml = tree.Invoke("ToXml");
                    //g2.WriteLine("Testing!");

                    var output = tree.Field("Root").Property("ChildNodes")[0];
                    g2.WriteLine(output);

                }
            }
            ag.Save();

            return path;

            throw new NotImplementedException();
        }
    }
}
