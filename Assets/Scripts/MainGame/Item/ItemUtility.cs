/**
 * @file ItemUtility.cs
 * @brief アイテム関連実行処理
 * @author yao
 * @date 2025/3/4
 */

public class ItemUtility {
	/// <summary>
	/// 全ての使用中アイテムに指定の処理を実行
	/// </summary>
	/// <param name="action"></param>
	public static void ExecuteAllItem(System.Action<ItemBase> action) {
		ItemManager.instance.ExecuteAllItem(action);
	}

	/// <summary>
	/// 床落ちアイテム生成
	/// </summary>
	/// <param name="masterID"></param>
	/// <param name="createSquare"></param>
	/// <returns></returns>
	public static ItemBase CreateFloorItem(int masterID, MapSquareData createSquare) {
		return ItemManager.instance.UseFloorItem(masterID, createSquare);
	}

	/// <summary>
	/// アイテムを未使用状態にする
	/// </summary>
	/// <param name="ID"></param>
	public static void UnuseItem(int ID) {
		ItemManager.instance.UnuseItemData(ID);
	}

	/// <summary>
	/// アイテムデータ取得
	/// </summary>
	/// <param name="ID"></param>
	/// <returns></returns>
	public static ItemBase GetItemData(int ID) {
		return ItemManager.instance.GetItemData(ID);
	}

}
