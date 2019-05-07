using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing;

namespace NM_Tool.GDIHelper
{
    /// <summary>
    /// 绘制帮助类
    /// </summary>
    class DrawHelper
    {
        public static void DrawRoundRectangle(Graphics g, Pen pen, Rectangle rect, int cornerRadius)
        {
            using (GraphicsPath path = CreateRoundedRectanglePath(rect, cornerRadius))
            {
                g.DrawPath(pen, path);
            }
        }

        public static void FillRoundRectangle(Graphics g, Brush brush, Rectangle rect, int cornerRadius)
        {
            using (GraphicsPath path = CreateRoundedRectanglePath(rect, cornerRadius))
            {
                g.FillPath(brush, path);
            }
        }

        private static GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int cornerRadius)
        {
            GraphicsPath roundedRect = new GraphicsPath();
            roundedRect.AddArc(rect.X, rect.Y, cornerRadius * 2, cornerRadius * 2, 180, 90);
            roundedRect.AddLine(rect.X + cornerRadius, rect.Y, rect.Right - cornerRadius * 2, rect.Y);
            roundedRect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y, cornerRadius * 2, cornerRadius * 2, 270, 90);
            roundedRect.AddLine(rect.Right, rect.Y + cornerRadius * 2, rect.Right, rect.Y + rect.Height - cornerRadius * 2);
            roundedRect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y + rect.Height - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 0, 90);
            roundedRect.AddLine(rect.Right - cornerRadius * 2, rect.Bottom, rect.X + cornerRadius * 2, rect.Bottom);
            roundedRect.AddArc(rect.X, rect.Bottom - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 90, 90);
            roundedRect.AddLine(rect.X, rect.Bottom - cornerRadius * 2, rect.X, rect.Y + cornerRadius * 2);
            roundedRect.CloseFigure();
            return roundedRect;
        }

        public static Bitmap WayOne(Image image)
        {
            using (Image i =  image)
            {
                Bitmap b = new Bitmap(i.Width, i.Height);
                using (Graphics g = Graphics.FromImage(b))
                {
                    g.FillEllipse(new TextureBrush(i), 0, 0, i.Width, i.Height);
                }
                return b;
            }
        }
        public static Bitmap WayTwo(Image image)
        {
            using (Image i =  image)
            {
                Bitmap b = new Bitmap(i.Width, i.Height);
                using (Graphics g = Graphics.FromImage(b))
                {
                    g.DrawImage(i, 0, 0, b.Width, b.Height);
                    int r = Math.Min(b.Width, b.Height) / 2;
                    PointF c = new PointF(b.Width / 2.0F, b.Height / 2.0F);
                    for (int h = 0; h < b.Height; h++)
                        for (int w = 0; w < b.Width; w++)
                            if ((int)Math.Pow(r, 2) < ((int)Math.Pow(w * 1.0 - c.X, 2) + (int)Math.Pow(h * 1.0 - c.Y, 2)))
                            {
                                b.SetPixel(w, h, Color.Transparent);
                            }
                }
                return b;
            }
        }

        public static Bitmap WaySOne(Bitmap image)
        {
            using (Image i = image)
            {
                Bitmap b = new Bitmap(i.Width, i.Height);
                using (Graphics g = Graphics.FromImage(b))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    using (System.Drawing.Drawing2D.GraphicsPath p = new System.Drawing.Drawing2D.GraphicsPath(System.Drawing.Drawing2D.FillMode.Alternate))
                    {
                        p.AddEllipse(0, 0, i.Width, i.Height);
                        g.FillPath(new TextureBrush(i), p);
                    }
                    //g.FillEllipse(new TextureBrush(i), 0, 0, i.Width, i.Height);
                }
                return b;
            }
        }
        public static Bitmap WaySTwo(Image image)
        {
            using (Image i =  image)
            {
                Bitmap b = new Bitmap(i.Width, i.Height);
                using (Graphics g = Graphics.FromImage(b))
                {
                    g.DrawImage(i, 0, 0, b.Width, b.Height);
                    int r = Math.Min(b.Width, b.Height) / 2;
                    PointF c = new PointF(b.Width / 2.0F, b.Height / 2.0F);
                    for (int h = 0; h < b.Height; h++)
                        for (int w = 0; w < b.Width; w++)
                            if ((int)Math.Pow(r, 2) < ((int)Math.Pow(w * 1.0 - c.X, 2) + (int)Math.Pow(h * 1.0 - c.Y, 2)))
                            {
                                b.SetPixel(w, h, Color.Transparent);
                            }
                    //画背景色圆
                    using (Pen p = new Pen(System.Drawing.SystemColors.Control))
                        g.DrawEllipse(p, 0, 0, b.Width, b.Height);
                }
                return b;
            }
        }
    }
}
