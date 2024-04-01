using System;
using System.Collections;
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
using static System.Reflection.Metadata.BlobBuilder;
using System.Reflection.Metadata;
using System.Windows.Threading;
using System.Xml.Linq;
using System.Text.Json.Serialization;

namespace BTD6_DIY_Rounds
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            wrap_init();
            hints_init(hints_default);
            attr_init();
            visual_init();
            Mouse.AddMouseWheelHandler(this, new MouseWheelEventHandler(wheel));
            time_start();
            time_show_start();
        }


        private void wheel(object sender, MouseWheelEventArgs e)
        {
            Point p = Mouse.GetPosition(main_grid);
            //Point scale = tool.getScale(main_grid);
            double ratioX = p.X / main_grid.Width;
            double ratioY = p.Y / main_grid.Height;
            if (ratioX <= 0.25)
            {
                if (e.Delta > 0)
                {
                    button_effect("round_grid_change_down");
                }
                if (e.Delta < 0)
                {
                    button_effect("round_grid_change_up");
                }
            }
            else if (ratioY > 0.75)
            {
                int index = control_2;
                ComboBox cb = (ComboBox)FindName("control_panel_select");
                if (e.Delta < 0)
                {
                    index++;
                }
                if (e.Delta > 0)
                {
                    index--;
                    if (index < 0)
                    {
                        index += cb.Items.Count;
                    }
                }
                index = index % cb.Items.Count;
                cb.SelectedIndex = index;
            }
            else
            {
                if (ext_list[0])
                {
                    if (e.Delta > 0)
                    {
                        button_effect("debut_up");
                    }
                    if (e.Delta < 0)
                    {
                        button_effect("debut_down");
                    }
                }
                else
                {
                    if (e.Delta > 0)
                    {
                        button_effect("control_up");
                    }
                    if (e.Delta < 0)
                    {
                        button_effect("control_down");
                    }
                }
            }
        }

        int curr_round = 1;
        public class round
        {
            public int R;
            public List<bloon_entry> bloon_entrys = new List<bloon_entry>();
            public double time_max = 1;

            [Newtonsoft.Json.JsonConstructor]
            public round(int r, List<bloon_entry> bloon_entrys, double time_max) : this(r)
            {
                this.bloon_entrys = bloon_entrys;
                this.time_max = time_max;
            }

            public round(int r)
            {
                this.R = r;
            }

            public void add_entry(bloon_entry e)
            {
                bloon_entrys.Add(e);
                if(e.end > time_max)
                {
                    time_max = e.end;
                }
            }
            public void delete_entry(int index)
            {
                bloon_entrys.RemoveAt(index);
                time_max = bloon_entrys[0].end;
                for(int i = 1; i < bloon_entrys.Count; i++)
                {
                    time_max = Math.Max(time_max, bloon_entrys[i].end);
                }
            }
        }
        round[] rounds = new round[141];
        public class bloon_entry
        {
            public string bloon_name = null;
            public double start = 0;
            public double end = 0;
            public int count = 1;

            public bool regrow = false;
            public bool fort = false;
            public bool camo = false;
            public bool elite = false;

            public bool boss = false;

            public int level = 1;

            public bool lead = false;
            public bool purple = false;
            public bool zebra = false;
            public bloon_entry()
            {
            }

            public bloon_entry(string bloon_name, double start, double end, int count)
            {
                this.bloon_name = bloon_name;
                this.start = start;
                this.end = end;
                this.count = count;
            }

            [Newtonsoft.Json.JsonConstructor]
            public bloon_entry(string bloon_name, double start, double end, int count, 
                bool regrow, bool fort, bool camo, bool elite, bool boss, int level, 
                bool lead, bool purple, bool zebra)
            {
                this.bloon_name = bloon_name;
                this.start = start;
                this.end = end;
                this.count = count;
                this.regrow = regrow;
                this.fort = fort;
                this.camo = camo;
                this.elite = elite;
                this.boss = boss;
                this.level = level;
                this.lead = lead;
                this.purple = purple;
                this.zebra = zebra;
            }
        }
        bloon_entry editing_entry = new bloon_entry();

        private void visual_init()
        {
            Rectangle r;
            TextBlock t;
            Grid g;
            Grid btn;

            for(int i = 0; i < rounds.Length; i++)
            {
                rounds[i] = new round(i);
                rounds[i].add_entry(new bloon_entry("Red", 0, 1000, 1));
            }
            init_bloon_types();
            all_bt = get_all_bt();

            btn = custom_button("wrap_btn", view_grid, 50, 26, "传送！", 11);
            btn.HorizontalAlignment = HorizontalAlignment.Left;
            btn.VerticalAlignment = VerticalAlignment.Center;
            btn.Margin = new Thickness(15, 0, 0, 0);
            ((Rectangle)FindName("wrap_btn_react")).Fill = C(127, 127, 255, 0);

            btn = custom_button("bloons_round", view_grid, 240, 35, "", 22, true,
                radius_mul: 0.5);
            btn.HorizontalAlignment = HorizontalAlignment.Center;
            btn.VerticalAlignment = VerticalAlignment.Center;
            btn.Margin = new Thickness(0, 0, 0, 0);

            btn = custom_button("prev_round", view_grid, 70, 26, "上一回合", 12);
            btn.HorizontalAlignment = HorizontalAlignment.Left;
            btn.VerticalAlignment = VerticalAlignment.Center;
            btn.Margin = new Thickness(80, 0, 0, 0);
            btn = custom_button("next_round", view_grid, 70, 26, "下一回合", 12);
            btn.HorizontalAlignment = HorizontalAlignment.Right;
            btn.VerticalAlignment = VerticalAlignment.Center;
            btn.Margin = new Thickness(0, 0, 80, 0);

            btn = custom_button("show", view_grid, 70, 26, "显示属性", 12);
            btn.HorizontalAlignment = HorizontalAlignment.Right;
            btn.VerticalAlignment = VerticalAlignment.Center;
            btn.Margin = new Thickness(0, 0, 0, 0);
            ((Rectangle)FindName("show_react")).Fill = C(127, 0, 255, 255);
            
            visual_round_grid_init();
            visual_bloons_grid_init();
            visual_control_grid_init();
            visual_debut_grid_init();
            visual_cash_grid_init();
            visual_hint_grid_init();
            visual_sh_init();

            TextBox x = (TextBox)FindName("control_pos_box");
            string filepath = "./Configs/GamePath.txt";
            if (File.Exists(filepath))
            {
                x.Text = File.ReadAllText(filepath);
            }
            else
            {
                x.Text = "C:/";
            }
            
            try
            {
                load_rounds("./Templates/normal/DIY Rounds");
            }
            catch
            {
                MessageBox.Show("./Templates/normal/DIY Rounds/rounds.json 文件丢失，" +
                    "无法加载正常回合",
                    "提示");
            }
            bloon_show();
        }

        private void round_select(int n)
        {
            if (n <= 0 || n > 140)
            {
                return;
            }

            round_unselect();

            int curr_no = (n - 1) - (current_page - 1) * page_i * page_j;
            while(curr_no > page_i * page_j)
            {
                curr_no -= page_i * page_j;
                button_effect("round_grid_change_up");
            }
            while (curr_no < 0)
            {
                curr_no += page_i * page_j;
                button_effect("round_grid_change_down");
            }
            int curr_i = curr_no / page_j;
            int curr_j = curr_no % page_j;
            Rectangle react = (Rectangle)FindName(
                "round_select_grid_" + curr_i + "_" + curr_j);
            if (react != null)
            {
                react.Fill = C(127, 0, 255, 0);
            }

            if (curr_round != n)
            {
                curr_round = n;
            }
            bloon_modifying = int.MinValue / 2;
            bloon_no_base = 1;

            ((TextBlock)FindName("bloons_round_text")).Text = "气球进攻计划 回合" + curr_round;
        }
        private void round_unselect()
        {
            int last_no = (curr_round - 1) - (current_page - 1) * 
                page_i * page_j;
            int last_i = last_no / page_j;
            int last_j = last_no % page_j;
            Rectangle react = (Rectangle)FindName(
                "round_select_grid_" + last_i + "_" + last_j);
            if(react != null)
            {
                react.Fill = C(0, 0, 0, 0);
            }
        }

        private void page_shift(int shift)
        {
            for(int i = 0; i < page_i; i++)
            {
                for(int j = 0; j < page_j; j++)
                {
                    TextBlock t = (TextBlock)FindName(
                        "round_select_grid_" + i + "_" + j + "_text");
                    int n = int.Parse(t.Text);
                    n += page_i * page_j * shift;
                    t.Text = n.ToString();

                    Grid g = (Grid)FindName(
                            "round_select_grid_" + i + "_" + j + "_grid"
                            );
                    visibility_transfer(g, n <= 140);
                }
            }
        }


        double main_w = 0;
        double main_h = 0;
        double main_grid_w = 0;
        double main_grid_h = 0;
        private void sizechanged(object sender, SizeChangedEventArgs e)
        {
            if (e.PreviousSize.Width == 0)
            {
                main_grid_w = main_grid.Width = main_grid.ActualWidth;
                main_grid_h = main_grid.Height = main_grid.ActualHeight;
                main_w = Width;
                main_h = Height;
                main_grid.HorizontalAlignment = HorizontalAlignment.Left;
                main_grid.VerticalAlignment = VerticalAlignment.Top;
                return;
            }
            double mulx = e.NewSize.Width / main_w;
            double muly = e.NewSize.Height / main_h;
            double mul = Math.Min(mulx, muly);
            if (mul < 1)
            {
                Width = main_w;
                Height = main_h;
                /*
                main_grid.Width = main_grid_w * mul;
                main_grid.Height = main_grid_h * mul;*/
            }
            else
            {
                tool.scale(main_grid, mul, mul);
            }
        }
    }
}
