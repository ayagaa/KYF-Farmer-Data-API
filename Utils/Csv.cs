using CsvHelper;
using System.Data;
using System.Text;

namespace Farmer.Data.API.Utils
{
    public static class Csv
    {
        public static List<RootObject> ReadRootObjectFile<RootObject>(string path)
        {
            try
            {
                var cultureInfo = System.Globalization.CultureInfo.InvariantCulture;
                CsvHelper.Configuration.CsvConfiguration config = new CsvHelper.Configuration.CsvConfiguration(cultureInfo);
                config.MissingFieldFound = null;
                config.IgnoreBlankLines = true;
                //config.MemberTypes = CsvHelper.Configuration.MemberTypes.Fields;
                using (TextReader reader = File.OpenText(path))
                using (var csv = new CsvReader(reader, config))
                {
                    Console.WriteLine("Reading File: {0}", path);
                    csv.Read();
                    csv.ReadHeader();
                    return csv.GetRecords<RootObject>().ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return new List<RootObject>();
        }

        public static List<RootObject> ReadRootObjectFile<RootObject>(string path, RootObject rootObject)
        {
            var data = new List<RootObject>();
            try
            {
                var cultureInfo = System.Globalization.CultureInfo.InvariantCulture;
                CsvHelper.Configuration.CsvConfiguration config = new CsvHelper.Configuration.CsvConfiguration(cultureInfo);
                config.MissingFieldFound = null;
                config.IgnoreBlankLines = true;



                var props = typeof(RootObject).GetProperties().Where(x => x.CanRead).ToList();
                using (TextReader reader = File.OpenText(path))
                using (var csv = new CsvReader(reader, cultureInfo))
                {
                    csv.Context.Configuration.BadDataFound = null;
                    csv.Context.Configuration.MissingFieldFound = null;
                    csv.Context.Configuration.IgnoreBlankLines = true;
                    csv.Read();
                    csv.ReadHeader();
                    csv.Context.Configuration.BadDataFound = null;
                    csv.Context.Configuration.MissingFieldFound = null;
                    csv.Context.Configuration.IgnoreBlankLines = true;
                    while (csv.Read())
                    {
                        try
                        {
                            csv.Context.Configuration.BadDataFound = null;
                            csv.Context.Configuration.MissingFieldFound = null;
                            csv.Context.Configuration.IgnoreBlankLines = true;
                            var newRow = Activator.CreateInstance<RootObject>();
                            foreach (var prop in props)
                            {
                                newRow?.GetType()?.GetProperty(prop.Name)?.SetValue(newRow, csv.Context.Reader.GetField(prop.Name));
                            }
                            data.Add(newRow);
                        }
                        catch (CsvHelper.CsvHelperException ex)
                        {
                            //Console.WriteLine(ex.Message);
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return data;
        }

        public static List<RootObject> ReadRootObjectFiles<RootObject>(string directory)
        {
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directory);
                if (directoryInfo.Exists)
                {
                    var resultRootObjects = new List<RootObject>();

                    var cultureInfo = System.Globalization.CultureInfo.InvariantCulture;
                    CsvHelper.Configuration.CsvConfiguration config = new CsvHelper.Configuration.CsvConfiguration(cultureInfo);
                    config.MemberTypes = CsvHelper.Configuration.MemberTypes.Fields;
                    config.Delimiter = ",";
                    config.MissingFieldFound = null;
                    config.IgnoreBlankLines = true;
                    FileInfo[] files = directoryInfo.GetFiles("*.csv");
                    var props = typeof(RootObject).GetProperties().Where(x => x.CanRead).ToList();
                    foreach (var file in files)
                    {
                        Console.WriteLine("Reading data from: {0}", file.Name);
                        using (TextReader reader = file.OpenText())
                        using (var csv = new CsvReader(reader, config))
                        {
                            csv.Read();
                            csv.ReadHeader();
                            //var csvData = csv.GetRecords<RootObject>().ToList();
                            //resultRootObjects.AddRange(csvData);
                            csv.Context.Configuration.BadDataFound = null;
                            csv.Context.Configuration.MissingFieldFound = null;
                            csv.Context.Configuration.IgnoreBlankLines = true;
                            while (csv.Read())
                            {
                                try
                                {
                                    csv.Context.Configuration.BadDataFound = null;
                                    csv.Context.Configuration.MissingFieldFound = null;
                                    csv.Context.Configuration.IgnoreBlankLines = true;
                                    var newRow = Activator.CreateInstance<RootObject>();
                                    foreach (var prop in props)
                                    {
                                        newRow?.GetType()?.GetProperty(prop.Name)?.SetValue(newRow, csv.Context.Reader.GetField(prop.Name));
                                    }
                                    resultRootObjects.Add(newRow);
                                }
                                catch (Exception ex)
                                {
                                }
                            }
                        }
                    }
                    return resultRootObjects;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return new List<RootObject>();
        }

        public static List<RootObject> ReadRootObjectFiles<RootObject>(string inputdirectory, string outputdirectory)
        {
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(outputdirectory);
                RegenerateFiles(inputdirectory, outputdirectory);
                if (directoryInfo.Exists)
                {
                    var resultRootObjects = new List<RootObject>();

                    var cultureInfo = System.Globalization.CultureInfo.InvariantCulture;
                    CsvHelper.Configuration.CsvConfiguration config = new CsvHelper.Configuration.CsvConfiguration(cultureInfo);
                    //config.MemberTypes = CsvHelper.Configuration.MemberTypes.Fields;
                    //config.Delimiter = ",";
                    config.MissingFieldFound = null;
                    //config.IgnoreBlankLines = true;
                    FileInfo[] files = directoryInfo.GetFiles("*.csv");

                    foreach (var file in files)
                    {
                        Console.WriteLine("Reading data from: {0}", file.Name);
                        using (TextReader reader = file.OpenText())
                        using (var csv = new CsvReader(reader, config))
                        {
                            csv.Read();
                            csv.ReadHeader();
                            var csvData = csv.GetRecords<RootObject>().ToList();
                            resultRootObjects.AddRange(csvData);
                        }
                    }
                    return resultRootObjects;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return new List<RootObject>();
        }

        public static bool WriteRootObjectFile<RootObject>(string filePath, List<RootObject> rootObjects)
        {
            try
            {
                if (rootObjects.Count > 0)
                {
                    var cultureInfo = System.Globalization.CultureInfo.InvariantCulture;
                    CsvHelper.Configuration.CsvConfiguration config = new CsvHelper.Configuration.CsvConfiguration(cultureInfo);
                    config.MissingFieldFound = null;
                    using TextWriter writer = File.CreateText(filePath);
                    using var csv = new CsvWriter(writer, config);
                    csv.WriteRecords(rootObjects);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        public static void RegenerateFiles(string inputdirectory, string outputdirectory)
        {

            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(inputdirectory);
                if (directoryInfo.Exists)
                {
                    FileInfo[] files = directoryInfo.GetFiles("*.csv");

                    foreach (FileInfo file in files)
                    {
                        var lines = File.ReadAllLines(file.FullName).ToList();

                        var columns = lines[0].Split(",");
                        var newColumns = new StringBuilder();
                        foreach (var column in columns)
                        {
                            var newColumn = column.Replace(" ", string.Empty).Replace("?", string.Empty);
                            newColumns.Append(newColumn);
                            newColumns.Append(",");
                        }
                        var newData = new StringBuilder();
                        newData.AppendLine(newColumns.ToString());
                        for (int i = 1; i < lines.Count; i++)
                        {
                            newData.AppendLine(lines[i]);
                        }

                        File.WriteAllText(Path.Combine(outputdirectory, file.Name), newData.ToString());
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static void ToCSV(this DataTable dtDataTable, string strFilePath)
        {
            StreamWriter sw = new StreamWriter(strFilePath, false);
            //headers    
            for (int i = 0; i < dtDataTable.Columns.Count; i++)
            {
                sw.Write(dtDataTable.Columns[i]);
                if (i < dtDataTable.Columns.Count - 1)
                {
                    sw.Write("#");
                }
            }
            sw.Write(sw.NewLine);
            foreach (DataRow dr in dtDataTable.Rows)
            {
                for (int i = 0; i < dtDataTable.Columns.Count; i++)
                {
                    if (!Convert.IsDBNull(dr[i]))
                    {
                        string value = dr[i].ToString();
                        if (value.Contains('#'))
                        {
                            value = String.Format("\"{0}\"", value);
                            sw.Write(value);
                        }
                        else
                        {
                            sw.Write(string.Format("\"{0}\"", value));
                        }
                    }
                    if (i < dtDataTable.Columns.Count - 1)
                    {
                        sw.Write("#");
                    }
                }
                sw.Write(sw.NewLine);
            }
            sw.Close();
        }

        public static void DataTableToCsv(this DataTable table, string filePath)
        {
            StringBuilder csv = new StringBuilder();

            // Write headers
            string[] columnNames = new string[table.Columns.Count];
            for (int i = 0; i < table.Columns.Count; i++)
                columnNames[i] = EscapeCsvValue(table.Columns[i].ColumnName);
            csv.AppendLine(string.Join("*", columnNames));

            // Write rows
            foreach (DataRow row in table.Rows)
            {
                try
                {
                    string[] fields = new string[table.Columns.Count];
                    for (int i = 0; i < table.Columns.Count; i++)
                        fields[i] = row[i]?.ToString().Replace("\n", "").Replace("*", "");// EscapeCsvValue(row[i]?.ToString() ?? string.Empty);
                    csv.AppendLine(string.Join("*", fields));
                }
                catch (Exception)
                {
                }

            }

            // Write to file
            File.WriteAllText(filePath, csv.ToString(), Encoding.UTF8);
        }
        private static string EscapeCsvValue(string value)
        {
            if (value.Contains("*") || value.Contains("\n"))
            {
                value = value.Replace("\n", "").Replace("*", "");
                return $"{value}";
            }
            return value;
        }
        public static int solution(int[] A)
        {
            // Implement your solution here
            var testFunc = int (int[] B) =>
            {

                var maxInt = A.Max(x => x);
                bool decreasingFound = false;
                bool increasingFound = false;
                while (!decreasingFound)
                {
                    maxInt--;
                    if (!A.Any(a => a == maxInt) && maxInt > 0)
                    {
                        decreasingFound = true;
                        break;
                    }
                    else if (maxInt == 0)
                        break;
                }
                while (!increasingFound)
                {
                    maxInt++;
                    if (!A.Any(a => a == maxInt) && maxInt > 0)
                    {
                        decreasingFound = true;
                    }
                }
                return maxInt;
            };

            var result = testFunc(new int[] { 1, 2, 3 });
            Console.WriteLine("Solution is: {0}", result);
            return 0;
        }
    }
}
