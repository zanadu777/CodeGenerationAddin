﻿using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;

namespace Electric.Navigation.ToolWindows
{
  /// <summary>
  /// Interaction logic for SolutionHistoryToolWindowControl.
  /// </summary>
  public partial class SolutionHistoryToolWindowControl : UserControl
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="SolutionHistoryToolWindowControl"/> class.
    /// </summary>
    public SolutionHistoryToolWindowControl()
    {
      this.InitializeComponent();
    }

    /// <summary>
    /// Handles click on the button by displaying a message box.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event args.</param>
    [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
    private void button1_Click(object sender, RoutedEventArgs e)
    {
      MessageBox.Show(
          string.Format(System.Globalization.CultureInfo.CurrentUICulture, "Invoked '{0}'", this.ToString()),
          "SolutionHistoryToolWindow");
    }
  }
}