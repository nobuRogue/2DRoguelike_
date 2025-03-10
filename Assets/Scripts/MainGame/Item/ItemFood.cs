/**
 * @file ItemFood.cs
 * @brief 食べ物アイテムデータ
 * @author yao
 * @date 2025/3/6
 */
public class ItemFood : ItemBase {
	/// <summary>
	/// カテゴリ取得
	/// </summary>
	/// <returns></returns>
	public override eItemCategory GetCategory() {
		return eItemCategory.Food;
	}
}
