namespace System
{
    using Windows.Forms.VisualStyles;

    public static class ExplorerVisualStyle
    {
        private static bool? useVisualStyles;
        private static bool? visualStylesEnabled;

        public static bool VisualStylesEnabled
        {
            get
            {
                if (!visualStylesEnabled.HasValue)
                    visualStylesEnabled = (System.Windows.Forms.Application.RenderWithVisualStyles && VisualStyleInformation.IsEnabledByUser);

                return visualStylesEnabled.Value;
            }
        }

        /// <summary>
        /// Checks Application and OS Visual Style status
        /// </summary>
        public static bool UseVisualStyles
        {
            get 
            {
                // if useVisualStyles is null, check application and OS VisualStyle status, cache result and return cached result.
                // these are the only two checks required because of the way RenderWithVisualStyles and IsEnabledByUser is implemented.
  
                if (!useVisualStyles.HasValue)
                    useVisualStyles = VisualStylesEnabled && VisualStyleRenderer.IsElementDefined(VisualStyleElement.CreateElement("Explorer::TreeView", 4, 1));

                return useVisualStyles.Value; 
            }
            set 
            {
                // allow application to enable/disable VisualStyles if required.
                useVisualStyles = value;
            }
        }

        #region Default Renderers

        // TODO: add Backwards compatibility with XP by moving Custom Colors used in ListTreeControl.cs to here.

        private static VisualStyleRenderer minusRenderer;
        private static VisualStyleRenderer plusRenderer;

        public static VisualStyleRenderer MinusRenderer
        {
            get
            {
                if (minusRenderer == null)
                    minusRenderer = new VisualStyleRenderer(VisualStyleElement.TreeView.Glyph.Closed);

                return minusRenderer;
            }
        }
   
        public static VisualStyleRenderer PlusRenderer
        {
            get
            {
                if (plusRenderer == null)
                    plusRenderer = new VisualStyleRenderer(VisualStyleElement.TreeView.Glyph.Opened);

                return plusRenderer;
            }
        }

        #endregion

        #region TreeView Renderers

        private static VisualStyleRenderer tvClosedRenderer;
        private static VisualStyleRenderer tvOpenedRenderer;
        private static VisualStyleRenderer tvHoverClosedRenderer;
        private static VisualStyleRenderer tvHoverOpenedRenderer;
        private static VisualStyleRenderer tvItemHoverRenderer;
        private static VisualStyleRenderer tvItemSelectedRenderer;
        private static VisualStyleRenderer tvLostFocusSelectedRenderer;
        private static VisualStyleRenderer tvSelectedHoverRenderer;

        /// <summary>
        /// Default style used for the TreeViewNode closed Glyph.
        /// </summary>
        public static VisualStyleRenderer TvClosedRenderer
        {
            get
            {
                if (tvClosedRenderer == null)
                    tvClosedRenderer = new VisualStyleRenderer("Explorer::TreeView", 2, 1);

                return tvClosedRenderer;
            }
        }

        /// <summary>
        /// Default style used for the TreeViewNode opened Glyph.
        /// </summary>
        public static VisualStyleRenderer TvOpenedRenderer
        {
            get
            {
                if (tvOpenedRenderer == null)
                    tvOpenedRenderer = new VisualStyleRenderer("Explorer::TreeView", 2, 2);

                return tvOpenedRenderer;
            }
        }

        /// <summary>
        /// Style used when the mouse is positioned over the TreeView's Closed Glyph.
        /// </summary>
        public static VisualStyleRenderer TvHoverClosedRenderer
        {
            get
            {
                if (tvHoverClosedRenderer == null)
                    tvHoverClosedRenderer = new VisualStyleRenderer("Explorer::TreeView", 4, 1);

                return tvHoverClosedRenderer;
            }
        }

        /// <summary>
        /// Style used when the mouse is positioned over the TreeView's Opened Glyph.
        /// </summary>
        public static VisualStyleRenderer TvHoverOpenedRenderer
        {
            get
            {
                if (tvHoverOpenedRenderer == null)
                    tvHoverOpenedRenderer = new VisualStyleRenderer("Explorer::TreeView", 4, 2);

                return tvHoverOpenedRenderer;
            }
        }

