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
using EmployeeDirectory.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace EmployeeDirectory.ViewModels
{
	public class PeopleGroup : IEnumerable<Person>
	{
		public string ShortTitle { get; private set; }
		public string Title { get; private set; }

		public List<Person> People { get; private set; }

		public PeopleGroup (string title)
		{
			Title = title;
			ShortTitle = Title.Substring(0, 1);
			People = new List<Person> ();
		}

		public static ObservableCollection<PeopleGroup> CreateGroups (IEnumerable<Person> people, bool groupByLastName)
		{
			var pgs = new Dictionary<string, PeopleGroup> ();

			var sorted = groupByLastName ? people.OrderBy(x => x.Anzeigename) : people.OrderBy(x => x.Index);

			foreach (var p in sorted) {
				try
				{
					var g = groupByLastName && p.IsPerson ? p.Name?.Substring(0,1).ToUpper() : p.SafeDepartment;

                    if (!pgs.TryGetValue(g, out PeopleGroup pg))
                    {
                        pg = new PeopleGroup(g);
                        pgs.Add(g, pg);
                    }

                    pg.People.Add(p);
                }
                catch (NullReferenceException)
                {
                    //remove that person from favs
                }
			}

			return new ObservableCollection<PeopleGroup> (pgs.Values.OrderBy (x => x.Title));
		}

		IEnumerator<Person> IEnumerable<Person>.GetEnumerator ()
		{
			return People.GetEnumerator ();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
		{
			return People.GetEnumerator ();
		}
	}
}

