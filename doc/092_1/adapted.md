# Apologies In Code (Извинения/оправдания в коде)

*Source: http://butunclebob.com/ArticleS.TimOttinger.ApologizeIncode*

I was thinking about the whole "bad comment smell" the other week when teaching a class. Well, actually I was talking out loud about it, and the whole class got mad at me (ну, на самом деле я громко говорил об этом, и весь класс разозлился на меня). They felt (чувствовали) that comments were very important and should be included. They put all kinds of information into comments, information that would otherwise (в противном случае) be part of the version control log, mostly. There were explanatory comments all over the place (пояснительные комментарии повсюду). A week or two later, I was reviewing coding standards that demand (требовуют) comments for many purposes and in many places. I then attended (присутствовал) a code review where the analyst called (аналитик призвал) for more (к еще бОльшим) comments.

Bah, humbug (вздор).

## A comment is an apology (извинение/оправдание).

A comment is an apology for not choosing a more clear name, or a more reasonable set of parameters, or for the failure to use explanatory variables and explanatory functions. Apologies for making the code unmaintainable, apologies for not using well-known algorithms, apologies for writing 'clever' (умного) code, apologies for not having a good version control system, apologies for not having finished the job of writing the code, or for leaving vulnerabilities (уязвимости) or flaws (недостатки) in the code, apologies for hand-optimizing C code in ugly ways. And documentation comments are no better. In fact, I have my doubts (у меня есть сомнения) about docstrings.

If something is hard to understand or inobvious, then someone *ought* (должен) to apologize for not fixing it (извиниться за то, что не исправил это). That's the worst kind of coding misstep (ошибка). It's okay if I don't really get how something works so long as I know how to use it, and it really does work. But if it's too easy to misuse (неправильно использовать), you had better start writing. And if you write more comment than code, it serves you right. This stuff is supposed to be useful and maintainable, you know?

Is there any use of comments that are not apologies? I don't think so. I can't think of one. Is there any good reason to write a comment? Only if you've done something "wrong".

*And that's all I have to say about that -- F. Gump*

*PS I found that this was all discussed a long time ago on [Ward's Wiki](http://c2.com/cgi/wiki?ToNeedComments). It's not just me. -- Tim*
