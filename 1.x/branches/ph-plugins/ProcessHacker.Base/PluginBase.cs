using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ProcessHacker.Base
{
    public abstract class PluginBase
    {
        private ApplicationInstance _appInstance;

        public abstract string Author { get; }
        public abstract string Description { get; }
        public abstract string Name { get; }
        public abstract string Title { get; }
        public abstract void OnLoad();
        public virtual PluginSettingsControlBase OnRetrieveSettingsControl() { return null; }
        public abstract void OnUnload();

        public ApplicationInstance AppInstance
        {
            get { return _appInstance; }
            internal set { _appInstance = value; }
        }
    }
}
