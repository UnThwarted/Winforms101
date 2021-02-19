using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MergeSandbox
{
    class MergeProfile
    {
        public string filePrimary;
        public string fileSecondary;
        public string fileMapping;

        public XDocument xPrimary = new XDocument();
        public XDocument xSecondary = new XDocument();

        // Merge Type - just "xml" here
        public string mergeType;

        public Dictionary<string, string> mapPriToSec = new Dictionary<string, string>();
        public Dictionary<string, string> mapSecToPri = new Dictionary<string, string>();

        // Merge State - "new" / "inprogress" / "done" / "failed"
        public string mergeState;

        public void SetState(string state)
        {
            mergeState = state;
        }

        public class EndOfTrain
        {
            public XDocument xml = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XComment("Accounts appended from secondary file as no match in primary"),

                new XElement("Accounts",
                    new XElement("AccountReport",
                        new XElement("Account", "DUMMY")
                                ),
                    new XComment("Accounts appended from secondary file as no match in primary")
                            ) // Accounts
                        );

            public string state = "new";
            public string log = "";

        }

        public EndOfTrain endOfTrain = new EndOfTrain();

        public List<string> priClientIds = new List<string>();
        public List<string> priPersonIds = new List<string>();
        public List<string> secClientIds = new List<string>();
        public List<string> secPersonIds = new List<string>();

        public KeyedArray<bool> secClientIsInPri = new KeyedArray<bool>();

        public List<MergeBlock> primaryBlocks = new List<MergeBlock>();
        public List<MergeBlock> secondaryBlocks = new List<MergeBlock>();

        public void ReadMappings()
        {
            // Read in the mappings and check for errors such as duplicates
            try
            {
                mapPriToSec = CSVToDictionary(fileMapping, 0, 1);
                mapSecToPri = CSVToDictionary(fileMapping, 1, 0);
                if (mapPriToSec.ContainsKey("*ERROR*"))
                {
                    Dictionary<string, string>.ValueCollection values = mapPriToSec.Values;
                    foreach (string message in values)
                        throw new Exception("Error in mapping file, primary: " + message);
                    return;
                }
                if (mapSecToPri.ContainsKey("*ERROR*"))
                {
                    Dictionary<string, string>.ValueCollection values = mapSecToPri.Values;
                    foreach (string message in values)
                        throw new Exception("Error in mapping file, secondary: " + message);
                    return;
                }
            }
            catch (Exception)
            {

                throw new Exception("Invalid mapping file was specified - csv expected");
            }

        }
        static Dictionary<string, string> CSVToDictionary(string path, int colForKey, int colForValue)
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

        public void ReadIdentifiers(HotTag tags)
        {

            // Primary file - find all Identifiers
            IEnumerable<string> accID = from customers in
            xPrimary.Descendants(tags.tagAccBlock)
                                        // where (double)customers.Descendants("Payment") > 400.00
                                    select customers.Element(tags.tagAccID).Value;

            foreach (string strName in accID)
            {
                priClientIds.Add(strName);
            }

            var names2 = from customers2 in
                        xPrimary.Element(tags.tagReportingGroup).Elements(tags.tagAccBlock)
                            select customers2;

            var names3 = names2.Descendants(tags.tagIndID).Select(v => v.Value);

            foreach (string strName3 in names3)
            {
                priPersonIds.Add(strName3);
            }

            // Secondary file - find all Identifiers
            IEnumerable<string> secaccID = from customers in
               xSecondary.Descendants(tags.tagAccBlock)
                                            // where (double)customers.Descendants("Payment") > 400.00
                                        select customers.Element(tags.tagAccID).Value;

            foreach (string strName in secaccID)
            {
                secClientIds.Add(strName);
            }
            var secnames2 = from customers2 in
                        xSecondary.Element(tags.tagReportingGroup).Elements(tags.tagAccBlock)
                         select customers2;

            var secnames3 = secnames2.Descendants(tags.tagIndID).Select(v => v.Value);
            foreach (string strName3 in secnames3)
            {
                secPersonIds.Add(strName3);
            }

        }

        public void AssessSecondaryClients()
        {
            foreach (var secClient in secClientIds)
            {
                secClientIsInPri[secClient] = false;

                if (mapSecToPri.ContainsKey(secClient))
                {
                    var lkPriClient = mapSecToPri[secClient];

                    foreach (var priClient in priClientIds)
                    {
                        if (priClient == lkPriClient)
                        {
                            secClientIsInPri[secClient] = true;
                        }
                    }
                }
            }
        }

        public void MakeChunks(HotTag tags)
        {
            // Create Primary Client Blocks
            IEnumerable<XElement> clientChunks = from customers2 in
                        xPrimary.Element(tags.tagReportingGroup).Elements(tags.tagAccBlock)
                                                 select customers2;
            //IEnumerable<XElement>  = from customers in
            //        xPrimary.Descendants(htSWIUS.tagAccBlock)
            //                                // where (double)customers.Descendants("Payment") > 400.00
            //                            select customers.Value;

            foreach (var clientChunk in clientChunks)
            {
                // mp.priClientIds.Add(clientChunk);
                MergeBlock mbPri = new MergeBlock();
                mbPri.blockId = MergeBlock.nextBlockId;
                mbPri.xmlChunk = clientChunk;

                mbPri.clientId = (from customer in
                        mbPri.xmlChunk.Descendants(tags.tagAccID)  //.Descendants(htSWIUS.tagAccBlock)
                                  select customer.FirstNode)
                        .First().ToString();
                primaryBlocks.Add(mbPri);
                MergeBlock.nextBlockId++;
            }
            // Create Secondary Client Blocks
            var secBlocks = (from c in
                        xSecondary.Element(tags.tagReportingGroup).Elements(tags.tagAccBlock)
                                select new MergeBlock
                                {
                                    blockId = MergeBlock.nextBlockId,
                                    clientId = (string)c.Element(tags.tagAccID),
                                    xmlChunk = c,
                                    balCurrency = (string)c.Element("balanceCurrency"),
                                    balValue = 0,
                                    Persons = (
                                    from p in c.Elements("Individual")
                                    select new Person
                                    {
                                        PersonId = (string)p.Element(tags.tagIndID),
                                        BirthDate = (string)p.Element("Name")
                                    }).ToArray()
                                    //,
                                    //Payments = (
                                    //from p in c.Elements("Individual")
                                    //select new Person
                                    //{
                                    //    PersonId = (string)p.Element(tags.tagIndID),
                                    //    BirthDate = (string)p.Element("Name")
                                    //}).ToArray()
                                });

            //IEnumerable<XElement>  = from customers in
            //        xPrimary.Descendants(htSWIUS.tagAccBlock)
            //                                // where (double)customers.Descendants("Payment") > 400.00
            //                            select customers.Value;

            foreach (var secBlock in secBlocks)
            {
                // mp.priClientIds.Add(clientChunk);
                MergeBlock mbSec = new MergeBlock();
                mbSec = secBlock;
                mbSec.blockId = MergeBlock.nextBlockId;
                secondaryBlocks.Add(mbSec);
                if (!secClientIsInPri[mbSec.clientId])  // Client is only in secondary
                {
                    var newjob = new List<ToDo>();
                    newjob.Add(new ToDo
                    {
                        actionReq = "AddToEndofTrain"
                    });
                    mbSec.ToDos = newjob.ToArray();

                    XElement newPosition = endOfTrain.xml.Descendants("AccountReport").LastOrDefault();
                    newPosition.AddAfterSelf(mbSec.xmlChunk);
                }
                
                MergeBlock.nextBlockId++;
            }

        }

    }
}
