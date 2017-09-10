using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.UI.Screens;
using System.Collections;
using KSP.Localization;

namespace KerbalScienceInnovation
{
    [KSPAddon(KSPAddon.Startup.AllGameScenes, false)]
    public class AppLauncherButton : MonoBehaviour
    {
        private const ApplicationLauncher.AppScenes VisibleInScenes = ApplicationLauncher.AppScenes.MAPVIEW | ApplicationLauncher.AppScenes.TRACKSTATION | ApplicationLauncher.AppScenes.SPACECENTER | ApplicationLauncher.AppScenes.FLIGHT;

        private ApplicationLauncherButton _button;
        private static readonly string AppIconPath = "KSI/Resources/AppIcon";
        private static readonly Texture2D AppIcon = GameDatabase.Instance.GetTexture(AppIconPath, false);

        private static readonly string tooltipTitle = Localizer.Format("#KSI_AppLauncher_TooltipTitle");
        private static readonly string tooltipText = Localizer.Format("#KSI_AppLauncher_TooltipText");

        public void Start()
        {
            GameEvents.onGUIApplicationLauncherReady.Add(AddLauncherButton);
            GameEvents.onGUIApplicationLauncherDestroyed.Add(RemoveLauncherButton);
            GameEvents.onHideUI.Add(OnHideUI);
            GameEvents.onShowUI.Add(OnShowUI);
            GameEvents.onGamePause.Add(OnGamePause);
            GameEvents.onGameUnpause.Add(OnGameUnpause);
        }

        public void OnDisable()
        {
            GameEvents.onGUIApplicationLauncherReady.Remove(AddLauncherButton);
            GameEvents.onGUIApplicationLauncherDestroyed.Remove(RemoveLauncherButton);
            RemoveLauncherButton();
        }

        public void OnDestroy()
        {
            GameEvents.onHideUI.Remove(OnHideUI);
            GameEvents.onShowUI.Remove(OnShowUI);
            GameEvents.onGamePause.Remove(OnGamePause);
            GameEvents.onGameUnpause.Remove(OnGameUnpause);
        }

        public void OnGamePause()
        {
            if (view != null)
                view.Dismiss();
        }

        public void OnGameUnpause()
        {
            if (view != null)
                view.Show();
        }

        /// <summary>
		/// Hide UI.
		/// </summary>
		private void OnHideUI()
        {
            if (view != null)
                view.Dismiss();
        }

        /// <summary>
        /// Show UI.
        /// </summary>
        private void OnShowUI()
        {
            if (view != null)
                view.Dismiss();
        }

        private void AddLauncherButton()
        {
            if (ApplicationLauncher.Ready && _button == null)
            {
                _button = ApplicationLauncher.Instance.AddModApplication(
                    OnTrue, OnFalse,
                    null, null,
                    null, null,
                    VisibleInScenes, AppIcon
                );
                GameEvents.onGUIApplicationLauncherUnreadifying.Add(RemoveLauncherButton);
                _button?.gameObject?.SetTooltip(tooltipTitle, tooltipText);
            }
        }

        private void RemoveLauncherButton()
        {
            if (_button == null)
                return;

            ApplicationLauncher.Instance.RemoveModApplication(_button);

            _button = null;
        }

        private void RemoveLauncherButton(GameScenes scenes)
        {
            RemoveLauncherButton();
        }

        public void ToggleButtonState(bool isOn)
        {
            if (_button == null)
                return;

            if (isOn)
                _button.SetTrue(false);
            else
                _button.SetFalse(false);
        }

        private MainWindow view;

        private void OnTrue()
        {
            if (view == null)
            {
                view = new MainWindow(() => { _button.SetFalse(true); });
            }
            view.Show();
        }

        private void OnFalse()
        {
            if (view != null)
            {
                view.Dismiss();
                view = null;
            }
        }
    }
}
