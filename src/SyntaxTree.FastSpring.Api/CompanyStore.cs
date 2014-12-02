using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Serialization;

namespace SyntaxTree.FastSpring.Api
{
	public sealed class CompanyStore
	{
		private readonly StoreCredential _credential;

		public CompanyStore(StoreCredential credential)
		{
			if (credential == null)
				throw new ArgumentNullException("credential");

			_credential = credential;
		}

        private static T DeserializeResponse<T>(WebResponse response)
        {
            if (response == null)
                throw new InvalidOperationException("No response.");

            var responseStream = response.GetResponseStream();
            if (responseStream == null)
                throw new InvalidOperationException("Unable to acquire response stream.");

            return (T)new XmlSerializer(typeof(T)).Deserialize(responseStream);
        }

        private static string SerializeRequest<T>(T request)
        {
            if (request == null)
                throw new InvalidOperationException("No request.");

            using (var writer = new StringWriter())
            {
                new XmlSerializer(typeof(T), "").Serialize(writer, request);
                return writer.ToString()
                    .Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>", "")
                    .Replace(" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"", "")
                    .Replace("\r\n", "");
            }
        }

		public Coupon GenerateCoupon(string prefix)
		{
			if (prefix == null)
				throw new ArgumentNullException("prefix");
			if (prefix.Length == 0)
				throw new ArgumentException("Prefix is empty.", "prefix");

			var request = Request("POST", string.Concat("/coupon/", prefix, "/generate"));
			return DeserializeResponse<Coupon>(request.GetResponse());
		}

		public Order GetOrder(string reference)
		{
			if (reference == null)
				throw new ArgumentNullException("reference");
			if (reference.Length == 0)
				throw new ArgumentException("Reference is empty.", "reference");

			var request = Request("GET", "/order/" + reference);
			return DeserializeResponse<Order>(request.GetResponse());
		}

		public OrderSearchResult GetOrders(string query)
		{
			if (query == null)
				throw new ArgumentNullException("query");
			if (query.Length == 0)
				throw new ArgumentException("Query is empty.", "query");

			var request = Request("GET", "/orders/search?query=" + Uri.EscapeDataString(query));
			return DeserializeResponse<OrderSearchResult>(request.GetResponse());
		}

        public Subscription GetSubscription(string reference)
        {
            if (reference == null)
                throw new ArgumentNullException("reference");
            if (reference.Length == 0)
                throw new ArgumentException("Reference is empty.", "reference");

            var request = Request("GET", "/subscription/" + reference);
            return DeserializeResponse<Subscription>(request.GetResponse());
        }

        public bool UpdateSubscription(string reference, SubscriptionUpdate subscriptionUpdate)
        {
            if (reference == null)
                throw new ArgumentNullException("reference");
            if (reference.Length == 0)
                throw new ArgumentException("Reference is empty.", "reference");

            var request = Request("PUT", "/subscription/" + reference, SerializeRequest(subscriptionUpdate));
            try
            {
                using (var response = (HttpWebResponse) request.GetResponse())
                {
                    return response.StatusCode == HttpStatusCode.OK;
                }
            }
            catch (WebException webEx)
            {
                using (var response = (HttpWebResponse)webEx.Response)
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        throw new Exception(reader.ReadToEnd());
                    }
                }
            }

            /*
             * TODO:
             
                200: OK  on success. The response contains updated subscription data. See Get Subscription.
                403: Forbidden  if un-canceling isn't possible anymore.
                400: Bad Request  if the request was invalid. The response will contain a detailed description.
                422: Unprocessable entity  if productPath was unknown.
                412: Precondition Failed [error_code]  if it wasn't able to perform the update. Error codes: not-changeable, discount-zero-or-less, end-date-not-supported, end-date-too-early, next-period-date-empty, next-period-date-not-supported-on-demand, next-period-date-too-early, proration-not-supported-on-demand, proration-not-supported-not-started, proration-not-supported-refund-period-expired, quantity-zero-or-less, invalid-coupon, invalid-subtotal.
                500: Internal Server Error  if an unexpected server error happened.
             */
        }

        public bool CancelSubscription(string reference)
        {
            if (reference == null)
                throw new ArgumentNullException("reference");
            if (reference.Length == 0)
                throw new ArgumentException("Reference is empty.", "reference");

            var request = Request("DELETE", "/subscription/" + reference);
            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    return response.StatusCode == HttpStatusCode.OK;
                }
            }
            catch (WebException webEx)
            {
                using (var response = (HttpWebResponse)webEx.Response)
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        throw new Exception(reader.ReadToEnd());
                    }
                }
            }
        }

		private WebRequest Request(string method, string uri, string body = null)
		{
			var request = WebRequest.Create(StoreUri(uri));
			request.ContentType = "application/xml";
			request.Method = method;
			request.Headers["Authorization"] = AuthorizationHeader();

		    if (body != null)
		    {
                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] bodyBytes = encoding.GetBytes(body);

		        request.ContentLength = bodyBytes.Length;
                request.GetRequestStream().Write(bodyBytes, 0, bodyBytes.Length);
		    }

			return request;
		}

		private string AuthorizationHeader()
		{
			return "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(_credential.Username + ":" + _credential.Password));
		}

		private string StoreUri(string uri)
		{
			return "https://api.fastspring.com/company/" + _credential.Company + uri;
		}
	}
}
