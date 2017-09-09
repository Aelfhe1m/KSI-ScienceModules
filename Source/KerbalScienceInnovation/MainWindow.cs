using KSP.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace KerbalScienceInnovation
{
    public class MainWindow : DialogGUIVerticalLayout
    {
        private const float leftColWidth = 90;
        private const float rightColWidth = 1.5f * leftColWidth;
        private const float textFieldHeight = 25;
        private const float boxWidth = leftColWidth + rightColWidth;
        private const float boxHeight = 5 * textFieldHeight;
        private const float boxSpacing = 2;
        private const int bigPadding = 10;
        private static readonly RectOffset boxPadding = new RectOffset(bigPadding, bigPadding, bigPadding, bigPadding);
        private static readonly RectOffset winPadding = new RectOffset(2, 2, 2, 2);
        private const float windowWidth = 2 * boxWidth + 2 * bigPadding;
        private const float textureWidth = 200;
        private const float textureHeight = textureWidth;

        private PopupDialog dialog;

        private static Version modVersion = Assembly.GetExecutingAssembly().GetName().Version;

        public MainWindow(UnityAction close)
            : base(
                windowWidth, -1, 2,
                winPadding,
                TextAnchor.UpperLeft
            )
        {
            // setup dialog here
                AddChild(new DialogGUIHorizontalLayout(
				windowWidth, -1,

				new DialogGUIBox(
					"", boxWidth, boxHeight, null,

					new DialogGUIVerticalLayout(
						boxWidth, boxHeight, boxSpacing, boxPadding,
						TextAnchor.UpperLeft,

						new DialogGUILabel("TODO")
					)
				),
				new DialogGUIBox(
					"", boxWidth, boxHeight, null,

					new DialogGUIVerticalLayout(
						boxWidth, boxHeight, boxSpacing, boxPadding,
						TextAnchor.UpperLeft,

						new DialogGUILabel("TODO"),

						new DialogGUIHorizontalLayout(
							TextAnchor.MiddleLeft,

							new DialogGUILabel("TEST")
						)
					)
				)
			));

			AddChild(new DialogGUISpace(bigPadding));
			AddChild(new DialogGUIHorizontalLayout(
				new DialogGUIFlexibleSpace(),
				new DialogGUIButton(
					"#KSI_MainWindow_CloseButtonText",
					() => { close(); },
					200, -1,
					false
				),
				new DialogGUIFlexibleSpace()
			));
        }

        /// <summary>
		/// Create a MultiOptionDialog containing this view
		/// and display it on the screen.
		/// </summary>
		/// <returns>
		/// The PopupDialog being displayed.
		/// </returns>
		public PopupDialog Show()
        {
            dialog = PopupDialog.SpawnPopupDialog(
                new MultiOptionDialog(
                    "KSIMainWindow",
                    "#KSI_MainWindow_Subtitle",
                    Localizer.Format(
                        "#KSI_MainWindow_Title",
                        modVersion.Major, modVersion.Minor, modVersion.Build, modVersion.Revision
                    ),
                    UISkinManager.defaultSkin,
                    windowWidth,
                    this
                ),
                false,
                UISkinManager.defaultSkin,
                true
            );
            return dialog;
        }

        /// <summary>
        /// Save the settings and close the visible dialog, if any.
        /// </summary>
        public void Dismiss()
        {
            if (dialog != null)
            {
                // Settings.Instance.Save();
                dialog.Dismiss();
                dialog = null;
            }
        }
    }
}
