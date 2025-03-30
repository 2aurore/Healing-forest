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

            // TODO : Custom Order After System Load
            //     UIManager.Show<IngameUI>(UIList.IngameUI);
            //     UIManager.Show<LogUI>(UIList.LogUI);

        }
    }
}
#endif