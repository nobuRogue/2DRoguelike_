/**
 * @file CharacterBase.cs
 * @brief �L�����N�^�[�̊��
 * @author yao
 * @date 2025/1/21
 */

using Cysharp.Threading.Tasks;
using UnityEngine;

using static CommonModule;

public abstract class CharacterBase {
	protected static System.Func<int, CharacterObject> _GetObject = null;

	public static void SetGetObjectCallback(System.Func<int, CharacterObject> setCallback) {
		_GetObject = setCallback;
	}

	public int ID { get; protected set; } = -1;
	private int _masterID = -1;
	public int positionX { get; protected set; } = -1;
	public int positionY { get; protected set; } = -1;
	/// <summary>
	/// �L�����̌���
	/// </summary>
	public eDirectionEight direction { get; protected set; } = eDirectionEight.Invalid;
	public int maxHP { get; protected set; } = -1;
	public int HP { get; private set; } = -1;
	public int attack { get; private set; } = -1;
	public int defense { get; private set; } = -1;

	private static readonly int _POSSESS_ITEM_MAX = 10;
	/// <summary>
	/// �����A�C�e����ID���X�g
	/// </summary>
	public int[] possessItemList { get; private set; } = null;

	public virtual void Setup(int setID, MapSquareData squareData, int masterID) {
		ID = setID;
		SetSquare(squareData);
		_masterID = masterID;
		// �X�e�[�^�X���l�̏�����
		ResetStatus();
		_GetObject(ID).Setup(CharacterMasterUtility.GetCharacterMaster(_masterID));
		SetDirection(eDirectionEight.Down);
		// �����A�C�e���̏�����
		possessItemList = new int[_POSSESS_ITEM_MAX];
		for (int i = 0; i < _POSSESS_ITEM_MAX; i++) {
			possessItemList[i] = -1;
		}
	}

	/// <summary>
	/// �X�e�[�^�X������
	/// </summary>
	public virtual void ResetStatus() {
		var characterMaster = CharacterMasterUtility.GetCharacterMaster(_masterID);
		if (characterMaster == null) return;

		SetMaxHP(characterMaster.HP);
		SetHP(characterMaster.HP);
		SetAttack(characterMaster.Attack);
		SetDefense(characterMaster.Defense);
	}

	public void Teardown() {
		_GetObject(ID).Teardown();
		ID = -1;
	}

	/// <summary>
	/// �L�����̌����ݒ�
	/// </summary>
	/// <param name="dir"></param>
	public void SetDirection(eDirectionEight dir) {
		if (direction == dir) return;

		direction = dir;
		_GetObject(ID).SetDirection(direction);
	}

	public virtual void SetMaxHP(int setValue) {
		maxHP = setValue;
	}

	public bool IsDead() {
		return HP <= 0;
	}

	public virtual void SetHP(int setValue) {
		HP = Mathf.Clamp(setValue, 0, maxHP);
	}

	public void AddHP(int addValue) {
		SetHP(HP + addValue);
	}

	public void RemoveHP(int removeValue) {
		SetHP(HP - removeValue);
	}

	public virtual void SetAttack(int setValue) {
		attack = setValue;
	}

	public virtual void SetDefense(int setValue) {
		defense = setValue;
	}

	/// <summary>
	/// �����ڂƏ��A�����̕ύX
	/// </summary>
	/// <param name="squareData"></param>
	public void SetSquare(MapSquareData squareData) {
		SetSquareData(squareData);
		SetPosition(squareData.GetCharacterRoot().position);
	}

	/// <summary>
	/// ���݂̂̈ړ�
	/// </summary>
	/// <param name="squareData"></param>
	public virtual void SetSquareData(MapSquareData squareData) {
		MapSquareData prevSquare = MapSquareManager.instance.Get(positionX, positionY);
		if (prevSquare != null) prevSquare.RemoveCharacter();

		positionX = squareData.positionX;
		positionY = squareData.positionY;
		squareData.SetCharacter(ID);
	}

	/// <summary>
	/// �����ڂ̈ړ�
	/// </summary>
	/// <param name="position"></param>
	public virtual void SetPosition(Vector3 position) {
		_GetObject(ID).SetPosition(position);
	}

	public abstract bool IsPlayer();

	/// <summary>
	/// �s���̎v�l
	/// </summary>
	public virtual void ThinkAction() {

	}

	/// <summary>
	/// �\��s���̎��s
	/// </summary>
	/// <returns></returns>
	public virtual async UniTask ExecuteScheduleAction() {
		await UniTask.CompletedTask;
	}

	/// <summary>
	/// �\��s���̃N���A
	/// </summary>
	public virtual void ResetScheduleAction() {

	}

	/// <summary>
	/// �^�[���I��������
	/// </summary>
	/// <returns></returns>
	public virtual async UniTask OnEndTurn() {
		await UniTask.CompletedTask;
	}

	/// <summary>
	/// �t���A�I��������
	/// </summary>
	public virtual void OnEndFloor() {

	}

	/// <summary>
	/// �A�j���[�V�����Đ�
	/// </summary>
	/// <param name="setAnim"></param>
	public void SetAnimation(eCharacterAnimation setAnim) {
		_GetObject(ID).SetAnimation(setAnim);
	}

	public eCharacterAnimation GetCurrentAnimation() {
		return _GetObject(ID).currentAnim;
	}

	/// <summary>
	/// �A�C�e����ǉ��o���邩�ۂ�
	/// </summary>
	/// <returns></returns>
	public bool CanAddItem() {
		for (int i = 0, max = possessItemList.Length; i < max; i++) {
			if (possessItemList[i] < 0) return true;

		}
		return false;
	}

	/// <summary>
	/// �A�C�e���̒ǉ�
	/// </summary>
	/// <param name="addItemID"></param>
	public void AddItem(int addItemID) {
		for (int i = 0, max = possessItemList.Length; i < max; i++) {
			if (possessItemList[i] >= 0) continue;

			possessItemList[i] = addItemID;
			break;
		}
	}

	/// <summary>
	/// ID�w��̏����A�C�e������
	/// </summary>
	/// <param name="removeItemID"></param>
	public void RemoveIDItem(int removeItemID) {
		bool doneRemove = false;
		for (int i = 0, max = possessItemList.Length; i < max; i++) {
			if (!doneRemove) doneRemove = possessItemList[i] == removeItemID;

			if (!doneRemove) continue;
			// �C���f�N�X��������炷
			if (IsEnableIndex(possessItemList, i + 1)) {
				possessItemList[i] = possessItemList[i + 1];
			} else {
				possessItemList[i] = -1;
			}
		}
	}

	/// <summary>
	/// �C���f�N�X�w��̏����A�C�e������
	/// </summary>
	/// <param name="removeIndex"></param>
	public void RemoveIndexItem(int removeIndex) {
		if (!IsEnableIndex(possessItemList, removeIndex)) return;

		for (int i = removeIndex, max = possessItemList.Length; i < max; i++) {
			// �C���f�N�X��������炷
			if (IsEnableIndex(possessItemList, i + 1)) {
				possessItemList[i] = possessItemList[i + 1];
			} else {
				possessItemList[i] = -1;
			}
		}
	}

}
