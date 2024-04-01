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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BTD6_DIY_Rounds
{
    public partial class MainWindow : Window
    {
        int current_page;
        int page_max;
        int page_i = 10;
        int page_j = 5;
        private void visual_round_grid_init(int round_i = 10, int round_j = 5)
        {
            Rectangle r;
            TextBlock t;
            Grid g;

            round_select_grid.Children.Clear();
            round_select_grid.RowDefinitions.Clear();
            round_select_grid.ColumnDefinitions.Clear();

            double size_mul = Math.Min(5.0 / round_j, 10.0 / round_i);

            for (int i = 0; i < round_i; i++)
            {
                round_select_grid.RowDefinitions.Add(new RowDefinition());
            }
            for (int i = 0; i < round_j; i++)
            {
                round_select_grid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (int i = 0; i < round_i; i++)
            {
                for (int j = 0; j < round_j; j++)
                {
                    string namebase = round_select_grid.Name + "_" + i + "_" + j;
                    g = custom_button(namebase, round_select_grid,
                        180.0 / round_j, 300.0 / round_i,
                        (i * round_j + j + 1).ToString(), 15.0 * size_mul, true);
                    Grid.SetRow(g, i);
                    Grid.SetColumn(g, j);
                }
            }

            g = new Grid()
            {
                Name = "round_grid_change",
            };
            reg_name(round_grid, g);
            Grid.SetRow(g, 1);

            current_page = 1;
            page_i = round_i;
            page_j = round_j;
            page_max = (int)Math.Ceiling(140.0 / (page_i * page_j));
            t = new TextBlock()
            {
                Name = "round_grid_change_text",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 13,
                Foreground = C(255, 255, 255),
                Text = "第 1 / " + page_max + " 页",
                Margin = new Thickness(0, 0, 0, 3)
            };
            reg_name(g, t);
            Grid btn;
            btn = custom_button("round_grid_change_down", g, 40, 25, "←", 14);
            btn.HorizontalAlignment = HorizontalAlignment.Left;
            btn.VerticalAlignment = VerticalAlignment.Center;
            btn.Margin = new Thickness(10, 0, 0, 0);
            btn = custom_button("round_grid_change_up", g, 40, 25, "→", 14);
            btn.HorizontalAlignment = HorizontalAlignment.Right;
            btn.VerticalAlignment = VerticalAlignment.Center;
            btn.Margin = new Thickness(0, 0, 10, 0);

            round_select(curr_round);
            GC.Collect();
        }

        int bloon_no_base = 1;
        int bloon_row;
        int bloon_modifying = -1;
        bool stats_show = true;
        private void visual_bloons_grid_init(int row = 5)
        {
            Rectangle r;
            TextBlock t;
            Grid g;

            bloon_row = row;

            bloons_grid.Children.Clear();
            bloons_grid.RowDefinitions.Clear();
            for (int i = 0; i < row; i++)
            {
                bloons_grid.RowDefinitions.Add(new RowDefinition());
            }

            for (int i = 0; i < row; i++)
            {
                g = new Grid()
                {
                    Name = "bloons_grid_" + i,
                };
                reg_name(bloons_grid, g);
                Grid.SetRow(g, i);

                t = new TextBlock()
                {
                    Name = "bloons_no_" + (i + 1),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    FontSize = 60.0 / row,
                    Text = "No." + (i + 1),
                    Foreground = C(255, 255, 255),
                    Margin = new Thickness(5, 10.0 / row, 0, 0),
                };
                reg_name(g, t);

                t = new TextBlock()
                {
                    Name = "bloons_name_" + i,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    FontSize = 48.0 / row,
                    Text = "GoldenLeadPurpleZebraFortifiedCamo",
                    Foreground = C(255,
                                   255,
                                  (byte)(255 - 255.0 * i / row)),
                    Margin = new Thickness(5 + 280.0 / row, 16.0 / row, 0, 0),
                };
                reg_name(g, t);

                Grid center_grid = new Grid()
                {
                    Name = "bloons_center_grid_" + i,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Top,
                    Width = 460,
                    Height = 150.0 / row,
                    ClipToBounds = false,
                    Margin = new Thickness(0, 100.0 / row, 0, 0),
                };
                ColumnDefinition cd = new ColumnDefinition();
                cd.Width = new GridLength(450.0 / row, GridUnitType.Pixel);
                center_grid.ColumnDefinitions.Add(cd);

                cd = new ColumnDefinition();
                cd.Width = new GridLength(20, GridUnitType.Pixel);
                center_grid.ColumnDefinitions.Add(cd);

                cd = new ColumnDefinition();
                cd.Width = new GridLength(0, GridUnitType.Star);
                center_grid.ColumnDefinitions.Add(cd);

                center_grid.ColumnDefinitions.Add(new ColumnDefinition());

                cd = new ColumnDefinition();
                cd.Width = new GridLength(0, GridUnitType.Star);
                center_grid.ColumnDefinitions.Add(cd);
                reg_name(g, center_grid);
                #region img_grid
                Grid img_grid = new Grid()
                {
                    Name = "bloons_img_grid_" + i,
                    Width = 450.0 / row,
                    Height = 150.0 / row,
                };
                reg_name(center_grid, img_grid);

                Grid btn;
                btn = custom_button_pic("bloons_img_" + i, img_grid,
                    150.0 / row, 150.0 / row, pic("Bloons Pic/No Icon.png"));
                btn.HorizontalAlignment = HorizontalAlignment.Left;
                btn.VerticalAlignment = VerticalAlignment.Top;
                ((Rectangle)FindName("bloons_img_" + i + "_react")).Visibility = Visibility.Hidden;
                for (int pi = 0; pi < 2; pi++)
                {
                    for (int pj = 0; pj < 4; pj++)
                    {
                        string s = "";
                        switch(pi * 4 + pj)
                        {
                            case 0:
                                s = "Bloons Pic/Camo.png";
                                break;
                            case 1:
                                s = "Bloons Pic/Regrow.png";
                                break;
                            case 2:
                                s = "Bloons Pic/Fortified.png";
                                break;
                            case 3:
                                s = "Bloons Pic/Elite.png";
                                break;
                            case 4:
                                s = "Bloons Pic/Lead.png";
                                break;
                            case 5:
                                s = "Bloons Pic/Purple.png";
                                break;
                            case 6:
                                s = "Bloons Pic/Zebra.png";
                                break;
                            case 7:
                                s = "Bloons Pic/1.png";
                                break;
                        }
                        btn = custom_button_pic(
                            "bloons_img_stat_" + i + "_" + pi + "_" + pj,
                            img_grid, 75.0 / row, 75.0 / row, pic(s));
                        btn.HorizontalAlignment = HorizontalAlignment.Left;
                        btn.VerticalAlignment = VerticalAlignment.Top;
                        btn.Margin = new Thickness((150.0 + pj * 75.0) / row,
                            (pi * 75.0) / row, 0, 0);
                    }
                }
                #endregion img_grid

                r = new Rectangle()
                {
                    Name = "bloons_bar_" + i,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Height = 75.0 / row,
                    RadiusX = 25.0 / row,
                    RadiusY = 25.0 / row,
                    Fill = C(127, 255, 0),
                    Margin = new Thickness(0, 0, 0, 0),
                };
                Grid.SetColumn(r, 3);
                reg_name(center_grid, r);

                t = new TextBlock()
                {
                    Name = "bloons_stext_" + i,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    FontSize = 8,
                    Text = "Start",
                    Foreground = C(255,
                                   255,
                                  (byte)(255 - 255.0 * i / row)),
                    ClipToBounds = false,
                    Margin = new Thickness(0, 0, 0, 75.0 / row + 1),
                };
                Grid.SetColumn(t, 3);
                reg_name(center_grid, t);

                t = new TextBlock()
                {
                    Name = "bloons_etext_" + i,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    FontSize = 8,
                    Text = "end",
                    Foreground = C(255,
                                   255,
                                  (byte)(255 - 255.0 * i / row)),
                    ClipToBounds = false,
                    Margin = new Thickness(0, 0, 0, 75.0 / row + 1),
                };
                Grid.SetColumn(t, 3);
                reg_name(center_grid, t);

                #region data
                Grid data = new Grid()
                {
                    Name = "bloons_data_grid_" + i,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Background = C(75, 255, 255, 255),
                    Height = 75.0 / row,
                };
                Grid.SetColumn(data, 2);
                Grid.SetColumnSpan(data, 10);
                reg_name(center_grid, data);

                for(int k = 0; k < 5; k++)
                {
                    data.ColumnDefinitions.Add(new ColumnDefinition());
                }

                for(int k = 0; k < 5; k++)
                {
                    t = new TextBlock()
                    {
                        Name = "bloons_data_text_"  + i + "_" + k,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center,
                        FontSize = 7.5,
                        Text = "扣血:1233330",
                        Foreground = C(0, 0, 0),
                    };
                    Grid.SetColumn(t, k);
                    reg_name(data, t);
                }
                #endregion data

                t = new TextBlock()
                {
                    Name = "bloons_ctext_" + i,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    FontSize = 45.0 / row + 3,
                    Text = "×1000",
                    Foreground = C(255, 255, 255),
                    Margin = new Thickness(525, 0, 0, 15.0 / row),
                };
                reg_name(g, t);

                btn = custom_button("bloons_copy_" + i, g,
                    35, 100.0 / row, "复制", 20.0 / row + 5, true);
                btn.HorizontalAlignment = HorizontalAlignment.Left;
                btn.VerticalAlignment = VerticalAlignment.Top;
                btn.Margin = new Thickness(415, 5.0 / row, 0, 0);

                btn = custom_button("bloons_exchange_up_" + i, g,
                    35, 100.0 / row, "↑", 20.0 / row + 5);
                btn.HorizontalAlignment = HorizontalAlignment.Left;
                btn.VerticalAlignment = VerticalAlignment.Top;
                btn.Margin = new Thickness(460, 5.0 / row, 0, 0);

                btn = custom_button("bloons_exchange_down_" + i, g,
                    35, 100.0 / row, "↓", 20.0 / row + 5);
                btn.HorizontalAlignment = HorizontalAlignment.Left;
                btn.VerticalAlignment = VerticalAlignment.Top;
                btn.Margin = new Thickness(505, 5.0 / row, 0, 0);

                btn = custom_button("bloons_delete_" + i, g,
                    35, 100.0 / row, "删除", 20.0 / row + 5);
                btn.HorizontalAlignment = HorizontalAlignment.Left;
                btn.VerticalAlignment = VerticalAlignment.Top;
                btn.Margin = new Thickness(550, 5.0 / row, 0, 0);
            }
        }
        private void visual_control_grid_init()
        {
            Rectangle r;
            TextBlock t;
            Grid g;
            Grid btn;

            for (int i = 0; i < 3; i++)
            {
                control_grid.RowDefinitions.Add(new RowDefinition());
            }
            control_grid.RowDefinitions[1].Height = new GridLength(0.6, GridUnitType.Star);
            control_grid.RowDefinitions[2].Height = new GridLength(0.9, GridUnitType.Star);

            #region control 0
            btn = custom_button("control_add", control_grid, 75, 30, "添加一行");
            btn.HorizontalAlignment = HorizontalAlignment.Left;
            btn.VerticalAlignment = VerticalAlignment.Center;
            btn.Margin = new Thickness(5, 0, 0, 0);

            #region control_select_grid
            g = new Grid()
            {
                Name = "control_select_grid",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Stretch,
                Width = 165, 
                Margin = new Thickness(85, 0, 0, 0)
            };
            g.RowDefinitions.Add(new RowDefinition());
            g.RowDefinitions.Add(new RowDefinition());
            reg_name(control_grid, g);

            #region group
            t = new TextBlock()
            {
                Name = "control_group_text",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 0),
                Foreground = C(255, 255, 127),
                FontSize = 14,
                Text = "模组："
            };
            reg_name(g, t);

            ComboBox group_selector = new ComboBox()
            {
                Name = "control_group_cb_select",
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 125,
                Height = 20,
                Margin = new Thickness(0, 0, 0, 0),
            };
            foreach(KeyValuePair<string, List<bloon_template>> pair
                in bloon_types)
            {
                var item = new ComboBoxItem();
                item.Content = pair.Key;
                group_selector.Items.Add(item);
            }
            group_selector.SelectedIndex = 0;
            reg_name(g, group_selector);

            r = new Rectangle()
            {
                Name = "control_group_blocker",
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                Width = 125,
                Height = 20 / 4,
                Margin = new Thickness(0, 0, 0, 0),
                Fill = C(0, 0, 0, 0),
            };
            reg_name(g, r);
            #endregion group
            #region bloon
            t = new TextBlock()
            {
                Name = "control_bloon_text",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 0),
                Foreground = C(127, 255, 127),
                FontSize = 14,
                Text = "气球："
            };
            Grid.SetRow(t, 1);
            reg_name(g, t);

            ComboBox bloon_selector = new ComboBox()
            {
                Name = "control_bloon_cb_select",
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 125,
                Height = 20,
                Margin = new Thickness(0, 0, 0, 0),
            };
            Grid.SetRow(bloon_selector, 1);
            reg_name(g, bloon_selector);
            bloon_selector_init();
            bloon_selector.SelectionChanged += selector_SelectionChanged;

            r = new Rectangle()
            {
                Name = "control_bloon_blocker",
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                Width = 125,
                Height = 20 / 4,
                Margin = new Thickness(0, 0, 0, 0),
                Fill = C(0, 0, 0, 0),
            };
            Grid.SetRow(r, 1);
            reg_name(g, r);
            #endregion bloon
            #endregion control_select_grid

            #region control_input
            g = new Grid()
            {
                Name = "control_input_grid",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Stretch,
                Width = 330,
                Margin = new Thickness(260, 0, 0, 0)
            };
            g.RowDefinitions.Add(new RowDefinition());
            g.RowDefinitions.Add(new RowDefinition());
            g.ColumnDefinitions.Add(new ColumnDefinition());
            g.ColumnDefinitions.Add(new ColumnDefinition());
            g.ColumnDefinitions.Add(new ColumnDefinition());
            reg_name(control_grid, g);

            StackPanel box;

            box = custom_input("control_input_start", g, 60, "0", "开始(ms):",
                C(255, 255, 255));

            box = custom_input("control_input_end", g, 60, "0", "结束(ms):",
                C(255, 255, 255));
            Grid.SetColumn(box, 0);
            Grid.SetRow(box, 1);

            box = custom_input("control_input_interval", g, 60, "0", "间隔(ms):",
                C(255, 255, 255));
            Grid.SetColumn(box, 1);
            Grid.SetRow(box, 1);

            box = custom_input("control_input_count", g, 70, "1", "数量:",
                C(255, 255, 255));
            Grid.SetColumn(box, 2);
            Grid.SetRow(box, 1);

            t = new TextBlock()
            {
                Name = "control_locker_text",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Text = "修改优先级:",
                Foreground = C(255, 255, 100),
                Margin = new Thickness(0, 0, 0, 0),
            };
            Grid.SetColumn(t, 1);
            Grid.SetColumnSpan(t, 2);
            reg_name(g, t);

            ComboBox locker = new ComboBox()
            {
                Name = "control_locker_select",
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 135,
                Height = 20,
                Margin = new Thickness(0, 0, 20, 0),
            };
            string[] strings =
            {
                "结束时间>间隔时间",
                "结束时间>数量",
                "间隔时间>结束时间",
                "间隔时间>数量",
                "数量>结束时间",
                "数量>间隔时间"
            };
            foreach (string s in strings)
            {
                var item = new ComboBoxItem();
                item.Content = s;
                locker.Items.Add(item);
            }
            locker.SelectedIndex = 0;
            Grid.SetColumn(locker, 1);
            Grid.SetColumnSpan(locker, 2);
            reg_name(g, locker);

            r = new Rectangle()
            {
                Name = "control_locker_blocker",
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                Width = 135,
                Height = 20 / 4,
                Margin = new Thickness(0, 0, 20, 0),
                Fill = C(0, 0, 0, 0),
            };
            Grid.SetColumn(r, 1);
            Grid.SetColumnSpan(r, 2);
            reg_name(g, r);
            #endregion control_input
            #endregion control 0
            #region control 1
            g = new Grid()
            {
                Name = "control_1_grid",
            };
            Grid.SetRow(g, 1);
            for (int i = 0; i < 9; i++)
            {
                g.ColumnDefinitions.Add(new ColumnDefinition());
            }
            reg_name(control_grid, g);

            btn = custom_button("control_camo", g, 55, 22, "迷彩");
            btn.HorizontalAlignment = HorizontalAlignment.Center;
            btn.VerticalAlignment = VerticalAlignment.Center;
            btn.Margin = new Thickness(0, 0, 0, 0);
            Grid.SetColumn(btn, 0);

            btn = custom_button("control_regrow", g, 55, 22, "重生");
            btn.HorizontalAlignment = HorizontalAlignment.Center;
            btn.VerticalAlignment = VerticalAlignment.Center;
            btn.Margin = new Thickness(0, 0, 0, 0);
            Grid.SetColumn(btn, 1);

            btn = custom_button("control_fort", g, 55, 22, "加固");
            btn.HorizontalAlignment = HorizontalAlignment.Center;
            btn.VerticalAlignment = VerticalAlignment.Center;
            btn.Margin = new Thickness(0, 0, 0, 0);
            Grid.SetColumn(btn, 2);

            btn = custom_button("control_lead", g, 55, 22, "铅(金气球)", 10);
            btn.HorizontalAlignment = HorizontalAlignment.Center;
            btn.VerticalAlignment = VerticalAlignment.Center;
            btn.Margin = new Thickness(0, 0, 0, 0);
            Grid.SetColumn(btn, 3);

            btn = custom_button("control_purple", g, 55, 22, "紫(金气球)", 10);
            btn.HorizontalAlignment = HorizontalAlignment.Center;
            btn.VerticalAlignment = VerticalAlignment.Center;
            btn.Margin = new Thickness(0, 0, 0, 0);
            Grid.SetColumn(btn, 4);

            btn = custom_button("control_zebra", g, 55, 22, "斑马纹(金)", 10);
            btn.HorizontalAlignment = HorizontalAlignment.Center;
            btn.VerticalAlignment = VerticalAlignment.Center;
            btn.Margin = new Thickness(0, 0, 0, 0);
            Grid.SetColumn(btn, 5);

            btn = custom_button("control_elite", g, 55, 22, "精英");
            btn.HorizontalAlignment = HorizontalAlignment.Center;
            btn.VerticalAlignment = VerticalAlignment.Center;
            btn.Margin = new Thickness(0, 0, 0, 0);
            Grid.SetColumn(btn, 6);

            Panel select = custom_select("control_panel", g, 90, 20,
                new List<string> 
                { 
                    "文件存取", "界面布局", "气球统计", "批量操作", "属性设置", "随机生成", "设置"
                }, "面板：", C(0, 255, 255), 12);
            Grid.SetColumn(select, 7);
            Grid.SetColumnSpan(select, 2);
            #endregion control 1
            #region control 2 0
            g = new Grid()
            {
                Name = "control_2_0_grid",
                Background = C(63, 0, 255, 255),
            };
            Grid.SetRow(g, 2);
            reg_name(control_grid, g);

            box = custom_input("control_template", g, 100, "normal", "导入模板: ",
                C(255, 255, 255));
            box.Margin = new Thickness(10, 0, 0, 18);

            btn = custom_button("control_load_template", g, 40, 16, "导入");
            btn.HorizontalAlignment = HorizontalAlignment.Left;
            btn.VerticalAlignment = VerticalAlignment.Center;
            btn.Margin = new Thickness(165, 0, 0, 18);

            t = new TextBlock()
            {
                Name = "control_template_hint",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Text = "初始模板有normal和ABR",
                Foreground = C(255, 255, 0),
                Margin = new Thickness(10, 18, 0, 0),
            };

            btn = custom_button("control_explore", g, 40, 16, "导航");
            btn.HorizontalAlignment = HorizontalAlignment.Left;
            btn.VerticalAlignment = VerticalAlignment.Center;
            btn.Margin = new Thickness(165, 18, 0, 0);


            Grid sl = new Grid()
            {
                Name = "control_sl",
                HorizontalAlignment = HorizontalAlignment.Left,
                Width = 180,
                Margin = new Thickness(210, 0, 0, 0),
            };
            reg_name(g, sl);
            sl.RowDefinitions.Add(new RowDefinition());
            sl.RowDefinitions.Add(new RowDefinition());
            #region save
            box = custom_input("control_save", sl, 100, "MyRound", "保存为: ",
                C(255, 255, 255));
            box.Margin = new Thickness(0, 0, 0, 0);

            btn = custom_button("control_save_workspace", sl, 40, 15, "保存");
            btn.HorizontalAlignment = HorizontalAlignment.Left;
            btn.VerticalAlignment = VerticalAlignment.Center;
            btn.Margin = new Thickness(140, 0, 0, 0);
            reg_name(g, t);
            #endregion save 
            #region load
            box = custom_input("control_load", sl, 100, "MyRound", "读取名: ",
                C(255, 255, 255));
            box.Margin = new Thickness(0, 0, 0, 0);
            Grid.SetRow(box, 1);

            btn = custom_button("control_load_workspace", sl, 40, 15, "读取");
            btn.HorizontalAlignment = HorizontalAlignment.Left;
            btn.VerticalAlignment = VerticalAlignment.Center;
            btn.Margin = new Thickness(140, 0, 0, 0);
            Grid.SetRow(btn, 1);
            reg_name(g, t);
            #endregion load

            /*
            btn = custom_button("control_reset", g, 80, 20, "重置为红气球");
            btn.HorizontalAlignment = HorizontalAlignment.Left;
            btn.VerticalAlignment = VerticalAlignment.Center;
            btn.Margin = new Thickness(400, 0, 0, 0);*/

            box = custom_input("control_pos", g, 120, "", "",
                C(255, 255, 255), 8, true, 33);
            box.Margin = new Thickness(400, 0, 0, 0);


            btn = custom_button("control_search", g, 60, 15, "定位游戏", 10);
            btn.HorizontalAlignment = HorizontalAlignment.Right;
            btn.VerticalAlignment = VerticalAlignment.Top;
            btn.Margin = new Thickness(0, 2, 5, 0);

            btn = custom_button("control_save_game", g, 29, 15, "存储", 10);
            btn.HorizontalAlignment = HorizontalAlignment.Right;
            btn.VerticalAlignment = VerticalAlignment.Top;
            btn.Margin = new Thickness(0, 18, 36, 0);

            btn = custom_button("control_load_game", g, 29, 15, "读取", 10);
            btn.HorizontalAlignment = HorizontalAlignment.Right;
            btn.VerticalAlignment = VerticalAlignment.Top;
            btn.Margin = new Thickness(0, 18, 5, 0);

            #endregion control 2 0
            #region control 2 1 layout
            g = new Grid()
            {
                Name = "control_2_1_grid",
                Background = C(63, 255, 255, 0),
                Visibility = Visibility.Hidden,
            };
            Grid.SetRow(g, 2);
            for(int i = 0; i < 8; i++)
            {
                g.ColumnDefinitions.Add(new ColumnDefinition());
            }
            reg_name(control_grid, g);

            btn = custom_button("control_up", g, 60, 25, "气球行↑");
            btn.HorizontalAlignment = HorizontalAlignment.Center;
            btn.VerticalAlignment = VerticalAlignment.Center;
            btn.Margin = new Thickness(0, 0, 0, 0);
            Grid.SetColumn(btn, 0);

            btn = custom_button("control_down", g, 60, 25, "气球行↓");
            btn.HorizontalAlignment = HorizontalAlignment.Center;
            btn.VerticalAlignment = VerticalAlignment.Center;
            btn.Margin = new Thickness(0, 0, 0, 0);
            Grid.SetColumn(btn, 1);

            btn = custom_button("control_lineadd", g, 60, 25, "气球行+");
            btn.HorizontalAlignment = HorizontalAlignment.Center;
            btn.VerticalAlignment = VerticalAlignment.Center;
            btn.Margin = new Thickness(0, 0, 0, 0);
            Grid.SetColumn(btn, 2);

            btn = custom_button("control_linesub", g, 60, 25, "气球行-");
            btn.HorizontalAlignment = HorizontalAlignment.Center;
            btn.VerticalAlignment = VerticalAlignment.Center;
            btn.Margin = new Thickness(0, 0, 0, 0);
            Grid.SetColumn(btn, 3);

            btn = custom_button("control_pageiadd", g, 60, 25, "关卡行+");
            btn.HorizontalAlignment = HorizontalAlignment.Center;
            btn.VerticalAlignment = VerticalAlignment.Center;
            btn.Margin = new Thickness(0, 0, 0, 0);
            Grid.SetColumn(btn, 4);

            btn = custom_button("control_pageisub", g, 60, 25, "关卡行-");
            btn.HorizontalAlignment = HorizontalAlignment.Center;
            btn.VerticalAlignment = VerticalAlignment.Center;
            btn.Margin = new Thickness(0, 0, 0, 0);
            Grid.SetColumn(btn, 5);

            btn = custom_button("control_pagejadd", g, 60, 25, "关卡列+");
            btn.HorizontalAlignment = HorizontalAlignment.Center;
            btn.VerticalAlignment = VerticalAlignment.Center;
            btn.Margin = new Thickness(0, 0, 0, 0);
            Grid.SetColumn(btn, 6);

            btn = custom_button("control_pagejsub", g, 60, 25, "关卡列-");
            btn.HorizontalAlignment = HorizontalAlignment.Center;
            btn.VerticalAlignment = VerticalAlignment.Center;
            btn.Margin = new Thickness(0, 0, 0, 0);
            Grid.SetColumn(btn, 7);
            #endregion control 2 1
            #region control 2 2 stats
            g = new Grid()
            {
                Name = "control_2_2_grid",
                Background = C(63, 255, 255, 255),
                Visibility = Visibility.Hidden,
            };
            for (int i = 0; i < 2; i++)
            {
                g.RowDefinitions.Add(new RowDefinition());
            }
            for (int i = 0; i < 6; i++)
            {
                g.ColumnDefinitions.Add(new ColumnDefinition());
            }
            g.ColumnDefinitions[0].Width = new GridLength(0.5, GridUnitType.Star);
            Grid.SetRow(g, 2);
            reg_name(control_grid, g);

            for(int i = 0; i < 2; i++)
            {
                for(int j = 0; j < 4; j++)
                {
                    t = new TextBlock()
                    {
                        Name = "control_stat_text_" + i + "_" + j,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center,
                        Foreground = C(255, 255, 180),
                        Text = "RBE: 123456789",
                    };
                    Grid.SetRow(t, i);
                    Grid.SetColumn(t, j);
                    reg_name(g, t);

                    switch (i * 4 + j)
                    {
                        case 0:
                            t.Text = "当前回合：";
                            t.Foreground = C(100, 255, 255);
                            t.HorizontalAlignment = HorizontalAlignment.Center;
                            break;
                        case 4:
                            t.Text = "之前回合：";
                            t.Foreground = C(100, 255, 255);
                            t.HorizontalAlignment = HorizontalAlignment.Center;
                            break;
                    }
                }
            }

            box = custom_input("control_cash", g, 70, "650", "初始金钱: ",
                C(255, 255, 255));
            Grid.SetRow(box, 0);
            Grid.SetColumn(box, 4);

            box = custom_input("control_round", g, 70, "1", "开始关卡: ",
                C(255, 255, 255));
            Grid.SetRow(box, 1);
            Grid.SetColumn(box, 4);

            btn = custom_button("control_debut", g, 70, 18, "登场回合");
            Grid.SetRow(btn, 0);
            Grid.SetColumn(btn, 5);
            #endregion control 2 2
            #region control 2 3 batch
            g = new Grid()
            {
                Name = "control_2_3_grid",
                Background = C(63, 255, 255, 255),
                Visibility = Visibility.Hidden,
            };
            Grid.SetRow(g, 2);
            for(int i = 0; i < 2; i++)
            {
                g.RowDefinitions.Add(new RowDefinition());
            }
            reg_name(control_grid, g);

            #region load
            Grid load = new Grid()
            {
                Name = "loading_grid",
                Background = C(0, 0, 0, 0),
            };
            Grid.SetRowSpan(load, 100);
            Grid.SetColumnSpan(load, 100);
            reg_name(load_grid, load);

            r = new Rectangle()
            {
                Name = "loading_mask1",
                HorizontalAlignment = HorizontalAlignment.Left,
                Fill = C(127, 0, 0, 0),
                Width = 200,
            };
            reg_name(load, r);

            Grid loading_control = new Grid()
            {
                Name = "loading_control_grid",
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom,
                Width = control_grid.ActualWidth,
                Height = control_grid.ActualHeight,
            };
            reg_name(load, loading_control);

            r = new Rectangle()
            {
                Name = "loading_mask2",
                Fill = C(127, 0, 0, 0),
            };
            reg_name(loading_control, r);

            t = new TextBlock()
            {
                Name = "loading_hint",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = C(255, 255, 255),
                Background = C(90, 143, 212),
                FontSize = 10,
                MaxWidth = 480,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(10, 0, 0, 0),
            };
            reg_name(loading_control, t);

            btn = custom_button("loading_cancel", loading_control, 80, 25, "取消", 14);
            btn.HorizontalAlignment = HorizontalAlignment.Right;
            btn.VerticalAlignment = VerticalAlignment.Top;
            btn.Margin = new Thickness(0, 20, 10, 0);

            btn = custom_button("loading_confirm", loading_control, 80, 25, "确认", 14);
            btn.HorizontalAlignment = HorizontalAlignment.Right;
            btn.VerticalAlignment = VerticalAlignment.Bottom;
            btn.Margin = new Thickness(0, 0, 10, 20);
            #endregion load


            btn = custom_button("control_redbloon", g, 60, 16, "红气球", 10, true);
            btn.HorizontalAlignment = HorizontalAlignment.Left;
            btn.VerticalAlignment = VerticalAlignment.Center;
            btn.Margin = new Thickness(10, 0, 0, 0);

            btn = custom_button("control_only", g, 60, 16, "唯一", 10, true);
            btn.HorizontalAlignment = HorizontalAlignment.Left;
            btn.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetRow(btn, 1);
            btn.Margin = new Thickness(10, 0, 0, 0);

            btn = custom_button("control_copyround", g, 60, 16, "复制", 10, true);
            btn.HorizontalAlignment = HorizontalAlignment.Left;
            btn.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetRow(btn, 0);
            btn.Margin = new Thickness(80, 0, 0, 0);

            btn = custom_button("control_pasteround", g, 60, 16, "粘贴", 10, true);
            btn.HorizontalAlignment = HorizontalAlignment.Left;
            btn.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetRow(btn, 1);
            btn.Margin = new Thickness(80, 0, 0, 0);

            r = new Rectangle()
            {
                Name = "control_batch_bg1",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Stretch,
                Width = 60,
                Fill = C(50, 255, 127, 0),
                Margin = new Thickness(150, 0, 0, 0),
            };
            Grid.SetRowSpan(r, 2);
            reg_name(g, r);

            StackPanel input;

            input = custom_input("control_batch_switch_input", g, 25, "1", "目标：",
                C(255, 255, 0));
            input.HorizontalAlignment = HorizontalAlignment.Left;
            input.VerticalAlignment = VerticalAlignment.Center;
            input.Margin = new Thickness(150, 0, 0, 0);

            btn = custom_button("control_batch_switch", g, 60, 16, "交换", 10, true);
            btn.HorizontalAlignment = HorizontalAlignment.Left;
            btn.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetRow(btn, 1);
            btn.Margin = new Thickness(150, 0, 0, 0);


            r = new Rectangle()
            {
                Name = "control_batch_bg2",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Stretch,
                Width = 130,
                Fill = C(50, 127, 240, 0),
                Margin = new Thickness(220, 0, 0, 0),
            };
            Grid.SetRowSpan(r, 2);
            reg_name(g, r);

            input = custom_input("control_batch_count_input", g, 75, "1", "气球倍率：",
                C(127, 255, 0));
            input.HorizontalAlignment = HorizontalAlignment.Left;
            input.VerticalAlignment = VerticalAlignment.Center;
            input.Margin = new Thickness(220, 0, 0, 0);

            btn = custom_button("control_batch_count_span", g, 60, 16, "÷间距", 10, true);
            btn.HorizontalAlignment = HorizontalAlignment.Left;
            btn.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetRow(btn, 1);
            btn.Margin = new Thickness(220, 0, 0, 0);

            btn = custom_button("control_batch_count_time", g, 60, 16, "×时长", 10, true);
            btn.HorizontalAlignment = HorizontalAlignment.Left;
            btn.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetRow(btn, 1);
            btn.Margin = new Thickness(290, 0, 0, 0);

            r = new Rectangle()
            {
                Name = "control_batch_bg3",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Stretch,
                Width = 130,
                Fill = C(50, 0, 240, 127),
                Margin = new Thickness(360, 0, 0, 0),
            };
            Grid.SetRowSpan(r, 2);
            reg_name(g, r);

            input = custom_input("control_batch_span_input", g, 75, "1", "间距倍率：",
                C(127, 255, 0));
            input.HorizontalAlignment = HorizontalAlignment.Left;
            input.VerticalAlignment = VerticalAlignment.Center;
            input.Margin = new Thickness(360, 0, 0, 0);

            btn = custom_button("control_batch_span_count", g, 60, 16, "÷数量", 10, true);
            btn.HorizontalAlignment = HorizontalAlignment.Left;
            btn.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetRow(btn, 1);
            btn.Margin = new Thickness(360, 0, 0, 0);

            btn = custom_button("control_batch_span_time", g, 60, 16, "×时长", 10, true);
            btn.HorizontalAlignment = HorizontalAlignment.Left;
            btn.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetRow(btn, 1);
            btn.Margin = new Thickness(430, 0, 0, 0);

            r = new Rectangle()
            {
                Name = "control_batch_bg4",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Stretch,
                Width = 100,
                Fill = C(50, 127, 240, 255),
                Margin = new Thickness(500, 0, 0, 0),
            };
            Grid.SetRowSpan(r, 2);
            reg_name(g, r);

            input = custom_input("control_batch_expand_input", g, 30, "4", "多线扩展：",
                C(127, 255, 0));
            input.HorizontalAlignment = HorizontalAlignment.Left;
            input.VerticalAlignment = VerticalAlignment.Center;
            input.Margin = new Thickness(500, 0, 0, 0);

            btn = custom_button("control_batch_expand", g, 60, 16, "扩展", 10, true);
            btn.HorizontalAlignment = HorizontalAlignment.Left;
            btn.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetRow(btn, 1);
            btn.Margin = new Thickness(520, 0, 0, 0);
            //输入倍率 倍增气球数量（>0 向上取整）
            //1. 密度不变
            //2. 时间不变

            /* 红气球
             * 剪切
             * 复制 回合a -> b
             * 
             * 
             */

            #endregion control 2 3 batch
            #region control 2 4 attr
            g = new Grid()
            {
                Name = "control_2_4_grid",
                Background = C(63, 255, 255, 255),
                Visibility = Visibility.Hidden,
            };
            Grid.SetRow(g, 2);
            reg_name(control_grid, g);

            StackPanel attr_select = custom_select("control_attr_select", g, 190, 20, 
                get_attr_choices(), "选择属性：", C(255, 255, 0), 10);
            attr_select.HorizontalAlignment = HorizontalAlignment.Left;
            attr_select.VerticalAlignment = VerticalAlignment.Top;
            attr_select.Margin = new Thickness(10, 1, 0, 0);

            t = new TextBlock()
            {
                Name = "control_attr_old_text",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Text = " 当前值：默认",
                Foreground = C(127, 255, 255),
                FontSize = 10,
            };
            reg_name(attr_select, t);

            StackPanel attr_input = custom_input("control_attr_input", attr_select,
                70, "默认", "    新值：", C(150, 255, 150), 10);
            attr_input.HorizontalAlignment = HorizontalAlignment.Left;
            attr_input.VerticalAlignment = VerticalAlignment.Center;

            btn = custom_button("control_attr_confirm", attr_select, 50, 20, "更改", 10, true);
            btn.Width = 60;
            btn.HorizontalAlignment = HorizontalAlignment.Left;
            btn.VerticalAlignment = VerticalAlignment.Center;

            btn = custom_button("control_attr_advanced", g, 70, 20, "高级设置", 10);
            btn.HorizontalAlignment = HorizontalAlignment.Right;
            btn.VerticalAlignment = VerticalAlignment.Center;
            btn.Margin = new Thickness(0, 0, 5, 0);

            t = new TextBlock()
            {
                Name = "control_attr_inf_text",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Bottom,
                FontSize = 11,
                Text = "介绍：" + attrs_inf["start cash"],
                Foreground = C(127, 255, 0),
                Margin = new Thickness(20, 0, 0, 1)
            };
            reg_name(g, t);
            #endregion control 2 4 attr
            #region control 2 5 random
            g = new Grid()
            {
                Name = "control_2_5_grid",
                Background = C(63, 255, 255, 255),
                Visibility = Visibility.Hidden,
            };
            for (int i = 0; i < 2; i++)
            {
                g.RowDefinitions.Add(new RowDefinition());
            }
            for (int i = 0; i < 7; i++)
            {
                g.ColumnDefinitions.Add(new ColumnDefinition());
            }
            g.ColumnDefinitions[2].Width = new GridLength(1.27, GridUnitType.Star);
            g.ColumnDefinitions[3].Width = new GridLength(1.1, GridUnitType.Star);
            Grid.SetRow(g, 2);
            reg_name(control_grid, g);

            input = custom_input("control_random_seed_input", g, 105, "skycoop", "输入种子: ",
                C(255, 255, 0));
            input.HorizontalAlignment = HorizontalAlignment.Right;
            input.VerticalAlignment = VerticalAlignment.Center;
            input.Margin = new Thickness(0, 0, 3, 0);
            Grid.SetColumnSpan(input, 2);

            select = custom_select("control_random_preset", g, 50, 20, random_presets, "战况: ",
                C(0, 255, 0), 10, 1);
            select.HorizontalAlignment = HorizontalAlignment.Right;
            select.VerticalAlignment = VerticalAlignment.Center;
            select.Margin = new Thickness(0, 0, 3, 0);
            Grid.SetRow(select, 1);

            select = custom_select("control_random_length", g, 50, 20, random_lengths, "长度: ",
                C(0, 255, 0), 10, 2);
            select.HorizontalAlignment = HorizontalAlignment.Right;
            select.VerticalAlignment = VerticalAlignment.Center;
            select.Margin = new Thickness(0, 0, 3, 0);
            Grid.SetRow(select, 1);
            Grid.SetColumn(select, 1);

            select = custom_select("control_random_regrow", g, 50, 20, random_regrows, "重生: ",
                C(0, 255, 255), 10, 2);
            select.HorizontalAlignment = HorizontalAlignment.Right;
            select.VerticalAlignment = VerticalAlignment.Center;
            select.Margin = new Thickness(0, 0, 3, 0);
            Grid.SetRow(select, 0);
            Grid.SetColumn(select, 2);

            select = custom_select("control_random_camo", g, 50, 20, random_camos, "迷彩: ",
                C(0, 255, 255), 10, 2);
            select.HorizontalAlignment = HorizontalAlignment.Right;
            select.VerticalAlignment = VerticalAlignment.Center;
            select.Margin = new Thickness(0, 0, 3, 0);
            Grid.SetRow(select, 0);
            Grid.SetColumn(select, 3);

            select = custom_select("control_random_fort", g, 50, 20, random_forts, "加固: ",
                C(0, 255, 255), 10, 2);
            select.HorizontalAlignment = HorizontalAlignment.Right;
            select.VerticalAlignment = VerticalAlignment.Center;
            select.Margin = new Thickness(0, 0, 3, 0);
            Grid.SetRow(select, 0);
            Grid.SetColumn(select, 4);

            select = custom_select("control_random_earliers", g, 62, 20, random_earliers, "新气球: ",
                C(255, 255, 0), 10, 1);
            select.HorizontalAlignment = HorizontalAlignment.Right;
            select.VerticalAlignment = VerticalAlignment.Center;
            select.Margin = new Thickness(0, 0, 3, 0);
            Grid.SetRow(select, 1);
            Grid.SetColumn(select, 2);

            select = custom_select("control_random_bosses", g, 50, 20, random_bosses, "BOSS: ",
                C(255, 255, 0), 10, 0);
            select.HorizontalAlignment = HorizontalAlignment.Right;
            select.VerticalAlignment = VerticalAlignment.Center;
            select.Margin = new Thickness(0, 0, 3, 0);
            Grid.SetRow(select, 1);
            Grid.SetColumn(select, 3);

            select = custom_select("control_random_attrs", g, 50, 20, random_attrs, "属性: ",
                C(255, 255, 0), 10, 0);
            select.HorizontalAlignment = HorizontalAlignment.Right;
            select.VerticalAlignment = VerticalAlignment.Center;
            select.Margin = new Thickness(0, 0, 3, 0);
            Grid.SetRow(select, 1);
            Grid.SetColumn(select, 4);

            select = custom_select("control_random_diffs", g, 45, 20, random_difficulties, "难度: ",
                C(255, 255, 255), 11, 5);
            select.HorizontalAlignment = HorizontalAlignment.Right;
            select.VerticalAlignment = VerticalAlignment.Center;
            select.Margin = new Thickness(0, 0, 3, 0);
            Grid.SetRowSpan(select, 2);
            Grid.SetColumn(select, 5);

            btn = custom_button("control_random_roll", g, 60, 18, "生成！");
            btn.HorizontalAlignment = HorizontalAlignment.Center;
            btn.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumn(btn, 6);
            #endregion control 2 5 random
            #region control 2 6 config
            g = new Grid()
            {
                Name = "control_2_6_grid",
                Background = C(63, 255, 255, 255),
                Visibility = Visibility.Hidden,
            };
            g.RowDefinitions.Add(new RowDefinition());
            g.RowDefinitions.Add(new RowDefinition());
            for (int i = 0; i < 4; i++)
            {
                g.ColumnDefinitions.Add(new ColumnDefinition());
            }
            Grid.SetRow(g, 2);
            reg_name(control_grid, g);

            btn = custom_button("control_auto_save", g, 120, 18, "自动保存到游戏", 11, true, true);
            Grid.SetRow(btn, 0);
            Grid.SetColumn(btn, 0);

            btn = custom_button("control_screenshot", g, 120, 18, "保存到工作区时截图", 11, false, true);
            Grid.SetRow(btn, 0);
            Grid.SetColumn(btn, 1);

            btn = custom_button("control_wrapswitch", g, 120, 18, "禁止从游戏传送到此", 11, false, true);
            Grid.SetRow(btn, 0);
            Grid.SetColumn(btn, 2);
            #endregion control 2 6
        }




        private void bloon_selector_init()
        {
            ComboBox bloon_selector = (ComboBox)FindName("control_bloon_cb_select");
            bloon_selector.Items.Clear();
            string group = get_sel_str("control_group_cb_select");
            foreach (bloon_template bt
                in bloon_types[group])
            {
                var item = new ComboBoxItem();
                item.Content = bt.name;
                bloon_selector.Items.Add(item);
            }
            bloon_selector.SelectedIndex = 0;
            if (group == "Original")
            {
                bloon_selector.SelectedIndex = 1;
            }
        }
    }
}
