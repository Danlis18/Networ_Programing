/* 
 Licensed under the Apache License, Version 2.0

 http://www.apache.org/licenses/LICENSE-2.0
 */
using System;
using System.Xml.Serialization;
using System.Collections.Generic;
namespace _008_HTTP
{
    [XmlRoot(ElementName = "region")]
    public class Region
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "title")]
        public string Title { get; set; }
    }

    [XmlRoot(ElementName = "city")]
    public class City
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "title")]
        public string Title { get; set; }
    }

    [XmlRoot(ElementName = "link")]
    public class Link
    {
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "href")]
        public string Href { get; set; }
    }

    [XmlRoot(ElementName = "n")]
    public class N
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "br")]
        public string Br { get; set; }
        [XmlAttribute(AttributeName = "ar")]
        public string Ar { get; set; }
    }

    [XmlRoot(ElementName = "metal")]
    public class Metal
    {
        [XmlElement(ElementName = "n")]
        public List<N> N { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "metals")]
    public class Metals
    {
        [XmlElement(ElementName = "metal")]
        public List<Metal> Metal { get; set; }
    }

    [XmlRoot(ElementName = "organization")]
    public class Organization
    {
        [XmlElement(ElementName = "title")]
        public string Title { get; set; }
        [XmlElement(ElementName = "region")]
        public Region Region { get; set; }
        [XmlElement(ElementName = "city")]
        public City City { get; set; }
        [XmlElement(ElementName = "phone")]
        public string Phone { get; set; }
        [XmlElement(ElementName = "address")]
        public string Address { get; set; }
        [XmlElement(ElementName = "link")]
        public Link Link { get; set; }
        [XmlElement(ElementName = "metals")]
        public Metals Metals { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "organizations")]
    public class Organizations
    {
        [XmlElement(ElementName = "organization")]
        public List<Organization> Organization { get; set; }
    }

    [XmlRoot(ElementName = "nominal")]
    public class Nominal
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "nominals")]
    public class Nominals
    {
        [XmlElement(ElementName = "nominal")]
        public List<Nominal> Nominal { get; set; }
    }

    [XmlRoot(ElementName = "regions")]
    public class Regions
    {
        [XmlElement(ElementName = "region")]
        public List<Region> Region { get; set; }
    }

    [XmlRoot(ElementName = "cities")]
    public class Cities
    {
        [XmlElement(ElementName = "city")]
        public List<City> City { get; set; }
    }

    [XmlRoot(ElementName = "source")]
    public class Source
    {
        [XmlElement(ElementName = "organizations")]
        public Organizations Organizations { get; set; }
        [XmlElement(ElementName = "metals")]
        public Metals Metals { get; set; }
        [XmlElement(ElementName = "nominals")]
        public Nominals Nominals { get; set; }
        [XmlElement(ElementName = "regions")]
        public Regions Regions { get; set; }
        [XmlElement(ElementName = "cities")]
        public Cities Cities { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "date")]
        public string Date { get; set; }
    }

}
