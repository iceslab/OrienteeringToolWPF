using System.Windows.Controls;

namespace OrienteeringToolWPF.Utils
{
    public static class TreeViewExtension
    {
        public static bool SetSelectedItem(this TreeView treeView, object item)
        {
            return SetSelected(treeView, item);
        }

        private static bool SetSelected(ItemsControl parent, object child)
        {
            if (parent == null || child == null)
                return false;

            TreeViewItem childNode = parent.ItemContainerGenerator
            .ContainerFromItem(child) as TreeViewItem;

            if (childNode != null)
            {
                var parentCasted = parent as TreeViewItem;
                if (parentCasted != null)
                {
                    parentCasted.IsExpanded = true;
                }
                    
                childNode.Focus();
                return childNode.IsSelected = true;
            }

            if (parent.Items.Count > 0)
            {
                foreach (object childItem in parent.Items)
                {
                    ItemsControl childControl = parent
                      .ItemContainerGenerator
                      .ContainerFromItem(childItem)
                      as ItemsControl;

                    if (SetSelected(childControl, child))
                        return true;
                }
            }

            return false;
        }
    }
}
