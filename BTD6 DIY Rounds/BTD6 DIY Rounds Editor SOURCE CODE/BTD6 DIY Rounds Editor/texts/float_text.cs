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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.CodeDom;
using System.Xml.Linq;
using System.Security.Cryptography;

namespace BTD6_DIY_Rounds
{
    public partial class MainWindow : Window
    {
        public class float_message
        {
            public string text;
            public SolidColorBrush text_color;
            public double fontsize;

            public double remain_time;
            public double time;
            public bool showing = false;

            public string name;
            public string target;
            public HorizontalAlignment ha;
            public VerticalAlignment va;
            public double width;
            public double height;
            public Thickness t;

            public SolidColorBrush background = null;

            public float_message(string name, string target,
                HorizontalAlignment ha, VerticalAlignment va,
                double w, double h, Thickness t,
                SolidColorBrush background,
                string text, SolidColorBrush text_color, double fontsize,
                double time)
            {
                this.name = name;
                this.target = target;
                this.text = text;
                this.text_color = text_color;
                this.fontsize = fontsize;
                this.time = time;
                remain_time = time;

                this.background = background;
                this.ha = ha;
                this.va = va;
                this.width = w;
                this.height = h;
                this.t = t;
            }
        }

        public List<float_message> float_messages =
            new List<float_message>();

        public decimal float_message_id = 0;

        public Dictionary<string, Tuple<string, Point>> temp_texts =
            new Dictionary<string, Tuple<string, Point>>();

        public void float_message_tick(double tick_time)
        {
            List<float_message> del_temp = new List<float_message>();
            foreach (float_message f in float_messages)
            {
                double ratio = f.remain_time / f.time;
                byte a = (byte)(ratio * 255);
                f.remain_time -= tick_time;

                if (f.showing == false)
                {
                    Panel p = (Panel)FindName(f.target);
                    TextBlock x = new TextBlock
                    {
                        Name = p.Name + "_float_" + f.name,
                        HorizontalAlignment = f.ha,
                        VerticalAlignment = f.va,
                        Foreground = f.text_color,
                        Text = f.text,
                        TextWrapping = TextWrapping.Wrap,
                        FontSize = f.fontsize,
                        FontFamily = new FontFamily("SimHei"),
                        Margin = new Thickness(f.t.Left, f.t.Top, f.t.Right, f.t.Bottom),
                    };
                    if (f.background != null)
                    {
                        x.Background = f.background;
                    }
                    x.IsHitTestVisible = false;
                    reg_name(p, x);
                    f.showing = true;
                }
                if (ratio > 0)
                {
                    TextBlock x = (TextBlock)FindName
                        (f.target + "_float_" + f.name);

                    f.text_color = C(a, 
                        f.text_color.Color.R,
                        f.text_color.Color.G,
                        f.text_color.Color.B);
                    x.Foreground = f.text_color;

                    if (f.background != null)
                    {
                        f.background = C(a,
                            f.background.Color.R,
                            f.background.Color.G,
                            f.background.Color.B);
                        x.Background = f.background;
                    }
                }
                else
                {
                    Panel p = (Panel)FindName(f.target);
                    TextBlock x = (TextBlock)FindName
                        (f.target + "_float_" + f.name);
                    p.Children.Remove(x);
                    del_temp.Add(f);
                }
            }
            foreach (float_message f in del_temp)
            {
                float_messages.Remove(f);
            }
        }

        public void float_default(double dx, double dy, string text, SolidColorBrush back, 
            SolidColorBrush fore, double fontsize = 11, double time = 2000)
        {
            Point p = Mouse.GetPosition(inf_grid);
            Point p2 = new Point(p.X + dx, p.Y + dy);
            float_message f = new float_message(
                "f" + float_message_id, inf_grid.Name, HorizontalAlignment.Left,
                VerticalAlignment.Top, double.NaN, double.NaN,
                new Thickness(p2.X, p2.Y, 0, 0),
                back,
                text, fore, fontsize, time);
            float_message_id++;
            float_messages.Add(f);
        }
    }
}
