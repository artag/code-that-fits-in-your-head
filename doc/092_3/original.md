# C# will eventually get all F# features, right?

*Source: https://blog.ploeh.dk/2015/04/15/c-will-eventually-get-all-f-features-right/*

*C# will never get the important features that F# has. Here's why.*

The relationship between C# and F# is interesting, no matter if you look at it from the C# or the F# perspective:

- Before releasing F# to the world, Don Syme, its inventor, was [instrumental in getting generics into C# and .NET](http://blogs.msdn.com/b/dsyme/archive/2011/03/15/net-c-generics-history-some-photos-from-feb-1999.aspx). Had it not been for his and his Cambridge colleagues' effort, we wouldn't have generics today, or we would have the subpar generics that Java has. (Okay, so this point isn't strictly about F#.)

- F# has had async workflows [since 2007](http://blogs.msdn.com/b/dsyme/archive/2007/10/11/introducing-f-asynchronous-workflows.aspx). In F#, this is simply one of many implementations of a more general language feature called [Computation Expressions](https://msdn.microsoft.com/en-us/library/dd233182.aspx) - other common examples are [Sequence Expressions](https://msdn.microsoft.com/en-us/library/dd233209.aspx) and [Query Expressions](https://msdn.microsoft.com/en-us/library/hh225374.aspx), but you can also create your own. When async/await was added to C# in 2012, it was a port of that particular implementation, but turned into a one-shot language feature.

- C# 6 gets [a lot of small language features](http://blogs.msdn.com/b/csharpfaq/archive/2014/11/20/new-features-in-c-6.aspx), some of which F# already has: auto-property initializers, exception filters, expression-bodied function members...

- For C# 7, [the design team is considering many Functional language features](https://github.com/dotnet/roslyn/issues/98) that F# already has: pattern matching, records, immutability, tuples...

**There's nothing wrong with this**. F# is a great language, so obviously it makes sense to look there for inspiration. Some features also go the other way, such as F# Query Expressions, which were inspired by LINQ.

