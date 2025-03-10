/**
 * @file ItemBase.cs
 * @brief アイテムデータの基底
 * @author yao
 * @date 2025/3/4
 */


public abstract class ItemBase {
	/// <summary>
	/// IDに紐づいたオブジェクトを取得
	/// </summary>
	private static System.Func<int, ItemObject> _GetObject = null;

	public static void SetGetObjectCallback(System.Func<int, ItemObject> setProcess) {
		_GetObject = setProcess;
	}

	// ユニークID
	public int ID { get; private set; } = -1;
	// マスターデータID
	private int _masterID = -1;
	// アイテム名ID
	private int _nameID = -1;
	public int positionX { get; private set; } = -1;
	public int positionY { get; private set; } = -1;
	public int possessCharacterID { get; private set; } = -1;
	public abstract eItemCategory GetCategory();

	public void Setup(int setID, int setMasterID, MapSquareData square) {
		ID = setID;
		_masterID = setMasterID;
		SetSquare(square);
		// マスターデータ取得
		var itemMaster = ItemMasterUtility.GetItemMaster(_masterID);
		_nameID = itemMaster.nameID;
		_GetObject(ID).Setup(ID, itemMaster);
	}

	public void Teardown() {
		RemoveCurrentPlace();
		ID = -1;
		_masterID = -1;
		_nameID = -1;
	}

	/// <summary>
	/// マスにアイテムを置く
	/// </summary>
	/// <param name="square"></param>
	public void SetSquare(MapSquareData square) {
		if (square == null) return;
		// 現在の場所から取り除く
		RemoveCurrentPlace();
		positionX = square.positionX;
		positionY = square.positionY;
		square.SetItem(ID);
		// オブジェクトの処理
		ItemObject itemObject = _GetObject(ID);
		if (itemObject == null) {
			ItemManager.instance.UseItemObject(ID);
		} else {
			_GetObject(ID).SetSquare(square);
		}
	}

	/// <summary>
	/// キャラクターの手持ちに追加
	/// </summary>
	/// <param name="character"></param>
	public void AddCharcter(CharacterBase character) {
		if (character == null) return;
		// 現在の場所から取り除く
		RemoveCurrentPlace();
		character.AddItem(ID);
		possessCharacterID = character.ID;
	}

	/// <summary>
	/// アイテムを現在の場所から取り除く
	/// </summary>
	public void RemoveCurrentPlace() {
		if (positionX >= 0 && positionY >= 0) {
			// 床落ちアイテム
			MapSquareUtility.GetSquare(positionX, positionY)?.RemoveItem();
			positionX = -1;
			positionY = -1;
			// オブジェクトの処理
			_GetObject(ID).UnuseSelf();
		} else if (possessCharacterID >= 0) {
			// キャラの手持ちから取り除く
			CharacterUtility.GetCharacter(possessCharacterID).RemoveIDItem(ID);
			possessCharacterID = -1;
		}
	}

	/// <summary>
	/// アイテム名取得
	/// </summary>
	/// <returns></returns>
	public string GetItemName() {
		return _nameID.ToMessage();
	}

}
