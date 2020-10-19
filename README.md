I started this because I got fed up with Azure Table Storage. It feels too complicated to me, and I thought it would be a good exercise to see if I could implement the functionality and experience I was looking for. This is a good way to see if the complexity is justified and to get a deeper understanding of the issues in play. Here's the kind of code I'm wanting to write:

```csharp
var client = new CloudObjectsClient("myaccount", "apikey");

// save something to the cloud
await client.CreateAsync("my stuff/object1", new Something() 
{
    FirstName = "whoever",
    LastName = "whatever",
    Date = DateTime.Today,
    Price = 234.34m
});

// get object from cloud
var obj = await client.GetAsync<Something>("my stuff/object1");
```

## My Approach
My solution has three core projects:
- [CloudObjects.App](https://github.com/adamfoneil/CloudObjects/tree/master/CloudObjects.App) a .NET Core web app using the API template. This uses a SQL Server backend and my [Dapper.CX](https://github.com/adamfoneil/Dapper.CX) project for all the data operations.
- [CloudObjects.Client](https://github.com/adamfoneil/CloudObjects/tree/master/CloudObjects.Client) the NuGet package, the part you'd install and use in your programs.
- [CloudObjects.Models](https://github.com/adamfoneil/CloudObjects/tree/master/CloudObjects.Models) the model classes shared by the App and Client projects.

### Note about linked source files
I do some seemingly oddball things with the Model project dependency. Instead of the App and Client projects having a project reference to the Model class project, I used linked source files, for example [these](https://github.com/adamfoneil/CloudObjects/blob/master/CloudObjects.Client/CloudObjects.Client.csproj#L15-L19). There are a couple reasons for this. One, it makes the NuGet package deployment easier. Project references aren't included in a NuGet package. So, I would end up needing to release my referenced project in a separate package. This would complicate the release management, and I don't need more complexity. Two, linked source files work better when you have things like server side validation and auditing that require database connectivity. Using partial classes with clear dependency separation lets me keep the database dependency out of the Client project, where it doesn't belong anyway. (The Client package never opens a direct database connection.)

### Note about Testing projects
There are two test projects [Testing.App](https://github.com/adamfoneil/CloudObjects/tree/master/Testing.App) and [Testing.Client](https://github.com/adamfoneil/CloudObjects/tree/master/Testing.Client). I had to separate them because of how the Model classes are linked in both projects. If I had a single test project, then the Model classes would be linked twice, which would offend the C# compiler.
