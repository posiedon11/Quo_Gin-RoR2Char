using IL.RoR2.UI;
using Quo_Gin.Modules;
using Quo_Gin.Modules.Survivors;
using RoR2;
using RoR2.HudOverlay;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Quo_Gin.Componenets
{
    public class ResourceHandler : NetworkBehaviour
    {
        //Resource Regen
        private float baseResourceRegen = 1f;
        private float resourceRegen = 20f;
        private float bonusResourceRegen;
        private float cappedResourceRegen = 1000f;

        //Resource capacity
        private bool reduceCost = false;
        //Max Resouce
        private float baseMaxResource = 100f;
        private float maxResource = 100f;
        private float cappedMaxResource = 100f;
        private float cachedMaxResource;
        private float[] bonusMaxResourceArray = new float[]
       {
            10f,
            15f,
            20f,
            40f
       };

        //Min Resource
        private float baseMinResource = 100f;
        private float minResource = 100f;
        private float cappedMinResource = 20f;
        private float cachedMinResource;
        private float[] reduceMinResourceArray = new float[]
        {
            1f,
            2f,
            3f,
            4f
        };


        private float currentResource=0f;

        public List<IHUDElement> hudElements = new List<IHUDElement>();


        private bool lockResource;
        //this prevents the max resouce from increasing, instead, it reduces the cost needed.
        
        private SkillLocator skillLocator;
        public CharacterBody owner;
        public HUDResourceBar hudResourceBar;

        public bool isReducedResource()
        {
            return this.reduceCost;
        }

        public virtual void Start()
        {
            //Log.Debug("Starting Resource Handler");
            this.skillLocator = base.GetComponent<SkillLocator>();
            this.owner = base.GetComponent<CharacterBody>();
            this.Init();
            this.InitHUDElements();
            this.HUDSetup();
        }

        public virtual void InitHUDElements()
        {
            hudResourceBar = new HUDResourceBar();
            hudResourceBar.source = this;
            hudResourceBar.elementName = "Resource Handler Element";
            hudResourceBar.characterBody = this.owner;
            this.hudElements.Add(hudResourceBar);
        }
        private void HUDSetup()
        {            
            

            foreach (IHUDElement element in this.hudElements)
            {
                Quo_GinChar.hudmanager.addHUDElement(element);
            }
        }
        public void Init()
        {
            //Log.Debug("Init");
            if (base.hasAuthority)
            {
                
                //Log.Debug("Initializing Ki Handler");
                //this.skillLocator = base.GetComponent<SkillLocator>();
                
            }
            else
            {
                //Log.Debug("No Authority");
            }
            this.InitializeResourceHandler();
        }

       

        private void InitializeResourceHandler()
        {
            
            //Log.Debug("Initializing Resource Handler");
            //Log.Debug(owner.name);
            if (reduceCost)
            {
                this.setMinResource(this.getMinResouce());
               // Log.Debug($"Min Resource: {this.getCurrentResource()}");
            }
            else
            {
                this.setMaxResource(this.getMaxResource());
                //Log.Debug($"Max Resource: {this.getMaxResource()}");

            }
            //Log.Debug(this.getCurrentResource());

            this.setCurrentResource(250f);
           // Log.Debug($"Current Resource: {this.getCurrentResource()}");


        }

        public void RecalculateResource()
        {
            if (owner)
            {
                if (reduceCost)
                {
                    this.setMinResource(baseMinResource + this.getMinResourceReduction(3));
                }
                else
                {
                    this.setMaxResource(baseMaxResource + this.getBonusMaxResource());
                }
                
            }
        }
        public float getBonusMaxResource()
        {
            int num = 0;
            foreach (GenericSkill genericSkill in base.transform.GetComponents<GenericSkill>())
            {
                if (genericSkill != null && num < 4)
                {
                    this.cachedMaxResource += this.bonusMaxResourceArray[num] * (float)(genericSkill.maxStock -1);
                }
                ++num;
            }
            return this.cachedMaxResource;
        }

        public float getMinResourceReduction(int skillforResource)
        {
            
            return reduceMinResourceArray[skillforResource] * this.skillLocator.special.maxStock - 1;
        }
        public ResourceHandler GetResourceHandler()
        {
            return base.GetComponent<ResourceHandler>();
        }

        //Current Resource
        #region Current Resource
        public float getCurrentResource()
        {
            return this.currentResource;
        }
        public void setCurrentResource(float amount)
        {
            //float lowerBound, upperBound;

            //lowerBound = Mathf.Max(0f, amount);
            //upperBound = Mathf.Min(amount, this.getMaxResource());
            //Log.Debug($"{lowerBound}    {upperBound}");
            //this.currentResource = Mathf.Max(lowerBound, upperBound);

            this.currentResource = Mathf.Clamp(amount, 0f, this.getMaxResource());
        }
        public void addCurrentResource(float amount)
        {
            this.setCurrentResource(this.getCurrentResource() + amount);
        }
        #endregion


        //Base max resource
        #region MaxResource
        public float getMaxResource()
        {
            return this.maxResource;
        }
        public void setMaxResource(float resourceVal)
        {
            //float lowerBound, upperBound;
            //lowerBound = Mathf.Max(this.cappedMinResource, resourceVal);
            //upperBound = Mathf.Min(resourceVal, this.cappedMaxResource);
            //this.maxResource = Mathf.Max(lowerBound, upperBound);

            this.maxResource = Mathf.Clamp(resourceVal, cappedMinResource, cappedMaxResource);
        }
        public void addMaxResource(float resourceVal)
        {
            setMaxResource(this.getMaxResource() + resourceVal);
        } 
        #endregion

        //Base min Resource
        #region MinResource
        public float getMinResouce()
        {
            return this.minResource;
        }
        public void setMinResource(float resourceVal)
        {
            //float lowerBound, upperBound;
            //lowerBound = Mathf.Max(this.cappedMinResource, resourceVal);
            //upperBound = Mathf.Min(resourceVal, this.cappedMaxResource);
            //this.minResource = Mathf.Min(lowerBound, upperBound);
            this.minResource = Mathf.Clamp(resourceVal, this.cappedMinResource, this.cappedMaxResource);
        }
        public void addMinResource(float resourceVal)
        {
            setMaxResource(this.getMinResouce() + resourceVal);
        } 
        #endregion

        //base resource regen
        #region Base Regen
        public float getResourceRegen()
        {
            return this.resourceRegen;
        }
        public void setResourceRegen(float regenVal)
        {

            this.resourceRegen = Mathf.Min(regenVal, this.cappedResourceRegen); ;
        }
        public void addResourceRegen(float regenVal)
        {
            setResourceRegen(this.getResourceRegen() + regenVal);
        } 
        #endregion

        //bonus resource regen
        #region BonusResource
        public float getBonusResourceRegen()
        {
            return this.bonusResourceRegen;
        }

        public void setBonusResourceRegen(float bonusRege)
        {
            this.bonusResourceRegen = bonusRege;
        }
        public void AddBonusResourceRegen(float bonusRegen)
        {
            this.setBonusResourceRegen(this.getBonusResourceRegen() + bonusRegen);
        }
        public void resetBonusRegen()
        {
            this.bonusResourceRegen = 0;
        } 
        #endregion

        //combined resource regen
        public float getTotalRegen()
        {
            return this.resourceRegen + this.bonusResourceRegen;
        }

        public void lockCurrentSuper(bool val)
        {
            this.lockResource = val;
        }

        public float getCurrentResourceRatio()
        {
            return this.getCurrentResource() / this.getMaxResource();
        }
        private void Update()
        {
            this.addCurrentResource(this.getTotalRegen() * Time.deltaTime);
            //if (Time.deltaTime %1 == 0)
            //{
            //    //Log.Debug($"Current Resource {this.getCurrentResource()}");
            //}
        }
    }
}
