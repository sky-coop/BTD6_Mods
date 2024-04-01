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

namespace BTD6_DIY_Rounds
{
    public partial class MainWindow : Window
    {
        private void visual_debut_grid_init()
        {
            Rectangle r;
            TextBlock t;
            Grid g;
            Grid part;
            Grid btn;

            g = new Grid()
            {
                Name = "ext_debut_grid",
            };
            reg_name(ext_grid, g);
            g.RowDefinitions.Add(new RowDefinition());
            g.RowDefinitions.Add(new RowDefinition());
            g.RowDefinitions[1].Height = new GridLength(4, GridUnitType.Star);

            #region top
            part = new Grid()
            {
                Name = "ext_debut_top_grid",
            };
            reg_name(g, part);

            btn = custom_button("ext_debut_exit", part, 40, 40, "返回", 12);
            btn.HorizontalAlignment = HorizontalAlignment.Left;
            btn.VerticalAlignment = VerticalAlignment.Center;
            btn.Margin = new Thickness(40, 0, 0, 0);

            StackPanel debut_text_panel = new StackPanel
            {
                Name = "ext_debut_text_panel",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 0, 50, 0)
            };
            reg_name(part, debut_text_panel);

            t = new TextBlock()
            {
                Name = "ext_debut_text1",
                HorizontalAlignment = HorizontalAlignment.Left,
                Text = "气球首次登场回合",
                Foreground = C(255, 255, 255),
                FontSize = 22,
            };
            reg_name(debut_text_panel, t);

            t = new TextBlock()
            {
                Name = "ext_debut_text2",
                HorizontalAlignment = HorizontalAlignment.Left,
                Text = "（无属性）",
                Foreground = C(0, 255, 0),
                FontSize = 22,
            };
            reg_name(debut_text_panel, t);

            btn = custom_button("ext_debut_switch", part, 40, 40, "切换", 12);
            btn.HorizontalAlignment = HorizontalAlignment.Right;
            btn.VerticalAlignment = VerticalAlignment.Center;
            btn.Margin = new Thickness(0, 0, 110, 0);

            btn = custom_button("ext_debut_camo", part, 40, 40, "迷彩", 12);
            btn.HorizontalAlignment = HorizontalAlignment.Right;
            btn.VerticalAlignment = VerticalAlignment.Center;
            btn.Margin = new Thickness(0, 0, 60, 0);
            get_react(btn).Fill = C(127, 0, 255, 0);

            btn = custom_button("ext_debut_sort", part, 40, 40, "排序", 12);
            btn.HorizontalAlignment = HorizontalAlignment.Right;
            btn.VerticalAlignment = VerticalAlignment.Center;
            btn.Margin = new Thickness(0, 0, 10, 0);
            get_react(btn).Fill = C(127, 0, 255, 0);
            #endregion top
            #region center
            part = new Grid()
            {
                Name = "ext_debut_center_grid",
            };
            reg_name(g, part);
            Grid.SetRow(part, 1);

