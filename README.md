<h1>View Wrapper</h1>
<br/>
Programmatical Razor Template Idea.

For those that are curious,
this project is a prototype of building Views programmatically using the RazorViewEngine.

*Goals:*
<br/>
Scoping data down to the component/element, so that the elements only access what they are given.
Views generated no longer (probably) require if statements to make conditional views, you just provide a set of components and the view is generated based on that.
If you use strict typing to do this, you would not need if statements.

We could call it branchless View templates.

*Plans:*
<br/>
Caching based on component relationship hashing, rather than the content of the template which generates a hash.
If you make a hash to use as a template key, you're literally spending the time calculating the resulting hash to use as a key, which isn't really worth using as a key to get the cached object.

There's no license on this project yet.
If you understand how this works, you can type it yourself for now. Sorry lol.
This is a general idea. Not proprietary. If I make a framework out of it, you'll know.
