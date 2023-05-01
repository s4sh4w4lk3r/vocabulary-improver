using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
using wpf.Model;
using wpf.Model.Local;

namespace wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LocalProcessing.CreateFile("pizda.json");
            var lp = new LocalProcessing("pizda.json");
            lp.LocalProcessingEvent += DisplayMessage;
            lp.ReadFromJSON();
        }
        void DisplayMessage(string message) => Debug.WriteLine(message);
    }
}
