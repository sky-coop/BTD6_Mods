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

namespace BTD6_DIY_Rounds
{
    public partial class MainWindow : Window
    {
        public class bloon_template
        {
            public string name;
            public bool has_regrow;
            public bool has_fort;
            public bool has_camo;
            public bool has_elite;

            public bool has_lead;
            public bool has_purple;
            public bool has_zebra;

            public int RBE;
            public int RBE_f;
            public int RBE_high;
            public int RBE_high_f;
            public int health_lost;
            public int health_lost_f;
            public int health_lost_high;
            public int health_lost_high_f;
            public int hp;
            public int hp_f;
            public int hp_high;
            public int hp_high_f;
            public int cash;
            public int cash_high;
            public double speed;
            public double speed_f;

            public bool moab;
            public int RBE_high_boost;

            public bloon_template(string name, bool has_regen, bool has_fort, bool has_camo,
                bool has_elite, 
                int RBE, int RBE_f,
                int RBE_high, int RBE_high_f,
                int health_lost, int health_lost_f,
                int health_lost_high, int health_lost_high_f,
                int hp, int hp_f, int hp_high, int hp_high_f,
                int cash, int cash_high, 
                double speed, double speed_f,
                bool moab = false, int RBE_high_boost = 0)
            {
                this.name = name;
                this.has_regrow = has_regen;
                this.has_fort = has_fort;
                this.has_camo = has_camo;
                this.has_elite = has_elite;
                this.RBE = RBE;
                this.RBE_f = RBE_f;
                this.RBE_high = RBE_high;
                this.RBE_high_f = RBE_high_f;
                this.health_lost = health_lost;
                this.health_lost_f = health_lost_f;
                this.health_lost_high = health_lost_high;
                this.health_lost_high_f = health_lost_high_f;
                this.hp = hp;
                this.hp_f = hp_f;
                this.hp_high = hp_high;
                this.hp_high_f = hp_high_f;
                this.cash = cash;
                this.cash_high = cash_high;
                this.speed = speed;
                this.speed_f = speed_f;
                this.moab = moab;
                this.RBE_high_boost = RBE_high_boost;
            }
        }

        Dictionary<string, List<bloon_template>> bloon_types = 
            new Dictionary<string, List<bloon_template>>();

        List<string> bloon_groups = new List<string> 
        { 
            "Original"
        };
        private bloon_template find_bt(string name)
        {
            foreach(string group in bloon_groups)
            {
                foreach(bloon_template bt in bloon_types[group])
                {
                    if(bt.name == name)
                    {
                        return bt;
                    }
                }
            }
            return null;
        }

        List<bloon_template> all_bt;
        private List<bloon_template> get_all_bt()
        {
            List<bloon_template> ret = new List<bloon_template>();
            foreach (string group in bloon_groups)
            {
                foreach (bloon_template bt in bloon_types[group])
                {
                    ret.Add(bt);
                }
            }
            return ret;
        }

