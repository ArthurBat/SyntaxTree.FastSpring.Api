using System;
using System.Xml.Serialization;

namespace SyntaxTree.FastSpring.Api
{
    [XmlRoot(ElementName = "subscription", IsNullable = false, Namespace = "")]
    public sealed class Subscription
    {
        [XmlElement("status")]
        public Status Status { get; set; }

        [XmlElement("statusChanged")]
        public DateTime StatusChanged { get; set; }

        [XmlElement("statusReason")]
        public string StatusReason { get; set; }

        [XmlElement("cancelable")]
        public bool Cancelable { get; set; }

        [XmlElement("reference")]
        public string Reference { get; set; }

        [XmlElement("test")]
        public bool Test { get; set; }

        [XmlElement("referrer")]
        public string Referrer { get; set; }

        [XmlElement("sourceName")]
        public string SourceName { get; set; }

        [XmlElement("sourceKey")]
        public string SourceKey { get; set; }

        [XmlElement("sourceCampaign")]
        public string SourceCampaign { get; set; }

        [XmlElement("customer")]
        public Contact Customer { get; set; }

        [XmlElement("customerUrl")]
        public string CustomerUrl { get; set; }

        [XmlElement("productName")]
        public string ProductName { get; set; }

        [XmlElement("tags")]
        public string Tags { get; set; }

        [XmlElement("quantity")]
        public int Quantity { get; set; }

        [XmlElement("coupon")]
        public string Coupon { get; set; }

        [XmlElement("nextPeriodDate")]
        public DateTime NextPeriodDate { get; set; }

        [XmlElement("end")]
        public DateTime End { get; set; }
    }
}