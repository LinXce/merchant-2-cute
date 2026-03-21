using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Merchant2Cute;

[ModInitializer("Init")]
public static class Merchant2CutePatcher
{
	private static Harmony? _harmony;
	private const string CustomMerchantTopSpinePath = "res://animations/merchant/custom_merchant_top.tres";
	private static bool _patched = false;

	public static void Init()
	{
		if (_patched) return;

		try
		{
			_harmony ??= new Harmony("Merchant2Cute.Mod");

			var readyMethod = AccessTools.Method(typeof(NMerchantButton), "_Ready");
			if (readyMethod != null)
			{
				var postfix = typeof(Merchant2CutePatcher).GetMethod(nameof(OnMerchantButtonReady));
				_harmony.Patch(readyMethod, postfix: new HarmonyMethod(postfix));

				_patched = true;
				GD.Print("[Merchant2Cute] Patcher initialized");
			}
			else
			{
				GD.PrintErr("[Merchant2Cute] Failed to find NMerchantButton._Ready");
			}
		}
		catch (System.Exception ex)
		{
			GD.PrintErr($"[Merchant2Cute] Init failed: {ex.Message}");
		}
	}

	public static void OnMerchantButtonReady(NMerchantButton __instance)
	{
		if (__instance == null) return;

		try
		{
			// 获取私有字段 _merchantSkeleton
			var skeletonField = AccessTools.Field(typeof(NMerchantButton), "_merchantSkeleton");
			var skeleton = skeletonField?.GetValue(__instance) as MegaSkeleton;

			if (skeleton != null)
			{
				var customSpine = ResourceLoader.Load<MegaSkeletonDataResource>(CustomMerchantTopSpinePath);

				if (customSpine != null)
				{
					GD.Print($"[Merchant2Cute] Loaded custom spine: {CustomMerchantTopSpinePath}");

					// 通过节点方式替换
					ReplaceViaMerchantVisual(__instance);
				}
				else
				{
					GD.PrintErr($"[Merchant2Cute] Cannot load: {CustomMerchantTopSpinePath}");
				}
			}
			else
			{
				ReplaceViaMerchantVisual(__instance);
			}
		}
		catch (System.Exception ex)
		{
			GD.PrintErr($"[Merchant2Cute] Error: {ex.Message}");
		}
	}

	// 通过 MerchantVisual 节点直接替换
	private static void ReplaceViaMerchantVisual(NMerchantButton button)
	{
		try
		{
			// 查找 %MerchantVisual 节点
			var merchantVisual = button.GetNodeOrNull("%MerchantVisual");

			if (merchantVisual != null)
			{
				GD.Print($"[Merchant2Cute] Found MerchantVisual: {merchantVisual.GetType()}");

				// 获取节点的所有属性，看看有哪些可用的
				var properties = merchantVisual.GetPropertyList();

				// 设置 skeleton_data_resource 属性
				try
				{
					var customSpine = ResourceLoader.Load<MegaSkeletonDataResource>(CustomMerchantTopSpinePath);
					var propertyInfo = merchantVisual.GetType().GetProperty("SkeletonDataResource");
					if (propertyInfo != null && propertyInfo.CanWrite)
					{
						propertyInfo.SetValue(merchantVisual, customSpine);
						GD.Print("[Merchant2Cute] Replaced via property");
						return;
					}
					GD.Print("[Merchant2Cute] Set via set_skeleton_data_resource");
				}
				catch
				{
					GD.Print("[Merchant2Cute] No suitable property/method found, trying child nodes");
				}
			}
			else
			{
				GD.PrintErr("[Merchant2Cute] Cannot find %MerchantVisual node");
			}
		}
		catch (System.Exception ex)
		{
			GD.PrintErr($"[Merchant2Cute] ReplaceViaMerchantVisual error: {ex.Message}");
		}
	}
}