/**
 * @file ItemFood.cs
 * @brief �H�ו��A�C�e���f�[�^
 * @author yao
 * @date 2025/3/6
 */
public class ItemFood : ItemBase {
	/// <summary>
	/// �J�e�S���擾
	/// </summary>
	/// <returns></returns>
	public override eItemCategory GetCategory() {
		return eItemCategory.Food;
	}
}
