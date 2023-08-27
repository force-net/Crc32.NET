namespace Force.Crc32.Intrinsics
{
    /// <summary>
    /// Hardware intrinsics platforms supported by the current CPU
    /// </summary>
    public enum Platform
    {
        /// Hardware intrinsics are not supported on this CPU
        Unsupported,
        /// Intel SSE4.2 hardware instructions
        X86,
        /// Intel SSE4.2 x64 hardware instructions
        X64,
        /// ARM Crc32 hardware instructions
        Arm32,
        /// ARM64 Crc32 hardware instructions
        Arm64,
    }
}
