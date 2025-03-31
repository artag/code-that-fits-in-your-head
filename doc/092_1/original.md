# Apologies In Code

*Source: http://butunclebob.com/ArticleS.TimOttinger.ApologizeIncode*

I was thinking about the whole "bad comment smell" the other week when teaching a class. Well, actually I was talking out loud about it, and the whole class got mad at me. They felt that comments were very important and should be included. They put all kinds of information into comments, information that would otherwise be part of the version control log, mostly. There were explanatory comments all over the place. A week or two later, I was reviewing coding standards that demand comments for many purposes and in many places. I then attended a code review where the analyst called for more comments.

Bah, humbug.

## A comment is an apology.

A comment is an apology for not choosing a more clear name, or a more reasonable set of parameters, or for the failure to use explanatory variables and explanatory functions. Apologies for making the code unmaintainable, apologies for not using well-known algorithms, apologies for writing 'clever' code, apologies for not having a good version control system, apologies for not having finished the job of writing the code, or for leaving vulnerabilities or flaws in the code, apologies for hand-optimizing C code in ugly ways. And documentation comments are no better. In fact, I have my doubts about docstrings.

If something is hard to understand or inobvious, then someone *ought* to apologize for not fixing it. That's the worst kind of coding misstep. It's okay if I don't really get how something works so long as I know how to use it, and it really does work. But if it's too easy to misuse, you had better start writing. And if you write more comment than code, it serves you right. This stuff is supposed to be useful and maintainable, you know?

Is there any use of comments that are not apologies? I don't think so. I can't think of one. Is there any good reason to write a comment? Only if you've done something "wrong".

*And that's all I have to say about that -- F. Gump*

*PS I found that this was all discussed a long time ago on [Ward's Wiki](http://c2.com/cgi/wiki?ToNeedComments). It's not just me. -- Tim*
