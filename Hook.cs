using System.Diagnostics;
using System.Drawing;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using PeNet;
using PeNet.FileParser;
using PeNet.Header.Pe;

namespace IIDXAdditiveOmni;

public unsafe static partial class Hook
{
    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool VirtualProtect(nint address, int size, int protect, out int oldProtect);

    [LibraryImport("kernel32.dll", StringMarshalling = StringMarshalling.Utf16)]
    internal static partial nint GetModuleHandleW(string library);

    static StreamWriter logFile;

    internal static void LogLine(string line)
    {
        if (logFile == null)
            logFile = File.AppendText("omni.log");
        
        logFile.WriteLine(line);
        logFile.Flush();
    }

    public static void Set(nint addr, nint value)
    {
        VirtualProtect(addr, nint.Size, 0x40, out int oldProtect);
        *(nint*)(addr) = value;
        VirtualProtect(addr, nint.Size, oldProtect, out int _);
    }

    public static void Init()
    {
        var handle = GetModuleHandleW("bm2dx.dll");
        if (handle == IntPtr.Zero)
        {
            LogLine("Uninitialized");
            return;
        }

        var file = new ReadonlyMemoryFile(handle);
        var dosHeader = new ImageDosHeader(file, 0);
        var ntHeader = new ImageNtHeaders(file, dosHeader.E_lfanew);
  
        var importRva = ntHeader.OptionalHeader.DataDirectory[(int)DataDirectoryType.Import].VirtualAddress;

        while (true)
        {
            var desc = new ImageImportDescriptor(file, importRva);
            if (desc.Name == 0)
                break;

            var dllName = file.ReadAsciiString(desc.Name);
            var nameTable = desc.OriginalFirstThunk;
            var addrTable = desc.FirstThunk;

            // LogLine(dllName);

            while (true)
            {
                var trunk = new ImageThunkData(file, nameTable, nint.Size == 8);
                if (trunk.AddressOfData == 0)
                    break;

                string importName;
                ulong mask = (ulong)(nint.Size == 8 ? long.MinValue : int.MinValue);
                if ((trunk.AddressOfData & mask) == mask)
                {
                    importName = $"#{trunk.AddressOfData & ~mask}";
                }
                else
                {
                    var import = new ImageImportByName(file, (uint) trunk.AddressOfData);
                    importName = import.Name;
                }

                // LogLine($" - {importName}: 0x{addrTable:x}");

                if (dllName == "avs2-core.dll" && importName == "#82")
                {
                    MusicDataPatch.OriginalRead = Marshal.GetDelegateForFunctionPointer<MusicDataPatch.AvsRead>(*(nint*)((nint)addrTable + handle));

                    var target = Marshal.GetFunctionPointerForDelegate<MusicDataPatch.AvsRead>(MusicDataPatch.ReadHook);
                    Set((nint)addrTable + handle, target);

                    LogLine("Hook attached");
                }

                nameTable += (uint) nint.Size;
                addrTable += (uint) nint.Size;
            }

            importRva += 20;
        }
    }
}
