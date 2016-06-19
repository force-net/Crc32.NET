# Crc32.NET

Optimized and fast implementation of Crc32 algorithm in pure .NET. 

*(But if you need, I can native implementation which is twice faster .NET version)*

This library is port of [Crc32C.NET](https://crc32c.angeloflogic.com/) by Robert Važan but for Crc32 algorithm. 

If you did not catch the difference, it is *C* (Castagnoli). I recommend to use Crc32C, not usual CRC32, because it is faster (up to 20GB/s) and slightly better in error detection. But if you need exactly Crc32, this library is the best choice.

### Performance

Library | Speed
--------|-------
[Crc32](https://github.com/dariogriffo/Crc32) by Dario Griffo | 382 MB/s
[Data.HashFunction.CRC](https://github.com/brandondahler/Data.HashFunction/) by Brandon Dahler | 203 MB/s
This library | **1066** MB/s

## Some notes

I think about making a pull request to [Crc32](https://github.com/dariogriffo/Crc32) library, but it seems, this library was abandoned. Anyway, I check, that results of my and Crc32 library are fully compatible. And you can switch from Crc32 library to this.

Api interface has been taken from [Crc32C.NET](https://crc32c.angeloflogic.com/) library. It is very handy for using in applications.

## License

[MIT](https://github.com/force-net/Crc32.NET/blob/develop/LICENSE) license
