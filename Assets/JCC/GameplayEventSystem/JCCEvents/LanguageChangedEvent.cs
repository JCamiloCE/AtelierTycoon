using JCC.Enums;

namespace JCC.GameplayEventSystem
{
    public class LanguageChangedEvent : EventBase
    {
        public ELanguageID languageID;

        public override void SetParameters(params object[] parameters)
        {
            languageID = (ELanguageID)parameters[0];
        }
    }
}