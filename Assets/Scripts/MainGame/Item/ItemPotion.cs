/**
 * @file ItemPotion.cs
 * @brief ��A�C�e���f�[�^
 * @author yao
 * @date 2025/3/4
 */

public class ItemPotion : ItemBase {
	/// <summary>
	/// �J�e�S���擾
	/// </summary>
	/// <returns></returns>
	public override eItemCategory GetCategory() {
		return eItemCategory.Potion;
	}
}