        private string find_group(string name)
        {
            foreach (string group in bloon_groups)
            {
                foreach (bloon_template bt in bloon_types[group])
                {
                    if (bt.name == name)
                    {
                        return group;
                    }
                }
            }
            return null;
        }

        
        private void init_bloon_types()
        {
            bloon_types.Clear();

            bloon_types.Add("Original", new List<bloon_template>()
            {
                new bloon_template("TestBloon", false, false, false, false, 
                    999999, 999999, 999999, 999999,
                    0, 0, 0, 0,
                    999999, 999999, 999999, 999999,
                    0, 0, 
                    1.0, 1.0),
                new bloon_template("Red", true, false, true, false, 
                    1, 1, 1, 1, 
                    1, 1, 1, 1,
                    1, 1, 1, 1,
                    1, 1, 
                    1.0, 1.0),
                new bloon_template("Blue", true, false, true, false, 
                    2, 2, 2, 2, 
                    2, 2, 2, 2, 
                    1, 1, 1, 1,
                    2, 2, 
                    1.4, 1.4),
                new bloon_template("Green", true, false, true, false, 
                    3, 3, 3, 3,
                    3, 3, 3, 3,
                    1, 1, 1, 1,
                    3, 3, 
                    1.8, 1.8),
                new bloon_template("Yellow", true, false, true, false,
                    4, 4, 4, 4,
                    4, 4, 4, 4,
                    1, 1, 1, 1,
                    4, 4, 
                    3.2, 3.2),
                new bloon_template("Pink", true, false, true, false,
                    5, 5, 5, 5,
                    5, 5, 5, 5,
                    1, 1, 1, 1,
                    5, 5, 
                    3.5, 3.5),
                new bloon_template("Black", true, false, true, false, 
                    11, 11, 6, 6, 
                    11, 11, 6, 6,
                    1, 1, 1, 1,
                    11, 6, 
                    1.8, 1.8),
                new bloon_template("White", true, false, true, false,
                    11, 11, 6, 6,
                    11, 11, 6, 6,
                    1, 1, 1, 1,
                    11, 6, 
                    2.0, 2.0),
                new bloon_template("Purple", true, false, true, false,
                    11, 11, 6, 6,
                    11, 11, 6, 6,
                    1, 1, 1, 1,
                    11, 6, 
                    3.0, 3.0),
                new bloon_template("Zebra", true, false, true, false, 
                    23, 23, 7, 7,
                    23, 23, 7, 7,
                    1, 1, 1, 1,
                    23, 7, 
                    1.8, 1.8),
                new bloon_template("Lead", true, true, true, false, 
                    23, 26, 7, 10,
                    23, 26, 7, 10,
                    1, 4, 1, 4,
                    23, 7, 
                    1.0, 1.0),
                new bloon_template("Rainbow", true, false, true, false, 
                    47, 47, 8, 8,
                    47, 47, 8, 8,
                    1, 1, 1, 1,
                    47, 8, 
                    2.2, 2.2),
                new bloon_template("Ceramic", true, true, true, false, 
                    104, 114, 68, 128,
                    104, 114, 65, 75,
                    10, 20, 60, 120,
                    95, 95, 
                    2.5, 2.5),

                new bloon_template("Moab", false, true, false, false,
                    616, 856, 472, 912,
                    616, 856, 460, 700,
                    200, 400, 200, 400,
                    381, 381, 
                    1.0, 1.0, true, 200),
                new bloon_template("Bfb", false, true, false, false,
                    3164, 4824, 2588, 5048,
                    3164, 4824, 2540, 4200,
                    700, 1400, 700, 1400,
                    1525, 1525, 
                    0.25, 0.25, true, 1500),
                new bloon_template("Zomg", false, true, false, false,
                    16656, 27296, 14352, 28192,
                    16656, 27296, 14160, 24800,
                    4000, 8000, 4000, 8000,
                    6101, 6101, 
                    0.18, 0.18, true, 10000),
                new bloon_template("Ddt", false, true, true, false,
                    816, 1256, 672, 1312,
                    816, 1256, 660, 1100,
                    400, 800, 400, 800,
                    381, 381, 
                    2.64, 2.64, true, 400),
                new bloon_template("Bad", false, true, false, false,
                    55760, 98360, 50720, 100320,
                    55760, 98360, 50300, 92900,
                    20000, 40000, 20000, 40000,
                    13346, 13346, 
                    0.18, 0.18, true, 41200),

                new bloon_template("Golden", false, true, true, false,
                    8, 21, 8, 21,
                    0, 0, 0, 0,
                    8, 21, 8, 21,
                    0, 0, 
                    3.5, 3.5),

                #region Bloonarius
                new bloon_template("Bloonarius1", false, false, false, true,
                    20000, 50000, 20000, 50000,
                    32767, 0, 32767, 0,
                    20000, 50000, 20000, 50000,
                    0, 0, 
                    0.05, 0.05),
                new bloon_template("Bloonarius2", false, false, false, true,
                    75000, 300000, 75000, 300000,
                    32767, 0, 32767, 0,
                    75000, 300000, 75000, 300000,
                    0, 0, 
                    0.05, 0.05),
                new bloon_template("Bloonarius3", false, false, false, true,
                    350000, 2000000, 350000, 2000000,
                    32767, 0, 32767, 0,
                    350000, 2000000, 350000, 2000000,
                    0, 0, 
                    0.05, 0.05),
                new bloon_template("Bloonarius4", false, false, false, true,
                    750000, 8000000, 750000, 8000000,
                    32767, 0, 32767, 0,
                    750000, 8000000, 750000, 8000000,
                    0, 0, 
                    0.06, 0.06),
                new bloon_template("Bloonarius5", false, false, false, true,
                    3000000, 40000000, 3000000, 40000000,
                    32767, 0, 32767, 0,
                    3000000, 40000000, 3000000, 40000000,
                    0, 0, 
                    0.06, 0.06),
                #endregion Bloonarius
                #region Lych
                new bloon_template("Lych1", false, false, false, true,
                    14000, 30000, 14000, 30000,
                    32767, 0, 32767, 0,
                    14000, 30000, 14000, 30000,
                    0, 0,
                    0.092, 0.1),
                new bloon_template("Lych2", false, false, false, true,
                    52500, 180000, 52500, 180000,
                    32767, 0, 32767, 0,
                    52500, 180000, 52500, 180000,
                    0, 0,
                    0.092, 0.108),
                new bloon_template("Lych3", false, false, false, true,
                    220000, 1100000, 220000, 1100000,
                    32767, 0, 32767, 0,
                    220000, 1100000, 220000, 1100000,
                    0, 0,
                    0.1, 0.116),
                new bloon_template("Lych4", false, false, false, true,
                    525000, 4800000, 525000, 4800000,
                    32767, 0, 32767, 0,
                    525000, 4800000, 525000, 4800000,
                    0, 0,
                    0.108, 0.12),
                new bloon_template("Lych5", false, false, false, true,
                    2100000, 24000000, 2100000, 24000000,
                    32767, 0, 32767, 0,
                    2100000, 24000000, 2100000, 24000000,
                    0, 0,
                    0.108, 0.124),
                #endregion Lych
                #region MiniLych
                new bloon_template("MiniLych1", false, false, false, true,
                    1280, 1600, 1280, 1600,
                    100000, 100000, 100000, 100000,
                    1280, 1600, 1280, 1600,
                    0, 0,
                    1.0, 1.0),
                new bloon_template("MiniLych2", false, false, false, true,
                    3050, 5600, 3050, 5600,
                    100000, 100000, 100000, 100000,
                    3050, 5600, 3050, 5600,
                    0, 0,
                    1.0, 1.0),
                new bloon_template("MiniLych3", false, false, false, true,
                    7000, 25000, 7000, 25000,
                    100000, 100000, 100000, 100000,
                    7000, 25000, 7000, 25000,
                    0, 0,
                    1.0, 1.0),
                new bloon_template("MiniLych4", false, false, false, true,
                    14500, 100000, 14500, 100000,
                    100000, 100000, 100000, 100000,
                    14500, 100000, 14500, 100000,
                    0, 0,
                    1.0, 1.0),
                new bloon_template("MiniLych5", false, false, false, true,
                    47000, 485000, 47000, 485000,
                    100000, 100000, 100000, 100000,
                    47000, 485000, 47000, 485000,
                    0, 0,
                    1.0, 1.0),
                #endregion MiniLych
                #region Vortex
                new bloon_template("Vortex1", false, false, false, true,
                    20000, 41800, 20000, 41800,
                    32767, 0, 32767, 0,
                    20000, 41800, 20000, 41800,
                    0, 0,
                    0.144, 0.15),
                new bloon_template("Vortex2", false, false, false, true,
                    62800, 251000, 62800, 251000,
                    32767, 0, 32767, 0,
                    62800, 251000, 62800, 251000,
                    0, 0,
                    0.144, 0.162),
                new bloon_template("Vortex3", false, false, false, true,
                    294000, 1675000, 294000, 1675000,
                    32767, 0, 32767, 0,
                    294000, 1675000, 294000, 1675000,
                    0, 0,
                    0.156, 0.18),
                new bloon_template("Vortex4", false, false, false, true,
                    628000, 6700000, 628000, 6700000,
                    32767, 0, 32767, 0,
                    628000, 6700000, 628000, 6700000,
                    0, 0,
                    0.162, 0.86),
                new bloon_template("Vortex5", false, false, false, true,
                    2512500, 33500000, 2512500, 33500000,
                    32767, 0, 32767, 0,
                    2512500, 33500000, 2512500, 33500000,
                    0, 0,
                    0.168, 0.192),
                #endregion Vortex
                #region Dreadbloon
                new bloon_template("Dreadbloon1", false, false, false, true,
                    18750, 37500, 18750, 37500,
                    32767, 0, 32767, 0,
                    7500, 15000, 7500, 15000,
                    0, 0,
                    0.05, 0.052),
                new bloon_template("Dreadbloon2", false, false, false, true,
                    62500, 225000, 62500, 225000,
                    32767, 0, 32767, 0,
                    25000, 90000, 25000, 90000,
                    0, 0,
                    0.05, 0.052),
                new bloon_template("Dreadbloon3", false, false, false, true,
                    300000, 1625000, 300000, 1625000,
                    32767, 0, 32767, 0,
                    120000, 650000, 120000, 650000,
                    0, 0,
                    0.05, 0.052),
                new bloon_template("Dreadbloon4", false, false, false, true,
                    650000, 6562500, 650000, 6562500,
                    32767, 0, 32767, 0,
                    260000, 2625000, 260000, 2625000,
                    0, 0,
                    0.06, 0.06),
                new bloon_template("Dreadbloon5", false, false, false, true,
                    2500000, 31250000, 2500000, 31250000,
                    32767, 0, 32767, 0,
                    1000000, 12500000, 1000000, 12500000,
                    0, 0,
                    0.06, 0.06),
                #endregion Dreadbloon
                #region DreadRockBloon Standard/Elite
                new bloon_template("DreadRockBloon1", false, false, false, true,
                    300, 600, 300, 600,
                    5, 20, 5, 20,
                    300, 600, 300, 600,
                    0, 0,
                    0.88, 0.88),
                new bloon_template("DreadRockBloon2", false, false, false, true,
                    600, 2500, 600, 2500,
                    10, 30, 10, 30,
                    600, 2500, 600, 2500,
                    0, 0,
                    0.88, 0.88),
                new bloon_template("DreadRockBloon3", false, false, false, true,
                    900, 10000, 900, 10000,
                    15, 50, 15, 50,
                    900, 10000, 900, 10000,
                    0, 0,
                    0.88, 0.88),
                new bloon_template("DreadRockBloon4", false, false, false, true,
                    5000, 22000, 5000, 22000,
                    20, 75, 20, 75,
                    5000, 22000, 5000, 22000,
                    0, 0,
                    0.88, 0.88),
                new bloon_template("DreadRockBloon5", false, false, false, true,
                    12000, 45000, 12000, 45000,
                    25, 100, 25, 100,
                    12000, 45000, 12000, 45000,
                    0, 0,
                    0.88, 0.88),
                #endregion DreadRockBloon Standard/Elite
                #region Phayze
                new bloon_template("Phayze1", false, false, false, true,
                    17500, 35000, 17500, 35000,
                    0, 0, 0, 0,
                    10000, 20000, 10000, 20000,
                    0, 0,
                    0.036, 0.036),
                new bloon_template("Phayze2", false, false, false, true,
                    65625, 210000, 65625, 210000,
                    0, 0, 0, 0,
                    37500, 120000, 37500, 120000,
                    0, 0,
                    0.036, 0.036),
                new bloon_template("Phayze3", false, false, false, true,
                    306250, 1400000, 306250, 1400000,
                    0, 0, 0, 0,
                    175000, 800000, 175000, 800000,
                    0, 0,
                    0.036, 0.036),
                new bloon_template("Phayze4", false, false, false, true,
                    656250, 5600000, 656250, 5600000,
                    0, 0, 0, 0,
                    375000, 3200000, 375000, 3200000,
                    0, 0,
                    0.036, 0.036),
                new bloon_template("Phayze5", false, false, false, true,
                    2625000, 28000000, 2625000, 28000000,
                    0, 0, 0, 0,
                    1500000, 16000000, 1500000, 16000000,
                    0, 0,
                    0.036, 0.036),
                #endregion Phayze
            });

            bloon_template golden = find_bt("Golden");
            golden.has_zebra = true;
            golden.has_purple = true;
            golden.has_lead = true;

            /*
            bloon_types.Add("MoarBloons", new List<bloon_template>()
            {
                new bloon_template("Cash", false, false, false, false, 1, 0, 0),
                new bloon_template("Orange", true, false, true),
                new bloon_template("Grey", true, false, true),
                new bloon_template("Pixel", true, false, true),
                new bloon_template("Trans", true, false, true),
                new bloon_template("Mosaic", true, true, true),
                new bloon_template("Obsidian", true, true, true),

                new bloon_template("Lmao", false, true, false),

                new bloon_template("Troll", false, false, false),
            });*/
        }

