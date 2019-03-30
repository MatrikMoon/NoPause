using IllusionPlugin;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

/**
 * Created by Moon on 3/30/2019 in a rush as I was about to compete in Cube Community's tournament
 * Hooks the menu button listener's Tick() method and prevents it from being called,
 * thus preventing the user from pausing.
 * Additionally, the user can hold both triggers and press a menu button to pause
 */

namespace NoPause
{
    public class Plugin : IPlugin
    {
        public string Name => "NoPause";
        public string Version => "0.0.1";

        private Redirection _currentRedirect;

        public void OnApplicationStart()
        {
            SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        }

        private void SceneManagerOnActiveSceneChanged(Scene arg0, Scene arg1)
        {
            if (arg1.name == "GameCore")
            {
                OverrideLongPause();
            }
        }

        private void OverrideLongPause()
        {
            if (_currentRedirect == null)
            {
                MethodInfo original = typeof(DelayedPauseTrigger).GetMethod(nameof(DelayedPauseTrigger.Tick), BindingFlags.Public | BindingFlags.Instance);
                MethodInfo modified = typeof(Plugin).GetMethod(nameof(OverrideTick), BindingFlags.Public | BindingFlags.Instance);
                _currentRedirect = new Redirection(original, modified, true);
            }
        }

        public void OverrideTick()
        {
            bool leftTrigger = VRControllersInputManager.TriggerValue(XRNode.LeftHand) >= 0.9f;
            bool rightTrigger = VRControllersInputManager.TriggerValue(XRNode.RightHand) >= 0.9f;
            bool menu = VRControllersInputManager.MenuButton();

            if (leftTrigger && rightTrigger && menu)
            {
                var slgm = Resources.FindObjectsOfTypeAll<StandardLevelGameplayManager>().First();
                slgm.HandlePauseTriggered();
            }
        }

        private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
        }

        public void OnApplicationQuit()
        {
            SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
            SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
        }

        public void OnLevelWasLoaded(int level)
        {

        }

        public void OnLevelWasInitialized(int level)
        {
        }

        public void OnUpdate()
        {
        }

        public void OnFixedUpdate()
        {
        }
    }
}
