## Kukkii
Kukkii (japanese for *Cookie*) is a simple KeyValue system inspired by memcached and [Akavache](http://github.com/Akavache/Akavache). The aim is to create something relatively simple, small and easy to use.

### Why Kukkii?
I created Kukkii out of laziness, really. I was using Akavache for a cross platform project (WPF and WP8) and I ran into problems because I was using the deprecated Akavache filesystem drivers. Instead of switching 
to the newer SQLite filesystem drivers, I decided to write my own system that works similarly, without the added dependencies such as Rx and Splat.

I do recommend you check out Akavache because it has more advanced features compared to the minimal ones provided by Kukkii.

### How does it work?

There are two classes you will deal with directly. They are ```CookieRegistration``` and ```CookieJar``` (like Akavache's ```BlobCache```).