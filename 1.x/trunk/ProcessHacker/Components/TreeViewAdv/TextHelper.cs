using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Aga.Controls
{
	public static class TextHelper
	{
		public static StringAlignment TranslateAligment(HorizontalAlignment aligment)
		{
		    switch (aligment)
		    {
                case HorizontalAlignment.Left:
		            {
		                return StringAlignment.Near;
		            }
                case HorizontalAlignment.Right:
		            {
		                return StringAlignment.Far;
		            }
                default:
		            {
		                return StringAlignment.Center;
		            }
		    }
		}

	    public static TextFormatFlags TranslateAligmentToFlag(HorizontalAlignment aligment)
        {
            switch (aligment)
            {
                case HorizontalAlignment.Left:
                    {
                        return TextFormatFlags.Left;
                    }
                case HorizontalAlignment.Right:
                    {
                        return TextFormatFlags.Right;
                    }
                default:
                    {
                        return TextFormatFlags.HorizontalCenter;
                    }
            }
        }

	    public static TextFormatFlags TranslateTrimmingToFlag(StringTrimming trimming)
		{
			switch (trimming)
			{
                case StringTrimming.EllipsisCharacter:
			        {
			            return TextFormatFlags.EndEllipsis;
			        }
                case StringTrimming.EllipsisPath:
			        {
			            return TextFormatFlags.PathEllipsis;
			        }
                case StringTrimming.EllipsisWord:
			        {
			            return TextFormatFlags.WordEllipsis;
			        }
                case StringTrimming.Word:
			        {
			            return TextFormatFlags.WordBreak;
			        }
                default:
			        {
			            return TextFormatFlags.Default;
			        }
			}
		}
	}
}
