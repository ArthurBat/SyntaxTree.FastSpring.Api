## SyntaxTree.FastSpring.Api

SyntaxTree.FastSpring.Api is a C# library to query the [FastSpring](http://www.fastspring/) REST API.

It works on .NET 3.5, .NET 4.0 and .NET 4.5.

## API

```csharp
namespace SyntaxTree.FastSpring.Api
{
	public sealed class CompanyStore
	{
		public CompanyStore(StoreCredential credential) {}
		
		public async Task<Order> GetOrderAsync(string reference) {}
	}

	public sealed class StoreCredential
	{
		public string Company { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		
		public StoreCredential(string company, string username, string password) {}
	}
}
```

Usage sample:

```csharp
var store = new CompanyStore(
	new StoreCredential(
		company: "Microsoft",
		username: "api-user",
		password: "xxx"));

var order = await store.GetOrderAsync(reference: "SYNXXXXXX-XXXX-XXXXX");

Console.WriteLine(order.Customer.FirstName);
```

All calls made to the FastSpring server are asynchronous.
