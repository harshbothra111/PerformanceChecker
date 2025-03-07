using BenchmarkDotNet.Attributes;
using System;
using System.Xml;

namespace PerformanceChecker.Benchmark.Benchmarks
{
    [ReturnValueValidator]
    public class XmlReaderBenchmark : BaseBenchmark
    {
        private string _xml = string.Empty;

        [Params("FirstName", "Department", "FavoriteDish")]
        public string? ElementName { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            _xml =
                @"
                    <Data>
                        <FirstName>John</FirstName>
                        <LastName>Doe</LastName>
                        <Age>30</Age>
                        <Gender>Male</Gender>
                        <Email>johndoe@example.com</Email>
                        <PhoneNumber>123-456-7890</PhoneNumber>
                        <Address>123 Main St, City, Country</Address>
                        <City>New York</City>
                        <State>NY</State>
                        <Country>USA</Country>
                        <ZipCode>10001</ZipCode>
                        <JobTitle>Software Engineer</JobTitle>
                        <Company>Tech Corp</Company>
                        <Salary>100000</Salary>
                        <Department>IT</Department>
                        <Hobby>Reading</Hobby>
                        <FavoriteColor>Blue</FavoriteColor>
                        <BloodType>O+</BloodType>
                        <MaritalStatus>Single</MaritalStatus>
                        <Nationality>American</Nationality>
                        <PassportNumber>A12345678</PassportNumber>
                        <DrivingLicense>DL123456</DrivingLicense>
                        <BankName>ABC Bank</BankName>
                        <AccountNumber>9876543210</AccountNumber>
                        <IBAN>US12345678901234567890</IBAN>
                        <SwiftCode>ABCDEF12345</SwiftCode>
                        <VehicleType>Car</VehicleType>
                        <VehicleNumber>XYZ-1234</VehicleNumber>
                        <PetName>Buddy</PetName>
                        <FavoriteDish>Pizza</FavoriteDish>
                    </Data>
                ";
        }

        [Benchmark]
        public string ReadDataUsingString()
        {
            return _xml.Substring(
                _xml.IndexOf($"<{ElementName}>") + $"<{ElementName}>".Length, 
                _xml.IndexOf($"</{ElementName}>") - _xml.IndexOf($"<{ElementName}>") - $"<{ElementName}>".Length
                );
        }

        [Benchmark]
        public string ReadDataUsingSpan()
        {
            ReadOnlySpan<char> xmlSpan = _xml.AsSpan();
            int startIndex = xmlSpan.IndexOf($"<{ElementName}>".AsSpan(), StringComparison.Ordinal) + $"<{ElementName}>".Length;
            int endIndex = xmlSpan.IndexOf($"</{ElementName}>".AsSpan(), StringComparison.Ordinal);
            return xmlSpan[startIndex..endIndex].ToString();
        }

        [Benchmark]
        public string ReadDataUsingXmlDocument()
        {
            var doc = new XmlDocument();
            doc.LoadXml(_xml);
            return doc.GetElementsByTagName(ElementName!)[0]!.InnerText;
        }

        [Benchmark]
        public string ReadDataUsingXmlReader()
        {
            using var reader = XmlReader.Create(new StringReader(_xml));
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == ElementName)
                {
                    return reader.ReadElementContentAsString();
                }
            }
            return string.Empty;
        }

        [Benchmark]
        public string ReadDataUsingXmlReaderTillElement()
        {
            using var reader = XmlReader.Create(new StringReader(_xml));
            return GetElementValue(reader, ElementName!);
        }

        private static string GetElementValue(XmlReader reader, string elementName)
        {
            _ = reader.ReadToFollowing(elementName);
            return reader.ReadElementContentAsString();
        }       
    }
}
