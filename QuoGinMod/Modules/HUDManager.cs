using System;
using System.Collections.Generic;
using System.Text;
using Quo_Gin.Componenets;
using Quo_Gin.Modules.Survivors;
using RoR2;
using RoR2.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Quo_Gin.Modules
{
    public class HUDManager : MonoBehaviour
    {
        private CharacterBody characterBody;
        private static HUDManager instance;

        public static RectTransform defaultUI;
        public static HUD hud;
        public static GameObject uiPrefab;
        public static ChildLocator childLocator;
        private static List<IHUDElement> elementList = new List<IHUDElement>();


        public static ref HUDManager Get()
        {
            return ref instance;
        }
        private void Awake()
        {
           // Log.Debug("Hud Manager Awake");
            if (HUDManager.instance != null && HUDManager.instance != this)
            {
                //Log.Debug("Destroying Instance");
                UnityEngine.Object.Destroy(base.gameObject);
            }
            else
            {
                //Log.Debug("Creating New Instance");
                HUDManager.instance = this;
            }
            Init();
        }
        private void OnDestroy()
        {
            Log.Debug("Destroying HUDManager");
            On.RoR2.UI.HUD.Awake -= HUD_Awake;
            On.RoR2.UI.HUD.Update -= HUD_Update;
        }
        public void Init()
        {
            //Log.Debug("Startign Hud Manager");
            On.RoR2.UI.HUD.Awake += HUD_Awake;
            On.RoR2.UI.HUD.Update += HUD_Update;
        }

        
        public void HUD_Awake(On.RoR2.UI.HUD.orig_Awake orig, RoR2.UI.HUD self)
        {
            orig(self);
            hud = self;
            //Log.Debug("HUD Awake");

            Transform transform = self.mainContainer.transform.Find("MainUIArea");
            transform.Find("SpringCanvas");

            //add Unity Prefab to ROR2 HUD
            GameObject UIPrefab = UnityEngine.Object.Instantiate<GameObject>(Assets.UIPrefab);
            UIPrefab.transform.SetParent(self.mainContainer.transform);

            //position of MainHUd 
            RectTransform rectTransform = UIPrefab.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchoredPosition = new Vector2(0f,0f);

            childLocator = UIPrefab.GetComponent<ChildLocator>();
            defaultUI = rectTransform;
            uiPrefab = UIPrefab;

            //Initialize all hud Elements
            foreach (IHUDElement element in elementList)
            {
                element.baseInit(self);
            }
        }
        public void HUD_Update(On.RoR2.UI.HUD.orig_Update orig, RoR2.UI.HUD self)
        {
            orig(self);
            foreach (IHUDElement element in elementList)
            {
                element.UpdateUI();
            }
        }
        public void addHUDElement(IHUDElement element)
        {
            elementList.Add(element);
            Log.Debug("Added  :" + element.elementName + ":  to HUDManager");
        }
    }
}
