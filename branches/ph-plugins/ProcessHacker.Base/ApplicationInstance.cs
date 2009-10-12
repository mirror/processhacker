using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ProcessHacker.Base
{
    public sealed class ApplicationInstance
    {
        private Dictionary<string, PluginBase> _plugins = new Dictionary<string, PluginBase>();

        private ProcessWindowManager _processWindow = new ProcessWindowManager();

        public ProcessWindowManager ProcessWindow
        {
            get { return _processWindow; }
        }

        public void AddPlugin(PluginBase plugin)
        {
            if (_plugins.ContainsKey(plugin.Name))
                throw new InvalidOperationException("The plugin has already been loaded.");

            plugin.AppInstance = this;
            plugin.OnLoad();
            _plugins.Add(plugin.Name, plugin);
        }

        public PluginBase GetPlugin(string name)
        {
            if (!_plugins.ContainsKey(name))
                return null;

            return _plugins[name];
        }

        public void LoadPlugin(string fileName)
        {
            Assembly assembly = Assembly.LoadFile(fileName);

            foreach (var type in assembly.GetTypes())
            {
                if (type.IsPublic && type.IsSubclassOf(typeof(PluginBase)))
                {
                    this.AddPlugin((PluginBase)Activator.CreateInstance(type));
                }
            }
        }

        public void RemovePlugin(PluginBase plugin)
        {
            if (!_plugins.ContainsKey(plugin.Name))
                throw new InvalidOperationException("The plugin is not loaded.");

            plugin.OnUnload();
            _plugins.Remove(plugin.Name);
        }
    }
}
