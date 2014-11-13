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
identifier = ""id"";
eq = ""="";
bnfRule =  rule | bnfRule + binOp + bnfRule;
bnfRules = bnfRule[+];
nodeDefinition = identifier + sem;
ruleDefinition = identifier + eq + bnfRules + sem;
statement = nodeDefinition | ruleDefinition;
statements = statement[+];
root = statements;
";
//root = t[+] + g[*];
//root = t[+] + j;


            this.Input = @"
id;
id;
id = rule;
            ";
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

        private void process() {
            if (this.executionPath == null) {
                this.compile();
            }
            AppDomain.CurrentDomain.ExecuteAssembly(executionPath, new string[] { this.Input });

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
