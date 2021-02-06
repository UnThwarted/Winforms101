﻿using System;
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

                new XComment("AValoq"),

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

                new XComment("AValoq"),

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

            // Save 'em
            var directory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var AvqPath = Path.Combine(directory, "xmlValidate", @"AvaloqTest.xml");
            var HPPath = Path.Combine(directory, "xmlValidate", @"HPTest.xml");
            xmlAvaloq.Save(AvqPath);
            xmlLegacy.Save(HPPath);

            // Create second xml document
            XDocument Legacy = new XDocument();
            // Create mapping tables from Avaloq Ids to Legacy Ids
            DataTable dt1 = new DataTable();
            dt1.Columns.Add("Type", typeof(string)); // Account or Person
            dt1.Columns.Add("AvaloqId", typeof(string)); // Avaloq Id
            dt1.Columns.Add("LegacyId", typeof(string)); // Legacy ID
            dt1.Columns.Add("SCVcode", typeof(string)); // SCVcode
            dt1.Columns.Add("DOB", typeof(string)); // DOB

            DataRow row = dt1.NewRow();
            row["Type"] = "Account";
            row["AvaloqId"] = "bp1";
            row["LegacyId"] = "MO201";
            row["SCVcode"] = String.Empty;
            row["DOB"] = String.Empty;
            dt1.Rows.Add(row);

            row = dt1.NewRow();
            row["Type"] = "Account";
            row["AvaloqId"] = "bp3";
            row["LegacyId"] = "SH203";
            row["SCVcode"] = "2346173";
            row["DOB"] = String.Empty;
            dt1.Rows.Add(row);

            row = dt1.NewRow();
            row["Type"] = "Account";
            row["AvaloqId"] = "bp5";
            row["LegacyId"] = "NN425";
            row["SCVcode"] = String.Empty;
            row["DOB"] = String.Empty;
            dt1.Rows.Add(row);

            row = dt1.NewRow();
            row["Type"] = "Account";
            row["AvaloqId"] = "bp7";
            row["LegacyId"] = "HI777";
            row["SCVcode"] = "02643827";
            row["DOB"] = String.Empty;
            dt1.Rows.Add(row);

            row = dt1.NewRow();
            row["Type"] = "Person";
            row["AvaloqId"] = "bp11";
            row["LegacyId"] = "MO201AAA";
            row["SCVcode"] = "08723461";
            row["DOB"] = "1987-04-22";
            dt1.Rows.Add(row);

            row = dt1.NewRow();
            row["Type"] = "Person";
            row["AvaloqId"] = "bp43";
            row["LegacyId"] = "MM222AAA";
            row["SCVcode"] = "02346173";
            row["DOB"] = "1963-08-13";
            dt1.Rows.Add(row);

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
                        Viewxml(Legacy);
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

        private static void InsertInAvq(XDocument xDocMain, string newChunk)
        {
            //xdoc.Element("Customers").Elements("Customer")
            //.Where(X => X.Attribute("ID").Value == "10003").SingleOrDefault()
            //.AddBeforeSelf(
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
