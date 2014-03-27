## Kukkii
Kukkii (japanese for *Cookie*) is a simple KeyValue system inspired by memcached and [Akavache](http://github.com/Akavache/Akavache). The aim is to create something relatively simple, small and easy to use.

### Why Kukkii?
I created Kukkii out of laziness, really. I was using Akavache for a cross platform project (WPF and WP8) and I ran into problems because I was using the deprecated Akavache filesystem drivers. Instead of switching 
to the newer SQLite filesystem drivers, I decided to write my own system that works similarly, without the added dependencies such as Rx and Splat.

I do recommend you check out Akavache because it has more advanced features compared to the minimal ones provided by Kukkii.

### How does it work?

There are two classes you will deal with directly. They are ```CookieRegistration``` and ```CookieJar``` (like Akavache's ```BlobCache```). 
You use ```CookieRegistration``` to setup the ```CookieJar``` singleton. If you try to change ```CookieRegistration``` after the ```CookieJar``` has been initialized,
 you will recieve an ```InvalidOperationException```.

```CookieJar``` has two to three different ```ICookieContainer``` instances. 
- The first is returned by a property called ```InMemory```. ```InMemory``` returns an object cache (```BasicCookieContainer```) that persists as long as the application is running. It is wiped when the application exits.
- The second is returned by a property called ```Device```. ```Device``` returns an object cache (```PersistentCookieContainer``` which inherits from ```BasicCookieContainer```) that can load and save it's contents to the device's storage.
- The last is returned by a property called ```Secure```. ```Secure``` returns an object cache (```EncryptedPersistentCookieContainer``` which inherits from ```PersistentCookieContainer```) that can load and save its contents to the device's storage. All content is encrypted using the system's data protection APIs.
