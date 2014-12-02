using System.Xml.Serialization;

namespace SyntaxTree.FastSpring.Api
{
    public enum SubscriptionStatusReason
    {
        [XmlEnum("canceled-non-payment")]
        CanceledNonPayent,

        
        [XmlEnum("completed")]
        Completed,

        [XmlEnum("canceled")]
        Canceled
    }
}