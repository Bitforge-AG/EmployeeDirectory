using System;
using System.Windows.Input;
using EmployeeDirectory;
using EmployeeDirectoryUI.Xaml;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.PlatformConfiguration;

namespace EmployeeDirectoryUI.ViewsXaml
{
    public partial class MainMenu : ContentPage
    {
        public MainMenu()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            On<iOS>().SetUseSafeArea(true);

            BindingContext = this;
            if (ToolbarItems.Count == 0)
            {
                var toolbarItem = new ToolbarItem("Info", null, () =>
                {
                    Navigation.PushAsync(new AboutPage());
                }, 0, 0);
                ToolbarItems.Add(toolbarItem);
            }
        }

        public ICommand OnPersonenClicked
        {
            get
            {
                return new Command(() =>
                {
                    var search = new SearchListXaml(SearchProperty.Personen, "Personen");
                    Navigation.PushAsync(search);
                });
            }
        }

        public ICommand OnDienstnummerClicked
        {
            get
            {
                return new Command(() =>
                {
                    var search = new SearchListXaml(SearchProperty.Dienstnummer, "Dienstnummer");
                    Navigation.PushAsync(search);
                });
            }
        }

        public ICommand OnStationszimmerClicked
        {
            get
            {
                return new Command(() =>
                {
                    var search = new SearchListXaml(SearchProperty.Stationszimmer, "Stationszimmer");
                    Navigation.PushAsync(search);
                });
            }
        }

        public ICommand OnDispoClicked
        {
            get
            {
                return new Command(() =>
                {
                    var search = new SearchListXaml(SearchProperty.Diposition, "Stationäre Disposition");
                    Navigation.PushAsync(search);
                });
            }
        }

        public ICommand OnWichtigClicked
        {
            get
            {
                return new Command(() =>
                {
                    var search = new SearchListXaml(SearchProperty.Wichtig, "Wichtige Rufnummern");
                    Navigation.PushAsync(search);
                });
            }
        }

        public ICommand OnAlleClicked
        {
            get
            {
                return new Command(() =>
                {
                    var search = new SearchListXaml(SearchProperty.Alle, "Alle Einträge");
                    Navigation.PushAsync(search);
                });
            }
        }

        public ICommand OnFavoritenClicked
        {
            get
            {
                return new Command(() =>
                {
                    var employeeList = new EmployeeListXaml();
                    Navigation.PushAsync(employeeList);
                });
            }
        }
    }
}
