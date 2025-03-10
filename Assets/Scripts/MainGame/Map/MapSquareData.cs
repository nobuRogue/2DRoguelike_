/**
 * @file MapSquareData.cs
 * @brief 1�}�X�̏��
 * @author yao
 * @date 2025/1/9
 */

using UnityEngine;

public class MapSquareData {
	// �}�X�I�u�W�F�N�g���擾����R�[���o�b�N
	private static System.Func<int, MapSquareObject> _GetObject = null;
	public static void SetGetObjectCallback(System.Func<int, MapSquareObject> setCallback) {
		_GetObject = setCallback;
	}

	public int ID { get; private set; } = -1;
	public int positionX { get; private set; } = -1;
	public int positionY { get; private set; } = -1;
	public eTerrain terrain { get; private set; } = eTerrain.Invalid;
	public int roomID { get; private set; } = -1;
	/// <summary>
	/// �}�X�ɂ���L�����N�^�[��ID
	/// </summary>
	public int characterID { get; private set; } = -1;
	/// <summary>
	/// �}�X�ɃL�����N�^�[�����݂��邩
	/// </summary>
	public bool existCharacter { get { return characterID >= 0; } }
	/// <summary>
	/// �}�X�ɂ���A�C�e����ID
	/// </summary>
	public int itemID { get; private set; } = -1;

	public void Setup(int setID, int setX, int setY) {
		ID = setID;
		positionX = setX;
		positionY = setY;
		_GetObject(ID)?.Setup(positionX, positionY);
	}

	public void SetTerrain(eTerrain setTerrain, int spriteIndex = -1) {
		terrain = setTerrain;
		_GetObject(ID)?.SetTerrain(terrain, spriteIndex);
	}

	public void SetRoomID(int setID) {
		roomID = setID;
	}

	public Transform GetCharacterRoot() {
		return _GetObject(ID)?.GetCharacterRoot();
	}

	public void SetCharacter(int setCharacterID) {
		characterID = setCharacterID;
	}

	public void RemoveCharacter() {
		characterID = -1;
	}

	public void SetItem(int setItemID) {
		itemID = setItemID;
	}

	public void RemoveItem() {
		itemID = -1;
	}

	public void ShowMark(Color color) {
		_GetObject(ID).ShowMark(color);
	}

	public void HideMark() {
		_GetObject(ID).HideMark();
	}

}
