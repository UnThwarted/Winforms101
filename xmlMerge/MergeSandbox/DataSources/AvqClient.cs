using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MergeSandbox.DataSources
{
    public class AvqClient
    {
        public string AccountID { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public Person[] Persons { get; set; }
        public override string ToString() =>
            $"{AccountID} {CompanyName}\n{Address}\n{City}, {Region} {PostalCode} {Country}\n{Phone}";
    }
    public class Person
    {
        public int PersonID { get; set; }
        public DateTime BirthDate { get; set; }
        public decimal Total { get; set; }
        public override string ToString() => $"{PersonID}: {BirthDate:d} for {Total:C2}";
    }

    public static class AvqClients
    {
        public static List<AvqClient> AvqClientList { get; } =
            (from e in XDocument.Parse(InputValues.AvaloqXml).Root.Elements("account")
             select new AvqClient
             {
                 AccountID = (string)e.Element("id"),
                 CompanyName = (string)e.Element("name"),
                 Address = (string)e.Element("address"),
                 City = (string)e.Element("city"),
                 Region = (string)e.Element("region"),
                 PostalCode = (string)e.Element("postalcode"),
                 Country = (string)e.Element("country"),
                 Phone = (string)e.Element("phone"),
                 Persons = (
                    from o in e.Elements("persons").Elements("person")
                    select new Person
                    {
                        PersonID = (int)o.Element("id"),
                        BirthDate = (DateTime)o.Element("persondate"),
                        Total = (decimal)o.Element("total")
                    }).ToArray()
             }).ToList();
    }
}
