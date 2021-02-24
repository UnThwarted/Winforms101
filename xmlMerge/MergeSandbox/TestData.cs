using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MergeSandbox
{
    public class TestData
    {
        // Create first xml document
        public XDocument xmlAvaloq = new XDocument(
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

        // Create first xml document
        public XDocument xmlLegacy = new XDocument(
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

    }
}
