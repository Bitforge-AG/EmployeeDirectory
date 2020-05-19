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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using EmployeeDirectory.Data;
using PCLStorage;
using Xamarin.Forms;

namespace EmployeeDirectory
{
    public class MemoryDirectoryService : IDirectoryService
    {
        List<Person> people;

        Dictionary<string, PropertyInfo> properties;

        public event ExtensionCallbackDelegate ExtensionCallback;

        public MemoryDirectoryService(IEnumerable<Person> people)
        {
            this.people = people.ToList();
            this.properties = typeof(Person).GetRuntimeProperties().ToDictionary(p => p.Name);


            Task.Factory.StartNew(async () =>
            {
                try
                {
                    var lookup = new Dictionary<long, string>();

                    foreach (var person in people)
                    {
                        if (!string.IsNullOrEmpty(person.Hauptnummer1))
                        {
                            AddEntry(lookup, GetPhoneNumber(person.Hauptnummer1), person.SafeDisplayName);
                        }
                        if (!string.IsNullOrEmpty(person.Hauptnummer2))
                        {
                            AddEntry(lookup, GetPhoneNumber(person.Hauptnummer2), person.SafeDisplayName + " (Hauptnummer 2)");
                        }
                        if (!string.IsNullOrEmpty(person.TelefonSekretariat))
                        {
                            AddEntry(lookup, GetPhoneNumber(person.TelefonSekretariat), $"{person.SafeDisplayName} (Sekretariat) {person.Personalbereich} {person.Organisation}");
                        }
                        if (!string.IsNullOrEmpty(person.Telefon))
                        {
                            AddEntry(lookup, GetPhoneNumber(person.Telefon), $"{person.SafeDisplayName} {person.Personalbereich} {person.Organisation}" );
                        }
                        if (!string.IsNullOrEmpty(person.Telefon2))
                        {
                            AddEntry(lookup, GetPhoneNumber(person.Telefon2), $"{person.SafeDisplayName} {person.Personalbereich} {person.Organisation} (Telefon 2)");
                        }
                        if (!string.IsNullOrEmpty(person.Mobil))
                        {
                            AddEntry(lookup, GetPhoneNumber(person.Mobil), $"{person.SafeDisplayName} {person.Personalbereich} {person.Organisation} (Mobil)");
                        }
                        if (!string.IsNullOrEmpty(person.SMS))
                        {
                            AddEntry(lookup, GetPhoneNumber(person.SMS), $"{person.SafeDisplayName} {person.Personalbereich} {person.Organisation} (Mobil Direkt)");
                        }
                    }

                    var sorted = lookup.OrderBy(i => i.Key);

                    var result = await DependencyService.Get<ICallDirectoryStore>()?.Store(sorted);
                    ExtensionCallback?.Invoke(result);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            });

        }

        private void AddEntry(Dictionary<long, string> lookup, long number, string displayName)
        {
            if (number == 0) return;

            if (lookup.ContainsKey(number))
            {
                //Console.WriteLine($"Skipping {number} {displayName} - duplicate of {lookup[number]}");
                return;
            }
            lookup.Add(number, displayName);
        }

        private long GetPhoneNumber(string value)
        {
            value = value.Replace(" ", "");

            long.TryParse(value, out var result);

            return result;
        }

        #region IDirectoryService implementation

        public void Dispose()
        {
        }

        public Task LoginAsync(string username, string password, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                //Just for testing
                //HACK: Thread.Sleep (2000);
            });
        }

        public Task<IList<Person>> SearchAsync(SearchProperty filter, string searchString, int sizeLimit, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                var s = Search(filter, searchString);
                var list = s.ToList();
                return (IList<Person>)list;
            }, cancellationToken);
        }

        IEnumerable<Person> Search(SearchProperty filter, string searchString)
        {
            IEnumerable<Person> result;

            if (filter != SearchProperty.Alle)
            {
                var f = Enum.GetName(typeof(SearchProperty),filter);
                var prop = properties["Art"];
                result = from p in people
                        where p.Art.Contains(f)
                        select p;
            }
            else
            {
                result = from p in people
                        select p;
            }
            if (!string.IsNullOrEmpty(searchString))
            {
                var searchLower = searchString.ToLower();

                result = from p in result
                         where p.SafeDisplayName.ToLower().Contains(searchLower) || p.Organisation !=null && p.Organisation.ToLower().Contains(searchLower) || p.Personalbereich != null && p.Personalbereich.ToLower().Contains(searchLower)
                         select p;

            }
            return result;
        }

        #endregion

        #region File IO

        public async static Task<MemoryDirectoryService> FromCsv(string path)
        {
            IFolder store = FileSystem.Current.LocalStorage;
            var file = await store.GetFileAsync(path);

            using (var reader = new StreamReader(await file.OpenAsync(PCLStorage.FileAccess.Read)))
            {
                return FromCsv(reader);
            }
        }

        public static MemoryDirectoryService FromCsv(TextReader textReader)
        {
            var reader = new CsvReader<Person>(textReader);
            return new MemoryDirectoryService(reader.ReadAll());
        }

        public async static Task<MemoryDirectoryService> FromXls(string[] paths)
        {
            IFolder store = FileSystem.Current.LocalStorage;

            List<Person> people = new List<Person>();

            foreach(var path in paths)
            {

                var file = await store.GetFileAsync(path);

                using (var stream = File.Open(file.Path, FileMode.Open, System.IO.FileAccess.Read))
                {
                    var reader = new XlsReader<Person>(stream);
                    people.AddRange(reader.ReadAll());
                }
            }
            return new MemoryDirectoryService(people);
        }

        #endregion
    }
}

