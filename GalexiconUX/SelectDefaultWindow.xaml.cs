using Galexicon;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for SelectDefaultWindow.xaml
    /// </summary>
    public partial class SelectDefaultWindow : Window
    {
        public SelectDefaultWindow()
        {
            InitializeComponent();
        }

        public FreeBody Selection;
        public IEnumerable<FreeBody> Defaults { 
            get
            {
                return _Defaults;
            }
            set
            {
                _Defaults = value;
                DefaultsList.ItemsSource = _Defaults;
            }
        } IEnumerable<FreeBody> _Defaults;


        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            if (DefaultsList.SelectedItem == null)
                return;

            Selection = DefaultsList.SelectedItem as FreeBody;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void DefaultsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PropertyGrid.SelectedObject = DefaultsList.SelectedItem;
        }
    }
}
