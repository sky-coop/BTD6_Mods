using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;
using static System.Reflection.Metadata.BlobBuilder;

namespace BTD6_DIY_Rounds
{
    public partial class MainWindow : Window
    {

        private FrameworkElement get_mouse()
        {
            IInputElement m = Mouse.DirectlyOver;
            if (m != null)
            {
                return m as FrameworkElement;
            }
            return null;
        }

        private void button_hint() 
        { 
            FrameworkElement f = get_mouse();
            hint.Visibility = Visibility.Visible;
            if (f != null)
            {
                string code = f.Name;
                string[] parts = code.Split('_');

                Point p = Mouse.GetPosition(main_grid);
                //Point scale = tool.getScale(main_grid);
                double ratioX = p.X / main_grid.Width;
                double ratioY = p.Y / main_grid.Height;
                double left = p.X + 10;
                double right = 0;
                double top = p.Y + 10;
                double bottom = 0;
                if (ratioX > 0.5)
                {
                    hint.HorizontalAlignment = HorizontalAlignment.Right;
                    left = 0;
                    right = p.X * (1 - ratioX) + 20;
                }
                else
                {
                    hint.HorizontalAlignment = HorizontalAlignment.Left;
                }
                if (ratioY > 0.5)
                {
                    hint.VerticalAlignment = VerticalAlignment.Bottom;
                    top = 0;
                    bottom = p.Y * (1 - ratioY) + 20;
                }
                else
                {
                    hint.VerticalAlignment = VerticalAlignment.Top;
                }
                hint.Margin = new Thickness(
                    left, 
                    top, 
                    right, 
                    bottom);
                hint.MaxWidth = 300;

                rainbow_text r = new rainbow_text("hint");
                r.prepare("hint", HorizontalAlignment.Center, VerticalAlignment.Top,
                    new Thickness(0, 0, 0, 0), double.NaN, double.NaN, 
                    12);
                if (code == "wrap_btn")
                {
                    r.add("如果", 0, 255, 255);
                    r.add("编辑器", 127, 255, 0);
                    r.add("和", 0, 255, 255);
                    r.add("游戏", 0, 255, 127);
                    r.add("同时运行中，", 0, 255, 255);
                    r.add("左击", 255, 255, 0);
                    r.add("此按钮可以传送到游戏界面！", 255, 255, 255);
                    r.add("你也可以使用快捷键Alt+Z来传送到游戏，" +
                        "并能在游戏中使用Alt+X传送到编辑器", 0, 255, 0);
                }
                else if (code.Contains("round_select_grid"))
                {
                    r.add("左击", 255, 255, 0);
                    r.add("以浏览此回合的情况", 255, 255, 255);
                    r.add("", 255, 0, 0);
                    r.add("右击", 255, 0, 0);
                    r.add("进入", 255, 255, 255);
                    r.add("提示编辑器", 0, 255, 0);
                    r.add("，以编辑本回合的结束提示", 255, 255, 255);
                    TextBlock t = (TextBlock)FindName(code + "_text");
                    try
                    {
                        int i = Convert.ToInt32(parts[3]);
                        int j = Convert.ToInt32(parts[4]);
                        int index = i * page_j + j;
                        int round = (current_page - 1) * page_i * page_j + index + 1;
                        if (t != null && ((SolidColorBrush)t.Foreground).Color.R > 0)
                        {
                            r.add("", 0, 0, 0);
                            r.add("本回合有", 0, 255, 0);
                            r.add("回合结束提示", 0, 255, 255);
                            r.add("：", 0, 255, 0);
                            var tuple = hints[round];
                            ARGB c = tuple.Item1;
                            string[] lines = tuple.Item2.Split('\n');
                            foreach(string s in lines)
                            {
                                r.add("", 0, 0, 0);
                                r.add(s, c.r, c.g, c.b, c.a);
                            }
                        }
                        int start_round = get_start_round();
                        int end_round = get_end_round();
                        if (round < start_round)
                        {
                            r.add("", 0, 0, 0);
                            r.add("由于此回合早于", 255, 0, 0);
                            r.add("开始回合(start round属性)", 0, 255, 255);
                            r.add("，不可用", 255, 0, 0);
                        }
                        if (round > end_round)
                        {
                            r.add("", 0, 0, 0);
                            r.add("由于此回合晚于", 255, 0, 0);
                            r.add("结束回合(end round属性)", 0, 255, 255);
                            r.add("，不可用", 255, 0, 0);
                            r.add("（但作用于Free play模式）", 0, 255, 0);
                        }
                    }
                    catch
                    {

                    }
                }
                else if (code.Contains("round_grid_change_down"))
                {
                    r.add("左击", 255, 255, 0);
                    r.add("浏览上一页，", 255, 255, 255);
                    r.add("你也可以使用鼠标滚轮进行切换", 0, 255, 255);
                }
                else if (code.Contains("round_grid_change_up"))
                {
                    r.add("左击", 255, 255, 0);
                    r.add("浏览下一页，", 255, 255, 255);
                    r.add("你也可以使用鼠标滚轮进行切换", 0, 255, 255);
                }
                else if (code.Contains("bloons_round"))
                {
                    r.add("左击", 255, 255, 0);
                    r.add("进入", 255, 255, 255);
                    r.add("提示编辑器", 0, 255, 0);
                    r.add("，以编辑本回合的结束提示", 255, 255, 255);
                    if (curr_round > 1)
                    {
                        r.add("", 255, 0, 0);
                        r.add("右击", 255, 0, 0);
                        r.add("会编辑", 255, 255, 255);
                        r.add("上一回合的结束提示", 255, 255, 255);
                        r.add("（即连续游玩时本回合的开始提示）", 127, 255, 127);
                    }
                    r.add("", 255, 0, 0);
                    r.add("当前回合的结束提示：", 0, 255, 255);
                    if (hints.ContainsKey(curr_round))
                    {
                        r.add("", 0, 0, 0);
                        var tuple = hints[curr_round];
                        ARGB c = tuple.Item1;
                        string[] lines = tuple.Item2.Split('\n');
                        foreach (string s in lines)
                        {
                            r.add("", 0, 0, 0);
                            r.add(s, c.r, c.g, c.b, c.a);
                        }
                    }
                    else
                    {
                        r.add("无", 255, 127, 0);
                    }
                    if (curr_round > 1)
                    {
                        r.add("", 255, 0, 0);
                        r.add("上一回合的结束提示：", 0, 255, 255);
                        if (hints.ContainsKey(curr_round - 1))
                        {
                            r.add("", 0, 0, 0);
                            var tuple = hints[curr_round - 1];
                            ARGB c = tuple.Item1;
                            string[] lines = tuple.Item2.Split('\n');
                            foreach (string s in lines)
                            {
                                r.add("", 0, 0, 0);
                                r.add(s, c.r, c.g, c.b, c.a);
                            }
                        }
                        else
                        {
                            r.add("无", 255, 127, 0);
                        }
                    }
                }
                else if (code.Contains("prev_round"))
                {
                    r.add("左击", 255, 255, 0);
                    r.add("浏览上一回合", 255, 255, 255);
                }
                else if (code.Contains("next_round"))
                {
                    r.add("左击", 255, 255, 0);
                    r.add("浏览下一回合", 255, 255, 255);
                }
                else if (code == "show")
                {
                    r.add("左击", 255, 255, 0);
                    r.add("以", 255, 255, 255);
                    r.add("切换", 0, 255, 0);
                    r.add("是否查看气球行的", 255, 255, 255);
                    r.add("统计属性", 0, 255, 255);
                }
                else if (code.Contains("bloons_img_stat"))
                {
                    r.add("左击", 255, 255, 0);
                    r.add("以", 255, 255, 255);
                    r.add("切换", 0, 255, 0);
                    r.add("此气球行的", 255, 255, 255);
                    r.add("特殊属性", 0, 255, 255);
                }
                else if (code.Contains("bloons_copy"))
                {
                    r.add("左击", 255, 255, 0);
                    r.add("将此气球行的属性", 255, 255, 255);
                    r.add("复制", 0, 255, 0);
                    r.add("到下方的控制区", 255, 255, 255);
                    r.add("", 255, 255, 255);
                    r.add("右击", 255, 0, 0);
                    r.add("在", 255, 255, 255);
                    r.add("复制", 0, 255, 0);
                    r.add("的同时", 255, 255, 255);
                    r.add("选中", 0, 255, 0);
                    r.add("它，使接下来的提交将", 255, 255, 255);
                    r.add("修改", 0, 255, 0);
                    r.add("它，", 255, 255, 255);
                    r.add("再次右击", 255, 0, 0);
                    r.add("取消", 255, 255, 255);
                    r.add("选中（不会复制）", 0, 255, 0);
                }
                else if (code.Contains("bloons_delete"))
                {
                    r.add("左击", 255, 255, 0);
                    r.add("删除", 255, 127, 0);
                    r.add("此气球行，", 255, 255, 255);
                    r.add("当本回合仅剩此气球行时无法", 255, 255, 255);
                    r.add("删除", 255, 127, 0);
                }
                else if (code.Contains("bloons_exchange_up"))
                {
                    r.add("左击", 255, 255, 0);
                    r.add("将此气球行与", 255, 255, 255);
                    r.add("上一", 0, 255, 255);
                    r.add("气球行", 255, 255, 255);
                    r.add("交换位置", 255, 255, 0);
                    r.add("，交换位置不影响气球的数量和密度，", 255, 255, 255);
                    r.add("但在多线图中它们的出场路线可能会变化", 255, 127, 0);
                }
                else if (code.Contains("bloons_exchange_down"))
                {
                    r.add("左击", 255, 255, 0);
                    r.add("将此气球行与", 255, 255, 255);
                    r.add("下一", 0, 255, 255);
                    r.add("气球行", 255, 255, 255);
                    r.add("交换位置", 255, 255, 0);
                    r.add("，交换位置不影响气球的数量和密度，", 255, 255, 255);
                    r.add("但在多线图中它们的出场路线可能会变化", 255, 127, 0);
                }
                else if (code.Contains("control_add"))
                {
                    r.add("左击", 255, 255, 0);
                    r.add("尝试提交气球行", 255, 255, 255);
                    r.add("", 255, 255, 255);
                    r.add("但必须在右方输入框为", 255, 255, 0);
                    r.add("绿色", 0, 255, 0);
                    r.add("时才可执行", 255, 255, 0);
                }
                else if (code == "control_up")             
                {
                    r.add("左击", 255, 255, 0);
                    r.add("上移气球区", 255, 255, 255);
                    r.add("", 255, 255, 255);
                    r.add("你可以使用鼠标滚轮达到此效果", 255, 255, 0);
                }
                else if (code == "control_down")
                {
                    r.add("左击", 255, 255, 0);
                    r.add("下移气球区", 255, 255, 255);
                    r.add("", 255, 255, 255);
                    r.add("你可以使用鼠标滚轮达到此效果", 255, 255, 0);
                }
                else if (code == "control_lineadd")
                {
                    r.add("左击", 255, 255, 0);
                    r.add("使气球区多显示一个气球行", 255, 255, 255);
                    r.add("", 255, 255, 255);
                    r.add("最多7行", 255, 255, 0);
                }
                else if (code == "control_linesub")
                {
                    r.add("左击", 255, 255, 0);
                    r.add("使气球区少显示一个气球行", 255, 255, 255);
                    r.add("", 255, 255, 255);
                    r.add("最少3行", 255, 255, 0);
                }
                else if (code == "control_pageiadd")
                {
                    r.add("左击", 255, 255, 0);
                    r.add("使回合区多显示一行按钮", 255, 255, 255);
                    r.add("", 255, 255, 255);
                    r.add("最多20行", 255, 255, 0);
                }
                else if (code == "control_pageisub")
                {
                    r.add("左击", 255, 255, 0);
                    r.add("使回合区少显示一行按钮", 255, 255, 255);
                    r.add("", 255, 255, 255);
                    r.add("最少5行", 255, 255, 0);
                }
                else if (code == "control_pagejadd")
                {
                    r.add("左击", 255, 255, 0);
                    r.add("使回合区多显示一列按钮", 255, 255, 255);
                    r.add("", 255, 255, 255);
                    r.add("最多7列", 255, 255, 0);
                }
                else if (code == "control_pagejsub")
                {
                    r.add("左击", 255, 255, 0);
                    r.add("使回合区少显示一列按钮", 255, 255, 255);
                    r.add("", 255, 255, 255);
                    r.add("最少2列", 255, 255, 0);
                }
                else if (code == "control_regrow")
                {
                    r.add("左击", 255, 255, 0);
                    r.add("以", 255, 255, 255);
                    r.add("切换", 0, 255, 0);
                    r.add("将要被提交的气球行的", 255, 255, 255);
                    r.add("重生", 0, 255, 0);
                    r.add("属性", 255, 255, 255);
                }
                else if (code == "control_fort")
                {
                    r.add("左击", 255, 255, 0);
                    r.add("以", 255, 255, 255);
                    r.add("切换", 0, 255, 0);
                    r.add("将要被提交的气球行的", 255, 255, 255);
                    r.add("加固", 0, 255, 0);
                    r.add("属性", 255, 255, 255);
                    r.add("", 255, 255, 255);
                    r.add("若气球是金气球，此属性依赖于“铅”属性", 255, 255, 0);
                }
                else if (code == "control_camo")
                {
                    r.add("左击", 255, 255, 0);
                    r.add("以", 255, 255, 255);
                    r.add("切换", 0, 255, 0);
                    r.add("将要被提交的气球行的", 255, 255, 255);
                    r.add("迷彩", 0, 255, 0);
                    r.add("属性", 255, 255, 255);
                }
                else if (code == "control_lead")
                {
                    r.add("左击", 255, 255, 0);
                    r.add("以", 255, 255, 255);
                    r.add("切换", 0, 255, 0);
                    r.add("将要被提交的气球行的", 255, 255, 255);
                    r.add("铅", 0, 255, 0);
                    r.add("属性，目前仅", 255, 255, 255);
                    r.add("金气球", 255, 255, 0);
                    r.add("有此属性", 255, 255, 255);
                }
                else if (code == "control_purple")
                {
                    r.add("左击", 255, 255, 0);
                    r.add("以", 255, 255, 255);
                    r.add("切换", 0, 255, 0);
                    r.add("将要被提交的气球行的", 255, 255, 255);
                    r.add("紫气球", 0, 255, 0);
                    r.add("属性，目前仅", 255, 255, 255);
                    r.add("金气球", 255, 255, 0);
                    r.add("有此属性，此属性依赖于“铅”属性", 255, 255, 255);
                }
                else if (code == "control_zebra")
                {
                    r.add("左击", 255, 255, 0);
                    r.add("以", 255, 255, 255);
                    r.add("切换", 0, 255, 0);
                    r.add("将要被提交的气球行的", 255, 255, 255);
                    r.add("斑马纹气球", 0, 255, 0);
                    r.add("属性，目前仅", 255, 255, 255);
                    r.add("金气球", 255, 255, 0);
                    r.add("有此属性，此属性依赖于“紫气球”属性", 255, 255, 255);
                }
                else if (code == "control_elite")
                {
                    r.add("左击", 255, 255, 0);
                    r.add("以", 255, 255, 255);
                    r.add("切换", 0, 255, 0);
                    r.add("将要被提交的气球行的", 255, 255, 255);
                    r.add("精英", 0, 255, 0);
                    r.add("属性，目前仅", 255, 255, 255);
                    r.add("BOSS级气球", 255, 0, 0);
                    r.add("有此属性", 255, 255, 255);
                }
                else if (code == "control_load_template")
                {
                    r.add("左击", 255, 255, 0);
                    r.add("以", 255, 255, 255);
                    r.add("读取", 0, 255, 0);
                    r.add("模板回合文件，", 255, 255, 255);
                    r.add("请在读取前注意目前正在编辑的数据是否需要保存", 255, 127, 0);
                }
                else if (code == "control_load_workspace")
                {
                    r.add("左击", 255, 255, 0);
                    r.add("以", 255, 255, 255);
                    r.add("读取", 0, 255, 0);
                    r.add("工作区回合文件，", 255, 255, 255);
                    r.add("请在读取前注意目前正在编辑的数据是否需要保存", 255, 127, 0);
                }
                else if (code == "control_save_workspace")
                {
                    r.add("左击", 255, 255, 0);
                    r.add("以", 255, 255, 255);
                    r.add("保存", 0, 255, 0);
                    r.add("回合文件到工作区，", 255, 255, 255);
                    r.add("请检查输入的名称，避免覆盖到不想覆盖的回合文件", 255, 127, 0);
                    if (save_shot)
                    {
                        r.add("", 255, 127, 0);
                        r.add("你设置了", 255, 0, 0);
                        r.add("“保存时截图”", 255, 255, 0);
                        r.add("，点击此按钮后需要一些时间来截图，请耐心等待", 255, 0, 0);
                    }
                }
                else if (code == "control_explore")
                {
                    r.add("左击", 255, 255, 0);
                    r.add("来打开", 255, 255, 255);
                    r.add("文件管理器", 0, 255, 255);
                    r.add("并", 255, 255, 255);
                    r.add("导航", 0, 255, 0);
                    r.add("到你的", 255, 255, 255);
                    r.add("工作区", 0, 255, 255);
                }
                else if (code == "control_search")
                {
                    r.add("左击", 255, 255, 0);
                    r.add("以", 255, 255, 255);
                    r.add("浏览文件", 0, 255, 0);
                    r.add("（你也可以在左方输入框中手动输入路径），", 255, 255, 255);
                    r.add("请找到你的BTD6游戏文件，格式为.exe", 0, 255, 255);
                }
                else if (code == "control_save_game")
                {
                    r.add("左击", 255, 255, 0);
                    r.add("将回合文件", 255, 255, 255);
                    r.add("直接保存到游戏的文件夹", 255, 127, 0);
                    r.add("中，使得游戏可立刻读入回合文件。", 255, 255, 255);
                    r.add("请在保存前注意是否会覆盖到重要数据", 255, 127, 0);
                }
                else if (code == "control_load_game")
                {
                    r.add("左击", 255, 255, 0);
                    r.add("以", 255, 255, 255);
                    r.add("读取", 0, 255, 0);
                    r.add("游戏正在使用的回合文件", 0, 255, 255);
                    r.add("，", 255, 255, 255);
                    r.add("请在读取前注意目前正在编辑的数据是否需要保存", 255, 127, 0);
                }

                #region batch
                else if(code == "control_redbloon")
                {
                    r.add("左击", 255, 255, 0);
                    r.add("将", 255, 255, 255);
                    r.add("本回合", 0, 255, 255);
                    r.add("的气球变为", 255, 255, 255);
                    r.add("1个红气球", 0, 255, 255);
                    r.add("", 255, 255, 255);
                    r.add("右击", 255, 0, 0);
                    r.add("将", 255, 255, 255);
                    r.add("所有回合", 255, 0, 0);
                    r.add("的气球变为", 255, 255, 255);
                    r.add("1个红气球", 0, 255, 255);
                }
                else if (code == "control_only")
                {
                    if (!control_create_valid)
                    {
                        r.add("你的输入不正确！请检查上方的输入框", 255, 127, 0);
                        r.add("", 255, 127, 0);
                    }
                    r.add("左击", 255, 255, 0);
                    r.add("将", 255, 255, 255);
                    r.add("本回合", 0, 255, 255);
                    r.add("的气球变为", 255, 255, 255);
                    r.add("当前正在编辑的气球", 0, 255, 255);
                    r.add("", 255, 255, 255);
                    r.add("右击", 255, 0, 0);
                    r.add("将", 255, 255, 255);
                    r.add("所有回合", 255, 0, 0);
                    r.add("的气球变为", 255, 255, 255);
                    r.add("当前正在编辑的气球", 0, 255, 255);
                }
                else if (code == "control_copyround")
                {
                    r.add("左击", 255, 255, 0);
                    r.add("将", 255, 255, 255);
                    r.add("本回合的气球", 0, 255, 255);
                    r.add("保存到", 255, 255, 255);
                    r.add("剪贴板", 0, 255, 0);
                    r.add("中", 255, 255, 255);
                    r.add("", 255, 255, 255);
                    r.add("右击", 255, 0, 0);
                    r.add("还会额外保存", 255, 255, 255);
                    r.add("提示信息", 255, 0, 0);
                    r.add("", 255, 255, 255);
                    r.add("你可以通过这种方式来在两个编辑器之间交叉编辑", 0, 255, 127);
                }
                else if (code == "control_pasteround")
                {
                    r.add("点击", 127, 255, 0);
                    r.add("以读取", 255, 255, 255);
                    r.add("剪贴板", 0, 255, 0);
                    r.add("中的内容，并提供", 255, 255, 255);
                    r.add("预览窗口", 0, 255, 255);
                    r.add("，它提供本回合被粘贴后的情况，", 255, 255, 255);
                    r.add("你可以在实际粘贴前进行二次确认", 0, 255, 127);
                    r.add("", 0, 255, 127);
                    r.add("左击", 255, 255, 0);
                    r.add("会", 255, 255, 255);
                    r.add("覆盖", 255, 127, 0);
                    r.add("整个回合", 255, 255, 255);
                    r.add("", 0, 255, 127);
                    r.add("右击", 255, 0, 0);
                    r.add("则会把气球添加到", 255, 255, 255);
                    r.add("回合末尾", 255, 255, 255);
                    r.add("", 0, 255, 127);
                    r.add("当", 255, 255, 255);
                    r.add("剪贴板", 0, 255, 0);
                    r.add("中有提示信息时，会进行覆盖", 255, 255, 255);
                }
                else if (code == "loading_cancel")
                {
                    r.add("除了鼠标滚轮外你不能进行任何操作！", 0, 255, 255);
                    r.add("因此，如果对粘贴结果不满意，", 255, 255, 255);
                    r.add("左击", 255, 255, 0);
                    r.add("此按钮以", 255, 255, 255);
                    r.add("取消粘贴操作", 255, 127, 0);
                }
                else if (code == "loading_confirm")
                {
                    r.add("左击", 255, 255, 0);
                    r.add("此按钮以", 255, 255, 255);
                    r.add("应用粘贴结果", 127, 255, 0);
                    r.add("，", 255, 255, 255);
                    r.add("这会修改编辑器中的内容", 0, 255, 0);
                }
                else if (code == "control_batch_switch")
                {
                    r.add("左击", 255, 255, 0);
                    r.add("将", 255, 255, 255);
                    r.add("本回合", 0, 255, 255);
                    r.add("与", 255, 255, 255);
                    r.add("目标回合", 0, 255, 255);
                    r.add("交换", 255, 255, 255);
                    r.add("", 255, 255, 255);
                    r.add("右击", 255, 0, 0);
                    r.add("不仅", 255, 255, 255);
                    r.add("交换气球", 255, 255, 0);
                    r.add("还", 255, 255, 255);
                    r.add("交换回合的提示信息", 255, 255, 0);
                }
                else if (code == "control_batch_count_span")
                {
                    r.add("左击", 255, 255, 0);
                    r.add("将", 255, 255, 255);
                    r.add("本回合的", 0, 255, 255);
                    r.add("每行气球", 0, 255, 127);
                    r.add("的", 255, 255, 255);
                    r.add("数量", 255, 255, 0);
                    r.add("均变为", 255, 255, 255);
                    r.add("[气球倍率]", 0, 255, 0);
                    r.add("倍", 255, 255, 255);
                    r.add("（向上取整）", 0, 255, 255);
                    r.add("，并改变气球之间的", 255, 255, 255);
                    r.add("间隔", 255, 255, 0);
                    r.add("，使", 255, 255, 255);
                    r.add("总时间不变", 255, 255, 0);
                    r.add("", 255, 255, 255);
                    r.add("右击", 255, 0, 0);
                    r.add("则对", 255, 255, 255);
                    r.add("所有回合", 255, 0, 0);
                    r.add("进行此操作", 255, 255, 255);
                }
                else if (code == "control_batch_count_time")
                {
                    r.add("左击", 255, 255, 0);
                    r.add("将", 255, 255, 255);
                    r.add("本回合的", 0, 255, 255);
                    r.add("每行气球", 0, 255, 127);
                    r.add("的", 255, 255, 255);
                    r.add("数量", 255, 255, 0);
                    r.add("均变为", 255, 255, 255);
                    r.add("[气球倍率]", 0, 255, 0);
                    r.add("倍", 255, 255, 255);
                    r.add("（向上取整）", 0, 255, 255);
                    r.add("，并改变气球的", 255, 255, 255);
                    r.add("开始时间", 255, 255, 0);
                    r.add("和", 255, 255, 255);
                    r.add("结束时间", 255, 255, 0);
                    r.add("，使", 255, 255, 255);
                    r.add("气球间距不变", 255, 255, 0);
                    r.add("", 255, 255, 255);
                    r.add("右击", 255, 0, 0);
                    r.add("则对", 255, 255, 255);
                    r.add("所有回合", 255, 0, 0);
                    r.add("进行此操作", 255, 255, 255);
                }
                else if (code == "control_batch_span_count")
                {
                    r.add("左击", 255, 255, 0);
                    r.add("将", 255, 255, 255);
                    r.add("本回合的", 0, 255, 255);
                    r.add("每行气球", 0, 255, 127);
                    r.add("的", 255, 255, 255);
                    r.add("间隔", 255, 255, 0);
                    r.add("均变为", 255, 255, 255);
                    r.add("[气球倍率]", 0, 255, 0);
                    r.add("倍", 255, 255, 255);
                    r.add("，并改变气球的", 255, 255, 255);
                    r.add("数量", 255, 255, 0);
                    r.add("（向上取整）", 127, 255, 0);
                    r.add("，保证", 255, 255, 255);
                    r.add("总时间不变", 255, 255, 0);
                    r.add("", 255, 255, 255);
                    r.add("右击", 255, 0, 0);
                    r.add("则对", 255, 255, 255);
                    r.add("所有回合", 255, 0, 0);
                    r.add("进行此操作", 255, 255, 255);
                }
                else if (code == "control_batch_span_time")
                {
                    r.add("左击", 255, 255, 0);
                    r.add("将", 255, 255, 255);
                    r.add("本回合的", 0, 255, 255);
                    r.add("每行气球", 0, 255, 127);
                    r.add("的", 255, 255, 255);
                    r.add("间隔", 255, 255, 0);
                    r.add("均变为", 255, 255, 255);
                    r.add("[气球倍率]", 0, 255, 0);
                    r.add("倍", 255, 255, 255);
                    r.add("，并改变气球的", 255, 255, 255);
                    r.add("开始时间", 255, 255, 0);
                    r.add("和", 255, 255, 255);
                    r.add("结束时间", 255, 255, 0);
                    r.add("，使", 255, 255, 255);
                    r.add("气球数量不变", 255, 255, 0);
                    r.add("", 255, 255, 255);
                    r.add("右击", 255, 0, 0);
                    r.add("则对", 255, 255, 255);
                    r.add("所有回合", 255, 0, 0);
                    r.add("进行此操作", 255, 255, 255);
                }
                else if (code == "control_batch_expand")
                {
                    r.add("左击", 255, 255, 0);
                    r.add("将", 255, 255, 255);
                    r.add("本回合的", 0, 255, 255);
                    r.add("气球数量", 0, 255, 127);
                    r.add("和", 0, 255, 127);
                    r.add("气球行数", 0, 255, 127);
                    r.add("变为", 255, 255, 255);
                    r.add("[扩展路线数]", 0, 255, 0);
                    r.add("倍", 255, 255, 255);
                    r.add("，以此让这些路线的气球", 255, 255, 255);
                    r.add("同时出现", 255, 255, 0);
                    r.add("（若气球数量与路线数不互质，会填充红气球）", 255, 255, 255);
                    r.add("", 255, 255, 255);
                    r.add("右击", 255, 0, 0);
                    r.add("则对", 255, 255, 255);
                    r.add("所有回合", 255, 0, 0);
                    r.add("进行此操作，并将", 255, 255, 255);
                    r.add("初始金（如果不是默认状态）", 255, 255, 0);
                    r.add("乘以", 0, 255, 255);
                    r.add("（路线数 ^ 0.7）", 255, 255, 0);
                    r.add("，", 255, 255, 255);
                    r.add("击破气球金", 255, 255, 0);
                    r.add("除以", 255, 0, 0);
                    r.add("（路线数 ^ 0.3）", 255, 255, 0);
                }
                #endregion batch
                #region attr
                else if (code == "control_attr_confirm")
                {
                    r.add("（这些属性能很大地改变游戏平衡性）", 0, 255, 255);
                    r.add("", 0, 255, 255);
                    r.add("左击", 255, 255, 0);
                    r.add("以", 255, 255, 255);
                    r.add("修改", 0, 255, 0);
                    r.add("选中的属性为新值", 255, 255, 255);
                    r.add("，", 255, 255, 255);
                    r.add("某些属性可以设置为“默认”，默认值不会修改游戏数据", 0, 255, 127);
                    r.add("", 0, 255, 127);
                    r.add("右击", 255, 0, 0);
                    r.add("可以将当前属性还原为", 255, 255, 255);
                    r.add("正常值", 0, 255, 255);

                    string select = get_sel_str("control_attr_select_select");
                    if (select == "tower attack speed multiplier" ||
                        select == "tower damage multiplier")
                    {
                        r.add("", 0, 0, 0);
                        r.add("（警告：若你在游戏时修改此数据，" +
                            "已出场的特定等级的塔将可能无法修改，" +
                            "若要在此局游戏中更新修改效果，" +
                            "请退出到主页然后重进，或是将你的塔" +
                            "升级到一个从未达到过的升级路径）", 255, 50, 0);
                    }
                }
                #endregion attr
                #region random
                else if (code == "control_random_roll")
                {
                    r.add("左击", 255, 255, 0);
                    r.add("以", 255, 255, 255);
                    r.add("根据你的种子和设置生成回合", 0, 255, 0);
                    r.add("，", 255, 255, 255);
                    r.add("请注意这会覆盖编辑器里的数据，若不想覆盖，请保存它", 255, 255, 255);
                    r.add("", 255, 255, 255);
                    r.add("注：只有种子和所有设置都相同，生成的回合才相同", 0, 255, 255);
                }
                #endregion random
                #region config
                else if (code == "control_auto_save")
                {
                    r.add("点击", 255, 255, 0);
                    r.add("以", 255, 255, 255);
                    r.add("切换", 0, 255, 0);
                    r.add("是否要将", 255, 255, 255);
                    r.add("所有于编辑器中的改动", 0, 255, 0);
                    r.add("立刻保存到游戏的回合文件中，这会实时覆盖它，", 255, 0, 0);
                    r.add("这能使游戏中的气球与编辑器中的内容同步。", 0, 255, 0);
                    r.add("", 0, 255, 0);
                    r.add("左击：立即将编辑器中的内容导入到游戏中，", 255, 127, 0);
                    r.add("覆盖游戏的回合文件", 255, 0, 0);
                    r.add("", 0, 255, 0);
                    r.add("右击：立即将游戏中的内容导入到编辑器中，", 255, 127, 0);
                    r.add("覆盖编辑器的回合文件", 255, 0, 0);
                    r.add("", 0, 255, 0);
                    r.add("打开此功能后，背景将变色以提醒您此功能正在工作中，" +
                        "且每次自动保存时都会有提示", 0, 255, 255);
                }
                else if (code == "control_screenshot")
                {
                    r.add("左击", 255, 255, 0);
                    r.add("以", 255, 255, 255);
                    r.add("切换", 0, 255, 0);
                    r.add("是否要在", 255, 255, 255);
                    r.add("手动保存", 0, 255, 0);
                    r.add("时花一些时间来将所有回合的状况截图，", 255, 255, 255);
                    r.add("截图文件会保存在工作区中", 0, 255, 255);
                    r.add("", 0, 255, 0);
                    r.add("打开此功能后，背景将有一个截图球来提醒你此功能已开启，", 0, 255, 255);
                    r.add("请放心，截图球之类的内容不会影响截图效果", 0, 255, 0);
                }
                else if (code == "control_wrapswitch")
                {
                    r.add("左击", 255, 255, 0);
                    r.add("以", 255, 255, 255);
                    r.add("切换", 0, 255, 0);
                    r.add("是否（通过改变窗口名称）禁止你在游戏中按", 255, 255, 255);
                    r.add("Alt+X", 255, 255, 0);
                    r.add("传送到此编辑器", 0, 255, 0);
                    r.add("，此设置主要用于", 255, 255, 255);
                    r.add("与其他mod按键冲突", 0, 255, 255);
                    r.add("或", 255, 255, 255);
                    r.add("打开了多个编辑器，并需要指定一个主编辑器进行传送", 0, 255, 255);
                    r.add("的情况", 255, 255, 255);
                }
                #endregion config
                #region ext
                #region debut
                else if (code == "control_debut")
                {
                    r.add("左击", 255, 255, 0);
                    r.add("以", 255, 255, 255);
                    r.add("切换", 0, 255, 0);
                    r.add("是否显示", 255, 255, 255);
                    r.add("气球首次出场", 0, 255, 0);
                    r.add("的浏览器，你可以在其中查看每种气球的出场情况，", 255, 255, 255);
                    r.add("使用鼠标滚轮来上下移动", 0, 255, 255);
                }
                else if(code == "control_attr_advanced")
                {
                    r.add("左击", 255, 255, 0);
                    r.add("以", 255, 255, 255);
                    r.add("切换", 0, 255, 0);
                    r.add("是否显示", 255, 255, 255);
                    r.add("高级属性", 0, 255, 0);
                    r.add("的浏览器，你可以在其中查看所有属性的值，", 255, 255, 255);
                    r.add("还能对回合金钱倍率进行修改！", 127, 255, 0);
                }

                else if (code == "ext_debut_exit")
                {
                    r.add("左击", 255, 255, 0);
                    r.add("以", 255, 255, 255);
                    r.add("关闭", 0, 255, 0);
                    r.add("此浏览器", 255, 255, 255);
                }
                else if (code == "ext_debut_switch")
                {
                    r.add("左击", 255, 255, 0);
                    r.add("以", 255, 255, 255);
                    r.add("切换", 0, 255, 0);
                    r.add("是否根据气球属性对气球进行分类统计", 255, 255, 255);
                }
                else if (code == "ext_debut_camo")
                {
                    r.add("左击", 255, 255, 0);
                    r.add("以", 255, 255, 255);
                    r.add("切换", 0, 255, 0);
                    r.add("是否只展示迷彩气球", 255, 255, 255);
                }
                else if (code == "ext_debut_sort")
                {
                    r.add("左击", 255, 255, 0);
                    r.add("以在", 255, 255, 255);
                    r.add("根据气球排序", 255, 255, 0);
                    r.add("和", 255, 255, 255);
                    r.add("根据回合排序", 255, 255, 0);
                    r.add("间", 255, 255, 255);
                    r.add("切换", 0, 255, 0);
                }
                #endregion debut
                #region cash
                else if (code == "ext_cash_exit")
                {
                    r.add("左击", 255, 255, 0);
                    r.add("以", 255, 255, 255);
                    r.add("关闭", 0, 255, 0);
                    r.add("此浏览器", 255, 255, 255);
                }
                else if (code == "ext_cash_switch")
                {
                    r.add("左击", 255, 255, 0);
                    r.add("以", 255, 255, 255);
                    r.add("切换", 0, 255, 0);
                    r.add("普通属性浏览/回合金钱编辑", 255, 255, 255);
                }
                else if (code == "ext_cash_page_up")
                {
                    r.add("左击", 255, 255, 0);
                    r.add("浏览下一页", 255, 255, 255);
                }
                else if (code == "ext_cash_page_down")
                {
                    r.add("左击", 255, 255, 0);
                    r.add("浏览上一页", 255, 255, 255);
                }
                else if (code.Contains("ext_cash_add"))
                {
                    r.add("左击", 255, 255, 0);
                    r.add("以", 255, 255, 255);
                    r.add("插入", 0, 255, 0);
                    r.add("一条规则，使得从此回合开始的所有回合", 255, 255, 255);
                    r.add("击破气球产生的收益", 0, 255, 255);
                    r.add("根据倍率倍增，", 255, 255, 255);
                    r.add("效果直到下一规则的开始", 255, 127, 0);
                }
                else if (code.Contains("ext_cash_del"))
                {
                    r.add("左击", 255, 255, 0);
                    r.add("以", 255, 255, 255);
                    r.add("删除", 255, 127, 0);
                    r.add("此规则", 255, 255, 255);
                }
                else if (code.Contains("ext_cash_default"))
                {
                    r.add("左击", 255, 255, 0);
                    r.add("将", 255, 255, 255);
                    r.add("所有规则重置", 255, 127, 0);
                    r.add("到原始情况", 255, 255, 255);
                }
                #endregion cash
                #region hint
                else if(code == "ext_hint_exit")
                {
                    r.add("左击", 255, 255, 0);
                    r.add("以", 255, 255, 255);
                    r.add("关闭", 0, 255, 0);
                    r.add("此编辑器", 255, 255, 255);
                }
                else if(code == "ext_hint_prev")
                {
                    r.add("左击", 255, 255, 0);
                    r.add("浏览上一回合", 255, 255, 255);
                }
                else if(code == "ext_hint_next")
                {
                    r.add("左击", 255, 255, 0);
                    r.add("浏览下一回合", 255, 255, 255);
                }
                else if (code == "ext_hint_default")
                {
                    r.add("左击", 255, 255, 0);
                    r.add("将所有", 255, 255, 255);
                    r.add("颜", 255, 0, 0);
                    r.add("色", 0, 255, 0);
                    r.add("值", 0, 127, 255);
                    r.add("重置为", 255, 255, 255);
                    r.add("255(100%)", 255, 255, 255, 160);
                    r.add("，即完全不透明的白色", 255, 255, 255);
                }
                else if (code == "ext_hint_commit")
                {
                    r.add("左击", 255, 255, 0);
                    r.add("以", 255, 255, 255);
                    r.add("提交", 0, 255, 0);
                    r.add("当前的", 255, 255, 255);
                    r.add("提示信息", 0, 255, 255);
                    r.add("和", 255, 255, 255);
                    r.add("颜色", 0, 255, 255);
                    r.add("，这会保存到编辑器，", 255, 255, 255);
                    r.add("并在下方提供不精确的预览以供参考", 0, 255, 0);
                    r.add("", 255, 0, 0);
                    r.add("右击", 255, 0, 0);
                    r.add("可", 255, 255, 255);
                    r.add("导入", 0, 255, 0);
                    r.add("本回合的", 255, 255, 255);
                    r.add("提示信息", 0, 255, 255);
                    r.add("和", 255, 255, 255);
                    r.add("颜色", 0, 255, 255);
                    r.add("(若存在)到", 255, 255, 255);
                    r.add("左边的输入框", 0, 255, 255);
                }
                else if (code == "ext_hint_del")
                {
                    r.add("左击", 255, 255, 0);
                    r.add("以", 255, 255, 255);
                    r.add("删除", 255, 127, 0);
                    r.add("本回合的", 255, 255, 255);
                    r.add("提示信息", 0, 255, 255);
                }
                #endregion hint
                #endregion ext
                else
                {
                    bool find = false;
                    f = get_mouse();
                    if (f != null)
                    {
                        if (f.Name == "templateRoot")
                        {
                            while (VisualTreeHelper.GetParent(f) != null)
                            {
                                f = (FrameworkElement)VisualTreeHelper.GetParent(f);
                                if (f.Name.Contains("_select") && f is ComboBox)
                                {
                                    find = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (find)
                    {
                        code = f.Name;
                        parts = code.Split('_');
                        if (code == "control_group_cb_select")
                        {
                            r.add("在这里选择气球所属的模组", 0, 255, 255);
                            r.add("", 255, 255, 255);
                            r.add("目前只包含原版气球，未来可能会添加更多模组", 255, 255, 255);
                        }
                        if (code == "control_bloon_cb_select")
                        {
                            r.add("在这里选择模组中的气球", 0, 255, 255);
                            r.add("，选择完毕后，", 255, 255, 255);
                            r.add("下方会出现它所能附加的属性，", 150, 255, 0);
                            r.add("在右边的输入框中输入数据，", 0, 255, 0);
                            r.add("最后点击左边的按钮即可提交", 255, 255, 0);
                        }
                        if (code == "control_locker_select")
                        {
                            r.add("为了让左方的四个数据符合一定的关系，当你修改一个值时，" +
                                "其余的某个值会随之更改", 255, 127, 0);
                            r.add("", 255, 255, 255);
                            r.add("在这里设置更改的优先级，", 0, 255, 255);
                            r.add("默认优先更改", 255, 255, 255);
                            r.add("第一个数据", 255, 255, 0);
                            r.add("，若你手动修改的就是它，则更改", 255, 255, 255);
                            r.add("第二个数据", 255, 255, 0);
                        }
                        if (code == "control_panel_select")
                        {
                            r.add("非常重要！", 255, 255, 0);
                            r.add("其中的每个面板都提供丰富的功能，", 255, 255, 255);
                            r.add("你可以使用鼠标滚轮进行切换", 0, 255, 0);
                        }

                        if (code.Contains("control_random_preset"))
                        {
                            r.add("在此设置", 255, 255, 0);
                            r.add("战况的激烈程度", 0, 255, 0);
                            r.add("，越是激烈，生成的气球越多，你能使用的钱也随之变多", 255, 255, 255);
                        }
                        if (code.Contains("control_random_length"))
                        {
                            r.add("在此设置", 255, 255, 0);
                            r.add("战斗的长度", 0, 255, 0);
                            r.add("，这会影响回合数的上限", 255, 255, 255);
                        }
                        if (code.Contains("control_random_regrow"))
                        {
                            r.add("在此设置", 255, 255, 0);
                            r.add("重生气球", 0, 255, 0);
                            r.add("的出现概率", 255, 255, 0);
                        }
                        if (code.Contains("control_random_camo"))
                        {
                            r.add("在此设置", 255, 255, 0);
                            r.add("迷彩气球", 0, 255, 0);
                            r.add("的出现概率", 255, 255, 0);
                        }
                        if (code.Contains("control_random_fort"))
                        {
                            r.add("在此设置", 255, 255, 0);
                            r.add("强化气球", 0, 255, 0);
                            r.add("的出现概率", 255, 255, 0);
                        }
                        if (code.Contains("control_random_earliers"))
                        {
                            r.add("在此设置新气球的出现概率", 255, 255, 0);
                            r.add("，生成器会对", 255, 255, 255);
                            r.add("尚未出现过的气球", 0, 255, 255);
                            r.add("进行概率判定，", 255, 255, 255);
                            r.add("未通过判定的气球将不会生成", 0, 255, 0);
                            r.add("，将此概率调高将更容易在早期生成多种气球", 255, 255, 255);
                        }
                        if (code.Contains("control_random_bosses"))
                        {
                            r.add("在此设置", 255, 255, 0);
                            r.add("BOSS", 0, 255, 0);
                            r.add("级气球的出场上限", 255, 255, 0);
                        }
                        if (code.Contains("control_random_attrs"))
                        {
                            r.add("在此设置", 255, 255, 0);
                            r.add("游戏属性", 0, 255, 0);
                            r.add("的复杂程度", 255, 255, 0);
                            r.add("，更复杂的属性会带来更丰富的效果，例如容易同时出现", 255, 255, 255);
                            r.add("防御塔2倍价格", 0, 255, 255);
                            r.add("和", 255, 255, 255);
                            r.add("每回合自动获得10%利息", 0, 255, 255);
                            r.add("之类的情况", 255, 255, 255);
                        }
                        if (code.Contains("control_random_diffs"))
                        {
                            r.add("在此设置", 255, 255, 0);
                            r.add("游戏难度", 0, 255, 0);
                            r.add("，除此之外其余的设置都尽量不会去影响难度，", 255, 255, 255);
                            r.add("因此可以轻松地生成具有目标难度的气球回合", 0, 255, 127);
                            r.add("，游戏原版回合的难度被视为5", 255, 255, 255);
                        }
                    }
                    else
                    {
                        hint.Visibility = Visibility.Hidden;
                    }
                }
                draw_r_text(r);
            }
            else
            {
                hint.Visibility = Visibility.Hidden;
            }
            if (screenshooting)
            {
                hint.Visibility = Visibility.Hidden;
            }
        }
    }
}
