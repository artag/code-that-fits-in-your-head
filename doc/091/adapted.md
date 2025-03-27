# 10 tips for better Pull Requests

*Source: https://blog.ploeh.dk/2015/01/15/10-tips-for-better-pull-requests*

*Making a good Pull Request involves more (включает в себя больше) than writing good code.*

The Pull Request model has turned out (оказался) to be a great way to build software in teams - particularly for distributed teams; not only for open source development, but also in enterprises. Since some time around 2010, I've been reviewing Pull Requests both for my open source projects, but also as a team member for some of my customers, doing closed-source software, but still using the Pull Request work flow internally.

During all of that time, I've seen many great Pull Requests, and some that needed some work.

A good Pull Request involves more than just some code. In most cases, there's one or more reviewer(s) involved, who will have to review your Pull Request in order to evaluate (оценить) whether it's a good fit for inclusion (включения) in the code base. Not only must you produce good code, but you must also cater (обслуживать) to the person(s) doing the review.

Here's a list of tips to make your Pull Request better. It isn't exhaustive (исчерпывающий), but I think it addresses some of the more important aspects of creating a good Pull Request.

## 1. Make it small

A small, focused Pull Request gives you the best chance of having it accepted.

The first thing I do when I get a notification about a Pull Request is that I look it over to get an idea about its size. It takes time to properly review a Pull Request, and in my experience, the time it takes is exponential to the size; the relationship certainly (отношение несомненно) isn't linear.

If I get a big Pull Request for an open source project, I do realize (понимаю) that the submitter has most likely already put in substantial (существенную) work in his or her spare time (свободное время), so I do go to some lengths to review a big Pull Request, even if I think it's too big - particularly when it looks like it's a first-time contributor. Still, if the Pull Request is big, I'll need to *schedule* time to review it: I can't review a big chunk of code using five minutes here and five minutes there; I need contiguous time to do that. This already introduces a delay into the review process.

If I get a big Pull Request in a professional setting (i.e. where the submitter is being paid to write the code), I often **reject** the Pull Request simply because of the size of it.

How small is small enough? Obviously, it depends on what the Pull Request is about, but a Pull Request that touches less than a dozen (**дюжина**) files isn't too bad.

## 2. Do only one thing

