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

        private void button_effect_r(string code)
        {
            string[] parts = code.Split('_');
            TextBox box;

            if (code.Contains("round_select_grid"))
            {
                ext_open(2);
                TextBlock t = (TextBlock)FindName(code + "_text");
                int round = int.Parse(t.Text);
                curr_hint = round;
            }
            if (code == "bloons_round")
            {
                ext_open(2);
                if (curr_round > 1)
                {
                    curr_hint = curr_round - 1;
                }
                else
                {
                    curr_hint = curr_round;
                }
            }
            if (code.Contains("bloons_copy"))
            {
                int i = Convert.ToInt32(parts[2]);
                if (i == bloon_modifying)
                {
                    bloon_modifying = int.MinValue / 2;
                }
                else
                {
                    bloon_modifying = i;
                    button_effect(code);
                }
            }
            if (code == "control_attr_confirm")
            {
                string select = get_sel_str("control_attr_select_select");
                attrs[select] = attrs_default[select];
                update_stat_input();
                modifyed = true;
            }
            if (code == "control_auto_save")
            {
                box = (TextBox)FindName("control_pos_box");
                if (box_ok(box))
                {
                    auto_save = !auto_save;
                    last_code = "control_load_game";
                    button_effect("control_load_game");
                }
                else
                {
                    float_default(10, 10, "无法找到游戏，设置失败！请检查\"文件存取\"中游戏的路径！",
                        C(255, 255, 255), C(255, 0, 0), 11, 4000);
                }
            }
            if (code == "ext_hint_commit")
            {
                if (hints.ContainsKey(curr_hint))
                {
                    var tuple = hints[curr_hint];
                    var argb = tuple.Item1;
                    BOX("ext_hint_input").Text = tuple.Item2;
                    BOX("ext_hint_r").Text = argb.r.ToString();
                    BOX("ext_hint_g").Text = argb.g.ToString();
                    BOX("ext_hint_b").Text = argb.b.ToString();
                    BOX("ext_hint_a").Text = argb.a.ToString();
                }
            }
            #region batch
            if (code == "control_redbloon")
            {
                if (last_code != code)
                {
                    MessageBox.Show("确定要将所有回合置为1个红气球吗？若需要，请再次点击此按钮", "提示");
                    last_code = code;
                }
                else
                {
                    for (int i = 1; i < 141; i++)
                    {
                        round r = rounds[i];
                        r.bloon_entrys.Clear();
                        r.time_max = 0;
                        r.add_entry(new bloon_entry("Red", 0, 1000, 1));
                    }
                    MessageBox.Show("设置成功！", "提示");
                    last_code = "";
                    modifyed = true;
                }
            }
            if (code == "control_only" && control_create_valid)
            {
                if (last_code != code)
                {
                    MessageBox.Show("确定要将所有回合置为当前的气球吗？若需要，请再次点击此按钮", "提示");
                    last_code = code;
                }
                else
                {
                    for (int i = 1; i < 141; i++)
                    {
                        round r = rounds[i];
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
                    }
                    bloon_modifying = int.MinValue / 2;
                    bloon_no_base = 1;
                    modifyed = true;
                    MessageBox.Show("设置成功！", "提示");
                    last_code = "";
                }
            }
            if (code == "control_copyround")
            {
                save_curr_round_to_clip(true);
            }
            if (code == "control_pasteround")
            {
                load_clip_to_curr_round(true);
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
                        swap_hint(round, curr_round);
                        modifyed = true;
                    }
                }
            }
            if (code == "control_batch_count_span")
            {
                box = BOX("control_batch_count_input");
                if (box_ok(box))
                {
                    if (last_code != code)
                    {
                        MessageBox.Show("确定要改变所有回合吗？若需要，请再次点击此按钮", "提示");
                        last_code = code;
                    }
                    else
                    {
                        double mul = double.Parse(box.Text);
                        for (int i = 1; i <= 140; i++)
                        {
                            round r = rounds[i];
                            foreach (bloon_entry b in r.bloon_entrys)
                            {
                                int count = b.count;
                                count = (int)decimal.Ceiling((decimal)(b.count * mul));
                                b.count = count;
                            }
                        }
                        last_code = "";
                    }
                }
            }
            if (code == "control_batch_count_time")
            {
                box = BOX("control_batch_count_input");
                if (box_ok(box))
                {
                    if (last_code != code)
                    {
                        MessageBox.Show("确定要改变所有回合吗？若需要，请再次点击此按钮", "提示");
                        last_code = code;
                    }
                    else
                    {
                        double mul = double.Parse(box.Text);
                        for (int i = 1; i <= 140; i++)
                        {
                            round r = rounds[i];
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
                        }
                        modifyed = true;
                        last_code = "";
                    }
                }
            }
            if (code == "control_batch_span_count")
            {
                box = BOX("control_batch_span_input");
                if (box_ok(box))
                {
                    if (last_code != code)
                    {
                        MessageBox.Show("确定要改变所有回合吗？若需要，请再次点击此按钮", "提示");
                        last_code = code;
                    }
                    else
                    {
                        double mul = double.Parse(box.Text);
                        for (int i = 1; i <= 140; i++)
                        {
                            round r = rounds[i];
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
                        }
                        last_code = "";
                    }
                }
            }
            if (code == "control_batch_span_time")
            {
                box = BOX("control_batch_span_input");
                if (box_ok(box))
                {
                    if (last_code != code)
                    {
                        MessageBox.Show("确定要改变所有回合吗？若需要，请再次点击此按钮", "提示");
                        last_code = code;
                    }
                    else
                    {
                        double mul = double.Parse(box.Text);
                        for (int i = 1; i <= 140; i++)
                        {
                            round r = rounds[i];
                            foreach (bloon_entry b in r.bloon_entrys)
                            {
                                b.start *= mul;
                                b.end *= mul;
                            }
                            r.time_max *= mul;
                        }
                        modifyed = true;
                        last_code = "";
                    }
                }
            }
            if (code == "control_batch_expand")
            {
                box = BOX("control_batch_expand_input");
                if (box_ok(box))
                {
                    int mul = int.Parse(box.Text);
                    if (last_code != code)
                    {
                        MessageBox.Show("确定要" + mul + "线扩展所有回合吗？" +
                            "若需要，请再次点击此按钮", "提示");
                        last_code = code;
                    }
                    else
                    {
                        for (int i = 1; i <= 140; i++)
                        {
                            expand_round(i, mul);
                        }
                        if (!double.IsNaN(attrs["start cash"]))
                        {
                            attrs["start cash"] *= Math.Pow(mul, 0.7);
                        }
                        attrs["cash multiplier[pop]"] /= Math.Pow(mul, 0.3);
                        attrs["cash multiplier[end round]"] /= Math.Pow(mul, 0.3);
                        modifyed = true;
                        last_code = "";
                    }
                }
            }
            #endregion batch
            bloon_show();
        }
    }
}
