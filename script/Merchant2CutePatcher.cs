using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Merchant2Cute.script;

[HarmonyPatch(typeof(NMerchantButton), "_Ready")]
public static class Merchant2CutePatcher
{
	private const string LMerchantTopSpinePath = "res://animations/merchant_l.tres";

	[HarmonyPostfix]
	public static void OnMerchantButtonReady(NMerchantButton __instance)
	{
		if (__instance == null) return;

		// GD.Print("[Merchant2Cute] OnMerchantButtonReady ENTER");

		try
		{
			// 获取私有字段 _merchantSkeleton
			var skeletonField = AccessTools.Field(typeof(NMerchantButton), "_merchantSkeleton");
			var skeleton = skeletonField?.GetValue(__instance) as MegaSkeleton;

			// 加载 Spine
			var raw = ResourceLoader.Load(LMerchantTopSpinePath);
			if (raw == null)
			{
				GD.PrintErr($"[Merchant2Cute] Cannot load: {LMerchantTopSpinePath}");
				return;
			}
			GD.Print($"raw type = {raw.GetType().FullName}"); // SpineSkeletonDataResource

			var spineData = raw;
			ReplaceViaMerchantVisual(__instance, spineData);
		}
		catch (System.Exception ex)
		{
			GD.PrintErr($"[Merchant2Cute] Error: {ex.Message}");
		}
	}

	// 通过 MerchantVisual 节点直接替换
	private static void ReplaceViaMerchantVisual(NMerchantButton button, Resource customSpine)
	{
		try
		{
			var merchantVisual = button.GetNodeOrNull("%MerchantVisual");
			if (merchantVisual == null)
			{
				GD.PrintErr("[Merchant2Cute] Cannot find %MerchantVisual node");
				return;
			}

			GD.Print($"[Merchant2Cute] Found MerchantVisual: {merchantVisual.GetType()}");

			// 直接设置 SpineSprite 属性
			try
			{
				merchantVisual.Set("skeleton_data_res", customSpine);
				merchantVisual.Call("set_skeleton_data_res", customSpine);
				GD.Print("[Merchant2Cute] Replaced via skeleton_data_res");
				return;
			}
			catch (System.Exception e)
			{
				GD.Print($"[Merchant2Cute] set skeleton_data_res failed: {e.Message}");
			}

			// 尝试反射 CamelCase 名字
			var propertyInfo = merchantVisual.GetType().GetProperty("SkeletonDataResource");
			if (propertyInfo != null && propertyInfo.CanWrite)
			{
				propertyInfo.SetValue(merchantVisual, customSpine);
				GD.Print("[Merchant2Cute] Replaced via MerchantVisual.SkeletonDataResource");
				return;
			}

			// 检查 child
			GD.Print("[Merchant2Cute] Trying child nodes");
			foreach (Node child in merchantVisual.GetChildren())
			{
				var childPropInfo = child.GetType().GetProperty("SkeletonDataResource");
				if (childPropInfo != null && childPropInfo.CanWrite)
				{
					childPropInfo.SetValue(child, customSpine);
					GD.Print($"[Merchant2Cute] Replaced via child node property: {child.GetType()}");
					return;
				}
				try
				{
					child.Set("skeleton_data_res", customSpine);
					child.Call("set_skeleton_data_res", customSpine);
					GD.Print($"[Merchant2Cute] Replaced via child node set/call: {child.GetType()}");
					return;
				}
				catch { }
			}

			GD.PrintErr("[Merchant2Cute] No suitable property found in MerchantVisual or children");
		}
		catch (System.Exception ex)
		{
			GD.PrintErr($"[Merchant2Cute] ReplaceViaMerchantVisual error: {ex.Message}");
		}
	}
}