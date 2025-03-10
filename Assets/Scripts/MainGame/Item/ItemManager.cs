/**
 * @file ItemManager.cs
 * @brief アイテム管理
 * @author yao
 * @date 2025/3/4
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CommonModule;

public class ItemManager : MonoBehaviour {
	[SerializeField]
	private ItemObject _itemObjectOrigin = null;

	[SerializeField]
	private Transform _useObjectRoot = null;
	[SerializeField]
	private Transform _unuseRoot = null;

	public static ItemManager instance { get; private set; } = null;
	// 使用中のアイテムリスト
	private List<ItemBase> _useList = null;
	// 未使用状態のアイテムリスト
	private List<List<ItemBase>> _unuseList = null;

	// 使用中のオブジェクトリスト
	private List<ItemObject> _useObjectList = null;
	// 未使用状態のオブジェクトリスト
	private List<ItemObject> _unuseObjectList = null;

	private readonly int _ITEM_MAX = 256;

	public void Initialize() {
		instance = this;
		ItemBase.SetGetObjectCallback(GetItemObject);
		// アイテムデータ側の初期化
		_useList = new List<ItemBase>(_ITEM_MAX);
		int categoryMax = (int)eItemCategory.Max;
		_unuseList = new List<List<ItemBase>>(categoryMax);
		for (int i = 0; i < categoryMax; i++) {
			_unuseList.Add(new List<ItemBase>(_ITEM_MAX));
			for (int j = 0; j < _ITEM_MAX; j++) {
				// 未使用状態のアイテム追加
				_unuseList[i].Add(CreateCategoryItem((eItemCategory)i));
			}
		}
		// アイテムオブジェクト側の初期化
		_useObjectList = new List<ItemObject>(_ITEM_MAX);
		_unuseObjectList = new List<ItemObject>(_ITEM_MAX);
		for (int i = 0; i < _ITEM_MAX; i++) {
			// 未使用状態のアイテムオブジェクト追加
			_unuseObjectList.Add(Instantiate(_itemObjectOrigin, _unuseRoot));
		}
	}

	/// <summary>
	/// アイテムカテゴリに紐づいたクラスを生成
	/// </summary>
	/// <param name="category"></param>
	/// <returns></returns>
	private ItemBase CreateCategoryItem(eItemCategory category) {
		switch (category) {
			case eItemCategory.Potion:
			return new ItemPotion();
			case eItemCategory.Food:
			return new ItemFood();
			case eItemCategory.Wand:
			return new ItemWand();
		}
		return null;
	}

	/// <summary>
	/// アイテムオブジェクト取得
	/// </summary>
	/// <param name="ID"></param>
	/// <returns></returns>
	private ItemObject GetItemObject(int ID) {
		if (!IsEnableIndex(_useObjectList, ID)) return null;

		return _useObjectList[ID];
	}

	/// <summary>
	/// アイテムデータの取得
	/// </summary>
	/// <param name="ID"></param>
	/// <returns></returns>
	public ItemBase GetItemData(int ID) {
		if (!IsEnableIndex(_useList, ID)) return null;

		return _useList[ID];
	}

	/// <summary>
	/// 床落ちアイテムの生成
	/// </summary>
	/// <param name="masterID"></param>
	/// <param name="square"></param>
	/// <returns></returns>
	public ItemBase UseFloorItem(int masterID, MapSquareData square) {
		// 使用可能なインスタンス取得
		var itemMaster = ItemMasterUtility.GetItemMaster(masterID);
		if (itemMaster == null) return null;
		// データの生成
		int useID = UseItemData(itemMaster.category);
		// オブジェクトの生成
		UseItemObject(useID);

		ItemBase useItem = GetItemData(useID);
		useItem.Setup(useID, masterID, square);
		return useItem;
	}

	/// <summary>
	/// アイテムデータの使用化
	/// </summary>
	/// <param name="categoryIndex"></param>
	/// <returns></returns>
	private int UseItemData(int categoryIndex) {
		ItemBase useItem = GetUsableItemData(categoryIndex);
		// 使用可能なIDを取得して使用リストに追加
		int useID = -1;
		for (int i = 0, max = _useList.Count; i < max; i++) {
			if (_useList[i] != null) continue;

			_useList[i] = useItem;
			useID = i;
			break;
		}
		if (useID < 0) {
			useID = _useList.Count;
			_useList.Add(useItem);
		}
		return useID;
	}

	/// <summary>
	/// アイテムオブジェクトの使用化
	/// </summary>
	/// <param name="useID"></param>
	public void UseItemObject(int useID) {
		ItemObject useObject = null;
		if (IsEmpty(_unuseObjectList)) {
			// 未使用リストが空なので生成
			useObject = Instantiate(_itemObjectOrigin, _useObjectRoot);
		} else {
			// 未使用があるので使用
			useObject = _unuseObjectList[0];
			_unuseObjectList.RemoveAt(0);
			useObject.transform.SetParent(_useObjectRoot);
		}
		// 仕様オブジェクトリストへの追加
		while (!IsEnableIndex(_useObjectList, useID)) _useObjectList.Add(null);

		_useObjectList[useID] = useObject;
	}

	/// <summary>
	/// アイテムの未使用化
	/// </summary>
	/// <param name="ID"></param>
	public void UnuseItemData(int ID) {
		if (!IsEnableIndex(_useList, ID) || _useList[ID] == null) return;
		// データの未使用化
		ItemBase unuseItem = _useList[ID];
		_useList[ID] = null;
		unuseItem.Teardown();
		_unuseList[(int)unuseItem.GetCategory()].Add(unuseItem);
		// オブジェクトの未使用化
		UnuseItemObject(ID);
	}

	/// <summary>
	/// アイテムオブジェクトの未使用化
	/// </summary>
	/// <param name="ID"></param>
	public void UnuseItemObject(int ID) {
		if (!IsEnableIndex(_useObjectList, ID) || _useObjectList[ID] == null) return;
		// オブジェクトの未使用化
		ItemObject unuseObject = _useObjectList[ID];
		_useObjectList[ID] = null;
		unuseObject.Teardown();
		_unuseObjectList.Add(unuseObject);
		unuseObject.transform.SetParent(_useObjectRoot);
	}

	/// <summary>
	/// 使用可能なアイテムデータのインスタンスを渡す
	/// </summary>
	/// <param name="category"></param>
	/// <returns></returns>
	private ItemBase GetUsableItemData(int categoryIndex) {
		List<ItemBase> targetList = _unuseList[categoryIndex];
		// 未使用状態のインスタンスが無ければ生成して返す
		if (IsEmpty(targetList)) return CreateCategoryItem((eItemCategory)categoryIndex);
		// 未使用状態のリストから1つ返す
		ItemBase result = targetList[0];
		targetList.RemoveAt(0);
		return result;
	}

	/// <summary>
	/// 全ての使用中アイテムに指定の処理を実行
	/// </summary>
	/// <param name="action"></param>
	public void ExecuteAllItem(System.Action<ItemBase> action) {
		if (action == null || IsEmpty(_useList)) return;

		for (int i = 0, max = _useList.Count; i < max; i++) {
			if (_useList[i] == null) continue;

			action(_useList[i]);
		}
	}

}
