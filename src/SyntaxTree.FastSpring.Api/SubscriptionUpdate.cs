using System.Xml.Serialization;

namespace SyntaxTree.FastSpring.Api
{
    [XmlRoot(ElementName = "subscription", IsNullable = false, Namespace = "")]
    public sealed class SubscriptionUpdate
    {
        [XmlElement("firstName")]
        public string FirstName { get; set; }

        [XmlElement("lastName")]
        public string LastName { get; set; }

        [XmlElement("company")]
        public string Company { get; set; }

        [XmlElement("email")]
        public string Email { get; set; }

        [XmlElement("phoneNumber")]
        public string PhoneNumber { get; set; }

        [XmlElement("productPath")]
        public string ProductPath { get; set; }

        [XmlElement("quantity")]
        public int Quantity { get; set; }

        [XmlElement("tags")]
        public string Tags { get; set; }

        [XmlElement("coupon")]
        public string Coupon { get; set; }

        [XmlElement("discount-duration")]
        public string DiscountDuration { get; set; }

        [XmlElement("proration")]
        public bool Proration { get; set; }
    }
}