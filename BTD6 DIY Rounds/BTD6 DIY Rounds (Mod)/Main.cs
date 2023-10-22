using MelonLoader;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Models.Rounds;
using System;
using System.IO;
using System.Collections.Generic;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Api.Bloons;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models;
using BTD_Mod_Helper.Api.ModOptions;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Simulation.Bloons;
using Il2CppAssets.Scripts.Simulation.Track;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Newtonsoft.Json;
using System.Threading;
using System.Text.RegularExpressions;
using static Il2Cpp.Interop;
using FuzzySharp.Edits;
using UnityEngine.InputSystem;
using System.Diagnostics;
using Il2CppAssets.Scripts.Unity;
using UnityEngine;
using Il2CppAssets.Scripts.Simulation;
using Il2CppAssets.Scripts.Simulation.Towers;
using static Il2CppSystem.Linq.Expressions.Interpreter.CastInstruction.CastInstructionNoT;
using static UnityEngine.UIElements.StylePropertyAnimationSystem;
using System.Xml.Linq;
using BTD_Mod_Helper.UI.Modded;
using Il2CppNinjaKiwi.Common;
using System.Drawing;
using Il2CppAssets.Scripts.Models.Towers.Upgrades;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Unity.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using UnityEngine.Playables;
using static UnityEngine.SpookyHash;
using static Il2CppNinjaKiwi.LiNK.DataModels.Invitation;

