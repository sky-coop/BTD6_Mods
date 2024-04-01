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
using System.Globalization;

namespace BTD6_DIY_Rounds
{
    public partial class MainWindow : Window
    {
        public static ARGB A(byte r, byte g, byte b)
        {
            return new ARGB(255, r, g, b);
        }

        public static ARGB A(byte a, byte r, byte g, byte b)
        {
            return new ARGB(a, r, g, b);
        }

        Dictionary<int, Tuple<ARGB, string>> hints_default = 
            new Dictionary<int, Tuple<ARGB, string>>() 
            {
                {2, new Tuple<ARGB, string>(A(255, 255, 255), 
                    "每击破一层气球都会获得1笔现金，并且在每回合结束时还会获得一些奖励。") },
                {7, new Tuple<ARGB, string>(A(255, 255, 255),
                    "定期玩《Bloons TD 6》，并查看更新——地图、英雄、挑战和其他精彩部分。") },
                {12, new Tuple<ARGB, string>(A(255, 255, 255),
                    "使用某个猴子的次数越多，因使用它而需要解锁的升级也越多。") },
                {26, new Tuple<ARGB, string>(A(255, 255, 255),
                    "看到那些斑马条纹气球了吗？它们结合了白气球和黑气球的残暴属性，黑白相间，非常有趣。") },
                {32, new Tuple<ARGB, string>(A(255, 255, 255),
                    "你在第24回合又见到了一个迷彩气球。希望你已经准备好迎接更多！") },
                {38, new Tuple<ARGB, string>(A(255, 255, 255),
                    "下一回合结束时有2个重生彩虹气球。如果处理得不好，2个可能会变成多个......") },
                {39, new Tuple<ARGB, string>(A(255, 255, 255),
                    "下一回合将首次出现首MOAB级气球。不要说你没有接到警告！") },
                {41, new Tuple<ARGB, string>(A(255, 255, 255),
                    "接下来：再生彩虹气球和迷彩彩虹气球。你确定没问题吗？") },
                {45, new Tuple<ARGB, string>(A(255, 255, 255),
                    "接下来是强化陶瓷气球。") },
                {50, new Tuple<ARGB, string>(A(255, 255, 255),
                    "顺带一说，大量迷彩陶瓷气球可能会毁了你一天的好心情。") },
                {54, new Tuple<ARGB, string>(A(255, 255, 255),
                    "BTD6很棒。生活也很棒。不要忘记多多休息，做点别的事情。然后玩更多的BTD6！") },
                {58, new Tuple<ARGB, string>(A(255, 255, 255),
                    "你能击破铅气球吗？你能击破迷彩气球吗？那...迷彩铅气球呢？") },
                {59, new Tuple<ARGB, string>(A(255, 255, 255),
                    "接下来是中等难度的最后一回合。恭喜！击破BFB气球即可...") },
                {62, new Tuple<ARGB, string>(A(255, 255, 255),
                    "如果你玩过BTD5，你会记得第63回合是什么样的。如果你是这个游戏的新手，最好还是透露一下，那一回合有很多陶瓷气球。") },
                {73, new Tuple<ARGB, string>(A(255, 255, 255),
                    "下一回合：迷彩重生强化陶瓷气球。噗...真拗口。") },
                {77, new Tuple<ARGB, string>(A(255, 255, 255),
                    "当你将75个陶瓷气球塞进一个小空间时会发生什么？你即将找到答案！") },
                {79, new Tuple<ARGB, string>(A(255, 255, 255),
                    "接下来是困难模式的最后一回合。把你在对战ZOMG气球时获得的经验都丢到一边吧。" +
                    "最后一回合的气球很大、飞行很慢，但是非常难对付。") },
                {89, new Tuple<ARGB, string>(A(255, 255, 255),
                    "你问什么是DDT气球？大概就相当于MOAB气球、粉气球、迷彩气球和铅气球的合体一样吧。" +
                    "当然，继承的都是它们邪恶的一面。") },
                {100, new Tuple<ARGB, string>(A(255, 255, 255),
                    "恭喜你完成101回合！领取你的奖励吧！") },
            };
        Dictionary<int, Tuple<ARGB, string>> hints_default_abr =
            new Dictionary<int, Tuple<ARGB, string>>()
            {
                {3, new Tuple<ARGB, string>(A(255, 255, 255),
                    "替代气球回合...") },
                {4, new Tuple<ARGB, string>(A(255, 255, 255),
                    "你知道'替代'意味着更难吗？") },
                {5, new Tuple<ARGB, string>(A(255, 255, 255),
                    "你看到迷彩时是否恐慌？") },
                {7, new Tuple<ARGB, string>(A(255, 255, 255),
                    "仍然出于最大生命状态？") },
                {8, new Tuple<ARGB, string>(A(255, 255, 255),
                    "嗯，不知道什么时候第一个铅气球会出现？") },
                {21, new Tuple<ARGB, string>(A(255, 255, 255),
                    "这就是我们所说的“现金饥饿”回合......") },
                {39, new Tuple<ARGB, string>(A(255, 255, 255),
                    "下一回合，MOAB会像往常一样。真的是这样吗？") },
                {59, new Tuple<ARGB, string>(A(255, 255, 255),
                    "BFB下一回合出场。或者是FBFB？") },
                {77, new Tuple<ARGB, string>(A(255, 255, 255),
                    "下一回合是一个真正棘手......") },
                {79, new Tuple<ARGB, string>(A(255, 255, 255),
                    "ZOMG很难...") },
                {99, new Tuple<ARGB, string>(A(255, 255, 255),
                    "但还是没有BAD难对付，不是吗？") },
                {100, new Tuple<ARGB, string>(A(255, 255, 255),
                    "恭喜你完成101回合！领取你的奖励吧！") },
            };
        Dictionary<int, Tuple<ARGB, string>> hints =
            new Dictionary<int, Tuple<ARGB, string>>();

        private void hints_init(Dictionary<int, Tuple<ARGB, string>> source)
        {
            hints.Clear();
            foreach (var pair in source)
            {
                hints.Add(pair.Key, pair.Value);
            }
        }
        private double cal_font_mul(TextBlock t)
        {
            double ret = 1;

            int shrink_level = 2;

        start:
            int line = 1; 
            double textvalue = 0;
            foreach (char ch in t.Text)
            {
                if (char.IsLetter(ch))
                {
                    textvalue += 12 / 21;
                }
                if (char.IsDigit(ch))
                {
                    textvalue += 12 / 21;
                }
                else if (ch == '\n')
                {
                    line++;
                    textvalue = 0;
                }
                else if (ch == ' ')
                {
                    textvalue += 12 / 21;
                }
                else
                {
                    textvalue += 1;
                }
                if (shrink_level == 2 && textvalue > 26)
                {
                    line++;
                    textvalue = 0;
                    if (line == 3)
                    {
                        shrink_level = 3;
                        goto start;
                    }
                }
                if (shrink_level == 3 && textvalue > 34)
                {
                    line++;
                    textvalue = 0;
                    if (line == 4)
                    {
                        shrink_level = 4;
                        break;
                    }
                }
                if (line >= 4)
                {
                    shrink_level = line;
                    break;
                }
            }
            shrink_level = Math.Max(line, shrink_level);

            switch (shrink_level)
            {
                case 3:
                    ret = 6 / 8.0; 
                    break;
                case 4:
                    ret = 1 / 1.6;
                    break;
            }
            return ret;
        }
    }
}
