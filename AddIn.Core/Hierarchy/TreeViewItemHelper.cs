using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AddIn.Core.Hierarchy
{
  public static class TreeViewItemHelper
  {
    public static readonly DependencyProperty IsLeafProperty =
      DependencyProperty.RegisterAttached("IsLeaf", typeof(bool), typeof(TreeViewItemHelper), new UIPropertyMetadata(false));

    public static bool GetIsLeaf(DependencyObject obj)
    {
      return (bool)obj.GetValue(IsLeafProperty);
    }

    public static void SetIsLeaf(DependencyObject obj, bool value)
    {
      obj.SetValue(IsLeafProperty, value);
    }
  }
}
