using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ProgressBarEx
{
  public enum MarqueeStyle { Continuous, LeftRight }

  public class XPProgressBarRenderer : IProgressBarRenderer
  {
    public const int DefaultChunkWidth = 7;

    private int FChunkWidth;
    private int FSeparatorWidth = 1;
    private int FMarqueeChunks = 5;
    private Blend BarBlend;

    public Color BarColor;
    public Color BarBackgroundColor = Color.White;
    public MarqueeStyle MarqueeStyle = MarqueeStyle.LeftRight;

    public int ChunkWidth
    {
      get { return FChunkWidth; }
      set
      {
        if (value < 2)
          throw new ArgumentOutOfRangeException();
        FChunkWidth = value;
      }
    }

    public int SeparatorWidth
    {
      get { return FSeparatorWidth; }
      set
      {
        if (value < 0)
          throw new ArgumentOutOfRangeException();
        FSeparatorWidth = value;
      }
    }

    public int MarqueeChunks
    {
      get { return FMarqueeChunks; }
      set
      {
        if (value < 1)
          throw new ArgumentOutOfRangeException();
        FMarqueeChunks = value;
      }
    }

    public XPProgressBarRenderer() : this(Color.LimeGreen, DefaultChunkWidth) { }
    public XPProgressBarRenderer(Color barColor) : this(barColor, DefaultChunkWidth) { }
    public XPProgressBarRenderer(Color barColor, int chunkWidth)
    {
      BarColor = barColor;
      ChunkWidth = chunkWidth;

      float[] relativeIntensities = { 0.1f, 1.0f, 1.0f, 1.0f, 1.0f, 0.85f, 0.1f };
      float[] relativePositions = { 0.0f, 0.2f, 0.5f, 0.5f, 0.5f, 0.8f, 1.0f };

      BarBlend = new Blend();
      BarBlend.Factors = relativeIntensities;
      BarBlend.Positions = relativePositions;
    }

    public void DrawBackground(ProgressBarRenderEventArgs e)
    {
      Point[] points = {   
        new Point(e.Bounds.Left + 2, e.Bounds.Top + 1), 
				new Point(e.Bounds.Right - 2, e.Bounds.Top + 1), 
				new Point(e.Bounds.Right - 1, e.Bounds.Top + 2), 
				new Point(e.Bounds.Right, e.Bounds.Top + 3), 
				new Point(e.Bounds.Right, e.Bounds.Bottom - 4), 
				new Point(e.Bounds.Right - 1, e.Bounds.Bottom - 3), 
				new Point(e.Bounds.Right - 2, e.Bounds.Bottom - 2), 
				new Point(e.Bounds.Left + 2, e.Bounds.Bottom - 1), 
				new Point(e.Bounds.Left + 1, e.Bounds.Bottom - 3), 
				new Point(e.Bounds.Left, e.Bounds.Bottom - 4), 
				new Point(e.Bounds.Left, e.Bounds.Top + 3),
				new Point(e.Bounds.Left + 1, e.Bounds.Top + 2),
			};

      using (GraphicsPath path = new GraphicsPath())
      {
        path.AddLines(points);
        using (Brush brush = new SolidBrush(BarBackgroundColor))
          e.Graphics.FillPath(brush, path);
      }

      DrawBorder(e.Graphics, e.Bounds);
    }

    public void DrawBarValue(ProgressBarValueRenderEventArgs e)
    {
      if ((e.Maximum == e.Minimum) || (e.Value == 0))
        return;

      int AvailableWidth = e.Bounds.Width - 6;
      int FillWidth = AvailableWidth * e.Value / (e.Maximum - e.Minimum);

      if (FillWidth == 0)
        return;

      if ((e.ProgressBar.Style == ProgressBarStyle.Blocks) && (FillWidth % ChunkWidth != 0))
      {
        int rest = FillWidth % ChunkWidth;
        FillWidth += ChunkWidth - rest;
        if (FillWidth > AvailableWidth)
          FillWidth = AvailableWidth;
      }

      Rectangle ChunkBar = new Rectangle(e.Bounds.Left + 3, e.Bounds.Top + 2, FillWidth, e.Bounds.Bottom - 4);

      using (LinearGradientBrush brush = new LinearGradientBrush(ChunkBar, BarBackgroundColor, BarColor, 90.0f))
      {
        brush.Blend = BarBlend;
        e.Graphics.FillRectangle(brush, ChunkBar);
      }

      if ((e.ProgressBar.Style == ProgressBarStyle.Blocks) && (SeparatorWidth > 0))
      {
        int sepCount = (int)(FillWidth / ChunkWidth);

        using (Pen SeparatorPen = new Pen(BarBackgroundColor, SeparatorWidth))
          for (int i = 0; i <= sepCount; i++)
          {
            int X = e.Bounds.Left + ChunkWidth * i + 2;
            e.Graphics.DrawLine(SeparatorPen, X, e.Bounds.Top + 2, X, e.Bounds.Bottom - 2);
          }
      }
    }

    private struct MarqueeData
    {
      public int StartChunk;
      public bool Reverse;
    }

    private MarqueeData GetMarqueeData(object marqueeTag)
    {
      MarqueeData Marquee;
      if (marqueeTag is MarqueeData)
        Marquee = (MarqueeData)marqueeTag;
      else
      {
        Marquee = new MarqueeData();
        Marquee.StartChunk = -MarqueeChunks;
      }
      return Marquee;
    }

    public void DrawMarquee(ProgressBarMarqueeRenderEventArgs e)
    {
      int AvailableWidth = e.Bounds.Width - 6;

      MarqueeData Marquee = GetMarqueeData(e.MarqueeTag);

      int StartPosition;
      int MarqueeWidth;

      if (Marquee.StartChunk < 0)
      {
        StartPosition = 0;
        MarqueeWidth = (MarqueeChunks + Math.Max(-MarqueeChunks, Marquee.StartChunk)) * ChunkWidth;
      }
      else
      {
        StartPosition = Marquee.StartChunk * ChunkWidth;
        MarqueeWidth = ChunkWidth * MarqueeChunks;
      }

      if (StartPosition + MarqueeWidth > AvailableWidth)
        MarqueeWidth = AvailableWidth - StartPosition;

      Rectangle ChunkBar = new Rectangle(e.Bounds.Left + 3 + StartPosition, e.Bounds.Top + 2, MarqueeWidth, e.Bounds.Bottom - 4);
      if (ChunkBar.Width > 0)
      {
        using (LinearGradientBrush brush = new LinearGradientBrush(ChunkBar, BarBackgroundColor, BarColor, 90.0f))
        {
          brush.Blend = BarBlend;
          e.Graphics.FillRectangle(brush, ChunkBar);
        }

        if (SeparatorWidth > 0)
          using (Pen SeparatorPen = new Pen(BarBackgroundColor, SeparatorWidth))
            for (int i = 0; (i < MarqueeChunks) && (StartPosition + ChunkWidth * i < AvailableWidth); i++)
            {
              int X = e.Bounds.Left + StartPosition + ChunkWidth * i + 2;
              e.Graphics.DrawLine(SeparatorPen, X, e.Bounds.Top + 2, X, e.Bounds.Bottom - 2);
            }
      }

    }

    public bool UpdateMarquee(ProgressBarMarqueeEventArgs e)
    {
      MarqueeData Marquee = GetMarqueeData(e.MarqueeTag);

      int MaxChunks = (e.Bounds.Width - 5) / ChunkWidth;

      if (MarqueeStyle == MarqueeStyle.LeftRight)
      {
        Marquee.StartChunk += Marquee.Reverse ? -1 : 1;

        if (Marquee.StartChunk > MaxChunks)
          Marquee.Reverse = true;
        else
          if (Marquee.StartChunk < -MarqueeChunks)
            Marquee.Reverse = false;
      }
      else
      {
        Marquee.StartChunk++;

        if (Marquee.StartChunk > MaxChunks)
          Marquee.StartChunk = -MarqueeChunks;
      }
      e.MarqueeTag = Marquee;

      return true;
    }

    private void DrawBorder(Graphics g, Rectangle rect)
    {
      Point[] points = {   
        new Point(rect.Left, rect.Top + 2), 
				new Point(rect.Left + 1, rect.Top + 1), 
				new Point(rect.Left + 2, rect.Top + 0), 
				new Point(rect.Right - 3, rect.Top + 0), 
				new Point(rect.Right - 2, rect.Top + 1), 
				new Point(rect.Right - 1, rect.Top + 2), 
				new Point(rect.Right - 1, rect.Bottom - 3), 
				new Point(rect.Right - 2, rect.Bottom - 2), 
				new Point(rect.Right - 3, rect.Bottom - 1), 
				new Point(rect.Left + 2, rect.Bottom - 1), 
				new Point(rect.Left + 1, rect.Bottom - 2),
				new Point(rect.Left, rect.Bottom - 3),
				new Point(rect.Left, 2), 
			};

      Point[] points2 = {  
        new Point(rect.Left + 1, rect.Bottom - 3), 
        new Point(rect.Left + 1, rect.Top + 2),
				new Point(rect.Left + 2, rect.Top + 1), 
				new Point(rect.Left + 3, rect.Top + 1), 
				new Point(rect.Right - 4, rect.Top + 1), 
				new Point(rect.Right - 3, rect.Top + 1), 
				new Point(rect.Right - 2, rect.Top + 2), 
			};

      g.SmoothingMode = SmoothingMode.AntiAlias;

      g.DrawCurve(Pens.Gray, points, 0);
      g.DrawCurve(Pens.LightGray, points2, 0);

      g.SmoothingMode = SmoothingMode.None;
    }
  }
}
