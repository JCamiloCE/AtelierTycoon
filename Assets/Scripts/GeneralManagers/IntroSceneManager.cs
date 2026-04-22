using Emc2.Scripts.Enums;
using JCC.Scenes;
using UnityEngine;

namespace Emc2.Scripts.GeneralManagers
{
    public class IntroSceneManager : MonoBehaviour
    {
        private void Start()
        {
            Invoke(nameof(WaitIntro), 3f);
        }

        private void WaitIntro() 
        {
            SceneController<ESceneIds>.Instance.LoadScene(ESceneIds.Intermediate);
        }
    }
}