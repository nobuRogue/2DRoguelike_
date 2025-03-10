/**
 * @file ItemUtility.cs
 * @brief �A�C�e���֘A���s����
 * @author yao
 * @date 2025/3/4
 */

public class ItemUtility {
	/// <summary>
	/// �S�Ă̎g�p���A�C�e���Ɏw��̏��������s
	/// </summary>
	/// <param name="action"></param>
	public static void ExecuteAllItem( System.Action<ItemBase> action ) {
		ItemManager.instance.ExecuteAllItem( action );
	}

	/// <summary>
	/// �������A�C�e������
	/// </summary>
	/// <param name="masterID"></param>
	/// <param name="createSquare"></param>
	/// <returns></returns>
	public static ItemBase CreateFloorItem( int masterID, MapSquareData createSquare ) {
		return ItemManager.instance.UseFloorItem( masterID, createSquare );
	}

	/// <summary>
	/// �A�C�e���𖢎g�p��Ԃɂ���
	/// </summary>
	/// <param name="ID"></param>
	public static void UnuseItem( int ID ) {
		ItemManager.instance.UnuseItemData( ID );
	}

	/// <summary>
	/// �A�C�e���f�[�^�擾
	/// </summary>
	/// <param name="ID"></param>
	/// <returns></returns>
	public static ItemBase GetItemData( int ID ) {
		return ItemManager.instance.GetItemData( ID );
	}

	public static Entity_ItemData.Param GetMasterDataFromID( int itemID ) {
		var itemData = GetItemData( itemID );
		if (itemData == null) return null;

		return ItemMasterUtility.GetItemMaster( itemData.masterID );
	}

}
