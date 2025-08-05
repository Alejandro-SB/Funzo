[![NuGet](https://img.shields.io/nuget/v/Funzo?label=Funzo)](https://www.nuget.org/packages/Funzo/)
[![NuGet](https://img.shields.io/nuget/v/Funzo.Serialization?label=Funzo.Serialization)](https://www.nuget.org/packages/Funzo.Serialization/)
[![NuGet](https://img.shields.io/nuget/v/Funzo.SourceGenerators?label=Funzo.SourceGenerators)](https://www.nuget.org/packages/Funzo.SourceGenerators/)

# Funzo

## Contents
- [Where is this coming from](#where-is-this-coming-from)
- [Description](#description)
- [Usage (recommended way)](#usage-recommended-way)
- [Documentation](#documentation)
    - [Unit](#unit)
    - [Option](#option)
    - [Result](#result)
    - [ResultBuilder](#resultbuilder)
    - [Union](#union)
- [Serialization](#serialization)
- [Example](#example)
- [Further work](#further-work)
- [Design Philosophy](#design-philosophy)
- [What's missing (updated after adding union types & source generators)](#whats-missing-updated)

## Where is this coming from
This package was previously called OptionTypes, but given that now more things are being added, I think a new name could fit better. Wanted to make it sound like something functional, so Funzo it is. Sounds fine.
Apart from the name, the `Maybe<T>` class was renamed to `Option<T>`. It is more common than `Maybe<T>` so I thought it would be better for people to identify it. I ~~copied a lot of things~~ took inspiration of the work done by [`OneOf`](https://github.com/mcintyre321/OneOf) (specially source generators) but decided to adapt it to my way of doing things: **making everything explicit**.

## Description
_Funzo_ allows the developer to use some classes more commonly used in functional programming for error-proof programming and better type definition. It contains 4 classes:

- The `Unit` class represents an empty class. Because functions always return something, `Unit` is the equivalent to `void`

- The `Option<T>` class allows to create an item of type T that may have no value. This value cannot be accessed in an unsafe manner by design, making really easy to completely remove null references from your code and reducing the number of `NullReferenceException` exceptions thrown.

- The `Result<TOk, TErr>` class represents an operation that has been completed. This reduces the number of `try/catch` blocks needed to manage application flow, making error management explicit and ending with more reliable code.

- The `Union<T1, T2>` and all its siblings allows you to create union types. This is pretty similar to the [`OneOf`](https://github.com/mcintyre321/OneOf) library (in fact, heavily inspired in it), but here you will **NOT** be able to get values without checking first.

- The attributes `ResultAttribute` and `UnionAttribute` allow you to generate types and use them with ease.


## Usage (recommended way)
The best way to approach this package is through source generators. This will allow you to define your types with ease and work on your problems straight away.
You will need the `Funzo` and `Funzo.SourceGenerators` packages.
For unions, decorate it with the `Union` attribute, supplying the generic parameters you want as possible options.
For results, decorate it with the attribute `Result`, using generics for the Error and Ok variant, if needed.
In both cases, the class should be partial so new code can be generated.

```Csharp
[Union<Error1, Error2, Error3>]
public partial class ApiError;

[Result<ApiError>]
public partial class ApiCallResult;

[Union<ItemWithNameAlreadyExists, InvalidItemName, EmptyItemName>]
public partial class ItemCreationError;

[Result<ItemId, ItemCreationError>]
public partial class ItemCreatedResult;
```


## Documentation

### Unit
Unit is a helper type to represent the absence of a return value (think of it as void). Because in functional programming every function returns a value, it is added here for compatibility.

### Option

Create a new Option using one of the helper methods (`Option.FromValue`, `Option.Some`, and `Option.None`):
```Csharp
var optionInt = Option.Some(1);
var optionFloat = Option.FromValue(12);
string? nullableString = null;
var optionString = Option.FromValue(nullableString);
```

Map its content using the `Map` method:
```Csharp
var optionText = await ReadTextFromFile(filePath);

var uppercaseText = optionText.Map(text => text.ToUpper());
```

You can also map to another `Option` and it will be flatten:
```Csharp
var number = Option.FromValue(1);

// double type is Option<int>
var double = number.Map(x => Option.FromValue(x * 2));
```

If you want to check both options, use the `Match` method:
```Csharp
let user = await GetUser();


var userName = user.Match(x => x.Name, () => "User not found");
```

In case you want to provide a fallback value, you can use `ValueOr`:
```Csharp
var optionUserName = await GetUserName();
var userName = optionUserName.ValueOr("Unknown user");
```

In case you want to do something if there is a value present, you can use the `IsSome` method:
```Csharp
Option<User> optionUser = await GetUser();

if(!optionUser.IsSome(out User user))
{
    return Results.NotFound();
}

var posts = postService.GetPostsByUserId(user.Id);
```

You can force the value out using the `Unwrap` method. This approach is **not recommended**:
```Csharp
var optionValue = Option.Some(1);

var value = optionValue.Unwrap() // 1

var optionString = Option<string>.None;
optionString.Unwrap(); // throws NullReferenceException
```

There are also extension methods for `Task<Option<T>>` so you can chain `Map`, `Match`, `ValueOr`, and `Unwrap` to your tasks.
```Csharp
var userBalance = GetUser() // GetUser returns a Task<Option<User>>
                    .Map(user => bankService.GetAccounts(user.Id))
                    .Map(accounts => accounts.Sum(a => a.Balance))
                    .ValueOr(0m);
```

In case you want to do something with the value first, you can use the `Inspect` function:
```Csharp
Option<User> maybeUser = GetUser();

user.Inspect(user => Console.WriteLine($"Retrieved user {user.Name}");
```

### Result
To use results, you can use the already existing classes `Result<TOk, TErr>` or `Result<TErr>`, depending on whether you need an Ok value, or use the source generators (much recommended, see above).

You can create an instance using the Ok/Err static methods:
```Csharp
var okResult = Result<ProcessError>.Ok();
var errorResult = Result<Unit, ProcessError>.Err(ProcessError.DatabaseConnection);
```

You can map the ok value or err value using `Map` and `MapErr` methods:
```Csharp
var okResult = Result<int, Exception>.Ok(3).Map(x => x*2)); // Ok(6)
var errResult = Result<string>.Err("failure").MapErr(x => x.ToUpper()); // Err("FAILURE")
```

To provide handlers for both cases, which should be the normal usage, use the `Match` method:
```Csharp
var result = await CreateUser();

result.Match(
    user => Results.Created("/user", { id: user.Id }),
    err => err switch {
        CreateUserError.EmailExists => Results.Conflict(),
        _ => Results.BadRequest()
    });
```

Sometimes you only want to know if an operation has completed successfully to get the ok value. You can use the `AsOk` method:
```Csharp
var parsingResult = ParseLines(path);

var linesParsed = parsingResult.AsOk().ValueOr(0);

Console.WriteLine($"Parsed {linesParsed} lines");
```

In order to get early returns when needed, there is an `IsErr` method:
```Csharp
Result<User, string> userCreationResult = await CreateUser(userPayload);

if (userCreationResult.IsErr(out User user, out string error))
{
    return Result<Unit, UserCreationError>.Err(UserCreationError.CannotCreateUser);
}

var userRoleResult = await AssignRoles(user, Roles.Admin);

if (userRoleResult.IsErr(out var roleError)) 
{
    return Result<Unit, UserCreationError>.Err(UserCreationError.CannotAssignRole);
}

emailService.NotifyUser(user.Email);
```

As with `Option`, there are some extensions in Task to be able to chain methods:
```Csharp
public async IResult Post([FromBody] UserPayload payload)
    => await CreateUser(payload).Match<IResult>(
        user => Results.Created("/user", { id: user.Id }),
        err => err switch {
            CreateUserError.EmailExists => Results.Conflict(),
            _ => Results.BadRequest()
        });
```

Same as `Option`, we have 2 methods: `Inspect` and `InspectErr` (and their async variants) in case you want to do something with the values without actually changing anything.

### ResultBuilder
Created by using the static method `For`, lets you map different exceptions to errors, allowing you to migrate from an exception based approach to a result based approach.

So you can move from this:
```Csharp
public async Task Main()
{
    try
    {
        await ReserveTable(userId, reservationDate);
    }
    catch(Exception e)
    {
        // Manage
    }
}

public Task<int> ReserveTable(int userId, DateTime reservationDate)
{
    if(reservationDate.TimeOfDay > TimeSpan.FromHours(22))
    {
        throw new RestaurantClosedException("Restaurant is closed at 22:00h");
    }

    var freeTables = await GetFreeTables(reservationDate);

    if(freeTables is null || freeTables.Count == 0)
    {
        throw new RoomFullException("There are no free tables");
    }

    var reservation = new Reservation(userId, reservationDate);
    context.Reservations.Add(reservation);

    await context.SaveChangesAsync();

    return reservation.Id;
}
```

To this:
```Csharp
public async Task Main()
{
    ReservationResult.For()
        .MapException<RestaurantClosedException>(e => new RestaurantClosedError())
        .MapException<RoomFullException>(e => new RoomFullError())
        .MapElse(e => new UnknownError())
        .TryAsync(async () => await ReserveTable(userId, reservationDate));
}

[Result<int, ReservationError>]
public partial class ReservationResult;

[Union<RestaurantClosed, RoomFullError, UnknownError>]
public partial class ReservationError;
```

In this way, you can refactor safely and remove exceptions one by one, making all your methods explicit about how they can fail so the developer can handle it.

### Union
Unions represent a variable that can be of several different types. At the moment, there are only unions for 5 generic types max. This was on purpose, as it usually more than 5 means you need a refactor to group some of them (at least in my opinion). If you need more, feel free to use the `Funzo.Generator` project and change the ordinality to whatever you need.

You can create an instance by using the constructor or by implicitly converting from it:

```Csharp
var union = new Union<int, string, DateTime>("text");

Union<int, string, DateTime> newUnion = 44;
```

To check if the union is from an specific type, you can use the `Is` method:

```Csharp
Union<int, string, DateTime> union = "text";

if (union.Is<string>(out var text))
{
    Console.WriteLine("Union is a string");
}
```

If you want to do different actions depending on the inner value, you can use the `Switch` or the `Match` methods:
```Csharp
Union<int, string, DateTime> union = DateTime.UtcNow;

var typeOfDate = union.Match(
    i => "Unix timestamp",
    s => "ISO string",
    d => "DateTime");

union.Switch(
    i => Console.WriteLine("Unix timestamp"),
    s => Console.WriteLine("ISO string"),
    d => Console.WriteLine("DateTime"))
```

Unions in this package don't have a `.Value` property and they will never have.

## Serialization
The package `Funzo.Serialization` holds the `JsonConverterFactory` classes needed to work with `System.Text.Json`. At the moment, `Option` serializes to:
```JSON
{ "HasValue": bool, "Value": *whatever* }
```
and `Result` to:
```JSON
{ "IsOk": true, "Ok": *whatever* }
or
{ "IsOk": false, "Err": *whatever* }
```

`Union` serializes as an object with 2 properties: `tag` and `value`:
```chsarp
[Union<int, string, DateTime>]
public partial class MyUnion ;

public class MyClass
{
    public MyUnion Union { get; set; } = new MyUnion(DateTime.UtcNow);
}
```
```JSON
{"Union":{"tag":"DateTime","value":"2025-07-12T13:34:57.6941483Z"}}
```

## Example
There is a working example of a payment system under `Funzo.Example`. Feel free to take a look at how the code will look after using it.

## Design philosophy
The idea behind this small package was to provide `Option`/`Result` monads that work idiomatically with C#, whithout losing the essence of them.

In order to achieve this, an approach of *Explicit better than implicit* was used:
- When working with `Option`, minimize the posibility of `NullReferenceException` by limiting the options to get the value out, enforcing the developer to handle all the cases.
- When working with `Result`, minimize the risk of unforseen consequences (λ) by encouraging to use the `Match` statement.
- Unions don't have the possibility of getting the value explicitly, forcing the developer to use the `Is` method or `Switch`/`Match`
- Encourage the usage of Error values, let it be records with some payload or enums, that provide useful information and force the developer to take action for each one of them. By being explicit in what kind of errors can pop out, the developer is forced to handle all the cases than can go wrong and not rely on catch blocks.