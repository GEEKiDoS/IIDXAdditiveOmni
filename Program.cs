using IIDXAdditiveOmni;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

static class Program
{
    [UnmanagedCallersOnly(EntryPoint = "DllMain", CallConvs = [typeof(CallConvStdcall)])]
    public static bool DllMain(IntPtr module, uint callReason, IntPtr reserved)
    {
        if (callReason != 1)
            return true;

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        Hook.Init();

        return true;
    }
}
