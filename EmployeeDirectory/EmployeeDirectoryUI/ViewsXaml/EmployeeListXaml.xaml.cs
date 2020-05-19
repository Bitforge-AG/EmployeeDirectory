using System;
using Xamarin.Forms;
using System.Linq;
using EmployeeDirectory.Data;
using EmployeeDirectory.ViewModels;
using EmployeeDirectory;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.PlatformConfiguration;

namespace EmployeeDirectoryUI.Xaml
{
    public partial class EmployeeListXaml : ContentPage
    {
        private FavoritesViewModel viewModel;
        private IFavoritesRepository favoritesRepository;
        //private ToolbarItem toolbarItem;

        public EmployeeListXaml()
        {
            InitializeComponent();
            On<iOS>().SetUseSafeArea(true);

            /*toolbarItem = new ToolbarItem("search", "Search.png", () =>
            {
                var search = new SearchListXaml(SearchProperty.Alle);
                Navigation.PushAsync(search);
            }, 0, 0);

            ToolbarItems.Add(toolbarItem);*/
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            if (LoginViewModel.ShouldShowLogin(App.LastUseTime))
                await Navigation.PushModalAsync(new LoginXaml());

            favoritesRepository = await XmlFavoritesRepository.OpenIsolatedStorage("XamarinFavorites.xml");

            if (favoritesRepository.GetAll().Count() == 0)
                favoritesRepository = await XmlFavoritesRepository.OpenFile("XamarinFavorites.xml");

            viewModel = new FavoritesViewModel(favoritesRepository, true);

            listView.ItemsSource = viewModel.Groups;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        public void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null) return;
            var person = e.SelectedItem as Person;
            if (person == null) return;
            var employeeView = new EmployeeXaml
            {
                BindingContext = new PersonViewModel(person, favoritesRepository)
            };
            listView.SelectedItem = null;

            Navigation.PushAsync(employeeView);
        }
    }
}
