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
using System.Security.Policy;

namespace BTD6_DIY_Rounds
{
    public partial class MainWindow : Window
    {
        List<string> random_presets = new List<string>()
        {
            "平静",
            "正常",
            "激烈",
            "炸裂"
        };

        List<string> random_lengths = new List<string>()
        {
            "极短",
            "较短",
            "正常",
            "长",
            "全部"
        };

        List<string> random_regrows = new List<string>()
        {
            "无",
            "稀少",
            "正常",
            "较多",
            "全部"
        };

        List<string> random_camos = new List<string>()
        {
            "无",
            "稀少",
            "正常",
            "较多",
            "全部"
        };

        List<string> random_forts = new List<string>()
        {
            "无",
            "稀少",
            "正常",
            "较多",
            "全部"
        };

        List<string> random_earliers = new List<string>()
        {
            "低概率",
            "正常",
            "高概率",
            "无限制"
        };

        List<string> random_bosses = new List<string>()
        {
            "无",
            "普通",
            "精英",
        };

        List<string> random_attrs = new List<string>()
        {
            "正常",
            "复杂",
            "混沌",
        };

        List<string> random_difficulties = new List<string>()
        {
            "0", "1", "2", "3", "4", "5",
            "6", "7", "8", "9", "10",
        };

        //先决定界限 再决定概率
        //例如： NextDouble() * 40 + 5 铅气球出现
        private void roll(string seed)
        {
            double temp = 0;
            Random x = new Random(1337);
            foreach (char c in seed)
            {
                temp += Convert.ToInt32(c) * 10000;
                temp *= x.NextDouble();
            }
            Random a = new Random((int)temp);

            hints.Clear();
            attrs.Clear();
            foreach (var pair in attrs_default)
            {
                if (!pair.Key.Contains("cash multiplier[pop][round after"))
                {
                    attrs[pair.Key] = pair.Value;
                }
            }

            double size = SEL("control_random_preset").SelectedIndex;
            double length = SEL("control_random_length").SelectedIndex;
            double regrow = SEL("control_random_regrow").SelectedIndex;
            double camo = SEL("control_random_camo").SelectedIndex;
            double fort = SEL("control_random_fort").SelectedIndex;
            double early = SEL("control_random_earliers").SelectedIndex;
            double boss = SEL("control_random_bosses").SelectedIndex;
            double attr = SEL("control_random_attrs").SelectedIndex;
            double diff = SEL("control_random_diffs").SelectedIndex;

            double size_base = ((size + 1) + a.NextDouble()) / 5;
            double length_base = Math.Pow(length + 2, 1.75) * (a.NextDouble() + 4) * 0.01;
            if (length == 4)
            {
                length_base = 1;
            }

            double regrow_base = Math.Pow(regrow, 1.5) * (a.NextDouble() + 2) * 0.04;
            if (regrow == 4)
            {
                regrow_base = 1;
            }
            double camo_base = Math.Pow(camo, 1.5) * (a.NextDouble() + 2) * 0.04;
            if (regrow == 4)
            {
                regrow_base = 1;
            }
            double fort_base = Math.Pow(fort, 1.5) * (a.NextDouble() + 2) * 0.04;
            if (regrow == 4)
            {
                regrow_base = 1;
            }

            double early_base = (early + 1) * (a.NextDouble() + 0.4) * 0.2; 
            if (early == 3)
            {
                early_base = 1;
            }
            diff -= early_base * 2;

            double boss_base = boss * (a.NextDouble() + 1) / 4;

            double attr_base = Math.Pow(attr + 1, 1.5) * (a.NextDouble() + 1) * 0.2;
            //test.Text = attr_base.ToString();

            //设置开始和结束
            double start_percent = (1 - length_base) / 3 * Math.Pow(a.NextDouble(), 1.5);
            int start = (int)(start_percent * 139 + 1);
            int end = (int)((start_percent + length_base) * 139 + 1);
            attrs["start round"] = start;
            attrs["end round"] = end;
            round_select(start);
            TextBox tb = (TextBox)FindName("control_round_box");
            tb.Text = start.ToString();
            tb = (TextBox)FindName("control_cash_box");
            tb.Text = "650";

            double player_cash = 650;
            double price_mul = 1;

            //设置属性
            random_mods.Clear();
            List<string> attr_list = get_attr_choices();
            int fail_count = 0;
            while (attr_base > 0.1)
            {
                int index = a.Next(attr_list.Count);
                string s = attr_list[index];
                if (s != "start round" && s != "end round" && s != "player max health" &&
                    !random_mods.ContainsKey(s))
                {
                    attr_base -= 0.1;
                    double mod = -1;
                    double more_power = Math.Min(attr_base, a.NextDouble());
                    attr_base -= more_power * 0.3;
                    double value = (a.NextDouble() + 1) / 2 * (1 + more_power * 4);
                    //value: 0 - 5
                    switch (s)
                    {
                        case "start cash":
                            mod = 700 * (1 + Math.Pow(value, 2));
                            player_cash = mod;
                            tb = (TextBox)FindName("control_cash_box");
                            tb.Text = mod.ToString("f2");
                            break;
                        case "player health":
                            mod = (int)(100 * Math.Pow(value + 0.5, size_base * length_base * 3) + 1);
                            diff += (Math.Log10(mod) - 2) / (1 + size_base * length_base * 3);
                            attrs["player max health"] = mod * 100;
                            break;
                        case "cash multiplier[sell]":
                            mod = 0.7 + (a.NextDouble() - 0.5) * value * 0.1;
                            diff += (mod - 0.7) * 4;
                            break;
                        case "cash multiplier[farm]":
                            mod = Math.Pow(value + 1, a.NextDouble() - 0.5);
                            diff += Math.Pow(mod, 2) - 1;
                            break;
                        case "cash multiplier[end round]":
                            mod = Math.Pow(value + 1, (a.NextDouble() - 0.5) * 4);
                            break;
                        case "cash multiplier[pop]":
                            mod = Math.Pow(value + 1, (a.NextDouble() - 0.5) * 4);
                            break;
                        case "tower price multiplier":
                            if (attr_base > 0.4)
                            {
                                mod = Math.Pow(value + 1, a.NextDouble() - 0.5);
                                price_mul = mod;
                            }
                            break;
                        case "tower attack speed multiplier":
                            if (attr_base > 0.4)
                            {
                                mod = Math.Pow(value * 0.5 + 1, a.NextDouble() - 0.4);
                                diff += (mod - 1) * 5;
                            }
                            break;
                        case "tower damage multiplier":
                            if (attr_base > 0.6)
                            {
                                mod = Math.Pow(value * 0.5 + 1, a.NextDouble() - 0.2);
                                diff += (mod - 1) * 5;
                            }
                            break;
                        case "tower range multiplier":
                            if (attr_base > 0.4)
                            {
                                mod = Math.Pow(value * 0.2 + 1, a.NextDouble() - 0.5);
                                diff += (mod - 1) * 2;
                            }
                            break;
                        case "HEALTHY[add]":
                            if (attr_base > 0.4)
                            {
                                mod = (int)(10 * Math.Pow(value + 0.5, size_base * length_base * 3));
                                diff += (Math.Log10(mod) - 1) / (1 + size_base * length_base * 3);
                            }
                            break;
                        case "HEALTHY[mul]":
                            if (attr_base > 0.4)
                            {
                                mod = value * size_base * length_base * 0.02;
                                diff += value * size_base * length_base * 0.25;
                            }
                            break;
                        case "AUTOFARM[mul]":
                            if (attr_base > 0.4)
                            {
                                mod = value * 0.04;
                                diff += Math.Pow(4, (mod - 0.07) * 10) - 1;
                            }
                            break;
                        case "TIMETAX[/s]":
                            if (attr_base > 1)
                            {
                                mod = value;
                                diff -= mod * 0.4;
                            }
                            break;
                        case "TIMETAX[mul]":
                            if (attr_base > 1)
                            {
                                mod = value * 0.0001;
                                diff -= value * 0.4;
                            }
                            break;
                    }
                    if (mod != -1)
                    {
                        attrs[s] = mod;
                        random_mods[s] = mod;
                    }
                    else
                    {
                        attr_base += 0.1;
                    }
                }
                else
                {
                    fail_count++;
                }
                if (fail_count > 100)
                {
                    break;
                }
            }
            attrs["cash multiplier[farm]"] *= attrs["tower price multiplier"];

            List<(double, double)> droper = new List<(double, double)>();
            foreach (var c in random_cash_drop)
            {
                droper.Add((c.Item1 * (12.5 / (diff + 7.5)) * (a.NextDouble() * 0.1 + 1) * 
                    (1 + 0.08 * size_base),
                    c.Item2 * (a.NextDouble() + 8) / 9));
            }
            int drop_index = 0;

            Dictionary<string, bloon_compare> compares =
                new Dictionary<string, bloon_compare>();

            Dictionary<string, (double, double)> random_power_table_curr =
                new Dictionary<string, (double, double)>();
            foreach(var pair in random_power_table)
            {
                random_power_table_curr[pair.Key] = pair.Value;
            }
            if (boss >= 1)
            {
                foreach (var pair in random_power_table_boss)
                {
                    random_power_table_curr[pair.Key] = pair.Value;
                }
            }
            if (boss >= 2)
            {
                foreach (var pair in random_power_table_eboss)
                {
                    random_power_table_curr[pair.Key] = pair.Value;
                }
            }

            double boss_check = 0;
            //生成气球
            for (int i = start; i <= end; i++)
            {
                if(i == 47)
                {

                }
                if (i == 46)
                {

                }
            restart:

                round r = rounds[i];
                r.bloon_entrys.Clear();
                r.time_max = 0;

                double player_actual_cash() { return player_cash / price_mul; };

                double all_poss = 0;
                List<(string, double)> poss_list = new List<(string, double)>();
                List<(string, double)> temp_list = new List<(string, double)>();
                List<(string, double)> p_list = new List<(string, double)>();
                double speed_m = speed_mul(i);
                double health_m = health_mul(i);
                foreach (var v in random_power_table_curr)
                {
                    double cash_need = v.Value.Item1;
                    cash_need = cash_need / ((diff + 5) / 10);
                    if (!v.Key.Contains("BOSS"))
                    {
                        cash_need *= 0.3 * Math.Log10(speed_m * health_m) + 1;
                    }
                    else
                    {
                        if (a.NextDouble() < boss_check)
                        {
                            continue;
                        }
                    }
                    double ratio = player_actual_cash() / cash_need;
                    bool b = compares.ContainsKey(v.Key);
                    if (!b)
                    {
                        if (ratio >= 1.2 * (1 - early_base + 0.01))
                        {
                            b = true;
                        }
                    }
                    if (b)
                    {
                        if (v.Key == "Ddt")
                        {

                        }
                        if (v.Key == "Lead")
                        {
                            ratio = Math.Pow(ratio, 1.3);
                        }
                        double p = Math.Pow(0.48, Math.Pow(ratio, 0.4)) 
                            * (a.NextDouble() + 0.25);
                        if (ratio > 100)
                        {
                            p = 0;
                        }
                        poss_list.Add((v.Key, p));
                        all_poss += p;
                    }
                }
                if (all_poss == 0)
                {
                    goto restart;
                }
                foreach(var b in poss_list)
                {
                    p_list.Add((b.Item1, b.Item2 / all_poss));
                }

                all_poss = 0;
                int type_amount = Math.Min(a.Next(4) + 2, p_list.Count);
                for(int k = 0; k < type_amount; k++)
                {
                    double p = a.NextDouble() * (1 - all_poss);
                    int n = 0;
                    foreach(var v in p_list)
                    {
                        double p_need = v.Item2;
                        if (p < p_need)
                        {
                            break;
                        }
                        else
                        {
                            p -= p_need;
                        }
                        n++;
                    }
                    try
                    {
                        temp_list.Add(p_list[n]);
                        all_poss += p_list[n].Item2;
                        p_list.RemoveAt(n);
                    }
                    catch
                    {
                        break;
                    }
                }

                p_list.Clear();
                foreach (var b in temp_list)
                {
                    /*
                    bool elite = false;
                    string bloon_name = b.Item1;
                    if (bloon_name.Contains("EBOSS"))
                    {
                        elite = true;
                    }
                    if (bloon_name.Contains("BOSS"))
                    {
                        string boss_name = boss_list[a.Next(boss_list.Count)];
                        bloon_name = boss_name + bloon_name.Last();
                    }*/
                    p_list.Add((b.Item1, b.Item2 / all_poss));
                }

                double time_base = 15000;
                double time_factor = (Math.Pow(a.NextDouble(), 1.5) * 1.5 + 1) * time_base
                    // Math.Pow(1 + (20 + diff) * 0.002, Math.Max(0, start + 20 - i))
                    / (1 + Math.Log10(1 + 10000 / player_actual_cash()));
                double pressure_need = Math.Pow(player_actual_cash(), 1.4 + 0.02 * size_base) *
                    Math.Pow(Math.Log10(player_actual_cash() / 1000 + 10), size_base * 2) *
                    Math.Pow(1.15, diff - 5) *
                    Math.Max(1, (start + 10 - i) / 10) * 
                    (Math.Pow(a.NextDouble(), 0.7) * 0.75 + 1.75) *
                    Math.Pow(time_factor / time_base, 0.5);
                double pressure = 0;
                double div = Math.Pow(a.NextDouble() * 3, 1.3) /
                    (1 + Math.Log10(1 + 5000 / player_actual_cash())) + 1;
                fail_count = 0;
                double start_min = time_factor * 2;
                int success = 0;

                Dictionary<string, List<bloon_entry>> temp_entrys =
                    new Dictionary<string, List<bloon_entry>>();
                foreach (var v in random_power_table_curr)
                {
                    string name = v.Key;
                    if (!name.Contains("BOSS"))
                    {
                        temp_entrys[name] = new List<bloon_entry>();
                    }
                }
                Dictionary<string, int> boss_spawns = new Dictionary<string, int>();
                foreach (var b in boss_list)
                {
                    boss_spawns[b] = 0;
                    for(int k = 1; k <= 5; k++)
                    {
                        temp_entrys[b + k] = new List<bloon_entry>();
                    }
                }


                while (pressure < pressure_need * 0.95)
                {
                    if (fail_count > 10)
                    {
                        div /= 0.99;
                    }
                    if (fail_count > 100)
                    {
                        break;
                    }
                    double pressure_duty = pressure_need * (a.NextDouble() + 0.5) / 1.25 / div;
                    pressure_duty = Math.Min(pressure_duty, pressure_need - pressure);

                    double p = a.NextDouble();
                    string name = p_list[0].Item1;
                    foreach (var b in p_list)
                    {
                        double p_need = b.Item2;
                        if (p < p_need)
                        {
                            name = b.Item1;
                            break;
                        }
                        else
                        {
                            p -= p_need;
                        }
                    }

                    double power_base = 1200;
                    double power_cash = random_power_table_curr[name].Item2;
                    if (i > 80 && random_power_table_high.ContainsKey(name))
                    {
                        power_cash = random_power_table_high[name];
                    }
                    double bloon_power = power_cash * power_base;

                    bool elite = false;
                    bool is_boss = false;
                    string boss_name = "";
                    int level = 1;
                    if (name.Contains("EBOSS"))
                    {
                        elite = true;
                    }
                    if (name.Contains("BOSS"))
                    {
                        boss_name = boss_list[a.Next(boss_list.Count)];
                        level = int.Parse(name.Last() + "");
                        name = boss_name + level;

                        if (boss_spawns[boss_name] >= 1)
                        {
                            fail_count++;
                            continue;
                        }
                        boss_spawns[boss_name] = 1;
                        is_boss = true;
                    }
                    bloon_template bt = find_bt(name);

                    /*if (!is_boss)
                    {
                        bloon_power *= Math.Pow(speed_mul(i) * health_mul(i), 0.7);
                    }*/
                    if (bloon_power > 1.5 * pressure_duty)
                    {
                        fail_count++;
                        continue;
                    }
                    
                    if (bloon_power < 0.02 * pressure_need)
                    {
                        bloon_power /= Math.Pow(0.02 * pressure_need / bloon_power, 0.125);
                    }

                    #region regrow
                    double regrowed = 0;
                    if (compares.ContainsKey(name) && compares[name].bools[1])
                    {
                        regrowed = 1;
                    }

                    bool is_regrow = (a.NextDouble() * (2 - regrowed)) < regrow_base && bt.has_regrow;
                    is_regrow |= regrow_base == 1;
                    if (is_regrow)
                    {
                        bloon_power *= (1 + (12 - diff) / 20 / 
                            Math.Pow(Math.Log10(10 + player_actual_cash() / 1000), 1.5));
                    }
                    #endregion regrow

                    #region camo
                    double camoed = 0;
                    if (compares.ContainsKey(name) && compares[name].bools[0])
                    {
                        camoed = 1;
                    }

                    bool is_camo = false;
                    if (name == "Ddt")
                    {
                        is_camo = true;
                        bloon_power *= Math.Pow(1 + 100000 / player_actual_cash(), 1.5);
                    }
                    else
                    {
                        is_camo = (a.NextDouble() * (2 - camoed)) < camo_base && bt.has_camo;
                        if (player_actual_cash() < 3000 / (1 + 0.1 * diff))
                        {
                            is_camo = false;
                        }
                        is_camo |= camo_base == 1;
                        if (is_camo)
                        {
                            bloon_power += Math.Pow(4 + 15000 / player_actual_cash(), 1.5) * power_base;
                            if (name == "Lead")
                            {
                                bloon_power *= Math.Pow(1 + 50000 / player_actual_cash(), 0.4);
                            }
                            bloon_power *= (1 + (12 - diff) / 10 /
                                Math.Pow(Math.Log10(10 + player_actual_cash() / 1250), 1.5));
                        }
                    }
                    #endregion camo

                    #region fort
                    double forted = 0;
                    if (compares.ContainsKey(name) && compares[name].bools[2])
                    {
                        forted = 1;
                    }

                    bool is_fort = (a.NextDouble() * (2 - forted)) < fort_base && bt.has_fort;
                    is_fort |= fort_base == 1;
                    if (is_fort)
                    {
                        bloon_power *= (1.75 + (10 - diff) / 20);
                        bloon_power *= Math.Max(1,
                            Math.Pow(power_cash * 1.8 / player_actual_cash(), 0.75));
                    }
                    #endregion fort

                    double density = a.NextDouble();
                    double rand_d = a.NextDouble();
                    double t_length;
                    if (density < 0.35)
                    {
                        t_length = (rand_d + 1) / 2;
                    }
                    else if (density < 0.6)
                    {
                        t_length = (rand_d + 0.5) / 3;
                    }
                    else if (density < 0.8)
                    {
                        t_length = (rand_d + 0.35) / 4;
                    }
                    else if (density < 0.88)
                    {
                        t_length = (rand_d + 0.3) / 6;
                    }
                    else if (density < 0.94)
                    {
                        t_length = (rand_d + 0.25) / 8;
                    }
                    else if (density < 0.97)
                    {
                        t_length = (rand_d + 0.2) / 15;
                    }
                    else if (density < 0.982)
                    {
                        t_length = (rand_d + 0.2) / 30;
                    }
                    else if (density < 0.99)
                    {
                        t_length = (rand_d + 0.2) / 50;
                    }
                    else if (density < 0.996)
                    {
                        t_length = (rand_d + 0.2) / 100;
                    }
                    else
                    {
                        t_length = (rand_d + 0.15) / 300;
                    }
                    double t_start = (1 - t_length) * a.NextDouble();
                    int t_count = (int)(pressure_duty / bloon_power * (1.8 - density) / 2);
                    if (t_count == 0)
                    {
                        t_count = 1;
                    }
                    if (t_count > 3 && is_boss)
                    {
                        t_count = 3;
                    }
                    if ((t_count >= 10000) || (name == "Lead" && t_count > 200))
                    {
                        fail_count++;
                        continue;
                    }

                    double pressure_get = t_count * bloon_power / (1.8 - density) * 2;
                    if (pressure_get > pressure_duty * 1.5)
                    {
                        fail_count++;
                        continue;
                    }
                    else if (pressure_get > pressure_duty * 0.5)
                    {
                        start_min = Math.Min(start_min, t_start * time_factor);

                        pressure += pressure_get;
                        bloon_entry be = new bloon_entry(name, t_start * time_factor,
                            (t_start + t_length) * time_factor, t_count);
                        be.regrow = is_regrow;
                        be.camo = is_camo;
                        be.fort = is_fort;
                        be.elite = elite;
                        be.level = level;
                        temp_entrys[name].Add(be);
                        if (is_boss)
                        {
                            boss_check += 0.4 + t_count * 0.1;
                        }
                        success++;
                        //r.add_entry(be);
                        player_cash += CASH(be, i);

                        bloon_compare bc = new bloon_compare(name, name,
                            new bool[]
                            {
                                be.camo, be.regrow, be.fort, false, false, false, false
                            }, be.level, i, t_count);
                        if (!compares.ContainsKey(name) ||
                            bc > compares[name])
                        {
                            compares[name] = bc;
                        }
                    }
                    else
                    {
                        fail_count++;
                    }
                }
                if (success == 0)
                {
                    goto restart;
                }
                player_cash += (100 + i) * attrs["cash multiplier[end round]"];

                foreach(var v in temp_entrys)
                {
                    foreach(var be in v.Value)
                    {
                        r.add_entry(be);
                    }
                }
                r.bloon_entrys.Reverse();
                r.time_max = 0;
                foreach (bloon_entry be in r.bloon_entrys)
                {
                    be.start -= start_min;
                    be.end -= start_min;
                    r.time_max = Math.Max(r.time_max, be.end);
                }

                while (drop_index < droper.Count)
                {
                    var c = droper[drop_index];
                    if (player_actual_cash() > c.Item1)
                    {
                        drop_index++;
                        attrs["cash multiplier[pop][round after " + i + "]"] = c.Item2;
                        round_cash_mul_attrs = get_round_cash_mul_attrs();
                    }
                    else
                    {
                        break;
                    }
                }
                boss_check -= 0.1;
                boss_check *= 0.925;
                boss_check = Math.Max(0, boss_check);
            }

            button_effect("debut_rollup");
        }

