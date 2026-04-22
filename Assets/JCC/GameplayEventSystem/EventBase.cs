namespace JCC.GameplayEventSystem
{
    public abstract class EventBase 
    {
        internal bool IsAvailable { get; set; }

        public virtual void SetParameters(params object[] parameters) { }
    }
}

