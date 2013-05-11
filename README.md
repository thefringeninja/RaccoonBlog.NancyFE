RaccoonBlog.NancyFE
===================

Nancy front end for the RavenDB Blog Engine

Just a little sample / demo app I threw together showing Nancy + Cassette + RavenDB. This code is meant to run on appharbor.

The idea is you setup RaccoonBlog + Live Writer on your local machine but point the data connection to your production RavenDB instance. The only code you run on the server is the Nancy front end.

Why not just use the MVC front end?
===================

Not really a fan of MVC or the MVC pattern frankly. Just don't jive with the way I think of the web. 

Also, it seemed as if it would be too much effort to get the skin / theme I wanted. I want to maximize performance within a tight budget. You can easliy configure cloudfront to use the blog engine as its origin server. Then you leverage Cassette's html rewriting idea and BAM - huge reduction in remote calls.

Once you start walking the SDHP, you can't go back!

What happened to Comments?
===================

I don't feel like configuring an anti comment spam service or CAPTCHA; It's way less effort to let DISQUS manage that for me. Plus I can now get rid of the PostComments document collection.
