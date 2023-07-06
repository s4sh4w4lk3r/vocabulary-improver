using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using ConsoleClient.View;

namespace ConsoleClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void btn_click(object sender, RoutedEventArgs e)
        {
/*            var daw = new DictionaryAdditionWizard();
            daw.ShowDialog();*/


            /*string btnXml = XamlWriter.Save(buttonTemplate);
            StringReader stringReader = new StringReader(btnXml);
            XmlReader xmlReader = XmlReader.Create(stringReader);
            Button newButton = (Button)XamlReader.Load(xmlReader); 
            listDictsPanel.Children.Add(newButton);*/
        }

        private void listDictsPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void listDictsPanel_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
        }
    }
}
