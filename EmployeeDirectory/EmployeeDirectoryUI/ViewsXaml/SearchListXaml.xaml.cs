using Xamarin.Forms;
using EmployeeDirectory.Data;
using EmployeeDirectory.ViewModels;
using System.Threading.Tasks;
using EmployeeDirectory;
using System;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.PlatformConfiguration;

namespace EmployeeDirectoryUI.Xaml
{
	public partial class SearchListXaml : ContentPage
	{
		private Search search;
		private SearchViewModel viewModel;
		private IFavoritesRepository favoritesRepository;

		public SearchListXaml (SearchProperty filter, string title)
		{
			InitializeComponent ();
			On<iOS>().SetUseSafeArea(true);
			Title = title;

			var task = Task.Run(async () => {
				favoritesRepository = await XmlFavoritesRepository.OpenFile ("XamarinFavorites.xml");
			});
			task.Wait();

			search = new Search ("test", filter);

			/*if( filter != SearchProperty.Personen && filter != SearchProperty.Alle)
            {
				listView.GroupShortNameBinding = null;
			}*/

			viewModel = new SearchViewModel (App.Service, search);

			viewModel.SearchCompleted += (sender, e) => {
				if (viewModel.Groups == null) {
					listView.ItemsSource = new string [1];
				} else {
					listView.ItemsSource = viewModel.Groups;
				}
			};

			viewModel.Error += (sender, e) => {
				DisplayAlert ("Error", e.Exception.Message, "OK", null);
			};

			BindingContext = viewModel;
		}

		private void OnValueChanged (object sender, EventArgs e)
		{
			viewModel.Search ();
		}

		private void OnItemSelected (object sender, SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItem == null) return;
			var personInfo = e.SelectedItem as Person;
			if (personInfo == null) return;
			var employeeView = new EmployeeXaml {
				BindingContext = new PersonViewModel (personInfo, favoritesRepository)
			};

			listView.SelectedItem = null;

			Navigation.PushAsync (employeeView);
		}
	}
}