using KSP.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace KerbalScienceInnovation
{
    public class GRBMessageBox : DialogGUIVerticalLayout
    {
        private static float windowWidth = 600;
        private static readonly RectOffset winPadding = new RectOffset(2, 2, 2, 2);

        private static readonly Vector2 imageSize = new Vector2(300, 300);
        private static readonly Vector2 imagePosition = new Vector2(0, 0);
        private Color colour = new Color(1, 1, 1);

        private PopupDialog dialog;
        private Texture2D image = null;
        

        public GRBMessageBox(UnityAction close, GRBEvent grbEvent)
            : base(
                windowWidth, -1, 2,
                winPadding,
                TextAnchor.UpperLeft
            )
        {
            if (GameDatabase.Instance.ExistsTexture(grbEvent.ImageUrl))
            {
                image = GameDatabase.Instance.GetTexture(grbEvent.ImageUrl, false);
            }
            else
            {
                Debug.Log($"[KSI] Unknown texture: {grbEvent.ImageUrl}");
            }

            string formattedTime = KSPUtil.dateTimeFormatter.PrintDate(grbEvent.UT, includeTime: true, includeSeconds: true);

            AddChild(
                new DialogGUIHorizontalLayout(
                    new DialogGUIImage(imageSize, imagePosition, colour, image),
                    new DialogGUISpace(10),
                    new DialogGUILabel(Localizer.Format(grbEvent.Message, formattedTime, grbEvent.RA, grbEvent.Dec)) 
                    // Our orbital telescope detected an intense gamma ray event at <<1>> coming from <<2>>, <<3>>
                )
            );
            AddChild(
                new DialogGUISpace(30)
            );
            AddChild(
                new DialogGUIHorizontalLayout(
                    new DialogGUIFlexibleSpace(),
                    new DialogGUIButton(
                        "#KSI_MainWindow_CloseButtonText",
                        () => { close(); },
                        200, -1,
                        false
                    ),
                    new DialogGUIFlexibleSpace()
                )
            );
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
                    "GRBEventWindow",
                    "",
                    "#KSI_GRBWindow_Title",
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
