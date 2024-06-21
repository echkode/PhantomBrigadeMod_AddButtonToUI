// Copyright (c) 2024 EchKode
// SPDX-License-Identifier: BSD-3-Clause

using HarmonyLib;

using PhantomBrigade.Data;

using UnityEngine;

namespace EchKode.PBMods.AddButtonToUI
{
	[HarmonyPatch]
	static partial class Patch
	{
		[HarmonyPatch(typeof(CIViewBasePilotInfo), nameof(CIViewBasePilotInfo.RedrawForPilot))]
		[HarmonyPostfix]
		static void Civbpi_RedrawForPilotPostfix(
			CIViewBasePilotInfo __instance,
			PersistentEntity pilot,
			bool editable)
		{
			var (found, buttonCustom) = FindCustomButton(__instance);
			switch (found)
			{
				case FindResult.NotFound:
					buttonCustom = CreateCustomButton(__instance);
					break;
				case FindResult.Error:
					return;
			}

			if (pilot == null)
			{
				buttonCustom.gameObject.SetActive(false);
				return;
			}
			buttonCustom.gameObject.SetActive(editable);
		}

		static (FindResult, CIButton) FindCustomButton (CIViewBasePilotInfo view)
		{
			var buttonEdit = view.buttonEdit;
			var parent = buttonEdit.transform.parent;
			var transformCustom = parent.Find(nameCustomButton);
			if (transformCustom == null)
			{
				return (FindResult.NotFound, null);
			}

			var buttonCustom = transformCustom.gameObject.GetComponent<CIButton>();
			if (buttonCustom == null)
			{
				Debug.LogWarningFormat(
					"Mod {0} ({1}) Found game object with name \"{2}\" but without CIButton component -- somebody has hijacked our name",
					ModLink.ModIndex,
					ModLink.ModID,
					nameCustomButton);
				return (FindResult.Error, null);
			}
			return (FindResult.Found, buttonCustom);
		}

		static CIButton CreateCustomButton(CIViewBasePilotInfo view)
		{
			var buttonEdit = view.buttonEdit;
			var parent = buttonEdit.transform.parent;
			var o = Object.Instantiate(buttonEdit, parent);
			var go = o.gameObject;
			go.name = nameCustomButton;
			go.layer = layerUI;

			var widget = go.GetComponent<UIWidget>();
			// Make the custom button the same size as the existing edit button.
			InitializeAnchors(widget, buttonEdit.transform);
			// Move the custom button into place.
			widget.bottomAnchor.Set(buttonEdit.transform, 1f, buttonSeparation);
			widget.topAnchor.Set(buttonEdit.transform, 1f, buttonSeparation + widget.height);

			go.SetActive(true);

			var buttonCustom = o.GetComponent<CIButton>();
			foreach (var elem in buttonCustom.elements)
			{
				elem.widget.gameObject.SetActive(true);
				if (elem.widget is UILabel label)
				{
					// There's an extra game object under the label from the clone.
					// It's a UILabelSymbols object that we don't need. Remove it and let
					// the label make a new one that's properly initialized.
					NGUITools.DestroyChildren(label.transform);
					Co.Delay(0.1f, () => RefreshButtonText(label));
				}
				else if (elem.widget.name == nameSpriteFill)
				{
					elem.colorMain = colorCustom;
				}
			}
			buttonCustom.callbackOnClick = new UICallback(CustomPilotInfo.OnCustom);
			return buttonCustom;
		}

		static void InitializeAnchors(UIWidget widget, Transform t)
		{
			widget.leftAnchor.Set(t, 0.0f, 0.0f);
			widget.rightAnchor.Set(t, 1f, 0.0f);
			widget.topAnchor.Set(t, 1f, 0.0f);
			widget.bottomAnchor.Set(t, 0.0f, 0.0f);
			widget.updateAnchors = UIRect.AnchorUpdate.OnUpdate;
			widget.ResetAnchors();
			widget.UpdateAnchors();
		}

		static void RefreshButtonText(UILabel label)
		{
			// Give Unity time to call the various lifecycle events to stand up
			// the new button object before swapping out its text.
			label.text = DataManagerText.GetText(TextLibs.uiBase, keyButtonText);
			label.SetDirty();
		}

		// Use a unique prefix on the localization key to ensure no conflicts with keys from the game.
		const string keyButtonText = "ek_pilot_info_button_custom";
		// Try to avoid naming conflicts with game objects as well.
		const string nameCustomButton = "EK_Button_Custom";
		// This name was discovered by dumping the existing Edit button.
		const string nameSpriteFill = "Sprite_Fill_Idle";

		// All UI objects should be on this layer.
		const int layerUI = 5;
		const int buttonSeparation = 8;

		static readonly Color colorCustom = new Color(0.201f, 0.377f, 0.317f);

		enum FindResult
		{
			Error,
			NotFound,
			Found,
		}
	}
}