            for (int i = 0; i < debut_row; i++)
            {
                part.RowDefinitions.Add(new RowDefinition());
            }
            for (int i = 0; i < debut_col; i++)
            {
                part.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (int i = 0; i < debut_row; i++)
            {
                for(int j = 0; j < debut_col; j++)
                {
                    Grid debut = new Grid()
                    {
                        Name = "ext_debut_grid_" + i + "_" + j,
                        Background = C(127, 0, (byte)(40 + 20 * i), (byte)(40 + 20 * j)),
                    };
                    Grid.SetRow(debut, i);
                    Grid.SetColumn(debut, j);
                    reg_name(part, debut);

                    t = new TextBlock()
                    {
                        Name = "ext_debut_no_" + i + "_" + j,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        Text = "No." + (i * debut_col + j + 1 + 100) + "/" + (all_bt.Count + 100),
                        FontSize = 9,
                        Foreground = C(255, 175, 0),
                    };
                    reg_name(debut, t);

                    Grid img;

                    img = custom_button_pic("ext_debut_pic_" + i + "_" + j, debut, 
                        40, 40, pic("Bloons Pic/Red.png"));
                    img.HorizontalAlignment = HorizontalAlignment.Left;
                    img.VerticalAlignment = VerticalAlignment.Top;
                    img.Margin = new Thickness(5, 15, 0, 0);
                    get_react(img).Visibility = Visibility.Hidden;

                    t = new TextBlock()
                    {
                        Name = "ext_debut_name_" + i + "_" + j,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Top,
                        Text = "GoldenLeadPurpleZebraFortifiedCamo",
                        TextWrapping = TextWrapping.Wrap,
                        Foreground = C(255, 255, 255),
                        Margin = new Thickness(50, 15, 10, 0),
                        FontSize = 8,
                    };
                    reg_name(debut, t);

                    t = new TextBlock()
                    {
                        Name = "ext_debut_round_" + i + "_" + j,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Top,
                        Text = "首次出现：140 回合",
                        TextWrapping = TextWrapping.Wrap,
                        Foreground = C(0, 255, 255),
                        Margin = new Thickness(50, 36, 10, 0),
                        FontSize = 8.5,
                    };
                    reg_name(debut, t);

                    t = new TextBlock()
                    {
                        Name = "ext_debut_count_" + i + "_" + j,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Top,
                        Text = "数量：999999",
                        TextWrapping = TextWrapping.Wrap,
                        Foreground = C(255, 255, 0),
                        Margin = new Thickness(50, 48, 10, 0),
                        FontSize = 8.5,
                    };
                    reg_name(debut, t);

                    Grid prop = new Grid()
                    {
                        Name = "ext_debut_prop_grid_"+ i + "_" + j,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Top,
                        Width = 92,
                        Height = 11.5,
                        Margin = new Thickness(0, 0, 1, 0),
                    };
                    for (int k = 0; k < 8; k++)
                    {
                        prop.ColumnDefinitions.Add(new ColumnDefinition());
                    }
                    reg_name(debut, prop);

                    for (int k = 0; k < 8; k++)
                    {
                        string s = "";
                        switch(k)
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
                                s = "Bloons Pic/Lead.png";
                                break;
                            case 4:
                                s = "Bloons Pic/Purple.png";
                                break;
                            case 5:
                                s = "Bloons Pic/Zebra.png";
                                break;
                            case 6:
                                s = "Bloons Pic/Elite.png";
                                break;
                            case 7:
                                s = "Bloons Pic/1.png";
                                break;
                        }
                        img = custom_button_pic("ext_debut_pic_" + i + "_" + j + "_" + k, prop,
                            11.5, 11.5, pic(s));
                        Grid.SetColumn(img, k);
                        get_react(img).Visibility = Visibility.Hidden;
                    }
                }
            }
            #endregion center
        }


