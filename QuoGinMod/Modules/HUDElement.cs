
using Quo_Gin.Componenets;
using RoR2;
using RoR2.UI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


namespace Quo_Gin.Modules
{

    public interface IHUDElement
    {
        string elementName { get; }
        void UpdateUI();
        void baseInit(HUD hud);

    }
    public abstract class HUDElement<T> : MonoBehaviour, IHUDElement
    {
        private T _source;
        private GameObject uiPrefab;
        private HUD hud;
        private RectTransform rectTransform;
        public RectTransform componentRectTrans;
        public ChildLocator childLocator;
        private bool isAuthority = false;
        private Dictionary<string, Image> imageDictionary = new Dictionary<string, Image>();

        public CharacterBody characterBody { get; set; }
        public abstract string componentName { get; }

        public static HUDManager instance;


        //public string elementName;
        public string elementName { get; set; }

        public T source
        {
            get
            {
                return this._source;
            }
            set
            {
                this._source = value;
            }
        }
        public abstract void UpdateUI();
        public abstract void Init();


        private void OnDestroy()
        {

        }
        //Default Init, should be same for all;
        public void baseInit(HUD hud)
        {
            Log.Debug($"Initializing {elementName} " + Environment.NewLine);
            setHUD(hud);
            instance = HUDManager.Get();
            this.childLocator = HUDManager.childLocator;
            setUIPrefab(HUDManager.uiPrefab);
            setRectTransform(HUDManager.defaultUI);


            //position of current Hud Element;
            RectTransform uiRect = this.childLocator.FindChild(componentName).GetComponent<RectTransform>();

            if (uiRect != null )
            { 
                //Log.Debug("Found " + componentName); 
            }
            else
            {
               // Log.Debug("Could not find " + componentName);
            }

            this.componentRectTrans = uiRect;
            this.uiPrefab.SetActive(true);

            childLocator.FindChild(componentName).gameObject.SetActive(true);
            Init();
        }


        public bool setAuthority()
        {
            this.isAuthority = Util.HasEffectiveAuthority(this.characterBody.networkIdentity) && this.characterBody.isPlayerControlled;
            //Log.Debug("Authroity: " + this.isAuthority + Environment.NewLine);
            return this.isAuthority;
        }
        public bool hasAuthority()
        {
            return this.isAuthority;
        }
        public void setSource(T source) 
        {
            if (source == null) 
            {
            }
            else
            {
                this._source = source;
            }
        }

        public void setHUD(HUD hud) { this.hud = hud; }
        public HUD getHUD() { return this.hud; }

        public void setRectTransform(RectTransform rectTransform) 
        { RectTransform tem = uiPrefab.GetComponent<RectTransform>();
            tem = rectTransform; 
        }
        public RectTransform getRectTransform() {  return this.rectTransform; }

        public void setUIPrefab(GameObject uiPrefab) 
        {
            this.uiPrefab = uiPrefab;
        }
        public GameObject getUIPrefab() 
        {
            return this.uiPrefab;
        }


        public void addImage(string imageName, Image image)
        {
            this.imageDictionary.Add(imageName, image);
        }
        public Image getImage(string imageName)
        {
           // return instance.getHUDImage(elementName+ " " + imageName);
            return this.imageDictionary[imageName];
        }
        public T findComponent<T>(string path)
        {
            return componentRectTrans.Find(path).GetComponent<T>();
        }
    }
}
