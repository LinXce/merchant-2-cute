using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Screens.Shops;
using MegaCrit.Sts2.Core.Nodes.Events.Custom;

namespace Merchant2Cute.script;

public static class MerchantState
{
	// 默认真商人
	public static bool IsRealMerchant = true;
}

[HarmonyPatch(typeof(NMerchantButton), "_Ready")]
public static class LMerchant2CutePatcher
{
	[HarmonyPrefix]
	public static bool OnMerchantButtonReady(NMerchantButton __instance)
	{
		try
		{
			if (__instance == null) return true;

			Node current = __instance.GetParent();
			bool isFakeMerchant = false;

			for (int i = 0; i < 3 && current != null; ++i)
			{
				GD.Print($"[Merchant2Cute] Checking parent: {current.GetType().FullName}");

				string typeName = current.GetType().FullName;
				if (typeName.Contains("NFakeMerchant"))
				{
					isFakeMerchant = true;
					GD.Print($"[Merchant2Cute] Found NFakeMerchant at: {typeName}");
					break;
				}

				current = current.GetParent();
			}

			MerchantState.IsRealMerchant = !isFakeMerchant;

			if (isFakeMerchant)
			{
				GD.Print("[Merchant2Cute] Fake merchant detected, skipping modifications");
				return true;
			}

			var raw = ResourceLoader.Load(ModConfig.SpineAnimationPath);
			if (raw == null)
			{
				GD.PrintErr($"[Merchant2Cute] Cannot load: {ModConfig.SpineAnimationPath}");
				return true;
			}

			GD.Print($"[Merchant2Cute] Loaded raw resource: {raw.GetType()}");

			var merchantVisual = __instance.GetNodeOrNull("%MerchantVisual");
			if (merchantVisual != null)
			{
				merchantVisual.Set("skeleton_data_res", Variant.From(raw));
				var currentPos = merchantVisual.Get("position").AsVector2();
				merchantVisual.Set("position", currentPos + ModConfig.MerchantVisualPositionOffset);
				merchantVisual.Set("scale", ModConfig.MerchantVisualScale);

				GD.Print("[Merchant2Cute] Set skeleton_data_res in Prefix");
			}
			else
			{
				GD.PrintErr("[Merchant2Cute] Cannot find %MerchantVisual in Prefix");
			}
		}
		catch (System.Exception ex)
		{
			GD.PrintErr($"[Merchant2Cute] Error in Prefix: {ex.Message}");
		}
		return true;
	}
}

[HarmonyPatch(typeof(NMerchantHand), "_Ready")]
public static class LMerchantHandScalePatcher
{
	[HarmonyPostfix]
	public static void OnNMerchantHandReady(NMerchantHand __instance)
	{
		if (!MerchantState.IsRealMerchant) return;

		try
		{
			var parentField = AccessTools.Field(typeof(NMerchantHand), "_parent");
			var parent = parentField.GetValue(__instance) as Node2D;
			if (parent != null)
			{
				parent.Set("scale", ModConfig.MerchantHandScale);
				GD.Print($"[Merchant2Cute] Set scale on NMerchantHand's parent: {ModConfig.MerchantHandScale}");
			}
		}
		catch (System.Exception ex)
		{
			GD.PrintErr($"[Merchant2Cute] Error setting scale: {ex.Message}");
		}
	}
}

[HarmonyPatch(typeof(NMerchantHand), "PointAtTarget")]
public static class LMerchantHandPointAtTargetPatcher
{
	[HarmonyPostfix]
	public static void OnPointAtTarget(NMerchantHand __instance, Vector2 pos)
	{
		if (!MerchantState.IsRealMerchant) return;

		try
		{
			var targetPosField = AccessTools.Field(typeof(NMerchantHand), "_targetPos");
			var value = targetPosField.GetValue(__instance);
			if (value != null)
			{
				var currentTarget = (Vector2)value;
				targetPosField.SetValue(__instance, currentTarget + ModConfig.PointAtTargetOffset);
				// GD.Print($"[Merchant2Cute] Adjusted position in PointAtTarget by: {ModConfig.PointAtTargetOffset}");
			}
		}
		catch (System.Exception ex)
		{
			GD.PrintErr($"[Merchant2Cute] Error adjusting PointAtTarget: {ex.Message}");
		}
	}
}

[HarmonyPatch(typeof(NMerchantRoom), "FoulPotionThrown")]
public static class LMerchantFoulPotion
{
	[HarmonyPostfix]
	public static void OnFoulPotionThrown(NMerchantRoom __instance)
	{
		try
		{
			var merchantVisual = __instance.GetNodeOrNull("%MerchantVisual");
			if (merchantVisual != null)
			{
				var megaSprite = new MegaSprite(merchantVisual);
				megaSprite.GetAnimationState().SetAnimation("poison");
				GD.Print("[Merchant2Cute] Set animation to poison");
			}
			else
			{
				GD.PrintErr("[Merchant2Cute] Cannot set animation in poison");
			}
		}
		catch (System.Exception ex)
		{
			GD.PrintErr($"[Merchant2Cute] Error adjusting FoulPotionThrown: {ex.Message}");
		}
	}
}