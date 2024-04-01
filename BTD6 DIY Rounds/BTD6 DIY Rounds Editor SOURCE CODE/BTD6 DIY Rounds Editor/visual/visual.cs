using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
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

namespace BTD6_DIY_Rounds
{
    public partial class MainWindow : Window
    {
        public class ARGB
        {
            [NonSerialized]
            public SolidColorBrush color;

            public byte a;
            public byte r;
            public byte g;
            public byte b;

            [Newtonsoft.Json.JsonConstructor]
            public ARGB(byte A, byte R, byte G, byte B)
            {
                change(A, R, G, B);
            }

            public ARGB(SolidColorBrush s)
            {
                fromColor(s.Color);
            }

            public void fromColor(Color x)
            {
                a = x.A;
                r = x.R;
                g = x.G;
                b = x.B;
                update();
            }

            public SolidColorBrush toBrush()
            {
                if (color == null)
                {
                    update();
                }
                return color;
            }

            public Color toColor()
            {
                return Color.FromArgb(a, r, g, b);
            }

            public void change(byte A, byte R, byte G, byte B)
            {
                a = A;
                r = R;
                g = G;
                b = B;
                update();
            }

            public void update()
            {
                color = new SolidColorBrush(Color.FromArgb(a, r, g, b));
            }
        };
        private SolidColorBrush getSCB(Color c)
        {
            return new SolidColorBrush(c);
        }

        public SolidColorBrush C(byte r, byte g, byte b)
        {
            return getSCB(Color.FromRgb(r, g, b));
        }

        public SolidColorBrush C(byte a, byte r, byte g, byte b)
        {
            return getSCB(Color.FromArgb(a, r, g, b));
        }

        private void set_lbtn(FrameworkElement r)
        {
            r.Tag = new button_tag();
            r.MouseEnter += btn_enter;
            r.MouseLeave += btn_leave;
            r.MouseLeftButtonDown += btn_down;
            r.MouseLeftButtonUp += btn_up;
            r.MouseMove += btn_move;
        }

        private void set_lrbtn(FrameworkElement r)
        {
            r.Tag = new button_tag();
            r.MouseEnter += btn_enter;
            r.MouseLeave += btn_leave;
            r.MouseLeftButtonDown += btn_down;
            r.MouseLeftButtonUp += btn_up;
            r.MouseMove += btn_move;
            r.MouseRightButtonDown += btn_rdown;
            r.MouseRightButtonUp += btn_rup;
        }

        private class button_tag
        {
            public bool enter = false;
            public bool left_clicking = false;
            public bool right_clicking = false;
        }

        private void btn_rup(object sender, MouseButtonEventArgs e)
        {
            Rectangle btn = (Rectangle)sender;
            Rectangle background = (Rectangle)FindName(btn.Name + "_bg");
            TextBlock text = (TextBlock)FindName(btn.Name + "_text");

            button_tag tag = (button_tag)btn.Tag;
            if (tag.right_clicking)
            {
                background.Fill = color_mul_r(background.Fill, 2);
                tag.right_clicking = false;

                if (!screenshooting)
                {
                    button_effect_r(btn.Name);
                }
            }
        }

        private void btn_rdown(object sender, MouseButtonEventArgs e)
        {
            Rectangle btn = (Rectangle)sender;
            Rectangle background = (Rectangle)FindName(btn.Name + "_bg");
            TextBlock text = (TextBlock)FindName(btn.Name + "_text");

            button_tag tag = (button_tag)btn.Tag;
            if (!tag.right_clicking)
            {
                background.Fill = color_mul_r(background.Fill, 0.5);
                tag.right_clicking = true;
            }
        }

        private void btn_move(object sender, MouseEventArgs e)
        {
            Rectangle btn = (Rectangle)sender;
            Rectangle background = (Rectangle)FindName(btn.Name + "_bg");
            TextBlock text = (TextBlock)FindName(btn.Name + "_text");

            button_tag tag = (button_tag)btn.Tag;
        }

        private void btn_up(object sender, MouseButtonEventArgs e)
        {
            Rectangle btn = (Rectangle)sender;
            Rectangle background = (Rectangle)FindName(btn.Name + "_bg");
            TextBlock text = (TextBlock)FindName(btn.Name + "_text");

            button_tag tag = (button_tag)btn.Tag;
            if (tag.left_clicking)
            {
                background.Fill = color_mul(background.Fill, 2);
                tag.left_clicking = false;

                if (!screenshooting)
                {
                    button_effect(btn.Name);
                }
            }
        }

        private void btn_down(object sender, MouseButtonEventArgs e)
        {
            Rectangle btn = (Rectangle)sender;
            Rectangle background = (Rectangle)FindName(btn.Name + "_bg");
            TextBlock text = (TextBlock)FindName(btn.Name + "_text");

            button_tag tag = (button_tag)btn.Tag;
            if (!tag.left_clicking)
            {
                background.Fill = color_mul(background.Fill, 0.5);
                tag.left_clicking = true;
            }
        }

