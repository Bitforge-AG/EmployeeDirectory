using System;
using Xamarin.Forms;
using EmployeeDirectory.Data;
using EmployeeDirectory.ViewModels;
using EmployeeDirectory.Utilities;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.PlatformConfiguration;

namespace EmployeeDirectoryUI.Xaml
{
    public partial class EmployeeXaml : ContentPage
    {
        private const int IMAGE_SIZE = 150;
        private ToolbarItem toolbarItem;

        public EmployeeXaml()
        {
            InitializeComponent();
            On<iOS>().SetUseSafeArea(true);
            toolbarItem = new ToolbarItem("fav", "star_empty.png", () =>
            {
            }, 0, 0);
            ToolbarItems.Add(toolbarItem);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            var personInfo = (PersonViewModel)BindingContext;
            toolbarItem = new ToolbarItem("fav", personInfo.IsFavorite?"star.png": "star_empty.png", () =>
            {
                personInfo.ToggleFavorite();
                toolbarItem.IconImageSource = personInfo.IsFavorite? "star.png" : "star_empty.png";
            }, 0, 0);
            ToolbarItems.Clear();
            ToolbarItems.Add(toolbarItem);
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            var personInfo = (PersonViewModel)BindingContext;
            Title = personInfo.Person.Anzeigename;
        }

        public void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null) return;
            var property = (PersonViewModel.Property)e.SelectedItem;
            System.Diagnostics.Debug.WriteLine("Property clicked " + property.Type + " " + property.Value);

            switch (property.Type)
            {
                case PersonViewModel.PropertyType.Email:
                    App.PhoneFeatureService.Email(property.Value);
                    break;
                case PersonViewModel.PropertyType.Twitter:
                    App.PhoneFeatureService.Tweet(property.Value);
                    break;
                case PersonViewModel.PropertyType.Url:
                    App.PhoneFeatureService.Browse(property.Value);
                    break;
                case PersonViewModel.PropertyType.Phone:
                    App.PhoneFeatureService.Call(property.Value);
                    break;
                case PersonViewModel.PropertyType.Sms:
                    App.PhoneFeatureService.SendSms(property.Value);
                    break;
                case PersonViewModel.PropertyType.Address:
                    App.PhoneFeatureService.Map(property.Value);
                    break;
            }
            listView.SelectedItem = null;
        }

        private void OnCancelClicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }
    }
}
