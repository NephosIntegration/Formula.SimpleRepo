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


# Packages / Projects Used
- [Dapper](https://github.com/StackExchange/Dapper)
- [Dapper.SimpleCRUD](https://github.com/ericdc1/Dapper.SimpleCRUD)
- [Dapper.SqlBuilder](https://github.com/StackExchange/Dapper/tree/master/Dapper.SqlBuilder)
- [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)
