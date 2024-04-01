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

        bool control_create_valid = true;
        bool control_cal_valid = true;
        bool control_cash_valid = true;
        bool control_round_valid = true;
        double control_cash = 650;
        int control_round = 1;
        private void Box_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox box = sender as TextBox;
            string name = box.Name;

            if (name.Contains("control_input"))
            {
                control_create_valid = false;

                string curr = name.Split('_')[2];
                box.Background = getSCB(Color.FromRgb(200, 0, 0));
                box.Foreground = getSCB(Color.FromRgb(255, 255, 255));

                string translate(string t)
                {
                    switch (t)
                    {
                        case "结束时间":
                            return "end";
                        case "间隔时间":
                            return "interval";
                        case "数量":
                            return "count";
                    }
                    return "";
                }

                string locked = get_sel_str("control_locker_select");
                string[] parts = locked.Split('>');
                string first = translate(parts[0]);
                string second = translate(parts[1]);

                double d;
                int i;
                bool success = false;
                if (name.Contains("control_input_count"))
                {
                    if (int.TryParse(box.Text, out i))
                    {
                        if (i >= 1 && i <= 999999)
                        {
                            box.Background = getSCB(Color.FromRgb(0, 255, 0));
                            box.Foreground = getSCB(Color.FromRgb(0, 0, 0));
                            success = true;
                        }
                    }
                }
                else
                {
                    if (double.TryParse(box.Text, out d))
                    {
                        if (d >= 0)
                        {
                            box.Background = getSCB(Color.FromRgb(0, 255, 0));
                            box.Foreground = getSCB(Color.FromRgb(0, 0, 0));
                            success = true;
                        }
                    }
                }
                if (success)
                {
                    string modify;
                    if (first != curr)
                    {
                        modify = first;
                    }
                    else
                    {
                        modify = second;
                    }
                    //start + (n - 1)interval = end

                    double start;
                    double end;
                    double interval;
                    int count;
                    TextBox box_start = (TextBox)FindName("control_input_start_box");
                    TextBox box_end = (TextBox)FindName("control_input_end_box");
                    TextBox box_interval = (TextBox)FindName("control_input_interval_box");
                    TextBox box_count = (TextBox)FindName("control_input_count_box");
                    box_start.TextChanged -= Box_TextChanged;
                    box_end.TextChanged -= Box_TextChanged;
                    box_interval.TextChanged -= Box_TextChanged;
                    box_count.TextChanged -= Box_TextChanged;
                    if (double.TryParse(box_start.Text, out start) &&
                        (double.TryParse(box_end.Text, out end) || modify == "end") &&
                        (double.TryParse(box_interval.Text, out interval) || modify == "interval") &&
                        (int.TryParse(box_count.Text, out count) || modify == "count"))
                    {
                        control_create_valid = true;
                        /*
                        if (start > end)
                        {
                            end = start;
                            box_end.Text = end.ToString("f2");
                        }*/
                        if (modify == "end")
                        {
                            end = start + (count - 1) * interval;
                            box_end.Text = end.ToString("f2");
                            box_change(box_end, end >= 0);
                            control_create_valid &= end >= 0;
                        }
                        if (modify == "interval")
                        {
                            if (count == 1)
                            {
                                interval = 0;
                            }
                            else
                            {
                                interval = (end - start) / (count - 1);
                            }
                            if (end < start)
                            {
                                interval = -1;
                            }
                            box_interval.Text = interval.ToString("f2");
                            box_change(box_interval, interval >= 0);
                            control_create_valid &= interval >= 0;
                        }
                        if (modify == "count")
                        {
                            if (interval == 0)
                            {
                                if (end == start)
                                {
                                    count = 1;
                                }
                                else
                                {
                                    count = -1;
                                }
                            }
                            else
                            {
                                count = (int)((end - start) / interval) + 1;
                            }

                            if (count >= 1)
                            {
                                box_count.Text = count.ToString();
                                box_count.Background = getSCB(Color.FromRgb(0, 255, 0));
                                box_count.Foreground = getSCB(Color.FromRgb(0, 0, 0));
                            }
                            else
                            {
                                control_create_valid = false;
                                box_count.Text = "∞";
                                box_count.Background = getSCB(Color.FromRgb(200, 0, 0));
                                box_count.Foreground = getSCB(Color.FromRgb(255, 255, 255));
                            }
                            if (count >= 1000000)
                            {
                                control_create_valid = false;
                                box_count.Text = count.ToString();
                                box_count.Background = getSCB(Color.FromRgb(200, 0, 0));
                                box_count.Foreground = getSCB(Color.FromRgb(255, 255, 255));
                            }
                        }
                        editing_entry.start = start;
                        editing_entry.end = end;
                        editing_entry.count = count;
                        /*input_start = start;
                        input_end = end;
                        input_interval = interval;
                        input_count = count;*/
                    }
                    box_start.TextChanged += Box_TextChanged;
                    box_end.TextChanged += Box_TextChanged;
                    box_interval.TextChanged += Box_TextChanged;
                    box_count.TextChanged += Box_TextChanged;
                }
            }
            if (name == "control_cash_box")
            {
                control_cash_valid = false;

                box.Background = getSCB(Color.FromRgb(200, 0, 0));
                box.Foreground = getSCB(Color.FromRgb(255, 255, 255));
                if (double.TryParse(box.Text, out control_cash))
                {
                    if (control_cash >= 0 && control_cash <= 1e9)
                    {
                        control_cash_valid = true;
                        box.Background = getSCB(Color.FromRgb(0, 255, 0));
                        box.Foreground = getSCB(Color.FromRgb(0, 0, 0));
                    }
                }
            }
            if (name == "control_round_box")
            {
                control_round_valid = false;

                box.Background = getSCB(Color.FromRgb(200, 0, 0));
                box.Foreground = getSCB(Color.FromRgb(255, 255, 255));
                if (int.TryParse(box.Text, out control_round))
                {
                    if (control_round >= 1 && control_round <= 140)
                    {
                        control_round_valid = true;
                        box.Background = getSCB(Color.FromRgb(0, 255, 0));
                        box.Foreground = getSCB(Color.FromRgb(0, 0, 0));
                    }
                }
            }
            control_cal_valid = control_cash_valid && control_round_valid;

            if (name == "control_pos_box")
            {
                bool good = true;
                if (!Directory.Exists(box.Text))
                {
                    good = false;
                }
                if (!File.Exists(box.Text + "/BloonsTD6.exe"))
                {
                    good = false;
                }
                if (!Directory.Exists(box.Text + "/Mods"))
                {
                    good = false;
                }
                if (!File.Exists(box.Text + "/Mods/DIY Rounds.dll"))
                {
                    good = false;
                }
                if (!Directory.Exists(box.Text + "/Mods/DIY Rounds"))
                {
                    good = false;
                }
                box_change(box, good);
                if (good)
                {
                    string filepath = "./Configs/GamePath.txt";
                    if (File.Exists(filepath))
                    {
                        File.Delete(filepath);
                    }
                    FileStream a = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write);
                    tool.fs_write(a, box.Text);
                    a.Close();
                }
            }
            if (name == "control_batch_switch_input_box")
            {
                bool b = false;
                int round = 0;
                if (int.TryParse(box.Text, out round))
                {
                    if (round >= 1 && round <= 140)
                    {
                        b = true;
                    }
                }
                box_change(box, b);
            }
            if (name == "control_batch_count_input_box" ||
                name == "control_batch_span_input_box")
            {
                bool b = false;
                double mul = 0;
                if (double.TryParse(box.Text, out mul))
                {
                    if (mul > 0 && mul <= 10000)
                    {
                        b = true;
                    }
                }
                box_change(box, b);
            }
            if (name == "control_batch_expand_input_box")
            {
                bool b = false;
                int mul = 0;
                if (int.TryParse(box.Text, out mul))
                {
                    if (mul <= 10 && mul >= 2)
                    {
                        b = true;
                    }
                }
                box_change(box, b);
            }
            if (name == "control_attr_input_box")
            {
                string select = get_sel_str("control_attr_select_select");

                bool b = false;
                if (double.IsNaN(attrs_default[select]) &&
                    (box.Text == "自动" || box.Text == "默认" ||
                    box.Text.ToLower() == "auto" || box.Text.ToLower() == "default"))
                {
                    b = true;
                }
                else
                {
                    int ivalue;
                    double dvalue;
                    if (int.TryParse(box.Text, out ivalue))
                    {
                        if (select == "start cash" && ivalue >= 0 && ivalue <= 1000000000)
                        {
                            b = true;
                        }
                        if ((select == "start round" || select == "end round")
                            && ivalue >= 1 && ivalue <= 140)
                        {
                            if (select == "start round" && ivalue <= get_end_round() ||
                                select == "end round" && ivalue >= get_start_round())
                            {
                                b = true;
                            }
                        }
                        if ((select == "player health" || select == "player max health")
                            && ivalue >= 1)
                        {
                            b = true;
                        }
                        if (select == "HEALTHY[add]" && ivalue >= 0 && ivalue <= 1000000000)
                        {
                            b = true;
                        }
                    }
                    if (double.TryParse(box.Text, out dvalue))
                    {
                        if (select.Contains("cash multiplier") && dvalue >= 0 && dvalue <= 1e6)
                        {
                            if (select.Contains("sell"))
                            {
                                if (dvalue <= 0.95)
                                {
                                    b = true;
                                }
                            }
                            else
                            {
                                b = true;
                            }
                        }
                        if (select.Contains("tower") &&
                            select.Contains("multiplier") && dvalue > 0 && dvalue <= 1e6)
                        {
                            b = true;
                            if (select.Contains("attack speed") && dvalue > 100)
                            {
                                b = false;
                            }
                            if (select.Contains("range") && dvalue > 10)
                            {
                                b = false;
                            }
                        }
                        if (select == "HEALTHY[mul]" && dvalue >= 0 && dvalue <= 1000000000)
                        {
                            b = true;
                        }
                        if (select == "AUTOFARM[mul]" && dvalue >= 0 && dvalue <= 1000000000)
                        {
                            b = true;
                        }
                        if (select == "TIMETAX[/s]" && dvalue >= 0 && dvalue <= 1000000000)
                        {
                            b = true;
                        }
                        if (select == "TIMETAX[mul]" && dvalue >= 0 && dvalue < 1)
                        {
                            b = true;
                        }
                    }
                }
                box_change(box, b);
            }

            if (name == "ext_cash_input_int_box")
            {
                bool b = false;
                int round = 0;
                if (int.TryParse(box.Text, out round))
                {
                    if (round >= 2 && round <= 141)
                    {
                        b = true;
                    }
                }
                box_change(box, b);
            }
            if (name == "ext_cash_input_double_box")
            {
                bool b = false;
                double mul = 0;
                if (double.TryParse(box.Text, out mul))
                {
                    if (mul >= 0)
                    {
                        b = true;
                    }
                }
                box_change(box, b);
            }
            if (name == "ext_hint_r_box" ||
                name == "ext_hint_g_box" ||
                name == "ext_hint_b_box" ||
                name == "ext_hint_a_box")
            {
                bool b = false;
                int color = 0;
                if (int.TryParse(box.Text, out color))
                {
                    if (color >= 0 && color <= 255)
                    {
                        b = true;
                    }
                }
                box_change(box, b);
            }
            bloon_show();
        }

        private void box_change(TextBox box, bool b)
        {
            if (b)
            {
                box.Background = getSCB(Color.FromRgb(0, 255, 0));
                box.Foreground = getSCB(Color.FromRgb(0, 0, 0));
            }
            else
            {
                box.Background = getSCB(Color.FromRgb(200, 0, 0));
                box.Foreground = getSCB(Color.FromRgb(255, 255, 255));
            }
        }

        private bool box_ok(TextBox box)
        {
            byte r = ((SolidColorBrush)(box.Foreground)).Color.R;
            return r == 0;
        }
    }
}