        bool cash_show_cash = false;
        int cash_row = 7;
        private void visual_cash_grid_init()
        {
            Rectangle r;
            TextBlock t;
            Grid g;
            Grid part;
            Grid btn;

            g = new Grid()
            {
                Name = "ext_cash_grid",
            };
            reg_name(ext_grid, g);
            g.RowDefinitions.Add(new RowDefinition());
            g.RowDefinitions.Add(new RowDefinition());
            g.RowDefinitions.Add(new RowDefinition());
            g.RowDefinitions[1].Height = new GridLength(5, GridUnitType.Star);

            #region top
            part = new Grid()
            {
                Name = "ext_cash_top_grid",
            };
            reg_name(g, part);

            btn = custom_button("ext_cash_exit", part, 35, 35, "返回", 11);
            btn.HorizontalAlignment = HorizontalAlignment.Left;
            btn.VerticalAlignment = VerticalAlignment.Center;
            btn.Margin = new Thickness(40, 0, 0, 0);

            t = new TextBlock()
            {
                Name = "ext_cash_top_text",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 20,
                Text = "属性浏览器",
                Foreground = C(255, 255, 0),
            };
            reg_name(part, t);

            btn = custom_button("ext_cash_switch", part, 120, 30, "设置回合金钱倍率", 11, false, true);
            btn.HorizontalAlignment = HorizontalAlignment.Right;
            btn.VerticalAlignment = VerticalAlignment.Center;
            btn.Margin = new Thickness(0, 0, 20, 0);
            #endregion top
            #region center
            part = new Grid()
            {
                Name = "ext_cash_center_grid",
            };
            Grid.SetRow(part, 1);
            reg_name(g, part);

            for(int i = 0; i < cash_row; i++)
            {
                part.RowDefinitions.Add(new RowDefinition());
            }

            for(int i = 0; i < cash_row; i++)
            {
                Grid grid = new Grid()
                {
                    Name = "ext_cash_grid_" + i,
                    Background = C(100, 0, (byte)(140 + 100 * i / cash_row), 100),
                };
                Grid.SetRow(grid, i);
                reg_name(part, grid);

                t = new TextBlock()
                {
                    Name = "ext_cash_text1_" + i,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center,
                    Text = "123",
                    Foreground = C(255, 255, 127),
                    Margin = new Thickness(10, 0, 0, 0),
                };
                reg_name(grid, t);

                t = new TextBlock()
                {
                    Name = "ext_cash_text2_" + i,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center,
                    Text = "123",
                    Foreground = C(255, 255, 255),
                    Margin = new Thickness(300, 0, 0, 0),
                };
                reg_name(grid, t);

                t = new TextBlock()
                {
                    Name = "ext_cash_text3_" + i,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Center,
                    Text = "123",
                    TextWrapping = TextWrapping.Wrap,
                    Foreground = C(0, 255, 127),
                    MaxWidth = 150,
                    Margin = new Thickness(0, 0, 10, 0),
                    FontSize = 9,
                    FontFamily = new FontFamily("SimHei")
                };
                reg_name(grid, t);

                btn = custom_button("ext_cash_del_" + i, grid, 50, 15, "删除", 10);
                btn.HorizontalAlignment = HorizontalAlignment.Right;
                btn.VerticalAlignment = VerticalAlignment.Center;
                btn.Margin = new Thickness(0, 0, 5, 0);
            }
            #endregion center
            #region bottom
            part = new Grid()
            {
                Name = "ext_cash_bottom_grid",
            };
            Grid.SetRow(part, 2);
            reg_name(g, part);

            #region cash
            Grid cash = new Grid()
            {
                Name = "ext_cash_cash_grid",
                HorizontalAlignment = HorizontalAlignment.Left,
                Width = 400,
            };
            reg_name(part, cash);

            StackPanel input;

            input = custom_input("ext_cash_input_int", cash, 70, "51", "回合：",
                C(255, 255, 0));
            input.HorizontalAlignment = HorizontalAlignment.Left;
            input.VerticalAlignment = VerticalAlignment.Center;
            input.Margin = new Thickness(5, 0, 0, 0);

            input = custom_input("ext_cash_input_double", cash, 70, "0.5", "金钱倍率：",
                C(255, 255, 0));
            input.HorizontalAlignment = HorizontalAlignment.Left;
            input.VerticalAlignment = VerticalAlignment.Center;
            input.Margin = new Thickness(125, 0, 0, 0);

            btn = custom_button("ext_cash_add", cash, 60, 20, "插入");
            btn.HorizontalAlignment = HorizontalAlignment.Left;
            btn.VerticalAlignment = VerticalAlignment.Center;
            btn.Margin = new Thickness(260, 0, 0, 0);

            btn = custom_button("ext_cash_default", cash, 70, 20, "恢复默认");
            btn.HorizontalAlignment = HorizontalAlignment.Left;
            btn.VerticalAlignment = VerticalAlignment.Center;
            btn.Margin = new Thickness(325, 0, 0, 0);
            #endregion cash

            #region page
            Grid page = new Grid()
            {
                Name = "ext_cash_page_grid",
                HorizontalAlignment = HorizontalAlignment.Right,
                Width = 180,
            };
            reg_name(part, page);

            btn = custom_button("ext_cash_page_down", page, 60, 20, "上一页", 10);
            btn.HorizontalAlignment = HorizontalAlignment.Left;
            btn.VerticalAlignment = VerticalAlignment.Center;

            t = new TextBlock()
            {
                Name = "ext_cash_page_text",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 10,
                Foreground = C(0, 255, 255),
            };
            reg_name(page, t);

            btn = custom_button("ext_cash_page_up", page, 60, 20, "下一页", 10);
            btn.HorizontalAlignment = HorizontalAlignment.Right;
            btn.VerticalAlignment = VerticalAlignment.Center;
            #endregion page

            #endregion center
        }