        Dictionary<string, double> random_mods =
            new Dictionary<string, double>();

        Dictionary<string, (double, double)> random_power_table = 
            new Dictionary<string, (double, double)>()
        {
            {"Red", (200, 1)},
            {"Blue", (400, 2.3)},
            {"Green", (800, 4)},
            {"Yellow", (1500, 7)},

            {"Pink", (2500, 11)},
            {"Black", (4000, 18)},
            {"White", (4000, 18)},
            {"Purple", (4500, 23)},
            {"Zebra", (5200, 30)},
            {"Lead", (5500, 32)},
            {"Rainbow", (8000, 51)},
            {"Ceramic", (11000, 120)},

            {"Moab", (17500, 550)},
            {"Bfb", (45000, 2400)},
            {"Zomg", (87000, 12500)},
            {"Ddt", (120000, 2500)},
            {"Bad", (180000, 80000)},
        };

        Dictionary<string, double> random_power_table_high =
            new Dictionary<string, double>()
        {
            {"Red", 1},
            {"Blue", 2},
            {"Green", 3},
            {"Yellow", 4},

            {"Pink", 5},
            {"Black", 5},
            {"White", 5},
            {"Purple", 6},
            {"Zebra", 5},
            {"Lead", 6},
            {"Rainbow", 7},
            {"Ceramic", 100},

            {"Moab", 550},
            {"Bfb", 2400},
            {"Zomg", 12500},
            {"Ddt", 2500},
            {"Bad", 80000},
        };

