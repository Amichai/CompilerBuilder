using BNFRuleParser;
using Irony.Parsing;
using Microsoft.Win32;
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
using System.Xml.Linq;
using TriAxis.RunSharp;

namespace CompilerProject {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged {
        public MainWindow() {
            InitializeComponent();

//            this.Rules = @"
//sem = "";"";
//binOp = ""+"" | ""-"";
//eq = ""="";
//bnfRule =  <csidentifier> | <number>;
//bnfRules = bnfRule | bnfRules + binOp + bnfRule;
//ruleDefinition = <csidentifier> + eq + bnfRules + sem;
//statement = ruleDefinition;
//statements = statement[+];
//root = statements;
//";



//            this.Input = @"
//identif = rule + 4 + 4 - testing + 3.34234;
//            ";

            var path = Properties.Settings.Default.LastSavePath;
            if (!string.IsNullOrWhiteSpace(path)) {
                this.open(path);
            }
            t.WriteLineEvent += (s, e) => {
                this.Output += e.Value + "\n";
            };
            Console.SetOut(t);
            if (this.SavePath != null) {
                try {
                    this.process();
                } catch {

                }
            }

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

        public string TitleText {
            get {
                string suffix = string.Empty;
                if (!string.IsNullOrWhiteSpace(SavePath)) {
                    suffix += " - ";
                    if (!this.Saved) {
                        suffix += "*";
                    }
                    suffix += SavePath;
                }
                return string.Format("Compiler Builder {0}", suffix); 
            }
        }

        private string _SavePath;
        public string SavePath {
            get { return _SavePath; }
            set {
                _SavePath = value;
                NotifyPropertyChanged("TitleText");
            }
        }

        private bool _Saved;
        public bool Saved {
            get { return _Saved; }
            set {
                _Saved = value;
                NotifyPropertyChanged("TitleText");
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e) {
            if (!this.Saved) {
                this.saveAs();
            } else {
                this.save(this.SavePath);
            }
        }


        private void saveAs() {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.ShowDialog();
            var name = sfd.FileName;
            if (string.IsNullOrWhiteSpace(name)) {
                return;
            }
            this.SavePath = name;
            this.save(name);
        }

        private void save(string name) {
            XElement def = new XElement("State");
            def.Add(new XAttribute("Definition", this.Rules));
            def.Add(new XAttribute("Input", this.Input));
            def.Save(name);
            this.Saved = true;
            Properties.Settings.Default.LastSavePath = name;
            Properties.Settings.Default.Save();
        }


        private void SaveAs_Click(object sender, RoutedEventArgs e) {
            this.saveAs();
        }

        private void Open_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog();
            var path = ofd.FileName;
            open(path);
        }

        private void open(string path) {
            Properties.Settings.Default.LastSavePath = path;
            Properties.Settings.Default.Save();
            this.SavePath = path;
            var xml = XElement.Load(path);
            this.Rules = xml.Attribute("Definition").Value;
            this.Input = xml.Attribute("Input").Value;
            this.Saved = true;
        }

        private void Input_TextChanged(object sender, TextChangedEventArgs e) {
            this.Saved = false;
        }

        private void Rules_TextChanged(object sender, TextChangedEventArgs e) {
            this.Saved = false;
        }
    }
}
