# Formula.SimpleRepo
Easy repositories for .Net built on Dapper.

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

## Step 2 - Create a Repository

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

## (Optional) Step 3 - Expose via API
The [Formula.SimpleAPI](https://github.com/NephosIntegration/Formula.SimpleAPI) project provides utilities to expose your repository as a RESTful API.


## Registering Repositories
Repositories can be registered into the depencey injection system by implementing a couple steps.  In the **ConfigureServices** section of **Startup.cs** make sure to make a call to **AddRepositories**.  Failing to do so will result in controllers depending on these respositories bing unable to resolve service for these repository types.

```c#
using Formula.SimpleRepo;
services.AddRepositories();
```

All repositories decorated with the [Repo] attribute will be injected.

# Custom Contraints
A constraint is anything you want to be able to expose querying for resources by.  By default, you can simply provide the POCO model as the constrainable definition, however, you can also provide custom / dynamic fields to allow your resource models to be constrained by.

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

# Scoped Constraints
Scoped constraints, are contraints that get applied automatically with every request.  These are applied in addition to (and instead of) any contraints applied that might be present.  These are useful for applying default constraints that need to be applied every time, and also as a strategy for limiting the scope of the data returned for security reasons, or other creative business rule purposes.  You can also programatically turn these on and off.

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

# No Query Constraints
If you wish to implement business logic constraints that will not impact the query, you can use a combination of scoped query constraints and *NoQueryConstraint* to still be able to receive input from the endpoint, but not have any bindable parameters you wish executed to against the database.
An example use case might be, based on a users request, you may want to provide a switch for the request, that may or may not require you to supply certain scoped constraints (If I'm and admin and I want to view everything, allow it, otherwise limit the scope by applying a scoped constraint).

# Packages / Projects Used
- [Dapper](https://github.com/StackExchange/Dapper)
- [Dapper.SimpleCRUD](https://github.com/ericdc1/Dapper.SimpleCRUD)
- [Dapper.SqlBuilder](https://github.com/StackExchange/Dapper/tree/master/Dapper.SqlBuilder)
- [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)
