using JCC.Enums;

namespace JCC.Localization
{
    public interface ILocalizationProcessor
    {
        public string GetValue(string key);
        public void LoadFromCSV(string text, ELanguageID langID, string fileName);
        public void Clear();
        public bool IsReady();
    }
}