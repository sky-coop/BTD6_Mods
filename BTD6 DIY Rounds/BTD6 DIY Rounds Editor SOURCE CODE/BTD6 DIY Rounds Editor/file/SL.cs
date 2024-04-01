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
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.Xml.Linq;
using System.Runtime.InteropServices;

namespace BTD6_DIY_Rounds
{
    public partial class MainWindow : Window
    {
        private class pack
        {
            public Dictionary<string, List<bloon_entry_json>> rounds;
            public Dictionary<string, double> attrs;
            public Dictionary<int, Tuple<ARGB, string>> hints;
        }
        pack pa = new pack(); 

        private class bloon_entry_json
        {
            public string bloon_name;
            public string start_time;
            public string end_time;
            public string amount;
        }

        private bloon_entry_json create_bej(bloon_entry be)
        {
            return new bloon_entry_json()
            {
                bloon_name = bloon_name_mix(be),
                start_time = be.start.ToString("f2"),
                end_time = be.end.ToString("f2"),
                amount = be.count.ToString(),
            };
        }


        private Dictionary<string, List<bloon_entry_json>> rounds2pack()
        {
            Dictionary<string, List<bloon_entry_json>> dic =
                new Dictionary<string, List<bloon_entry_json>>();
            for (int i = 1; i < rounds.Length; i++)
            {
                List<bloon_entry_json> round = new List<bloon_entry_json>();
                foreach (bloon_entry be in rounds[i].bloon_entrys)
                {
                    round.Add(create_bej(be));
                }
                dic.Add("Round " + i, round);
            }
            return dic;
        }
        private string pack2json()
        {
            pa.rounds = rounds2pack();
            pa.attrs = attrs;
            pa.hints = hints;
            return JsonConvert.SerializeObject(pa);
        }

        private string round_file_name = "rounds.json";
        private bool exist_rounds(string folderpath)
        {
            if (Directory.Exists(folderpath))
            {
                if (!File.Exists(folderpath + "/" + round_file_name))
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            return true;
        }
        private void load_rounds(string pathname)
        {
            string filepath = pathname + "/" + round_file_name;
            string s = File.ReadAllText(filepath);
            pa = JsonConvert.DeserializeObject<pack>(s);
            
            if (pa.rounds != null)
            {
                foreach (KeyValuePair<string, List<bloon_entry_json>> pair in pa.rounds)
                {
                    int r = Convert.ToInt32(pair.Key.Split(' ')[1]);
                    List<bloon_entry_json> list = pair.Value;
                    rounds[r] = new round(r);

                    foreach (bloon_entry_json b in list)
                    {
                        bloon_entry be = new bloon_entry();

                        be.regrow = try_remove_substring(ref b.bloon_name, "Regrow");
                        be.fort = try_remove_substring(ref b.bloon_name, "Fortified");
                        be.camo = try_remove_substring(ref b.bloon_name, "Camo");
                        if (b.bloon_name.Contains("Golden"))
                        {
                            be.lead = try_remove_substring(ref b.bloon_name, "Lead");
                            be.purple = try_remove_substring(ref b.bloon_name, "Purple");
                            be.zebra = try_remove_substring(ref b.bloon_name, "Zebra");
                        }
                        be.elite = try_remove_substring(ref b.bloon_name, "Elite");
                        try_remove_substring(ref b.bloon_name, "Standard");
                        be.bloon_name = b.bloon_name;

                        be.start = double.Parse(b.start_time);
                        be.end = double.Parse(b.end_time);
                        be.count = int.Parse(b.amount);

                        rounds[r].add_entry(be);
                    }
                }
            }

            if (pa.attrs != null)
            {
                attrs.Clear();
                foreach (var v in pa.attrs)
                {
                    attrs[v.Key] = v.Value;
                }
            }
            if (pa.hints != null)
            {
                hints.Clear();
                foreach (var v in pa.hints)
                {
                    hints[v.Key] = v.Value;
                }
            }
            button_effect("debut_rollup");
            update_stat_input();
            round_cash_mul_attrs = get_round_cash_mul_attrs();
        }

        private bool save_rounds_workspace(string name)
        {
            if (name.Contains("/"))
            {
                MessageBox.Show("创建文件夹失败，请注意不要使用特殊名称");
                return false;
            }
            if (!Directory.Exists("./Workspace/" + name))
            {
                try
                {
                    Directory.CreateDirectory("./Workspace/" + name);
                }
                catch 
                {
                    MessageBox.Show("创建文件夹失败，请注意不要使用特殊名称");
                    return false;
                }
            }
            string pathbase = "./Workspace/" + name + "/DIY Rounds";
            if (!Directory.Exists(pathbase))
            {
                Directory.CreateDirectory(pathbase);
            }

            string filepath = pathbase + "/" + round_file_name;
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }
            FileStream a = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write);
            tool.fs_write(a, pack2json());
            a.Close();

            return true;
        }

        private bool save_rounds_game(string diy_folder)
        {
            if (!Directory.Exists(diy_folder))
            {
                Directory.CreateDirectory(diy_folder);
            }
            
            string filepath = diy_folder + "/" + round_file_name;
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }
            FileStream a = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write);
            tool.fs_write(a, pack2json());
            a.Close();

            return true;
        }

        private void files_update()
        {
            try
            {
                foreach(var work in Directory.GetDirectories("./Workspace"))
                {
                    string[] parts = work.Split('/');
                    string name = parts.Last();
                    string loadpos = work + "/DIY Rounds";
                    load_rounds(loadpos);
                    save_rounds_workspace(name);
                }
            }
            catch (Exception e)
            {
                float_default(10, 10, e.Message, C(255, 0, 0), C(255, 255, 255), 11, 18000);
                //MessageBox.Show(e.Message);
            }
        }

    }
}
