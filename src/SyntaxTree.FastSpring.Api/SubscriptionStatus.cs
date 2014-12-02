using System.Xml.Serialization;

namespace SyntaxTree.FastSpring.Api
{
    public enum SubscriptionStatus
    {
        [XmlEnum("active")]
        Active,

        [XmlEnum("inactive")]
        Inactive
    }
}