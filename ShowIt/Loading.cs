using ColossalFramework.UI;
using ICities;
using System;
using UnityEngine;

namespace ShowIt
{

    public class Loading : LoadingExtensionBase
    {        
        private LoadMode _loadMode;
        private GameObject _zbepObject;
        
        public override void OnLevelLoaded(LoadMode mode)
        {
            try
            {
                _loadMode = mode;

                if (_loadMode != LoadMode.LoadGame && _loadMode != LoadMode.NewGame && _loadMode != LoadMode.NewGameFromScenario)
                {
                    return;
                }

                UIView objectOfType = UnityEngine.Object.FindObjectOfType<UIView>();
                if (objectOfType != null)
                {
                    _zbepObject = new GameObject("ShowItZonedBuildingExtenderPanel");
                    _zbepObject.transform.parent = objectOfType.transform;
                    _zbepObject.AddComponent<ZonedBuildingExtenderPanel>();
                }
            }
            catch (Exception e)
            {
                Debug.Log("[Show It!] Loading:OnLevelLoaded -> Exception: " + e.Message);
            }
        }

        public override void OnLevelUnloading()
        {
            try
            {
                if (_loadMode != LoadMode.LoadGame && _loadMode != LoadMode.NewGame && _loadMode != LoadMode.NewGameFromScenario)
                {
                    return;
                }

                if (_zbepObject != null)
                {
                    UnityEngine.Object.Destroy(_zbepObject);
                }
            }
            catch (Exception e)
            {
                Debug.Log("[Show It!] Loading:OnLevelUnloading -> Exception: " + e.Message);
            }
        }
    }
}