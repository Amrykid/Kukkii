## Kukkii
Kukkii (Japanese for *Cookie*) is a simple, asynchronous Key-Value object system inspired by memcached and [Akavache](http://github.com/Akavache/Akavache) for UWP. The aim is to create something relatively simple, small and easy to use.

### Why Kukkii?
I created Kukkii out of laziness, really. I was using Akavache for a cross-platform .NET project (WPF and WP8) and I ran into problems because I was using the deprecated Akavache filesystem drivers. Instead of switching 
to the newer SQLite filesystem drivers, I decided to write my own system that works similarly, without the added dependencies such as Rx and Splat.

**I do recommend that you check out Akavache because it has more advanced features compared to the minimal ones provided by Kukkii.**

### How does it work?

There are two classes you will deal with directly. They are ```CookieRegistration``` and ```CookieJar``` (like Akavache's ```BlobCache```). 
You use ```CookieRegistration``` to setup the ```CookieJar``` singleton. If you try to change ```CookieRegistration``` after the ```CookieJar``` has been initialized,
 you will recieve an ```InvalidOperationException```.

```CookieJar``` has two to three different ```ICookieContainer``` instances. 
- The first is returned by a property called ```InMemory```. ```InMemory``` returns an object cache (```BasicCookieContainer```) that persists as long as the application is running. It is wiped when the application exits.
- The second is returned by a property called ```Device```. ```Device``` returns an object cache (```PersistentCookieContainer``` which inherits from ```BasicCookieContainer```) that can load and save it's contents to the device's storage.
- The last is returned by a property called ```Secure```. ```Secure``` returns an object cache (```EncryptedPersistentCookieContainer``` which inherits from ```PersistentCookieContainer```) that can load and save its contents to the device's storage. All content is encrypted using the system's data protection APIs.

### Setup

Setting Kukkii up is fairly straight forward, but not as straight forward as Akavache.

To begin, you **must** set ```CookieJar.ApplicationName``` (this may be moved to ```CookieRegistration``` later).

If you want to use Kukkii was a simple in-memory cache, configuration ends there. Simply access the in memory cache container by calling ```CookieJar.InMemory```.

#### Persistence
If you want to be able to save and load your cache on the disk, configuration becomes a little more involved. Depending on your project's target, you have to add a reference the matching FileSystem driver. If you're targeting the desktop, you have to add a reference to *Kukkii.FS.Windows*. If you're targeting Windows Phone 8, you need you need *Kukkii.FS.WP8*, etc.

Once you have the correct reference, you need to initialize the correct driver for your platform and assign it to ```CookieRegistration.FileSystemProvider```. You could do this using conditional compilation if you're sharing code across platforms.

```
#if DESKTOP
     CookieRegistration.FileSystemProvider = new Kukkii.FS.Windows.WindowsFileSystemProvider();
#else
     CookieRegistration.FileSystemProvider = ...
#endif
```

After that, you should be able to utilize ```CookieJar.Device``` to have a persisting cache.

**NOTE: Kukkii never *saves* data automatically. You must call the ```FlushAsync``` method if you want to save the current state of the cache/container.**

### Testing
If you find any bugs, please let me know on the issue tracker!
