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
        public int blockId { get; set; }
        public string clientId { get; set; }

        public XElement xmlChunk { get; set; }

        public string balCurrency { get; set; }
        public double balValue { get; set; }
        public Person[] Persons { get; set; }
        public Payment[] Payments { get; set; }
        public ToDo[] ToDos { get; set; }

        public string actionReq = string.Empty;

        // "new" "assessed" "done"
        public string state = "new";

        public static int nextBlockId = 0;


    }

    public class Payment
    {
        public string PayCurrency { get; set; }
        public string PayType { get; set; }
        public double PayValue { get; set; }
        public ToDo[] ToDos { get; set; }
        public override string ToString() => $"{PayType}: {PayValue:C2} {PayCurrency}";
    }

    public class Person
    {
        public string PersonId { get; set; }
        public string BirthDate { get; set; }
        public string SCV { get; set; }
        public ToDo[] ToDos { get; set; }
        public override string ToString() => $"{PersonId}: {BirthDate:d}";
    }

    public class ToDo
    {
        // Action required "EndTrain" "Append", "addValue", "subtractValue", "none"
        public string actionReq         = string.Empty;

        public string PrimaryChunkId    = string.Empty;

        public string PrimaryTag        = string.Empty;

        public string SecondaryTag      = string.Empty;

        public string log               = string.Empty;

        public int      doneRef         = 0;
        
    }

}
