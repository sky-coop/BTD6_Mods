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
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace BTD6_DIY_Rounds
{
    public partial class MainWindow : Window
    {

        private void wrap_init()
        {
            string folder = "C:\\BTD6 DIY Rounds Editor";
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string wrap = folder + "\\path.txt";
            if (File.Exists(wrap))
            {
                File.Delete(wrap);
            }
            File.Create(wrap).Close();

            File.WriteAllText(wrap, Environment.CurrentDirectory);

            wrap = folder + "\\program.txt";
            if (File.Exists(wrap))
            {
                File.Delete(wrap);
            }
            File.Create(wrap).Close();

            File.WriteAllText(wrap, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
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
        private const int SW_MAXIMIZE = 3;
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr handle);
        public void wrap()
        {
            try
            {
                bool found = false;
                {
                    Process[] processes = Process.GetProcessesByName("BloonsTD6");//模糊查找进程
                    TextBox box = (TextBox)FindName("control_pos_box");
                    //string expect_path = box.Text + "\\BloonsTD6.exe";
                    foreach (Process p in processes)
                    {
                        var handles = EnumerateProcessWindowHandles(p.Id);
                        foreach (var h in handles)
                        {
                            StringBuilder title = new StringBuilder(256);
                            GetWindowText(h, title, title.Capacity);
                            if (title.ToString() == "BloonsTD6")
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
                    WindowState = WindowState.Minimized;
                }
            }
            catch (Exception e)
            {
                float_default(10, 10, e.Message, C(255, 0, 0), C(255, 255, 255), 11, 15000);
            }
        }
    }
}
