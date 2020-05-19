using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace EmployeeDirectoryUI.ViewsXaml
{
    public partial class AboutPage : ContentPage
    {
        public AboutPage()
        {
            InitializeComponent();
            if (ToolbarItems.Count == 0)
            {
                var toolbarItem = new ToolbarItem("Hilfe", null, () =>
                {
                    Navigation.PushAsync(new HelpPage());
                }, 0, 0);
                ToolbarItems.Add(toolbarItem);
            }
        }
    }
}
