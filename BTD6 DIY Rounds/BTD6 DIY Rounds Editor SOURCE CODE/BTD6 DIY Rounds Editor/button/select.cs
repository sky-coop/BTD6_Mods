using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;
using static System.Reflection.Metadata.BlobBuilder;

namespace BTD6_DIY_Rounds
{
    public partial class MainWindow : Window
    {
        private void selector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string name = ((FrameworkElement)sender).Name;

            button_effect(name);
        }

        private string get_sel_str(string name)
        {
            return (string)((ComboBoxItem)((ComboBox)FindName
                (name)).SelectedItem).Content;
        }

    }
}
