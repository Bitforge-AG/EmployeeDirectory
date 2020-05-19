using System;
using EmployeeDirectory;
using Xamarin.Forms;
using EmployeeDirectoryUI.Xaml;
using System.Threading.Tasks;
using EmployeeDirectoryUI.ViewsXaml;

namespace EmployeeDirectoryUI
{
	public enum UIImplementation {
		CSharp = 0,
		Xaml
	}

	public class App : Application
	{
		//Change the following line to switch between XAML and C# versions

		public static IDirectoryService Service { get; set; }

		public static IPhoneFeatureService PhoneFeatureService { get; set; }

		public static DateTime LastUseTime { get; set; }

        private MainMenu _mainMenu;

		public App ()
		{
			System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

			_mainMenu = new MainMenu ();
			MainPage = new NavigationPage (_mainMenu);

			var task = Task.Run(async () => {
				Service = await MemoryDirectoryService.FromXls(new string[] { "Departments.xlsx", "People.xlsx" });
				Service.ExtensionCallback += ExtensionCallback;
			});
			task.Wait();
		}

        private void ExtensionCallback(RegistrationResponse response)
        {

			Device.BeginInvokeOnMainThread(async () =>
			{
				switch (response.Response)
				{
					case RegistrationResponse.RegistrationResponseEnum.NotActivated:
						

						var result = await _mainMenu.DisplayAlert("Anruferkennung", "Bitte Telefonbuch-Erweiterung aktivieren (Anrufe blockieren u. identifizieren)", "Hilfe", "Ignorieren");
                        if(result)
							_mainMenu.Navigation.PushAsync(new HelpPage());
						break;
					case RegistrationResponse.RegistrationResponseEnum.Error:
						await _mainMenu.DisplayAlert("Anruferkennung", $"Telefonbuch-Erweiterung konnte nicht aktiviert werden (Fehler {response.Reason})", "Ok");
						break;
					case RegistrationResponse.RegistrationResponseEnum.Ok:
						//await _mainMenu.DisplayAlert("Anruferkennung", "Telefonbuch-Erweiterung wurde aktiviert", "Ok");
						break;
				}
			});
		}
    }
}

