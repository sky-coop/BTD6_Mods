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
using System.Windows.Documents.Serialization;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Threading;

namespace BTD6_DIY_Rounds
{
    public partial class MainWindow : Window
    {
        /*回合处理：
         * 提供循环函数和预览 进行批量处理
         * 
         * 提供json复制（带操作）
         * 
         * 提供将某种气球批量处理的方法：添加属性 气球数倍增 气球密度倍增 转换为另一种气球（注意属性）
         * 
         */
        
        public class copy_pack
        {
            public round round;
            public Tuple<ARGB, string> hint;
            public copy_pack()
            {
            }

            [JsonConstructor]
            public copy_pack(round round, Tuple<ARGB, string> hint)
            {
                this.round = round;
                this.hint = hint;
            }
        }
        copy_pack cp_export = new copy_pack();
        copy_pack cp_import = new copy_pack();

        private void create_cp(int r, bool h)
        {
            cp_export.round = rounds[r];
            if(h && hints.ContainsKey(r))
            {
                cp_export.hint = hints[r];
            }
            else
            {
                cp_export.hint = null;
            }
        }

        private string save_cp(int r, bool h)
        {
            try
            {
                create_cp(r, h);
                return JsonConvert.SerializeObject(cp_export);
            }
            catch (Exception e)
            {
                float_default(10, -20, e.Message, C(255, 0, 0), C(255, 255, 255), 11, 30000);
                return "";
            }
        }

        private void save_curr_round_to_clip(bool hint)
        {
            if (rounds[curr_round].bloon_entrys.Count > 1000)
            {
                float_default(10, -20, "复制失败，不允许在超过1000项的回合进行此操作",
                    C(255, 0, 0), C(255, 255, 255), 11, 5000);
                return;
            }
            Clipboard.SetText(save_cp(curr_round, hint));
            float_default(10, 10, "第" + curr_round + "回合 [气球"  + 
                (hint ? "+提示信息": "") + 
                "] 复制成功！",
                C(0, 255, 0), C(0, 0, 0), 11, 4000);
        }

        private void check_clip_for_cp()
        {
            string s = Clipboard.GetText();
            try
            {
                cp_import = JsonConvert.DeserializeObject<copy_pack>(s);
            }
            catch
            {
                float_default(10, -30, "粘贴失败，请检查剪贴板的内容", 
                    C(255, 0, 0), C(255, 255, 255), 11, 5000);
                cp_import = null;
            }
        }
        bool loading = false;
        int round_store = -1;
        private void load_clip_to_curr_round(bool append)
        {
            int r = curr_round;
            if (rounds[r].bloon_entrys.Count > 1000)
            {
                float_default(10, -30, "粘贴失败，不允许向超过1000项的回合进行此操作",
                    C(255, 0, 0), C(255, 255, 255), 11, 5000);
                return;
            }
            check_clip_for_cp();
            loading = false;
            if (cp_import != null)
            {
                loading = true;
                round_store = r;
                curr_round = 0;

                rounds[0].bloon_entrys.Clear();
                rounds[0].time_max = 0;
                if (append)
                {
                    foreach(bloon_entry be in rounds[r].bloon_entrys)
                    {
                        rounds[0].add_entry(be);
                    }
                }
                foreach(bloon_entry be in cp_import.round.bloon_entrys)
                {
                    rounds[0].add_entry(be);
                }
            }
        }


        private void swap_round(int a, int b)
        {
            round ra = rounds[a];
            round rb = rounds[b];
            round r0 = new round(0);

            r0.bloon_entrys = ra.bloon_entrys;
            ra.bloon_entrys = rb.bloon_entrys;
            rb.bloon_entrys = r0.bloon_entrys;

            r0.time_max = ra.time_max;
            ra.time_max = rb.time_max;
            rb.time_max = r0.time_max;
        }

        private void swap_hint(int a, int b)
        {
            Tuple<ARGB, string> temp = null;
            if (hints.ContainsKey(a))
            {
                temp = hints[a];
                if (hints.ContainsKey(b))
                {
                    hints[a] = hints[b];  // 11
                }
                else
                {
                    hints.Remove(a);      // 10
                }
                hints[b] = temp;
            }
            else
            {
                if (hints.ContainsKey(b)) // 01
                {
                    hints[a] = hints[b];
                    hints.Remove(b);
                }
                // else 00
            }
        }

        int gcd(int a, int b)
        {
            return b == 0 ? a : gcd(b, a % b);
        }
        private void expand_round(int round, int n)
        {
            round r = rounds[round];
            if (r.bloon_entrys.Count * n > 1000)
            {
                float_default(10, -30, "扩展失败，不允许向将要超过1000项的回合进行此操作",
                    C(255, 0, 0), C(255, 255, 255), 11, 5000);
                return;
            }

            rounds[0].bloon_entrys.Clear();
            rounds[0].time_max = 0;
            foreach (bloon_entry be in r.bloon_entrys)
            {
                rounds[0].add_entry(be);
            }

            r.bloon_entrys.Clear();
            r.time_max = 0;
            foreach (bloon_entry be in rounds[0].bloon_entrys)
            {
                for(int i = 0; i < n; i++)
                {
                    r.add_entry(new bloon_entry(be.bloon_name, 
                        be.start, 
                        be.end, 
                        be.count, be.regrow, be.fort, be.camo, be.elite, be.boss, be.level,
                        be.lead, be.purple, be.zebra));
                    int t = 0;
                    while (gcd(be.count + t, n) != 1)
                    {
                        r.add_entry(new bloon_entry("Red", be.start, be.end, 1));
                        t++;
                    }
                }
            }
        }
    }
}
