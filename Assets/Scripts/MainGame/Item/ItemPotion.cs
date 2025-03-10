/**
 * @file ItemPotion.cs
 * @brief 薬アイテムデータ
 * @author yao
 * @date 2025/3/4
 */

public class ItemPotion : ItemBase {
	/// <summary>
	/// カテゴリ取得
	/// </summary>
	/// <returns></returns>
	public override eItemCategory GetCategory() {
		return eItemCategory.Potion;
	}
}
