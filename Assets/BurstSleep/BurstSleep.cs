#define CAN_USE_UNMANAGED_DELEGATES

using System.Runtime.InteropServices;
using System.Threading;
using Unity.Burst;
using UnityEngine;

[BurstCompile]
public static class BurstSleep
{
    // Call somewhere in managed C#. Don't call from static constructor - Burst can call it if you never touched the class from managed before!
    [BurstDiscard]
    public static void InitializeFromManaged()
    {
        if (Burst2ManagedCall<SleepManagedDelegate, SleepManagedDelegateKey>.IsCreated == false)
            Burst2ManagedCall<SleepManagedDelegate, SleepManagedDelegateKey>.Init(SleepManaged);
    }

    public static void Sleep(int milliseconds)
    {
        var ptr = Burst2ManagedCall<SleepManagedDelegate, SleepManagedDelegateKey>.Ptr();
#if CAN_USE_UNMANAGED_DELEGATES
        // this is better variant - not going to alloc if burst is disabled
        unsafe
        {
            ((delegate * unmanaged[Cdecl] <int, void>)ptr.Value)(milliseconds);
        }
#else
        ptr.Invoke(milliseconds);
#endif 
    }
    
    private struct SleepManagedDelegateKey{}
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void SleepManagedDelegate(int milliseconds);
    
    [AOT.MonoPInvokeCallback(typeof(SleepManagedDelegate))]
    private static void SleepManaged(int milliseconds)
    {
        // C# managed land
        Debug.Log($"Sleep({milliseconds}) is called from the managed!");
        
        Thread.Sleep(milliseconds);
    }
}