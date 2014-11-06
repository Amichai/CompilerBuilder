using BNFRuleParser;
using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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
equation;
number;
root;
root = t;
";

            this.Input = "t";

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

            var executionPath = CompilerBuilder.Build(langTree);
            //Process.Start(executionPath, this.Input);
            AppDomain.CurrentDomain.ExecuteAssembly(executionPath, new string[] {this.Input});
        }
    }
}
