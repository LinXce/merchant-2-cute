using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Screens.Shops;
namespace Merchant2Cute.script;

[HarmonyPatch(typeof(NMerchantButton), "_Ready")]
public static class Merchant2CutePatcher
{
	private const string LMerchantTopSpinePath = "res://animations/merchant_l.tres";

	[HarmonyPrefix]
	public static bool OnMerchantButtonReady(NMerchantButton __instance)
	{
		if (__instance == null) return true;

		try
		{
			var raw = ResourceLoader.Load(LMerchantTopSpinePath);
			if (raw == null)
			{
				GD.PrintErr($"[Merchant2Cute] Cannot load: {LMerchantTopSpinePath}");
				return true;
			}

			GD.Print($"[Merchant2Cute] Loaded raw resource: {raw.GetType()}");

			var merchantVisual = __instance.GetNodeOrNull("%MerchantVisual");
			if (merchantVisual != null)
			{
				// _Ready 使用新skeldata
				merchantVisual.Set("skeleton_data_res", Variant.From(raw));
				var currentPos = merchantVisual.Get("position").AsVector2();
				merchantVisual.Set("position", currentPos + new Vector2(1280, 540));
				merchantVisual.Set("scale", new Vector2(0.72f, 0.72f));
				/**
					注意spine要保证存在:
					Animation:idle_loop
					skin:default,outline
					（理论上可以自己设置，原动画默认如上）
				*/
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
public static class NMerchantHandScalePatcher
{
	[HarmonyPostfix]
	public static void OnNMerchantHandReady(NMerchantHand __instance)
	{
		try
		{
			var parentField = AccessTools.Field(typeof(NMerchantHand), "_parent");
			var parent = parentField.GetValue(__instance) as Node2D;
			if (parent != null)
			{
				// var currentPos = parent.Get("position").AsVector2();
				// parent.Set("position", currentPos + new Vector2(0, 200));
				parent.Set("scale", new Vector2(0.4f, 0.4f));
				GD.Print("[Merchant2Cute] Set scale on NMerchantHand's parent");
			}
		}
		catch (System.Exception ex)
		{
			GD.PrintErr($"[Merchant2Cute] Error setting scale: {ex.Message}");
		}
	}
}

[HarmonyPatch(typeof(NMerchantHand), "PointAtTarget")]
public static class NMerchantHandPointAtTargetPatcher
{
	[HarmonyPostfix]
	public static void OnPointAtTarget(NMerchantHand __instance, Vector2 pos)
	{
		try
		{
			var targetPosField = AccessTools.Field(typeof(NMerchantHand), "_targetPos");
			var value = targetPosField.GetValue(__instance);
			if (value != null)
			{
				var currentTarget = (Vector2)value;
				targetPosField.SetValue(__instance, currentTarget + new Vector2(0, 10f));
				GD.Print("[Merchant2Cute] Adjusted y-axis in PointAtTarget");
			}
		}
		catch (System.Exception ex)
		{
			GD.PrintErr($"[Merchant2Cute] Error adjusting PointAtTarget: {ex.Message}");
		}
	}
}