using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using Xamarin.Forms;
using System.IO;
using EmployeeDirectory;
using EmployeeDirectoryUI;
using Xamarin.Forms.Platform.iOS;

namespace EmployeeDirectory.iOS
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : FormsApplicationDelegate
	{
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			Forms.SetFlags("IndicatorView_Experimental","CarouselView_Experimental");
			Forms.Init ();

			CopyInfoIntoWorkingFolder("XamarinFavorites.xml", false);
			CopyInfoIntoWorkingFolder ("Departments.xlsx", true);
			CopyInfoIntoWorkingFolder("People.xlsx", true);

			LoadApplication (new App());

			var b = base.FinishedLaunching (app,options);

            App.PhoneFeatureService = new iOSPhoneFeatureService(app.KeyWindow.RootViewController);

			return b;
		}

		private void CopyInfoIntoWorkingFolder (string filename, bool overwrite)
		{
			string documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			string libraryPath = documentsPath.Replace ("Documents", "Library");

			var destination = Path.Combine (libraryPath, filename);

			if (overwrite && File.Exists(destination))
			{
				File.Delete(destination);
			}

			if (!File.Exists (destination)) {
				File.Copy (filename, destination);
			}
		}
	}
}

