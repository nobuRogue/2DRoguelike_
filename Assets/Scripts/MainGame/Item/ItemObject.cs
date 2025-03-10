/**
 * @file ItemObject.cs
 * @brief �A�C�e���I�u�W�F�N�g
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
		// �J�e�S�����猩���ڂ��擾
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
		// ���g�𖢎g�p��Ԃɂ���
		ItemManager.instance.UnuseItemObject(ID);
	}

}
