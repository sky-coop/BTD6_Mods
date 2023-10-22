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
        [DllImport("user32.dll")]
        public static extern Int32 SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, String lParam);
        private const int WM_SETTEXT = 0x000C;

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();


        private string get_wrap_path()
        {
            string wrap = "C:\\BTD6 DIY Rounds Editor\\path.txt";
            if (File.Exists(wrap))
            {
                string s = File.ReadAllText(wrap);
                return s;
            }
            else
            {
                return "";
            }
        }
        private string get_wrap_program()
        {
            string wrap = "C:\\BTD6 DIY Rounds Editor\\program.txt";
            if (File.Exists(wrap))
            {
                string s = File.ReadAllText(wrap);
                return s;
            }
            else
            {
                return "";
            }
        }


        //获取窗口标题
        [DllImport("user32", SetLastError = true)]
        private static extern int GetWindowText(
             IntPtr hWnd,//窗口句柄
            StringBuilder lpString,//标题
            int nMaxCount //最大值
            );
        delegate bool EnumThreadDelegate(IntPtr hWnd, IntPtr lParam);


        [DllImport("user32.dll")]
        static extern bool EnumThreadWindows(int dwThreadId, EnumThreadDelegate lpfn,
            IntPtr lParam);

        static IEnumerable<IntPtr> EnumerateProcessWindowHandles(int processId)
        {
            var handles = new List<IntPtr>();

            foreach (ProcessThread thread in Process.GetProcessById(processId).Threads)
                EnumThreadWindows(thread.Id,
                    (hWnd, lParam) => { handles.Add(hWnd); return true; }, IntPtr.Zero);

            return handles;
        }
        private const int SW_MINIMIZE = 2;
        private const int SW_MAXIMIZE = 3;
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr handle);
        public void wrap()
        {
            try
            {
                string path = get_wrap_path();
                string program = get_wrap_program();
                bool found = false;
                {
                    Process[] processes = Process.GetProcessesByName(program);//模糊查找进程
                    string expect_path = path + "\\" + program + ".exe";

                    foreach (Process p in processes)
                    {
                        var handles = EnumerateProcessWindowHandles(p.Id);
                        foreach (var h in handles)
                        {
                            StringBuilder title = new StringBuilder(256);
                            GetWindowText(h, title, title.Capacity);
                            if (title.ToString().Contains(program))
                            {
                                ShowWindow(h, SW_MAXIMIZE);
                                SetForegroundWindow(h);
                                found = true;
                            }
                        }
                        /*
                        if (expect_path == p.MainModule.FileName)//比较完整路径
                        {
                        }*/
                    }
                }
                if (found)
                {
                    Process[] processes = Process.GetProcessesByName("BloonsTD6");//模糊查找进程
                    foreach (Process p in processes)
                    {
                        var handles = EnumerateProcessWindowHandles(p.Id);
                        foreach (var h in handles)
                        {
                            StringBuilder title = new StringBuilder(256);
                            GetWindowText(h, title, title.Capacity);
                            if (title.ToString() == "BloonsTD6")
                            {
                                ShowWindow(h, SW_MINIMIZE);
                                /*
                                SendMessage(h, WM_SETTEXT, IntPtr.Zero, "hello!");
                                Console.WriteLine("hello sent.");*/
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private string get_time_str()
        {
            var d = System.DateTime.Now;
            d.AddHours(8);
            return "[" + d.ToString() + "]";
        }
    }
}