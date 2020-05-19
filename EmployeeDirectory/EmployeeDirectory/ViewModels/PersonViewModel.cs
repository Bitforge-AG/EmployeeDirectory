//
//  Copyright 2012, Xamarin Inc.
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using EmployeeDirectory.Data;

namespace EmployeeDirectory.ViewModels
{
	public class PersonViewModel : ViewModelBase
	{
		private IFavoritesRepository FavoritesRepository { get; set; }

		public PersonViewModel (Person person, IFavoritesRepository favoritesRepository)
		{
			if (person == null)
				throw new ArgumentNullException ("person");

			if (favoritesRepository == null)
				throw new ArgumentNullException ("favoritesRepository");

			Person = person;
			FavoritesRepository = favoritesRepository;

			PropertyGroups = new ObservableCollection<PropertyGroup> ();

			var general = new PropertyGroup ("Allgemein");
			general.Add ("Anzeigename", person.Anzeigename, PropertyType.Generic);
			general.Add ("Klinik / Ort", person.KlinikOrt, PropertyType.Generic);
			general.Add ("Bereich", person.Bereich, PropertyType.Generic);
			general.Add ("Bezeichnung", person.Bezeichnung, PropertyType.Generic);
			general.Add("Name", person.Name, PropertyType.Generic);
			general.Add("Vorname", person.Vorname, PropertyType.Generic);
			general.Add("Funktion", person.Funktion, PropertyType.Generic);
			general.Add("Organisation", person.Organisation, PropertyType.Generic);
			general.Add("Personalbereich", person.Personalbereich, PropertyType.Generic);
			general.Add("Arbeitsort", person.Arbeitsort, PropertyType.Generic);
			general.Add("Benutzername", person.Benutzername, PropertyType.Generic);
			general.Add("Titel", person.Titel, PropertyType.Generic);

			if (general.Properties.Count > 0)
				PropertyGroups.Add (general);

			var phone = new PropertyGroup ("Telefon");
			phone.Add("Hauptnummer", person.Hauptnummer1, PropertyType.Phone);
			phone.Add("Telefon", person.Telefon, PropertyType.Phone);
			phone.Add("Mobil", person.Mobil, PropertyType.Phone);
			phone.Add("Mobil (Direktwahl)", person.SMS, PropertyType.Phone);
			phone.Add("SMS", person.SMS, PropertyType.Sms);
			phone.Add("Alternativnummer", person.Telefon2, PropertyType.Phone);
			phone.Add("Alternativnummer", person.Hauptnummer2, PropertyType.Phone);
			phone.Add("Sucher", person.Sucher, PropertyType.Phone);
			phone.Add("Sekretariat", person.TelefonSekretariat, PropertyType.Phone);
			phone.Add("Fax", person.Fax, PropertyType.Phone);
			phone.Add("E-Mail", person.Email, PropertyType.Email);

			if (phone.Properties.Count > 0)
				PropertyGroups.Add (phone);

			var details = new PropertyGroup ("Details");
			details.Add ("Details", person.Details, PropertyType.Generic);
			details.Add("Arbeitsort", person.Arbeitsort, PropertyType.Generic);
			details.Add("Postadresse", person.Postadresse, PropertyType.Generic);
			if (details.Properties.Count > 0)
				PropertyGroups.Add(details);
		}

		private static string CleanUrl (string url)
		{
			var trimmed = (url ?? "").Trim ();
			if (trimmed.Length == 0) return "";

			var upper = trimmed.ToUpperInvariant ();
			if (!upper.StartsWith ("HTTP")) {
				return "http://" + trimmed;
			} else {
				return trimmed;
			}
		}

		private static string CleanTwitter (string username)
		{
			var trimmed = (username ?? "").Trim ();
			if (trimmed.Length == 0) return "";

			if (!trimmed.StartsWith ("@")) {
				return "@" + trimmed;
			} else {
				return trimmed;
			}
		}

		#region View Data

		public Person Person { get; private set; }

		public ObservableCollection<PropertyGroup> PropertyGroups { get; private set; }

		public bool IsFavorite {
			get { return FavoritesRepository.IsFavorite (Person); }
		}
		public bool IsNotFavorite {
			get { return !FavoritesRepository.IsFavorite (Person); }
		}

		public class PropertyGroup : IEnumerable<Property>
		{
			public string Title { get; private set; }
			public ObservableCollection<Property> Properties { get; private set; }

			public PropertyGroup (string title)
			{
				Title = title;
				Properties = new ObservableCollection<Property> ();
			}

			public void Add (string name, string value, PropertyType type)
			{
				if (!string.IsNullOrWhiteSpace (value))
					Properties.Add (new Property (name, value, type));
			}

			IEnumerator<Property> IEnumerable<Property>.GetEnumerator ()
			{
				return Properties.GetEnumerator ();
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
			{
				return Properties.GetEnumerator ();
			}
		}

		public class Property
		{
			public string Name { get; private set; }
			public string Value { get; private set; }
			public PropertyType Type { get; private set; }

			public Property (string name, string value, PropertyType type)
			{
				Name = name;
				Value = value.Trim ();
				Type = type;
			}

			public override string ToString ()
			{
				return string.Format ("{0} = {1}", Name, Value);
			}
		}

		public enum PropertyType
		{
			Generic,
			Phone,
			Sms,
			Email,
			Url,
			Twitter,
			Address,
		}

		#endregion

		#region Commands

		public void ToggleFavorite ()
		{			
			if (FavoritesRepository.IsFavorite (Person)) {
				FavoritesRepository.Delete (Person);
			} else {
				FavoritesRepository.InsertOrUpdate (Person);
			}
			OnPropertyChanged ("IsFavorite");
		}

		#endregion
	}
}

