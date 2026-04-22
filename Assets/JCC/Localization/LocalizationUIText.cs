using JCC.GameplayEventSystem;
using TMPro;
using UnityEngine;

namespace JCC.Localization
{
    public class LocalizationUIText : MonoBehaviour, IEventListener<LanguageChangedEvent>
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private string _key;

#if UNITY_EDITOR
        private void Reset()
        {
            _text = GetComponent<TextMeshProUGUI>();
        }
#endif

        public void OnEvent(LanguageChangedEvent event_data)
        {
            UpdateText();
        }

        private void Start()
        {
            UpdateText();
            EventManager.AddListener(this);
        }

        private void OnDestroy()
        {
            EventManager.RemoveListener(this);
        }

        private void UpdateText() 
        {
            _text.text = LocalizationManager.Instance.GetTranslation(_key);
        }
    }
}