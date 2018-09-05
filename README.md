## Exercises

These exercises will guide you through various expects of implementing event sourcing with [Marten](http://jasperfx.github.io/marten). Exercises build on top of each other.

The last exercises builds the skeleton of a CQRS solution on top of the event sourcing, using the [MediatR](http://jasperfx.github.io/marten) library.

### Exercise 1: Up and running

 1. Run the PostgreSQL container by going to `/container` in powershell and calling `docker-compose up`
 1. Open the `EventSourcing.sln`
 1. Run the test in `ConfigurationTests`
 1. Open PgAdmin
 1. In PgAmdin connect to localhost, open eventstore -> schemas -> public -> tables
 1. Right click on `mt_doc_configurationtests_testdocument` and get first 100 rows 

 ### Exercise 2: Basic UserAggregate

  1. Add a class called `UserAggregate` to `Domain`. Add name, password hash, and id properties to `UserAggregate`
  1. Add two event type classes to `Domain`: `UserCreated` and `PasswordUpdated`. What data should be in each one? Add that as properties to each event.
  1. Add methods `public void Apply(UserCreated @event)` and `public void Apply(PasswordUpdated @event)`. These should do state changes to the `UserAggregate` that correspond to each event.
  1. Add a new test. The test should add a `UserCreated` event and a `PasswordUpdated` event to new `UserAggregate` stream. Adding to a stream is done like this: `session.Events.StartStream<UserAggregate>(event1, event2)`.
  1. Run the test and look in the database
  1. In the test read the `UserAggregate` back out by doing `var user = session.Events.AggregateStream<UserAggregate>()` and assert that the state of `user` is correct.

### Exercise 3: Store UserAggregates to database

  1. Add `x.Events.InlineProjections.AggregateStreamsWith<UserAggregate>();` to `Initialize.Databse()`. This tells Marten to project the useraggregate when events come in and store the document to the database.
  1. Run test from previous exercise and look in the database
  1. Read the stored aggregate out using `session.Load<UserAggregate>(id)` and assert that the state is correct


### Exercise 4: Adding some business logic

This solution to this exercise is up to you. The objective is to add a `LogingAttemptFailureCount` and associated logic to the `UserAggregate` such that:

   1. `LogingAttemptFailureCount` starts at zero
   1. `LogingAttemptFailureCount` is incremented when a wrong password is used
   1. `LogingAttemptFailureCount` is reset to zero when the right password is used

What events could model this? Where is logic ensuring the above business rules.

N.B. New events are added to an existing stream with the `session.Events.Append` method.


### Exercise 5: Adding a projection

In this exercise we add a persisted projection that keeps track of the number failed login attempts a user has. We do so implementing `ITransform<>`. The transform should transform the event from the previous exercise that signifies a failed login to a document like this one:

```
public class UserNameToFailedLoginAttemptsReadmodel
{
    public string Id {get; set; } // the user name should be used for the id, to ensure efficient querying
    public int FailedAttempts { get; set; };
}
```

To make Marten aware of this transform add a line to the `Initialize.Database()` method:

```
    x.Events.InlineProjections.TransformEvents(new MyTransform());
```

Write a test that uses `session.Load<UserNameToFailedLoginAttemptsReadmodel>` to get number of failed attempts.

### Exercise 6: CQRS

 1. Add the MediatR NuGet package to the `Domain` and `DomainTests` projects
 1. Add class `AttemptLoginCommand` to `Domain` and let it implement the marker interface `IRequest'
 1. Add class `AttemptLoginCommandHandler` to `Domain` and let it implement `RequestHandler`. Call the business logic from exercise 4 in the `Handle` method. Moreover add any data needed by that logic to the `AttemptLoginCommand`
 1. Now follow the setting up instructions here https://github.com/jbogard/MediatR/wiki#setting-up to setup Mediatr.
 1. Then get an `IMediator` instance and call `.Send(loginAttemptCommand)` in a test.
 1. You now have Command Query Responsibility Segragation in a simple form: The command is created in the test, sent through MediatR, handled in the domain which raises events, and read model are updated by Marten.
