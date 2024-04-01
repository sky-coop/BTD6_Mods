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
        List<bool> ext_list = new List<bool>() 
        { 
            false, //debut
            false, //cash
            false, //hint
            false, //batch
            false, //random
            false,
        }; 
        
        public void wrap_switch()
        {
            wrap_off = !wrap_off;
            if (Title.Contains("BTD6 DIY Rounds Editor"))
            {
                Title = Title.Replace("BTD6 DIY Rounds Editor", "BTD6 DRE");
            }
            else if (Title.Contains("BTD6 DRE"))
            {
                Title = Title.Replace("BTD6 DRE", "BTD6 DIY Rounds Editor");
            }
        }

        private void ext_change(int index)
        {
            if (ext_list[index])
            {
                ext_close();
            }
            else
            {
                ext_open(index);
            }
        }
        private void ext_open(int index)
        {
            ext_close();
            ext_list[index] = true;
        }
        private void ext_close()
        {
            for(int i = 0; i < ext_list.Count; i++)
            {
                ext_list[i] = false;
            }
        }

        bool modifyed = false;

        bool auto_save = false;
        bool save_shot = false;
        bool wrap_off = false;

        Point ball_vector;
        Point ball_acc = new Point(0, 0);
        bool ball_need_random = true;
        private void visual_sh_init()
        {
            Grid g;
            Rectangle r;
            TextBlock t;

            g = new Grid()
            {
                Name = "sh_ball_grid",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Width = 100,
                Height = 100,
                Margin = new Thickness(main_grid.Width / 2 - 50, main_grid.Height / 2 - 50, 0, 0),
            };
            reg_name(background_grid, g);

            r = new Rectangle()
            {
                Name = "sh_ball",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 100,
                Height = 100,
                RadiusX = 100,
                RadiusY = 100,
                Fill = C(60, 0, 255, 255),
                Stroke = C(60, 0, 255, 0),
            };
            reg_name(g, r);

            t = new TextBlock()
            {
                Name = "sh_text",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 30,
                Text = "截图",
                FontFamily = new FontFamily("SimHei"),
                Foreground = C(90, 0, 0, 0),
            };
            reg_name(g, t);


            g = new Grid()
            {
                Name = "sh_control_grid",
                Background = C(90, 180, 180),
                Visibility = Visibility.Hidden,
            };
            Grid.SetRowSpan(g, 10);
            reg_name(control_grid, g);

            double ratio = 0.5;
            Grid container = new Grid()
            {
                Name = "sh_hint_preview_grid",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 923 * ratio,
                Height = 151 * ratio,
            };
            reg_name(g, container);
        }



        int save_round = -1;
        private void print_screen(string save_name)
        {
            if (!save_shot)
            {
                return;
            }

            string folder = "./Workspace/" + save_name + "/pics";
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            else
            {
                foreach(var f in Directory.GetFiles(folder))
                {
                    File.Delete(f);
                }
            }

            ext_close();
            save_round = curr_round;
            /*
            while (current_page > 1)
            {
                button_effect("round_grid_change_down");
            }
            button_effect("round_select_grid_0_0");*/

            screenshooting = true;
            screenshooting_name = save_name;
            screenshooting_level = 1;
            screenshooting_end = 140;
            screenshooting_page = 1;
            if (get_start_round() != 0)
            {
                screenshooting_level = get_start_round();
            }
            if (get_end_round() != 150)
            {
                screenshooting_end = get_end_round();
            }
            round_select(screenshooting_level);

            background_grid.Visibility = Visibility.Hidden;

            G("sh_control_grid").Visibility = Visibility.Visible;
            G("ext_hint_container_grid").Children.Clear();
            G("sh_hint_preview_grid").Children.Add(G("ext_hint_preview_grid"));
            screen_hint_change();

            InvalidateVisual();
            UpdateLayout();
        }

        private void screen_hint_change()
        {
            curr_hint = screenshooting_level;
            TextBlock hint_text = T("ext_hint_text");
            if (hints.ContainsKey(curr_hint))
            {
                R("ext_hint_preview_bg").Visibility = Visibility.Visible;
                hint_text.HorizontalAlignment = HorizontalAlignment.Left;

                var tuple = hints[curr_hint];
                double base_font_size = (double)hint_text.Tag;

                hint_text.Text = tuple.Item2;
                hint_text.FontSize = base_font_size * cal_font_mul(hint_text);
                hint_text.LineHeight = hint_text.FontSize * 1.4;
                hint_text.Foreground = tuple.Item1.toBrush();
            }
            else
            {
                R("ext_hint_preview_bg").Visibility = Visibility.Hidden;
                hint_text.HorizontalAlignment = HorizontalAlignment.Center;
                hint_text.FontSize = 18;
                hint_text.Text = "第 " + curr_hint + " 回合没有结束提示";
                hint_text.Foreground = C(255, 255, 0);
            }
        }
    }
}
