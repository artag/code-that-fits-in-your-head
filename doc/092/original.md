# A heuristic for formatting code according to the AAA pattern

*Source: https://blog.ploeh.dk/2013/06/24/a-heuristic-for-formatting-code-according-to-the-aaa-pattern/*

*This article describes a rule of thumb for formatting unit tests.*

The Arrange Act Assert (AAA) pattern is one of the most fundamental and important patterns for writing maintainable unit tests. It states that you should separate each test into three phases (Arrange, Act, and Assert).

Like most other code, unit tests are read more than they are written, so it's important to make the tests readable. This article presents one way to make it easy for a test reader easily to distinguish the three AAA phases of a test method.

## The way of AAA

The technique is simple:

- As long as there are less than three lines of code in a test, they appear without any special formatting or white space.

- When a test contains more than three lines of code, you separate the three phases with a blank line.

- When a single phase contains so many lines of code that you'll need to divide it into subsections to make it readable, you should explicitly mark the beginning of each phase with a code comment.

This way avoids the use of comments until they are unavoidable; at that time, you should consider whether the need for a comment constitutes a code smell.

## Motivating example

Many programmers use the AAA pattern by explicitly demarking each phase with a code comment:

```csharp
[Fact]
public void UseBasketPipelineOnExpensiveBasket()
{
    // Arrange
    var basket = new Basket(
        new BasketItem("Chocolate", 50, 3),
        new BasketItem("Gruyere", 45.5m, 1),
        new BasketItem("Barolo", 250, 2));
    CompositePipe<Basket> pipeline = new BasketPipeline();
    // Act
    var actual = pipeline.Pipe(basket);
    // Assert
    var expected = new Basket(
        new BasketItem("Chocolate", 50, 3),
        new BasketItem("Gruyere", 45.5m, 1),
        new BasketItem("Barolo", 250, 2),
        new Discount(34.775m),
        new Vat(165.18125m),
        new BasketTotal(825.90625m));
    Assert.Equal(expected, actual);
}
```

Notice the use of code comments to indicate the beginning of each of the three phases.

Given an example like the above, this seems like a benign approach, but mandatory use of code comments starts to fall apart when tests are very simple.

Consider this Structural Inspection test:

```csharp
[Fact]
public void SutIsBasketElement()
{
    // Arrange
    // Act?
    var sut = new Vat();
    // Assert
    Assert.IsAssignableFrom<IBasketElement>(sut);
}
```

Notice the question mark after the `// Act` comment. It seems that the writer of the test was unsure if the act of creating an instance of the System Under Test (SUT) constitutes the Act phase.

You could just as well argue that creating the SUT is part of the Arrange phase:

```csharp
[Fact]
public void SutIsBasketElement()
{
    // Arrange
    var sut = new Vat();
    // Act
    // Assert
    Assert.IsAssignableFrom<IBasketElement>(sut);
}
```

However, now the Act phase is empty. Clearly, using code comments to split two lines of code into three phases is not helpful to the reader.

## Three lines of code and less

Here's a simpler alternative:

```csharp
[Fact]
public void SutIsBasketElement()
{
    var sut = new Vat();
    Assert.IsAssignableFrom<IBasketElement>(sut);
}
```

When there's only two lines of code, the test is so simple that you don't need help from code comments. If you wanted, you could even reduce that test to a single line of code, by inlining the `sut` variable:

```csharp
[Fact]
public void SutIsBasketElement()
{
    Assert.IsAssignableFrom<IBasketElement>(new Vat());
}
```

Such a test is either a degenerate case of AAA where one or more phase is empty, or else it doesn't really fit into the AAA pattern at all. In these cases, code comments are only in the way, so it's better to omit them.

Even if you have a test that you can properly divide into the three distinct AAA phases, you don't need comments or formatting if it's only three lines of code:

```csharp
[Theory]
[InlineData("", "", 1, 1, 1, 1, true)]
[InlineData("foo", "", 1, 1, 1, 1, false)]
[InlineData("", "bar", 1, 1, 1, 1, false)]
[InlineData("foo", "foo", 1, 1, 1, 1, true)]
[InlineData("foo", "foo", 2, 1, 1, 1, false)]
[InlineData("foo", "foo", 2, 2, 1, 1, true)]
[InlineData("foo", "foo", 2, 2, 2, 1, false)]
[InlineData("foo", "foo", 2, 2, 2, 2, true)]
public void EqualsReturnsCorrectResult(
    string sutName,
    string otherName,
    int sutUnitPrice,
    int otherUnitPrice,
    int sutQuantity,
    int otherQuantity,
    bool expected)
{
    var sut = new BasketItem(sutName, sutUnitPrice, sutQuantity);
    var actual = sut.Equals(
        new BasketItem(otherName, otherUnitPrice, otherQuantity));
    Assert.Equal(expected, actual);
}
```

Three lines of code, and three phases of AAA; I think it's obvious what goes where - even if this single test method captures eight different test cases.

## Simple tests with more than three lines of code

When you have more than three lines of code, you'll need to help the reader identify what goes where. As long as you can keep it simple, I think that you accomplish this best with simple whitespace:

```csharp
[Fact]
public void UseBasketPipelineOnExpensiveBasket()
{
    var basket = new Basket(
        new BasketItem("Chocolate", 50, 3),
        new BasketItem("Gruyere", 45.5m, 1),
        new BasketItem("Barolo", 250, 2));
    CompositePipe<Basket> pipeline = new BasketPipeline();
 
    var actual = pipeline.Pipe(basket);
 
    var expected = new Basket(
        new BasketItem("Chocolate", 50, 3),
        new BasketItem("Gruyere", 45.5m, 1),
        new BasketItem("Barolo", 250, 2),
        new Discount(34.775m),
        new Vat(165.18125m),
        new BasketTotal(825.90625m));
    Assert.Equal(expected, actual);
}
```