        //GoldenLeadPurpleZebraFortifiedCamo
        private string bloon_name_mix(bloon_entry be)
        {
            string ret = be.bloon_name;
            int level = 1;
            bool has_elite = find_bt(be.bloon_name).has_elite;
            if (has_elite)
            {
                level = Convert.ToInt32(ret.Substring(ret.Length - 1));
                ret = ret.Substring(0, ret.Length - 1);
            }

            if (be.lead)
            {
                ret += "Lead";
            }
            if (be.purple)
            {
                ret += "Purple";
            }
            if (be.zebra)
            {
                ret += "Zebra";
            }

            if (be.regrow)
            {
                ret += "Regrow";
            }
            if (be.fort)
            {
                ret += "Fortified";
            }
            if (be.camo)
            {
                ret += "Camo";
            }

            if (has_elite)
            {
                if (be.elite)
                {
                    ret += "Elite";
                }
                else
                {
                    if (be.bloon_name.Contains("DreadRockBloon"))
                    {
                        ret += "Standard";
                    }
                }
                ret += level;
                be.level = level;
            }

            return ret;
        }

        private static bool try_remove_substring(ref string source, string substring)
        {
            if (source.Contains(substring))
            {
                source = source.Replace(substring, "");
                return true;
            }
            else
            {
                return false;
            }
        }

