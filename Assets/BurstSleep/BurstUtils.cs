using Unity.Burst;

public static class BurstUtils
{
    public static bool IsCalledFromBurst
    {
        get
        {
            [BurstDiscard]
            // ReSharper disable once RedundantAssignment
            static void ChangeToZeroInManaged(ref byte isBurst) { isBurst = 0; }
            
            byte isBurst = 1;
            ChangeToZeroInManaged(ref isBurst);
            return isBurst != 0;
        }
    }
}