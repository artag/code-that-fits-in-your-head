# Zone of Ceremony (формальность/церемония/обряд)

*Source: https://blog.ploeh.dk/2019/12/16/zone-of-ceremony/*

*Static typing doesn't have to involve (включать) much ceremony.*

I seem to get involved (кажется, я вмешиваюсь) in long and passionate (пылкие) debates about static versus dynamic typing on a regular basis. I find myself clearly on the side of static typing, but this article isn't about the virtues (достоинствах) of static versus dynamic typing. The purpose is to correct a common misconception (заблуждение) about statically typed languages.

## Ceremony (формальности)

People who favour (предпочитает) dynamically typed languages over statically typed languages often emphasise (подчеркивают) that they find the lack (отсутствие) of ceremony productive. That seems reasonable; only, it's a false dichotomy (это кажется разумным; только это ложная дихотомия).

>"Ceremony is what you have to do before you get to do what you really want to do.
>Ceremony - это то, что вам нужно сделать, прежде чем делать то, что вы действительно хотите сделать."
>
>[Venkat Subramaniam](https://youtu.be/4jCjDEb9KZI)

Dynamically typed languages do seem to be light (кажутся легкими) on ceremony, but you can't infer (сделать вывод) from that that statically typed languages have to require lots of ceremony. Unfortunately, all mainstream statically typed languages belong to the same family, and they do involve ceremony. I think that people extrapolate from what they know; they falsely conclude that all statically typed languages must come with the overhead of ceremony.

It looks to me more as though there's an unfortunate *Zone of Ceremony*:

![image](/doc/092_2/zone-of-ceremony.png)

Such a diagram can never be anything but a simplification, but I hope that it's illuminating (является разъясяющей). C++, Java, and C# are all languages that involve (включают в себя) ceremony. To the right of them are what we could term the *trans-ceremonial languages*. These include [F#](https://fsharp.org/) and [Haskell](https://www.haskell.org/).

In the following, I'll show some code examples in various languages. I'll discuss ceremony according to the above definition. The discussion focuses on the amount of preparatory work (подготовительных работ) one has to do, such as creating a new file, declaring a new class, and declaring types. The discussion is *not* about the implementation code. For that reason, I've removed colouring from the implementation code, and emphasised (выделил) the code that I consider ceremonial.

## Low ceremony of JavaScript

Imagine that you're given a list of numbers, as well as a quantity. The quantity is a number to be consumed. You must remove elements from the left until you've consumed at least that quantity. Then return the rest of the list.

```text
> consume ([1,2,3], 1);
[ 2, 3 ]
> consume ([1,2,3], 2);
[ 3 ]
> consume ([1,2,3], 3);
[ 3 ]
> consume ([1,2,3], 4);
[]
```

The first example consumes only the leading `1`, while both the second and the third example consumes both `1` and `2` because the sum of those values is `3`, and the requested quantity is `2` and `3`, respectively. The fourth example consumes all elements because the requested quantity is `4`, and you need both `1`, `2`, and `3` before the sum is large enough. You have to pick strictly (строго) from the left, so you can't decide to just take the elements `1` and `3`.

If you're wondering why such a function (интересно, почему такая функция) would be useful, [here's my motivating example](https://blog.ploeh.dk/2020/04/27/an-f-implementation-of-the-maitre-d-kata).

In JavaScript, you could implement the `consume` function like this:

```js
var consume = function (source, quantity) {
    if (!source) {
        return [];
    }

    var accumulator = 0;
    var result = [];
    for (var i = 0; i < source.length; i++) {
        var x = source[i];
        if (quantity <= accumulator)
            result.push(x);

        accumulator += x;
    }
    return result;
}
```

I'm a terrible JavaScript programmer, so I'm sure that it could have been done more elegantly, but as far as I can tell (насколько я могу судить), it gets the job done. I wrote some tests, and I have 17 passing test cases. The point isn't about how you write the function, but how much ceremony is required. In JavaScript you don't need to declare any types. Just name (дайте наименование) the function and its arguments, and you're ready to write code.

## High ceremony of C#

Contrast the JavaScript example with C#. The same function in C# would look like this:

```csharp
public static class Enumerable
{
    public static IEnumerable<int> Consume(
        this IEnumerable<int> source,
        int quantity)
    {
        if (source is null)
            yield break;

        var accumulator = 0;
        foreach (var i in source)
        {
            if (quantity <= accumulator)
                yield return i;

            accumulator += i;
        }
    }
}
```

Here you have to declare the type of each method argument, as well as (а также) the return type of the method. You also have to put the method in a class. This may not seem like much overhead, but if you later need to change the types, editing is required. This can affect downstream callers, so simple type changes ripple through code bases (изменения пробегают через кодовые базы).

It gets worse, though (Хотя, все еще хуже). The above `Consume` method only handles `int` values. What if you need to call the method with `long` arrays?

You'd have to add an overload:

```csharp
public static IEnumerable<long> Consume(
    this IEnumerable<long> source,
    long quantity)
{
    if (source is null)
        yield break;

    var accumulator = 0L;
    foreach (var i in source)
    {
        if (quantity <= accumulator)
            yield return i;

        accumulator += i;
    }
}
```

Do you need support for `short`? Add an overload. `decimal`? Add an overload. `byte`? Add an overload.

No wonder (неудивительно, что) people used to dynamic languages find this awkward (неудобным).

## Low ceremony of F#

You can write the same functionality in F#:

```fsharp
let inline consume quantity =
    let go (acc, xs) x =
        if quantity <= acc
        then (acc, Seq.append xs (Seq.singleton x))
        else (acc + x, xs)
    Seq.fold go (LanguagePrimitives.GenericZero, Seq.empty) >> snd
```

There's no type declaration in sight (на первый взгляд), but nonetheless (тем не менее) the function is statically typed. It has this somewhat (до некоторой степени) complicated type:

```fsharp
quantity: ^a -> (seq< ^b> -> seq< ^b>)
  when ( ^a or  ^b) : (static member ( + ) :  ^a *  ^b ->  ^a) and
        ^a : (static member get_Zero : ->  ^a) and  ^a : comparison
```

While this looks arcane (это выглядит удивительно), it means that it support sequences of any type that comes with a zero value and supports addition (добавление) and comparison. You can call it with both 32-bit integers, decimals, and so on:

```fsharp
> consume 2 [1;2;3];;
val it : seq<int> = seq [3]

> consume 2m [1m;2m;3m];;
val it : seq<decimal> = seq [3M]
```

Static typing still means that you can't just call it with any type of value. An expression like `consume "foo" [true;false;true]` will not compile.

You can explicitly declare types in F# (like you can in C#), but my experience is that if you don't, type changes tend to just propagate throughout your code base (просто распространяются по всей вашей кодовой базе). Change (при изменении) a type of a function, and upstream callers generally just 'figure it out' ('выяснят/выведут его'). If you think of functions calling other functions as a graph, you often only have to adjust leaf nodes (листовые узлы) even when you change the type of something deep in your code base.

## Low ceremony of Haskell

Likewise (аналогично), you can write the function in Haskell:

```hs
consume quantity = reverse . snd . foldl go (0, [])
  where
    go (acc, ys) x = if quantity <= acc then (acc, x:ys) else (acc + x, ys)
```

Again, you don't have to explicitly declare any types. The compiler figures them out (выведет их сам). You can ask GHCi about the function's type, and it'll tell you:

```hs
> :t consume
consume :: (Foldable t, Ord a, Num a) => a -> t a -> [a]
```

It's more compact than the inferred (вывод) F# type, but the idea is the same. It'll compile for any `Foldable` container `t` and any type `a` that belongs to the classes of types called `Ord` and `Num`. `Num` supports addition and `Ord` supports comparison.

There's little ceremony involved (связанная) with the types in Haskell or F#, yet both languages are statically typed. In fact (фактически), their type systems are more powerful than C#'s or Java's. They can express relationships between types that those languages can't.

## Summary

In debates about static versus dynamic typing, contributors often generalise (обобщают) from their experience with C++, Java, or C#. They dislike (не любят) the amount of ceremony required in these languages, but falsely believe (ошибочно верят) that it means that you can't have static types without ceremony.

The statically typed mainstream languages seem to occupy a *Zone of Ceremony*.

Static typing without ceremony is possible, as evidenced (как доказывают) by languages like F# and Haskell. You could call such languages *trans-ceremonial languages*. They offer the best of both worlds: compile-time checking and little ceremony.

## Comments

- **Tyson Williams**

In your initial `int` C# example, I think your point (точка зрения) is that method arguments and the return type require [manifest](https://en.wikipedia.org/wiki/Manifest_typing) typing. Then for your example about `long` (and comments about `short`, `decimal`, and `byte`), I think your point is that C#'s type system is primarily [nominal](https://en.wikipedia.org/wiki/Nominal_type_system). You then contrast those C# examples with F# and Haskell examples that utilize [inferred](https://en.wikipedia.org/wiki/Type_inference) and [structural](https://en.wikipedia.org/wiki/Structural_type_system) aspects of their type systems.

I also sometimes get involved in debates (учавствую в обсуждениях) about static versus dynamic typing and find myself on the side of static typing. Furthermore (к тому же), I also typically hear arguments against manifest and nominal typing instead of against static typing. In theory, I agree with those arguments; I also prefer type systems that are inferred and structural instead of those that are manifest and nominal.

I see the tradeoff (компромисс) as being among (быть среди) the users of the programming language, those responsible for writing and maintaining the compiler/interpreter, and what can be said about the correctness of the code. (In the rest of this paragraph, all statements about things being simple or complex are meant to be relative. I will also exaggerate for the sake of simplifying my statements (преувеличиваю ради упрощения моих заявлений).) For a dynamic language, the interpreter and coding are simple but there are no guarantees about correctness. For a static, manifest, and nominal language, the compiler is somewhere between simple and complex, the coding is complex, but at least (по меньшей мере) there are some guarantees about correctness. For a static, inferred, structural language, the compiler is complex, coding is simple, and there are some guarantees about correctness.

Contrasting a dynamic language with one that is static, inferred, and structural, I see the tradeoff (компромисс) as being directly between the the compiler/interpreter writers and what can be said about the correctness of the code while the experience of those writing code in the language is mostly unchanged. I think that is your point being made by contrasting the JavaScript example (a dynamic language) with the F# and Haskell examples (that demonstrate the static, inferred, and structural behavior of their type systems).

While we are on the topic, I would like to say something that I think is controversial (спорный) about [duck typing](https://en.wikipedia.org/wiki/Duck_typing). I think duck typing is "just" a dynamic type system that is also structural. This contradicts (противоречит) the lead (примеру) of its Wikipedia article (linked above) as well as the [subsection about structural type systems](https://en.wikipedia.org/wiki/Duck_typing#Structural_type_systems). They both imply (подразумевают) that nominal vs structural typing is a spectrum that only exists for static languages. I disagree; I think dynamic languages can also exist on that spectrum. It is just that most dynamic languages are also structural. In contrast (в отличие), I think that the manifest vs inferred spectrum exists for static languages but not for dynamic languages.

Nonetheless (тем не менее), that subsection (раздел) makes a great observation (наблюдение). For structural languages, the difference between static and dynamic languages is not just some guarantees about correctness. Dynamic languages check for type correctness at the last possible moment. (That is saying more than saying that the type check happens at runtime.) For example, consider a function with dead code that "doesn't type". If the type system were static, then this function cannot be executed, but if the type system were dynamic, then it could be executed. More practically, suppose the function is a simple `if-else` statement with code in the `else` branch that "doesn't type" and that the corresponding Boolean expression always evaluates to `true`. If the type system were static, then this function cannot be executed, but if the type system were dynamic, then it could be executed.

In my experience, the typical solution of a functional programmer would be to strengthen the input types so that the `else` branch can be proved by the compiler to be dead code and then delete the dead code. This approach makes this one function simpler, and I generally am in favor of this (я вообще за это). However, there is a sense (есть смысл) in which we can't always repeat this for the calling function. Otherwise (в противном случае), we would end up with a program that is provably correct, which is impossible for a Turning-complete language. Instead, I think the practical solution is to (at some appropriate level) short-circuit the computation when given input that is not known to be good and either do nothing or report back to the user that the input wasn't accepted.

- **Romain Deneau**

Using mostly both C# and TypeScript, two statically typed languages, I’ve experienced how it’s terser in TypeScript, essentially thanks to its type inference and its structural typing. I like the notion (понятие) of *“Ceremony”* you gave (вы дали) to describe this and the fact that it’s not correlated to the kind of typing, dynamic or static 👍

Still, TypeScript is more verbose (более многословный) than F#, as we can see with the following code translation from F# to TypeScript using object literal instead of tuple for the better support (лучшей поддержки) of the former (первого):

```hs
// const consume = (source: number[], quantity: number): number[]
const consume = (source: number[], quantity: number) =>
  source.reduce(({ acc, xs }, x) =>
    quantity <= acc
      ? { acc, xs: xs.concat(x) }
      : { acc: acc + x, xs },
    { acc: 0, xs: [] as number[] }
  ).xs;
```

Checks:

```hs
> consume(1, [1,2,3])
[2,3]
> consume(2, [1,2,3])
[3]
> consume(3, [1,2,3])
[3]
> consume(4, [1,2,3])
[]
```

As we can see, the code is a little more verbose (немного более многословен) than in JavaScript but still terser than in C#. The returned type is inferred as `number[]` but the as `number[]` is a pity (сожаление/жалость/печальный факт), necessary (нужно) because the inferred type of the empty array `[]` is `any[]`.

`consume` is not generic: TypeScript/JavaScript as only one primitive for numbers: `number`. It works for common scenarios but their no simple way to make it work with `BigInt`, for instance (например) using the union type `number | bigint`. The more pragmatic option (практичный вариант) would be to copy-paste, replacing `number` with `bigint` and `0` with `0n`.