        private static void try_remove_number(ref string source)
        {
            source = Regex.Replace(source, "[0-9]", "");
        }

        private decimal RBE(bloon_entry be, int round)
        {
            bloon_template bt = find_bt(be.bloon_name);
            decimal RBE = 1;
            if (round > 80)
            {
                if (be.fort || be.elite)
                {
                    RBE = bt.RBE_high_f;
                }
                else
                {
                    RBE = bt.RBE_high;
                }
                if (bt.moab)
                {
                    double mul = 1;
                    if(be.fort)
                    {
                        mul = 2;
                    }
                    mul *= health_mul(round) - 1;
                    RBE += (decimal)(mul * bt.RBE_high_boost);
                }
            }
            else
            {
                if (be.fort || be.elite)
                {
                    RBE = bt.RBE_f;
                }
                else
                {
                    RBE = bt.RBE;
                }
            }
            if (be.bloon_name == "Golden")
            {
                RBE = golden_health(be);
            }
            return RBE * be.count;
        }
        private double CASH(bloon_entry be, int round)
        {
            bloon_template bt = find_bt(be.bloon_name);
            long cash = be.count;
            if (round > 80)
            {
                cash *= bt.cash_high;
            }
            else
            {
                cash *= bt.cash;
            }
            return cash * cash_mul(round) * attrs["cash multiplier[pop]"];
        }
        private long LOST(bloon_entry be, int round)
        {
            bloon_template bt = find_bt(be.bloon_name);
            long lost = be.count;
            if (round > 80)
            {
                if (be.fort || be.elite)
                {
                    lost *= bt.health_lost_high_f;
                }
                else
                {
                    lost *= bt.health_lost_high;
                }
            }
            else
            {
                if (be.fort || be.elite)
                {
                    lost *= bt.health_lost_f;
                }
                else
                {
                    lost *= bt.health_lost;
                }
            }
            return lost;
        }
        private double HP(bloon_entry be, int round)
        {
            bloon_template bt = find_bt(be.bloon_name);
            double hp = bt.hp;
            if (round > 80)
            {
                hp = bt.hp_high;
            }
            if (be.fort || be.elite)
            {
                hp = bt.hp_f;
                if(round > 80)
                {
                    hp = bt.hp_high_f;
                }
            }
            if (bt.moab)
            {
                hp *= health_mul(round);
            }
            if (be.bloon_name == "Golden")
            {
                hp = golden_health(be);
            }
            return hp;
        }
        private double SPEED(bloon_entry be, int round)
        {
            bloon_template bt = find_bt(be.bloon_name);
            double speed = bt.speed;
            if (be.fort || be.elite)
            {
                speed = bt.speed_f;
            }
            if (!bt.has_elite)
            {
                speed *= speed_mul(round);
            }
            return speed;
        }
        /*
        private double cash_mul(int r)
        {
            double c = 0;
            if (r <= 50) c = 1;
            else if (r <= 60) c = 0.5;
            else if (r <= 85) c = 0.2;
            else if (r <= 100) c = 0.1;
            else if (r <= 120) c = 0.05;
            else c = 0.02;
            return c;
        }*/
        private double cash_mul(int r)
        {
            double c = 1;
            foreach(Tuple<int, double> tuple in round_cash_mul_attrs)
            {
                if (r > tuple.Item1)
                {
                    c = tuple.Item2;
                    break;
                }
            }
            return c;
        }
        private double health_mul(int r)
        {
            double h = 0;
            if (r <= 80) h = 1;
            else if (r <= 100) h = 1 + (r - 80) * 0.02;
            else if (r <= 124) h = 1.45 + (r - 101) * 0.05;
            else if (r <= 150) h = 2.75 + (r - 125) * 0.15;
            else if (r <= 250) h = 6.85 + (r - 151) * 0.35;
            else if (r <= 300) h = 42.5 + (r - 251);
            else if (r <= 400) h = 93 + (r - 301) * 1.5;
            else if (r <= 500) h = 244 + (r - 401) * 2.5;
            else h = 5 * r - 2008.5;
            return h;
        }
        private double speed_mul(int r)
        {
            double v = 0;
            if (r <= 80) v = 1;
            else if (r <= 100) v = 1 + (r - 80) * 0.02;
            else if (r <= 150) v = 1.6 + (r - 101) * 0.02;
            else if (r <= 200) v = 3.0 + (r - 151) * 0.02;
            else if (r <= 250) v = 4.5 + (r - 201) * 0.02;
            else v = 6.0 + (r - 252) * 0.02;
            return v;
        }
        /*
         *  
         *  Lead -> Fortified
         *  Lead -> Purple -> Zebra
         *  
            Golden
            GoldenCamo
            GoldenLead
            GoldenLeadCamo
            GoldenLeadFortified
            GoldenLeadFortifiedCamo
            GoldenLeadPurple
            GoldenLeadPurpleCamo
            GoldenLeadPurpleFortified
            GoldenLeadPurpleFortifiedCamo
            GoldenLeadPurpleZebra
            GoldenLeadPurpleZebraCamo
            GoldenLeadPurpleZebraFortified
            GoldenLeadPurpleZebraFortifiedCamo
         */
        private int golden_health(bloon_entry be)
        {
            int n = 8;
            if (be.lead)
            {
                n += 6;
            }
            if (be.fort)
            {
                n += 7;
            }
            if (be.purple)
            {
                if (be.fort)
                {
                    n += 2;
                }
                else
                {
                    n += 1;
                }
            }
            if (be.zebra)
            {
                if (be.fort)
                {
                    n += 4;
                }
                else
                {
                    n += 3;
                }
            }
            return n;
        }
    }
}
