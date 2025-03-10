/**
 * @file MenuListItem.cs
 * @brief リスト項目の基底クラス
 * @author yao
 * @date 2025/3/6
 */

using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CommonModule;

public class MenuItemList : MenuList {

	public override async UniTask Initialize() {
		await base.Initialize();
		// コールバックの設定
		var itemListFormat = new MenuListCallbackFortmat();
		itemListFormat.OnDecide = CloseItemList;
		itemListFormat.OnCancel = CloseItemList;
		SetCallbackFortmat(itemListFormat);
	}

	/// <summary>
	/// アイテムリストの入力受付を終了する
	/// </summary>
	/// <param name="currentItem"></param>
	/// <returns></returns>
	private async UniTask<bool> CloseItemList(MenuListItem currentItem) {
		await UniTask.CompletedTask;
		return false;
	}

	/// <summary>
	/// リスト項目の生成
	/// </summary>
	/// <param name="itemList"></param>
	/// <returns></returns>
	public async UniTask Setup(int[] itemList) {
		await SetIndex(-1);
		RemoveAllItem();
		if (IsEmpty(itemList)) return;
		// 項目の生成
		bool existItem = false;
		for (int i = 0, max = itemList.Length; i < max; i++) {
			if (itemList[i] < 0) break;
			// 項目有無の判定
			if (!existItem) existItem = true;
			// 項目の生成
			var addItem = AddListItem() as MenuItemListItem;
			addItem.Setup(itemList[i]);
		}
		if (existItem) await SetIndex(0);

	}

}