Just as the [Single Responsibility Principle](http://en.wikipedia.org/wiki/Single_responsibility_principle) states that a class should have only one responsibility, so should a Pull Request address only a single concern (один предмет для рассмотрения/вещь/тема).

Imagine, as a counter-example, that you submit a Pull Request that addresses three independent, separate concerns (let's call them A, B, and C). The reviewer may immediately agree with you that A and C are valid concerns, and that your solution is correct. However, the reviewer has issues (спорный вопрос) with your B concern. Perhaps he or she thinks it's not a concern at all (совсем не), or she disagrees with the way you've addressed (решили) it.

This becomes the start of a lengthy discussion about concern B, and how it's being addressed. This discussion can go on for days (particularly if you're in different time zones), while you attempt (пытаетесь) to come to agreement; perhaps you'll need to make changes to your Pull Request to address the reviewer's concerns. This all takes time.

It may, in fact (фактически), take so much time that other commits have been merged into master in the meantime (тем временем), and your Pull Request has fallen so much behind that it no longer can be automatically merged. Welcome to Merge Hell.

All that time, your perfectly acceptable solutions to the A and C concerns are sitting idly (лениво) in your Pull Request, adding absolutely no value to the overall code base.

Instead, submit three independent Pull Requests that address respectively A, B, and C. If you do that, the reviewer who agrees with A and C will immediately accept two of those three Pull Requests. In this way, your non-controversial (неоспоримый) contributions (вклад) can immediately add value to the code base.

The more concerns you address in a single Pull Request, the bigger the risk that at least one of them will block acceptance (принятие) of your contribution. Do only one thing per Pull Request. It also helps you make each Pull Request smaller.

## 3. Watch your line width

The reviewer of your Pull Request will most likely be reviewing your contribution using a diff tool. Both [GitHub](https://github.com/) and [Stash](https://www.atlassian.com/software/stash) provide browser-based diff views for reviewing. A reviewer can even configure the diff view to be side-by-side; it makes it much easier to understand what changes are included in the contribution, but it also means that the code must be readable on half a screen.

If you have wide lines, you force the reviewer to scroll horizontally.

There are many [reasons to keep line width below 80 characters](http://richarddingwall.name/2008/05/31/is-the-80-character-line-limit-still-relevant); making your code easy to review just adds another reason to that list.

## 4. Avoid re-formatting

You may feel the urge (испытать сильное желание) to change the formatting of the existing code to fit 'your' style. Please abstain (**воздержитесь**).

Every byte you change in the source code shows up in the diff views. Some diff viewers have options to ignore changes of white space, but even with this option on, there are limits to what those diff viewers can ignore. Particularly, they can't ignore if you move code around, so please don't do that.

If you really need to address white space issues, move code around within files, change formatting, or do other stylistic changes to the code, please do so in an isolated pull request that does only that, and state so in your Pull Request comment.

## 5. Make sure the code builds

Before submitting a Pull Request, build it on your own machine. True, *works on my machine* isn't particularly useful (не особенно полезен), but it's a minimum bar. If it *doesn't work on your machine*, it's unlikely to work on other machines as well.

Watch out for compiler warnings. They may not prevent you from compiling, so you may not notice them if you don't explicitly look for them. However, if your Pull Request causes (more) compiler warnings, a reviewer may reject it; I do.

If the project has a build script, try to run that, and only submit your pull request if the build succeeds. In many of my open source projects, I have a build script that (among other things) **treats warnings as errors**. Such a build script may automate or implement various rules for that particular code base. Use it before submitting, because the reviewer most likely will use it before merging your branch.

## 6. Make sure all tests pass

Assuming that the code base in question has automated tests, make sure all tests pass before submitting a Pull Request.

This should go without saying (Это должно быть само собой разумеющимся), but I regularly receive Pull Requests where one or more tests are failing.

## 7. Add tests

Again, assuming that the code in question already has automated (unit) tests, do add tests for the code you submit.

It doesn't often happen that I receive a Pull Request without tests, but when I do, I often reject it.

This isn't a hard rule. There are various cases where you may need to add code without test coverage (e.g. when adding a [Humble Object](http://xunitpatterns.com/Humble%20Object.html)), but if it can be tested, it should be tested.

You'll need to follow the testing strategy already established (принятой) for the code base in question (для обсуждаемой кодовой базы).

## 8. Document your reasoning (документируйте свои рассуждения)

Self-documenting code rarely is.

Yes, [code comments are apologies](http://butunclebob.com/ArticleS.TimOttinger.ApologizeIncode), and I definitely prefer well-named operations, types, and values over comments. Still, when writing code, you often have to make decisions that aren't self-evident (самоочевидны) (particularly when dealing with Business 'Logic').

Document why you wrote the code in the way you did (почему вы написали код так); not what it does (а не то что он делает).

My preferred priority is this:

1. **Self-documenting code**: You *can* make some decisions about the code self-documenting. [Clean Code](http://amzn.to/XCJi9X) is literally a book on how to do that.

2. **Code comments**: If you can't make the code sufficiently (достаточно) self-documenting, add a code comment. At least, the comment is co-located (рядом) with the code, so even in the unlikely event that you decide to change version control system, the comment is still preserved (сохранится). [Here's an example where I found a comment more appropriate than attempting to design my way out of the problem](https://github.com/GreanTech/AtomEventStore/blob/e5fe679c08abb6fe108509117651d29dce17e270/AtomEventStore/AtomEventStorage.cs#L68-L71).

3. **Commit messages**: Most version control systems give you the opportunity (возможность) to write a commit message. Most people don't bother putting anything other than a bare minimum into these, but you can document your reasoning (рассуждения) here as well. Sometimes, you'll need to explain why you're doing things in a certain order. This doesn't fit well in code comments, but is a good fit for a commit message. As long as you keep using the same version control system, you preserve (сохраняете) these commit messages, but they're once removed from the actual source code, and you may loose the messages if you change to another source control system. [Here's an example where I felt the need to write an extensive commit message](https://github.com/GreanTech/AtomEventStore/commit/615cdee2c4d675d412e6669bcc0678655376c4d1) - коммит с комментарием, but [I don't always do that](https://github.com/GreanTech/AtomEventStore/commit/e5fe679c08abb6fe108509117651d29dce17e270) - коммит без комментария.

4. **Pull Request comments**: Rarely, you may find yourself in a situation where none of the above options are appropriate (ни одно не подходит). In Pull Request management systems such as GitHub or Stash, you can also add custom messages to the Pull Request itself. This message is twice removed from the actual source code, and will only persist as long as you keep using the same host. If you move from e.g. CodePlex to GitHub, you'll loose those Pull Request messages. Still, occasionally (изредка), I find that I need to explain myself to the reviewer, but the explanation involves something external to the source code anyway. [Here's an example where I found that a reasonable approach](https://github.com/GreanTech/AtomEventStore/pull/70) - пример комментариев в Pull Request.

You don't need to explain the obvious, but do consider erring (ошибки) on the side of caution (предупреждений). What's obvious to you today may not be obvious to anyone else, or to you in three months.

## 9. Write well

Write good code, but also write good prose (как пишите хорошую прозу). This is partly subjective, but there are rules for both code and prose. Code has correctness rules: if you break them, it doesn't compile (or, for interpreted languages, it fails at run-time).

The same goes for the prose you may add: Code comments. Commit messages. Pull Request messages.

Please use correct spelling, grammar, and punctuation. If you don't, your prose is harder to understand, and your reviewer is a human being.

## 10. Avoid thrashing (загрязнений)

Sometimes, a reviewer will point out (указывает на) various issues (проблемы) with your Pull Request, and you'll agree to address them.

This may cause you to add more commits to your Pull Request branch. There's nothing wrong with that per se. However, this *can* lead to unwarranted thrashing (необосованному загрязнению).

As an example, your pull request may contain five commits: A, B, C, D, and E. The reviewer doesn't like what you did in commits B and C, so she asks you to remove that code. Most people do that by checking out (проверяют) their pull request branch and deleting the offending (проблемный) code, adding yet another commit (F) to the commit list: [A, B, C, D, E, F]

Why should we have to merge a series of commits that first adds unwanted code, and then removes it again? It's just thrashing; it doesn't add any value.

Instead, remove the offending (проблемный) commits, and force push your modified branch: [A, D, E]. While under review, you're the sole owner of that branch, so you can modify and force push it all you want.

Another example of thrashing that I see a lot is when a Pull Request is becoming old (often due to lengthy discussions): in these cases, the author regularly merges his or her branch with *master* to keep the Pull Request branch up to date.

Again: why do I have to look at all those merge commits? You are the sole owner of that branch. Just [rebase](http://git-scm.com/book/en/v2/Git-Branching-Rebasing) your Pull Request branch and force push it. The resulting commit history will be cleaner.

## Summary

One or more persons will review your Pull Request. Don't make your reviewer work.

The more you make your reviewer work, the greater the risk is that your Pull Request will be rejected.

### Пример commit

- https://github.com/GreanTech/AtomEventStore/commit/615cdee2c4d675d412e6669bcc0678655376c4d1

Commit 615cdee с комментарием:

```text
Suppressed exceptions when updating the index.
The motivation is that when a new page is created, the second of two
update operations may fail, which means that the index document's last
link becomes stale. The system is still 'almost consistent' in the sense
that all other links point to correct documents, apart from the last link
in the index document. This link points to an existing document, but that
document is no longer the most recent document.

However, if the update operation of the index throws an exception, a
client may interpret that as a failure to write an event to storage, and
thus may retry the operation. This would be an error, because the event
has been written, and thus, a duplicate event would be written if the
operation is retried.

For that reason, while the first update operation (of the new previous
page) must succeed or throw an exception, the second update (of the index
page) must silently fail if an error occurs.
```

- https://github.com/GreanTech/AtomEventStore/commit/e5fe679c08abb6fe108509117651d29dce17e270

Commit e5fe679 без комментария:

```text
Removed a redundant return variable.
```

## Comments

- **Sam Frances**

How do you balance the advice to write small, focused Pull Requests with the practical necessity (необходимостью) of sometimes bundling refactoring in with features? Especially given the fact that most workplaces inevitably prioritise (неизбежно предпочитают) merging features.

- **Mark Seemann**

Sam, thank you for writing. Even without refactoring, it's common that a feature is so large that you can't implement it as a single, focused pull request. The best way to address that issue is to hide the work in progress behind a feature flag. You can do the same with refactoring.

(Добавляется кусок нового кода, который выключается отдельным флагом/параметром из конфигурации).

As Kent Beck puts it:

>"for each desired change, make the change easy (warning: this may be hard), then make the easy change"
>"(для каждой желаемой правки, делайте правку легко (предупреждение: это может быть сложно), затем делайте легкую правку")
>
>[Kent Beck twitter](https://x.com/kentbeck/status/250733358307500032)

You may need to first refactor to 'make room' for the new feature. I'd often put that in an isolated pull request and send that first. If anyone complains (жалуется) that I'm doing refactoring work instead of feature work, I'd truthfully (правдиво) respond that I'm doing the refactoring in order to be able to implement the feature.

I consider this to be part of being professional. It's how software should be developed, and [I think that non-technical stakeholders should have little to say about *how* things are done](https://blog.ploeh.dk/2019/03/18/the-programmer-as-decision-maker). You don't have to tell them every little detail about how you write code. You shouldn't have to ask for permission to do this, and you shouldn't have to inform them that that's what you're doing.

[My new book](https://blog.ploeh.dk/2021/06/14/new-book-code-that-fits-in-your-head) contains a realistic and practical example of a feature developed behind a feature flag.
