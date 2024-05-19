using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace AddIn.Core.Extensions
{
  public static class DependencyObjectExtensions
  {
    public static T GetParentOfType<T>(this DependencyObject element) where T : DependencyObject
    {
      Type type = typeof(T);
      if (element == null) return null;
      DependencyObject parent = VisualTreeHelper.GetParent(element);
      if (parent == null) return null;
      else if (parent.GetType() == type || parent.GetType().IsSubclassOf(type)) return parent as T;
      return GetParentOfType<T>(parent);
    }

  }
}