        private void btn_leave(object sender, MouseEventArgs e)
        {
            Rectangle btn = (Rectangle)sender;
            Rectangle background = (Rectangle)FindName(btn.Name + "_bg");
            TextBlock text = (TextBlock)FindName(btn.Name + "_text");

            button_tag tag = (button_tag)btn.Tag;
            if (tag.enter)
            {
                background.Fill = color_mul(background.Fill, 1.25);
                tag.enter = false;
            }
            if (tag.left_clicking)
            {
                background.Fill = color_mul(background.Fill, 2);
                tag.left_clicking = false;
            }
            if (tag.right_clicking)
            {
                background.Fill = color_mul_r(background.Fill, 2);
                tag.right_clicking = false;
            }
        }

        private void btn_enter(object sender, MouseEventArgs e)
        {
            Rectangle btn = (Rectangle)sender;
            Rectangle background = (Rectangle)FindName(btn.Name + "_bg");
            TextBlock text = (TextBlock)FindName(btn.Name + "_text");

            button_tag tag = (button_tag)btn.Tag;
            if (!tag.enter)
            {
                background.Fill = color_mul(background.Fill, 0.8);
                tag.enter = true;
            }
        }

        private SolidColorBrush color_mul(Brush b, double m, bool positive = true)
        {
            if (!positive)
            {
                m = 1 / m;
            }
            SolidColorBrush c = (SolidColorBrush)b;
            return getSCB(Color.FromArgb(
                c.Color.A,
                (byte)(c.Color.R * m),
                (byte)(c.Color.G * m),
                (byte)(c.Color.B * m)
                ));
        }

        private SolidColorBrush color_mul_r(Brush b, double m, bool positive = true)
        {
            if (!positive)
            {
                m = 1 / m;
            }
            SolidColorBrush c = (SolidColorBrush)b;
            return getSCB(Color.FromArgb(
                c.Color.A,
                (byte)(c.Color.R * m),
                c.Color.G,
                c.Color.B
                ));
        }

        public void reg_name(Panel parent, FrameworkElement elem)
        {
            FrameworkElement temp = null;
            foreach (FrameworkElement f in parent.Children)
            {
                if (f.Name == elem.Name)
                {
                    temp = f;
                }
            }
            if (temp != null)
            {
                parent.Children.Remove(temp);
            }
            parent.Children.Add(elem);
            if (FindName(elem.Name) != null)
            {
                UnregisterName(elem.Name);
                RegisterName(elem.Name, elem);
            }
            else
            {
                RegisterName(elem.Name, elem);
            }
        }

        public FrameworkElement find_name(string name)
        {
            return (FrameworkElement)FindName(name);
        }
        public Grid find_grid(string name)
        {
            return (Grid)FindName(name);
        }

        public void visibility_transfer(FrameworkElement f, bool b)
        {
            if (f == null)
            {
                return;
            }
            if (b)
            {
                f.Visibility = Visibility.Visible;
            }
            else
            {
                f.Visibility = Visibility.Hidden;
            }
        }

        public void visual_unlock(string name, bool visible = true)
        {
            FrameworkElement f = (FrameworkElement)FindName(name);
            visibility_transfer(f, visible);
        }

        public Grid custom_button(string namebase, Panel parent,
            double width, double height, string text, double fontsize = 11,
            bool right_btn = false, bool toggle_button = false,
            double radius_mul = 1)
        {
            double radius = Math.Min(width / 2, height / 2) * radius_mul;

            Grid grid = new Grid()
            {
                Name = namebase + "_grid",
            };
            reg_name(parent, grid);

            Rectangle bg = new Rectangle()
            {
                Name = namebase + "_bg",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Stroke = getSCB(Colors.Black),
                Fill = getSCB(Colors.White),
                Width = width,
                Height = height,
                RadiusX = radius,
                RadiusY = radius,
            };
            reg_name(grid, bg);

            //react    0 0 0 0
            //selected 127 0 255 0
            //disabled 127 0 0 0

            Rectangle react = new Rectangle()
            {
                Name = namebase + "_react",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Stroke = getSCB(Colors.Black),
                Fill = C(0, 0, 0, 0),
                Width = width,
                Height = height,
                RadiusX = radius,
                RadiusY = radius,
            };
            if (toggle_button)
            {
                react.Fill = C(127, 0, 255, 0);
                react.Visibility = Visibility.Hidden;
            }
            reg_name(grid, react);

            TextBlock textBlock = new TextBlock()
            {
                Name = namebase + "_text",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                Text = text,
                FontSize = fontsize,
            };
            reg_name(grid, textBlock);

            Rectangle cover = new Rectangle()
            {
                Name = namebase,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Stroke = getSCB(Colors.Black),
                Fill = C(0, 0, 0, 0),
                Width = width,
                Height = height,
                RadiusX = radius,
                RadiusY = radius,
            };
            if (right_btn)
            {
                set_lrbtn(cover);
            }
            else
            {
                set_lbtn(cover);
            }
            reg_name(grid, cover);

            return grid;
        }
        public Grid custom_button_pic(string namebase, Panel parent,
            double width, double height, ImageSource source,
            bool right_btn = false)
        {
            double radius = Math.Min(width / 2, height / 2);

            Grid grid = new Grid()
            {
                Name = namebase + "_grid",
            };
            reg_name(parent, grid);

            Image img = new Image()
            {
                Name = namebase + "_img",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Width = width,
                Height = height,
                Source = source,
            };
            reg_name(grid, img);

            Rectangle react = new Rectangle()
            {
                Name = namebase + "_react",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Stroke = getSCB(Colors.Black),
                Fill = C(127, 255, 255, 0),
                Width = width,
                Height = height,
            };
            reg_name(grid, react);

            Rectangle bg = new Rectangle()
            {
                Name = namebase + "_bg",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Stroke = getSCB(Colors.Black),
                Fill = C(20, 255, 255, 255),
                Width = width,
                Height = height,
            };
            reg_name(grid, bg);

            Rectangle cover = new Rectangle()
            {
                Name = namebase,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Stroke = getSCB(Colors.Black),
                Fill = C(0, 0, 0, 0),
                Width = width,
                Height = height,
            };
            if (right_btn)
            {
                set_lrbtn(cover);
            }
            else
            {
                set_lbtn(cover);
            }
            reg_name(grid, cover);

            return grid;
        }
        public Rectangle get_react(Grid grid)
        {
            string namebase = grid.Name.Substring(0, grid.Name.Length - 5);
            return (Rectangle)FindName(namebase + "_react");
        }
        public Rectangle get_react(string name)
        {
            return (Rectangle)FindName(name + "_react");
        }

