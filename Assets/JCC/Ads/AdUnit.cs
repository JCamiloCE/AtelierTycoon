namespace JCC.Ads
{
    public class AdUnit
    {
        private EAdState _currentState = EAdState.None;
        private ETypeAd _typeAd = ETypeAd.None;
        private IAdProvider _provider;

        public AdUnit(ETypeAd typeAd, IAdProvider adProvider)
        {
            _typeAd = typeAd;
            _provider = adProvider;
        }

        public void Load(float delay = 0)
        {
            if (_currentState == EAdState.Loading || _currentState == EAdState.Loaded)
            {
                return;
            }

            if(!_provider.IsInitialized)
            {
                return;
            }

            _currentState = EAdState.Loading;
            _provider.LoadAd(_typeAd);
        }

        public bool TryShow()
        {
            if (_currentState != EAdState.Loaded)
            {
                return false;
            }

            if (!_provider.IsInitialized)
            {
                return false;
            }

            _currentState = EAdState.Showing;
            _provider.ShowAd(_typeAd);
            return true;
        }

        public void SetState(EAdState newState) => _currentState = newState;
    }
}