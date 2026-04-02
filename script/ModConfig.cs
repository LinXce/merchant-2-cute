using Godot;

namespace Merchant2Cute.script;

public static class ModConfig
{
	// ========== 可调整参数 ==========
	// 商人视觉位置偏移
	public const float MERCHANT_VISUAL_OFFSET_X = 1280f;
	public const float MERCHANT_VISUAL_OFFSET_Y = 540f;

	// 商人视觉缩放
	public const float MERCHANT_VISUAL_SCALE_X = 0.72f;
	public const float MERCHANT_VISUAL_SCALE_Y = 0.72f;

	// 商人手部缩放
	public const float MERCHANT_HAND_SCALE_X = 0.4f;
	public const float MERCHANT_HAND_SCALE_Y = 0.4f;

	// 指向目标偏移
	public const float POINT_AT_TARGET_OFFSET_X = 0f;
	public const float POINT_AT_TARGET_OFFSET_Y = 10f;

	// Spine动画路径
	public const string SPINE_ANIMATION_PATH = "res://animations/merchant_l.tres";

	// 便捷属性
	public static Vector2 MerchantVisualPositionOffset => new Vector2(MERCHANT_VISUAL_OFFSET_X, MERCHANT_VISUAL_OFFSET_Y);
	public static Vector2 MerchantVisualScale => new Vector2(MERCHANT_VISUAL_SCALE_X, MERCHANT_VISUAL_SCALE_Y);
	public static Vector2 MerchantHandScale => new Vector2(MERCHANT_HAND_SCALE_X, MERCHANT_HAND_SCALE_Y);
	public static Vector2 PointAtTargetOffset => new Vector2(POINT_AT_TARGET_OFFSET_X, POINT_AT_TARGET_OFFSET_Y);
	public static string SpineAnimationPath => SPINE_ANIMATION_PATH;
}