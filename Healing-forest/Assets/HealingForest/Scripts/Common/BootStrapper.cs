#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HF
{
    /// <summary> Main을 타고 실행한 것처러므 씬이 동작되도록 도와주는 도우미 클래스 </summary>
    public class BootStrapper : MonoBehaviour
    {
        private const string BootStrapperMenuPath = "HealingForest/BootStrapper/Activate Ingame System";
        private static bool IsActivateBootStrapper
        {
            get => UnityEditor.EditorPrefs.GetBool(BootStrapperMenuPath, false);
            set
            {
                UnityEditor.EditorPrefs.SetBool(BootStrapperMenuPath, value);
                UnityEditor.Menu.SetChecked(BootStrapperMenuPath, value);
            }
        }

        [UnityEditor.MenuItem(BootStrapperMenuPath, false)]
        private static void ToggleActivateBootStrapper()
        {
            IsActivateBootStrapper = !IsActivateBootStrapper;
            UnityEditor.Menu.SetChecked(BootStrapperMenuPath, IsActivateBootStrapper);
        }


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void SystemBoot()
        {
            Scene activeScene = EditorSceneManager.GetActiveScene();
            if (IsActivateBootStrapper && false == activeScene.name.Equals("Main"))
            {
                InternalBoot();
            }
        }

        private static void InternalBoot()
        {
            Main.Singleton.Initialize();

            // EventSystem이 이미 존재하지 않으면 생성하고 DontDestroyOnLoad로 유지
            if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                var eventSystemGO = new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem), typeof(UnityEngine.EventSystems.StandaloneInputModule));
                DontDestroyOnLoad(eventSystemGO);
            }

            // TODO : Custom Order After System Load
            UIManager.Show<IngameUI>(UIList.IngameUI);
            //     UIManager.Show<LogUI>(UIList.LogUI);

        }
    }
}
#endif