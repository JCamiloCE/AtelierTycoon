using Emc2.Scripts.Scenes;
using JCC.Debug;
using JCC.Localization;
using System.Collections.Generic;
using UnityEngine;

namespace Emc2.Scripts.Initialization
{
    public class InitializationGame
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitGame()
        {
            DebugManager.Initialization(new DebugUnityImpl(), EDebugScope.All);

            LoadController("pf_FadeCanvas");
            LoadController("pf_AudioController");
            CreateNewMonobehaviourController<SceneController>("SceneController");
            InitializeLocalization();
            CreateLogicClasses();
        }

        private static void LoadController(string resourceName) 
        {
            GameObject newController = Resources.Load<GameObject>(resourceName);

            if (newController != null)
            {
                Object.Instantiate(newController, Vector3.zero, Quaternion.identity);
            }
        }

        private static T CreateNewMonobehaviourController<T>(string nameGo) where T : MonoBehaviour
        {
            GameObject newGoController = new GameObject(nameGo);
            T newClass = newGoController.AddComponent<T>();
            return newClass;
        }

        private static void CreateLogicClasses() 
        {
            //NOOP
        }

        private static void InitializeLocalization() 
        {
            //LocalizationManager localization = CreateNewMonobehaviourController<LocalizationManager>("LocalizationManager");
            //List<string> fileNames = new List<string> { "Localization/Loc_UI",
            //                                            "Localization/Loc_Tutorial"};
            //ILocalizationProcessor processor = new LocalizationProcessorCSV();
            //localization.Initialize(processor, fileNames);
        }
    }
}