﻿using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            CodeGen constructorDef = grammar.Constructor();
            Dictionary<string, Operand> operands = new Dictionary<string, Operand>();

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
                        var o1 = operands[name];
                        var rules = d.ChildNodes[1];
                        Operand toAssign = null;
                        foreach (var rule in rules.ChildNodes) {
                            string ruleType = rule.Term.ToString();
                            foreach (var r in rule.ChildNodes) {
                                string r2 = r.Term.ToString();
                                switch (r2) {
                                    case "QualifiedIdentifier":
                                        ///Check if this qualified identifier exists in our dictionary of operands
                                        Operand op;
                                        var identifierName = r.ChildNodes.Single().Token.Value.ToString();
                                        if (operands.TryGetValue(identifierName, out op)) {
                                            Debug.Print("matched");
                                            toAssign = op;
                                            
                                        } else {
                                            Debug.Print("not matched");
                                            //constructorDef.Invoke(constructorDef.This(), "ToTerm", Exp.New(typeof(string), identifierName));
                                            toAssign = constructorDef.This().Invoke("ToTerm", identifierName);
                                            //toAssign = constructorDef.Local(Exp.New(typeof(Terminal), identifierName));
                                        }
                                        break;
                                    case "BNFRule":
                                        break;
                                    default:
                                        throw new Exception();
                                }
                            }
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
                    var tree = localParser.Invoke("Parse", input);
                    ///Use the passed input;
                    ///Make it print the xml!
                    //var xml = tree.Invoke("ToXml");
                    //g2.WriteLine("Testing!");

                    var msgCount = tree.Field("ParserMessages").Property("Count");
                    g2.If(msgCount.GT(0));
                    var msg = tree.Field("ParserMessages")[0];
                    g2.WriteLine(msg);

                    g2.Else();
                    var output = tree.Field("Root").Property("ChildNodes")[0];
                    g2.WriteLine(output);
                    g2.End();


                }
            }
            ag.Save();
            return path;
        }
    }
}
