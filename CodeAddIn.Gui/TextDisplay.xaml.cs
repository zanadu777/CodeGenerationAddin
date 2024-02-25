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
using System.Windows.Shapes;

namespace CodeAddIn.Gui
{
  /// <summary>
  /// Interaction logic for TextDisplay.xaml
  /// </summary>
  public partial class TextDisplay : Window
  {
    public TextDisplay()
    {
      InitializeComponent();
    }

    public string Text
    {
      get { return txtDisplay.Text; }
      set { txtDisplay.Text = value; }
    }
  }
}
