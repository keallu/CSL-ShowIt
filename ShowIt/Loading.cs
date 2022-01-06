using ICities;
using System;
using UnityEngine;

namespace ShowIt
{
    public class Loading : LoadingExtensionBase
    {
        private GameObject _modManagerGameObject;
        private LoadMode _loadMode;

        public override void OnLevelLoaded(LoadMode mode)
        {
            try
            {
                _loadMode = mode;

                if (_loadMode != LoadMode.NewGame && _loadMode != LoadMode.LoadGame && _loadMode != LoadMode.NewGameFromScenario)
                {
                    return;
                }

                _modManagerGameObject = new GameObject("ShowItModManager");
                _modManagerGameObject.AddComponent<ModManager>();
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
                if (_loadMode != LoadMode.NewGame && _loadMode != LoadMode.LoadGame && _loadMode != LoadMode.NewGameFromScenario)
                {
                    return;
                }

                if (_modManagerGameObject != null)
                {
                    UnityEngine.Object.Destroy(_modManagerGameObject);
                }
            }
            catch (Exception e)
            {
                Debug.Log("[Show It!] Loading:OnLevelUnloading -> Exception: " + e.Message);
            }
        }
    }
}