using BNFRuleParser;
using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TriAxis.RunSharp;

namespace CompilerProject {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged {
        public MainWindow() {
            InitializeComponent();
            this.Rules = @"
testing;
program;
root;
root = z;
program = e[0] | e[0];
program = e[*] + e[+];
program = e[0] | e;
program = e | e;
";

        }

        private string _Input;
        public string Input {
            get { return _Input; }
            set {
                _Input = value;
                NotifyPropertyChanged();
            }
        }

        private string _Rules;
        public string Rules {
            get { return _Rules; }
            set {
                _Rules = value;
                NotifyPropertyChanged();
            }
        }

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "") {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion INotifyPropertyChanged Implementation

        private void Process_Click(object sender, RoutedEventArgs e) {
            ParseTree langTree = BNFParser.Parse(this.Rules);

            var executionPath = CompilerBuilder.Build(langTree, this.Input);

            var path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Grammar1.exe");
            AssemblyGen ag = new AssemblyGen(path);
            
            TypeGen grammar = ag.Public.Class("grammar", typeof(Irony.Parsing.Grammar));

            CodeGen g = grammar.Constructor();
            {
                var programLocal = g.Local(Exp.New(typeof(Irony.Parsing.NonTerminal), "Equation"));
                var numberLocal = g.Local(Exp.New(typeof(Irony.Parsing.NumberLiteral), "Number"));
                g.Assign(programLocal.Field("Rule"), numberLocal);
                g.Assign(g.This().Field("Root"), programLocal);

            }

            TypeGen p = ag.Public.Class("Program");
            {
                CodeGen g2 = p.Public.Static.Method(typeof(void), "Main").Parameter(typeof(string[]), "args");
                {

                    var localGrammar = g2.Local(Exp.New(grammar));
                    var localParser = g2.Local(Exp.New(typeof(Irony.Parsing.Parser), localGrammar));
                    var tree = localParser.Invoke("Parse", "1");
                    //var tree = localParser.Invoke(this.Input, "1");

                    //var xml = tree.Invoke("ToXml");
                    //g2.WriteLine("Testing!");
                    
                    var output = tree.Field("Root").Property("ChildNodes")[0];
                    g2.WriteLine(output);

                }
            }

            ag.Save();
            ///TODO: this should take the input as a command line argument
            AppDomain.CurrentDomain.ExecuteAssembly(path);
        }
    }
}
