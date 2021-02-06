using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MergeSandbox.DataSources
{
    public static class InputValues
    {

        // You don't really want to see this.
        public const string AvaloqXml =
@"<?xml version=""1.0""?>
<accounts>
  <account>
    <id>ALFKI</id>
    <name>Alfreds Futterkiste</name>
    <address>Obere Str. 57</address>
    <city>Berlin</city>
    <postalcode>12209</postalcode>
    <country>Germany</country>
    <phone>030-0074321</phone>
    <persons>
      <person>
        <!--scv:1234567-->
        <id>10643</id>
        <persondate>1997-08-25T00:00:00</persondate>
        <total>814.50</total>
      </person>
      <person>
        <!--scv:1234568-->
        <id>10692</id>
        <persondate>1997-10-03T00:00:00</persondate>
        <total>878.00</total>
      </person>
      <person>
        <id>10702</id>
        <persondate>1997-10-13T00:00:00</persondate>
        <total>330.00</total>
      </person>
      <person>
        <id>10835</id>
        <persondate>1998-01-15T00:00:00</persondate>
        <total>845.80</total>
      </person>
      <person>
        <id>10952</id>
        <persondate>1998-03-16T00:00:00</persondate>
        <total>471.20</total>
      </person>
      <person>
        <id>11011</id>
        <persondate>1998-04-09T00:00:00</persondate>
        <total>933.50</total>
      </person>
    </persons>
  </account>
  <account>
    <id>ANATR</id>
    <name>Ana Trujillo Emparedados y helados</name>
    <address>Avda.de la Constitución 2222</address>
    <city>México D.F.</city>
    <postalcode>05021</postalcode>
    <country>Mexico</country>
    <phone>(5) 555-4729</phone>
    <persons>
      <person>
        <!--scv:1234569-->
        <id>10308</id>
        <persondate>1996-09-18T00:00:00</persondate>
        <total>88.80</total>
      </person>
    </persons>
  </account>
  <account>
    <id>ANTON</id>
    <name>Antonio Moreno Taquería</name>
    <address>Mataderos  2312</address>
    <city>México D.F.</city>
    <postalcode>05023</postalcode>
    <country>Mexico</country>
    <phone>(5) 555-3932</phone>
    <persons>
      <person>
        <!--scv:1234544-->
        <id>10365</id>
        <persondate>1996-11-27T00:00:00</persondate>
        <total>403.20</total>
      </person>
    </persons>
  </account>
";
    }
}
