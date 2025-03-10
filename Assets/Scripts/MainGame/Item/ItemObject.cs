/**
 * @file ItemObject.cs
 * @brief アイテムオブジェクト
 * @author yao
 * @date 2025/3/4
 */
using UnityEngine;

public class ItemObject : MonoBehaviour {
	
	[SerializeField]
	private SpriteRenderer _itemSprite = null;

	public int ID { get; private set; } = -1;

	public void Setup(int setID, Entity_ItemData.Param itemMaster) {
		ID = setID;
		// カテゴリから見た目を取得
		_itemSprite.sprite = Resources.LoadAll<Sprite>(GameConst.ITEM_SPRITE_FILE_NAME)[itemMaster.category];
	}

	public void Teardown() {
		ID = -1;
		_itemSprite.sprite = null;
	}

	public void SetSquare(MapSquareData square) {
		transform.position = square.GetCharacterRoot().position;
	}

	public void UnuseSelf() {
		// 自身を未使用状態にする
		ItemManager.instance.UnuseItemObject(ID);
	}

}
