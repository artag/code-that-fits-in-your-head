# Church-encoded Maybe

*Source: https://blog.ploeh.dk/2018/06/04/church-encoded-maybe/*

*Programming languages don't have to have a built-in notion of null values. Missing or optional values can be created from first principles. An introduction for object-oriented programmers.*

This article is part of [a series of articles about Church encoding](https://blog.ploeh.dk/2018/05/22/church-encoding). In this series, you'll learn how to re-create various programming language features from first principles. In previous articles, you learned [how to implement Boolean logic without Boolean primitives](https://blog.ploeh.dk/2018/05/24/church-encoded-boolean-values), as well as [how to model natural numbers](https://blog.ploeh.dk/2018/05/28/church-encoded-natural-numbers). Through these examples, you'll learn how to model [sum types](https://en.wikipedia.org/wiki/Tagged_union) without explicit language support.

## The billion-dollar mistake

All mainstream programming languages have a built-in notion of *null*: a value that isn't there. There's nothing wrong with the concept; you often run into situations where you need to return a value, but in certain cases, you'll have nothing to return. Division by zero would be one example. Attempting to retrieve the first element from an empty collection would be another.

Unfortunately, for fifty years, we've been immersed in environments where null references have been the dominant way to model the absence of data. This, despite the fact that even [Sir Antony Hoare](https://en.wikipedia.org/wiki/Tony_Hoare), the inventor of null references, has publicly called it his *billion-dollar mistake*.

You can, however, model the potential absence of data in saner ways. [Haskell](https://www.haskell.org/), for example, has no built-in null support, but it does include a built-in [Maybe](https://blog.ploeh.dk/2018/03/26/the-maybe-functor) type. In Haskell (as well as in [F#](http://fsharp.org/), where it's called `option`), `Maybe` is defined as a sum type:

```text
data Maybe a = Nothing | Just a deriving (Eq, Ord)
```

If you're not familiar with Haskell syntax, this is a type declaration that states that the [parametrically polymorphic](https://en.wikipedia.org/wiki/Parametric_polymorphism) (AKA *generic*) data type `Maybe` is inhabited by `Just` values that contain other values, plus the constant `Nothing`.

This article series, however, examines how to implement sum types with Church encoding.

## Lambda calculus maybe

Church encoding is based on the [lambda calculus](https://en.wikipedia.org/wiki/Lambda_calculus), which defines a universal model of computation based entirely on functions (lambda expressions) and recursion. In lambda calculus, the contract of *Maybe* is defined as an expression that takes two arguments. [There's two fundamental 'implementations' of the contract](http://programmable.computer/posts/church_encoding.html):

```text
nothing =    λn.λj.n
   just = λx.λn.λj.j x
```

The contract is that the first function argument (`n`) represents the *nothing case*, whereas the second argument (`j`) represents the `just` case.

The `nothing` function is a lambda expression that takes two arguments (`n` and `j`), and always returns the first, left-most argument (`n`).

The `just` function is a lambda expression that takes three arguments (`x`, `n`, and `j`), and always returns `j x`. Recall that in the lambda calculus, everything is a function, including `j`, so `j x` means that the function `j` is called with the argument `x`.

A few paragraphs above, I wrote that the contract of *maybe* is modelled as an expression that takes two arguments, yet `just` takes three arguments. How does that fit?

In the lambda calculus, expressions are always curried, so instead of viewing `just` as a function with three arguments, you can view it as a function that takes a single element (`x`) and returns a function that takes two arguments. This agrees with Haskell's `Just` data constructor:

```text
Prelude> :t Just
Just :: a -> Maybe a
```

Haskell tells us that `Just` is a function that takes an `a` value (corresponding to `x` in the above `just` lambda expression) and returns a `Maybe a` value.

## Church-encoded Maybe in C#

Both lambda calculus and Haskell rely on currying and partial application to make the contract fit. In C#, as you've [previously](https://blog.ploeh.dk/2018/01/08/software-design-isomorphisms) seen, you can instead define an interface and rely on class fields for the 'extra' function arguments. Since Church-encoded Maybe is represented by a function that takes two arguments, we'll once again define an interface with a single method that takes two arguments:

```csharp
public interface IMaybe<T>
{
    TResult Match<TResult>(TResult nothing, Func<T, TResult> just);
}
```

In the first article, about Church-encoded Boolean values, you saw how two mutually exclusive values could be modelled as a method that takes two arguments. Boolean values are simply constants (*true* and *false*), where the next example (natural numbers) included a case where one case (*successor*) contained data. In that example, however, the data was statically typed as another `INaturalNumber` value. In the current `IMaybe<T>` example, the data contained in the *just* case is generic (it's of the type `T`).

Notice that there's two levels of generics in play. `IMaybe<T>` itself is a container of the generic type `T`, whereas `Match` enables you to convert the container into the rank-2 polymorphic type `TResult`.

Once more, the contract of `IMaybe<T>` is that the first, left-hand argument represents the *nothing case*, whereas the second, right-hand argument represents the *just* case. The *nothing* implementation, then, is similar to the previous `ChurchTrue` and `Zero` classes:

```csharp
public class Nothing<T> : IMaybe<T>
{
    public TResult Match<TResult>(TResult nothing, Func<T, TResult> just)
    {
        return nothing;
    }
}
```

Again, the implementation unconditionally returns `nothing` while ignoring `just`. You may, though, have noticed that, as is appropriate for Maybe, `Nothing<T>` has a distinct type. In other words, `Nothing<string>` doesn't have the same type as `Nothing<int>`. This is not only 'by design', but is a fundamental result of how we define *Maybe*. The code simply wouldn't compile if you tried to remove the type argument from the class. This is in contrast to [C# null, which has no type](https://blog.ploeh.dk/2015/11/13/null-has-no-type-but-maybe-has).

You implement the *just* case like this:

```csharp
public class Just<T> : IMaybe<T>
{
    private readonly T value;
 
    public Just(T value)
    {
        this.value = value;
    }
 
    public TResult Match<TResult>(TResult nothing, Func<T, TResult> just)
    {
        return just(value);
    }
}
```

According to the contract, `Just<T>` ignores `nothing` and works exclusively with the `just` function argument. Notice that the `value` class field is `private` and not exposed as a public member. The only way you, as a caller, can potentially extract the value is by calling `Match`.

Here are some examples of using the API:

```csharp
> new Nothing<Guid>().Match(nothing: "empty", just: g => g.ToString())
"empty"
> new Just<int>(42).Match(nothing: "empty", just: i => i.ToString())
"42"
> new Just<int>(1337).Match(nothing: 0, just: i => i)
1337
```

Notice that the third example shows how to extract the value contained in a `Nothing<int>` object without changing the output type. All you have to do is to supply a 'fall-back' value that can be used in case the value is *nothing*.

## Maybe predicates

You can easily implement the standard Maybe predicates `IsNothing` and `IsJust`:

```csharp
public static IChurchBoolean IsNothing<T>(this IMaybe<T> m)
{
    return m.Match<IChurchBoolean>(
        nothing :   new ChurchTrue(), 
        just : _ => new ChurchFalse());
}
 
public static IChurchBoolean IsJust<T>(this IMaybe<T> m)
{
    return m.Match<IChurchBoolean>(
        nothing :   new ChurchFalse(),
        just : _ => new ChurchTrue());
}
```

Here, I arbitrarily chose to implement `IsJust` 'from scratch', but I could also have implemented it by negating the result of calling `IsNothing`. Once again, notice that the predicates are expressed in terms of Church-encoded Boolean values, instead of the built-in `bool` primitives.

## Functor

From Haskell (and F#) we know that Maybe is a [functor](https://blog.ploeh.dk/2018/03/22/functors). In C#, you turn a container into a functor by implementing an appropriate `Select` method. You can do this with `IMaybe<T>` as well:

```csharp
public static IMaybe<TResult> Select<T, TResult>(
    this IMaybe<T> source,
    Func<T, TResult> selector)
{
    return source.Match<IMaybe<TResult>>(
        nothing:   new Nothing<TResult>(),
        just: x => new Just<TResult>(selector(x)));
}
```

Notice that this method turns an `IMaybe<T>` object into an `IMaybe<TResult>` object, using nothing but the `Match` method. This is possible because `Match` has a generic return type; thus, among other types of values, you can make it return `IMaybe<TResult>`.

When `source` is a `Nothing<T>` object, `Match` returns the object in the *nothing* case, which here becomes a new `Nothing<TResult>` object.

When `source` is a `Just<T>` object, `Match` invokes `selector` with the value contained in the *just* object, packages the result in a new `Just<TResult>` object, and returns it.

Because the `Select` method has the correct signature, you can use it with query syntax, as well as with normal method call syntax:

```csharp
IMaybe<int> m = new Just<int>(42);
IMaybe<string> actual = from i in m
                        select i.ToString();
```

This example simply creates a *just* value containing the number `42`, and then maps it to a string. Another way to write the same expression would be with method call syntax:

```csharp
IMaybe<int> m = new Just<int>(42);
IMaybe<string> actual = m.Select(i => i.ToString());
```

In both cases, the result is a just case containing the string `"42"`.

## Summary

In this article, you saw how it's possible to define the *Maybe* container from first principles, using nothing but functions (and, for the C# examples, interfaces and classes in order to make the code easier to understand for object-oriented developers).

The code shown in this article is available on [GitHub](https://github.com/ploeh/ChurchEncoding/tree/8d1e7501f486351e748646c915f0bd334332e386).

Church-encoding enables you to model sum types as functions. So far in this article series, you've seen how to model Boolean values, natural numbers, and Maybe. Common to all three examples is that the data type in question consists of two mutually exclusive cases. There's at least one more interesting variation on that pattern.

Next: [Church-encoded Either](https://blog.ploeh.dk/2018/06/11/church-encoded-either).

## Comments

- **Anthony Leatherwood**

It's probably not your favorite thing to do anymore, but I thank you so much for continuing to provide C# examples for these concepts. It's invaluable for programmers wishing to adopt these concepts into the OOP languages they work with for a living. It's also a tremendous aid in briding the gap of understanding between OOP and FP.

- **Nathaniel Bond**

Hey Mark, thanks so much for writing. Coming at this some 3 years later but it's been an insightful introduction to functional programming.

I've been trying Church encoding and algebraic data types in general out for a little while now, and had a question about a potential alternate implementation of `Select` and friends. I had been reviewing [Church-encoded Booleans](https://blog.ploeh.dk/2018/05/24/church-encoded-boolean-values/) and really appreciated the way the `And`/`Or`/`Not` `Match` implementations managed to defer executing their operator's actual operation until needed. So I figured there might be a way to do something similar with `Select`.

Here's what I came up with:

```csharp
public class MaybeSelected<T, TSelectResult> : IMaybe<TSelectResult>
{
    private readonly IMaybe<T> source;
    private readonly Func<T, TSelectResult> selector;

    public MaybeSelected(
        IMaybe<T> source,
        Func<T, TSelectResult> selector)
    {
        this.source = source;
        this.selector = selector;
    }
 
    public TResult Match<TResult>(TResult nothing, Func<TSelectResult, TResult> just)
    {
        return source.Match(
            nothing: nothing,
            just: x => just(selector(x)));
    }
}
```

Which resulted in the following `Select` refactoring:

```csharp
public static IMaybe<TResult> Select<T, TResult>(
    this IMaybe<T> source,
    Func<T, TResult> selector)
{
    return new MaybeSelected<T, TResult>(source, selector);
}
```

After ensuring the refactor passed unit tests, I went on to the monad:

```csharp
public class MaybeFlattened<T> : IMaybe<T>
{
    private readonly IMaybe<IMaybe<T>> source;

    public MaybeFlattened(IMaybe<IMaybe<T>> source)
    {
        this.source = source;
    }
 
    public TResult Match<TResult>(TResult nothing, Func<T, TResult> just)
    {
        return source.Match(
            nothing: nothing,
            just: x => x.Match(
                nothing: nothing,
                just: just));
    }
}
```

Which resulted in a similar refactoring of `Flatten`. I'm pretty happy with the results, especially in the case of `Select` not needing to execute its `selector` until `Match` is called. This seems to resemble the behavior of `IEnumerable` not actually enumerating until something like `ToList` is called. The unit tests pass, so I have some demonstration of correct behavior, though I suppose being new to this I may have failed to understand something more essential than the demonstrated behavior they exercise.

That said, I am more interested in the nature of the refactoring itself. It seems to be a kind of isomorphism similar to that of [argument lists](https://blog.ploeh.dk/2018/01/29/argument-list-isomorphisms/), but extending beyond the method's parameters to including its behavior as well. In general, the process involved taking, e.g., the `Select` method's parameters and making corresponding fields in the new `MaybeSelected` class; and then taking the method's implementation and shuffling it off to some method in the new class (`Match`) that would eventually perform the original execution. I like seeing how the `x => x` "phrase" lines up between the original `Flatten` and `MaybeFlattened.Match`. So going beyond "it works," would you mind sharing your thoughts on what kind of animal we've got here?

P.S. — I have included the above changes in [this fork](https://github.com/nabond251/ChurchEncoding/tree/feature/defurred-cat) of the post's repository.

- **Mark Seemann**

Nathaniel, thank you for writing. If you're new to this, I tip my hat to you. As far as I can tell, you've arrived at the insight that lazy evaluation seems to be a 'toggle' you can turn on or off without affecting the lawfulness of the underlying concepts.

I admit that I haven't done a full, formal analysis of your approach, but it looks completely legit to me.

The reason I feel confident that this is still a lawful Maybe monad is that Haskell is lazy by default. I've learned much of what I know about functors and monads from Haskell. In Haskell, most of the monads are lazy, and you explicitly have to opt in if you want strict evaluation (in Haskell, the opposite of lazy is usually called *strict*, although [it's a little more complicated than that](https://wiki.haskell.org/Performance/Strictness).) In most other languages (C# included), expressions are usually eagerly evaluated, and you have to explicitly opt into laziness.

In those languages, you can use something like `Lazy<T>` to [form an applicative functor](https://blog.ploeh.dk/2018/12/17/the-lazy-applicative-functor) (and monad, but I haven't written that article). You can also show that [it obeys the functor laws](https://blog.ploeh.dk/2018/09/10/the-lazy-functor). It also obeys the applicative functor and monad laws, but again, I haven't written those articles...

What you've done here is slightly different, since you haven't explicitly used `Lazy<T>`, but I'd expect your variation to be isomorphic to `Lazy<Maybe<T>>` (where `Maybe<T>` is an eagerly evaluated version of Maybe). If you're interested, you may consider this exercise:

*Write two functions that convert back and forth between the two representations, and demonstrate that the two implementations behave in the same way.*

In general, knowing how to enable lazy evaluation can be a useful tool to add to your toolkit, but as all Haskellers have learned, it comes with a disadvantage, too. Instead of evaluating values right away, you have to pass around *expressions*. Such expressions containing other expressions are called *thunks*, and they can grow quite large. In Haskell, which relies heavily on this mechanism, if you aren't careful, you can build up thunks that eat up all available memory.

In reality, I don't experience this to be a big problem as long as one stays aware of it, but the point is that you can't just forget about it. Laziness isn't a free ride to better performance. You trade more efficient *computation* for more inefficient *memory use*.
