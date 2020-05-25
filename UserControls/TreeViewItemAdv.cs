using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace AssetManager
{
    public class zTreeViewAdv : TreeView
    {
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new zTreeViewItemAdv();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is zTreeViewItemAdv;
        }
    }


    public class zTreeViewItemAdv : TreeViewItem
    {                       
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new zTreeViewItemAdv();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is zTreeViewItemAdv;
        }
    }



}
