using BNFRuleParser;
using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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
sem = "";"";
binOp = ""+"" | ""-"";
eq = ""="";
bnfRule =  <csidentifier> | <number>;
bnfRules = bnfRule | bnfRules + binOp + bnfRule;
ruleDefinition = <csidentifier>+ eq + bnfRules + sem;
statement = ruleDefinition;
statements = statement[+];
root = statements;
";



            this.Input = @"
identif = rule + 4 + 4 - testing + 3.34234;
            ";


            t.WriteLineEvent += (s, e) => {
                this.Output += e.Value + "\n";
            };
            Console.SetOut(t);
            this.process();

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
            process();
        }
        
        ConsoleWriter t = new ConsoleWriter();

        private string _Output;
        public string Output {
            get { return _Output; }
            set {
                _Output = value;
                NotifyPropertyChanged();
            }
        }

        private void process() {
            if (this.executionPath == null) {
                this.compile();
            }
            this.Output = string.Empty;
            var r = AppDomain.CurrentDomain.ExecuteAssembly(executionPath, new string[] { this.Input });

        }
        private string executionPath = null;

        private void Compile_Click(object sender, RoutedEventArgs e) {
            compile();
        }

        private static int assemblyCounter = 0;
        private void compile() {
            ParseTree langTree = BNFParser.Parse(this.Rules);
            var cb = new CompilerBuilder();
            this.executionPath = cb.Build(langTree, assemblyCounter++);
            langTree = BNFParser.Parse(this.Rules);
        }
    }
}
