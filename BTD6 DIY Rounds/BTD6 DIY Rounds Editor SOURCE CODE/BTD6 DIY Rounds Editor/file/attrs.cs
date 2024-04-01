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
using Newtonsoft.Json;
using System.Diagnostics.Metrics;
using System.Security.Policy;

namespace BTD6_DIY_Rounds
{
    public partial class MainWindow : Window
    {
        int cash_page = 1;
        int cash_page_max = 1;
        Dictionary<string, string> attrs_inf = new Dictionary<string, string>();
        Dictionary<string, double> attrs_default = new Dictionary<string, double>();
        Dictionary<string, double> attrs = new Dictionary<string, double>();

        
        private void attr_init()
        {
            attrs_default = new Dictionary<string, double>()
            {
                {"start cash", double.NaN},
                {"start round", double.NaN},
                {"end round", double.NaN},
                {"player health", double.NaN},
                {"player max health", double.NaN},
                {"cash multiplier[sell]", double.NaN},
                {"cash multiplier[farm]", 1},
                {"cash multiplier[end round]", 1},
                {"cash multiplier[pop]", 1},
                {"tower price multiplier", 1},
                {"tower attack speed multiplier", 1},
                {"tower damage multiplier", 1},
                {"tower range multiplier", 1},
                {"HEALTHY[add]", 0},
                {"HEALTHY[mul]", 0},
                {"AUTOFARM[mul]", 0},
                {"TIMETAX[/s]", 0},
                {"TIMETAX[mul]", 0},
                {"cash multiplier[pop][round after 50]", 0.5},
                {"cash multiplier[pop][round after 60]", 0.2},
                {"cash multiplier[pop][round after 85]", 0.1},
                {"cash multiplier[pop][round after 100]", 0.05},
                {"cash multiplier[pop][round after 120]", 0.02},
            };

            attrs_inf = new Dictionary<string, string>()
            {
                {"start cash", "游戏开始时的金钱，对沙盒模式无效"},
                {"start round", "游戏开始时的回合数"},
                {"end round", "游戏的最后一回合"},
                {"player health", "玩家的生命值"},
                {"player max health", 
                    "玩家的最大生命值，默认为5000，当你修改初始生命值大于5000时，它会随之变化"},
                {"cash multiplier[sell]", 
                    "卖塔回收金的比例，默认为0.7，不能超过0.95(95%)，此设置不能无视知识"},
                {"cash multiplier[farm]", "经济类金钱生成的倍率，不包括工程师的陷阱"},
                {"cash multiplier[end round]", "回合结束金会倍增于此倍率"},
                {"cash multiplier[pop]", "打气球赚钱的倍率"},
                {"tower price multiplier", "买塔和升级塔的价格倍率"},
                {"tower attack speed multiplier", 
                    "塔的攻速倍率，越大攻击越快"},
                {"tower damage multiplier", "塔的伤害倍率，对胶水猴无效，低于1的伤害会转化为命中率"},
                {"tower range multiplier", "塔的范围倍率，修改后，你需要升级已有的塔来更新数据"},
                {"HEALTHY[add]", "[自定义属性]每回合结束时增加固定的血量"},
                {"HEALTHY[mul]", "[自定义属性]每回合结束时基于原血量增加一定倍数的血量"},
                {"AUTOFARM[mul]", "[自定义属性]每回合结束时基于原金钱增加一定倍数的金钱"},
                {"TIMETAX[/s]", "[自定义属性]战斗时每秒扣除固定量的金钱，若钱不足则赊账"},
                {"TIMETAX[mul]", "[自定义属性]战斗时每秒扣除一定量的金钱，扣钱数为原金钱的指定比例"},
            };

            foreach (var pair in attrs_default)
            {
                attrs[pair.Key] = pair.Value;
            }

            round_cash_mul_attrs = get_round_cash_mul_attrs();
        }

        private List<string> get_attr_choices()
        {
            List<string> choices = new List<string>();
            foreach (var pair in attrs_default)
            {
                if (!pair.Key.Contains("cash multiplier[pop][round after"))
                {
                    choices.Add(pair.Key);
                }
            }
            return choices;
        }

        private List<Tuple<int, double>> round_cash_mul_attrs;
        private List<Tuple<int, double>> get_round_cash_mul_attrs()
        {
            List<Tuple<int, double>> ret = new List<Tuple<int, double>>();
            for (int i = 140; i >= 1; i--)
            {
                string s = "cash multiplier[pop][round after " + i + "]";
                if (attrs.ContainsKey(s))
                {
                    ret.Add(new Tuple<int, double>(i, attrs[s]));
                }
            }
            return ret;
        }

        private string double2str(double d)
        {
            if (double.IsNaN(d))
            {
                return "默认";
            }
            if (d == (int)d)
            {
                return d.ToString("f0");
            }
            if (d == 0)
            {
                return "0";
            }
            if (d < 1e-7)
            {
                return d.ToString("f9");
            }
            if (d < 1e-6)
            {
                return d.ToString("f8");
            }
            if (d < 1e-5)
            {
                return d.ToString("f7");
            }
            if (d < 1e-4)
            {
                return d.ToString("f6");
            }
            if (d < 1e-3)
            {
                return d.ToString("f5");
            }
            if (d < 1e-2)
            {
                return d.ToString("f4");
            }
            if (d < 1e-1)
            {
                return d.ToString("f3");
            }
            return d.ToString("f2");
        }

        private int get_start_round()
        {
            int start_round = 0;
            if (!double.IsNaN(attrs["start round"]))
            {
                start_round = (int)attrs["start round"];
            }
            return start_round;
        }
        private int get_end_round()
        {
            int end_round = 150;
            if (!double.IsNaN(attrs["end round"]))
            {
                end_round = (int)attrs["end round"];
            }
            return end_round;
        }

        private void update_stat_input()
        {
            double cash = attrs["start cash"];
            if (!double.IsNaN(cash))
            {
                TextBox t = BOX("control_cash");
                t.Text = double2str(cash);
            }

            int s = get_start_round();
            if (s != 0)
            {
                TextBox t = BOX("control_round");
                t.Text = s.ToString();
            }
        }
    }
}
