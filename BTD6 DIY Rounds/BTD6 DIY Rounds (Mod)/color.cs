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

namespace DIY_Rounds
{
    public partial class Main : BloonsTD6Mod
    {
        public class ARGB
        {
            public byte a;
            public byte r;
            public byte g;
            public byte b;

            [Newtonsoft.Json.JsonConstructor]
            public ARGB(byte A, byte R, byte G, byte B)
            {
                change(A, R, G, B);
            }

            public void change(byte A, byte R, byte G, byte B)
            {
                a = A;
                r = R;
                g = G;
                b = B;
            }
        };

        private UnityEngine.Color ARGB2Ucolor(ARGB x)
        {
            UnityEngine.Color ret = new UnityEngine.Color();
            ret.a = x.a / 255.0f;
            ret.r = x.r / 255.0f;
            ret.g = x.g / 255.0f;
            ret.b = x.b / 255.0f;
            return ret;
        }
    }
}