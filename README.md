# GenericRepository

A generic repository package that provides basic CRUD functionality that can be adapted to use any type of data storage provider.

# Implementations

## Stub

The in-memory implementation of generic repository. Good for unit testing purposes.

### How to use

Given an entity...

```csharp
class User
{
	public Guid Id { get; }
	public string Name { get; }

	public User(Guid id, string name)
	{
		Id = id;
		Name = name;
	}
}
```

use our simple interface for wiring up your repositories... Where `User` is our entity which has a `Guid` id and the predicate passed into the `Add` method defines how to get the id of `User`.

```csharp
var sc = new ServiceCollection();
sc.UseStubbedGenericRepositories()
	.Add<User, Guid>(x => x.Id)
	.Add<Foo, int>(x => x.Id);
```

Voila you can now use your repositories to save, get, update and delete your entities...

```csharp
var userRepo = sc.BuildServiceProvider().GetRequiredService<IGenericRepository<User, Guid>>();
var id = Guid.Parse("045523cb-9453-4a70-98a2-3d49aa687979");
await userRepo.Save(new User(id, "Callum Styles"));

var user = await userRepo.Get(id);
```

## Mongo

The MongoDB implementation of generic repository.

### How to use

Given an entity...

```csharp
class User
{
	public Guid Id { get; }
	public string Name { get; }

	public User(Guid id, string name)
	{
		Id = id;
		Name = name;
	}
}
```

Create a class that implements `GenericMongoRepositoryArgs<User, Guid>`... where `User` is an entity with a `Guid` id.

```csharp
class UserArgs : GenericMongoRepositoryArgs<User, Guid>
{
	public override Expression<Func<User, Guid>> KeySelector { get; } = x => x.Id;

	public override void RegisterClassMap(BsonClassMap<User> cm)
	{
		cm.MapMember(x => x.Name);
		cm.MapCreator(x => new User(x.Id, x.Name));
	}
}
```

Then call the following code, passing in the assemblies that contain your types that implement `GenericMongoRepositoryArgs<,>`

```csharp
var sc = new ServiceCollection();
sc.AddGenericMongoRepositories("ConnectionString", "DatabaseName", typeof(User).Assembly);
```

And again your repositories are ready to use...

```csharp
var userRepo = sc.BuildServiceProvider().GetRequiredService<IGenericRepository<User, Guid>>();
var id = Guid.Parse("045523cb-9453-4a70-98a2-3d49aa687979");
await userRepo.Save(new User(id, "Callum Styles"));

var user = await userRepo.Get(id);
```