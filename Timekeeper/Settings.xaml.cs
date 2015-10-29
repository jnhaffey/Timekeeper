using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;
using System.Xml.Serialization;
using Timekeeper.SettingsTypes;

namespace Company.Timekeeper
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
            var serial = new StringBuilder();
            XmlSerializer xml = new XmlSerializer(Properties.Settings.Default.StateNameConfiguration.GetType());
            using (var textStream = new StringWriter(serial))
            {
                xml.Serialize(textStream, Properties.Settings.Default.StateNameConfiguration);
            }
            TextBox.Text = serial.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var serial = new StringBuilder();
            XmlSerializer xml = new XmlSerializer(Properties.Settings.Default.StateNameConfiguration.GetType());
            using (var textStream = new StringReader(TextBox.Text))
            {
                Properties.Settings.Default.StateNameConfiguration = (ProjectStateNamesCollection)xml.Deserialize(textStream);
            }
            Properties.Settings.Default.Save();
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
