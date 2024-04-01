using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace BTD6_DIY_Rounds
{
    public partial class MainWindow : Window
    {
        Stopwatch stopwatch = new Stopwatch();
        bool first_watch = true;

        int tick_time = 40;
        int watch_time = 0;
        int watch_fail_count = 0;
        int success_count = 0;
        public DispatcherTimer ticker = new DispatcherTimer();
        public DispatcherTimer show_ticker = new DispatcherTimer();
        public void time_start()
        {
            ticker.Interval = new TimeSpan(0, 0, 0, 0, tick_time);
            ticker.Tick += tick;
            ticker.Start();
        }
        public void time_show_start()
        {
            show_ticker.Interval = new TimeSpan(0, 0, 0, 0, 10);
            show_ticker.Tick += show_tick;
            show_ticker.Start();
        }

        bool screenshooting = false;
        string screenshooting_name = null;
        int screenshooting_level = 1;
        int screenshooting_end = 140;
        int screenshooting_page = 1;
        bool ready_to_shot = false;
        public void tick(object sender, EventArgs e)
        {
            if (first_watch)
            {
                stopwatch.Start();
                first_watch = false;
            }
            else
            {
                stopwatch.Stop();
                watch_time = (int)stopwatch.ElapsedMilliseconds;
                //test.Text = watch_time.ToString();
                stopwatch.Restart();
            }
            if (watch_fail_count > 10)
            {
                watch_fail_count = 0;
                tick_time += 10;
                ticker.Interval = new TimeSpan(0, 0, 0, 0, tick_time);
                ticker.Start();
                return;
            }
            if (success_count > 500)
            {
                success_count = 0;
                tick_time = Math.Max(40, tick_time - 10);
                ticker.Interval = new TimeSpan(0, 0, 0, 0, tick_time);
                ticker.Start();
                return;
            }
            if (watch_time < tick_time * 2)
            {
                if (!ready_to_shot)
                {
                    ready_to_shot = true;
                }
                else if (IsActive && screenshooting && ready_to_shot)
                {
                    if (screenshooting_level > screenshooting_end)
                    {
                        screenshooting = false;
                        ready_to_shot = false;

                        background_grid.Visibility = Visibility.Visible;

                        G("sh_control_grid").Visibility = Visibility.Hidden;
                        G("sh_hint_preview_grid").Children.Clear();
                        G("ext_hint_container_grid").Children.Add(G("ext_hint_preview_grid"));

                        round_select(save_round);
                        MessageBox.Show("回合文件保存完毕，截图完成！图片已保存到" +
                            "./Workspace/" + screenshooting_name + "/pics/", "提示");
                        bloon_show();
                    }
                    else
                    {
                        string s = ((TextBlock)FindName("bloons_round_text")).Text.Split('合')[1];
                        int n = int.Parse(s);
                        if (n == screenshooting_level)
                        {
                            capture("./Workspace/" + screenshooting_name + "/pics/" 
                                + screenshooting_level + "_" + 
                                screenshooting_page +".png", main_grid);
                            if (bloon_no_base + bloon_row - 1 < 
                                rounds[screenshooting_level].bloon_entrys.Count)
                            {
                                screenshooting_page++;
                                for(int i = 0; i < bloon_row; i++)
                                {
                                    button_effect("control_down");
                                }
                            }
                            else
                            {
                                screenshooting_page = 1;

                                button_effect("next_round");
                                screenshooting_level++; 
                                screen_hint_change();
                            }
                            InvalidateVisual();
                            UpdateLayout();
                        }
                    }
                    ready_to_shot = false;
                }
                success_count += watch_time;
                watch_fail_count = 0;
            }
            else
            {
                watch_fail_count++;
                if (tick_time > 50)
                {
                    success_count /= 2;
                }
                else
                {
                    success_count = 0;
                }
            }

            if (modifyed && auto_save && s_ticker("auto_save", 500, watch_time))
            {
                TextBox box = (TextBox)FindName("control_pos_box");
                if (box_ok(box))
                {
                    string folder = box.Text + "/Mods/DIY Rounds";
                    string filepath = folder + "/rounds.json";
                    if (save_rounds_game(folder))
                    {
                        if (!Directory.Exists(folder + "/pics"))
                        {
                            Directory.CreateDirectory(folder + "/pics");
                        }
                        float_default(10, 10, "自动保存成功！", C(50, 50, 50), C(0, 255, 255),
                            11);
                        modifyed = false;
                    }
                }
                else
                {
                    float_default(10, 10, "自动保存失败！请检查\"文件存取\"中游戏的路径\n" +
                        "或关闭\"设置\"中的\"自动保存到游戏\"！", 
                        C(255, 255, 255), C(255, 0, 0), 11, 1000);
                }
            }
            float_message_tick(watch_time);

            key_check();
        }

        public void show_tick(object sender, EventArgs e)
        {
            button_hint();
            ball_move();
        }

        bool entered = false;
        public void key_check()
        {
            if (WindowState == WindowState.Maximized ||
                IsActive)
            {
                if ((Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt)) 
                    && Keyboard.IsKeyDown(Key.Z))
                {
                    wrap();
                }
                
                if ((Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
                    && Keyboard.IsKeyDown(Key.Q))
                {
                    files_update();
                    MessageBox.Show("ok!");
                }

                
                if ((Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
                    && Keyboard.IsKeyDown(Key.W))
                {
                    TextBlock hint_text = T("ext_hint_text");
                    MessageBox.Show(hint_text.FontSize.ToString());
                }
            }
            if (IsActive)
            {
                
                if (ext_list[2] && !entered && Keyboard.IsKeyDown(Key.Enter))
                {
                    TextBox box = BOX("ext_hint_input");
                    int n = box.CaretIndex;
                    string s = box.Text;
                    StringBuilder b = new StringBuilder();
                    for (int i = 0; i < s.Length; i++)
                    {
                        if (i == n)
                        {
                            b.Append('\n');
                        }
                        b.Append(s[i]);
                    }
                    if (n == s.Length)
                    {
                        b.Append('\n');
                    }
                    box.Text = b.ToString();
                    box.CaretIndex = n + 1;
                    entered = true;
                }
                if (ext_list[2] && entered && Keyboard.IsKeyUp(Key.Enter))
                {
                    entered = false;
                }
            }
        }

        public void ball_move()
        {
            if (save_shot)
            {
                Random random = new Random();
                if (ball_need_random)
                {
                    ball_vector = new Point(
                        (random.NextDouble() - 0.5) * 1, 
                        (random.NextDouble() - 0.5) * 1);
                    ball_need_random = false;
                }
                else
                {
                    ball_vector.X += (random.NextDouble() - 0.5) + ball_acc.X * 0.1;
                    ball_vector.Y += (random.NextDouble() - 0.5) + ball_acc.Y * 0.1;
                }

                Grid g = G("sh_ball_grid");
                double h_bound = main_grid.Width - 100;
                double v_bound = main_grid.Height - 100;
                Thickness old_t = g.Margin;

                double new_x = old_t.Left + (ball_vector.X / 8);
                double new_y = old_t.Top + (ball_vector.Y / 8);

                if (new_x < 0)
                {
                    new_x = 0;
                    ball_vector.X = -ball_vector.X / 2;
                    ball_acc.X++;
                }
                if (new_y < 0)
                {
                    new_y = 0;
                    ball_vector.Y = -ball_vector.Y / 2;
                    ball_acc.Y++;
                }
                if (new_x > h_bound)
                {
                    new_x = h_bound;
                    ball_vector.X = -ball_vector.X / 2;
                    ball_acc.X--;
                }
                if (new_y > v_bound)
                {
                    new_y = v_bound;
                    ball_vector.Y = -ball_vector.Y / 2;
                    ball_acc.Y--;
                }

                g.Margin = new Thickness(new_x, new_y, 0, 0);
            }
        }

        public class slow_ticker
        {
            public double acc = 0;
            public double last = 0;

            public slow_ticker(double now)
            {
                last = now;
            }
        }
        Dictionary<string, slow_ticker> s_tickers = new Dictionary<string, slow_ticker>();

        public bool s_ticker(string name, double t, double tick_time)
        {
            set_s_ticker(name, t, tick_time);
            slow_ticker s = s_tickers[name];
            s.acc += tick_time;
            if (s.acc >= t)
            {
                s.acc = 0;
                return true;
            }
            return false;
        }
        public void set_s_ticker(string name, double t, double tick_time)
        {
            if (!s_tickers.ContainsKey(name))
            {
                s_tickers.Add(name, new slow_ticker(tick_time));
                slow_ticker s = s_tickers[name];
                s.acc += t;
            }
        }
    }
}
