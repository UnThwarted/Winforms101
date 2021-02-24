using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MergeSandbox
{
    public class MergeBlock
    {
        public int BlockId { get; set; }
        public string ClientId { get; set; }

        public XElement XmlChunk { get; set; }

        public string BalCurrency { get; set; }
        public double BalValue { get; set; }
        public List<Person> Persons { get; set; }
        public List<Payment> Payments { get; set; }
        public List<ToDo> ToDos { get; set; }

        public string ActionReq { get; set; }

        // "new" "assessed" "done"
        public string State { get; set; }

        public static int nextBlockId = 0;

        public MergeBlock()
        {
            BlockId = 0;
            ClientId = string.Empty;
            XmlChunk = null;
            BalCurrency = string.Empty;
            BalValue = 0;
            Persons = null;
            Payments = null;
            ToDos = null;
            ActionReq = string.Empty;
            State = "new";
        }
        public static void AddPerson(MergeBlock mb, Person ps)
        {
           if (mb.Persons == null)
            {
                List<Person> psns = new List<Person>();
                psns.Add(ps);
                mb.Persons = psns;
            }
            else
            {
                mb.Persons.Add(ps);
            }
        }

        public static void AddPayment(MergeBlock mb, Payment ps)
        {
            if (mb.Payments == null)
            {
                List<Payment> pays = new List<Payment>();
                pays.Add(ps);
                mb.Payments = pays;
            }
            else
            {
                mb.Payments.Add(ps);
            }
        }

        public static void AddToDo(MergeBlock mb, ToDo ps)
        {
            if (mb.ToDos == null)
            {
                List<ToDo> tds = new List<ToDo>();
                tds.Add(ps);
                mb.ToDos = tds;
            }
            else
            {
                mb.ToDos.Add(ps);
            }
        }

    }

    public class Payment
    {
        public string PayCurrency { get; set; }
        public string PayType { get; set; }
        public double PayValue { get; set; }
        public List<ToDo> ToDos { get; set; }
        public override string ToString() => $"{PayType}: {PayValue:C2} {PayCurrency}";
    }

    public class Person
    {
        public string HolderOrSubtantialOwner { get; set; }
        public string OrgOrIndividual { get; set; }
        public string PersonId { get; set; }
        public string BirthDate { get; set; }
        public string SCV { get; set; }
        public string FullName { get; set; }
        public XElement XmlPerson { get; set; }
        public List<ToDo> ToDos { get; set; }
        public override string ToString() => $"{PersonId}: {BirthDate:d}";

        public Person()
        {
            HolderOrSubtantialOwner = string.Empty;
            OrgOrIndividual = string.Empty;
            PersonId = string.Empty;
            BirthDate = string.Empty;
            SCV = string.Empty;
            FullName = string.Empty;
        }

        public Person(Person previousPerson)
        {
            HolderOrSubtantialOwner = previousPerson.HolderOrSubtantialOwner;
            OrgOrIndividual = previousPerson.OrgOrIndividual;
            PersonId = string.Empty;
            BirthDate = string.Empty;
            SCV = string.Empty;
            ToDos.AddRange(previousPerson.ToDos);
        }

    }

    public class ToDo
    {
        // Action required "EndTrain" "Append", "addValue", "subtractValue", "none"
        public string ActionReq { get; set; }

        public string PrimaryChunkId { get; set; }

        public string PrimaryTag { get; set; }

        public string SecondaryTag { get; set; }

        public string Log { get; set; }

        public int doneRef { get; set; }

        public ToDo()
        {
            ActionReq = string.Empty;
            PrimaryChunkId = string.Empty;
            PrimaryTag = string.Empty;
            SecondaryTag = string.Empty;
            Log = string.Empty;
            doneRef = 0;
        }     
    }

}
