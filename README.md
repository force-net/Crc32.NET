# Crc32.NET

Optimized and fast managed implementation of Crc32 algorithm for .NET and .NET Core. 

*(But if you need, I can add native implementation which is twice faster .NET version with transparent .net wrapper)*

This library is port of [Crc32C.NET](https://crc32c.angeloflogic.com/) by Robert Važan but for Crc32 algorithm. 

If you do not not catch the difference, it is *C* (Castagnoli). I recommend to use Crc32C, not usual CRC32, because it is faster (up to 20GB/s) and slightly better in error detection. But if you need exactly Crc32, this library is the best choice.

### Performance

Library | Speed
--------|-------
[CH.Crc32](https://github.com/tanglebones/ch-crc32) by Cliff Hammerschmidt | 117 MB/s
[Crc32](https://github.com/dariogriffo/Crc32) by Dario Griffo | 401 MB/s
[Klinkby.Checksum](https://github.com/klinkby/klinkby.checksum) by Mads Breusch Klinkby | 400 MB/s
[Data.HashFunction.CRC](https://github.com/brandondahler/Data.HashFunction/) by Brandon Dahler | 206 MB/s
[Dexiom.QuickCrc32](https://github.com/Dexiom/Dexiom.QuickCrc32/) by Jonathan Paré | 364 MB/s
This library | **1137** MB/s

## Some notes

I thought about making a pull request to [Crc32](https://github.com/dariogriffo/Crc32) library, but it seems, this library was abandoned. Anyway, I implement my library to be fully compatible with Crc32 library. And you can switch from Crc32 library to this.

Api interface was taken from [Crc32C.NET](https://crc32c.angeloflogic.com/) library. It is very handy for using in applications.

## License

[MIT](https://github.com/force-net/Crc32.NET/blob/develop/LICENSE) license