        public StackPanel custom_select(string namebase, Panel parent,
            double width, double height, List<string> choices, string text,
            SolidColorBrush text_color,
            double fontsize = 11, int index = 0)
        {
            StackPanel grid = new StackPanel()
            {
                Name = namebase + "_grid",
                Orientation = Orientation.Horizontal,
            };
            reg_name(parent, grid);

            TextBlock t = new TextBlock()
            {
                Name = namebase + "_text",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 0),
                Foreground = text_color,
                FontSize = fontsize,
                Text = text
            };
            reg_name(grid, t);

            Grid container = new Grid
            {
                Name = namebase + "_container",
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                Width = width,
                Height = height,
                Margin = new Thickness(0, 0, 0, 0),
            };
            reg_name(grid, container);

            ComboBox select = new ComboBox()
            {
                Name = namebase + "_select",
                Width = width,
                Height = height,
                Margin = new Thickness(0, 0, 0, 0),
            };
            reg_name(container, select);
            foreach(string choice in choices) 
            {
                var item = new ComboBoxItem();
                item.Content = choice;
                select.Items.Add(item);
            }
            select.SelectedIndex = index;
            select.SelectionChanged += selector_SelectionChanged;

            Rectangle r = new Rectangle()
            {
                Name = namebase + "_blocker",
                VerticalAlignment = VerticalAlignment.Top,
                Width = width,
                Height = height / 4,
                Margin = new Thickness(0, 0, 0, 0),
                Fill = C(0, 0, 0, 0),
            };
            reg_name(container, r);

            r = new Rectangle()
            {
                Name = namebase,
                VerticalAlignment = VerticalAlignment.Top,
                Width = width,
                Height = height,
                Margin = new Thickness(0, 0, 0, 0),
                Fill = C(0, 0, 0, 0),
                IsHitTestVisible = false,
            };
            reg_name(container, r);

            return grid;
        }


        public StackPanel custom_input(string namebase, Panel parent,
            double width, string init, string text, 
            SolidColorBrush text_color,
            double fontsize = 11, bool lines = false, double height = double.NaN)
        {
            StackPanel grid = new StackPanel()
            {
                Name = namebase + "_grid",
                Orientation = Orientation.Horizontal,
            };
            reg_name(parent, grid);

            TextBlock t = new TextBlock()
            {
                Name = namebase + "_text",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Text = text,
                Foreground = text_color,
                FontSize = fontsize,
            };
            reg_name(grid, t);

            TextBox box = new TextBox()
            {
                Name = namebase + "_box",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = fontsize,
                Text = init,
                Width = width,
                Height = height,
                UndoLimit = 0,
                Background = getSCB(Color.FromRgb(0, 255, 0)),
                Foreground = getSCB(Color.FromRgb(0, 0, 0)),
            };
            if(lines)
            {
                box.TextWrapping = TextWrapping.Wrap;
            }
            box.TextChanged += Box_TextChanged;
            reg_name(grid, box);

            return grid;
        }

        public BitmapFrame pic(string realtive_path)
        {
            return BitmapFrame.Create(new Uri(realtive_path, UriKind.Relative));
        }

        public void capture(string realtive_path, FrameworkElement f)
        {
            Point p = f.PointToScreen(new Point(0, 0));
            Point mul = tool.getScale(f);
            double left = p.X;
            double top = p.Y;
            double w = f.ActualWidth * mul.X;
            double h = f.ActualHeight * mul.Y;

            tool.CaptureScreen(realtive_path, left, top, w, h,
                VisualTreeHelper.GetDpi(this));
        }
    }
}
