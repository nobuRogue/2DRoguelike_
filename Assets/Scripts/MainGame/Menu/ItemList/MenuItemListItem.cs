/**
 * @file MenuItemListItem.cs
 * @brief アイテムリストの項目クラス
 * @author yao
 * @date 2025/3/6
 */

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuItemListItem : MenuListItem {
	/// <summary>
	/// アイテムアイコン画像
	/// </summary>
	[SerializeField]
	private Image _itemIconImage = null;
	/// <summary>
	/// アイテム名テキスト
	/// </summary>
	[SerializeField]
	private TextMeshProUGUI _itemNameText = null;

	public void Setup(int itemID) {
		var itemData = ItemUtility.GetItemData(itemID);
		// アイコン画像の設定
		Sprite[] itemSpriteList = Resources.LoadAll<Sprite>(GameConst.ITEM_SPRITE_FILE_NAME);
		_itemIconImage.sprite = itemSpriteList[(int)itemData.GetCategory()];
		// アイテム名の設定
		_itemNameText.text = itemData.GetItemName();
	}

}
