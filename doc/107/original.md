# Visitor as a sum type

*Source: https://blog.ploeh.dk/2018/06/25/visitor-as-a-sum-type/*

*The Visitor design pattern is isomorphic to sum types.*

This article is part of [a series of articles about specific design patterns and their category theory counterparts](https://blog.ploeh.dk/2018/03/05/some-design-patterns-as-universal-abstractions). In it, you'll see how the [Visitor design pattern](https://en.wikipedia.org/wiki/Visitor_pattern) is equivalent to a [sum type](https://en.wikipedia.org/wiki/Tagged_union).

## Sum types

I think that the most important advantage of a statically typed programming language is that it gives you [immediate feedback](https://blog.ploeh.dk/2011/04/29/Feedbackmechanismsandtradeoffs) on your design and implementation work. Granted, that your code compiles may not be enough to instil confidence that you've done the right thing, but it's obvious that when your code doesn't compile, you still have work to do.

A static type system enables you to catch some programming errors at compile time. It prevents you from making obvious mistakes like trying to divide a [GUID](https://en.wikipedia.org/wiki/Universally_unique_identifier) by a date. Some type systems don't offer much more help than that, while others are more articulate; I think that [type systems inhabit a continuous spectrum of capabilities](https://blog.ploeh.dk/2016/02/10/types-properties-software), although that, too, is a simplification.

An often-touted advantage of programming languages like [F#](http://fsharp.org/), [OCaml](https://ocaml.org/), and [Haskell](https://www.haskell.org/) is that they, in the words of [Yaron Minsky](https://twitter.com/yminsky), enable you to *make illegal states unrepresentable*. The way these languages differ from languages like C# and Java is that they have [algebraic data types](https://en.wikipedia.org/wiki/Algebraic_data_type).

In short, algebraic data types distinguish between [product types](https://en.wikipedia.org/wiki/Product_type) and sum types. All statically typed language I've seen have product types, which you can think of as combinations of data. Objects with more than a single class fields would be product types.

Sum types (also known as *discriminated unions*), on the other hand, are types that express mutually exclusive alternatives. Object-oriented programmers might mistake such a statement for sub-classing, but the difference is that object-oriented sub-classing creates a potentially infinite hierarchy of subtypes, while a sum type is statically constrained to a finite number of mutually exclusive cases. This is often useful.

In this article, you'll see that a sum type is isomorphic to a corresponding Visitor.

## Church-encoded payment types

In a previous article, you saw how to [Church-encode a domain-specific sum type](https://blog.ploeh.dk/2018/06/18/church-encoded-payment-types). That article, again, demonstrated how to rewrite [a domain-specific F# discriminated union](https://blog.ploeh.dk/2016/11/28/easy-domain-modelling-with-types) as a C# API. The F# type was this `PaymentType` sum type:

```fsharp
type PaymentType =
| Individual of PaymentService
| Parent of PaymentService
| Child of originalTransactionKey : string * paymentService : PaymentService
```

Using Church-encoding in C#, you can arrive at this interface that models the same business problem:

```fsharp
public interface IPaymentType
{
    T Match<T>(
        Func<PaymentService, T> individual,
        Func<PaymentService, T> parent,
        Func<ChildPaymentService, T> child);
}
```

In order to use the API, the compiler obligates you to handle all three mutually exclusive cases defined by the three arguments to the `Match` method. Refer to the previous article for more details and code examples. All [the C# code is also available on GitHub](https://github.com/ploeh/ChurchEncoding).

While the C# code works, I think it'd be a fair criticism to say that it doesn't feel object-oriented. Particularly the use of function delegates (`Func<PaymentService, T>`, etcetera) seems off. These days, C# is a multi-paradigmatic language, and function delegates have been around since 2007, so it's a perfectly fine C# design. Still, if we're trying to understand how object-oriented programming relates to fundamental programming abstractions, it behoves us to consider a more classic form of object-orientation.

## Introduce Parameter Object

Through a series of refactorings you can transform the Church-encoded `IPaymentType` interface to a Visitor. The first step is to use [Refactoring's](http://amzn.to/YPdQDf) *Introduce Parameter Object* to turn the three method arguments of `Match` into a single object:

```csharp
public class PaymentTypeParameters<T>
{
    public PaymentTypeParameters(
        Func<PaymentService, T> individual,
        Func<PaymentService, T> parent,
        Func<ChildPaymentService, T> child)
    {
        Individual = individual;
        Parent = parent;
        Child = child;
    }
 
    public Func<PaymentService, T> Individual { get; }
    public Func<PaymentService, T> Parent { get; }
    public Func<ChildPaymentService, T> Child { get; }
}
```

The modified `IPaymentType` interface then looks like this:

```csharp
public interface IPaymentType
{
    T Match<T>(PaymentTypeParameters<T> parameters);
}
```

Clearly, this change means that you must also adjust each implementation of `IPaymentType` accordingly. Here's the `Match` method of `Individual`:

```csharp
public T Match<T>(PaymentTypeParameters<T> parameters)
{
    return parameters.Individual(paymentService);
}
```

The two other implementations (`Parent` and `Child`) change in the same way; the modifications are trivial, so I'm not going to show them here, but all the code is [available as a single commit](https://github.com/ploeh/ChurchEncoding/commit/64fa2638ffbdb81a077ac1dc3fbce697b3cba35b).

Likewise, client code that uses the API needs adjustment, like the `ToJson` method:

```csharp
public static PaymentJsonModel ToJson(this IPaymentType payment)
{
    return payment.Match(
        new PaymentTypeParameters<PaymentJsonModel>(
            individual : ps =>
                new PaymentJsonModel
                {
                    Name = ps.Name,
                    Action = ps.Action,
                    StartRecurrent = new ChurchFalse(),
                    TransactionKey = new Nothing<string>()
                },
            parent : ps =>
                new PaymentJsonModel
                {
                    Name = ps.Name,
                    Action = ps.Action,
                    StartRecurrent = new ChurchTrue(),
                    TransactionKey = new Nothing<string>()
                },
            child : cps =>
                new PaymentJsonModel
                {
                    Name = cps.PaymentService.Name,
                    Action = cps.PaymentService.Action,
                    StartRecurrent = new ChurchFalse(),
                    TransactionKey =
                        new Just<string>(cps.OriginalTransactionKey)
                }));
}
```

From [argument list isomorphisms](https://blog.ploeh.dk/2018/01/29/argument-list-isomorphisms) we know that an argument list is isomorphic to a Parameter Object, so this step should come as no surprise. We also know that the reverse translation (from Parameter Object to argument list) is possible.

## Add Run prefix

I think it looks a little strange that the functions comprising `PaymentTypeParameters<T>` are named `Individual`, `Parent`, and `Child`. Functions *do* something, so they ought to be named with verbs. This turns out only to be an intermediary step, but I'll add the prefix `Run` to all three:

```csharp
public class PaymentTypeParameters<T>
{
    public PaymentTypeParameters(
        Func<PaymentService, T> individual,
        Func<PaymentService, T> parent,
        Func<ChildPaymentService, T> child)
    {
        RunIndividual = individual;
        RunParent = parent;
        RunChild = child;
    }
 
    public Func<PaymentService, T> RunIndividual { get; }
    public Func<PaymentService, T> RunParent { get; }
    public Func<ChildPaymentService, T> RunChild { get; }
}
```

This doesn't change the structure of the code in any way, but sets it up for the next step.

## Refactor to interface

The definition of `PaymentTypeParameters<T>` still doesn't look object-oriented. While it's formally an object, it's an object that composes three function delegates. We've managed to move the function delegates around, but we haven't managed to get rid of them. From [object isomorphisms](https://blog.ploeh.dk/2018/02/12/object-isomorphisms), however, we know that tuples of functions are isomorphic to objects, and that's essentially what we have here. In this particular case, there's no implementation code in `PaymentTypeParameters<T>` itself - it's nothing but a group of three functions. You can refactor that class to an interface:

```csharp
public interface IPaymentTypeParameters<T>
{
    T RunIndividual(PaymentService individual);
    T RunParent(PaymentService parent);
    T RunChild(ChildPaymentService child);
}
```

The implementations of `Individual`, `Parent`, and `Child` don't change; only the signature of `Match` changes slightly:

```csharp
public interface IPaymentType
{
    T Match<T>(IPaymentTypeParameters<T> parameters);
}
```

Since this change removes the function delegates, it requires client code to change:

```csharp
public static PaymentJsonModel ToJson(this IPaymentType payment)
{
    return payment.Match(new PaymentTypeToJsonParameters());
}
 
private class PaymentTypeToJsonParameters : IPaymentTypeParameters<PaymentJsonModel>
{
    public PaymentJsonModel RunIndividual(PaymentService individual)
    {
        return new PaymentJsonModel
        {
            Name = individual.Name,
            Action = individual.Action,
            StartRecurrent = new ChurchFalse(),
            TransactionKey = new Nothing<string>()
        };
    }
 
    public PaymentJsonModel RunParent(PaymentService parent)
    {
        return new PaymentJsonModel
        {
            Name = parent.Name,
            Action = parent.Action,
            StartRecurrent = new ChurchTrue(),
            TransactionKey = new Nothing<string>()
        };
    }
 
    public PaymentJsonModel RunChild(ChildPaymentService child)
    {
        return new PaymentJsonModel
        {
            Name = child.PaymentService.Name,
            Action = child.PaymentService.Action,
            StartRecurrent = new ChurchFalse(),
            TransactionKey = new Just<string>(child.OriginalTransactionKey)
        };
    }
}
```

The `ToJson` method now has to delegate to a `private` class that implements `IPaymentTypeParameters<PaymentJsonModel>`. In Java and F# you'd be able to pass an object expression, but in C# you have to create an explicit class for the purpose. The implementations of the three methods of the interface still correspond to the three functions the previous incarnations of the code used.

## Rename to Visitor

At this point, the Visitor pattern's structure is already in place. The only remaining step is to rename the various parts of the API so that this becomes clear. You can start by renaming the `IPaymentTypeParameters<T>` interface to `IPaymentTypeVisitor<T>`:

```csharp
public interface IPaymentTypeVisitor<T>
{
    T VisitIndividual(PaymentService individual);
    T VisitParent(PaymentService parent);
    T VisitChild(ChildPaymentService child);
}
```

Notice that I've also renamed the methods from `RunIndividual`, `RunParent`, and `RunChild` to `VisitIndividual`, `VisitParent`, and `VisitChild`.

Likewise, you can rename the `Match` method to `Accept`:

```csharp
public interface IPaymentType
{
    T Accept<T>(IPaymentTypeVisitor<T> visitor);
}
```

In [Design Patterns](http://amzn.to/XBYukB), the Visitor design pattern is only described in such a way that both `Accept` and `Visit` methods have `void` return types, but from [unit isomorphisms](https://blog.ploeh.dk/2018/01/15/unit-isomorphisms) we know that this is equivalent to returning *unit*. Thus, setting `T` in the above API to a suitable *unit* type (like the one defined in F#), you arrive at the canonical Visitor pattern. The generic version here is simply a generalisation.

For the sake of completeness, client code now looks like this:

```csharp
public static PaymentJsonModel ToJson(this IPaymentType payment)
{
    return payment.Accept(new PaymentTypeToJsonVisitor());
}
 
private class PaymentTypeToJsonVisitor : IPaymentTypeVisitor<PaymentJsonModel>
{
    public PaymentJsonModel VisitIndividual(PaymentService individual)
    {
        return new PaymentJsonModel
        {
            Name = individual.Name,
            Action = individual.Action,
            StartRecurrent = new ChurchFalse(),
            TransactionKey = new Nothing<string>()
        };
    }
 
    public PaymentJsonModel VisitParent(PaymentService parent)
    {
        return new PaymentJsonModel
        {
            Name = parent.Name,
            Action = parent.Action,
            StartRecurrent = new ChurchTrue(),
            TransactionKey = new Nothing<string>()
        };
    }
 
    public PaymentJsonModel VisitChild(ChildPaymentService child)
    {
        return new PaymentJsonModel
        {
            Name = child.PaymentService.Name,
            Action = child.PaymentService.Action,
            StartRecurrent = new ChurchFalse(),
            TransactionKey = new Just<string>(child.OriginalTransactionKey)
        };
    }
}
```

You can refactor all the other [Church encoding examples I've shown you](https://blog.ploeh.dk/2018/05/22/church-encoding) to Visitor implementations. It doesn't always make the code more readable, but it's possible.

## From Visitor to sum types

In this article, I've shown how to refactor from a Church-encoded sum type to a Visitor, using the following refactoring steps:

1. Introduce Parameter Object
2. (Rename Method (by adding a `Run` prefix))
3. Refactor to interface
4. Rename to Visitor terminology

All those steps are, I believe, isomorphic, in that they have reverse translations. Thus, since (according to [Conceptual Mathematics](http://amzn.to/13tGJ0f)) isomorphisms are transitive, the translation from sum type to Visitor must have a reverse translation as well. This also seems to me to be intuitively correct, as it's clear to me how to go the other way. Starting with a Visitor:

1. Refactor the Visitor interface to a Parameter Object that composes functions
2. Refactor the Parameter Object to an argument list
3. Rename types and members as desired

You can, I think, read this article from the bottom towards the top to get an impression of what such a series of refactorings would look like, so I'm not going to explicitly provide an example.

## Summary

Algebraic data types enable you to *make illegal states unrepresentable*. Most programming languages have product types, so it's the lack of sum types that seems to make the difference between languages like C# and Java on the one side, and languages like F#, OCaml, or Haskell on the other side.

You can, however, achieve the same objective with object-oriented design. The Visitor design pattern is equivalent to sum types, so everything you can express with a sum type in, say, F#, you can express with a Visitor in C#.

That's not to say that these two representations are equal in readability or maintainability. F# and Haskell sum types are declarative types that usually only take up a few lines of code. Visitor, on the other hand, is a small object hierarchy; it's a more verbose way to express the idea that a type is defined by mutually exclusive and heterogeneous cases. I know which of these alternatives I prefer, but if I were caught in an object-oriented code base, it's nice to know that it's still possible to model a domain with algebraic data types.

## Comments

- **Oskar Gewalli**

I think that it's important to remember the type of abstractions you highlight by showing that the Vistor design pattern is the same as sum types. I appreciate this post.

When I read up on [Entity component system](https://en.wikipedia.org/wiki/Entity_component_system), I found it interesting how the pattern arrived from the need to be able to control memory. Perhaps the somewhat odd form of the Visitor pattern has arrived from the same limitations in some common languages? Perhaps there are other constructs that can be expressed more clearly using more modern patterns?

- **Mark Seemann**

Oskar, thank you for writing. I didn't know about entity component system.

[This very article series](https://blog.ploeh.dk/2018/03/05/some-design-patterns-as-universal-abstractions) tries to identify various design patterns and how they relate to more fundamental constructs. Most likely you're already aware of that, so perhaps you meant something else by your questions. If you did, however, I can't glean what.

- **Oskar Gewalli**

I was not aware of [that article series](https://blog.ploeh.dk/2018/03/05/some-design-patterns-as-universal-abstractions). The answer I'm looking for seems to be the identified subset.
