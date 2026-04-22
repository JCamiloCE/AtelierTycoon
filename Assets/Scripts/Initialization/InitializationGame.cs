using Emc2.Scripts.Enums;
using JCC.Debug;
using JCC.Scenes;
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
            SceneController<ESceneIds> sceneController = CreateNewMonobehaviourController<SceneController<ESceneIds>>("SceneController");
            Dictionary<ESceneIds, string> scenesNamesByIds = new Dictionary<ESceneIds, string>
            {
                { ESceneIds.MainMenu, "MainMenu" },
                { ESceneIds.Gameplay, "Gameplay" },
                { ESceneIds.Intermediate, "IntermediateInit" }
            };
            sceneController.SetScenes(scenesNamesByIds);
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