        int curr_hint = 1;
        private void visual_hint_grid_init()
        {
            Rectangle r;
            TextBlock t;
            Grid g;
            Grid part;
            Grid btn;

            g = new Grid()
            {
                Name = "ext_hint_grid",
            };
            reg_name(ext_grid, g);
            g.RowDefinitions.Add(new RowDefinition());
            g.RowDefinitions.Add(new RowDefinition());
            g.RowDefinitions[1].Height = new GridLength(4, GridUnitType.Star);

            #region top
            part = new Grid()
            {
                Name = "ext_hint_top_grid",
            };
            reg_name(g, part);

            btn = custom_button("ext_hint_exit", part, 40, 40, "返回", 12);
            btn.HorizontalAlignment = HorizontalAlignment.Left;
            btn.VerticalAlignment = VerticalAlignment.Center;
            btn.Margin = new Thickness(40, 0, 0, 0);

            t = new TextBlock()
            {
                Name = "ext_hint_top_text",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 20,
                Text = "提示编辑器",
                Foreground = C(0, 255, 0),
            };
            reg_name(part, t);

            btn = custom_button("ext_hint_prev", part, 40, 40, "←", 18, false);
            btn.HorizontalAlignment = HorizontalAlignment.Right;
            btn.VerticalAlignment = VerticalAlignment.Center;
            btn.Margin = new Thickness(0, 0, 80, 0);

            btn = custom_button("ext_hint_next", part, 40, 40, "→", 18, false);
            btn.HorizontalAlignment = HorizontalAlignment.Right;
            btn.VerticalAlignment = VerticalAlignment.Center;
            btn.Margin = new Thickness(0, 0, 20, 0);
            #endregion top
            #region center
            part = new Grid()
            {
                Name = "ext_hint_center_grid",
            };
            Grid.SetRow(part, 1);
            reg_name(g, part);

            Grid grid = new Grid() 
            { 
                Name = "ext_hint_hint_grid",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(10, 0, 10, 10),
            };
            reg_name(part, grid);
            for(int i = 0; i < 2; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition());
            }

            t = new TextBlock()
            {
                Name = "ext_hint_hintA",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Text = "提示：这是第1回合的结束语，当你连续游玩时，它也可被视为第2回合的介绍。",
                FontSize = 12,
                Padding = new Thickness(0, 2, 0, 2),
                Foreground = C(0, 255, 0),
            };
            reg_name(grid, t);

            t = new TextBlock()
            {
                Name = "ext_hint_hintB",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Text = "提示：若游戏从此回合（第1回合）开始，则它也会在游戏开始时显示一次，" +
                    "但重玩时不再次显示",
                FontSize = 12,
                Padding = new Thickness(0, 2, 0, 2),
                Foreground = C(0, 255, 255),
            };
            Grid.SetRow(t, 1);
            reg_name(grid, t);

            double ratio = 0.5;
            Grid container = new Grid()
            {
                Name = "ext_hint_container_grid",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(0, 0, 0, 56),
                Width = 923 * ratio,
                Height = 151 * ratio,
            };
            reg_name(part, container);

            Grid preview = new Grid()
            {
                Name = "ext_hint_preview_grid",
            };
            reg_name(container, preview);

            LinearGradientBrush lgb = new LinearGradientBrush();
            lgb.GradientStops.Add(new GradientStop(Color.FromRgb(106, 157, 218), 0));
            lgb.GradientStops.Add(new GradientStop(Color.FromRgb(74, 128, 207), 1));
            lgb.StartPoint = new Point(0.5, 0);
            lgb.EndPoint = new Point(0.5, 1);
            r = new Rectangle()
            {
                Name = "ext_hint_preview_bg",
                Fill = lgb,
                RadiusX = 20 * ratio,
                RadiusY = 20 * ratio,
                StrokeThickness = 3 * ratio,
                Stroke = C(58, 77, 121)
            };
            reg_name(preview, r);

            t = new TextBlock()
            {
                Name = "ext_hint_text",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(30 * ratio, 0, 30 * ratio, 0),
                FontSize = 32 * ratio,
                Tag = 32 * ratio,
                TextWrapping = TextWrapping.Wrap,
                FontFamily = new FontFamily("SimHei"),
                FontWeight = FontWeight.FromOpenTypeWeight(500),
                Foreground = C(255, 255, 255),
                LineHeight = 32 * 1.4 * ratio,
            };
            reg_name(preview, t);

