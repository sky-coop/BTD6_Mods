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

namespace BTD6_DIY_Rounds
{
    public partial class MainWindow : Window
    {
        bool debut_show_type = false;
        bool debut_show_type_onlycamo = false;
        bool debut_show_type_roundsort = false;


        int debut_row_base = 0;
        int debut_row = 4;
        int debut_col = 4;

        public struct bloon_compare
        {
            public string base_name;
            public string full_name;
            public bool[] bools;
            public int level;

            public int round;
            public int count;

            public bloon_compare(string base_name, string full_name, bool[] bools, int level,
                int round, int count)
            {
                this.base_name = base_name;
                this.full_name = full_name;
                this.bools = bools;
                this.level = level;
                this.round = round;
                this.count = count;
            }

            public static bool operator ==(bloon_compare a, bloon_compare b)
            {
                bool x = true;
                if (a.base_name != b.base_name)
                {
                    x = false;
                }
                for(int i = 0; i < a.bools.Length; i++)
                {
                    if (a.bools[i] != b.bools[i])
                    {
                        x = false;
                        break;
                    }
                }
                if (a.level != b.level)
                {
                    x = false;
                }
                return x;
            }
            public static bool operator !=(bloon_compare a, bloon_compare b)
            {
                bool x = !(a == b);
                if (a.base_name != b.base_name)
                {
                    x = false;
                }
                return x;
            }

            public static bool operator >(bloon_compare a, bloon_compare b)
            {
                bool x = a != b;
                for (int i = 0; i < a.bools.Length; i++)
                {
                    if (!a.bools[i] && b.bools[i])
                    {
                        x = false;
                        break;
                    }
                }
                if (a.level < b.level)
                {
                    x = false;
                }
                return x;
            }
            public static bool operator <(bloon_compare a, bloon_compare b)
            {
                bool x = a != b;
                for (int i = 0; i < a.bools.Length; i++)
                {
                    if (a.bools[i] && !b.bools[i])
                    {
                        x = false;
                        break;
                    }
                }
                if (a.level > b.level)
                {
                    x = false;
                }
                return x;
            }
        };

        Dictionary<string, Tuple<int ,int>> bloon_debuts = new Dictionary<string, Tuple<int, int>>();
        Dictionary<string, List<bloon_compare>> type_debuts = 
            new Dictionary<string, List<bloon_compare>>();
        private void create_bloon_debuts()
        {
            bloon_debuts.Clear();
            foreach (var pair in all_bt)
            {
                bloon_debuts.Add(pair.name, new Tuple<int, int>(-1, -1));
            }

            int start = get_start_round();
            if (start == 0)
            {
                start = 1;
            }
            int end = get_end_round();
            if (end == 150)
            {
                end = 140;
            }
            for (int i = start; i <= end; i++)
            {
                round r = rounds[i];
                foreach (var be in r.bloon_entrys)
                {
                    if (bloon_debuts.ContainsKey(be.bloon_name) &&
                        bloon_debuts[be.bloon_name].Item1 != -1)
                    {
                        Tuple<int, int> old = bloon_debuts[be.bloon_name];
                        if (old.Item1 == i)
                        {
                            bloon_debuts[be.bloon_name] = new Tuple<int, int>(
                                old.Item1, old.Item2 + be.count);
                        }
                    }
                    else
                    {
                        bloon_debuts[be.bloon_name] = new Tuple<int, int>(i, be.count);
                    }
                }
            }
        }

        private Dictionary<int, List<bloon_compare>> type_debuts_roundsort =
            new Dictionary<int, List<bloon_compare>>();
        private List<bloon_compare> type_debuts_roundsort_list =
            new List<bloon_compare>();
        private void create_type_debuts()
        {
            type_debuts = new Dictionary<string, List<bloon_compare>>();
            int start = get_start_round();
            if(start == 0)
            {
                start = 1;
            }
            int end = get_end_round();
            if (end == 150)
            {
                end = 140;
            }
            for (int i = start; i <= end; i++)
            {
                round r = rounds[i];
                foreach(var be in r.bloon_entrys)
                {
                    string base_name = be.bloon_name;
                    try_remove_number(ref base_name);
                    bloon_compare bc = new bloon_compare(base_name, bloon_name_mix(be),
                        new bool[7]
                        {
                            be.camo, be.regrow, be.fort, be.lead, be.purple, be.zebra, be.elite
                        }, be.level, i, be.count);
                    if (type_debuts.ContainsKey(base_name))
                    {
                        bool suppressed = false;
                        bool earlier = true;
                        List<bloon_compare> list = type_debuts[base_name];
                        for (int k = 0; k < list.Count; k++)
                        {
                            bloon_compare old = list[k];
                            if (bc == old && bc.round == old.round)
                            {
                                list[k] = new bloon_compare(
                                    base_name, bloon_name_mix(be),
                                    new bool[7]
                                    {
                                        be.camo, be.regrow, be.fort, be.lead, 
                                        be.purple, be.zebra, be.elite
                                    }, be.level, i, be.count + old.count);
                            }
                            if (bc > old && bc.round == old.round)
                            {
                                list.RemoveAt(k);
                            }
                            if (bc.round >= old.round)
                            {
                                earlier = false;
                            }
                            if (bc < old || bc == old)
                            {
                                suppressed = true;
                            }
                        }
                        if (!suppressed || earlier)
                        {
                            list.Add(bc);
                        }
                    }
                    else
                    {
                        type_debuts[base_name] = new List<bloon_compare>
                        {
                            bc
                        };
                    }
                }
            }

            type_debuts_roundsort = new Dictionary<int, List<bloon_compare>>();
            foreach (var pair in type_debuts)
            {
                foreach(var bc in pair.Value)
                {
                    int round = bc.round;
                    if (type_debuts_roundsort.ContainsKey(round))
                    {
                        type_debuts_roundsort[round].Add(bc);
                    }
                    else
                    {
                        type_debuts_roundsort[round] = new List<bloon_compare>
                        {
                            bc,
                        };
                    }
                }
            }

            type_debuts_roundsort_list = new List<bloon_compare>();
            for(int i = 1; i <= 140; i++)
            {
                if (type_debuts_roundsort.ContainsKey(i))
                {
                    foreach (var bc in type_debuts_roundsort[i])
                    {
                        type_debuts_roundsort_list.Add(bc);
                    }
                }
            }
        }


        private int get_type_debut_count()
        {
            int ret = 0;
            foreach(var pair in type_debuts)
            {
                ret += pair.Value.Count;
            }
            return ret;
        }

        private List<bloon_compare> get_debut_type_list()
        {
            List<bloon_compare> curr_list = new List<bloon_compare>();
            if (debut_show_type_roundsort)
            {
                foreach (bloon_compare bc in type_debuts_roundsort_list)
                {
                    if (debut_show_type_onlycamo)
                    {
                        if (bc.bools[0])
                        {
                            curr_list.Add(bc);
                        }
                    }
                    else
                    {
                        curr_list.Add(bc);
                    }
                }
            }
            else
            {
                foreach (var pair in type_debuts)
                {
                    foreach (var bc in pair.Value)
                    {
                        if (debut_show_type_onlycamo)
                        {
                            if (bc.bools[0])
                            {
                                curr_list.Add(bc);
                            }
                        }
                        else
                        {
                            curr_list.Add(bc);
                        }
                    }
                }
            }
            return curr_list;
        }

    }
}
