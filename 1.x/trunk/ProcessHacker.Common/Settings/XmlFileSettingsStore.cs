/*
 * Process Hacker - 
 *   XML file settings store
 * 
 * Copyright (C) 2009 wj32
 * 
 * This file is part of Process Hacker.
 * 
 * Process Hacker is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Process Hacker is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Process Hacker.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace ProcessHacker.Common.Settings
{
    /// <summary>
    /// Provides an XML-based settings store.
    /// </summary>
    public sealed class XmlFileSettingsStore : ISettingsStore
    {
        private const string _rootElementName = "settings";
        private const string _settingElementName = "setting";
        private const string _nameAttributeName = "name";

        private string _fileName;
        private object _docLock = new object();
        private XmlDocument _doc;
        private XmlNode _rootNode;

        /// <summary>
        /// Creates a new settings store from the specified file.
        /// </summary>
        /// <param name="fileName">The name of the settings file.</param>
        public XmlFileSettingsStore(string fileName)
        {
            _fileName = fileName;
            this.Initialize();
        }

        /// <summary>
        /// Gets the settings file name.
        /// </summary>
        public string FileName
        {
            get { return _fileName; }
        }

        /// <summary>
        /// Flushes persistent storage.
        /// </summary>
        public void Flush()
        {
            if (!string.IsNullOrEmpty(_fileName))
            {
                lock (_docLock)
                {
                    _doc.Save(_fileName);
                }
            }
        }

        /// <summary>
        /// Gets a setting value from persistent storage.
        /// </summary>
        /// <param name="name">The case-sensitive name of the setting.</param>
        /// <returns>A string if a value was found for the setting, otherwise null.</returns>
        public string GetValue(string name)
        {
            lock (_docLock)
            {
                var nodes = _rootNode.SelectNodes(_settingElementName + "[@" + _nameAttributeName + "='" + name + "']");

                if (nodes.Count == 0)
                    return null;

                return nodes[0].InnerText;
            }
        }

        private void Initialize()
        {
            _doc = new XmlDocument();

            // Does the file exist? If not, initialize a new XML document.
            if (!string.IsNullOrEmpty(_fileName) && File.Exists(_fileName))
            {
                _doc.Load(_fileName);
            }
            else
            {
                this.InitializeNew(_doc);
            }

            _rootNode = _doc.SelectSingleNode("/" + _rootElementName);
        }

        private void InitializeNew(XmlDocument doc)
        {
            doc.RemoveAll();
            doc.AppendChild(doc.CreateElement(_rootElementName));
        }

        /// <summary>
        /// Resets the persistent storage, deleting all stored values.
        /// </summary>
        /// <remarks>
        /// This usually means deleting the settings file. A flush operation 
        /// is also assumed to be performed.
        /// </remarks>
        public void Reset()
        {
            lock (_docLock)
            {
                this.InitializeNew(_doc);

                if (!string.IsNullOrEmpty(_fileName))
                    _doc.Save(_fileName);
            }
        }

        /// <summary>
        /// Saves a setting value into persistent storage.
        /// </summary>
        /// <param name="name">The case-sensitive name of the setting.</param>
        /// <param name="value">The new value of the setting.</param>
        /// <remarks>
        /// The <paramref name="value"/> string must not be null 
        /// (but can be empty).
        /// </remarks>
        public void SetValue(string name, string value)
        {
            lock (_docLock)
            {
                var nodes = _rootNode.SelectNodes(_settingElementName + "[@" + _nameAttributeName + "='" + name + "']");

                if (nodes.Count != 0)
                {
                    nodes[0].InnerText = value;
                }
                else
                {
                    var settingElement = _doc.CreateElement(_settingElementName);
                    var nameAttribute = _doc.CreateAttribute(_nameAttributeName);

                    nameAttribute.Value = name;
                    settingElement.Attributes.Append(nameAttribute);
                    settingElement.InnerText = value;

                    _rootNode.AppendChild(settingElement);
                }
            }
        }
    }
}
