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
        // HotTags to navigate each type of xml

        static void Main(string[] args)
        {
            // Set up new merge profile
            MergeProfile mp = new MergeProfile();
            mp.SetState("new");

            
            // Load the Primary xml (master) and the Secondary xml and csv with the to/from identifiers
            var directory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            mp.filePrimary = Path.Combine(directory, "xmlValidate", @"SWI_US_AV.xml");
            mp.fileSecondary = Path.Combine(directory, "xmlValidate", @"SWI_US_HP.xml");
            mp.fileMapping = Path.Combine(directory, "xmlValidate", @"SWI_US_MapAvqFirst.csv");
            mp.xPrimary = XDocument.Load(mp.filePrimary);
            mp.xSecondary = XDocument.Load(mp.fileSecondary);
            mp.mapPriToSec = new Dictionary<string, string>();
            mp.mapSecToPri = new Dictionary<string, string>();
            mp.fileType = "SWIUS";
            //try
            //{
            //    dsMerge myds = new dsMerge();
            //    myds.Test(mp.filePrimary.ToString(), mp.fileSecondary.ToString());
            //}
            //catch (Exception)
            //{

            //    throw;
            //}

            try
            {
                mp.ReadMappings();
            }
            catch (Exception)
            {

                throw;
            }

            XDocument xmlLegacyOnlyAccs = new XDocument();
            
            //xmlAvaloq.Save(mp.filePrimary);
            //xmlLegacy.Save(mp.fileSecondary);

            // Set up xml naigation name
            HotTag htSWIUS = new HotTag();
            htSWIUS.tagReportingGroup = "ReportingGroup";
            htSWIUS.tagAccBlock = "AccountReport";
            htSWIUS.tagAccID = "AccountNumber";
            htSWIUS.tagIndBlockowner = "AccountHolder";
            htSWIUS.tagIndBlocksubowner = "SubstantialOwner";
            htSWIUS.tagIndID = "TIN";
            htSWIUS.tagAccBalance = "AccountBalance";
            htSWIUS.tagAccBalCurrAtt = "currCode";
            htSWIUS.tagOrganisation = "Organisation";
            htSWIUS.tagIndividual = "Individual";

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
                        ShowMatches(mp.xSecondary, htSWIUS, mp.mapSecToPri);
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