[assembly: MelonInfo(typeof(DIY_Rounds.Main), DIY_Rounds.ModHelperData.Name, DIY_Rounds.ModHelperData.Version, DIY_Rounds.ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace DIY_Rounds
{
    public partial class Main : BloonsTD6Mod
    {
        private class pack
        {
            public Dictionary<string, List<bloon_entry_json>> rounds;
            public Dictionary<string, double> attrs;
            public Dictionary<int, Tuple<ARGB, string>> hints;
        }
        pack pa;

        private class def_attrs
        {
            public double start_cash;
            public int start_round;
            public int end_round;
            public double health;
            public double max_health;
        }
        def_attrs da;

        Dictionary<string, double> name_saves;

        private class bloon_entry_json
        {
            public string bloon_name;
            public string start_time;
            public string end_time;
            public string amount;
        }

        public Timer ticker = null;
        public Timer try_ticker = null;
        public override void OnApplicationStart()
        {
            base.OnApplicationStart();
            try
            {
                if (!Directory.Exists(folder_path))
                {
                    Directory.CreateDirectory(folder_path);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        const string folder_path = "Mods/DIY Rounds/";
        const string rounds_path = folder_path + "rounds.json";
        const string old_path = folder_path + "old rounds.json";

        public void change_rounds(GameModel model)
        {
            if (!custom)
            {
                return;
            }

            rounds_last_change_time = RoundsLastChangeTime();
            var rounds = model.roundSet.rounds;
            if (!Directory.Exists(folder_path))
            {
                Console.WriteLine(get_time_str() + "[DIY Rounds] 文件夹 " + folder_path + " 不存在！");
                return;
            }
            try
            {
                string s = File.ReadAllText(rounds_path);
                pa = JsonConvert.DeserializeObject<pack>(s);

                foreach (KeyValuePair<string, List<bloon_entry_json>> pair in pa.rounds)
                {
                    int r = Convert.ToInt32(pair.Key.Split(' ')[1]);
                    if (r > rounds.Count)
                    {
                        continue;
                    }
                    try
                    {
                        List<bloon_entry_json> list = pair.Value;

                        RoundModel rm = rounds[r - 1];
                        List<BloonGroupModel> groups = new List<BloonGroupModel>();
                        foreach (bloon_entry_json b in list)
                        {
                            float start = float.Parse(b.start_time) / 1000 * 60;
                            float end = float.Parse(b.end_time) / 1000 * 60;
                            int amount = int.Parse(b.amount);
                            groups.Add(new BloonGroupModel("", b.bloon_name, start, end, amount));
                        }
                        rm.groups = groups.ToArray();
                        rm.emissions_ = null;
                    }
                    catch
                    {
                        Console.WriteLine(get_time_str() + "[DIY Rounds] 回合 " + r +" 加载失败");
                    }
                }

                for(int i = 1; i <= 140; i++)
                {
                    LocalizationManager.Instance.textTable.Remove("Hint " + i);
                    LocalizationManager.Instance.textTable.Remove("Alternate Hint " + i);
                }
                foreach(var pair in pa.hints)
                {
                    int round = pair.Key;
                    var tuple = pair.Value;
                    LocalizationManager.Instance.textTable["Hint " + round] = tuple.Item2;
                    LocalizationManager.Instance.textTable["Alternate Hint " + round] = tuple.Item2;
                }

                round_cash_mul_attrs = get_round_cash_mul_attrs();
                double value;
                
                value = pa.attrs["cash multiplier[sell]"];
                if (!double.IsNaN(value))
                {
                    model.sellMultiplier = (float)value;
                }

                value = pa.attrs["start round"];
                if (!double.IsNaN(value))
                {
                    model.startRound = (int)value;
                }
                else
                {
                    model.startRound = da.start_round;
                }

                value = pa.attrs["end round"];
                if (!double.IsNaN(value))
                {
                    model.endRound = (int)value;
                }
                else
                {
                    model.endRound = da.end_round;
                }
                if(model.startRound > model.endRound)
                {
                    model.startRound = model.endRound;
                }

                value = pa.attrs["player health"];
                if (!double.IsNaN(value))
                {
                    model.startingHealth = (int)value;
                }
                else
                {
                    model.startingHealth = (float)da.health;
                }

                value = pa.attrs["player max health"];
                if (!double.IsNaN(value))
                {
                    model.maxHealth = (int)value;
                }
                else
                {
                    model.maxHealth = (float)da.max_health;
                }

                tower_change(model);
                var ingame_model = curr_model();
                if (ingame_model != null)
                {
                    tower_change(ingame_model);
                }

                Console.WriteLine(get_time_str() + "[DIY Rounds] 回合1" + "～"
                + rounds.Count + "加载完成！");
            }
            catch (Exception ex)
            {
                Console.WriteLine(get_time_str() + "[DIY Rounds] " + ex.Message);
                Console.WriteLine(get_time_str() + "[DIY Rounds] 回合加载失败");
            }
        }

        private static readonly ModSettingBool custom = new(true)
        {
            displayName = "使用自定义回合",
            button = true,
        };


        private GameModel curr_model()
        {
            if(InGame.instance == null)
            {
                return null;
            }
            GameModel g = InGame.instance.GetGameModel();
            if (g != null)
            {
                return g;
            }
            return null;
        }


        public override void OnNewGameModel(GameModel result)
        {
            save_rounds(result, "old rounds.json");
            da = new def_attrs();
            da.start_round = result.startRound;
            da.end_round = result.endRound;
            da.health = result.startingHealth;
            da.max_health = result.maxHealth;
            if (custom)
            {
                lost_cash = 0;
                name_saves = new Dictionary<string, double>();
                Console.WriteLine(get_time_str() + "[DIY Rounds] 加载自定义回合中...");
                change_rounds(result);

                result.doubleCashAllowed = false;
                if (pa != null)
                {
                    double value = pa.attrs["start cash"];
                    if (!double.IsNaN(value))
                    {
                        change_start_cash = true;
                        change_cash_amount = Math.Min(value, 1e9);
                    }
                }
                record_start_cash = true;
                //ingameext addhealth
            }
        }

        public override void OnRestart()
        {
            base.OnRestart();
            started = false;
            if (custom && pa != null)
            {
                double value = pa.attrs["start cash"];
                if (!double.IsNaN(value))
                {
                    change_start_cash = true;
                    change_cash_amount = value;
                }
                else
                {
                    change_start_cash_old = true;
                }
            }
        }

        public override void OnMatchStart()
        {
            base.OnMatchStart();
            started = false;
            int round = InGame.instance.bridge.GetCurrentRound() + 1;
            if (custom && pa != null)
            {
                double value;
                var model = curr_model();
                value = pa.attrs["start round"];
                if (!double.IsNaN(value))
                {
                    model.startRound = (int)value;
                }
                else
                {
                    model.startRound = da.start_round;
                }

                value = pa.attrs["end round"];
                if (!double.IsNaN(value))
                {
                    model.endRound = (int)value;
                }
                else
                {
                    model.endRound = da.end_round;
                }
                if (model.startRound > model.endRound)
                {
                    model.startRound = model.endRound;
                }

                value = pa.attrs["player health"];
                if (!double.IsNaN(value))
                {
                    model.startingHealth = (int)value;
                }
                else
                {
                    model.startingHealth = (float)da.health;
                }

                value = pa.attrs["player max health"];
                if (!double.IsNaN(value))
                {
                    model.maxHealth = (int)value;
                }
                else
                {
                    model.maxHealth = (float)da.max_health;
                }
            }
            if (custom && pa != null && pa.hints.ContainsKey(round))
            {
                var ins = InGame.instance;
                if (pa.hints.ContainsKey(round))
                {
                    var tuple = pa.hints[round];
                    LocalizationManager.Instance.textTable["Hint " + round] = tuple.Item2;
                    LocalizationManager.Instance.textTable["Alternate Hint " + round] = tuple.Item2;
                    ins.roundHintTxt.color = ARGB2Ucolor(pa.hints[round].Item1);
                    ins.roundHintAutoHideTime = 5;
                }
                else
                {
                    ins.roundHintTxt.text = "";
                }
            }
        }

        bool started = false;
        public override void OnRoundStart()
        {
            base.OnRoundStart();
            started = true;
            stopwatch.Stop();
            stopwatch.Restart();
        }
        public override void OnTitleScreen()
        {
            base.OnTitleScreen();
            started = false;
        }
        public override void OnRoundEnd()
        {
            base.OnRoundEnd(); 
            started = false;

            if (custom && pa != null)
            {

                var ins = InGame.instance;
                var bridge = ins.bridge;
                int round = bridge.GetCurrentRound() + 1;
                if (pa.hints.ContainsKey(round))
                {
                    var tuple = pa.hints[round];
                    LocalizationManager.Instance.textTable["Hint " + round] = tuple.Item2;
                    LocalizationManager.Instance.textTable["Alternate Hint " + round] = tuple.Item2;
                    ins.roundHintTxt.color = ARGB2Ucolor(pa.hints[round].Item1);
                    ins.roundHintAutoHideTime = 5;
                }
                else
                {
                    /*
                    LocalizationManager.Instance.textTable["Hint " + round] = "";
                    LocalizationManager.Instance.textTable["Alternate Hint " + round] = "";*/
                    ins.roundHintTxt.text = "";
                    //ins.roundHintAutoHideTime = 0.01f;
                }

                double value;

                double health = ins.GetHealth();
                double max = ins.GetMaxHealth();
                health = (health + pa.attrs["HEALTHY[add]"]) * (1 + pa.attrs["HEALTHY[mul]"]);
                health = Math.Min(health, 1e9);
                ins.SetHealth(health);
                if (health > max)
                {
                    ins.SetMaxHealth(health);
                }

                double cash = ins.GetCash();
                cash = cash * (1 + pa.attrs["AUTOFARM[mul]"]);
                cash = Math.Min(cash, 1e9);
                ins.SetCash(cash);
            }
        }
        //TowerSet 塔


        Stopwatch stopwatch = new Stopwatch();
        bool first_watch = true;
        bool change_start_cash = false;
        bool change_start_cash_old = false;
        bool record_start_cash = false;
        double change_cash_amount = 0;
        double lost_cash = 0;

        DateTime rounds_last_change_time = DateTime.MinValue;
        public DateTime RoundsLastChangeTime()
        {
            try
            {
                return File.GetLastWriteTime(rounds_path);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }
        public void hot_update_round()
        {
            if (rounds_last_change_time < RoundsLastChangeTime()) 
            {
                try
                {
                    GameModel model = curr_model();
                    Console.WriteLine(get_time_str() + "[DIY Rounds] 检测到回合文件修改，尝试重新加载回合中...");
                    change_rounds(model);
                }
                catch
                {
                    return;
                }
            }
        }

        public void save_rounds(GameModel model, string filename)
        {
            string filepath = folder_path + filename;
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }

            Dictionary<string, List<bloon_entry_json>> dic =
                new Dictionary<string, List<bloon_entry_json>>();

            var roundset = model.roundSet;
            var rounds = roundset.rounds;
            int i = 1;
            foreach (RoundModel rm in rounds)
            {
                var group = rm.groups;
                List<bloon_entry_json> round = new List<bloon_entry_json>();
                foreach (var item in group)
                {
                    bloon_entry_json bej = new bloon_entry_json()
                    {
                        bloon_name = item.bloon,
                        start_time = (item.start * 1000 / 60).ToString("f2"),
                        end_time = (item.end * 1000 / 60).ToString("f2"),
                        amount = item.count.ToString(),
                    };
                    round.Add(bej);
                }
                dic.Add("Round " + i, round);
                i++;
            }
            pack p = new pack();
            p.rounds = dic;
            string s = JsonConvert.SerializeObject(p);
            File.WriteAllText(filepath, s);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
                && Input.GetKeyDown(KeyCode.X))
            {
                wrap();
            }

            if (custom && curr_model() != null)
            {
                hot_update_round();
            }
            try
            {
                if (InGame.instance != null && InGame.instance.bridge != null
                    && InGame.instance.GetCashManager() != null)
                {
                    if (change_start_cash)
                    {
                        if (InGame.instance.bridge.IsSandboxMode())
                        {
                            InGameExt.SetHealth(InGame.instance, 9999999);
                            InGameExt.SetMaxHealth(InGame.instance, 9999999);
                            InGameExt.SetCash(InGame.instance, 9999999);
                            InGameExt.SetRound(InGame.instance, 0);
                        }
                        else
                        {
                            InGameExt.SetCash(InGame.instance, change_cash_amount);
                        }
                        change_start_cash = false;
                    }
                    if (!record_start_cash && change_start_cash_old)
                    {
                        if (InGame.instance.bridge.IsSandboxMode())
                        {
                            InGameExt.SetHealth(InGame.instance, 9999999);
                            InGameExt.SetMaxHealth(InGame.instance, 9999999);
                            InGameExt.SetCash(InGame.instance, 9999999);
                            InGameExt.SetRound(InGame.instance, 0);
                        }
                        else
                        {
                            InGameExt.SetCash(InGame.instance, da.start_cash);
                        }
                        change_start_cash_old = false;
                    }
                }
            }
            catch
            {

            }
            if (record_start_cash)
            {
                try
                {
                    if (InGame.instance != null && InGame.instance.bridge != null
                        && InGame.instance.GetCashManager() != null)
                    {
                        record_start_cash = false;
                        da.start_cash = InGame.instance.GetCash();
                    }
                }
                catch
                {

                }
            }
            try
            {
                if (custom && InGame.instance != null && InGame.instance.bridge != null)
                {
                    if (started)
                    {
                        var ins = InGame.instance;
                        int watch_time = 0;
                        if (first_watch)
                        {
                            stopwatch.Start();
                            first_watch = false;
                        }
                        else
                        {
                            stopwatch.Stop();
                            watch_time = (int)stopwatch.ElapsedMilliseconds;
                            stopwatch.Restart();
                        }


                        double time = watch_time / 1000.0 * get_speed();
                        if (pa != null)
                        {
                            if (ins == null)
                            {
                                return;
                            }
                            double cash = ins.GetCash();
                            double sub_cash = pa.attrs["TIMETAX[/s]"] * time + lost_cash;
                            sub_cash = Math.Min(sub_cash, 1e9);
                            if (cash < sub_cash)
                            {
                                sub_cash -= cash;
                                lost_cash = sub_cash;
                                cash = 0;
                            }
                            else
                            {
                                cash -= sub_cash;
                                lost_cash = 0;
                            }

                            cash = cash * Math.Pow(1 - pa.attrs["TIMETAX[mul]"], time);
                            ins.SetCash(cash);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(get_time_str() + e);
            }
            
        }


        public override void OnCashAdded(double amount, Simulation.CashType from, 
            int cashIndex, Simulation.CashSource source, 
            Il2CppAssets.Scripts.Simulation.Towers.Tower tower)
        {
            base.OnCashAdded(amount, from, cashIndex, source, tower);
            int round = InGame.instance.UnityToSimulation.GetCurrentRound() + 1;
            if (custom && pa != null)
            {
                double add = 0;
                double expect_mul;
                double actual_mul;
                double to_add_mul;
                switch (source)
                {
                    case Simulation.CashSource.Normal:
                        if (from == Simulation.CashType.EndOfRound)
                        {
                            expect_mul = pa.attrs["cash multiplier[end round]"];
                            actual_mul = 1;
                        }
                        else
                        {
                            expect_mul = pa.attrs["cash multiplier[pop]"] * cash_mul(round);
                            actual_mul = cash_mul_natural(round);
                        }
                        to_add_mul = (expect_mul / actual_mul) - 1;
                        add = amount * to_add_mul;
                        break;
                        /*
                    case Simulation.CashSource.TowerSold:
                        expect_mul = pa.attrs["cash multiplier[sell]"];
                        if (!double.IsNaN(expect_mul))
                        {
                            to_add_mul = expect_mul - 0.7;
                            add = tower.worth * to_add_mul;
                        }
                        break;*/
                    case Simulation.CashSource.EcoEarned:
                        expect_mul = pa.attrs["cash multiplier[farm]"];
                        to_add_mul = expect_mul - 1;
                        add = amount * to_add_mul;
                        if (tower != null && tower.towerModel != null
                            && tower.towerModel.name.Contains("EngineerMonkey"))
                        {
                            expect_mul = pa.attrs["cash multiplier[pop]"] * cash_mul(round);
                            actual_mul = cash_mul_natural(round);
                            to_add_mul = (expect_mul / actual_mul) - 1;
                            add = amount * to_add_mul;
                        }
                        break;
                }
                InGameExt.AddCash(InGame.instance, add);
            }
        }

        private List<Tuple<int, double>> round_cash_mul_attrs;
        private List<Tuple<int, double>> get_round_cash_mul_attrs()
        {
            List<Tuple<int, double>> ret = new List<Tuple<int, double>>();
            for (int i = 140; i >= 1; i--)
            {
                string s = "cash multiplier[pop][round after " + i + "]";
                if (pa.attrs.ContainsKey(s))
                {
                    ret.Add(new Tuple<int, double>(i, pa.attrs[s]));
                }
            }
            return ret;
        }

        private double cash_mul_natural(int r)
        {
            double c = 0;
            if (r <= 50) c = 1;
            else if (r <= 60) c = 0.5;
            else if (r <= 85) c = 0.2;
            else if (r <= 100) c = 0.1;
            else if (r <= 120) c = 0.05;
            else c = 0.02;
            return c;
        }
        private double cash_mul(int r)
        {
            double c = 1;
            foreach (Tuple<int, double> tuple in round_cash_mul_attrs)
            {
                if (r > tuple.Item1)
                {
                    c = tuple.Item2;
                    break;
                }
            }
            return c;
        }
    }
}
