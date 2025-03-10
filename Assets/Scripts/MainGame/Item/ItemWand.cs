/**
 * @file ItemWand.cs
 * @brief 杖アイテムデータ
 * @author yao
 * @date 2025/3/6
 */

public class ItemWand : ItemBase {
	/// <summary>
	/// カテゴリ取得
	/// </summary>
	/// <returns></returns>
	public override eItemCategory GetCategory() {
		return eItemCategory.Wand;
	}
}
