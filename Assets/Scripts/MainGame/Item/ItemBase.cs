/**
 * @file ItemBase.cs
 * @brief �A�C�e���f�[�^�̊��
 * @author yao
 * @date 2025/3/4
 */


public abstract class ItemBase {
	/// <summary>
	/// ID�ɕR�Â����I�u�W�F�N�g���擾
	/// </summary>
	private static System.Func<int, ItemObject> _GetObject = null;

	public static void SetGetObjectCallback( System.Func<int, ItemObject> setProcess ) {
		_GetObject = setProcess;
	}

	// ���j�[�NID
	public int ID { get; private set; } = -1;
	// �}�X�^�[�f�[�^ID
	public int masterID { get; private set; } = -1;
	// �A�C�e����ID
	private int _nameID = -1;
	public int positionX { get; private set; } = -1;
	public int positionY { get; private set; } = -1;
	public int possessCharacterID { get; private set; } = -1;
	public abstract eItemCategory GetCategory();

	public void Setup( int setID, int setMasterID, MapSquareData square ) {
		ID = setID;
		masterID = setMasterID;
		SetSquare( square );
		// �}�X�^�[�f�[�^�擾
		var itemMaster = ItemMasterUtility.GetItemMaster( masterID );
		_nameID = itemMaster.nameID;
		_GetObject( ID ).Setup( ID, itemMaster );
	}

	public void Teardown() {
		RemoveCurrentPlace();
		ID = -1;
		masterID = -1;
		_nameID = -1;
	}

	/// <summary>
	/// �}�X�ɃA�C�e����u��
	/// </summary>
	/// <param name="square"></param>
	public void SetSquare( MapSquareData square ) {
		if (square == null) return;
		// ���݂̏ꏊ�����菜��
		RemoveCurrentPlace();
		positionX = square.positionX;
		positionY = square.positionY;
		square.SetItem( ID );
		// �I�u�W�F�N�g�̏���
		ItemObject itemObject = _GetObject( ID );
		if (itemObject == null) {
			ItemManager.instance.UseItemObject( ID );
		} else {
			_GetObject( ID ).SetSquare( square );
		}
	}

	/// <summary>
	/// �L�����N�^�[�̎莝���ɒǉ�
	/// </summary>
	/// <param name="character"></param>
	public void AddCharcter( CharacterBase character ) {
		if (character == null) return;
		// ���݂̏ꏊ�����菜��
		RemoveCurrentPlace();
		character.AddItem( ID );
		possessCharacterID = character.ID;
	}

	/// <summary>
	/// �A�C�e�������݂̏ꏊ�����菜��
	/// </summary>
	public void RemoveCurrentPlace() {
		if (positionX >= 0 && positionY >= 0) {
			// �������A�C�e��
			MapSquareUtility.GetSquare( positionX, positionY )?.RemoveItem();
			positionX = -1;
			positionY = -1;
			// �I�u�W�F�N�g�̏���
			_GetObject( ID ).UnuseSelf();
		} else if (possessCharacterID >= 0) {
			// �L�����̎莝�������菜��
			CharacterUtility.GetCharacter( possessCharacterID ).RemoveIDItem( ID );
			possessCharacterID = -1;
		}
	}

	/// <summary>
	/// �A�C�e�����擾
	/// </summary>
	/// <returns></returns>
	public string GetItemName() {
		return _nameID.ToMessage();
	}

}
