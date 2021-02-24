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
    public class dsMerge
    {
        // Can the DataSet.Merge method help us ?  Alas XSD won't have the relationships and constraints needed

        public DataSet dsFirst = new DataSet();
        public DataSet dsSecond = new DataSet();

        public void Test(string masterPath, string legacyPath)
        {
            // read xmls
            dsFirst.ReadXml(masterPath);
            dsSecond.ReadXml(legacyPath);

            dsFirst.Merge(dsSecond);

            var result = Path.ChangeExtension(masterPath, ".1.xml");
            var fs = new FileStream(result, FileMode.CreateNew);
            StreamWriter newXml = new StreamWriter(fs);
            dsFirst.WriteXml(newXml);
        }
    }
}
