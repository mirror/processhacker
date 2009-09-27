using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ProgressBarEx
{
  public class VistaProgressBarRenderer : IProgressBarRenderer
  {
    private Color FStartColor;
    private Color FEndColor;
    private bool StartEndColorEqual; // = false;
    private Color FHighlightColor;
    private ColorBlend FMarqueeBlend;

    private ColorBlend ShadowBlend1;
    private ColorBlend ShadowBlend2;

    private Color Black20 = Color.FromArgb(20, Color.Black);
    private Color Black30 = Color.FromArgb(30, Color.Black);
    private Color Black40 = Color.FromArgb(40, Color.Black);
    private Color White100 = Color.FromArgb(100, Color.White);
    private Color White128 = Color.FromArgb(128, Color.White);
    private Color Color178 = Color.FromArgb(178, 178, 178);

    public Color BackgroundColor = Color.FromArgb(201, 201, 201);
    public Color HighlightColor
    {
      get { return Color.FromArgb(255, FHighlightColor); }
      set { FHighlightColor = Color.FromArgb(100, value); }
    }

    public Color StartColor
    {
      get { return FStartColor; }
      set
      {
        FStartColor = value;
        FMarqueeBlend = null;
        StartEndColorEqual = FStartColor == FEndColor;
      }
    }

    public Color EndColor
    {
      get { return FEndColor; }
      set
      {
        FEndColor = value;
        FMarqueeBlend = null;
        StartEndColorEqual = FStartColor == FEndColor;
      }
    }

    private void Initialize()
    {
      HighlightColor = Color.White;

      ShadowBlend1 = new ColorBlend(3);
      ShadowBlend1.Colors = new Color[] { Color.Transparent, Black40, Color.Transparent };
      ShadowBlend1.Positions = new float[] { 0.0F, 0.2F, 1.0F };

      ShadowBlend2 = new ColorBlend(3);
      ShadowBlend2.Colors = new Color[] { Color.Transparent, Black40, Color.Transparent };
      ShadowBlend2.Positions = new float[] { 0.0F, 0.8F, 1.0F };

      StartEndColorEqual = FStartColor == FEndColor;
    }

    public VistaProgressBarRenderer()
    {
      FStartColor = Color.LimeGreen;
      FEndColor = Color.LimeGreen;
      Initialize();
    }

    public VistaProgressBarRenderer(Color startColor, Color endColor)
    {
      FStartColor = startColor;
      FEndColor = endColor;
      Initialize();
    }

    public void DrawBackground(ProgressBarRenderEventArgs e)
    {
      e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
      e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

      DrawBackground(e.Graphics, e.Bounds);
      DrawBackgroundShadows(e.Graphics, e.Bounds);
    }

    public void DrawBarValue(ProgressBarValueRenderEventArgs e)
    {
      e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
      e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

      float ProgressValue = e.Value * 1.0F / (e.Maximum - e.Minimum);

      DrawBar(e.Graphics, e.Bounds, ProgressValue);
      DrawBarShadows(e.Graphics, e.Bounds, ProgressValue);
      DrawBarHighlight(e.Graphics, e.Bounds);
      DrawInnerStroke(e.Graphics, e.Bounds);
      DrawOuterStroke(e.Graphics, e.Bounds);
    }

    public void DrawMarquee(ProgressBarMarqueeRenderEventArgs e)
    {
      e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
      e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

      int Position = e.MarqueeTag is int ? (int)e.MarqueeTag : -120;

      DrawMarquee(e.Graphics, e.Bounds, Position);
      DrawBarHighlight(e.Graphics, e.Bounds);
      DrawInnerStroke(e.Graphics, e.Bounds);
      DrawOuterStroke(e.Graphics, e.Bounds);
    }

    public bool UpdateMarquee(ProgressBarMarqueeEventArgs e)
    {
      if (e.MarqueeTag is int)
      {
        int Position = (int)e.MarqueeTag;

        Position += 4;
        if (Position > e.Bounds.Width)
          Position = -120;
        e.MarqueeTag = Position;
      }
      else
        e.MarqueeTag = -120;

      return true;
    }

    private ColorBlend MarqueeBlend
    {
      get
      {
        if (FMarqueeBlend == null)
        {
          Color MarqueeColor = Color.FromArgb((FStartColor.R + FEndColor.R) / 2, (FStartColor.G + FEndColor.G) / 2, (FStartColor.B + FEndColor.B) / 2);
          FMarqueeBlend = new ColorBlend(3);
          FMarqueeBlend.Colors = new Color[] { Color.Transparent, MarqueeColor, Color.Transparent };
          FMarqueeBlend.Positions = new float[] { 0.0F, 0.5F, 1.0F };
        }
        return FMarqueeBlend;
      }
    }

    private GraphicsPath RoundRect(RectangleF r, float r1, float r2, float r3, float r4)
    {
      float x = r.X, y = r.Y, w = r.Width, h = r.Height;
      GraphicsPath rr = new GraphicsPath();
      rr.AddBezier(x, y + r1, x, y, x + r1, y, x + r1, y);
      rr.AddLine(x + r1, y, x + w - r2, y);
      rr.AddBezier(x + w - r2, y, x + w, y, x + w, y + r2, x + w, y + r2);
      rr.AddLine(x + w, y + r2, x + w, y + h - r3);
      rr.AddBezier(x + w, y + h - r3, x + w, y + h, x + w - r3, y + h, x + w - r3, y + h);
      rr.AddLine(x + w - r3, y + h, x + r4, y + h);
      rr.AddBezier(x + r4, y + h, x, y + h, x, y + h - r4, x, y + h - r4);
      rr.AddLine(x, y + h - r4, x, y + r1);
      return rr;
    }

    private Color GetIntermediateColor(float value)
    {
      int ca = FStartColor.A, cr = FStartColor.R, cg = FStartColor.G, cb = FStartColor.B;
      int c2a = FEndColor.A, c2r = FEndColor.R, c2g = FEndColor.G, c2b = FEndColor.B;

      int a = (int)Math.Abs(ca + (ca - c2a) * value);
      int r = (int)Math.Abs(cr - ((cr - c2r) * value));
      int g = (int)Math.Abs(cg - ((cg - c2g) * value));
      int b = (int)Math.Abs(cb - ((cb - c2b) * value));

      a = Math.Min(255, a);
      r = Math.Min(255, r);
      g = Math.Min(255, g);
      b = Math.Min(255, b);

      return Color.FromArgb(a, r, g, b);
    }

    private void DrawBackground(Graphics g, Rectangle rect)
    {
      rect.Width--;
      rect.Height--;
      using (GraphicsPath BackgroundPath = RoundRect(rect, 2, 2, 2, 2))
      using (Brush BackBrush = new SolidBrush(BackgroundColor))
        g.FillPath(BackBrush, BackgroundPath);
    }

    private void DrawBackgroundShadows(Graphics g, Rectangle rect)
    {
      Rectangle lr = new Rectangle(rect.Left + 2, rect.Top + 2, 10, rect.Height - 5);
      using (Brush lg = new LinearGradientBrush(lr, Black30, Color.Transparent, LinearGradientMode.Horizontal))
      {
        lr.X--;
        g.FillRectangle(lg, lr);
      }

      Rectangle rr = new Rectangle(rect.Right - 12, rect.Top + 2, 10, rect.Height - 5);
      using (Brush rg = new LinearGradientBrush(rr, Color.Transparent, Black20, LinearGradientMode.Horizontal))
        g.FillRectangle(rg, rr);
    }

    private void DrawBar(Graphics g, Rectangle rect, float value)
    {
      Rectangle r = new Rectangle(rect.Left + 1, rect.Top + 2, rect.Width - 3, rect.Height - 3);
      r.Width = (int)(value * rect.Width);
      using (Brush BarBrush = new SolidBrush(StartEndColorEqual ? FStartColor : GetIntermediateColor(value)))
        g.FillRectangle(BarBrush, r);
    }

    private void DrawBarShadows(Graphics g, Rectangle rect, float value)
    {
      Rectangle lr = new Rectangle(rect.Left + 1, rect.Top + 2, 15, rect.Height - 3);

      using (LinearGradientBrush lg = new LinearGradientBrush(lr, Color.White, Color.White, LinearGradientMode.Horizontal))
      {
        lg.InterpolationColors = ShadowBlend1;
        lr.X--;
        g.FillRectangle(lg, lr);
      }

      Rectangle rr = new Rectangle(rect.Right - 3, rect.Top + 2, 15, rect.Height - 3);
      rr.X = (int)(value * rect.Width) - 14;

      using (LinearGradientBrush rg = new LinearGradientBrush(rr, Color.Black, Color.Black, LinearGradientMode.Horizontal))
      {
        rg.InterpolationColors = ShadowBlend2;
        g.FillRectangle(rg, rr);
      }
    }

    private void DrawBarHighlight(Graphics g, Rectangle rect)
    {
      Rectangle tr = new Rectangle(rect.Left + 1, rect.Top + 1, rect.Width - 1, 6);
      using (GraphicsPath tp = RoundRect(tr, 2, 2, 0, 0))
      {
        g.SetClip(tp);
        using (Brush tg = new LinearGradientBrush(tr, Color.WhiteSmoke, White128, LinearGradientMode.Vertical))
          g.FillPath(tg, tp);
        g.ResetClip();
      }

      Rectangle br = new Rectangle(rect.Left + 1, rect.Bottom - 8, rect.Width - 1, 6);
      using (GraphicsPath bp = RoundRect(br, 0, 0, 2, 2))
      {
        g.SetClip(bp);
        using (Brush bg = new LinearGradientBrush(br, Color.Transparent, FHighlightColor, LinearGradientMode.Vertical))
          g.FillPath(bg, bp);
        g.ResetClip();
      }
    }

    private void DrawInnerStroke(Graphics g, Rectangle rect)
    {
      rect.X++;
      rect.Y++;
      rect.Width -= 3;
      rect.Height -= 3;
      using (GraphicsPath rr = RoundRect(rect, 2, 2, 2, 2))
      using (Pen InnerPen = new Pen(White100))
        g.DrawPath(InnerPen, rr);
    }

    private void DrawMarquee(Graphics g, Rectangle rect, int position)
    {
      Rectangle r = new Rectangle(rect.Left + position, rect.Top + 0, 100, rect.Height);
      using (LinearGradientBrush lgb = new LinearGradientBrush(r, Color.White, Color.White, LinearGradientMode.Horizontal))
      {
        lgb.InterpolationColors = MarqueeBlend;

        rect.Width--;
        rect.Height--;
        using (GraphicsPath rr = RoundRect(rect, 2, 2, 2, 2))
        {
          g.SetClip(rr);
          g.FillRectangle(lgb, r);
          g.ResetClip();
        }
      }
    }

    private void DrawOuterStroke(Graphics g, Rectangle rect)
    {
      rect.Width--;
      rect.Height--;
      using (GraphicsPath outerPath = RoundRect(rect, 2, 2, 2, 2))
      using (Pen OuterPen = new Pen(Color178))
        g.DrawPath(OuterPen, outerPath);
    }
  }
}