            t = new TextBlock()
            {
                Name = "ext_hint_notice",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(40, 99, 0, 0),
                Text = "下方是提示的预览，请注意它与实际的显示情况略有不同：",
                FontSize = 12,
                Foreground = C(127, 255, 127),
            };
            reg_name(part, t);

            r = new Rectangle
            {
                Name = "ext_hint_color",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(20, 13, 0, 0),
                Width = 40,
                Height = 40,
                Stroke = C(0, 0, 0),
            };
            reg_name(part, r);

            StackPanel input = custom_input("ext_hint_input", part, 426, 
                "这是示例文本：\n" + 
                "你可以按回车键换行，但你不可以选中换行符，否则会因中文输入法修改换行符引发巨型BUG\n" + 
                "你还可以在下方更改文字颜色\n" + 
                "按“提交”按钮进行保存，下方可以显示预览\n" + 
                "文字最多显示5行，每行最多42字，否则会超出框的范围",
                "", C(255, 255, 0), 10, true, 60);
            input.HorizontalAlignment = HorizontalAlignment.Center;
            input.VerticalAlignment = VerticalAlignment.Top;
            input.Margin = new Thickness(0, 3, 0, 0);
            BOX("ext_hint_input").SelectionChanged += input_selection_change;
            BOX("ext_hint_input").FontFamily = new FontFamily("SimHei");

            input = custom_input("ext_hint_r", part, 50, "255", "红色：",
                C(255, 0, 0));
            input.HorizontalAlignment = HorizontalAlignment.Left;
            input.VerticalAlignment = VerticalAlignment.Top;
            input.Margin = new Thickness(40, 75, 0, 0);

            input = custom_input("ext_hint_g", part, 50, "255", "绿色：",
                C(0, 255, 0));
            input.HorizontalAlignment = HorizontalAlignment.Left;
            input.VerticalAlignment = VerticalAlignment.Top;
            input.Margin = new Thickness(140, 75, 0, 0);

            input = custom_input("ext_hint_b", part, 50, "255", "蓝色：",
                C(0, 127, 255));
            input.HorizontalAlignment = HorizontalAlignment.Left;
            input.VerticalAlignment = VerticalAlignment.Top;
            input.Margin = new Thickness(240, 75, 0, 0);

            input = custom_input("ext_hint_a", part, 50, "255", "不透明度：",
                C(127, 255, 255, 255));
            input.HorizontalAlignment = HorizontalAlignment.Left;
            input.VerticalAlignment = VerticalAlignment.Top;
            input.Margin = new Thickness(340, 75, 0, 0);

            btn = custom_button("ext_hint_default", part, 60, 18, "重置颜色", 10);
            btn.HorizontalAlignment = HorizontalAlignment.Left;
            btn.VerticalAlignment = VerticalAlignment.Top;
            btn.Margin = new Thickness(460, 74, 0, 0);

            btn = custom_button("ext_hint_commit", part, 60, 21, "提交", 12, true);
            btn.HorizontalAlignment = HorizontalAlignment.Right;
            btn.VerticalAlignment = VerticalAlignment.Top;
            btn.Margin = new Thickness(0, 7, 10, 0);

            btn = custom_button("ext_hint_del", part, 60, 21, "删除", 12);
            btn.HorizontalAlignment = HorizontalAlignment.Right;
            btn.VerticalAlignment = VerticalAlignment.Top;
            btn.Margin = new Thickness(0, 38, 10, 0);


            //1 80
            //2 80
            //3 59.65
            //4 50
            //5 50
            #endregion center
        }

        private void input_selection_change(object sender, RoutedEventArgs e)
        {
            TextBox box = (TextBox)sender;
            int n = box.SelectionStart;
            string s = box.SelectedText;
            if (s.Contains("\n"))
            {
                box.SelectionChanged -= input_selection_change;
                string[] part = s.Split('\n');
                int confirmed_start = n;
                int max = -1;
                for (int i = 0; i < part.Length; i++)
                {
                    if (part[i].Length > max)
                    {
                        confirmed_start = n;
                        max = part[i].Length;
                    }
                    n += part[i].Length + 1;
                }
                box.SelectionStart = confirmed_start;
                box.SelectionLength = max;

                box.SelectionChanged += input_selection_change;
            }
        }
    }
}
