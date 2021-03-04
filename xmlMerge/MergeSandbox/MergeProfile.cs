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
        public string fileMerged;

        public string errorLog;

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

        // Merge Type - "Primary" or "Secondary"
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

                new XElement("Accounts", "") // Accounts
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

            // Primary file - find all Identifiers
            priClientIds = PopulateStringListFromXdoc(xPrimary,     ns, tags.tagAccBlock, ns, tags.tagAccID);
            priPersonIds = PopulateStringListFromXdoc(xPrimary,     ns, tags.tagAccBlock, sfa, tags.tagIndID);
            // Secondary file - find all Identifiers
            secClientIds = PopulateStringListFromXdoc(xSecondary,   ns, tags.tagAccBlock, ns, tags.tagAccID);
            secPersonIds = PopulateStringListFromXdoc(xSecondary,   ns, tags.tagAccBlock, sfa, tags.tagIndID);
        }

        private List<string> PopulateStringListFromXdoc(XDocument readXdoc, XNamespace ns1, string tag1,
                                                                            XNamespace ns2, string tag2)
        {
            List<string> results = new List<string>();

            //var accID = from cust in readXdoc.Descendants(ns1 + tag1)
            //            select cust.Element(ns2 + tag2).Value;
            var accID = readXdoc.Descendants(ns1 + tag1)
                                .Descendants(ns2 + tag2).Select(v => v.Value);

            foreach (string strName in accID)
            {
                results.Add(strName);
            }
            return results;
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
            var priBlocks = (from c in
                        xPrimary.Descendants(ns + tags.tagAccBlock)
                             select new MergeBlock
                             {
                                 //BlockId = MergeBlock.nextBlockId
                                 ClientId = (string)c.Element(ns + tags.tagAccID)
                                 ,
                                 XmlChunk = c
                             });

            mergeType = "Primary";
            ExtractMoreDetails(tags, priBlocks, primaryBlocks);

            // Create Secondary Client Blocks
            var secBlocks = (from c in
                        xSecondary.Descendants(ns + tags.tagAccBlock)
                             select new MergeBlock
                             {
                                 //BlockId = MergeBlock.nextBlockId
                                 ClientId = (string)c.Element(ns + tags.tagAccID)
                                 ,
                                 XmlChunk = c
                             });

            mergeType = "Secondary";
            ExtractMoreDetails(tags, secBlocks, secondaryBlocks);

            // Collect Clients that only appear on the Secondary File
            SetEndOfTrain(tags);

            MarkMatchedSecondaryPeople(tags, primaryBlocks, secondaryBlocks);

            PrepUnmatchedSecondaryPeople(tags, primaryBlocks, secondaryBlocks);
        }

        private void MarkMatchedSecondaryPeople(HotTag tags, List<MergeBlock> priBlockList, List<MergeBlock> secBlockList)
        {
            foreach (MergeBlock pb in priBlockList)
            {
                string secAcc;
                if (!mapPriToSec.TryGetValue(pb.ClientId, out secAcc))
                {
                    continue;
                }
                else
                {
                    MergeBlock sb = GetMergeBlock(secBlockList, secAcc);
                    foreach (Person pbp in pb.Persons)
                    {
                        if (sb != null)
                        {
                            MarkTheMatches(tags, pb, pbp, sb);
                        }
                    }
                }
            }
        }
        private void PrepUnmatchedSecondaryPeople(HotTag tags, List<MergeBlock> priBlockList, List<MergeBlock> secBlockList)
        {
            foreach (MergeBlock sb in secBlockList)
            {
                string priAcc;
                if (!mapSecToPri.TryGetValue(sb.ClientId, out priAcc))
                {
                    continue;
                }
                else
                {
                    MergeBlock pb = GetMergeBlock(priBlockList, priAcc);
                    int personIdx = 0;
                    foreach (Person sbp in sb.Persons)
                    {
                        if (sbp.ExistsInPrimaryBlockId == 0)
                        {
                            // Add ToDo to the secondary person : Action append Person to Primary Client
                            int np = sb.Persons[personIdx].ToDos != null ? sb.Persons[personIdx].ToDos.Count() : 0;
                            if (np == 0)
                            {
                                ToDo td = new ToDo();
                                td.ActionReq = "AddPerson";
                                td.PrimaryChunkId = pb.ClientId;
                                td.PrimaryTag = tags.tagIndividual;
                                td.SecondaryTag = string.Empty;
                                td.Log = string.Empty;
                                td.doneRef = 0;
                                MergeBlock.AddPersonToDo(sb, personIdx, td);

                            }                        }
                        personIdx++;
                    }
                    //AddToDoForThoseWeNeedToAdd(tags, pb, sb);
                }
            }
        }

        private static void MarkTheMatches(HotTag tags, MergeBlock pb, Person pbp, MergeBlock sb)
        {
            foreach (Person sbp in sb.Persons)
            {
                // skip to next iteration if already matched
                if ((sbp.ExistsInPrimaryBlockId > 0) || (sbp.OrgOrIndividual == tags.tagOrganisation))
                {
                    continue;
                }
                else
                {
                    // check for a match
                    if (sbp.PersonId == pbp.PersonId && sbp.FullName == pbp.FullName)
                    {
                        sbp.ExistsInPrimaryBlockId = pb.BlockId;
                    }
                    if (sbp.FullName == pbp.FullName)
                    {
                        sbp.ExistsInPrimaryBlockId = pb.BlockId;
                    }
                }
            }
        }

        private MergeBlock GetMergeBlock(List<MergeBlock> blockList, string clientId)
        {
            foreach(MergeBlock mb in blockList)
            {
                if (mb.ClientId == clientId)
                {
                    return mb;
                }
            }
            return null;
        }
        private void ExtractMoreDetails(HotTag tags, IEnumerable<MergeBlock> Blocks, List<MergeBlock> blockList)
        {
            //
            //  Extract Individuals within each block
            //
            //   SWIUS Can have individuals at the tags :
            //             AccountHolder.Organisation
            //             AccountHolder.Individual
            //             SubstantialOwner.Individual

            foreach (var block in Blocks)
            {
                string holderOrSubtantialOwner = string.Empty;


                // Look for people under "Account Holder"
                holderOrSubtantialOwner = "AccountHolder";
                List<XElement> ahPerson = block.XmlChunk.Descendants(ns + tags.tagIndBlockowner).ToList();
                ExtractPeople(tags, block, holderOrSubtantialOwner, ahPerson);

                // Look for people under "Substantial Owner"
                holderOrSubtantialOwner = "SubstantialOwner";
                List<XElement> soPerson = block.XmlChunk.Descendants(ns + tags.tagIndBlocksubowner).ToList();
                ExtractPeople(tags, block, holderOrSubtantialOwner, soPerson);

                List<XElement> payments = block.XmlChunk.Descendants(ns + tags.tagPayment).ToList();
                ExtractPayments(tags, block, payments);

                if (mergeType == "Secondary")
                {
                    if (!secClientIsInPri[block.ClientId])  // Client is only in secondary
                    {
                        var actionReq = "AddToEndofTrain";
                        var primaryChunkId = string.Empty;
                        var primaryTag = string.Empty;
                        var secondaryTag = block.ClientId;
                        AddToDo(block, actionReq, primaryChunkId, primaryTag, secondaryTag);
                    }

                }

                block.BlockId = MergeBlock.nextBlockId;
                blockList.Add(block);
                MergeBlock.nextBlockId++;

            }
        }
        private void SetEndOfTrain(HotTag tags)
        {
            endOfTrain.xml = new XDocument(xSecondary);
            // Remove any AccountReports that are matched in the primary
            endOfTrain.xml.Descendants(ftc + tags.tagAccBlock)
                .Where(e => secClientIsInPri[e.Element(ftc + tags.tagAccID).Value] == true)
                .Remove();

            // Remove any remaining empty ReportingGroups
            endOfTrain.xml.Descendants(ftc + tags.tagReportingGroup)
                .Where(e => e.Element(ftc + tags.tagAccBlock) == null)
                .Remove();

            // Add a comment "Added from secondary file" for each AccountReport
            foreach (XElement eleDocCli in endOfTrain.xml.Descendants(ftc + tags.tagReportingGroup).Elements(ftc + tags.tagAccBlock))
            {
                        eleDocCli.AddBeforeSelf(new XComment("** Added from secondary file **"));
            }
        }

        private void ExtractPeople(HotTag tags, MergeBlock block, string holderOrSubtantialOwner, List<XElement> ahPerson)
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

                        var bd = p.Descendants(sfa + "BirthInfo").Descendants(sfa + "BirthDate").Select(s => s.Value).FirstOrDefault();
                        birthDate = bd != null ? bd.ToString() : "";

                        var nm = p.Descendants(sfa + "Name").FirstOrDefault();
                        if (orgOrIndividual == tags.tagOrganisation)
                        {
                            fullName = p.Element(sfa + "Name") != null ? p.Element(sfa + "Name").Value.ToString().Trim() : "";

                        }
                        else
                        {
                            firstName = nm.Element(sfa + "FirstName") != null ? nm.Element(sfa + "FirstName").Value.ToString() : "";
                            middleName = nm.Element(sfa + "MiddleName") != null ? nm.Element(sfa + "MiddleName").Value.ToString() : "";
                            lastName = nm.Element(sfa + "LastName") != null ? nm.Element(sfa + "LastName").Value.ToString() : "";
                            fullName = (lastName + "," + firstName + " " + middleName).Trim();
                        }
                        xmlPerson = p;
                        break;
                    }
                    //personId = person.Element(sfa + tags.tagIndID)[0] != null ? person.Element(sfa + tags.tagIndID)[0].Value.ToString() : "";
                    if (personId != "")
                    {
                        AddPerson(block, holderOrSubtantialOwner, orgOrIndividual, personId, birthDate, scv, fullName, xmlPerson);
                    }
                }
            }
        }

        private static void AddPerson(MergeBlock block, string holderOrSubtantialOwner, string orgOrIndividual, string personId, string birthDate, string scv, string fullName, XElement xmlPerson)
        {
            int np = block.Persons != null ? block.Persons.Count() : 0;
            Person mp = new Person();
            mp.HolderOrSubtantialOwner = holderOrSubtantialOwner;
            mp.OrgOrIndividual = orgOrIndividual;
            mp.PersonId = personId;
            mp.SCV = scv;
            mp.FullName = fullName;
            mp.XmlPerson = xmlPerson;
            mp.BirthDate = birthDate;
            MergeBlock.AddPerson(block, mp);
        }

        private void ExtractPayments(HotTag tags, MergeBlock block, List<XElement> payments)
        {
            string payType = string.Empty;
            string spayValue = string.Empty;
            double payValue;
            string payCurrency = string.Empty;

            XElement xmlPayment = new XElement("dummy", "dummy");

            foreach (var p in payments)
            {
                if (p.HasElements)
                {
                    //orgOrIndividual = item.Element(ns + tags.tagOrganisation) != null ? tags.tagOrganisation : tags.tagIndividual;
                    //foreach (XElement p in item.Descendants(ns + orgOrIndividual))
                    //{
                    payType = p.Element(ns + tags.tagPayType) != null ? p.Element(ns + tags.tagPayType).Value.ToString() : "";

                    spayValue = p.Element(ns + tags.tagPayAmount) != null ? p.Element(ns + tags.tagPayAmount).Value : "";

                    try
                    {
                        payValue = Convert.ToDouble(spayValue);
                    }
                    catch (Exception)
                    {

                        errorLog = errorLog + "\n" + "Invalid Payment Amount:" + spayValue + 
                            payments.ToString() + "\n";
                        return;
                    }
                    var fred = p.Element(ns + tags.tagPayAmount);
                    var barney = fred.Attribute(tags.tagAccBalCurrAtt);

                    payCurrency = p.Element(ns + tags.tagPayAmount).Attribute(tags.tagAccBalCurrAtt) != null ?
                                        p.Element(ns + tags.tagPayAmount).Attribute(tags.tagAccBalCurrAtt).Value : "";

                    AddPayment(block, payType, payValue, payCurrency, xmlPayment);
                }
            }
        }

        private static void AddPayment(MergeBlock block, string payType, double payValue, string payCurrency, XElement xmlPayment)
            {
                int npy = block.Payments != null ? block.Payments.Count() : 0;
                Payment py = new Payment();
                py.PayType = payType;
                py.PayValue = payValue;
                py.PayCurrency = payCurrency;
                MergeBlock.AddPayment(block, py);
            }

        private static void AddToDo(MergeBlock secBlock, string actionReq, string primaryChunkId,
                            string primaryTag, string secondaryTag)
        {
        int np = secBlock.ToDos != null ? secBlock.ToDos.Count() : 0;
            ToDo td = new ToDo();
            td.ActionReq = actionReq;
            td.PrimaryChunkId = primaryChunkId;
            td.PrimaryTag = primaryTag;
            td.SecondaryTag = secondaryTag;
            td.Log = string.Empty;
            td.doneRef = 0;
            MergeBlock.AddToDo(secBlock, td);
        }


        #region MergeProcessing
        public void MergeIntoPrimary(HotTag tags)
        {
            MergeInsertPersons(tags);

            MergeInsertPayments(tags);

            MergeAppendClientThatOnlyExistInSecondary(tags);

        }


        private void MergeInsertPersons(HotTag tags)
        {
            // ***********************************************************************************************
            // ** Read the Person.ToDos and append them to the client specified in the ToDo.PrimaryChunkId
            // ***********************************************************************************************

            foreach (MergeBlock secBlock in secondaryBlocks)
            {
                foreach (Person p in secBlock.Persons)
                {
                    if (p.ToDos != null)
                    {
                        foreach (ToDo td in p.ToDos)
                        {
                            if (p.HolderOrSubtantialOwner == "SubstantialOwner")
                            {
                                var xmlPriClient = xPrimary.Descendants(ftc + tags.tagReportingGroup)
                                                            .Elements(ftc + tags.tagAccBlock)
                            .Where(e => e.Element(ftc + tags.tagAccID).Value == td.PrimaryChunkId);

                                var xmlIndividuals = xmlPriClient.Descendants(ftc + tags.tagIndBlocksubowner);
                                xmlIndividuals
                                    .LastOrDefault()
                                    .AddAfterSelf(new XComment("** Individual added from Secondary File **"),
                                                    new XElement(ftc + tags.tagIndBlocksubowner,p.XmlPerson));
                            }
                        }

                    }
                }
            }
        }
        private void MergeInsertPayments(HotTag tags)
        {
            foreach (XElement eleDocCli in xPrimary.Descendants(ftc + tags.tagReportingGroup)
                                                .Elements(ftc + tags.tagAccBlock)
                .Where(e => mapPriToSec.ContainsKey(e.Element(ftc + tags.tagAccID).Value) == true))
            {
                var priAcc = eleDocCli.Element(ftc + tags.tagAccID).Value;
                string secAcc;
                if (!mapPriToSec.TryGetValue(priAcc, out secAcc))
                {
                    continue;
                }
                else
                {
                    foreach (MergeBlock secBlock in secondaryBlocks)
                    {
                        if (secBlock.ClientId == secAcc)
                        {
                            if ((secBlock.Payments != null ? secBlock.Payments.Count() : 0) > 0)
                            {
                                eleDocCli.Element(ftc + tags.tagAccBalance)                             // After Balance as Balance is a mandatory Element
                                    .AddAfterSelf(new XComment("** Inserted Payments - Start **"),
                                                    secBlock.XmlChunk.Elements(ftc + tags.tagPayment),
                                                  new XComment("** Inserted Payments - End **"));
                            }
                        }
                    }
                }
            }
        }
        private void MergeAppendClientThatOnlyExistInSecondary(HotTag tags)
        {
            var xmlPri = xPrimary.Descendants(ftc + tags.tagReportingGroup);

            xmlPri
                .Elements(ftc + tags.tagAccBlock)
                .Last()
                .AddAfterSelf(new XComment("** The following have been added from the Secondary File **"),
                                endOfTrain.xml.Descendants(ftc + tags.tagReportingGroup).Elements());
        }

        #endregion MergeProcessing

        public void SavedMergedFile(HotTag tags)
        {
            xPrimary.Save(fileMerged);
        }



    }
}
