# A red-green-refactor checklist

*Source: https://blog.ploeh.dk/2019/10/21/a-red-green-refactor-checklist/*

*A simple read-do checklist for test-driven development.*

I recently read [The Checklist Manifesto](https://amzn.to/35Wk5yD), a book about the power of checklists. That may sound off-putting and tedious, but I actually [found it inspiring](https://www.goodreads.com/review/show/2949987528). It explains how checklists empower skilled professionals to focus on difficult problems, while preventing avoidable mistakes.

Since I read the book with the intent to see if there were ideas that we could apply in software development, I thought about checklists one might create for software development. Possibly the simplest checklist is one that describes the *red-green-refactor* cycle of test-driven development.

## Types of checklists

As the book describes, there's basically two types of checklists:

- **Do-confirm**. With such a checklist, you perform a set of tasks, and then subsequently, at a sufficient pause point go through the checklist to verify that you remembered to perform all the tasks on the list.

- **Read-do**. With this type of checklist, you read each item for instructions and then perform the task. Only when you've performed the task do you move on to the next item on the list.

I find it most intuitive to describe the red-green-refactor cycle as a *read-do* list. I did, however, find it expedient to include a *do-confirm* sub-list for one of the overall steps.

This list is, I think, mostly useful if you're still learning test-driven development. It can be easily internalised. As such, I offer this for inspiration, and as a learning aid.

## Red-green-refactor checklist

Read each of the steps in the list and perform the task.

1. Write a failing test.

- Did you run the test?

- Did it fail?

- Did it fail because of an assertion?

- Did it fail because of the last assertion?

2. Make all tests pass by doing the simplest thing that could possibly work.

3. Consider the resulting code. Can it be improved? If so, do it, but make sure that all tests still pass.

4. Repeat

Perhaps the most value this checklist provides isn't so much the overall *read-do* list, but rather the subordinate *do-confirm* list associated with the first step.

I regularly see people write failing tests as an initial step. The reason the test fails, however, is because the implementation throws an exception.

## Improperly failing tests

Consider, as an example, the first test you might write when doing the [FizzBuzz](https://en.wikipedia.org/wiki/Fizz_buzz) kata.

```csharp
[Fact]
public void One()
{
    string actual = FizzBuzz.Convert(1);
    Assert.Equal("1", actual);
}
```

I wrote this test first (i.e. before the 'production' code) and used Visual Studio's refactoring tools to generate the implied type and method.

When I run the test, it fails.

Further investigation, however, reveals that the test fails when `Convert` is called:

```text
Ploeh.Katas.FizzBuzzKata.FizzBuzzTests.One
    Source: FizzBuzzTests.cs line: 11
    Duration: 8 ms

  Message:
    System.NotImplementedException : The method or operation is not implemented.
  Stack Trace:
    at FizzBuzz.Convert(Int32 i) in FizzBuzz.cs line: 9
    at FizzBuzzTests.One() in FizzBuzzTests.cs line: 13
```

This is hardly surprising, since this is the current 'implementation':

```csharp
public static string Convert(int i)
{
    throw new NotImplementedException();
}
```

This is what the subordinate `do-confirm` checklist is for. Did the test fail because of an assertion? In this case, the answer is no.

This means that you're not yet done with the `read` phase.

## Properly failing tests

You can address the issue by changing the `Convert` method:

```csharp
public static string Convert(int i)
{
    return "";
}
```

This causes the test to fail because of an assertion:

```text
Ploeh.Katas.FizzBuzzKata.FizzBuzzTests.One
    Source: FizzBuzzTests.cs line: 11
    Duration: 13 ms

  Message:
    Assert.Equal() Failure
              ↓ (pos 0)
    Expected: 1
    Actual:
              ↑ (pos 0)
  Stack Trace:
    at FizzBuzzTests.One() in FizzBuzzTests.cs line: 14
```

Not only does the test fail because of an assertion - it fails because of the last assertion (since there's only one assertion). This completes the `do-confirm` checklist, and you're now ready to make the simplest change that could possibly work:

```csharp
public static string Convert(int i)
{
    return "1";
}
```

This passes the test suite.

## Conclusion

It's important to see tests fail. Particularly, it's important to see tests fail for the reason you expect them to fail. You'd be surprised how often you inadvertently write an [assertion that can never fail](https://blog.ploeh.dk/2019/10/14/tautological-assertion).

Once you've seen the test fail for the proper reason, make it pass.

Finally, refactor the code if necessary.

## Comments

- **Tyson Williams**

I remember the first time that I realized that I did the red step wrong because my test didn't fail for the intended reason (i.e. it didn't fail because of an assertion). Before that, I didn't realize that I needed to This is a nice programming checklist. Thanks for sharing it :)

>3. Consider the resulting code. Can it be improved? If so, do it, but make sure that all tests still pass.
>
>Finally, refactor the code if necessary.

If I can be a [Devil's advocate](https://blog.ploeh.dk/2019/10/07/devils-advocate/) for a moment, then I would say that code can always be improved and few things are necessary. In all honesty though, I think the refactoring step is the most interesting. All three steps include aspects of science and art, but I think the refactor step includes the most of both. On the one hand, it is extremely creative and full of judgement calls about what code should be refactored and what properties the resulting code should have. On the other hand, much of the work of how to (properly) refactor is laid out in books like [Martin Fowler's Refacoring](https://www.amazon.com/Refactoring-Improving-Existing-Addison-Wesley-Signature/dp/0134757599) and is akin to algebraic manipulations of an algebraic formula.

In other words, I feel like there is room to expand on this checklist in the refactor step. Do you have any thoughts about you might expand it?

- **Mark Seemann**

Tyson, thank you for writing. I agree that the *refactoring* step is both important and compelling. I can't, however, imagine how a checklist would be useful.

The point of *The Checklist Manifesto* is that checklists help identify avoidable mistakes. A checklist isn't intended to describe an algorithm, but rather to make sure that crucial steps aren't forgotten.

Another important point from *The Checklist Manifesto* is that a checklist is only effective if it's not too big. A checklist that tries to cover every eventuality isn't useful, because then people don't follow it.

As you write, refactoring is a big topic, covered by several books. All the creativity and experience that goes into refactoring doesn't seem like something that can easily be expressed as an effective checklist.

I don't mind being proven wrong, though, so by all means give it a go.
