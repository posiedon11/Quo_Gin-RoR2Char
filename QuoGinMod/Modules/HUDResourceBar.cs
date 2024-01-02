using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using UnityEngine;
using UnityEngine.UI;
using RoR2.UI;
using Quo_Gin.Componenets;
using TMPro;

namespace Quo_Gin.Modules
{
    public class HUDResourceBar : HUDElement<ResourceHandler>
    {
        private float fractionalVal;
        private Image backgroundBar;

        private Image currentFillBar;
        private TextMeshProUGUI currentResource;
        private Text currentResourceText;
        private Text currentRegenText;

        public override string componentName
        {
            get { return "Bars"; }
        }

        //public override Dictionary<string, Image> imageDictionary 
        //{ get { return imageDictionary; }
        //  set { imageDictionary = new Dictionary<string, Image>(); }
        //}
        public override void Init()
        {
           // Log.Debug("Finding Stuff");
            this.childLocator = childLocator.FindChild(componentName).GetComponent<ChildLocator>();

            this.backgroundBar = this.findComponent<Image>("ResourceBar");
            if (this.backgroundBar != null)
            {
                //Log.Debug("BackGround Image found");
                
            }
            this.currentFillBar = this.findComponent<Image>("ResourceBar/BarFill");
            if (this.currentFillBar != null)
            {
               // Log.Debug("FillBar Image found");
            }
            this.currentResourceText = this.findComponent<Text>("ResourceBar/BarFill/BarText");
            if (this.currentResourceText != null)
            {
               // Log.Debug("ResourceText Found");
            }

            //if(instance)
            //{
            //    instance.addHUDImage(elementName + " " +"ResourceBar", this.backgroundBar);
            //    instance.addHUDImage(elementName+ " " +"BarFill", this.currentFillBar);
            //}

            //this.addImage("ResourceBar", this.backgroundBar);
           // this.addImage("BarFill", this.currentFillBar);

            
            
        }
        public override void UpdateUI()

        {
            if (this.setAuthority())
            {
                
                if (instance)
                {
                    this.fractionalVal = this.source.getCurrentResourceRatio();
                    this.currentFillBar.fillAmount = this.fractionalVal;
                    this.currentResourceText.text = (Mathf.RoundToInt( this.fractionalVal*100)).ToString() + " / " + this.source.getMaxResource().ToString();
                }
            } 
        }
       
        private void Awake()
        {

        }
    }
}
