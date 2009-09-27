using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace ProgressBarEx
{
  [ToolboxBitmap(typeof(ProgressBar))]
  public class VistaProgressBar : ProgressBar
  {
    private ProgressRenderMode FRenderMode; // = ProgressRenderMode.System;
    private IProgressBarRenderer FRenderer; // = null;
    private ProgressState FState; // = ProgressState.Normal;
    private Timer MarqueeTimer;
    private object MarqueeTag;

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

    private const int WM_USER = 0x400;
    private const int PBM_SETSTATE = WM_USER + 16;
    private const int PBST_NORMAL = 0x0001;
    private const int PBST_ERROR = 0x0002;
    private const int PBST_PAUSED = 0x0003;

    public VistaProgressBar()
      : base()
    {
      MarqueeTimer = new Timer();
      MarqueeTimer.Interval = MarqueeAnimationSpeed;
      MarqueeTimer.Tick += AnimateTimer_OnTick;
      MarqueeTimer.Enabled = false;
    }

    private void AnimateTimer_OnTick(object sender, EventArgs e)
    {
      if (MarqueeTimer.Interval != MarqueeAnimationSpeed)
        MarqueeTimer.Interval = MarqueeAnimationSpeed;

      if (Style == ProgressBarStyle.Marquee)
      {
        ProgressBarMarqueeEventArgs RenderMarqueeArgs = new ProgressBarMarqueeEventArgs(this, MarqueeTag);
        if (FRenderer.UpdateMarquee(RenderMarqueeArgs))
          Invalidate();

        MarqueeTag = RenderMarqueeArgs.MarqueeTag;
      }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      using (e)
      {
        ProgressBarRenderEventArgs RenderArgs = new ProgressBarRenderEventArgs(e.Graphics, this);
        FRenderer.DrawBackground(RenderArgs);

        if (Style == ProgressBarStyle.Marquee)
        {
          ProgressBarMarqueeRenderEventArgs RenderMarqueeArgs = new ProgressBarMarqueeRenderEventArgs(e.Graphics, this, MarqueeTag);
          FRenderer.DrawMarquee(RenderMarqueeArgs);
        }
        else
        {
          ProgressBarValueRenderEventArgs RenderValueArgs = new ProgressBarValueRenderEventArgs(e.Graphics, this);
          FRenderer.DrawBarValue(RenderValueArgs);
        }
      }
    }

    protected override void OnHandleCreated(EventArgs e)
    {
      base.OnHandleCreated(e);
      if (IsWindowVista)
        SetVistaState(FState);
    }

    protected override void WndProc(ref System.Windows.Forms.Message m)
    {
      if ((m.Msg == 15) && IsWindowVista && (FState != ProgressState.Normal))
        SetVistaState(FState);
      base.WndProc(ref m);
    }

    private bool IsWindowVista
    {
      get { return (Environment.OSVersion.Platform == PlatformID.Win32NT) && (Environment.OSVersion.Version.Major > 5); }
    }

    private void SetVistaState(ProgressState state)
    {
      SendMessage(Handle, PBM_SETSTATE, (IntPtr)PBST_NORMAL, IntPtr.Zero);

      switch (state)
      {
        case ProgressState.Pause:
          SendMessage(Handle, PBM_SETSTATE, (IntPtr)PBST_PAUSED, IntPtr.Zero);
          break;
        case ProgressState.Error:
          SendMessage(Handle, PBM_SETSTATE, (IntPtr)PBST_ERROR, IntPtr.Zero);
          break;
        /*default:
          SendMessage(Handle, PBM_SETSTATE, (IntPtr)PBST_NORMAL, IntPtr.Zero);
          break;*/
      }
    }

    private void SetVistaRendererState(VistaProgressBarRenderer renderer, ProgressState state)
    {
      switch (state)
      {
        case ProgressState.Pause:
          renderer.StartColor = Color.FromArgb(192, 192, 0);
          break;
        case ProgressState.Error:
          renderer.StartColor = Color.FromArgb(192, 0, 0);
          break;
        default:
          renderer.StartColor = Color.LimeGreen;
          break;
      }
      renderer.EndColor = renderer.StartColor;
    }

    [Category("Appearance")]
    [DefaultValue(typeof(ProgressRenderMode), "System")]
    public ProgressRenderMode RenderMode
    {
      get { return FRenderMode; }
      set
      {
        if (FRenderMode == value)
          return;

        if (!Enum.IsDefined(typeof(ProgressRenderMode), value))
          throw new InvalidEnumArgumentException();

        switch (value)
        {
          case ProgressRenderMode.System:
            Renderer = null;
            if (IsWindowVista)
              SetVistaState(FState);
            else
              FState = ProgressState.Normal;
            break;
          case ProgressRenderMode.Vista:
            if (IsWindowVista)
            {
              Renderer = null;
              SetVistaState(FState);
            }
            else
            {
              Renderer = new VistaProgressBarRenderer();
              SetVistaRendererState((VistaProgressBarRenderer)Renderer, FState);
            }
            break;
          default:
            throw new NotSupportedException();
        }

        FRenderMode = value;
      }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IProgressBarRenderer Renderer
    {
      get { return FRenderMode == ProgressRenderMode.Custom ? FRenderer : null; }
      set
      {
        if (FRenderer == value)
          return;

        FRenderer = value;

        SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
          ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint, FRenderer != null);

        FRenderMode = FRenderer != null ? ProgressRenderMode.Custom : ProgressRenderMode.System;
        MarqueeTimer.Enabled = (FRenderer != null) && !DesignMode;

        Invalidate();
      }
    }

    [DefaultValue(typeof(ProgressState), "Normal")]
    [Category("Behavior")]
    public ProgressState State
    {
      get { return FState; }
      set
      {
        if ((Style != ProgressBarStyle.Blocks) || (RenderMode == ProgressRenderMode.Custom) ||
          ((RenderMode == ProgressRenderMode.System) && !IsWindowVista))
          value = ProgressState.Normal;

        if (FState == value)
          return;

        FState = value;

        if (!Enum.IsDefined(typeof(ProgressState), value))
          throw new InvalidEnumArgumentException();

        if ((FRenderMode != ProgressRenderMode.Custom) && IsWindowVista)
          SetVistaState(FState);
        else
          if (FRenderMode == ProgressRenderMode.Vista)
          {
            SetVistaRendererState((VistaProgressBarRenderer)FRenderer, FState);
            Invalidate();
          }
      }
    }
  }

  public enum ProgressRenderMode { System, Vista, Custom }

  public enum ProgressState { Normal, Pause, Error }
}
