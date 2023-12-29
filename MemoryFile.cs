using PeNet.FileParser;
using System.Runtime.InteropServices;
using System.Text;

public unsafe class ReadonlyMemoryFile : IRawFile
{
    private nint _address;

    public long Length => long.MaxValue;

    public ReadonlyMemoryFile(nint address) => _address = address;

    public string ReadAsciiString(long offset)
    {
        return Marshal.PtrToStringAnsi((nint)(_address + offset)) ?? "";
    }

    public Span<byte> AsSpan(long offset, long length) => new Span<byte>((void *)(_address + offset), (int)length);

    public string ReadUnicodeString(long offset)
    {
        return Marshal.PtrToStringUni((nint)(_address + offset)) ?? "";
    }

    public string ReadUnicodeString(long offset, long length)
    {
        return Marshal.PtrToStringUni((nint)(_address + offset), (int)length);
    }

    public byte ReadByte(long offset) => *(byte*)(_address + offset);

    public uint ReadUInt(long offset) => *(uint*)(_address + offset);

    public ulong ReadULong(long offset) => *(ulong*)(_address + offset);

    public ushort ReadUShort(long offset) => *(ushort*)(_address + offset);

    public void WriteByte(long offset, byte value) => throw new InvalidOperationException();

    public void WriteBytes(long offset, Span<byte> bytes) => throw new InvalidOperationException();

    public void WriteUInt(long offset, uint value) => throw new InvalidOperationException();

    public void WriteULong(long offset, ulong value) => throw new InvalidOperationException();

    public void WriteUShort(long offset, ushort value) => throw new InvalidOperationException();

    public byte[] ToArray() => throw new InvalidOperationException();

    public void RemoveRange(long offset, long length) => throw new InvalidOperationException();

    public int AppendBytes(Span<byte> bytes) => throw new InvalidOperationException();
}