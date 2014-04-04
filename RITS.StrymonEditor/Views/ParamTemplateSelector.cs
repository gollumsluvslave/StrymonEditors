using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

using RITS.StrymonEditor.ViewModels;
namespace RITS.StrymonEditor.Views
{
    public class ParameterTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var vm = item as ParameterViewModel;
            var parameter = vm._parameter;
            FrameworkElement element = container as FrameworkElement;
            if (parameter.Definition.OptionList != null && parameter.Definition.OptionList.Count != 0)
            {
                return element.FindResource("OptionList") as DataTemplate;
            }
            else if (parameter.Definition.Range != null)
            {
                return element.FindResource("Range") as DataTemplate;
            }
            else 
            { 
                return element.FindResource("OnOff") as DataTemplate;
                // Todo maybe for special option List??
            }
        }
    }
}
