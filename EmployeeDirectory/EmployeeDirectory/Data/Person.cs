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
using System.Text;
using System.Linq;
using Xamarin.Forms;

namespace EmployeeDirectory.Data
{
	/// <summary>
	/// Person.
	/// </summary>
	/// <remarks>
	/// Derived from <a href="http://fsuid.fsu.edu/admin/lib/WinADLDAPAttributes.html#RANGE!B19">Windows AD LDAP Schema</a>
	/// and <a href="http://www.zytrax.com/books/ldap/ape/core-schema.html">core.schema</a> from OpenLDAP.
	/// </remarks>
	public class Person
	{
		string id;

		public string Id {
			get {
				/*if (!string.IsNullOrEmpty (id)) {
					return id;
				} else if (!string.IsNullOrEmpty (Email)) {
					return Email;
				} else {
					return Name;
				}*/

                if (!string.IsNullOrEmpty(id))
				{
					return id;
                }
				else if (!string.IsNullOrEmpty(Personalnummer))
				{
					return Personalnummer;
				}
				else if(!string.IsNullOrEmpty(Anzeigename))
                {
					return Anzeigename + Bezeichnung + Hauptnummer1 + Hauptnummer2;
                }
                else
                {
					return SafeDisplayName;
                }
			}
			set {
				id = value;
			}
		}

		[Property(Group = "General", Ldap = "cn")]
		public string Anzeigename { get; set; }

		[Property(Group = "General", Ldap = "description")]
		public string Bezeichnung { get; set; }

		[Property(Group = "General", Ldap = "name")]
		public string Name { get; set; }

		[Property(Group = "General", Ldap = "firstName")]
		public string Vorname { get; set; }

		[Property(Group = "General", Ldap = "username")]
		public string Benutzername { get; set; }

		[Property(Group = "General", Ldap = "function")]
		public string Funktion { get; set; }

		[Property(Group = "General", Ldap = "org")]
		public string Organisation { get; set; }

		[Property(Group = "General", Ldap = "title")]
		public string Titel { get; set; }

		[Property(Group = "General", Ldap = "personalnr")]
		public string Personalnummer { get; set; }

		[Property(Group = "General", Ldap = "BezeichnungJob")]
		public string BezeichnungJob { get; set; }

		[Property(Group = "Contact", Ldap = "telephoneNumber")]
		public string Hauptnummer1 { get; set; }

		[Property(Group = "Contact", Ldap = "telephoneNumber")]
		public string Telefon { get; set; }

		[Property(Group = "Contact", Ldap = "mobile")]
		public string Mobil { get; set; }

		[Property(Group = "Contact", Ldap = "sms")]
		public string SMS { get; set; }

		[Property(Group = "Contact", Ldap = "otherTelephone")]
		public string Hauptnummer2 { get; set; }

		[Property(Group = "Contact", Ldap = "otherTelephone")]
		public string Telefon2 { get; set; }

		[Property(Group = "Contact", Ldap = "secretariat")]
		public string TelefonSekretariat { get; set; }

		[Property(Group = "Contact", Ldap = "email")]
		public string Email { get; set; }

		[Property(Group = "Contact", Ldap = "fax")]
		public string Fax { get; set; }

		[Property(Group = "Contact", Ldap = "sucher")]
		public string Sucher { get; set; }

		private string art;
		[Property(Group = "Organization", Ldap = "organizationalUnitName")]
		public string Art {
            get {
				if (!string.IsNullOrEmpty(Personalnummer))
				{
					return SearchProperty.Personen.ToString();
				}
                return art;
            }

            set {
				art = value;
            }
        }

		[Property(Group = "Organization", Ldap = "department")]
		public string KlinikOrt { get; set; }

		[Property(Group = "Organization", Ldap = "specialization")]
		public string Bereich { get; set; }

		[Property(Group = "Organization", Ldap = "details")]
		public string Details { get; set; }

		[Property(Group = "Organization", Ldap = "workplace")]
		public string Arbeitsort { get; set; }

		[Property(Group = "Organization", Ldap = "address")]
		public string Postadresse { get; set; }

		[Property(Group = "Organization", Ldap = "dept")]
		public string Personalbereich { get; set; }

		[Property(Group = "SortIndex")]
		public string Index { get; set; }

		#region Derived Properties
		public string SafeDepartment {
		get {
                if (!string.IsNullOrEmpty(KlinikOrt))
				{
					return KlinikOrt;
				}
				else if (!string.IsNullOrEmpty(Organisation))
				{
					return Personalbereich;
				}
				else
                {
					return "\u202FWichtige Nummern";
                }
		}
		}

		public string SafeDisplayName {
			get {
				if (Anzeigename != null)
				{
					return $"{Anzeigename}";
				}
				else
				{
					return $"{Vorname} {Name}";
				}
			}
		}

		public string SafeDetails
		{
			get
			{
				if (Details != null)
				{
					return $"{Details}";
				}
				else
				{
					return $"{Personalbereich} {Organisation}";
				}
			}
		}

		public bool IsDepartment
		{
			get
			{
				return Anzeigename != null;
			}
		}

		public bool IsPerson
		{
			get
			{
				return Anzeigename == null;
			}
		}

        #endregion

        public Person ()
		{
		}

		public override string ToString ()
		{
			return SafeDisplayName;
		}
	}
}
