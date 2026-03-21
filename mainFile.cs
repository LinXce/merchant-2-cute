/*
 * Merchant2Cute - A Slay the Spire 2 Mod
 * Copyright (C) 2026 LinXce
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;

namespace Merchant2Cute;

[ModInitializer("Init")]
public static class Merchant2CuteMod
{
	private const string ModName = "Merchant2CuteMod";
	public static void Init()
	{
		var harmony = new Harmony(ModName);
		harmony.PatchAll();
		GD.Print("[Merchant2Cute] Mod initialized");
	}
}