        Dictionary<string, (double, double)> random_power_table_boss =
            new Dictionary<string, (double, double)>()
            {
                {"BOSS1", (55000, 10000)},
                {"BOSS2", (120000, 45000)},
                {"BOSS3", (200000, 250000)},
                {"BOSS4", (320000, 600000)},
                {"BOSS5", (500000, 2000000)},

            };

        Dictionary<string, (double, double)> random_power_table_eboss =
            new Dictionary<string, (double, double)>()
            {
                {"EBOSS1", (100000, 35000)},
                {"EBOSS2", (200000, 280000)},
                {"EBOSS3", (425000, 1400000)},
                {"EBOSS4", (650000, 6000000)},
                {"EBOSS5", (1100000, 25000000)},
            };

        List<string> boss_list = new List<string>()
        {
            "Bloonarius",
            "Lych",
            "Vortex",
            "Dreadbloon",
            "Phayze"
        };

        List<(double, double)> random_cash_drop = new List<(double, double)>()
        {
            (10000, 0.7),
            (20000, 0.5),
            (30000, 0.35),
            (40000, 0.25),
            (50000, 0.175),
            (70000, 0.13),
            (100000, 0.1),
            (150000, 0.06),
            (200000, 0.04),
            (300000, 0.025),
            (500000, 0.014),
            (700000, 0.006),
            (1100000, 0.002),
            (1600000, 0.0009),
            (2500000, 0.0004),
            (3500000, 0.00015),
            (5000000, 0.00004),
            (7500000, 0.000008),
            (10000000, 0.0000015),
            (15000000, 0.0000004),
            (20000000, 0.0000001),
            (25000000, 0.00000002),
            (30000000, 0.000000004),
            (35000000, 0.0000000007),
            (40000000, 0.0000000001),
            (50000000, 0.0000000000016),
        };
    }
}
