using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Unity.Burst;

public static class Burst2ManagedCall<T, Key>
{
    private static T s_Delegate;
    // alignment 16 is important to not crash on arm cpu
    private static readonly SharedStatic<FunctionPointer<T>> s_SharedStaticFuncPtr = SharedStatic<FunctionPointer<T>>.GetOrCreate<FunctionPointer<T>, Key>(16);
    public static bool IsCreated => s_SharedStaticFuncPtr.Data.IsCreated;

    public static void Init(T @delegate)
    {
        CheckIsNotCreated();
        s_Delegate = @delegate;
        s_SharedStaticFuncPtr.Data = new FunctionPointer<T>(Marshal.GetFunctionPointerForDelegate(s_Delegate));
    }

    public static ref FunctionPointer<T> Ptr()
    {
        CheckIsCreated();
        return ref s_SharedStaticFuncPtr.Data;
    }

    [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS"), Conditional("UNITY_DOTS_DEBUG")] // ENABLE_UNITY_COLLECTIONS_CHECKS or UNITY_DOTS_DEBUG
    private static void CheckIsCreated()
    {
        if (IsCreated == false)
            throw new InvalidOperationException("Burst2ManagedCall was NOT created!");
    }

    [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS"), Conditional("UNITY_DOTS_DEBUG")] // ENABLE_UNITY_COLLECTIONS_CHECKS or UNITY_DOTS_DEBUG
    private static void CheckIsNotCreated()
    {
        if (IsCreated)
            throw new InvalidOperationException("Burst2ManagedCall was already created!");
    }
}