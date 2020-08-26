# Formula.SimpleRepo
Easy repositories for .Net built on Dapper.

## Getting Started

Install the nuget package

```bash
dotnet add package Formula.SimpleRepo --version 1.0.*
```

Add a connection to your database (appsettings.json)

```json
{
    "ConnectionStrings": {
        "DefaultConnection": "Server=database.server.com;Database=MyAppDB;User=my_user;Password=my_pw!;MultipleActiveResultSets=true"
    }
}
```

### Special Instructions For Console Applications
For console applications, enable configuration and dependency injection

```bash
dotnet add package Microsoft.Extensions.Configuration
dotnet add package Microsoft.Extensions.Configuration.FileExtensions
dotnet add package Microsoft.Extensions.Configuration.Json
```

```c#
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.FileExtensions;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;

...
var config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", true, true)
        .Build();

var services = new ServiceCollection();
services.AddSingleton<IConfiguration>(config)
        .BuildServiceProvider();
```

## Creating a repository

### Step 1 - Create a model

The model represents a single record mapping between a table or view on your datasource and a POCO (plain old c# object) for use within your application.

Annotate your model with the the connection to use from your appSettings, the Table or view to use.

Various other attributes can be used to provide additional mapping assistance (such as Key, Column, GUID). *See [Dapper.SimpleCRUD](https://github.com/ericdc1/Dapper.SimpleCRUD/) for details*


```c#
using System;
using Dapper;
using Formula.SimpleRepo;

namespace MyApi.Data.Models
{
    [ConnectionDetails("DefaultConnection")]
    [Table ("Todos")]
    public class Todo
    {
        [Key]
        public int Id { get; set; }

        public String Details { get; set; }

        public Boolean Completed { get; set; }
        
        public int? CategoryId { get; set; }
    }
}
```

SqlServer is treated as the default connection type, but you can alter the database used (any `IDbConnection` implementation), as well as the dialect details in the `ConnectionDetails` attribute.

```c#
    [ConnectionDetails("DefaultConnection", typeof(DB2Connection), Dapper.SimpleCRUD.Dialect.PostgreSQL)]
    [Table("MY_WEIRD_TABLE", Schema="MY_SCHEMA")]
    public class MyNormalModel
    {
        [Dapper.Key]
        [Column("ABC#")]
        public int Id { get; set; }
    }
```

### Step 2 - Create a Repository

The repository provides simple CRUD operations ( provided by [Dapper.SimpleCRUD](https://github.com/ericdc1/Dapper.SimpleCRUD/) ), simple *constrainable* operations (query by JSON), as well as a single place to wrap business concepts into data fetch / store operations by custom function you provide.

The simplest implementation of a repository to take advantage of simple CRUD and constrainables can be implemented as follows.

```c#
using System;
using Microsoft.Extensions.Configuration;
using Formula.SimpleRepo;
using MyApi.Data.Models;

namespace MyApi.Data.Repositories
{
    [Repo]
    public class TodoRepository : RepositoryBase<Todo, Todo>
    {
        public TodoRepository(IConfiguration config) : base (config)
        {
        }
    }
}
```

### Registering Repositories

Repositories can be registered into the depencey injection system by implementing a couple steps.  In the **ConfigureServices** section of **Startup.cs** make sure to make a call to **AddRepositories**.  Failing to do so will result in controllers depending on these respositories being unable to resolve service for these repository types. 

*InvalidOperationException: Unable to resolve service for type '...' while attempting to activate '...'.*

All repositories in your project, decorated with the [Repo] attribute will be injected.

```c#
using Formula.SimpleRepo;
...
services.AddRepositories();
```

If your repositories exists in a different project / library / assembly, you can either specify the assembly directly;

```c#
// var repoAssembly = <some function to determine the assembly>
services.AddRepositories(repoAssembly);
```

Or you can pass just one example repository, and the rest will be found.

```c#
services.AddRepositories(typeof(MyOtherProject.Data.MyRepository));
```

> **Note** - If using this strategy, you do not need to add each new repository, you only have to decorate them with the [Repo] annotation.


### Step 3 - Work with data

Now you can perform all queries and CRUD operations against the models.
For dynamic / business defined constrainables see further steps below.
But you now have enough to perform basic operations against your data.

Example...

```c#
var repo = new TodoRepository(config);
foreach(var item in repo.Get())
{
    Console.WriteLine(item.Details);
}
```

#### What can you do?

**Read Operations**

There are async versions of all methods.

***Get / GetAsync**- Fetch data*

```c#
// Get a single item by it's ID
var record = repo.Get(21); // Can be number or GUID

// Get all records
var records = repo.Get();

// Get by specific fields using JSON to define your constraints
records = repo.Get("{Completed:true}"); 

// Get by hash table
records = repo.Get(new Hashtable() { { "Completed", true } });

// Additional methods like fetching via JObject, Bindable (advanced concept), etc..

// Get a list of all the identity columns
var idFields = repo.GetIdFields();


// You also have access to other operations provided by SimpleCRUD via the Basic property
// These do not apply non database / dynamic constrainable concepts described below
records = repo.Basic.GetList("where column_name like '%asdf%'");

var pagedData = repo.Basic.GetListPaged(1, 10, "where column_a = 'asdf'");

var recordCount = repo.Basic.RecordCount("where column_name like '%asdf%'");

// etc...
```

**Dapper.SimpleCRUD**
You still have access to all the dapper SimpleCRUD you are used too.


**CRUD Operations**

Like above, there are async versions of all methods.

```c#
// Delete
repo.Delete(21); // Can be number or GUID

// Insert
var modelToSave = new Todo() {
    Details = "Do a backflip",
    Completed = false,
};

var newId = repo.Insert(modelToSave);

// Update
var modelToSave = new Todo() {
    Id = 21,
    Details = "Do a backflip",
    Completed = true,
};

repo.Update(modelToSave);

// Like with the basic query operations, you also still have access to other basic
// operations that don't apply to the constrainable types.

repo.Basic.DeleteList("where yadda yadda...");

```

### Step 5 - Advanced Topics

### Custom Contraints

A constraint is anything you want to be able to expose querying for resources by.  By default, you can simply provide the POCO model as the constrainable definition, however, you can also provide custom / dynamic fields to allow your resource models to be constrained by.

You can now start defining new columns on your repository that don't exist as columns in your database, which represent "concepts".  These further allow you to constrain your data.

The implementation of this is explained in implementation details below.  For now, try to understand the concept.  If you can define additional ways to represent a record / resource, you can then begin querying  against it as if it were a column in the database.

Consider that you have a date column, you might want to be able to fetch records that have dates in the past or in the future, or in the past, or other concepts which are hard to express without coding logic (paydays, near me geographically, if certain things exist on file systems, etc..).  You can implement concepts that depend on other criteria outside of things which can be expressed in your database.

For things like this, adding custom constraints are the solution.

```c#
var records = repo.Get(new Hashtable() { { "FutureRecords", true } });
...
var records = repo.Get(new Hashtable() { { "NearMe", true } });
...
var records = repo.Get(new Hashtable() { { "AtStockPoint", myVariable } });
```

Example.. 

Suppose we wanted to allow our todo list to be able to be queried by a particular keyword found in the details of the todo.

We can create a class extending the Todo model defining a new constraint called DetailsLike.
We first must define this new constraint (by extending Contraint), and we must implement the Bind method that handles building the query for values.

```c#
public class DetailsLike : Constraint
{
    public override Dictionary<String, Object> Bind(Dapper.SqlBuilder builder)
    {
        var parameters = new Dictionary<String, Object>();

        builder.Where("UPPER(Details) like @DetailsLike");
        parameters.Add("DetailsLike", "%" + this.Value.ToString().ToUpper() + "%");

        return parameters;
    }
}
```

Since we want to allow constraint binding against all of the current properties on our model, we can extend our Todo model and add an aditional field called DetailsLike.

```c#
public class TodoConstraints : Todo 
{
    public DetailsLike DetailsLike { get; set; }
}
```

We can now pass our custom constraints (which include our dynamic custom constraint DetailsLike) to our repository as the second template supplied to the RepositoryBase.

```c#
public class TodoRepository : RepositoryBase<Todo, TodoConstraints>
```

### Scoped Constraints

You might also only want certain records to be returned based on some certain "scope".  Scoped constraints, are contraints that get applied automatically with every request.  These are applied in addition to (and instead of) any contraints applied that might be present.  These are useful for applying default constraints that need to be applied every time, and also as a strategy for limiting the scope of the data returned for security reasons, or other creative business rule purposes.  You can also programatically turn these on and off.

For example, if I want to limit the scope of data, so that users can only see their data based on their user id, I can apply a scoped constraint.  This way if a request for all data, is made, the server side can limit the results.


On your repository implementation override the **ScopedConstraints** function.  
The input for this fucntion is all the currently applied constraints that are being applied (which could be useful for various business rule purposes).
The expected output of this function is a list of contraints you want to apply (or override).  You can be explicit on the behavior and introduce totally new constraints that have even been created on your model, or you can create a hashtable to match contraints on your model and call the **GetConstraints** function to have them generated using the design of the model based constraints (or custom constrains you have previously designed).  

```c#
public override List<Formula.SimpleRepo.Constraint> ScopedConstraints(List<Formula.SimpleRepo.Constraint> currentConstraints)
{
    var constraints = new Hashtable();
    constraints.Add("UserName", this._userId);
    return this.GetConstraints(constraints);
}

```

Example use;

```c#
var constraints = new Hashtable();
constraints.Add("Active", true);
_myRepositoryInstance.Get(constraints);
```

Will result in all active records, assigned to the currently logged in user, because my repository is limiting the scope by logged in user.

If you wish to temporarily disable the scoped constraints you can call the **RemoveScopedConstraints** on your repository before calling Get.

```c#
var constraints = new Hashtable();
constraints.Add("Active", true);
_myRepositoryInstance.RemoveScopedConstraints().Get(constraints);
```

This will result in all active records, regardless of the logged in user.

### No Query Constraints

If you wish to implement business logic constraints that will not impact the query, you can use a combination of scoped query constraints and *NoQueryConstraint* to still be able to receive input from the endpoint, but not have any bindable parameters you wish executed to against the database.
An example use case might be, based on a users request, you may want to provide a switch for the request, that may or may not require you to supply certain scoped constraints (If I'm and admin and I want to view everything, allow it, otherwise limit the scope by applying a scoped constraint)

### NULL constraints

Constraints are treated as "IS NULL" in one of 3 ways.

* Explicitly
* Verbosely
* Implicitly / Assumed

> See the `IsNullComparison` function in `Constriant.cs` for implementation.

In each case a constraint that is determined to be null will produce **"WHERE x IS NULL"**.

To explicitly set a constraint to null, leave the value null.

```c#
constraints.Add("MyValue", null);
```

To Verbosely set a constraint to null, use the word `NULL` as a string.

```c#
constraints.Add("MyValue", "NULL");
```

The implicitly / assumed null is a programatic decision within the library.  So this is the least desirable means of producing a null constraint.  
If a value is considered to be `empty` for a datatype that doesn't support empty *(such as strings)*, it will be assumed this is to be a null constraint.

```c#
constraints.Add("MyValue", ""); // Where MyValue is an int and "" is an empty value
```

### (Optional) Step 5 - Expose via API
The [Formula.SimpleAPI](https://github.com/NephosIntegration/Formula.SimpleAPI) project provides utilities to expose your repository as a RESTful API.

----

# Packages / Projects Used
- [Dapper](https://github.com/StackExchange/Dapper)
- [Dapper.SimpleCRUD](https://github.com/ericdc1/Dapper.SimpleCRUD)
- [Dapper.SqlBuilder](https://github.com/StackExchange/Dapper/tree/master/Dapper.SqlBuilder)
- [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)
