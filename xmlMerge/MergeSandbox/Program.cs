using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Xml.Linq;
using System.IO;



namespace MergeSandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create first xml document
            XDocument xmlAvaloq = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),

                new XComment("Avaloq"),

                new XElement("Accounts",
                  new XElement("AccountReport",
                    new XElement("Account", "bp1"),
                    new XElement("OrgName", "bp1 Org"),
                    new XElement("Individual",
                            new XComment("SCV:00020094"),
                            new XElement("Person", "BP.0001"),
                            new XElement("Name", "Abhishek"),
                            new XElement("Payment", "111.10"),
                            new XElement("Location", "Mumbai"),
                            new XElement("Address", "off link road malad west Mumbai")
                                     ) // Individual
                                 ) // AccountReport
                                             ,

                  new XElement("AccountReport",
                    new XElement("Account", "bp2"),
                    new XElement("OrgName", "bp2 Org"),
                    new XElement("Individual",
                            new XComment("SCV:00022222"),
                            new XElement("Person", "BP.0002"),
                            new XElement("Name", "Rajesh"),
                            new XElement("Payment", "222.20"),
                            new XElement("Location", "New Delhi"),
                            new XElement("Address", "off link road laljatnagar New delhi")
                                     ) // Individual
                                 ) // AccountReport
                                    ,

                  new XElement("AccountReport",
                    new XElement("Account", "bp3"),
                    new XElement("OrgName", "bp3 Org"),
                    new XElement("Individual",
                            new XComment("SCV:00023333"),
                            new XElement("Person", "BP.0003"),
                            new XElement("Name", "Rohan"),
                            new XElement("Payment", "333.30"),
                            new XElement("Location", "Mumbai"),
                            new XElement("Address", " link road Kandivali  west Mumbai")
                                     ) // Individual
                                 ) // AccountReport
                                    ,

                  new XElement("AccountReport",
                    new XElement("Account", "bp4"),
                    new XElement("OrgName", "bp4 Org"),
                    new XElement("Individual",
                            new XComment("SCV:00024444"),
                            new XElement("Person", "BP.0004"),
                            new XElement("Name", "Tony"),
                            new XElement("Payment", "444.40"),
                            new XElement("Location", "London"),
                            new XElement("Address", " 25 Moorgate London")
                                     ) // Individual
                                 ) // AccountReport
                                    ,

                  new XElement("AccountReport",
                    new XElement("Account", "bp7"),
                    new XElement("OrgName", "bp7 Org"),
                    new XElement("Individual",
                            new XComment("SCV:02643827"),
                            new XElement("Person", "BP.0007"),
                            new XElement("Name", "Sonali"),
                            new XElement("Payment", "555.50"),
                            new XElement("Location", "Mumbai"),
                            new XElement("Address", "khar west Mumbai")
                                     ) // Individual
                                 ) // AccountReport
                            ) // Accounts
                        );

            // Create first xml document
            XDocument xmlLegacy = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),

                new XComment("Legacy"),

                new XElement("Accounts",
                  new XElement("AccountReport",
                    new XElement("Account", "HP111"),
                    new XElement("OrgName", "bp1 Org"),
                    new XElement("Individual",
                            new XComment("SCV:00020094"),
                            new XElement("Person", "HP111AAA"),
                            new XElement("Name", "Abhishek"),
                            new XElement("Payment", "1000.90"),
                            new XElement("Location", "Mumbai"),
                            new XElement("Address", "off link road malad west Mumbai")
                                     ) // Individual
                                 ) // AccountReport
                                             ,

                  new XElement("AccountReport",
                    new XElement("Account", "HP222"),
                    new XElement("OrgName", "bp2 Org"),
                    new XElement("Individual",
                            new XComment("SCV:00022222"),
                            new XElement("Person", "HP222AAA"),
                            new XElement("Name", "Rajesh"),
                            new XElement("Payment", "-50.20"),
                            new XElement("Location", "New Delhi"),
                            new XElement("Address", "off link road laljatnagar New delhi")
                                     ) // Individual
                                     ,
                    new XElement("Individual",
                            new XComment("SCV:00022223"),
                            new XElement("Person", "HP222S01"),
                            new XElement("Name", "Dave Not Migrated"),
                            new XElement("Payment", "15.15"),
                            new XElement("Location", "London"),
                            new XElement("Address", "1 The Street SE1 7NA")
                                     ) // Individual
                                 ) // AccountReport
                                    ,

                  new XElement("AccountReport",
                    new XElement("Account", "HP666"),
                    new XElement("OrgName", "Legacy Only Org"),
                    new XElement("Individual",
                            new XComment("SCV:00026666"),
                            new XElement("Person", "HP666AAA"),
                            new XElement("Name", "Bowie DECEASED JAN 2020"),
                            new XElement("Payment", "2001.00"),
                            new XElement("Location", "New York"),
                            new XElement("Address", " CEEBEE GEEBEES Lower East Side NYC")
                                     ) // Individual
                                 ) // AccountReport

                                 ) // Accounts
                        );

            // Save 'em
            var directory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var avqPath = Path.Combine(directory, "xmlValidate", @"AvaloqTest.xml");
            var hpPath = Path.Combine(directory, "xmlValidate", @"HPTest.xml");
            var mapPath = Path.Combine(directory, "xmlValidate", @"MappingAvqFirst.csv");
            xmlAvaloq.Save(avqPath);
            xmlLegacy.Save(hpPath);

            // Present Menu
            ShowMenu();
            ConsoleKeyInfo cki;
            do
            {
                cki = Console.ReadKey();
                string option = cki.Key.ToString().Tail(1);

                switch (option)
                {
                    case "r":  //SpaceBa[r]
                        ShowMenu();
                        break;
                    case "1":
                        Viewxml(xmlAvaloq);
                        break;
                    case "2":
                        Viewxml(xmlLegacy);
                        break;
                    case "3":
                        ViewMapping(mapPath);
                        break;
                    default:
                        break;
                }

            } while (cki.Key != ConsoleKey.Escape);
        }

        private static void ShowMenu()
        {
            // Present Menu
            Console.WriteLine("Menu v1.0");
            Console.WriteLine("1 View xml 1");
            Console.WriteLine("2 View xml 2");
            Console.WriteLine("3 View Mapping table");
            Console.WriteLine("4 Show matches");
            Console.WriteLine("5 Append new Account for xml 2 at end of xml 1");
            Console.WriteLine("6 Insert new Person from xml 2 into xml 1 under equivalent account");
            Console.WriteLine("7 Combine value from xml 2 with equivalent value in file 1");
            Console.WriteLine("\nSelect option");
            Console.WriteLine("Press the Escape (Esc) key to quit: \n");
        }
        private static void Viewxml(XDocument xdoc)
        {
            Console.WriteLine(xdoc);

            Console.WriteLine("\nList of Accounts:\n");
            IEnumerable<string> names = from customers in
                        xdoc.Descendants("AccountReport")
                                       // where (double)customers.Descendants("Payment") > 400.00
                                        select customers.Element("Account").Value;

            foreach (string strName in names)
            {
                Console.WriteLine(strName);
            }
            var names2 = from customers2 in
                        xdoc.Element("Accounts").Elements("AccountReport")
                         select customers2 ;

            var names3 = names2.Descendants("Name").Select(v => v.Value);
            foreach (string strName3 in names3)
            {
                Console.WriteLine(strName3);
            }

            Console.WriteLine("\nhit Spacebar for menu");
        }

        private static void ViewMapping(string mapPath)
        {
            Console.WriteLine("\nAvaloq to HP Mappings\n");
            var mapAvqToHP = CSVToDictionary(mapPath, 0, 1);
            if (mapAvqToHP.ContainsKey("*ERROR*"))
            {
                Dictionary<string, string>.ValueCollection values = mapAvqToHP.Values;
                foreach (string message in values)
                    Console.WriteLine("Error in mapping file : {0}\n\nhit Spacebar for menu", message);
                return;
            }
            else
            {
                foreach (var avqToHP in mapAvqToHP)
                    Console.WriteLine("AvqRef={0} HPRef={1}", avqToHP.Key.PadLeft(10), avqToHP.Value.PadLeft(10));
            }

            Console.WriteLine("\nHP to Avaloq Mappings\n");
            var mapHPToAvq = CSVToDictionary(mapPath, 1, 0);

            if (mapHPToAvq.ContainsKey("*ERROR*"))
            {
                Dictionary<string, string>.ValueCollection values = mapHPToAvq.Values;
                foreach (string message in values)
                    Console.WriteLine("Error in mapping file: {0}\n\nhit Spacebar for menu", message);
                return;
            }
            else
            {
                foreach (var hpToAvq in mapHPToAvq)
                    Console.WriteLine("HPRef={0} AvqRef={1}", hpToAvq.Key.PadLeft(10), hpToAvq.Value.PadLeft(10));
            }
            Console.WriteLine("\nhit Spacebar for menu");
        }
        private static void InsertInAvq(XDocument xDocMain, string newChunk)
        {
            //xdoc.Element("Customers").Elements("Customer")
            //.Where(X => X.Attribute("ID").Value == "10003").SingleOrDefault()
            //.AddBeforeSelf(
        }

        static Dictionary<string, string> CSVToDictionary(string path,int colForKey, int colForValue)
        {
            // Read the file
            var data = System.IO.File.ReadAllLines(path);

            // Check for duplicates
            var listCola = data.Skip(1).Select(m => m.Split(','))
                .Select(m => m[colForKey]);
            var colaCount = listCola.Count();
            var colaDistinct = listCola.Distinct().Count();

            var listColb = data.Skip(1).Select(m => m.Split(','))
                .Select(m => m[colForValue]);
            var colbCount = listColb.Count();
            var colbDistinct = listColb.Distinct().Count();

            if ((colaCount != colaDistinct) || (colbCount != colbDistinct))
            {
                Dictionary<string, string> nada = new Dictionary<string, string>();
                string info = (colaCount != colaDistinct) ? "Duplicate(s) found in Key Column " : "Duplicate(s) found in Value Column ";
                nada.Add("*ERROR*", info);
                return nada;
            }

            // Populate the dictionary from the CSV file using the specified columns
            return data.Skip(1).Select(m => m.Split(',')).ToDictionary(m => m[colForKey], m => m[colForValue]);
      }


    }

}
    public static class StringExtension
    {
        public static string Tail(this string source, int tailLength)
        {
            if (tailLength >= source.Length)
                return source;
            return source.Substring(source.Length - tailLength);
        }
        public static string PathWithSlashes(this string source)
        {
            return source.Replace(@"\", @"\\");
        }
    }

