/**
 * @file MenuItemListItem.cs
 * @brief �A�C�e�����X�g�̍��ڃN���X
 * @author yao
 * @date 2025/3/6
 */

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuItemListItem : MenuListItem {
	/// <summary>
	/// �A�C�e���A�C�R���摜
	/// </summary>
	[SerializeField]
	private Image _itemIconImage = null;
	/// <summary>
	/// �A�C�e�����e�L�X�g
	/// </summary>
	[SerializeField]
	private TextMeshProUGUI _itemNameText = null;

	public void Setup(int itemID) {
		var itemData = ItemUtility.GetItemData(itemID);
		// �A�C�R���摜�̐ݒ�
		Sprite[] itemSpriteList = Resources.LoadAll<Sprite>(GameConst.ITEM_SPRITE_FILE_NAME);
		_itemIconImage.sprite = itemSpriteList[(int)itemData.GetCategory()];
		// �A�C�e�����̐ݒ�
		_itemNameText.text = itemData.GetItemName();
	}

}
