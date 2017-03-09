using System;
using System.Collections.Generic;
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
    /// Interaction logic for ListEditorWindow.xaml
    /// </summary>
    public partial class ListEditorWindow : Window
    {
            public ListEditorWindow()
            {
                InitializeComponent();
            }

            private void NewCommandBinding_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
            {
                e.CanExecute = true;
            }

            private void NewCommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
            {
                var cvs = CollectionViewSource.GetDefaultView(EditorSelector.ItemsSource);
                if (cvs == null)
                    return;

                //var collection = cvs.SourceCollection as ICollection<Address>;
                //if (collection != null)
                //{
                //    Address address = new Address();
                //    address.Line1 = "Empty";
                //    collection.Add(address);
                //    cvs.MoveCurrentToLast();
                //}
            }

            private void DeleteCommandBinding_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
            {
                var cvs = CollectionViewSource.GetDefaultView(EditorSelector.ItemsSource);
                if (cvs == null)
                    return;

                e.CanExecute = cvs.CurrentItem != null;
            }

            private void DeleteCommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
            {
                //var cvs = CollectionViewSource.GetDefaultView(EditorSelector.ItemsSource);
                //if (cvs == null)
                //    return;

                //var currentItem = cvs.CurrentItem as Address;
                //if (currentItem == null)
                //    return;

                //var collection = cvs.SourceCollection as ICollection<Address>;
                //if (collection != null)
                //{
                //    collection.Remove(currentItem);
                //}
            }

            protected virtual void OnEditorWindowCloseExecuted(object sender, ExecutedRoutedEventArgs e)
            {
                //Window window = (Window)sender;
                //PropertyGridProperty prop = window.DataContext as PropertyGridProperty;
                //if (prop != null)
                //{
                //    prop.Executed(sender, e);
                //    if (e.Handled)
                //        return;
                //}
                //window.Close();
            }

            protected virtual void OnEditorWindowCloseCanExecute(object sender, CanExecuteRoutedEventArgs e)
            {
                //Window window = (Window)sender;
                //PropertyGridProperty prop = window.DataContext as PropertyGridProperty;
                //if (prop != null)
                //{
                //    prop.CanExecute(sender, e);
                //    if (e.Handled)
                //        return;
                //}
                //e.CanExecute = true;
            }
        }
    }
