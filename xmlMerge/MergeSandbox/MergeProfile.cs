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
        public string fileType = string.Empty;
        public XNamespace ns = string.Empty;
        public XNamespace sfa = "urn:oecd:ties:stffatcatypes:v2";
        public XNamespace ftc = "urn:oecd:ties:fatca:v2";

        public XNamespace crs = "urn:oecd:ties:crs:v1";
        public XNamespace stf = "urn:oecd:ties:stf:v4";

        public XNamespace cfc = "urn:oecd:ties:commontypesfatcacrs:v1";
        public XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
        //public XNamespace ns = xDoc.Root.GetDefaultNamespace();

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
            public XDocument xml = new XDocument();
            //public XDocument xml = new XDocument(
            //    new XDeclaration("1.0", "utf-8", "yes"),
            //    new XComment("Accounts appended from secondary file as no match in primary"),

            //    new XElement("Accounts",
            //        new XElement("AccountReport",
            //            new XElement("Account", "DUMMY")
            //                    ),
            //        new XComment("Accounts appended from secondary file as no match in primary")
            //                ) // Accounts
            //            );

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

        #region ReadMappings
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

        #endregion
        public void ReadIdentifiers(HotTag tags)
        {
            ns = xPrimary.Root.GetDefaultNamespace();
            //if (fileType == "SWIUS")
            //{
                
            //    var accs = from cust in xPrimary.Descendants(ftc + tags.tagAccBlock) select cust;
            //}
            //if (fileType == "SWICRS")
            //{
            //    var accs = from cust in xPrimary.Descendants(crs + tags.tagAccBlock) select cust;
            //}
            //if (fileType == "SWIS")
            //{
            //    var accs = from cust in xPrimary.Descendants(tags.tagAccBlock) select cust;
            //}

            IEnumerable<string> accID = from cust in xPrimary.Descendants(ns + tags.tagAccBlock)
                          select cust.Element(ns + tags.tagAccID).Value;

            foreach (string strName in accID)
            {
                priClientIds.Add(strName);
            }

            var names2 = from customers2 in
                        xPrimary.Descendants(ns + tags.tagAccBlock)
                         select customers2;

            //var names2 = from customers2 in
            //            xPrimary.Element(ns + tags.tagReportingGroup).Elements(ns + tags.tagAccBlock)
            //             select customers2;

            var names3 = names2.Descendants(sfa + tags.tagIndID).Select(v => v.Value);

            foreach (string strName3 in names3)
            {
                priPersonIds.Add(strName3);
            }

            // Secondary file - find all Identifiers
            IEnumerable<string> secaccID = from customers in
               xSecondary.Descendants(ns + tags.tagAccBlock)
                                            // where (double)customers.Descendants("Payment") > 400.00
                                        select customers.Element(ns + tags.tagAccID).Value;

            foreach (string strName in secaccID)
            {
                secClientIds.Add(strName);
            }
            var secnames2 = from customers2 in
                        xSecondary.Descendants(ns + tags.tagAccBlock)
                         select customers2;

            var secnames3 = secnames2.Descendants(sfa + tags.tagIndID).Select(v => v.Value);
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
                        xPrimary.Descendants(ns + tags.tagAccBlock)
                                                 select customers2;
            //IEnumerable<XElement>  = from customers in
            //        xPrimary.Descendants(htSWIUS.tagAccBlock)
            //                                // where (double)customers.Descendants("Payment") > 400.00
            //                            select customers.Value;

            foreach (var clientChunk in clientChunks)
            {
                // mp.priClientIds.Add(clientChunk);
                MergeBlock mbPri = new MergeBlock();
                mbPri.BlockId = MergeBlock.nextBlockId;
                mbPri.XmlChunk = clientChunk;

                mbPri.ClientId = (from customer in
                        mbPri.XmlChunk.Descendants(ns + tags.tagAccID)  //.Descendants(htSWIUS.tagAccBlock)
                                  select customer.FirstNode)
                        .First().ToString();
                primaryBlocks.Add(mbPri);
                MergeBlock.nextBlockId++;
            }
            // Create Secondary Client Blocks
            var secBlocks = (from c in
                        xSecondary.Descendants(ns + tags.tagAccBlock)
                                select new MergeBlock
                                {
                                    BlockId = MergeBlock.nextBlockId
                                    ,ClientId = (string)c.Element(ns + tags.tagAccID)
                                    ,XmlChunk = c
                                    ,BalCurrency = (string)c.Attribute(ns + tags.tagAccBalCurrAtt)
                               });

            //
            //  Extract Individuals within each block
            //
            //   SWIUS Can have individuals at the tags :
            //             AccountHolder.Organisation
            //             AccountHolder.Individual
            //             SubstantialOwner.Individual

            foreach (var secBlock in secBlocks)
            {
                string holderOrSubtantialOwner = string.Empty;

                // Look for people under "Account Holder"
                holderOrSubtantialOwner = "AccountHolder";
                List<XElement> ahPerson = secBlock.XmlChunk.Descendants(ns + tags.tagIndBlockowner).ToList();
                ExtractPeople(tags, secBlock, holderOrSubtantialOwner, ahPerson);

                // Look for people under "Substantial Owner"
                holderOrSubtantialOwner = "SubstantialOwner";
                List<XElement> soPerson = secBlock.XmlChunk.Descendants(ns + tags.tagIndBlocksubowner).ToList();
                ExtractPeople(tags, secBlock, holderOrSubtantialOwner, soPerson);

            }

            // ***********************************************
            // ** End of Train process
            // ***********************************************
            foreach (var secBlock in secBlocks)
            {
                // mp.priClientIds.Add(clientChunk);
                MergeBlock mbSec = new MergeBlock();
                mbSec = secBlock;
                mbSec.BlockId = MergeBlock.nextBlockId;
                secondaryBlocks.Add(mbSec);
                if (!secClientIsInPri[mbSec.ClientId])  // Client is only in secondary
                {
                    var newjob = new List<ToDo>();
                    newjob.Add(new ToDo
                    {
                        ActionReq = "AddToEndofTrain"
                    });
                    //mbSec.ToDos = newjob.ToArray();


                    //***********************************
                    //**    E R R O R S   H E R E !!!!!!
                    //***********************************
                    XElement newPosition = endOfTrain.xml.Descendants("AccountReport").LastOrDefault();
                    newPosition.AddAfterSelf(mbSec.XmlChunk);
                }
                
                MergeBlock.nextBlockId++;
            }

        }

        private void ExtractPeople(HotTag tags, MergeBlock secBlock, string holderOrSubtantialOwner, List<XElement> ahPerson)
        {
            string orgOrIndividual = string.Empty;
            string personId = string.Empty;
            string birthDate = string.Empty;
            string scv = string.Empty;
            string fullName = string.Empty;
            string firstName, middleName, lastName = string.Empty;
            XElement xmlPerson = new XElement("dummy", "dummy");

            foreach (var item in ahPerson)
            {
                if (item.HasElements)
                {
                    orgOrIndividual = item.Element(ns + tags.tagOrganisation) != null ? tags.tagOrganisation : tags.tagIndividual;
                    foreach (XElement p in item.Descendants(ns + orgOrIndividual))
                    {
                        personId = p.Element(sfa + tags.tagIndID) != null ? p.Element(sfa + tags.tagIndID).Value.ToString() : "";
                        birthDate = p.Element(sfa + "BirthDate") != null ? p.Element(sfa + "BirthDate").Value.ToString() : "";
                        if (orgOrIndividual == tags.tagOrganisation)
                        {
                            fullName = p.Element(sfa + "Name") != null ? p.Element(sfa + "Name").Value.ToString() : "";

                        }
                        else
                        {
                            firstName = p.Element(sfa + "FirstName") != null ? p.Element(sfa + "FirstName").Value.ToString() : "";
                            middleName = p.Element(sfa + "MiddleName") != null ? p.Element(sfa + "MiddleName").Value.ToString() : "";
                            lastName = p.Element(sfa + "LastName") != null ? p.Element(sfa + "LastName").Value.ToString() : "";
                            fullName = lastName + "," + firstName + " " + middleName;
                        }
                        xmlPerson = p;
                        break;
                    }
                    //personId = person.Element(sfa + tags.tagIndID)[0] != null ? person.Element(sfa + tags.tagIndID)[0].Value.ToString() : "";
                    if (personId != "")
                    {
                        AddPerson(secBlock, holderOrSubtantialOwner, orgOrIndividual, personId, birthDate, scv, fullName, xmlPerson);
                    }
                }
            }
        }

        private static void AddPerson(MergeBlock secBlock, string holderOrSubtantialOwner, string orgOrIndividual, string personId, string birthDate, string scv, string fullName, XElement xmlPerson)
        {
            int np = secBlock.Persons != null ? secBlock.Persons.Count() : 0;
            Person mp = new Person();
            mp.HolderOrSubtantialOwner = holderOrSubtantialOwner;
            mp.OrgOrIndividual = orgOrIndividual;
            mp.PersonId = personId;
            mp.SCV = scv;
            mp.FullName = fullName;
            mp.XmlPerson = xmlPerson;
            mp.BirthDate = birthDate;
            MergeBlock.AddPerson(secBlock, mp);
        }

   }
}
