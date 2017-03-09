using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace GalexiconUX
{
    /// <summary>
    /// Interaction logic for NewGalexiconWindow.xaml
    /// </summary>
    public partial class NewGalexiconWindow : Window
    {
        public String DestinationFolder;
        public String GalexiconName;

        public NewGalexiconWindow()
        {
            InitializeComponent();
        }

        private void SetFolder_Click(object sender, RoutedEventArgs e)
        {
            var dg = new System.Windows.Forms.FolderBrowserDialog();
            dg.SelectedPath = DestinationFolder;

            if (dg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                DestinationFolder = dg.SelectedPath;
                tbFolder.Text = DestinationFolder;
            }
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            DestinationFolder = tbFolder.Text;
            GalexiconName = tbName.Text;

            if (DestinationFolder.Trim() == String.Empty || GalexiconName.Trim() == String.Empty)
            {
                MessageBox.Show("Name and Target folder cannot be empty!");
                return;
            }

            if (!Directory.Exists(DestinationFolder))
            {
                MessageBox.Show("Target folder doesn't exist!");
                return;
            }

            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