This is the same test as in the motivating example, only with the comments removed. The use of whitespace makes it easy for you to identify three phases in the method, so comments are redundant.

As long as you can express each phase without using whitespace *within* each phase, you can omit the comments. The only whitespace in the test marks the boundaries between each phase.

## Complex tests requiring more whitespace

If your tests grow in complexity, you may need to divide the code into various sub-phases in order to keep it readable. When this happens, you'll have to resort to using code comments to demark the phases, because use of only whitespace would be ambiguous:

```csharp
[Fact]
public void PipeReturnsCorrectResult()
{
    // Arrange
    var r = new MockRepository(MockBehavior.Default)
    {
        DefaultValue = DefaultValue.Mock
    };
 
    var v1Stub = r.Create<IBasketVisitor>();
    var v2Stub = r.Create<IBasketVisitor>();
    var v3Stub = r.Create<IBasketVisitor>();
 
    var e1Stub = r.Create<IBasketElement>();
    var e2Stub = r.Create<IBasketElement>();
    e1Stub.Setup(e => e.Accept(v1Stub.Object)).Returns(v2Stub.Object);
    e2Stub.Setup(e => e.Accept(v2Stub.Object)).Returns(v3Stub.Object);
 
    var newElements = new[]
    {
        r.Create<IBasketElement>().Object,
        r.Create<IBasketElement>().Object,
        r.Create<IBasketElement>().Object,
    };
    v3Stub
        .Setup(v => v.GetEnumerator())
        .Returns(newElements.AsEnumerable().GetEnumerator());
 
    var sut = new BasketVisitorPipe(v1Stub.Object);
    // Act
    var basket = new Basket(e1Stub.Object, e2Stub.Object);
    Basket actual = sut.Pipe(basket);
    // Assert
    Assert.True(basket.Concat(newElements).SequenceEqual(actual));
}
```

In this example, the Arrange phase is so complicated that I've had to divide it into various sections in order to make it just a bit more readable. Since I've had to use whitespace to indicate the various sections, I need another mechanism to indicate the three AAA phases. Code comments is an easy way to do this.

As [Tim Ottinger described back in 2006](http://butunclebob.com/ArticleS.TimOttinger.ApologizeIncode), code comments are apologies for not making the code clear enough. A code comment is a code smell, because it means that the code itself isn't sufficiently self-documenting. This is also true in this case.

Whenever I need to add code comments to indicate the three AAA phases, an alarm goes off in my head. Something is wrong; the test is too complex. It would be better if I could refactor either the test or the SUT to become simpler.

When TDD’ing, I tend to accept the occasional complicated unit test method, but if I seem to be writing too many complicated unit tests, it's time to stop and think.

## Summary

In [Growing Object-Oriented Software, Guided by Tests](http://amzn.to/VI81bP), one of the most consistent pieces of advice is that you should listen to your tests. If your tests are too hard to write; if your tests are too complicated, it's time to consider alternatives.

How do you know when a test has become too complicated? If you need to added code comments to it, it probably is.

*This article first appeared in the [The Developer No 2/2013](http://issuu.com/developermagazine/docs/developer_ndc_2013_web). It's reprinted here with kind permission.*

## Comments

- **Romain Vasseur**

Currently reading your last book **Code That Fits in Your Head** (Lucky draw, thank you for writing!!) and find this section. I used several techniques to refactor those unbalanced structure. A recent one is to leverage `local function` to gather noise, top structure acting like `TOC` when bottom one provides insight for those interested. More or less like `template method` but keeping stuff internally. I find it particularly useful when such noise is *one-shot*. Extracting it into `private` method or `fixture` does not smell good to me.

```csharp
[Theory]
[InlineData(194, 107, 37, "#C26B25")]
[InlineData(66, 138, 245, "#428AF5")]
public void Mapper(int r, int g, int b, string hex)
{
    var (sut, source, expected) = Arrange();
    var res = sut.Convert(source);
    Assert.Equal(expected, source);

    (Sut sut, Color1 color, Color2 color) Arrange()
    {
      // Do complex or verbose stuff eg:
      // Create C1 from hex
      // Create c2 from {r,g,b}
      // Create SUT
      return (s, c1, c2);
    }
}
```

I am used to apply the same pattern to process impure method: I provide a `local pure function` that I leverage above using instance filed, I/O, ... It is a good first step instead of right away create a private helper or equivalent. Then, if it apperas it can be reused, you only have to promote the `local function`.

- **Mark Seemann**

Romain, thank you for writing. Local functions can indeed be useful (also) in unit tests. The main use, as you imply, is as a cheaper alternative to the Template Method pattern. The main benefit, compared to private helper methods, is that you limit the scope of the method.

On the other hand, a local function still involves more [ceremony](https://blog.ploeh.dk/2019/12/16/zone-of-ceremony) than a lambda expression, which is often a better alternative.

With regards to your particular example, do you gain anything from moving the Arrange phase down?

Indeed, the three AAA phases may be more apparent. You're also following the principle of leading with the big overview, and then leaving the implementation details for later. I can never remember who originally described this principle for organising code, but I find it useful. (It's one the main challenges with F#, because of its single-pass compiler. That language feature, however, [provides some other great benefits](https://blog.ploeh.dk/2015/04/15/c-will-eventually-get-all-f-features-right).)

Still, isn't this mostly symptomatic relief? I wouldn't mind doing something like this if I could think of nothing better, but I'd still view the need for a complex Arrange phase as being a code smell. Not a test smell, but as feedback that the System Under Test is too complicated.
