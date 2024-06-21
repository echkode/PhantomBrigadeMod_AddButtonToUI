// Copyright (c) 2024 EchKode
// SPDX-License-Identifier: BSD-3-Clause

using PhantomBrigade;

using UnityEngine;

namespace EchKode.PBMods.AddButtonToUI
{
	public static class CustomPilotInfo
	{
		public static void OnCustom()
		{
			if (CIViewBasePilotInfo.ins == null)
			{
				return;
			}

			var pilot = IDUtility.GetPersistentEntity(CIViewBasePilotInfo.ins.cachedNameInternal);
			if (pilot == null)
			{
				return;
			}

			// Code to handle the button click should replace this log statement.
			Debug.LogFormat("Mod {0} ({1}) Showing custom info for {2}", ModLink.ModIndex, ModLink.ModID, pilot.ToLog());
		}
	}
}
