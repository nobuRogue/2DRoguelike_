/**
 * @file ItemMasterUtility.cs
 * @brief �A�C�e���}�X�^�[�f�[�^���s����
 * @author yao
 * @date 2025/2/4
 */

using System.Collections.Generic;

using static Entity_ItemData;

public class ItemMasterUtility {

	/// <summary>
	/// �A�C�e���}�X�^�[�f�[�^�擾
	/// </summary>
	/// <param name="ID"></param>
	/// <returns></returns>
	public static Param GetItemMaster(int ID) {
		List<Param> itemMasterList = MasterDataManager.itemData[0];
		for (int i = 0, max = itemMasterList.Count; i < max; i++) {
			if (itemMasterList[i].ID != ID) continue;

			return itemMasterList[i];
		}
		return null;
	}

}
