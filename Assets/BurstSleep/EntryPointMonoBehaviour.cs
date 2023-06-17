using System.Threading;
using Unity.Burst;
using Unity.Jobs;
using UnityEngine;

[BurstCompile]
public class EntryPointMonoBehaviour : MonoBehaviour
{
    private JobHandle m_SleepJobHandle;
    
    [BurstCompile]
    static void RunBurstDirectCall(int millisecondsSleep)
    {
        if (BurstUtils.IsCalledFromBurst)
        {
            Debug.Log("[RunBurstDirectCall] Called from burst");
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogWarning("[RunBurstDirectCall] Called from managed, but expected to be from burst. Did you disable Burst compilation in Jobs->Burst menu or forgot [BurstCompile] on the class and the method?");
#else
            Debug.LogWarning("[RunBurstDirectCall] Called from managed, but expected to be from burst");
#endif
        }
        
        Debug.Log("[RunBurstDirectCall] Before sleep");
        
        BurstSleep.Sleep(millisecondsSleep);
        
        Debug.Log("[RunBurstDirectCall] After sleep");
    }

    static JobHandle ScheduleBurstJob(int millisecondsSleep, JobHandle dependency = default)
    {
        var demoSleepJob = new DemoSleepJob
        {
            MillisecondsSleep = millisecondsSleep
        };
        return demoSleepJob.Schedule(dependency);
    }
    
    void Start()
    {
        BurstSleep.InitializeFromManaged();
        
        Debug.Log("Main thread is going to sleep in Burst");
        RunBurstDirectCall(50);
        Debug.Log("Main thread woke up, sorry for the lag!");
    }

    private void OnEnable()
    {
        BurstSleep.InitializeFromManaged();
        m_SleepJobHandle = ScheduleBurstJob(10000);
    }

    void Update()
    {
        if (m_SleepJobHandle.IsCompleted == false)
        {
            Debug.Log("Tshh! Burst job is sleeping!");
        }
        else
        {
            Debug.Log("That's how it is done!");
            enabled = false;
        }
    }
}

[BurstCompile]
internal struct DemoSleepJob : IJob
{
    public int MillisecondsSleep;

    public void Execute()
    {
        if (BurstUtils.IsCalledFromBurst)
        {
            Debug.Log("[Job] Called from burst");
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogWarning("[Job] Called from managed, but expected to be from burst. Did you disable Burst compilation in Jobs->Burst menu or forgot [BurstCompile] on the class and the method?");
#else
            Debug.LogWarning("[Job] Called from managed, but expected to be from burst");
#endif
        }
        
        Debug.Log("[Job] Before sleep");
        
        BurstSleep.Sleep(MillisecondsSleep);
        
        Debug.Log("[Job] After sleep");
    }
}