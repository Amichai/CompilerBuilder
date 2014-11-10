using Helpers;
using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TriAxis.RunSharp;

namespace CompilerProject {
    public class CompilerBuilder {
        /// <summary>
        /// Returns the path to an assembly that can be executed using 
        /// AppDomain.CurrentDomain.ExecuteAssembly(path);
        /// </summary>
        /// <returns></returns>
        public string Build(ParseTree languageDefinition, int id) {
            var path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), string.Format("Grammar{0}.exe", id));
            AssemblyGen ag = new AssemblyGen(path);
            TypeGen grammar = ag.Public.Class("grammar", typeof(Grammar));
            CodeGen constructorDef = grammar.Constructor();
            foreach (var statement in languageDefinition.Root.ChildNodes) {
                var d = statement.ChildNodes.Single();
                var type = d.Term.ToString();
                string name;
                switch(type){
                    case "NodeDefinition":
                        name = d.ChildNodes.Single().Token.Value.ToString();
                        var programLocal = constructorDef.Local(Exp.New(typeof(NonTerminal), name));
                        operands[name] = programLocal;
                        break;
                    case "RuleDefinition":
                        name = d.ChildNodes.First().Token.Value.ToString();
                        if (!operands.ContainsKey(name)) {
                            operands[name] = constructorDef.Local(Exp.New(typeof(NonTerminal), name));
                        }
                        var o1 = operands[name];
                        var rules = d.ChildNodes[1];
                        Operand toAssign = null;
                        foreach (var rule in rules.ChildNodes) {
                            toAssign = parseBNFRule(rule, constructorDef);
                            constructorDef.Assign(o1.Field("Rule"), toAssign);
                        }
                        break;
                    default:
                        throw new Exception();
                }
            }
            constructorDef.Assign(constructorDef.This().Field("Root"), operands["root"]);

            TypeGen p = ag.Public.Class("Program");
            {
                CodeGen g2 = p.Public.Static.Method(typeof(void), "Main").Parameter(typeof(string[]), "args");
                {

                    var localGrammar = g2.Local(Exp.New(grammar));
                    var localParser = g2.Local(Exp.New(typeof(Parser), localGrammar));

                    ///Pass back the dll so the caller can execute with command line arguments
                    var args = g2.Arg("args");
                    var tree = localParser.Invoke("Parse", args[0]);
                    
                    var msgCount = tree.Field("ParserMessages").Property("Count");
                    g2.If(msgCount.GT(0));
                    var msg = tree.Field("ParserMessages")[0];
                    g2.WriteLine(msg);

                    g2.Else();
                    var cb = g2.Local(Exp.New(typeof(ToXmlHelper)));
                    g2.WriteLine(cb.Invoke("ToXml", tree));
                    g2.End();
                }
            }
            ag.Save();
            return path;
        }

        private Dictionary<string, Operand> operands = new Dictionary<string, Operand>();

        private Operand parseBNFRule(ParseTreeNode node, CodeGen constructorDef) {
            if (node.Term.ToString() == "QualifiedIdentifier") {
                Operand op;
                var identifierName = node.ChildNodes.First().Token.Value.ToString();
                Operand toAssign;
                if (operands.TryGetValue(identifierName, out op)) {
                    Debug.Print("matched");
                    toAssign = op;
                } else {
                    Debug.Print("not matched");
                    toAssign = constructorDef.This().Invoke("ToTerm", identifierName);
                    //toAssign = constructorDef.Local(typeof(Terminal), Exp.New(typeof(Terminal), identifierName));
                }
                if (node.ChildNodes.Count() == 2) {
                    var qualifier = node.ChildNodes.Last().ChildNodes.Single().Token.Value.ToString();
                    var set = constructorDef.Local(Exp.New(typeof(NonTerminal), identifierName + "_set"));
                    switch(qualifier){
                        case "+":
                            constructorDef.Assign(set.Field("Rule"), constructorDef.This().Invoke("MakePlusRule", set, null, toAssign));
                            break;
                        case "*":
                            constructorDef.Assign(set.Field("Rule"), constructorDef.This().Invoke("MakeStarRule", set, null, toAssign));
                            break;
                        default:
                            throw new Exception();
                    }
                      return set;
                }
                return toAssign;
            }

            if (node.ChildNodes.Count == 1) {
                return parseBNFRule(node.ChildNodes.Single(), constructorDef);
            } else if (node.ChildNodes.Count == 3) {
                var a = parseBNFRule(node.ChildNodes.First(), constructorDef);
                var b = parseBNFRule(node.ChildNodes.Last(), constructorDef);
                var val = node.ChildNodes[1].ChildNodes.Single().Term.ToString();
                switch (val) {
                    case "+":
                        return a.Add(b);
                    case "|":
                        return a.BitwiseOr(b);
                    default:
                        throw new Exception();
                }
            }
            throw new NotImplementedException();
        }


    }
}