        /// <summary>
        /// Style used when mouse is hovering over a TreeViewNode.
        /// </summary>
        public static VisualStyleRenderer TvItemHoverRenderer
        {
            get
            {
                if (tvItemHoverRenderer == null)
                    tvItemHoverRenderer = new VisualStyleRenderer("Explorer::TreeView", 1, 2);

                return tvItemHoverRenderer;
            }
        }

        /// <summary>
        /// Style used when a TreeViewNode is selected.
        /// </summary>
        public static VisualStyleRenderer TvItemSelectedRenderer
        {
            get
            {
                if (tvItemSelectedRenderer == null)
                    tvItemSelectedRenderer = new VisualStyleRenderer("Explorer::TreeView", 1, 3);

                return tvItemSelectedRenderer;
            }
        }

        /// <summary>
        /// Style used when a TreeViewNode is selected but remains highlighted if the control has lost focus. (when TreeView.HideSelecton = false)) 
        /// </summary>
        public static VisualStyleRenderer TvLostFocusSelectedRenderer
        {
            get
            {
                if (tvLostFocusSelectedRenderer == null)
                    tvLostFocusSelectedRenderer = new VisualStyleRenderer("Explorer::TreeView", 1, 5);

                return tvLostFocusSelectedRenderer;
            }
        }

        /// <summary>
        /// Style used when the mouse is hovering over a currently selected TreeViewNode.
        /// </summary>
        public static VisualStyleRenderer TvSelectedItemHoverRenderer
        {
            get
            {
                if (tvSelectedHoverRenderer == null)
                    tvSelectedHoverRenderer = new VisualStyleRenderer("Explorer::TreeView", 1, 6);

                return tvSelectedHoverRenderer;
            }
        }

        #endregion

        #region ListView Renderers

        private static VisualStyleRenderer lvItemHoverRenderer;
        private static VisualStyleRenderer lvItemSelectedRenderer;
        private static VisualStyleRenderer lvLostFocusSelectedRenderer;
        private static VisualStyleRenderer lvSelectedItemHoverRenderer;

        /// <summary>
        /// Style used when mouse is hovering over a ListViewItem.
        /// </summary>
        public static VisualStyleRenderer LvItemHoverRenderer
        {
            get
            {
                if (lvItemHoverRenderer == null)
                    lvItemHoverRenderer = new VisualStyleRenderer("Explorer::ListView", 1, 2);

                return lvItemHoverRenderer;
            }
        }

        /// <summary>
        /// Style used when a ListViewItem is selected.
        /// </summary>
        public static VisualStyleRenderer LvItemSelectedRenderer
        {
            get
            {
                if (lvItemSelectedRenderer == null)
                    lvItemSelectedRenderer = new VisualStyleRenderer("Explorer::ListView", 1, 3);

                return lvItemSelectedRenderer;
            }
        }

        /// <summary>
        /// Style used when a ListViewItem is selected but remains highlighted if the control has lost focus. (when ListView.HideSelecton = false)) 
        /// </summary>
        public static VisualStyleRenderer LvLostFocusSelectedRenderer
        {
            get
            {
                if (lvLostFocusSelectedRenderer == null)
                    lvLostFocusSelectedRenderer = new VisualStyleRenderer("Explorer::ListView", 1, 5);

                return lvLostFocusSelectedRenderer;
            }
        }
        
        /// <summary>
        /// Style used when the mouse is hovering over a currently selected ListViewItem.
        /// </summary>
        public static VisualStyleRenderer LvSelectedItemHoverRenderer
        {
            get
            {
                if (lvSelectedItemHoverRenderer == null)
                    lvSelectedItemHoverRenderer = new VisualStyleRenderer("Explorer::ListView", 1, 6);

                return lvSelectedItemHoverRenderer;
            }
        }

        #endregion
    }
}
