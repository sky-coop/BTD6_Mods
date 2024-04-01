using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace BTD6_DIY_Rounds
{
    public partial class MainWindow : Window
    {
        private TextBlock T(string name)
        {
            return FindName(name) as TextBlock;
        }
        private TextBox BOX(string name)
        {
            return FindName(name + "_box") as TextBox;
        }
        private Rectangle R(string name)
        {
            return FindName(name) as Rectangle;
        }
        private Grid G(string name)
        {
            return FindName(name) as Grid;
        }
        private ComboBox SEL(string name)
        {
            return FindName(name + "_select") as ComboBox;
        }
    }
}
