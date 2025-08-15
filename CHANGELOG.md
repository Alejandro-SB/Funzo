# CHANGELOG

## 4.3.0
- Completely removed `Unit` class

## 4.2.0
- Removed unsafe code left

## 4.1.0
- Added implicit conversions for predefined results

## 4.0.0
- Changed union serialization to expose a tag/value pair

## 3.1.1
- Fixed bug in source code generation

## 3.1.0
- Now source generators don't need to inherit/implement interfaces. Using generic attributes types can be provided, avoiding innecesary clutter. This requires [C#11](https://learn.microsoft.com/en-us/dotnet/csharp/advanced-topics/reflection-and-attributes/generics-and-attributes)
- Added Union serialization. It serializes to a property with the name of the property being the name of the type. Check README for some examples.
- Union types now expose public shared properties so there is no need to Match. For Example:
```Csharp
public record User(int Id, string Name);
public record Admin(int Id, string Name, Permission[] Permissions);
public record SuperAdmin(int Id);

[Union<User, Admin, SuperAdmin>]
public partial class AppUser;
```
Will make `AppUser` have an `Id` property exposed (readonly);
- Added implicit conversions for unions when inside a Result. Now, you can do something like this:
```Csharp

public async Task<CreateUserResult> CreateUser(string username, string password)
{
	/// This could return a Result<IEnumerable<string>> with errors
	var isValidUsernameResult = _userNameValidator.IsValid(username);

	if (isValidUsernameResult.IsErr(out var errors)
	{
	    return new InvalidUsername(errors);
	}

	var existsUser = await _context.Users.AnyAsync(u => u.Username == username);

	if (existsUser)
	{
		return new UsernameAlreadyExists(username);
	}

	var passwordEncryptionResult = _passwordEncryptionService.TryHashPassword(password);

	if (passwordEncryptionResult.IsErr(out var hashedPassword, out var errors))
	{
		return new PasswordNotStrongEnough(errors);
	}

	var user = new User
	{
		Username = username,
		Password = hashedPassword
	};

	_context.Users.Add(user);

	await _context.SaveChangesAsync();

	return user.Id;
}


[Result<UserId, UserCreationError>]
public partial class CreateUserResult;

[Union<InvalidUsername, UsernameAlreadyExists, PasswordNotStrongEnough>]
public partial class UserCreationError;

public record InvalidUsername(IEnumerable<string> RuleErrors);
public record UsernameAlreadyExists(string Username);
public record PasswordNotStrongEnough(IEnumerable<string> Rules);
```

## 3.0.0
Big refactor of the whole codebase to provide better DX
### `Option<T>`
- Now it is a struct. That means that `default(Option<T>)` is always `None`.
- Added new methods `Inspect` and `InspectAsync`.
- `Option<T>.None` is now a property instead of a method.

## `Result`
Now it has been split in two different classes: `Result<TOk, TErr>` and `Result<TErr>`. 

Previously, `Result<TErr>` inherited from `Result<Unit, TErr>`. This created a bad experience, where in order to do a `Map`, you had to carry a useless value. Also, it forced you to carry the `Unit` in all the calls, providing a bad DX. By splitting it into two different classes, now methods are contextual within each other.

They inherit from `ResultBase<TResult, TOk, TErr>` and `ResultBase<TResult, TErr>` respectively. They serve as a basis for all the derived results created by the users and are the ones used by the source generator. The rationale behind this change was that in order to return the same instance, a reference to the type was needed, but supplying the type every time provided a very bad user experience. To mitigate it, `Result<TOk, TErr>` were created, inheriting from `ResultBase<Result<TOk, TErr>, TOk, TErr>`. This facilitates sharing the logic between the classes without too much noise in the code.

`Result<TOk, TErr>` and `Result<TErr>` are sealed classes, as **they are meant to be used, not to be extended**. The classes you should extend are the `ResultBase` ones, preferably using the source generators provided.

### `Result<TOk, TErr>`
Now inherits all its functionality from `ResultBase<Result<TOk, TErr>, TOk, TErr>`.

- `Match` method when an action is supplied returns the same instance. Works as the `Inspect` method for `Option`
- Added `Inspect`, `InspectAsync`, `InspectErr` and `InspectErrAsync` methods. Functionality is similar to the `Option` one.
- Renamed the `Ok` method to `AsOk`, that returns an `Option<TOk>`.
- Added the `For` method, that creates a `ResultBuilder` to map exceptions to a `Result`.

### `Result<TErr>`
Pretty similar to `Result<TOk, TErr>`.

- `Match` method has now the signatures `Match(Func<TOut>, Func<TErr, TOut>)` and `Match(Action, Action<TErr>)` instead of `Match(Func<Unit,TOut>, Func<TErr, TOut>)` and `Match(Action<Unit>, Action<TErr>)`.
- Added the method `EnsureOk` that throws if the result is not in an Ok state.
- Added the `Inspect` methods.
- There is no `IsErr(out TOk, out TErr)` method here (for obvious reasons).
- Added `For` method to create a `ResultBuilder`.

## `ResultBuilder`
New class that lets you map exceptions to results.

## Source generators
Now `Results` have to be marked with the `IResult` interface instead of inheriting from a `Result` class.
```csharp
[Result<int,string>]
public class MyResult : IResult<int, string>;
```
Also, generators have been split so the code generated for `IResult<TOk, TErr>` and `IResult<TErr>` is more explicit (no more carrying `Unit` around).