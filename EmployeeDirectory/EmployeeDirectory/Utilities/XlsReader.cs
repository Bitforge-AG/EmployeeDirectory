using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using ExcelDataReader;

namespace EmployeeDirectory
{
    public class XlsReader<T> where T : new()
	{
		private IFormatProvider formatProvider;
		DataSet dataSet;

		public XlsReader(System.IO.FileStream stream)
		{
			this.formatProvider = CultureInfo.InvariantCulture;

			// Auto-detect format, supports:
			//  - Binary Excel files (2.0-2003 format; *.xls)
			//  - OpenXml Excel files (2007 format; *.xlsx)
			using (var reader = ExcelReaderFactory.CreateReader(stream))
			{
				// Choose one of either 1 or 2:

				// 1. Use the reader methods
				do
				{
					while (reader.Read())
					{
						// reader.GetDouble(0);
					}
				} while (reader.NextResult());

				// 2. Use the AsDataSet extension method
				var result = reader.AsDataSet(new ExcelDataSetConfiguration()
				{
					ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
					{
						UseHeaderRow = true
					}
				});

				// The result of each spreadsheet is in result.Tables
				//Debug.WriteLine($"{result} {result.Tables[0].Rows[0]}");
				dataSet = result;

			}
		}

		public IEnumerable<T> ReadAll()
		{
			var table = dataSet.Tables[0];

			var columns = table.Columns;


			var headerNames = new string[columns.Count];
			var headerNameProperties = new string[columns.Count];

			for (int index=0; index < headerNames.Length; index++){
				var column = dataSet.Tables[0].Columns[index];
				var headerName = column.ColumnName;
				headerNames[index] = headerName;
				var headerNameProperty = headerName;
                
				headerNameProperty = headerNameProperty.Replace(" ", "");
				headerNameProperty = headerNameProperty.Replace("/", "");
				headerNameProperty = headerNameProperty.Replace(".", "");
				headerNameProperty = headerNameProperty.Replace("-", "");
				headerNameProperty = headerNameProperty.Replace("\t", "");

				headerNameProperties[index] = headerNameProperty;
			}
			var props = new PropertyInfo[headerNames.Length];

			for (var hi = 0; hi < props.Length; hi++)
			{
				var p = typeof(T).GetRuntimeProperty(headerNameProperties[hi]);
				if (p == null)
                {
					Debug.WriteLine("Unknown property: " + headerNameProperties[hi]);
                }
				props[hi] = p;
			}

			// Read all the records
			var result = new List<T>();

            for(int rowIndex = 0; rowIndex < table.Rows.Count; rowIndex++)
            {
				var row = table.Rows[rowIndex];

				var r = new T();
				result.Add(r);

                for(int propIndex = 0; propIndex < props.Length; propIndex++)
                {
					var prop = props[propIndex];
					var headerName = headerNames[propIndex];

					if (prop == null) continue; //skip unknown properties

                    var entry = row[headerName].ToString();
                    if(entry != null)
					{
						prop.SetValue(r, Convert.ChangeType(entry, prop.PropertyType, formatProvider), null);
					}
                }
			}

			return result;

			/*var i = 0;
            
			var ch = reader.Read();
			while (ch > 0)
			{
				if (ch == '\n')
				{
					yield return r;
					r = new T();
					i = 0;
					ch = reader.Read();
				}
				else if (ch == '\r')
				{
					ch = reader.Read();
				}
				else if (ch == '"')
				{
					ch = ReadQuoted(r, props[i]);
				}
				else if (ch == ',')
				{
					i++;
					ch = reader.Read();
				}
				else
				{
					ch = ReadNonQuoted(r, props[i], (char)ch);
				}
			}*/
		}
	}
}
