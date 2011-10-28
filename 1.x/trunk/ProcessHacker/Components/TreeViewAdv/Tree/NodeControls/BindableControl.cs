using System;
using System.Reflection;
using System.ComponentModel;

namespace Aga.Controls.Tree.NodeControls
{
    public abstract class BindableControl : NodeControl
    {
        private struct MemberAdapter
        {
            private object _obj;
            private PropertyInfo _pi;
            private FieldInfo _fi;

            public static readonly MemberAdapter Empty;

            public Type MemberType
            {
                get
                {
                    if (_pi != null)
                        return _pi.PropertyType;

                    if (_fi != null)
                        return _fi.FieldType;

                    return null;
                }
            }

            public object Value
            {
                get
                {
                    if (_pi != null && _pi.CanRead)
                        return _pi.GetValue(_obj, null);

                    if (_fi != null)
                        return _fi.GetValue(_obj);

                    return null;
                }
                set
                {
                    if (_pi != null && _pi.CanWrite)
                        _pi.SetValue(_obj, value, null);
                    else if (_fi != null)
                        _fi.SetValue(_obj, value);
                }
            }

            public MemberAdapter(object obj, PropertyInfo pi)
            {
                _obj = obj;
                _pi = pi;
                _fi = null;
            }

            public MemberAdapter(object obj, FieldInfo fi)
            {
                _obj = obj;
                _fi = fi;
                _pi = null;
            }
        }

        #region Properties

        [DefaultValue(false), Category("Data")]
        public bool VirtualMode { get; set; }

        protected BindableControl()
        {
            this.DataPropertyName = string.Empty;
        }

        [DefaultValue(""), Category("Data")]
        public string DataPropertyName { get; set; }

        [DefaultValue(false)]
        public bool IncrementalSearchEnabled { get; set; }

        #endregion

        public virtual object GetValue(TreeNodeAdv node)
        {
            if (this.VirtualMode)
            {
                NodeControlValueEventArgs args = new NodeControlValueEventArgs(node);
                OnValueNeeded(args);
                return args.Value;
            }

            return this.GetMemberAdapter(node).Value;
        }

        public virtual void SetValue(TreeNodeAdv node, object value)
        {
            if (this.VirtualMode)
            {
                NodeControlValueEventArgs args = new NodeControlValueEventArgs(node)
                {
                    Value = value
                };

                OnValuePushed(args);
            }
            else
            {
                MemberAdapter ma = GetMemberAdapter(node);

                ma.Value = value;
            }
        }

        public Type GetPropertyType(TreeNodeAdv node)
        {
            return GetMemberAdapter(node).MemberType;
        }

        private MemberAdapter GetMemberAdapter(TreeNodeAdv node)
        {

            MemberAdapter adapter = MemberAdapter.Empty;

            if (node.Tag != null && !string.IsNullOrEmpty(this.DataPropertyName))
            {
                Type type = node.Tag.GetType();
                PropertyInfo pi = type.GetProperty(this.DataPropertyName);

                if (pi != null)
                {
                    return new MemberAdapter(node.Tag, pi);
                }

                FieldInfo fi = type.GetField(this.DataPropertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                if (fi != null)
                    return new MemberAdapter(node.Tag, fi);
            }

            return adapter;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(this.DataPropertyName))
                return GetType().Name;

            return this.GetType().Name + " (" + this.DataPropertyName + ")";
        }

        public event EventHandler<NodeControlValueEventArgs> ValueNeeded;
        private void OnValueNeeded(NodeControlValueEventArgs args)
        {
            if (this.ValueNeeded != null)
                this.ValueNeeded(this, args);
        }

        public event EventHandler<NodeControlValueEventArgs> ValuePushed;
        private void OnValuePushed(NodeControlValueEventArgs args)
        {
            if (this.ValuePushed != null)
                this.ValuePushed(this, args);
        }
    }
}
