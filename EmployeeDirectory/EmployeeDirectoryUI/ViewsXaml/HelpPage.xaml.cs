using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace EmployeeDirectoryUI.ViewsXaml
{
    public class Item
    {
        public string ImageUrl { get; set; }
        public string Infotext { get; set; }
    }


    public partial class HelpPage : ContentPage
    {
        public List<Item> Items { get; set; }

        public HelpPage()
        {
            InitializeComponent();

            BindingContext = this;

            Items = new List<Item>()
            {
                new Item{ ImageUrl = "Help_Einstellungen.jpg" , Infotext = "Aktivieren der Anrufererkennung Schritt 1: \"Einstellungen\" öffnen, \"Telefon\" wöhlen"},
                new Item{ ImageUrl = "Help_Telefon.jpg" , Infotext = "Schritt 2: \"Anrufe\u202Fblockieren u.\u202Fidentifizieren\" wählen"},
                new Item{ ImageUrl = "Help_Identifizieren.jpg" , Infotext = "Schritt 3: \"Telefonbuch\" aktivieren"},
            };
            Carousel.ItemsSource = Items;
            //Carousel.SetBinding(ItemsView.ItemsSourceProperty, "Items");
        }

        void Settings_Button_Clicked(System.Object sender, System.EventArgs e)
        {
            Device.OpenUri(new Uri("App-Prefs:root=Phone")); // not allowed in store apps! https://stackoverflow.com/questions/23824054/how-to-open-settings-programmatically-like-in-facebook-app
        }
    }
}
