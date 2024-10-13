# GraphQL in .NET Core

Basics of creating a GraphQL .NET server with Hot Chocolate.

## Implementation steps
1) [Add required packages](#packages)
2) [Defining the models](#models)
2) [Add a Query type](#query-type)
2) [Add GraphQL services](#graphql-service)
2) [Map the GraphQL endpoint](#graphql-endpoint)
2) [Executing a query](#executing-query)
9) [Reference](#reference)

## Packages

- `dotnet add package HotChocolate.AspNetCore`

## Models

```csharp
public class Book
{
    public string Title { get; set; }

    public Author Author { get; set; }
}

public class Author
{
    public string Name { get; set; }
}
```

## Query type

We need to define a Query type that exposes the types we have just created through a field.

The field in question is called GetBook, but the name will be shortened to just book in the resulting schema.

## GraphQL Service

The `AddGraphQLServer` returns an IRequestExecutorBuilder, which has many extension methods, similar to an IServiceCollection, that can be used to configure the GraphQL schema. 

In this example we are specifying the Query type that should be exposed by our GraphQL server.

## GraphQL Endpoint

The `MapGraphQL` method is used to map the GraphQL endpoint to a specific URL.

`app.MapGraphQL();`

## Executing Query


You should be able to open http://localhost:5000/graphql 

in your browser and be greeted by our GraphQL IDE Banana Cake Pop.


Under "Schema Definition" tab shows the schema using the raw SDL (Schema Definition Language).

```
type Query {
book: Book!
}

type Book {
title: String!
author: Author!
}

type Author {
name: String!
}
```



## Reference

reference : 
- [chillicream](https://chillicream.com/docs/hotchocolate/v13/get-started-with-graphql-in-net-core)
- [graphql](https://graphql.org/learn/)