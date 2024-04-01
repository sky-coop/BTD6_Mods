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
        public class rainbow_text
        {
            public string name;
            public List<Tuple<string, ARGB>> list;
            public string target;
            public HorizontalAlignment ha;
            public VerticalAlignment va;
            //Thickness
            public double left;
            public double top;
            public double right;
            public double bottom;

            public double w;
            public double h;
            public double size;

            public rainbow_text(string NAME)
            {
                list = new List<Tuple<string, ARGB>>();
                name = NAME;
            }

            public void prepare(string TARGET, HorizontalAlignment HA,
                VerticalAlignment VA, Thickness T,
                double W, double H, double SIZE)
            {
                target = TARGET;
                ha = HA;
                va = VA;

                left = T.Left;
                top = T.Top;
                right = T.Right;
                bottom = T.Bottom;

                w = W;
                h = H;
                size = SIZE;
            }

            public Thickness GetThickness()
            {
                return new Thickness(left, top, right, bottom);
            }

            public void add(string s, byte r, byte g, byte b, byte a = 255)
            {
                list.Add(new Tuple<string, ARGB>(s, new ARGB(a, r, g, b)));
            }
        }
        public bool exist_elem(string name, Panel target)
        {
            FrameworkElement p = (FrameworkElement)FindName(name);
            if (p != null && p.Parent != null && p.Parent.Equals(target))
            {
                return true;
            }
            return false;
        }

        //必须rainbow_text::prepare()
        public WrapPanel draw_r_text(rainbow_text rt, int len = 2)
        {
            if (rt == null)
            {
                return null;
            }
            Panel g = (Panel)FindName(rt.target);
            string name_base = g.Name + "__" + rt.name;
            bool e = exist_elem(name_base, g);

            WrapPanel con;
            if (!e)
            {
                con = new WrapPanel
                {
                    Name = name_base,
                    HorizontalAlignment = rt.ha,
                    VerticalAlignment = rt.va,
                    Width = rt.w,
                    Height = rt.h,
                    Margin = rt.GetThickness(),
                };
                reg_name(g, con);
            }
            con = (WrapPanel)FindName(name_base);

            int k = 0;
            foreach (Tuple<string, ARGB> t in rt.list)
            {
                int index = 0;
                int remain = t.Item1.Length;
                while (remain > 0 || t.Item1 == "")
                {
                    int sublen = Math.Min(remain, len);
                    string sub = t.Item1.Substring(index, sublen);
                    remain -= sublen;
                    index += sublen;

                    TextBlock tb = null;
                    if (exist_elem(name_base + "_rt" + k, con))
                    {
                        tb = (TextBlock)FindName(name_base + "_rt" + k);
                    }
                    if (tb != null)
                    {
                        tb.FontSize = rt.size;
                        tb.Foreground = t.Item2.toBrush();
                        tb.Text = sub;
                        if (tb.Visibility == Visibility.Collapsed)
                        {
                            tb.Visibility = Visibility.Visible;
                        }
                    }
                    else
                    {
                        tb = new TextBlock
                        {
                            Name = name_base + "_rt" + k,
                            FontSize = rt.size,
                            Foreground = t.Item2.toBrush(),
                            Text = sub,
                            IsHitTestVisible = false,
                            FontFamily = new FontFamily("SimHei"),
                        };
                        reg_name(con, tb);
                    }
                    if (t.Item1 == "") //换行
                    {
                        if (k == 0)
                        {
                            tb.Width = g.ActualWidth;
                        }
                        else
                        {
                            TextBlock last = (TextBlock)FindName(name_base + "_rt" + (k - 1));
                            Point p = (Point)VisualTreeHelper.GetOffset(last);
                            tb.Width = Math.Max(0, g.ActualWidth - p.X - last.ActualWidth
                                - con.Margin.Left - con.Margin.Right);
                        }
                        k++;
                        break;
                    }
                    else
                    {
                        tb.Width = double.NaN;
                    }
                    k++;
                }
            }

            while (true)
            {
                TextBlock tb = (TextBlock)FindName(name_base + "_rt" + k);
                if (tb != null)
                {
                    tb.Width = double.NaN;
                    tb.Visibility = Visibility.Collapsed;
                }
                else
                {
                    break;
                }
                k++;
            }
            return con;
        }

    }
}
