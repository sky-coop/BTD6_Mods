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

namespace BTD6_DIY_Rounds
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private void bloon_show()
        {
            //roll("skycoop");
            round cr = rounds[curr_round];
            visibility_transfer(G("loading_grid"), loading);
            if (loading)
            {
                cr = rounds[0];
                TextBlock t = T("loading_hint");
                t.Text = "预览的提示信息: [无]";
                if (cp_import.hint != null)
                {
                    var argb = cp_import.hint.Item1;
                    t.Text = "预览的提示信息: \n" + cp_import.hint.Item2;
                    t.Foreground = C(argb.a, argb.r, argb.g, argb.b);
                }
            }

            List<bloon_entry> b = cr.bloon_entrys;
            bloon_template bt;

            while (bloon_no_base + bloon_row - 1 > rounds[curr_round].bloon_entrys.Count)
            {
                if (bloon_no_base > 1)
                {
                    bloon_no_base--;
                    bloon_modifying++;
                }
                else
                {
                    break;
                }
            }


            int start_round = get_start_round();
            int end_round = get_end_round();
            #region rounds
            for (int i = 0; i < page_i; i++)
            {
                for(int j = 0; j < page_j; j++)
                {
                    int index = i * page_j + j;
                    int round = (current_page - 1) * (page_i * page_j) + index + 1;
                    TextBlock text = (TextBlock)FindName(
                        round_select_grid.Name + "_" + i + "_" + j + "_text");
                    if (hints.ContainsKey(round))
                    {
                        text.Foreground = C(255, 0, 0);
                    }
                    else
                    {
                        text.Foreground = C(0, 0, 0);
                    }

                    Rectangle cover = (Rectangle)FindName
                        (round_select_grid.Name + "_" + i + "_" + j);
                    cover.StrokeThickness = 1.5;
                    if (round < start_round || round > end_round)
                    {
                        cover.Stroke = C(127, 0, 0);
                    }
                    else
                    {
                        cover.Stroke = C(0, 0, 200);
                    }

                    if (round == curr_round)
                    {
                        Rectangle react = (Rectangle)FindName(
                            "round_select_grid_" + i + "_" + j);
                        react.Fill = C(127, 0, 255, 0);
                    }
                }
            }
            visibility_transfer(invalid_react,
                curr_round < start_round || curr_round > end_round);
            #endregion rounds

            bool golden = false;
            #region bloon rows
            for (int i = 0; i < bloon_row; i++)
            {
                Grid main = (Grid)FindName("bloons_grid_" + i);
                int index = bloon_no_base + i - 1;
                if (b.Count > index)
                {
                    main.Visibility = Visibility.Visible;

                    bloon_entry be = b[index];
                    bt = find_bt(b[index].bloon_name);
                    golden = be.bloon_name == "Golden";
                    if (golden)
                    {
                        if (!be.lead)
                        {
                            be.fort = false;
                            be.purple = false;
                        }
                        if (!be.purple)
                        {
                            be.zebra = false;
                        }
                    }

                    TextBlock no_text = (TextBlock)FindName("bloons_no_" + (i + 1));
                    string no_s = "No." + (index + 1) + "/" + b.Count;
                    double shrink = Math.Max(8, no_s.Length) / 8.0;
                    no_text.Text = no_s;
                    no_text.FontSize = 60.0 / bloon_row / shrink;

                    if (b.Count > bloon_row ||
                       bloon_no_base > 1)
                    {
                        no_text.Foreground = C(255, 175, 0);
                    }
                    else
                    {
                        no_text.Foreground = C(255, 255, 255);
                    }

                    ((TextBlock)FindName("bloons_name_" + i)).Text = 
                       bloon_name_mix(b[index]);

                    Image img =
                        (Image)FindName("bloons_img_" + i + "_img");
                    string pic_name = be.bloon_name;
                    try_remove_number(ref pic_name);
                    string pic_path = "Bloons Pic/" + pic_name + ".png";
                    if (File.Exists(pic_path))
                    {
                        img.Source = pic(pic_path);
                    }
                    else
                    {
                        img.Source = pic("Bloons Pic/No Icon.png");
                    }

                    img = (Image)FindName("bloons_img_stat_" + i + "_1_3_img");
                    pic_name = be.level.ToString();
                    pic_path = "Bloons Pic/" + pic_name + ".png";
                    if (File.Exists(pic_path))
                    {
                        img.Source = pic(pic_path);
                    }
                    else
                    {
                        img.Source = pic("Bloons Pic/No Icon.png");
                    }


                    visibility_transfer(
                        (Grid)FindName("bloons_img_stat_" + i + "_0_0_grid"),
                        bt.has_camo);
                    visibility_transfer(
                        (Grid)FindName("bloons_img_stat_" + i + "_0_1_grid"),
                        bt.has_regrow);
                    visibility_transfer(
                        (Grid)FindName("bloons_img_stat_" + i + "_0_2_grid"),
                        bt.has_fort);
                    visibility_transfer(
                        (Grid)FindName("bloons_img_stat_" + i + "_1_0_grid"),
                        bt.has_lead);
                    visibility_transfer(
                        (Grid)FindName("bloons_img_stat_" + i + "_1_1_grid"),
                        bt.has_purple);
                    visibility_transfer(
                        (Grid)FindName("bloons_img_stat_" + i + "_1_2_grid"),
                        bt.has_zebra);
                    visibility_transfer(
                        (Grid)FindName("bloons_img_stat_" + i + "_0_3_grid"),
                        bt.has_elite);
                    visibility_transfer(
                        (Grid)FindName("bloons_img_stat_" + i + "_1_3_grid"),
                        bt.has_elite);

                    visibility_transfer(
                        (Rectangle)FindName("bloons_img_stat_" + i + "_0_0_react"),
                        be.camo);
                    visibility_transfer(
                        (Rectangle)FindName("bloons_img_stat_" + i + "_0_1_react"),
                        be.regrow);
                    visibility_transfer(
                        (Rectangle)FindName("bloons_img_stat_" + i + "_0_2_react"),
                        be.fort);
                    visibility_transfer(
                        (Rectangle)FindName("bloons_img_stat_" + i + "_1_0_react"),
                        be.lead);
                    visibility_transfer(
                        (Rectangle)FindName("bloons_img_stat_" + i + "_1_1_react"),
                        be.purple);
                    visibility_transfer(
                        (Rectangle)FindName("bloons_img_stat_" + i + "_1_2_react"),
                        be.zebra);
                    visibility_transfer(
                        (Rectangle)FindName("bloons_img_stat_" + i + "_0_3_react"),
                        be.elite);
                    visibility_transfer(
                        (Rectangle)FindName("bloons_img_stat_" + i + "_1_3_react"),
                        false);

                    double left_length = be.start;
                    double be_length = be.end - be.start;
                    double right_length = cr.time_max - be.end;
                    double ratio = be_length / cr.time_max;
                    double density = (be.count - 1) / (be_length + 0.001) * 1000;
                    Grid center = (Grid)find_name("bloons_center_grid_" + i);
                    center.ColumnDefinitions[2].Width = new GridLength(left_length, GridUnitType.Star);
                    center.ColumnDefinitions[3].Width = new GridLength(be_length, GridUnitType.Star); 
                    center.ColumnDefinitions[4].Width = new GridLength(right_length, GridUnitType.Star);

                    Rectangle bar = (Rectangle)FindName("bloons_bar_" + i);
                    double color_shift = 60 * Math.Log10(density + 1);
                    if (color_shift > 120)
                    {
                        color_shift = 120;
                    }
                    bar.Fill = getSCB(tool.HslToRgb(120 - color_shift, 255, 127));


                    visibility_transfer((Rectangle)FindName("show_react"), stats_show);
                    for (int k = 0; k < 5; k++)
                    {
                        TextBlock data = (TextBlock)FindName("bloons_data_text_" + i + "_" + k);
                        visibility_transfer(data, stats_show);
                        if (stats_show)
                        {
                            switch(k)
                            {
                                case 0:
                                    data.Text = "RBE:" + RBE(be, curr_round).ToString("f0");
                                    break;
                                case 1:
                                    data.Text = "金钱:" + CASH(be, curr_round).ToString("f2");
                                    break;
                                case 2:
                                    data.Text = "扣血:" + LOST(be, curr_round);
                                    break;
                                case 3:
                                    data.Text = "HP:" + HP(be, curr_round).ToString("f0");
                                    break;
                                case 4:
                                    data.Text = "速度:" + SPEED(be, curr_round).ToString("f3");
                                    break;
                            }
                        }
                    }



                    TextBlock stext = ((TextBlock)FindName("bloons_stext_" + i));
                    TextBlock etext = ((TextBlock)FindName("bloons_etext_" + i));

                    stext.Visibility = Visibility.Visible;
                    etext.Visibility = Visibility.Visible;
                    string str = "(" + be.start.ToString("f2") + "～" +
                                       be.end.ToString("f2") + ")ms";

                    Grid.SetColumn(stext, 3);
                    Grid.SetColumnSpan(stext, 1);
                    Grid.SetColumn(etext, 3);
                    Grid.SetColumnSpan(etext, 1);
                    if (cr.time_max == 0)
                    {
                        etext.Visibility = Visibility.Hidden;
                        stext.Visibility = Visibility.Visible;
                        stext.Text = str;
                        Grid.SetColumn(stext, 2);
                        Grid.SetColumnSpan(stext, 10);
                        center.ColumnDefinitions[4].Width = new GridLength(1, GridUnitType.Star);
                    }
                    else
                    {
                        if (ratio < 0.5)
                        {
                            if (right_length / cr.time_max < 0.25)
                            {
                                stext.Visibility = Visibility.Hidden;
                                etext.Visibility = Visibility.Visible;
                                etext.Text = str;
                                Grid.SetColumn(etext, 0);
                                Grid.SetColumnSpan(etext, 10);
                            }
                            else
                            {
                                etext.Visibility = Visibility.Hidden;
                                stext.Visibility = Visibility.Visible;
                                stext.Text = str;
                                Grid.SetColumnSpan(stext, 10);
                            }
                        }
                        else
                        {
                            stext.Visibility = Visibility.Visible;
                            stext.Text = "Start: " + be.start.ToString("f2") + "ms";
                            etext.Visibility = Visibility.Visible;
                            etext.Text = "End: " + be.end.ToString("f2") + "ms";
                        }
                    }
                    ((TextBlock)FindName("bloons_ctext_" + i)).Text =
                       "×" + be.count.ToString();

                    Rectangle react = (Rectangle)FindName(
                        "bloons_copy_" + i + "_react");
                    if (bloon_modifying + bloon_no_base - 1 == index)
                    {
                        react.Fill = C(127, 0, 255, 0);
                    }
                    else
                    {
                        react.Fill = C(0, 0, 0, 0);
                    }
                }
                else
                {
                    main.Visibility = Visibility.Hidden;
                }
            }
            #endregion bloon rows

            #region cal
            if (control_2 == 2)
            {
                decimal RBE_total = 0;
                double cash_total = control_cash;
                long lost_total = 0;
                decimal[] RBE_level = new decimal[curr_round + 1];
                double[] cash_level = new double[curr_round + 1];
                long[] lost_level = new long[curr_round + 1];
                for (int m = 1; m < curr_round + 1; m++)
                {
                    round r = rounds[m];
                    RBE_level[m] = 0;
                    cash_level[m] = 0;
                    lost_level[m] = 0;
                    foreach (bloon_entry x in r.bloon_entrys)
                    {
                        RBE_level[m] += RBE(x, m);
                        cash_level[m] += CASH(x, m);
                        lost_level[m] += LOST(x, m);
                    }
                    cash_level[m] += (100 + m) * attrs["cash multiplier[end round]"];

                    if (m != 1)
                    {
                        if (control_cal_valid && m > control_round)
                        {
                            RBE_total += RBE_level[m - 1];
                            lost_total += lost_level[m - 1];
                            cash_total += cash_level[m - 1];
                        }
                    }
                }
                for (int m = 0; m < 2; m++)
                {
                    for (int n = 0; n < 4; n++)
                    {
                        TextBlock data = (TextBlock)FindName("control_stat_text_" + m + "_" + n);
                        switch (m * 4 + n)
                        {
                            case 1:
                                data.Text = "RBE:" + RBE_level[curr_round].ToString("f0");
                                break;
                            case 2:
                                data.Text = "金钱:" + cash_level[curr_round].ToString("f2");
                                break;
                            case 3:
                                data.Text = "扣血:" + lost_level[curr_round];
                                break;
                            case 5:
                                data.Text = "RBE:" + RBE_total.ToString("f0");
                                break;
                            case 6:
                                if (control_cal_valid)
                                {
                                    data.Text = "金钱:" + cash_total.ToString("f2");
                                }
                                else
                                {
                                    data.Text = "金钱:未能计算";
                                }
                                break;
                            case 7:
                                data.Text = "扣血:" + lost_total;
                                break;
                        }
                    }
                }
            }
            #endregion cal

            TextBlock add_text = (TextBlock)FindName("control_add_text");
            if (bloon_modifying < int.MinValue / 4)
            {
                add_text.Text = "添加一行";
            }
            else
            {
                add_text.Text = "修改No." + (bloon_modifying + bloon_no_base);
            }

            bt = find_bt(get_sel_str("control_bloon_cb_select"));
            golden = bt.name == "Golden";
            if (golden)
            {
                if (!editing_entry.lead)
                {
                    editing_entry.fort = false;
                    editing_entry.purple = false;
                }
                if (!editing_entry.purple)
                {
                    editing_entry.zebra = false;
                }
            }
            Rectangle sp;
            #region regrow
            sp = (Rectangle)FindName("control_regrow_react");
            visibility_transfer((Grid)FindName("control_regrow_grid"), bt.has_regrow);
            if (editing_entry.regrow)
            {
                sp.Fill = C(127, 0, 255, 0);
            }
            else
            {
                sp.Fill = C(0, 0, 0, 0);
            }
            #endregion regrow
            #region fort
            sp = (Rectangle)FindName("control_fort_react");
            visibility_transfer((Grid)FindName("control_fort_grid"), bt.has_fort);
            if (editing_entry.fort)
            {
                sp.Fill = C(127, 0, 255, 0);
            }
            else
            {
                sp.Fill = C(0, 0, 0, 0);
            }
            #endregion fort
            #region camo
            sp = (Rectangle)FindName("control_camo_react");
            visibility_transfer((Grid)FindName("control_camo_grid"), bt.has_camo);
            if (editing_entry.camo)
            {
                sp.Fill = C(127, 0, 255, 0);
            }
            else
            {
                sp.Fill = C(0, 0, 0, 0);
            }
            #endregion camo
            #region lead
            sp = (Rectangle)FindName("control_lead_react");
            visibility_transfer((Grid)FindName("control_lead_grid"), bt.has_lead);
            if (editing_entry.lead)
            {
                sp.Fill = C(127, 0, 255, 0);
            }
            else
            {
                sp.Fill = C(0, 0, 0, 0);
            }
            #endregion lead
            #region purple
            sp = (Rectangle)FindName("control_purple_react");
            visibility_transfer((Grid)FindName("control_purple_grid"), bt.has_purple);
            if (editing_entry.purple)
            {
                sp.Fill = C(127, 0, 255, 0);
            }
            else
            {
                sp.Fill = C(0, 0, 0, 0);
            }
            #endregion purple
            #region zebra
            sp = (Rectangle)FindName("control_zebra_react");
            visibility_transfer((Grid)FindName("control_zebra_grid"), bt.has_zebra);
            if (editing_entry.zebra)
            {
                sp.Fill = C(127, 0, 255, 0);
            }
            else
            {
                sp.Fill = C(0, 0, 0, 0);
            }
            #endregion zebra
            #region elite
            sp = (Rectangle)FindName("control_elite_react");
            visibility_transfer((Grid)FindName("control_elite_grid"), bt.has_elite);
            if (editing_entry.elite)
            {
                sp.Fill = C(127, 0, 255, 0);
            }
            else
            {
                sp.Fill = C(0, 0, 0, 0);
            }
            #endregion elite

            TextBox box;
            #region load
            if (control_2 == 0)
            {
                box = (TextBox)FindName("control_load_box");
                box_change(box, exist_rounds("./Workspace/" + box.Text + "/DIY Rounds"));
                box = (TextBox)FindName("control_template_box");
                box_change(box, exist_rounds("./Templates/" + box.Text + "/DIY Rounds"));
            }
            #endregion load

            #region attr
            if (control_2 == 4)
            {
                string select = get_sel_str("control_attr_select_select");
                double old_value = attrs[select];
                TextBlock t = (TextBlock)FindName("control_attr_old_text");
                t.Text = " 当前值：" + double2str(old_value);

                t = T("control_attr_inf_text");
                t.Text = "介绍：" + attrs_inf[select];
            }
            #endregion attr
            #region config
            visibility_transfer(get_react("control_auto_save"), auto_save);
            visibility_transfer(back_react, auto_save);
            visibility_transfer(get_react("control_screenshot"), save_shot);
            visibility_transfer(G("sh_ball_grid"), save_shot);
            visibility_transfer(get_react("control_wrapswitch"), wrap_off);
            visibility_transfer(wrap_off_text, wrap_off);
            #endregion config

            #region ext
            visibility_transfer((Grid)FindName("ext_debut_grid"), ext_list[0]);
            visibility_transfer((Grid)FindName("ext_cash_grid"), ext_list[1]);
            visibility_transfer((Grid)FindName("ext_hint_grid"), ext_list[2]);
            bool v = false;
            foreach (Grid g in ext_grid.Children)
            {
                if (g.Visibility == 0)
                {
                    v = true;
                    break;
                }
            }
            visibility_transfer(ext_grid, v);

            if (v)
            {
                #region debut
                TextBlock debut_text2 = (TextBlock)FindName("ext_debut_text2");

                for(int i = 0; i < debut_row; i++)
                {
                    for(int j = 0; j < debut_col; j++)
                    {
                        TextBlock t;

                        t = (TextBlock)FindName("ext_debut_no_" + i + "_" + j);
                        t.Foreground = C(255, 127, 127);
                        t.Text = "无内容";

                        string postfix = i + "_" + j;
                        Image img = (Image)FindName("ext_debut_pic_" + postfix + "_img");
                        img.Source = pic("Bloons Pic/No Icon.png");

                        t = (TextBlock)FindName("ext_debut_name_" + postfix);
                        t.Text = "";

                        t = (TextBlock)FindName("ext_debut_round_" + postfix);
                        t.Text = "";

                        t = (TextBlock)FindName("ext_debut_count_" + postfix);
                        t.Text = "";

                        visibility_transfer((Grid)FindName("ext_debut_prop_grid_" + i + "_" + j),
                            debut_show_type);

                        if (debut_show_type)
                        {
                            for (int k = 0; k < 8; k++)
                            {
                                visibility_transfer((Grid)FindName("ext_debut_pic_" +
                                    postfix + "_" + k + "_grid"), false);
                            }
                        }
                    }
                }

                if (debut_row_base < 0)
                {
                    debut_row_base = 0;
                }
                int start_no = debut_row_base * debut_col + 1;
                int end_no = start_no - 1 + (debut_row * debut_col);

                visibility_transfer((Grid)FindName("ext_debut_camo_grid"), debut_show_type);
                visibility_transfer((Grid)FindName("ext_debut_sort_grid"), debut_show_type);

                if (debut_show_type)
                {
                    debut_text2.Text = "（包含属性）";
                    debut_text2.Foreground = C(255, 127, 127);
                    create_type_debuts();

                    int i = 1;
                    var list = get_debut_type_list();
                    int all = list.Count;

                    void show_bc(bloon_compare bc)
                    {
                        int round = bc.round;
                        int count = bc.count;

                        int index = i - start_no;
                        int m = index / debut_col;
                        int n = index % debut_col;

                        string postfix = m + "_" + n;

                        TextBlock t;

                        t = (TextBlock)FindName("ext_debut_no_" + postfix);
                        t.Text = "No." + i + "/" + all;

                        Image img = (Image)FindName("ext_debut_pic_" + postfix + "_img");
                        string pic_name = bc.base_name;
                        try_remove_number(ref pic_name);
                        string pic_path = "Bloons Pic/" + pic_name + ".png";
                        if (File.Exists(pic_path))
                        {
                            img.Source = pic(pic_path);
                        }
                        else
                        {
                            img.Source = pic("Bloons Pic/No Icon.png");
                        }

                        t = (TextBlock)FindName("ext_debut_name_" + postfix);
                        t.Text = bc.full_name;

                        t = (TextBlock)FindName("ext_debut_round_" + postfix);
                        t.Text = "首次出现：" + round + " 回合";

                        t = (TextBlock)FindName("ext_debut_count_" + postfix);
                        t.Text = "数量：" + count;

                        for (int k = 0; k < 7; k++)
                        {
                            visibility_transfer((Grid)FindName("ext_debut_pic_" +
                                postfix + "_" + k + "_grid"), bc.bools[k]);
                        }

                        bloon_template temp = find_bt(bc.base_name);
                        if (temp == null)
                        {
                            temp = find_bt(bc.base_name + "1");
                        }
                        visibility_transfer((Grid)FindName("ext_debut_pic_" +
                            postfix + "_7_grid"), temp.has_elite);

                        img = (Image)FindName("ext_debut_pic_" + postfix + "_7_img");
                        pic_name = bc.level.ToString();
                        pic_path = "Bloons Pic/" + pic_name + ".png";
                        if (File.Exists(pic_path))
                        {
                            img.Source = pic(pic_path);
                        }
                        else
                        {
                            img.Source = pic("Bloons Pic/No Icon.png");
                        }
                    }
                    
                    for (i = start_no; i <= end_no && i <= list.Count; i++)
                    {
                        bloon_compare bc = list[i - 1];
                        show_bc(bc);
                    }

                    visibility_transfer((Rectangle)FindName("ext_debut_camo_react"),
                        debut_show_type_onlycamo);
                    visibility_transfer((Rectangle)FindName("ext_debut_sort_react"),
                        debut_show_type_roundsort);
                }
                else
                {
                    debut_text2.Text = "（无属性）";
                    debut_text2.Foreground = C(0, 255, 0);
                    create_bloon_debuts();

                    int i = 1;
                    foreach (KeyValuePair<string, Tuple<int, int>> pair in bloon_debuts)
                    {
                        if (i >= start_no && i <= end_no)
                        {
                            string b_name = pair.Key;
                            Tuple<int, int> tuple = pair.Value;
                            int round = tuple.Item1;
                            int count = tuple.Item2;

                            int index = i - start_no;
                            int m = index / debut_col;
                            int n = index % debut_col;
                            string postfix = m + "_" + n;

                            TextBlock t;

                            t = (TextBlock)FindName("ext_debut_no_" + postfix);
                            t.Text = "No." + i + "/" + bloon_debuts.Count;

                            Image img = (Image)FindName("ext_debut_pic_" + postfix + "_img");
                            string pic_name = b_name;
                            try_remove_number(ref pic_name);
                            string pic_path = "Bloons Pic/" + pic_name + ".png";
                            if (File.Exists(pic_path))
                            {
                                img.Source = pic(pic_path);
                            }
                            else
                            {
                                img.Source = pic("Bloons Pic/No Icon.png");
                            }

                            t = (TextBlock)FindName("ext_debut_name_" + postfix);
                            t.Text = b_name;

                            t = (TextBlock)FindName("ext_debut_round_" + postfix);
                            t.Text = "首次出现：" + round + " 回合";
                            if (round == -1)
                            {
                                t.Text = "未出现";
                            }

                            t = (TextBlock)FindName("ext_debut_count_" + postfix);
                            t.Text = "数量：" + count;
                            if (round == -1)
                            {
                                t.Text = "";
                            }
                        }
                        i++;
                    }
                }
                #endregion debut
                #region cash
                visibility_transfer((Rectangle)FindName("ext_cash_switch_react"),
                    cash_show_cash);
                visibility_transfer((Grid)FindName("ext_cash_cash_grid"), cash_show_cash);
                for (int i = 0; i < cash_row; i++)
                {
                    Grid btn = (Grid)FindName("ext_cash_del_" + i + "_grid");
                    btn.Visibility = Visibility.Hidden;
                }
                TextBlock cash_text = (TextBlock)FindName("ext_cash_top_text");
                if (cash_show_cash)
                {
                    cash_text.Text = "回合金钱倍率设置器";
                    cash_text.Foreground = C(255, 255, 0);

                    round_cash_mul_attrs = get_round_cash_mul_attrs();
                    int count = round_cash_mul_attrs.Count;
                    cash_page_max = count / cash_row + 1;
                    if (cash_page > cash_page_max)
                    {
                        cash_page = cash_page_max;
                    }

                    for (int i = 0; i < cash_row; i++)
                    {
                        int index = i + (cash_page - 1) * cash_row - 1;
                        int r_index = count - 1 - index;
                        TextBlock t = (TextBlock)FindName("ext_cash_text1_" + i);
                        TextBlock t2 = (TextBlock)FindName("ext_cash_text2_" + i);
                        TextBlock t3 = (TextBlock)FindName("ext_cash_text3_" + i);
                        t3.Visibility = Visibility.Hidden;
                        if (r_index < 0)
                        {
                            t.Text = "";
                            t2.Text = "";
                        }
                        else if (r_index == count)
                        {
                            int next = 1;
                            if (r_index > 0)
                            {
                                next = round_cash_mul_attrs[r_index - 1].Item1;
                            }
                            t.Text = "回合1～" + next;
                            if (count == 0)
                            {
                                t.Text = "回合1～";
                            }

                            t2.Text = "金钱倍率：1";
                        }
                        else if (r_index == 0)
                        {
                            int curr = round_cash_mul_attrs[r_index].Item1 + 1;
                            t.Text = "回合" + curr + "～";

                            double value = round_cash_mul_attrs[r_index].Item2;
                            t2.Text = "金钱倍率：" + double2str(value);

                            Grid btn = (Grid)FindName("ext_cash_del_" + i + "_grid");
                            btn.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            int next = 1;
                            if (r_index > 0)
                            {
                                next = round_cash_mul_attrs[r_index - 1].Item1;
                            }
                            int curr = round_cash_mul_attrs[r_index].Item1 + 1;
                            t.Text = "回合" + curr + "～" + next;

                            double value = 1;
                            if (r_index > 0)
                            {
                                value = round_cash_mul_attrs[r_index].Item2;
                            }
                            t2.Text = "金钱倍率：" + double2str(value);

                            Grid btn = (Grid)FindName("ext_cash_del_" + i + "_grid");
                            btn.Visibility = Visibility.Visible;
                        }
                    }
                }
                else
                {
                    cash_text.Text = "属性浏览器";
                    cash_text.Foreground = C(0, 255, 255);

                    var list = get_attr_choices();
                    int count = list.Count;
                    cash_page_max = (count - 1) / cash_row + 1;
                    if (cash_page > cash_page_max)
                    {
                        cash_page = cash_page_max;
                    }

                    for (int i = 0; i < cash_row; i++)
                    {
                        int index = i + (cash_page - 1) * cash_row;
                        TextBlock t = (TextBlock)FindName("ext_cash_text1_" + i);
                        TextBlock t2 = (TextBlock)FindName("ext_cash_text2_" + i);
                        TextBlock t3 = (TextBlock)FindName("ext_cash_text3_" + i);
                        if (index >= count)
                        {
                            t.Text = "";
                            t2.Text = "";
                            t3.Text = "";
                        }
                        else
                        {
                            t.Text = "No." + (index + 1) + " 属性名：" + list[index];

                            double value = attrs[list[index]];
                            t2.Text = "属性值：" + double2str(value);
                            t3.Visibility = Visibility.Visible;
                            t3.Text = attrs_inf[list[index]];
                        }
                    }

                }
                TextBlock cash_page_text = (TextBlock)FindName("ext_cash_page_text");
                cash_page_text.Text = "第 " + cash_page + " / " + cash_page_max + " 页";
                #endregion cash
                #region hint
                T("ext_hint_top_text").Text = "提示编辑器 (回合 " + curr_hint + " 结束语)";
                T("ext_hint_hintA").Text = "提示：这是第" + curr_hint + "回合的结束语，" +
                    "当你连续游玩时，它也可被视为第" + (curr_hint + 1) + "回合的介绍。";
                T("ext_hint_hintB").Text = "提示：若游戏从此回合（第" + curr_hint +"回合）开始，" +
                    "则它也会在游戏开始时显示一次，" +
                    "但重玩时不再次显示";

                try
                {
                    byte xr = byte.Parse(BOX("ext_hint_r").Text);
                    byte xg = byte.Parse(BOX("ext_hint_g").Text);
                    byte xb = byte.Parse(BOX("ext_hint_b").Text);
                    byte xa = byte.Parse(BOX("ext_hint_a").Text);
                    R("ext_hint_color").Fill = C(xa, xr, xg, xb);
                }
                catch
                {

                }
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
                    hint_text.Text = "第 " + curr_hint + " 回合没有结束提示，若需要，请提交一个";
                    hint_text.Foreground = C(255, 255, 0);
                }
                //ext_hint_top_text
                #endregion hint
            }
            #endregion ext
        }
    }
}
