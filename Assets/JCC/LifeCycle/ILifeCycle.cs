namespace JCC.LifeCycle
{
    public interface ILifeCycle
    {
        public bool Initialization(params object[] parameters);
        public bool WasInitialized();
    }
}

