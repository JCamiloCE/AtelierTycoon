namespace Emc2.Scripts.Enums
{
    public enum EAnalityEventIdentifier
    {
        //Each component of the enum need a identification
        //The identification must be the index of the four letters
        //Index: A:00, B:01,...,Y:25,Z:25
        //Note: the  index -1 is reserved for Invalid and 0 for None
        Invalid = -1,
        None = 0,
        AdCompleted = 00030214,
        AdInterstitial = 00030813,
        AdRewarded = 00031704
    }
}