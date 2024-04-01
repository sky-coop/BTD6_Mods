using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.Drawing;
using System.Windows.Media.Media3D;

namespace BTD6_DIY_Rounds
{
    public static class tool
    {
        public static void fs_write(FileStream x, string str)
        {
            byte[] bs = Encoding.Default.GetBytes(str);
            x.Write(bs, 0, bs.Length);
        }

        public static void scale(FrameworkElement f, double mulx, double muly,
            double c_x = 0, double c_y = 0)
        {
            Transform old_transform = null;
            if (f.RenderTransform == null)
            {
                f.RenderTransform = new TransformGroup();
            }
            if (!(f.RenderTransform is TransformGroup))
            {
                old_transform = f.RenderTransform;
                f.RenderTransform = new TransformGroup();
            }
            TransformGroup t = (TransformGroup)f.RenderTransform;
            if (old_transform != null)
            {
                t.Children.Add(old_transform);
            }
            foreach (Transform a in t.Children)
            {
                if (a is ScaleTransform)
                {
                    ScaleTransform s = (ScaleTransform)a;
                    s.ScaleX = mulx;
                    s.ScaleY = muly;
                    if (f.ActualWidth != 0)
                    {
                        s.CenterX = c_x * f.ActualWidth;
                    }
                    if (f.ActualHeight != 0)
                    {
                        s.CenterY = c_y * f.ActualHeight;
                    }
                    return;
                }
            }
            if (f.Width.Equals(double.NaN) || f.Height.Equals(double.NaN))
            {
                t.Children.Add(new ScaleTransform(mulx, muly));
            }
            else
            {
                t.Children.Add(new ScaleTransform(mulx, muly, c_x * f.Width, c_y * f.Height));
            }
        }
        public static System.Windows.Point getScale(FrameworkElement f)
        {
            TransformGroup t;
            if (f.RenderTransform is TransformGroup)
            {
                t = (TransformGroup)f.RenderTransform;
                foreach (Transform a in t.Children)
                {
                    if (a is ScaleTransform)
                    {
                        ScaleTransform s = (ScaleTransform)a;
                        return new System.Windows.Point(s.ScaleX, s.ScaleY);
                    }
                }
            }
            return new System.Windows.Point(1, 1);
        }
        public static System.Windows.Media.Color HslToRgb(double Hue, double Saturation, double Lightness)
        {
            if (Hue < 0) Hue = 0.0;
            if (Saturation < 0) Saturation = 0.0;
            if (Lightness < 0) Lightness = 0.0;
            if (Hue >= 360) Hue = 359.0;
            if (Saturation > 255) Saturation = 255;
            if (Lightness > 255) Lightness = 255;
            Saturation = Saturation / 255.0;
            Lightness = Lightness / 255.0;

            double C = (1 - Math.Abs(2 * Lightness - 1)) * Saturation;
            double hh = Hue / 60.0;
            double X = C * (1 - Math.Abs(hh % 2 - 1));
            double r = 0, g = 0, b = 0;
            if (hh >= 0 && hh < 1)
            {
                r = C;
                g = X;
            }
            else if (hh >= 1 && hh < 2)
            {
                r = X;
                g = C;
            }
            else if (hh >= 2 && hh < 3)
            {
                g = C;
                b = X;
            }
            else if (hh >= 3 && hh < 4)
            {
                g = X;
                b = C;
            }
            else if (hh >= 4 && hh < 5)
            {
                r = X;
                b = C;
            }
            else
            {
                r = C;
                b = X;
            }
            double m = Lightness - C / 2;
            r += m;
            g += m;
            b += m;
            r = r * 255.0;
            g = g * 255.0;
            b = b * 255.0;
            r = Math.Round(r);
            g = Math.Round(g);
            b = Math.Round(b);
            return System.Windows.Media.Color.FromRgb((byte)r, (byte)g, (byte)b);
        }

        public static void CaptureScreen(string filePath, double x, double y, double width, double height, DpiScale dpi)
        {
            // 获取屏幕宽度和高度
            /*
            int screenWidth = (int)(SystemParameters.WorkArea.Size.Width * dpi.DpiScaleX);
            int screenHeight = (int)(SystemParameters.WorkArea.Size.Height * dpi.DpiScaleY);*/
            
            //x *= dpi.DpiScaleX;
            //y *= dpi.DpiScaleY;
            width *= dpi.DpiScaleX;
            height *= dpi.DpiScaleY;

            int screenWidth = (int)width;
            int screenHeight = (int)height;


            // 创建一个屏幕图像对象
            using (Bitmap bitmap = new Bitmap(screenWidth, screenHeight))
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    // 拷贝屏幕图像到位图中
                    graphics.CopyFromScreen((int)x, (int)y, 0, 0, new System.Drawing.Size(screenWidth, screenHeight));

                    // 保存位图为PNG格式
                    bitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
                }
            }
        }
    }
}
