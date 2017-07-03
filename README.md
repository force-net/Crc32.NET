# Crc32.NET

Optimized and fast managed implementation of Crc32 & Crc32C (Castagnoly) algorithms for .NET and .NET Core. 

*(But if you need, I can add native implementation (with transparent .net wrapper) which is twice faster .NET version)*

### Version 1.1.0 Remarks

Initially, this library only support Crc32 checksum, but I found, that there are lack of .NET Core libraries for Crc32 calculation. So, I've added Crc32C (Castagnoli) managed implementation in this library. Other Crc32 variants (like Crc32Q or Crc32K) seems to be unpopular to implement it here.

So, if you need to use Crc32C for .NET Core, you can use this library. But if you only use 'big' .NET frameworks, it is better to use [Crc32C.NET](https://crc32c.angeloflogic.com/) for Crc32C? because it has fast native implementation. 

## Description

This library is port of [Crc32C.NET](https://crc32c.angeloflogic.com/) by Robert Važan but for Crc32 algorithm. Also, this library contains optimizations for managed code, so, it really faster than other Crc32 implementations. 

If you do not not catch the difference, it is *C* (Castagnoli). I recommend to use Crc32C, not usual CRC32, because it can be faster (up to 20GB/s with native CPU implementation) and slightly better in error detection. But if you need exactly Crc32, this library is the best choice.

### Performance

This library has code, which is optimized for .NET (implementation is not dumb copy-paste from google), as result, it is really fast in comparison with other implemenations. 

Library | Speed
--------|-------
[CH.Crc32](https://github.com/tanglebones/ch-crc32) by Cliff Hammerschmidt | 117 MB/s
[Crc32](https://github.com/dariogriffo/Crc32) by Dario Griffo | 401 MB/s
[Klinkby.Checksum](https://github.com/klinkby/klinkby.checksum) by Mads Breusch Klinkby | 400 MB/s
[Data.HashFunction.CRC](https://github.com/brandondahler/Data.HashFunction/) by Brandon Dahler | 206 MB/s
[Dexiom.QuickCrc32](https://github.com/Dexiom/Dexiom.QuickCrc32/) by Jonathan Paré | 364 MB/s
This library | **1150** MB/s

## Some notes

I thought about making a pull request to [Crc32](https://github.com/dariogriffo/Crc32) library, but it seems, this library was abandoned. Anyway, I implement my library to be fully compatible with Crc32 library. And you can switch from Crc32 library to this.

Api interface was taken from [Crc32C.NET](https://crc32c.angeloflogic.com/) library. It is very handy for using in applications.

## License

[MIT](https://github.com/force-net/Crc32.NET/blob/develop/LICENSE) license
