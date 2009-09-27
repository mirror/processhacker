using System;
using System.Drawing;
using System.Windows.Forms;

namespace Nomad.Controls
{
  public class ProgressBarRenderEventArgs : EventArgs
  {
    private Rectangle? FBounds;

    public ProgressBarRenderEventArgs(Graphics graphics, VistaProgressBar progressBar)
    {
      if (graphics == null)
        throw new ArgumentNullException("graphics");
      if (progressBar == null)
        throw new ArgumentNullException("progressBar");

      Graphics = graphics;
      ProgressBar = progressBar;
    }
    public ProgressBarRenderEventArgs(Graphics graphics, VistaProgressBar progressBar, Rectangle bounds)
      : this(graphics, progressBar)
    {
      FBounds = bounds;
    }

    public Rectangle Bounds
    {
      get { return FBounds.HasValue ? FBounds.Value : ProgressBar.ClientRectangle; }
    }
    public readonly Graphics Graphics;
    public readonly VistaProgressBar ProgressBar;
  }

  public class ProgressBarValueRenderEventArgs : ProgressBarRenderEventArgs
  {
    public ProgressBarValueRenderEventArgs(Graphics graphics, VistaProgressBar progressBar)
      : base(graphics, progressBar)
    {
      Value = ProgressBar.Value;
      Minimum = ProgressBar.Minimum;
      Maximum = ProgressBar.Maximum;
    }

    public readonly int Value;
    public readonly int Minimum;
    public readonly int Maximum;
  }

  public class ProgressBarMarqueeRenderEventArgs : ProgressBarRenderEventArgs
  {
    public ProgressBarMarqueeRenderEventArgs(Graphics graphics, VistaProgressBar progressBar, object marqueeTag)
      : base(graphics, progressBar)
    {
      MarqueeTag = marqueeTag;
    }

    public object MarqueeTag;
  }

  public class ProgressBarMarqueeEventArgs : EventArgs
  {
    private Rectangle? FBounds;

    public ProgressBarMarqueeEventArgs(VistaProgressBar progressBar, object marqueeTag)
    {
      if (progressBar == null)
        throw new ArgumentNullException("progressBar");

      ProgressBar = progressBar;
      MarqueeTag = marqueeTag;
    }

    public Rectangle Bounds
    {
      get { return FBounds.HasValue ? FBounds.Value : ProgressBar.ClientRectangle; }
    }
    public readonly VistaProgressBar ProgressBar;
    public object MarqueeTag;
  }

  public interface IProgressBarRenderer
  {
    void DrawBackground(ProgressBarRenderEventArgs e);
    void DrawBarValue(ProgressBarValueRenderEventArgs e);
    void DrawMarquee(ProgressBarMarqueeRenderEventArgs e);
    bool UpdateMarquee(ProgressBarMarqueeEventArgs e);
  }
}
