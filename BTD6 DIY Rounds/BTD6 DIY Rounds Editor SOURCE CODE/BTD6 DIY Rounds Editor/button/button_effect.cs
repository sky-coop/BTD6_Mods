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
        private void button_effect(string code)
        {
            string[] parts = code.Split('_');
            if (code == "wrap_btn")
            {
                wrap();
            }
            if (code.Contains("round_select_grid"))
            {
                int i = Convert.ToInt32(parts[3]);
                int j = Convert.ToInt32(parts[4]);
                round_select(i * page_j + j + (current_page - 1) * page_i * page_j + 1);
            }
            if (code.Contains("round_grid_change_down"))
            {
                if (current_page == 1)
                {
                    return;
                }
                else
                {
                    round_unselect();
                    page_shift(-1);
                    current_page--;
                    TextBlock t = (TextBlock)FindName("round_grid_change_text");
                    t.Text = "第 " + current_page + " / " + page_max + " 页";
                }
            }
            if (code.Contains("round_grid_change_up"))
            {
                if (current_page == page_max)
                {
                    return;
                }
                else
                {
                    round_unselect();
                    page_shift(1);
                    current_page++;
                    TextBlock t = (TextBlock)FindName("round_grid_change_text");
                    t.Text = "第 " + current_page + " / " + page_max + " 页";
                }
            }
            if (code == "prev_round")
            {
                if (curr_round == 1)
                {
                    return;
                }
                if ((curr_round - 1) % (page_i * page_j) == 0)
                {
                    button_effect("round_grid_change_down");
                }
                round_select(curr_round - 1);
                bloon_no_base = 1;
            }
            if (code == "next_round")
            {
                if (curr_round == 140)
                {
                    return;
                }
                if ((curr_round + 1) > (current_page * page_i * page_j) &&
                    curr_round % (page_i * page_j) == 0)
                {
                    button_effect("round_grid_change_up");
                }
                round_select(curr_round + 1);
                bloon_no_base = 1;
            }
            if (code == "show")
            {
                stats_show = !stats_show;
            }
            /*
            if (code.Contains("bloons_img_grid"))
            {
                int i = Convert.ToInt32(parts[3]);
                Rectangle react;
                for (int k = 0; k < bloon_row; k++)
                {
                    react = (Rectangle)FindName("bloons_img_" + k + "_react");
                    react.Fill = C(127, 0, 255, 0);
                }
                react = (Rectangle)FindName("bloons_img_" + i + "_react");
                react.Fill = C(127, 0, 255, 0);
                bloon_no_selected = bloon_no_base + i;
            }*/
            if (code.Contains("bloons_img_stat"))
            {
                int i = Convert.ToInt32(parts[3]);
                int pi = Convert.ToInt32(parts[4]);
                int pj = Convert.ToInt32(parts[5]);
                round cr = rounds[curr_round];
                bloon_entry be = cr.bloon_entrys[i + bloon_no_base - 1];
                switch (pi * 4 + pj)
                {
                    case 0:
                        be.camo = !be.camo;
                        break;
                    case 1:
                        be.regrow = !be.regrow;
                        break;
                    case 2:
                        be.fort = !be.fort;
                        break;
                    case 3:
                        be.elite = !be.elite;
                        break;
                    case 4:
                        be.lead = !be.lead;
                        break;
                    case 5:
                        be.purple = !be.purple;
                        break;
                    case 6:
                        be.zebra = !be.zebra;
                        break;
                    case 7:
                        string name = be.bloon_name;
                        try_remove_number(ref name);
                        name += be.level % 5 + 1;
                        be.bloon_name = name;
                        break;
                    default:
                        break;
                }
                modifyed = true;
            }
            if (code.Contains("bloons_copy"))
            {
                int i = Convert.ToInt32(parts[2]);
                round cr = rounds[curr_round];
                bloon_entry entry = cr.bloon_entrys[i + bloon_no_base - 1];

                editing_entry.regrow = entry.regrow;
                editing_entry.fort = entry.fort;
                editing_entry.camo = entry.camo;
                editing_entry.lead = entry.lead;
                editing_entry.purple = entry.purple;
                editing_entry.zebra = entry.zebra;
                editing_entry.elite = entry.elite;

                TextBox box_start = (TextBox)FindName("control_input_start_box");
                TextBox box_end = (TextBox)FindName("control_input_end_box");
                TextBox box_interval = (TextBox)FindName("control_input_interval_box");
                TextBox box_count = (TextBox)FindName("control_input_count_box");
                box_start.TextChanged -= Box_TextChanged;
                box_end.TextChanged -= Box_TextChanged;
                box_interval.TextChanged -= Box_TextChanged;
                box_count.TextChanged -= Box_TextChanged;

                control_create_valid = true;

                box_start.Text = entry.start.ToString("f2");
                editing_entry.start = entry.start;
                box_change(box_start, entry.start >= 0);

                box_end.Text = entry.end.ToString("f2");
                editing_entry.end = entry.end;
                box_change(box_end, entry.end >= 0);

                double interval;
                if (entry.count == 1)
                {
                    interval = 0;
                }
                else
                {
                    interval = (entry.end - entry.start) / (entry.count - 1);
                }
                if (entry.end < entry.start)
                {
                    interval = -1;
                }
                box_interval.Text = interval.ToString("f2");
                box_change(box_interval, interval >= 0);

                box_count.Text = entry.count.ToString();
                editing_entry.count = entry.count;
                box_change(box_count, entry.count >= 1);

                box_start.TextChanged += Box_TextChanged;
                box_end.TextChanged += Box_TextChanged;
                box_interval.TextChanged += Box_TextChanged;
                box_count.TextChanged += Box_TextChanged;

                ComboBox group_cb = (ComboBox)FindName("control_group_cb_select");
                string group = find_group(entry.bloon_name);
                foreach(ComboBoxItem s in group_cb.Items)
                {
                    if((string)s.Content == group)
                    {
                        group_cb.SelectedItem = s;
                    }
                }

                ComboBox bloon_cb = (ComboBox)FindName("control_bloon_cb_select");
                string bloon = entry.bloon_name;
                foreach (ComboBoxItem s in bloon_cb.Items)
                {
                    if ((string)s.Content == bloon)
                    {
                        bloon_cb.SelectedItem = s;
                    }
                }
            }
            if (code.Contains("bloons_delete"))
            {
                int i = Convert.ToInt32(parts[2]);
                round cr = rounds[curr_round];
                if (cr.bloon_entrys.Count > 1)
                {
                    cr.delete_entry(i + bloon_no_base - 1);
                    if (bloon_modifying == i)
                    {
                        bloon_modifying = int.MinValue / 2;
                    }
                    if (bloon_modifying > i)
                    {
                        bloon_modifying--;
                    }
                    modifyed = true;
                }
            }
            if (code.Contains("bloons_exchange_up"))
            {
                int i = Convert.ToInt32(parts[3]);
                round cr = rounds[curr_round];
                if (i >= 1)
                {
                    bloon_entry temp = cr.bloon_entrys[i + bloon_no_base - 1 - 1];
                    cr.bloon_entrys[i + bloon_no_base - 1 - 1] = cr.bloon_entrys[i + bloon_no_base - 1];
                    cr.bloon_entrys[i + bloon_no_base - 1] = temp;
                    if (bloon_modifying == i)
                    {
                        bloon_modifying--;
                    }
                    else if (bloon_modifying == i - 1)
                    {
                        bloon_modifying++;
                    }
                }
                modifyed = true;
            }
            if (code.Contains("bloons_exchange_down"))
            {
                int i = Convert.ToInt32(parts[3]);
                round cr = rounds[curr_round];
                if (i < cr.bloon_entrys.Count - 1)
                {
                    bloon_entry temp = cr.bloon_entrys[i + bloon_no_base - 1 + 1];
                    cr.bloon_entrys[i + bloon_no_base - 1 + 1] = cr.bloon_entrys[i + bloon_no_base - 1];
                    cr.bloon_entrys[i + bloon_no_base - 1] = temp;
                    if (bloon_modifying == i)
                    {
                        bloon_modifying++;
                    }
                    else if (bloon_modifying == i + 1)
                    {
                        bloon_modifying--;
                    }
                }
                modifyed = true;
            }
            if (code.Contains("control_add") && control_create_valid)
            {
                if (editing_entry.end < editing_entry.start)
                {
                    return;
                }
                round cr = rounds[curr_round];
                string name = get_sel_str("control_bloon_cb_select");

                bloon_entry be_new = new bloon_entry(
                        name, editing_entry.start, editing_entry.end, 
                        editing_entry.count);
                bloon_template bloon_Template = find_bt(name);
                if (bloon_Template.has_regrow)
                {
                    be_new.regrow = editing_entry.regrow;
                }
                if (bloon_Template.has_fort)
                {
                    be_new.fort = editing_entry.fort;
                }
                if (bloon_Template.has_camo)
                {
                    be_new.camo = editing_entry.camo;
                }
                if (bloon_Template.has_lead)
                {
                    be_new.lead = editing_entry.lead;
                }
                if (bloon_Template.has_purple)
                {
                    be_new.purple = editing_entry.purple;
                }
                if (bloon_Template.has_zebra)
                {
                    be_new.zebra = editing_entry.zebra;
                }
                if (bloon_Template.has_elite)
                {
                    be_new.elite = editing_entry.elite;
                }

                if (bloon_modifying > int.MinValue / 4)
                {
                    cr.bloon_entrys[bloon_modifying + bloon_no_base - 1] = be_new;
                    cr.time_max = 0;
                    for (int i = 0; i < cr.bloon_entrys.Count; i++)
                    {
                        cr.time_max = Math.Max(cr.time_max, cr.bloon_entrys[i].end);
                    }
                }
                else
                {
                    cr.add_entry(be_new);
                }
                modifyed = true;
            }
            if (code == "control_up")
            {
                if (bloon_no_base > 1)
                {
                    bloon_no_base--;
                    bloon_modifying++;
                }
            }
            if (code == "control_down")
            {
                round cr = rounds[curr_round];
                List<bloon_entry> b = cr.bloon_entrys;
                if (bloon_no_base + bloon_row <= b.Count)
                {
                    bloon_no_base++;
                    bloon_modifying--;
                }
            }
            if (code == "control_lineadd")
            {
                if (bloon_row < 7)
                {
                    bloon_row++;
                    visual_bloons_grid_init(bloon_row);
                }
            }
            if (code == "control_linesub")
            {
                if (bloon_row > 3)
                {
                    bloon_row--;
                    visual_bloons_grid_init(bloon_row);
                }
            }
            if (code == "control_pageiadd")
            {
                if (page_i < 20)
                {
                    page_i++;
                    visual_round_grid_init(page_i, page_j);
                }
            }
            if (code == "control_pageisub")
            {
                if (page_i > 5)
                {
                    page_i--;
                    visual_round_grid_init(page_i, page_j);
                }
            }
            if (code == "control_pagejadd")
            {
                if (page_j < 7)
                {
                    page_j++;
                    visual_round_grid_init(page_i, page_j);
                }
            }
            if (code == "control_pagejsub")
            {
                if (page_j > 2)
                {
                    page_j--;
                    visual_round_grid_init(page_i, page_j);
                }
            }
            if (code == "control_regrow")
            {
                editing_entry.regrow = !editing_entry.regrow;
            }
            if (code == "control_fort")
            {
                editing_entry.fort = !editing_entry.fort;
            }
            if (code == "control_camo")
            {
                editing_entry.camo = !editing_entry.camo;
            }
            if (code == "control_lead")
            {
                editing_entry.lead = !editing_entry.lead;
            }
            if (code == "control_purple")
            {
                editing_entry.purple = !editing_entry.purple;
            }
            if (code == "control_zebra")
            {
                editing_entry.zebra = !editing_entry.zebra;
            }
            if (code == "control_elite")
            {
                editing_entry.elite = !editing_entry.elite;
            }
            if (code.Contains("control_panel"))
            {
                ComboBox c = (ComboBox)FindName(code);
                for(int i = 0; i < c.Items.Count; i++)
                {
                    Grid g = (Grid)FindName("control_2_" + i + "_grid");
                    visibility_transfer(g, i == c.SelectedIndex);
                }
                control_2 = c.SelectedIndex;
            }


            TextBox box;
            if (code == "control_load_template")
            {
                box = (TextBox)FindName("control_template_box");
                string pos = "./Templates/" + box.Text + "/DIY Rounds";
                if (((SolidColorBrush)box.Background).Color.R == 200)
                {
                    return;
                }
                if (last_code != code)
                {
                    MessageBox.Show("再次点击此按钮以确认导入回合文件" + pos, "提示");
                    last_code = code;
                }
                else
                {
                    load_rounds(pos);
                    MessageBox.Show("读取成功！", "提示");
                    last_code = "";
                }
                modifyed = true;
            }
            if (code == "control_load_workspace")
            {
                box = (TextBox)FindName("control_load_box");
                string pos = "./Workspace/" + box.Text + "/DIY Rounds";
                if (((SolidColorBrush)box.Background).Color.R == 200)
                {
                    return;
                }
                if (last_code != code)
                {
                    MessageBox.Show("再次点击此按钮以确认导入回合文件" + pos, "提示");
                    last_code = code;
                }
                else
                {
                    load_rounds(pos);
                    MessageBox.Show("读取成功！", "提示");
                    last_code = "";
                    modifyed = true;
                }
            }
            if (code == "control_save_workspace")
            {
                box = (TextBox)FindName("control_save_box");
                string pos = "./Workspace/" + box.Text + "/DIY Rounds";
                if (last_code != code && exist_rounds(pos))
                {
                    MessageBox.Show("已存在回合文件夹" + pos + 
                        "，若要覆盖，再次点击此按钮", "提示");
                    last_code = code;
                }
                else
                {
                    if (save_rounds_workspace(box.Text))
                    {
                        if (!Directory.Exists("./Workspace/" + box.Text + "/pics"))
                        {
                            Directory.CreateDirectory("./Workspace/" + box.Text + "/pics");
                        }
                        if (save_shot)
                        {
                            print_screen(box.Text);
                        }
                        else
                        {
                            MessageBox.Show("已保存到工作文件夹！", "提示");
                        }
                    }
                    last_code = "";
                }
            }
            if (code == "control_explore")
            {
                try
                {
                    System.Diagnostics.Process.Start("Explorer.exe",
                        Environment.CurrentDirectory + "\\Workspace");
                }
                catch
                {

                }
            }

            if (code == "control_search")
            {
                TextBox control_pos_box = (TextBox)FindName("control_pos_box");
                string dir = control_pos_box.Text;

                OpenFileDialog openFileDialog = new OpenFileDialog(); 
                openFileDialog.Title = "找到BloonsTD6.exe";
                openFileDialog.Filter = "BloonsTD6|*.exe";
                
                if (Directory.Exists(dir))
                {
                    string init = "";
                    foreach(char c in dir)
                    {
                        if(c == '/')
                        {
                            init += "\\";
                        }
                        else
                        {
                            init += c;
                        }
                    }
                    openFileDialog.InitialDirectory = init;
                }
                if (openFileDialog.ShowDialog() == true)
                {
                    string fileName = openFileDialog.FileName;
                    control_pos_box.Text = Directory.GetParent(fileName).FullName;
                }
            }
            if (code == "control_save_game")
            {
                box = (TextBox)FindName("control_pos_box");
                if (box_ok(box))
                {
                    string folder = box.Text + "/Mods/DIY Rounds";
                    string filepath = folder + "/rounds.json";
                    if (last_code != code && exist_rounds(folder))
                    {
                        MessageBox.Show(folder + " 中已存在回合文件，若要覆盖，" +
                            "再次点击此按钮", "警告");
                        last_code = code;
                    }
                    else
                    {
                        if (save_rounds_game(folder))
                        {
                            if (!Directory.Exists(folder + "/pics"))
                            {
                                Directory.CreateDirectory(folder + "/pics");
                            }
                            MessageBox.Show("已保存到游戏！游戏的回合会进行更改", "提示");
                        }
                        last_code = "";
                    }
                }
            }
            if (code == "control_load_game")
            {
                box = (TextBox)FindName("control_pos_box");
                if (box_ok(box))
                {
                    string folder = box.Text + "/Mods/DIY Rounds";
                    string filepath = folder + "/rounds.json";
                    try
                    {
                        if (last_code != code && File.Exists(filepath))
                        {
                            MessageBox.Show("再次点击此按钮以确认导入游戏中的回合", "提示");
                            last_code = code;
                        }
                        else
                        {
                            load_rounds(folder);
                            MessageBox.Show("读取成功！", "提示");
                            last_code = "";
                        }
                    }
                    catch (Exception ex)
                    {
                        float_default(-300, -50, ex.Message, C(255, 0, 0), C(255, 255, 255), 11, 10000);
                    }
                }
            }
            #region batch
            if (code == "control_redbloon")
            {
                round r = rounds[curr_round];
                r.bloon_entrys.Clear();
                r.time_max = 0;
                r.add_entry(new bloon_entry("Red", 0, 1000, 1));
                modifyed = true;
            }
            if (code == "control_only" && control_create_valid)
            {
                round r = rounds[curr_round];
                r.bloon_entrys.Clear();
                r.time_max = 0;

                string name = get_sel_str("control_bloon_cb_select");

                bloon_entry be_new = new bloon_entry(
                        name, editing_entry.start, editing_entry.end,
                        editing_entry.count);
                bloon_template bloon_Template = find_bt(name);
                if (bloon_Template.has_regrow)
                {
                    be_new.regrow = editing_entry.regrow;
                }
                if (bloon_Template.has_fort)
                {
                    be_new.fort = editing_entry.fort;
                }
                if (bloon_Template.has_camo)
                {
                    be_new.camo = editing_entry.camo;
                }
                if (bloon_Template.has_lead)
                {
                    be_new.lead = editing_entry.lead;
                }
                if (bloon_Template.has_purple)
                {
                    be_new.purple = editing_entry.purple;
                }
                if (bloon_Template.has_zebra)
                {
                    be_new.zebra = editing_entry.zebra;
                }
                if (bloon_Template.has_elite)
                {
                    be_new.elite = editing_entry.elite;
                }

                r.add_entry(be_new);

                bloon_modifying = int.MinValue / 2;
                bloon_no_base = 1;
                modifyed = true;
            }
            if (code == "control_copyround")
            {
                save_curr_round_to_clip(false);
            }
            if (code == "control_pasteround")
            {
                load_clip_to_curr_round(false);
                modifyed = true;
            }
            if (code == "loading_cancel")
            {
                curr_round = round_store;
                loading = false;
            }
            if (code == "loading_confirm")
            {
                curr_round = round_store;
                loading = false;

                rounds[curr_round].bloon_entrys.Clear();
                rounds[curr_round].time_max = 0;
                foreach (var be in rounds[0].bloon_entrys)
                {
                    rounds[curr_round].add_entry(be);
                }
                rounds[0].bloon_entrys.Clear();
                if (cp_import.hint != null)
                {
                    hints[curr_round] = cp_import.hint;
                }

                modifyed = true;
            }
            if (code == "control_batch_switch")
            {
                box = BOX("control_batch_switch_input");
                if (box_ok(box))
                {
                    int round = int.Parse(box.Text);
                    if (round != curr_round)
                    {
                        swap_round(round, curr_round);
                        modifyed = true;
                    }
                }
            }
            if (code == "control_batch_count_span")
            {
                box = BOX("control_batch_count_input");
                if (box_ok(box))
                {
                    double mul = double.Parse(box.Text);
                    round r = rounds[curr_round];
                    foreach (bloon_entry b in r.bloon_entrys)
                    {
                        int count = b.count;
                        count = (int)decimal.Ceiling((decimal)(b.count * mul));
                        b.count = count;
                    }
                    modifyed = true;
                }
            }
            if (code == "control_batch_count_time")
            {
                box = BOX("control_batch_count_input");
                if (box_ok(box))
                {
                    double mul = double.Parse(box.Text);
                    round r = rounds[curr_round];
                    double max = 0;
                    foreach (bloon_entry b in r.bloon_entrys)
                    {
                        int count = b.count;
                        count = (int)decimal.Ceiling((decimal)(b.count * mul));
                        b.start *= (double)count / b.count;
                        b.end *= (double)count / b.count;
                        max = Math.Max(max, b.end);
                        b.count = count;
                    }
                    r.time_max = max;
                    modifyed = true;
                }
            }
            if (code == "control_batch_span_count")
            {
                box = BOX("control_batch_span_input");
                if (box_ok(box))
                {
                    double mul = double.Parse(box.Text);
                    round r = rounds[curr_round];
                    foreach (bloon_entry b in r.bloon_entrys)
                    {
                        if (b.count != 1)
                        {
                            double span = 0;
                            span = (b.end - b.start) / (b.count - 1);
                            span *= mul;
                            if(span != 0)
                            {
                                b.count = (int)decimal.Floor((decimal)(
                                    (b.end - b.start) / span + 1));
                            }
                        }
                    }
                    modifyed = true;
                }
            }
            if (code == "control_batch_span_time")
            {
                box = BOX("control_batch_span_input");
                if (box_ok(box))
                {
                    double mul = double.Parse(box.Text);
                    round r = rounds[curr_round];
                    foreach (bloon_entry b in r.bloon_entrys)
                    {
                        b.start *= mul;
                        b.end *= mul;
                    }
                    r.time_max *= mul;
                    modifyed = true;
                }
            }
            if (code == "control_batch_expand")
            {
                box = BOX("control_batch_expand_input");
                if (box_ok(box))
                {
                    int mul = int.Parse(box.Text);
                    expand_round(curr_round, mul);
                    modifyed = true;
                }
            }
            #endregion batch
            #region attr
            if (code.Contains("control_attr_select"))
            {
                box = (TextBox)FindName("control_attr_input_box");
                string select = get_sel_str("control_attr_select_select");
                double old_value = attrs[select];
                if (old_value == (int)old_value)
                {
                    box.Text = old_value.ToString("f0");
                }
                else
                {
                    box.Text = double2str(old_value);
                }
            }
            if (code == "control_attr_confirm")
            {
                box = (TextBox)FindName("control_attr_input_box");
                string select = get_sel_str("control_attr_select_select");
                if (box_ok(box))
                {
                    if (box.Text == "自动" || box.Text == "默认" ||
                        box.Text.ToLower() == "auto" || box.Text.ToLower() == "default")
                    {
                        attrs[select] = double.NaN;
                    }
                    else
                    {
                        double value = double.Parse(box.Text);
                        attrs[select] = value;
                        if (select == "start cash")
                        {
                            TextBox tb = (TextBox)FindName("control_cash_box");
                            tb.Text = box.Text;
                        }
                        if (select == "start round")
                        {
                            TextBox tb = (TextBox)FindName("control_round_box");
                            tb.Text = box.Text;
                        }
                        if (select == "player health")
                        {
                            int max = 5000;
                            double amax = attrs["player max health"];
                            if (!double.IsNaN(amax))
                            {
                                max = (int)amax;
                            }
                            if (max < value)
                            {
                                attrs["player max health"] = value;
                            }
                        }
                    }
                    modifyed = true;
                }
            }
            #endregion attr
            #region random
            if (code == "control_random_roll")
            {
                if (last_code != code)
                {
                    MessageBox.Show("这会覆盖所有东西，" +
                        "确定要进行生成吗？若需要，请再次点击此按钮", "提示");
                    last_code = code;
                }
                else
                {
                    roll(BOX("control_random_seed_input").Text);
                    modifyed = true;
                    last_code = "";
                }
            }
            #endregion random
            #region config
            if (code == "control_auto_save")
            {
                auto_save = !auto_save;
                modifyed = true;
            }
            if (code == "control_screenshot")
            {
                save_shot = !save_shot;
            }
            if (code == "control_wrapswitch")
            {
                wrap_switch();
            }
            #endregion config
            #region ext
            #region debut
            if (code == "control_debut")
            {
                ext_change(0);
            }
            if (code == "control_attr_advanced")
            {
                ext_change(1);
            }
            if (code == "bloons_round")
            {
                ext_open(2);
                curr_hint = curr_round;
            }

            if (code == "ext_debut_exit")
            {
                ext_close();
            }
            if (code == "ext_debut_switch")
            {
                debut_show_type = !debut_show_type;
                button_effect("debut_rollup");
            }
            if (code == "ext_debut_camo")
            {
                debut_show_type_onlycamo = !debut_show_type_onlycamo;
                button_effect("debut_rollup");
            }
            if (code == "ext_debut_sort")
            {
                debut_show_type_roundsort = !debut_show_type_roundsort;
                button_effect("debut_rollup");
            }
            if (code == "debut_up")
            {
                if (debut_row_base > 0)
                {
                    debut_row_base--;
                }
            }
            if (code == "debut_down")
            {
                int maxline = 0;
                if (debut_show_type)
                {
                    maxline = (int)decimal.Ceiling((decimal)get_debut_type_list().Count / debut_col);
                }
                else
                {
                    maxline = (int)decimal.Ceiling((decimal)bloon_debuts.Count / debut_col);
                }
                int maxbase = maxline - debut_row;
                if (debut_row_base < maxbase)
                {
                    debut_row_base++;
                }
            }
            if (code == "debut_rollup")
            {
                int maxline = 0;
                if (debut_show_type)
                {
                    create_type_debuts();
                    maxline = (int)decimal.Ceiling((decimal)get_debut_type_list().Count / debut_col);
                }
                else
                {
                    create_bloon_debuts();
                    maxline = (int)decimal.Ceiling((decimal)bloon_debuts.Count / debut_col);
                }
                int maxbase = maxline - debut_row;
                if (debut_row_base > maxbase)
                {
                    debut_row_base = maxbase;
                }
            }
            #endregion debut

            #region cash
            if (code == "ext_cash_exit")
            {
                ext_close();
            }
            if (code == "ext_cash_switch")
            {
                cash_show_cash = !cash_show_cash;
            }
            if (code == "ext_cash_page_up")
            {
                if (cash_page < cash_page_max)
                {
                    cash_page++;
                }
            }
            if (code == "ext_cash_page_down")
            {
                if (cash_page > 1)
                {
                    cash_page--;
                }
            }
            if (code.Contains("ext_cash_add"))
            {
                TextBox t1 = (TextBox)FindName("ext_cash_input_int_box");
                TextBox t2 = (TextBox)FindName("ext_cash_input_double_box");
                if (box_ok(t1) && box_ok(t2))
                {
                    int round = int.Parse(t1.Text) - 1;
                    double mul = double.Parse(t2.Text);
                    string s = "cash multiplier[pop][round after " + round + "]";
                    attrs[s] = mul;
                    modifyed = true;
                }
            }
            if (code.Contains("ext_cash_del"))
            {
                int i = Convert.ToInt32(parts[3]);
                int index = i + (cash_page - 1) * cash_row - 1;
                int r_index = round_cash_mul_attrs.Count - 1 - index;
                var tuple = round_cash_mul_attrs[r_index];
                int round = tuple.Item1;
                attrs.Remove("cash multiplier[pop][round after " + round + "]");
                modifyed = true;
            }
            if (code.Contains("ext_cash_default"))
            {
                List<string> temp = new List<string>();
                foreach (var pair in attrs)
                {
                    if (pair.Key.Contains("cash multiplier[pop][round after"))
                    {
                        temp.Add(pair.Key);
                    }
                }
                foreach (var v in temp)
                {
                    attrs.Remove(v);
                }
                foreach (var pair in attrs_default)
                {
                    if (pair.Key.Contains("cash multiplier[pop][round after"))
                    {
                        attrs[pair.Key] = pair.Value;
                    }
                }
                modifyed = true;
            }
            #endregion cash

            #region hint
            if (code == "ext_hint_exit")
            {
                ext_close();
            }
            if (code == "ext_hint_prev")
            {
                if (curr_hint > 1)
                {
                    curr_hint--;
                }
            }
            if (code == "ext_hint_next")
            {
                if (curr_hint < 140)
                {
                    curr_hint++;
                }
            }
            if (code == "ext_hint_default")
            {
                BOX("ext_hint_r").Text = "255";
                BOX("ext_hint_g").Text = "255";
                BOX("ext_hint_b").Text = "255";
                BOX("ext_hint_a").Text = "255";
            }
            else if (code == "ext_hint_commit")
            {
                TextBox tr = BOX("ext_hint_r");
                TextBox tg = BOX("ext_hint_g");
                TextBox tb = BOX("ext_hint_b");
                TextBox ta = BOX("ext_hint_a");
                if (box_ok(tr) && box_ok(tg) && box_ok(tb) && box_ok(ta))
                {
                    byte r = byte.Parse(BOX("ext_hint_r").Text);
                    byte g = byte.Parse(BOX("ext_hint_g").Text);
                    byte b = byte.Parse(BOX("ext_hint_b").Text);
                    byte a = byte.Parse(BOX("ext_hint_a").Text);
                    string text = BOX("ext_hint_input").Text;
                    hints[curr_hint] = new Tuple<ARGB, string>(
                        A(a, r, g, b), text);
                    modifyed = true;
                }
            }
            else if (code == "ext_hint_del")
            {
                if (hints.ContainsKey(curr_hint))
                {
                    hints.Remove(curr_hint);
                    modifyed = true;
                }
            }
            #endregion hint
            #endregion ext
            bloon_show();
        }
        string last_code = "";
        int control_2 = 0;

    }
}
