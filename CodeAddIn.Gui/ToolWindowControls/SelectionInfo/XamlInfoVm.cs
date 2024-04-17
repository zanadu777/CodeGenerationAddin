using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using AddIn.Core.VisualStudio;

namespace CodeAddIn.Gui.ToolWindowControls.SelectionInfo
{
  public class XamlInfoVm : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;


    private VsItem vsItem;
    private string name;

    public XamlInfoVm()
    {
      
    }
    public XamlInfoVm(VsItem item)
    {
      vsItem = item;
      Name = item.Name;
      LoadXaml(item.Text);
    }

    public string Name
    {
      get => name;
      set => SetField(ref name, value);
    } 
    public NodeViewModel Root { get; set; }
   

    public void LoadXaml(string xamlContent)
    {
      XDocument xdoc = XDocument.Parse(xamlContent);
      Root = new NodeViewModel { Name = xdoc.Root.Name.LocalName };
      AddElementsToTreeView(xdoc.Root, Root);
    }

    private void AddElementsToTreeView(XElement xElement, NodeViewModel nodeViewModel)
    {
      // Add a node for each attribute
      foreach (XAttribute attribute in xElement.Attributes())
      {
        NodeViewModel attributeNode = new NodeViewModel { Name = $"{attribute.Name.LocalName}: {attribute.Value}" };
        nodeViewModel.Children.Add(attributeNode);
      }

      // Add a node for each child element
      foreach (XElement childElement in xElement.Elements())
      {
        NodeViewModel childNode = new NodeViewModel { Name = childElement.Name.LocalName };
        nodeViewModel.Children.Add(childNode);
        AddElementsToTreeView(childElement, childNode);
      }
    }


    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
      if (EqualityComparer<T>.Default.Equals(field, value)) return false;
      field = value;
      OnPropertyChanged(propertyName);
      return true;
    }
  }
}