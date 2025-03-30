using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public class UIManager : SingletonBase<UIManager>
    {
        public static T Show<T>(UIList uiName) where T : UIBase
        {
            var targetUI = Singleton.GetUI<T>(uiName);

            if (targetUI == null)
            {
                return null;
            }

            targetUI.Show();
            return targetUI;
        }

        public static T Hide<T>(UIList uiName) where T : UIBase
        {
            var targetUI = Singleton.GetUI<T>(uiName);

            if (targetUI == null)
            {
                return null;
            }

            targetUI.Hide();
            return targetUI;
        }

        private Dictionary<UIList, UIBase> panels = new Dictionary<UIList, UIBase>();
        private Dictionary<UIList, UIBase> popups = new Dictionary<UIList, UIBase>();

        private Transform panelRoot;
        private Transform popupRoot;
        private const string UI_PATH = "UI/Prefabs/";

        public void Initalize()
        {
            if (panelRoot == null)
            {
                GameObject panelGo = new GameObject("Panel Root");
                panelRoot = panelGo.transform;
                panelRoot.SetParent(transform);
            }
            if (popupRoot == null)
            {
                GameObject popupGo = new GameObject("Popup Root");
                popupRoot = popupGo.transform;
                popupRoot.SetParent(transform);
            }

            for (int i = (int)UIList.PANEL_START + 1; i < (int)UIList.PANEL_END; i++)
            {
                panels.Add((UIList)i, null);
            }

            for (int i = (int)UIList.POPUP_START + 1; i < (int)UIList.POPUP_END; i++)
            {
                popups.Add((UIList)i, null);
            }


        }

        public T GetUI<T>(UIList uiName, bool isReload = false) where T : UIBase
        {
            Dictionary<UIList, UIBase> container = null;
            if (uiName > UIList.PANEL_START && uiName < UIList.PANEL_END)
            {
                container = panels;
            }
            else
            {
                container = popups;
            }

            if (!container.ContainsKey(uiName))
            {
                return null;
            }

            if (isReload && container[uiName])
            {
                Destroy(container[uiName].gameObject);
                container[uiName] = null;
            }

            if (!container[uiName])
            {
                string path = UI_PATH + $"UI.{uiName}";
                T result = Resources.Load<UIBase>(path) as T;

                if (result)
                {
                    container[uiName] = Instantiate(result, container == panels ? panelRoot : popupRoot);
                    container[uiName].gameObject.SetActive(false);
                }
            }

            return container[uiName] as T;
        }
    }
}
