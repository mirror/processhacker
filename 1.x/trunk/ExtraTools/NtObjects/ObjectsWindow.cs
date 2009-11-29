using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;
using ProcessHacker.Native.Security.AccessControl;

namespace NtObjects
{
    public partial class ObjectsWindow : Form
    {
        private class TreeViewSorter : System.Collections.IComparer
        {
            public int Compare(object x, object y)
            {
                return ((TreeNode)x).Text.CompareTo(((TreeNode)y).Text);
            }
        }

        public ObjectsWindow()
        {
            InitializeComponent();

            listObjects.ContextMenu = menuObject;

            try
            {
                KProcessHacker.Instance = new KProcessHacker();
            }
            catch
            { }

            try
            {
                using (var thandle = ProcessHandle.GetCurrent().GetToken(TokenAccess.AdjustPrivileges))
                {
                    try { thandle.SetPrivilege("SeCreateGlobalPrivilege", SePrivilegeAttributes.Enabled); }
                    catch { }
                }
            }
            catch
            { }

            treeDirectories.TreeViewNodeSorter = new TreeViewSorter();
            Win32.SetWindowTheme(treeDirectories.Handle, "explorer", null);
            treeDirectories.Nodes.Add("\\", "\\");
            treeDirectories.SelectedNode = treeDirectories.Nodes["\\"];
            this.PopulateDirectories();
            this.ChangeDirectory();
            treeDirectories.SelectedNode.Expand();
        }

        private void treeDirectories_MouseDown(object sender, MouseEventArgs e)
        {
            treeDirectories.SelectedNode = treeDirectories.GetNodeAt(e.Location);
        }

        private void PopulateDirectories()
        {
            this.PopulateDirectory("\\");
        }

        private void PopulateDirectory(string directory)
        {
            try
            {
                using (DirectoryHandle dhandle =
                    new DirectoryHandle(directory, DirectoryAccess.Query))
                {
                    var objects = dhandle.GetObjects();

                    foreach (var obj in objects)
                    {
                        if (obj.TypeName != "Directory")
                            continue;

                        this.GetTreeNode(directory).Nodes.Add(obj.Name, obj.Name);

                        this.PopulateDirectory(this.NormalizePath(directory + "\\" + obj.Name));
                    }
                }
            }
            catch (WindowsException)
            { }
        }

        private string NormalizePath(string path)
        {
            string[] s = path.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);

            return "\\" + string.Join("\\", s);
        }

        private TreeNode GetTreeNode(string path)
        {
            return this.GetTreeNode(treeDirectories.Nodes["\\"], path, 0);
        }

        private TreeNode GetTreeNode(TreeNode root, string path, int index)
        {
            string[] s = path.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);

            if (index >= s.Length)
                return root;

            if (root.Nodes[s[index]] == null)
                return null;

            return this.GetTreeNode(root.Nodes[s[index]], path, index + 1);
        }

        private void ChangeDirectory()
        {
            listObjects.Items.Clear();

            if (treeDirectories.SelectedNode != null)
            {
                listObjects.BeginUpdate();

                try
                {
                    using (DirectoryHandle dhandle =
                        new DirectoryHandle(this.NormalizePath(treeDirectories.SelectedNode.FullPath), DirectoryAccess.Query))
                    {
                        var objects = dhandle.GetObjects();

                        foreach (var obj in objects)
                        {
                            var item = listObjects.Items.Add(new ListViewItem(new string[] { obj.Name, obj.TypeName, "" }));

                            if (imageList.Images.ContainsKey(obj.TypeName.ToLower()))
                                item.ImageKey = obj.TypeName.ToLower();
                            else
                                item.ImageKey = "object";

                            if (obj.TypeName == "SymbolicLink")
                            {
                                try
                                {
                                    using (SymbolicLinkHandle shandle =
                                        new SymbolicLinkHandle(
                                            this.NormalizePath(
                                            treeDirectories.SelectedNode.FullPath +
                                            "\\" + obj.Name),
                                            SymbolicLinkAccess.Query))
                                        item.SubItems[2].Text = shandle.GetTarget();
                                }
                                catch
                                { }
                            }
                        }
                    }
                }
                catch (WindowsException)
                { }

                listObjects.EndUpdate();
            }
        }

        private void treeDirectories_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            this.ChangeDirectory();
        }

        private void listObjects_DoubleClick(object sender, EventArgs e)
        {
            if (listObjects.SelectedItems.Count != 1)
                return;

            if (listObjects.SelectedItems[0].SubItems[1].Text == "Directory")
            {
                treeDirectories.SelectedNode =
                    treeDirectories.SelectedNode.Nodes[listObjects.SelectedItems[0].SubItems[0].Text];
                this.ChangeDirectory();
            }
        }

        private void permissionsMenuItem_Click(object sender, EventArgs e)
        {
            if (listObjects.SelectedItems.Count != 1)
                return;

            string objectName = this.NormalizePath(
                treeDirectories.SelectedNode.FullPath + "\\" + listObjects.SelectedItems[0].Text
                );

            SecurityEditor.EditSecurity(
                this,
                SecurityEditor.GetSecurableWrapper((access) => NativeUtils.OpenObject((int)access, objectName, 0, null)),
                objectName,
                NativeTypeFactory.GetAccessEntries(NativeTypeFactory.GetObjectType(
                listObjects.SelectedItems[0].SubItems[1].Text
                ))
                );
        }
    }
}
