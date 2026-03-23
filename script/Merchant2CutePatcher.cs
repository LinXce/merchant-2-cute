using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Nodes.Rooms;

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
				/**
					注意spine要满足：
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