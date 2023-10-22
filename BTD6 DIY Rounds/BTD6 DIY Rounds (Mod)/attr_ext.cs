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
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text;
using UnityEngine.UIElements;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Upgrades;
using Il2CppAssets.Scripts.Models.Towers;
using static Il2CppSystem.Linq.Expressions.Interpreter.CastInstruction.CastInstructionNoT;
using Il2CppAssets.Scripts.Utils;

namespace DIY_Rounds
{
    public partial class Main : BloonsTD6Mod
    {
        private bool name_exists(string a)
        {
            return name_saves.ContainsKey(a);
        }

        private void tower_change(GameModel model)
        {
            double value;
            try
            {
                #region price
                value = pa.attrs["tower price multiplier"];
                foreach (TowerModel towerModel in model.towers)
                {
                    string name = towerModel.name + "_price";
                    if (name_exists(name))
                    {
                        towerModel.cost *= (float)(value / name_saves[name]);
                    }
                    else
                    {
                        towerModel.cost *= (float)value;
                    }
                    name_saves[name] = value;
                }
                foreach (UpgradeModel upgradeModel in model.upgrades)
                {
                    string name = upgradeModel.name + "_price";
                    if (name_exists(name))
                    {
                        upgradeModel.cost = (int)
                            (upgradeModel.cost * value / name_saves[name]);
                    }
                    else
                    {
                        upgradeModel.cost = (int)(upgradeModel.cost * value);
                    }
                    name_saves[name] = value;
                }
                #endregion price
                #region attack speed
                value = pa.attrs["tower attack speed multiplier"];
                double value2 = pa.attrs["tower damage multiplier"];
                foreach (var tower in model.towers)
                {
                    string name = tower.name + "_attackspeed";
                    bool exist = name_exists(name);
                    double mul = value;
                    if (exist)
                    {
                        mul /= name_saves[name];
                    }

                    string name2 = tower.name + "_damage";
                    bool exist2 = name_exists(name2);
                    double mul2 = value2;
                    if (exist2)
                    {
                        mul2 /= name_saves[name2];
                    }
                    foreach (var attackModel in tower.GetAttackModels())
                    {
                        foreach (var weapon in attackModel.weapons)
                        {
                            weapon.Rate /= (float)mul;
                            var p = weapon.projectile;
                            if (p != null)
                            {
                                var dm = weapon.projectile.GetDamageModel();
                                if (dm != null)
                                {
                                    dm.damage *= (float)mul2;
                                    dm.maxDamage *= (float)mul2;
                                }
                            }
                        }
                    }
                    foreach (var ability in tower.GetAbilities())
                    {
                        foreach (var activateAttackModel in
                            ability.GetBehaviors<ActivateAttackModel>())
                        {
                            foreach (var attackModel in activateAttackModel.attacks)
                            {
                                foreach (var weapon in attackModel.weapons)
                                {
                                    weapon.Rate /= (float)mul;
                                    var p = weapon.projectile;
                                    if (p != null)
                                    {
                                        var dm = weapon.projectile.GetDamageModel();
                                        if (dm != null)
                                        {
                                            weapon.projectile.GetDamageModel().damage *= (float)mul2;
                                            weapon.projectile.GetDamageModel().maxDamage *= (float)mul2;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    name_saves[name] = value;
                    name_saves[name2] = value2;
                }
                #endregion attack speed
                #region range
                value = pa.attrs["tower range multiplier"];
                foreach (var tower in model.towers)
                {
                    string name = tower.name + "_range";
                    bool exist = name_exists(name);
                    double mul = value;
                    if (exist)
                    {
                        mul /= name_saves[name];
                    }
                    tower.range *= (float)mul;
                    name_saves[name] = value;
                    foreach (var behavior in tower.behaviors)
                    {
                        try
                        {
                            name = behavior.name + "_range";
                            exist = name_exists(name);
                            mul = value;
                            if (exist)
                            {
                                mul /= name_saves[name];
                            }
                            behavior.Cast<AttackModel>().range *= (float)mul;
                            name_saves[name] = value;
                        }
                        catch
                        {

                        }
                    }
                }
                #endregion range
                #region tower

                //foreach(var towermodel in model.towers)
                //{
                //}
                //foreach (var upgradeModel in model.upgrades)
                //{
                //    upgradeModel.locked = 1;
                //}
                //if (InGame.instance != null)
                //{
                //    foreach (var tower in InGame.instance.GetTowers())
                //    {
                //        var tm = tower.towerModel;
                //    }
                //}
                #endregion tower
                #region bloon
                foreach (var bloonmodel in model.bloons)
                {
                    //bloonmodel.isMoab
                    //bloonmodel.SetRegrow(bloonmodel.id, 3);
                    //bloonmodel.IsRegrowBloon
                    //bloonmodel.speed
                }
                #endregion bloon
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private double get_speed()
        {
            return TimeManager.timeScaleWithoutNetwork;
        }
    }
}