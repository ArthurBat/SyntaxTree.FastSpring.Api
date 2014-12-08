using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
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

		public async Task<Coupon> GenerateCoupon(string prefix)
		{
			if (prefix == null)
				throw new ArgumentNullException("prefix");
			if (prefix.Length == 0)
				throw new ArgumentException("Prefix is empty.", "prefix");

			var request = await Request("POST", string.Concat("/coupon/", prefix, "/generate"));
			return DeserializeResponse<Coupon>(await request.GetResponseAsync());
		}

		public async Task<Order> GetOrder(string reference)
		{
			if (reference == null)
				throw new ArgumentNullException("reference");
			if (reference.Length == 0)
				throw new ArgumentException("Reference is empty.", "reference");

            var request = await Request("GET", "/order/" + reference);
            return DeserializeResponse<Order>(await request.GetResponseAsync());
		}

		public async Task<OrderSearchResult> GetOrders(string query)
		{
			if (query == null)
				throw new ArgumentNullException("query");
			if (query.Length == 0)
				throw new ArgumentException("Query is empty.", "query");

            var request = await Request("GET", "/orders/search?query=" + Uri.EscapeDataString(query));
            return DeserializeResponse<OrderSearchResult>(await request.GetResponseAsync());
		}

        public async Task<Subscription> GetSubscription(string reference)
        {
            if (reference == null)
                throw new ArgumentNullException("reference");
            if (reference.Length == 0)
                throw new ArgumentException("Reference is empty.", "reference");

            var request = await Request("GET", "/subscription/" + reference);
            return DeserializeResponse<Subscription>(await request.GetResponseAsync());
        }

        public async Task UpdateSubscription(string reference, SubscriptionUpdate subscriptionUpdate)
        {
            if (reference == null)
                throw new ArgumentNullException("reference");
            if (reference.Length == 0)
                throw new ArgumentException("Reference is empty.", "reference");

            var request = await Request("PUT", "/subscription/" + reference, SerializeRequest(subscriptionUpdate));
            try
            {
                using (var response = (HttpWebResponse)await request.GetResponseAsync())
                {
                    if (!response.StatusCode.ToString().StartsWith("2"))
                    {
                        throw new Exception(response.StatusDescription);
                    }
                }
            }
            catch (WebException webEx)
            {
                using (var response = (HttpWebResponse)webEx.Response)
                {
                    string errorData = "";
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        errorData = reader.ReadToEnd();
                    }

                    switch ((int)response.StatusCode)
                    {
                        case 400:
                            throw new Exception(string.Format("Invalid request. {0}", errorData));
                        case 403:
                            throw new Exception("Un-canceling the subscription is not possible anymore.");
                        case 412:
                            throw new Exception(string.Format("Precondition failed. {0} {1}", response.StatusDescription, errorData));
                        case 422:
                            throw new Exception("Product path is unknown.");
                        case 500:
                            throw new Exception("Unknown error.");
                    }
                    
                }
            }
        }

        public async Task CancelSubscription(string reference)
        {
            if (reference == null)
                throw new ArgumentNullException("reference");
            if (reference.Length == 0)
                throw new ArgumentException("Reference is empty.", "reference");

            var request = await Request("DELETE", "/subscription/" + reference);
            try
            {
                using (var response = (HttpWebResponse)await request.GetResponseAsync())
                {
                    if (!response.StatusCode.ToString().StartsWith("2"))
                    {
                        throw new Exception(response.StatusDescription);
                    }
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

		private async Task<WebRequest> Request(string method, string uri, string body = null)
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
		        var requestStream = await request.GetRequestStreamAsync();
		        requestStream.Write(bodyBytes, 0, bodyBytes.Length);
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
