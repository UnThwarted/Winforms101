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
        // HotTags to navigate each type of xml

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
                            new XElement("Location", "Mumbai"),
                            new XElement("Address", "off link road malad west Mumbai")
                                     ), // Individual
                    new XElement("Balance", "11111.10"),
                    new XElement("Payment",
                            new XElement("Type", "FATCA501"),
                            new XElement("PaymentAmnt", new XAttribute("currCode", "EUR"), "111.10")
                                ) // Payment
                                 ) // AccountReport
                                             ,

                  new XElement("AccountReport",
                    new XElement("Account", "bp2"),
                    new XElement("OrgName", "bp2 Org"),
                    new XElement("Individual",
                            new XComment("SCV:00022222"),
                            new XElement("Person", "BP.0002"),
                            new XElement("Name", "Rajesh"),
                            new XElement("Location", "New Delhi"),
                            new XElement("Address", "off link road laljatnagar New delhi")
                                     ), // Individual
                    new XElement("Balance", "22222.20"),
                    new XElement("Payment", "222.20")
                                 ) // AccountReport
                                    ,

                  new XElement("AccountReport",
                    new XElement("Account", "bp3"),
                    new XElement("OrgName", "bp3 Org"),
                    new XElement("Individual",
                            new XComment("SCV:00023333"),
                            new XElement("Person", "BP.0003"),
                            new XElement("Name", "Rohan"),
                            new XElement("Location", "Mumbai"),
                            new XElement("Address", " link road Kandivali  west Mumbai")
                                     ), // Individual
                    new XElement("Balance", "33333.30"),
                    new XElement("Payment", "333.30")
                                 ) // AccountReport
                                    ,

                  new XElement("AccountReport",
                    new XElement("Account", "bp4"),
                    new XElement("OrgName", "bp4 Org"),
                    new XElement("Individual",
                            new XComment("SCV:00024444"),
                            new XElement("Person", "BP.0004"),
                            new XElement("Name", "Tony"),
                            new XElement("Location", "London"),
                            new XElement("Address", " 25 Moorgate London")
                                     ), // Individual
                    new XElement("Balance", "44444.40"),
                    new XElement("Payment", "444.40")
                                 ) // AccountReport
                                    ,

                  new XElement("AccountReport",
                    new XElement("Account", "bp7"),
                    new XElement("OrgName", "bp7 Org"),
                    new XElement("Individual",
                            new XComment("SCV:02643827"),
                            new XElement("Person", "BP.0007"),
                            new XElement("Name", "Sonali"),
                            new XElement("Location", "Mumbai"),
                            new XElement("Address", "khar west Mumbai")
                                     ), // Individual
                    new XElement("Balance", "55555.50"),
                    new XElement("Payment", "555.50")
                                 ) // AccountReport
                            ) // Accounts
                        );

            // Add an account programatically
            XElement fred =
            new XElement("AccountReport",
              new XElement("Account", "bp997"),
              new XElement("OrgName", "bp997 Org"),
              new XElement("Individual",
                      new XComment("SCV:02643827"),
                      new XElement("Person", "BP.0997"),
                      new XElement("Name", "Fred"),
                      new XElement("Location", "Flintstone"),
                      new XElement("Address", "khar west Mumbai")
                               ), // Individual
              new XElement("Balance", "99999.90"),
              new XElement("Payment", "999.90")
                           ); // AccountReport

            XElement newPosition = xmlAvaloq.Root.Element("Accounts");
            XElement parentElement = xmlAvaloq.Descendants("AccountReport").LastOrDefault();

            parentElement.AddAfterSelf(fred);



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
                            new XElement("Location", "Mumbai"),
                            new XElement("Address", "off link road malad west Mumbai")
                                     ), // Individual
                    new XElement("Balance", "100.11"),
                    new XElement("Payment", "1000.90")
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
                                     ), // Individual
                    new XElement("Balance", "21111.10"),
                    new XElement("Payment", "15.15")
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
                                     ), // Individual
                    new XElement("Balance", "21111.10"),
                    new XElement("Payment", "111.10")
                                 ) // AccountReport

                                 ) // Accounts
                        );

            // Set up new merge profile
            MergeProfile mp = new MergeProfile();
            mp.SetState("new");

            
            // Save 'em
            var directory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            mp.filePrimary = Path.Combine(directory, "xmlValidate", @"AvaloqTest.xml");
            mp.fileSecondary = Path.Combine(directory, "xmlValidate", @"HPTest.xml");
            mp.fileMapping = Path.Combine(directory, "xmlValidate", @"MappingAvqFirst.csv");
            mp.xPrimary = xmlAvaloq;
            mp.xSecondary = xmlLegacy;
            mp.mapPriToSec = new Dictionary<string, string>();
            mp.mapSecToPri = new Dictionary<string, string>();
            try
            {
                mp.ReadMappings();
            }
            catch (Exception)
            {

                throw;
            }

            XDocument xmlLegacyOnlyAccs = new XDocument();
            
            xmlAvaloq.Save(mp.filePrimary);
            xmlLegacy.Save(mp.fileSecondary);

            // Set up xml naigation name
            HotTag htSWIUS = new HotTag();
            htSWIUS.tagReportingGroup = "Accounts";
            htSWIUS.tagAccBlock = "AccountReport";
            htSWIUS.tagAccID = "Account";
            htSWIUS.tagIndBlockowner = "Individual";
            htSWIUS.tagIndBlocksubowner = "SubstantialOwner";
            htSWIUS.tagIndID = "Person";

            // Read all the identifiers in both xmls
            try
            {
                mp.ReadIdentifiers(htSWIUS);
            }
            catch (Exception)
            {

                throw;
            }
            // Create a list of Secondary clients that exist in the Primary
            try
            {
                mp.AssessSecondaryClients();
            }
            catch (Exception)
            {

                throw;
            }

            // Create Primary Client Blocks
            try
            {
                mp.MakeChunks(htSWIUS);
                var barn = mp.secondaryBlocks;
            }
            catch (Exception)
            {

                throw;
            }

            // Create Secondary Client Blocks

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
                        //Viewxml(xmlAvaloq, htSWIUS);
                        Console.WriteLine(mp.xPrimary);
                        Console.WriteLine("\nList of Accounts:\n");
                        foreach (string client in mp.priClientIds)
                        {
                            Console.WriteLine(client);
                        }
                        Console.WriteLine(" ");
                        foreach (string person in mp.priPersonIds)
                        {
                            Console.WriteLine(person);
                        }

                        Console.WriteLine("\nhit Spacebar for menu");
                        break;
                    case "2":
                        //Viewxml(xmlLegacy,htSWIUS);
                        Console.WriteLine(mp.xSecondary);
                        Console.WriteLine("\nList of Accounts:\n");
                        foreach (string client in mp.secClientIds)
                        {
                            Console.WriteLine(client);
                        }
                        Console.WriteLine(" ");
                        foreach (string person in mp.secPersonIds)
                        {
                            Console.WriteLine(person);
                        }

                        Console.WriteLine("\nhit Spacebar for menu");
                        break;
                    case "3":
                        ViewMapping(mp);
                        break;
                    case "4":
                        ShowMatches(xmlLegacy, htSWIUS, mp.mapSecToPri);
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

        private static void ViewMapping(MergeProfile mp)
        {
            Console.WriteLine("\nAvaloq to HP Mappings\n");

            foreach (var avqToHP in mp.mapPriToSec)
                Console.WriteLine("AvqRef={0} HPRef={1}", avqToHP.Key.PadLeft(10), avqToHP.Value.PadLeft(10));

            Console.WriteLine("\nHP to Avaloq Mappings\n");

            foreach (var hpToAvq in mp.mapSecToPri)
                    Console.WriteLine("HPRef={0} AvqRef={1}", hpToAvq.Key.PadLeft(10), hpToAvq.Value.PadLeft(10));
            Console.WriteLine("\nhit Spacebar for menu");
        }

        private static void ShowMatches(XDocument xdoc, HotTag tags, Dictionary<string, string> mapHPToAvq)
        {
            // 
            Console.WriteLine(xdoc);
            Console.WriteLine("\nShow Matches:\n");
            foreach (var hpToAvq in mapHPToAvq)
                Console.WriteLine("HPRef={0} AvqRef={1}", hpToAvq.Key.PadLeft(10), hpToAvq.Value.PadLeft(10));
            //mapHPToAvq = CSVToDictionary(mapPath, 1, 0);
            IEnumerable<string> accID = from customers in
                        xdoc.Descendants(tags.tagAccBlock)
                                          //   where (double)customers.Descendants("Payment") > 400.00
                                        select customers.Element(tags.tagAccID).Value;

            foreach (string strName in accID)
            {
                if (mapHPToAvq.ContainsKey(strName))
                {
                    Console.WriteLine(strName + " Account will be merged");
                }
                else
                {
                    Console.WriteLine(strName + " Account only in Legacy file");
                    //var names2 = from customers2 in
                    //            xdoc.Element(tags.tagReportingGroup).Elements(tags.tagAccBlock)
                    //            where (bool)customers2.Descendants(tags.tagAccID).Select(v => v.Value == strName)
                    //             select customers2;
                }
            }
            // Create a KeyedArray for ints (now generic).
            KeyedArray<int> ka = new KeyedArray<int>();

            // Save the ages of the Simpsons' kids.
            ka["Bart"] = 8;
            ka["Lisa"] = 10;
            ka["Maggie"] = 2;

            // Look up the age of Lisa.
            Console.WriteLine("Let's find Lisa's age");
            int age = ka["Lisa"];
            Console.WriteLine("Lisa is {0}", age);

            // Replace Bart's age with a new value (Bart already in list).
            ka["Bart"] = 108;
            Console.WriteLine(ka["Bart"]);

            //
            KeyedArray<string> la = new KeyedArray<string>();

            // Save the likes of the Simpsons' kids.
            string bartlike = "shorts";
            la["Bart"] = bartlike;
            la["Lisa"] = "sax";
            la["Maggie"] = "dummy";

            // Look up the age of Lisa.
            Console.WriteLine("Let's find Bart's like");
            string like = la["Bart"];
            Console.WriteLine("Bart likes {0}", like);

            // Replace Bart's age with a new value (Bart already in list).
            var who = "Lisa";
            la[who] = "Democracy";
            Console.WriteLine("{0} also likes {1}", who, la[who]);

            // Account Numbers
            var avAccTag = "Account";
            var avIndTag = "Person";

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
    }