It's not some hidden cabal that I'm trying to expose, either. [Mads Torgersen has been quite open about this relationship](http://www.dotnetrocks.com/default.aspx?showNum=935).

## Why care about F#, then?

A common reaction to all of this seems to be that if C# eventually gets all the best F# features, there's no reason to care about F#. Just stick with C#, and get those features in the future.

The most obvious answer to such statements is that F# already has those features, while you'll have to wait for a long time to get them in C#. While C# 6 gets a few features, they are hardly killer features. So perhaps you'll get the good F# features in C#, but they could be years away, and some features might be postponed to later versions again.

In my experience, that argument mostly falls on deaf ears. Many programmers are content to wait, perhaps because they feel that the language choice is out of their hands anyway.

## What F# features could C# get?

Often, when F# enthusiasts attempt to sell the language to other programmers, they have a long list of language features that F# has, and that (e.g.) C# doesn't have. However, in the future, C# could hypothetically have those features too:

- **Records**. C# could have those as well, and they're being considered for C# 7. Implementation-wise, F# records compile to immutable classes anyway.

- **Discriminated Unions**. Nothing in principle prevents C# from getting those. After all, F# Discriminated Unions compile to a class hierarchy.

- **Pattern matching**. Also being considered for C# 7.

- **No nulls**. It's a common myth that F# doesn't have nulls. It does. It's even [a keyword](https://msdn.microsoft.com/en-us/library/dd233249.aspx). It's true that F# doesn't allow its *Functional* data types (records, unions, tuples, etc.) to have null values, but it's only a compiler trick. At run-time, these types can have null values too, and you can provide null values via Reflection. C# could get such a compiler trick as well.

- **Immutability**. F#'s immutability 'feature' is similar to how it deals with nulls. Lots of F# can be mutable (everything that interacts with C# code), but the special *Functional* data types can't. Again, it's mostly in how these specific data types are implemented under the hood that provides this feature, and C# could get that as well.

- **Options**. These are really just specialised Discriminated Unions, so C# could get those too.

- **Object Expressions**. Java has had those for years, so there's no reason C# couldn't get them as well.

- **Partial Function Application**. You [can already do this in C# today, but the syntax for it is really awkward](https://blog.ploeh.dk/2014/03/10/solid-the-next-step-is-functional). Thus, there's *no technical* reason C# can't have that, but the C# language designers would have to come up with a better syntax.

- **Scripting**. F# is great for scripting, but as the success of [scriptcs](http://scriptcs.net/) has shown, nothing prevents C# from being a scripting language either.

- **REPL**. A [REPL](https://en.wikipedia.org/wiki/Read%E2%80%93eval%E2%80%93print_loop) is a really nice tool, but scriptcs already comes with a REPL, again demonstrating that C# could have that too.

This list *in no way* implies that C# will get any or all of these features. While [I'm an MVP](https://mvp.microsoft.com/en-us/mvp/Mark%20Seemann-5000205), I have no inside insight; I'm only speculating. My point is that I see no fundamental reason C# couldn't eventually get those features.

## What F# features can C# never get?

There are a few F# features that many people point to as their favourite, that C# is unlikely to get. A couple of them are:

- **Type Providers**. Someone that I completely trust on this issue told me quite authoritatively that "C# will *never* get Type Providers", and then laughed quietly. While I don't know enough about the technical details of Type Providers to be able to evaluate that statement, I trust this person completely on this issue.

- **Units of Measure**. Here, I simply have to confess ignorance. While I haven't seen talk about units of measure for C#, I have no idea whether it's doable or not.

These are some loved features of F# that look unlikely to be ported to C#, but there's one quality of F# that I'm absolutely convinced will *never* make it to C#, and this is **one of the killer features of F#**: it's what you can't do in the language.

In a recent article, I explained how [less is more when it comes to language features](https://blog.ploeh.dk/2015/04/13/less-is-more-language-features). Many languages come with redundant features (often for historical reasons), but the fewer redundant features a language has, the better.

The **F# compiler doesn't allow circular dependencies**. You can't use a type or a function before you've defined it. This may seem like a restriction, but is perhaps the most important quality of F#. Cyclic dependencies are closely correlated with coupling, and coupling is the deadliest maintainability killer of code bases.

In C# and most other languages, you can define dependency cycles, and the compiler makes it easy for you. In F#, the compiler makes it impossible.

[Studies show that F# projects have fewer and smaller cycles](http://fsharpforfunandprofit.com/posts/cycles-and-modularity-in-the-wild), and that [there are types of cycles (motifs) you don't see at all in F# code bases](http://evelinag.com/blog/2014/06-09-comparing-dependency-networks).

The F# compiler protects you from making cycles. **C# will never be able to do that**, because it would be a *massive breaking change*: if the C# compiler was changed to protect you as well, most existing C# code wouldn't be able to compile.

Microsoft has never been in the habit of introducing breaking changes, so I'm quite convinced that this will never happen.

## Summary

C# could, theoretically, get a lot of the features that F# has, but not the 'feature' that really matters: protection against coupling. Since coupling is one of the most common reasons for [code rot](http://en.wikipedia.org/wiki/Software_rot), this is one of the most compelling reasons to switch to F# today.

F# is a great language, not only because of the features it has, but even more so because if the undesirable traits it *doesn't* have.

## Comments

- **Vladimir Khorikov**

I would say C# won't get non-nullable reference types either, even in the form of a compiler trick. It would either introduce too much of breaking changes or be very limited and thus not especially usefull.

- **Mark Seemann**

Vladimir, thank you for writing. You're probably correct. Many years ago, I overheard Anders Hejlsberg say that it wouldn't be possible to introduce non-nullable reference types into the .NET platform without massive breaking changes. I can't say I ever understood the reasoning behind this (nor was it ever explained to me), but when Anders Hejlsberg tells you that, you sort of have to accept it :)

FWIW, there's a bit of discussion about non-nullable reference types in the [C# Design Meeting Notes for Jan 21, 2015](https://github.com/dotnet/roslyn/issues/98), but I have to admit that I didn't follow the link to Eric Lippert's blog :$
