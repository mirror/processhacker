using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ProcessHacker.Base
{
    public sealed class ApplicationInstance
    {
        private Dictionary<string, PluginBase> _plugins = new Dictionary<string, PluginBase>();

        private MainMenuManager _mainMenu;
        private ProcessHighlightingManager _processHighlighting = new ProcessHighlightingManager();
        private ProcessWindowManager _processWindow = new ProcessWindowManager();

        public MainMenuManager MainMenu
        {
            get { return _mainMenu; }
            set { _mainMenu = value; }
        }

        public ProcessHighlightingManager ProcessHighlighting
        {
            get { return _processHighlighting; }
        }

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

        public IEnumerable<PluginBase> GetPlugins()
        {
            return _plugins.Values;
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

        public void OnShutdown()
        {
            foreach (var plugin in _plugins.Values)
                plugin.OnUnload();
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
