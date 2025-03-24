# Is the 80 character line limit still relevant (уместно)?

*Source: https://richarddingwall.name/2008/05/31/is-the-80-character-line-limit-still-relevant*

Traditionally, it’s always been standard practice for programmers to wrap long lines of code so they don’t span more than 80 characters across the screen.

This is because, back in the bad old days, most computer terminals could only display 25 rows of 80 columns of text on screen at once. Any lines that were longer would simply trail off out of sight (выходили из поля зрения). To ensure this didn’t happen, programmers split up long lines of code so none of them exceeded (так что ни один из них не превышал) 80 characters.

Today, however, it’s pretty unlikely (довольно маловероятно) that you or anyone will still be writing code on an 80-column-width terminal. So why do we keep limiting (продолжаем ограничивать) our code to support them?

The answer, of course, is that **we don’t**. An 80 character limit has no relevance any more (больше не актуальны) with modern computer displays. The three-year-old Powerbook I am writing this post on, for example, can easily display over 200 characters across the screen, at a comfortable 10 point font size. That’s two and a half VT100s!

The reason this standard has stuck ("застрял") around all these years is because of the other benefits it provides.

## Advantages

Long lines that span too far across the monitor are hard to read. This is typography 101. The shorter your line lengths, the less your eye has to travel to see it.

If your code is narrow enough, you can fit two files on screen, side by side, at the same time. This can be very useful if you’re comparing files, or watching your application run side-by-side with a debugger in real time.

Plus, if you write code 80 columns wide, you can relax *knowing* that your code will be readable and maintainable on more-or-less (более-менее) any computer in the world.

Another nice side effect is that snippets of narrow code are much easier to embed into documents or blog posts.

## Disadvantages

Constraining (ограничение) the width of your code can sometimes require you to break up lines in unnatural (неестественных) places, making the code look awkward and disjointed (угловатым и несвязным). This particular problem is worse (проблема проявляется сильнее) with languages like Java and .NET, that tend to use long, descriptive identifier names.

Plus, the amount of usable space for code is also by impacted (влияет) by tab width. For example, if you’re using 8-space tabs and an 80-column page width, code within a class, a method, and an if statement will already have almost a third of the available space taken for indentation.

## Alternatives

Why 80? At work, my current project team uses a 120-character limit. We’ve all got 24″ wide-screen LCD displays, and 120 characters seems to be a good fit for our .NET/Visual Studio development environment, while still leaving ample (достаточно) whitespace.

There are a few factors you should think about, however. The average length of a line of code depends on what language and libraries you’re using. C generally has much shorter identifier names, and subsequently much shorter lines than, say, a .NET language.

It also depends on what sort of project you’re working on. For private and internal projects, use discretion (благоразумие). Find out what works best for your team, and follow it.

For open-source projects, or other situations where you don’t know who’s going to be reading your source code, tradition dictates that you stick with 80.

Another possibility is to make the limit a guideline (рекомендация), rather than a concrete rule (а не конкретное правило). Sometimes you might not *care* if a particular line continues out of sight. A long string literal, for example, isn’t going to cause the end of the world (не вызовет конец света) if you can’t see the whole thing on screen at once.

It may sound pedantic, but if you do decide to use something different, make sure everyone knows the rule, and obeys it (си ледуют ему). When there are unclear or conflicting rules, chaos ensues (приходит). You can end up with hilarious (веселые) games like **formatting tennis**, where every time a developer works on a piece of code, they first waste time reformatting the whole thing to reflect their own preferred coding style.

## Why bother (Зачем беспокоиться)?

Some of you might wonder why anyone would worry about such trivial details like the length of a line of code. And that’s cool. But, if like me, you believe that code isn’t finished until it not only works well, but looks beautiful too, balancing style with practicality is very important.
