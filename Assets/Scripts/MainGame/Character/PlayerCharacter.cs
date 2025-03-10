/**
 * @file PlayerCharacter.cs
 * @brief �v���C���[�L�����N�^�[
 * @author yao
 * @date 2025/1/21
 */

using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

using static CommonModule;

public class PlayerCharacter : CharacterBase {

	private PlayerMoveObserver _moveObserver = null;

	private List<int> _moveTrailSquareList = null;
	private readonly int PLAYER_MOVE_TRAIL_COUNT = 3;

	// ���������x
	private const int _DEFAULT_STAMINA = 10000;
	private const int _SHOW_STAMINA_RATIO = 100;
	private const int _TURN_DECREASE_STAMINA = 10;
	// ���݂̖����x
	private int _stamina = 0;

	public override void Setup(int setID, MapSquareData squareData, int masterID) {
		_moveTrailSquareList = new List<int>(PLAYER_MOVE_TRAIL_COUNT);
		base.Setup(setID, squareData, masterID);
	}

	public override void ResetStatus() {
		base.ResetStatus();
		SetStamina(_DEFAULT_STAMINA);
	}

	public override void SetMaxHP(int setValue) {
		base.SetMaxHP(setValue);
		MenuPlayerStatus.instance.SetHP(HP, maxHP);
	}

	public override void SetHP(int setValue) {
		base.SetHP(setValue);
		MenuPlayerStatus.instance.SetHP(HP, maxHP);
	}

	public override void SetAttack(int setValue) {
		base.SetAttack(setValue);
		MenuPlayerStatus.instance.SetAttack(attack);
	}

	public override void SetDefense(int setValue) {
		base.SetDefense(setValue);
		MenuPlayerStatus.instance.SetDefense(defense);
	}

	public void SetStamina(int setValue) {
		_stamina = Mathf.Max(0, setValue);
		// UI�ւ̔��f
		MenuPlayerStatus.instance.SetStamina(GetShowStamina());
	}

	public void RemoveStamina(int removeValue) {
		SetStamina(_stamina - removeValue);
	}

	/// <summary>
	/// �����x��%�\�L�ɕϊ�
	/// </summary>
	/// <returns></returns>
	private int GetShowStamina() {
		return (_stamina + _SHOW_STAMINA_RATIO - 1) / _SHOW_STAMINA_RATIO;
	}

	public void SetMoveObserver(PlayerMoveObserver setObserver) {
		_moveObserver = setObserver;
	}

	public override bool IsPlayer() {
		return true;
	}

	/// <summary>
	/// ���݂̂̈ړ�
	/// </summary>
	/// <param name="squareData"></param>
	public override void SetSquareData(MapSquareData squareData) {
		base.SetSquareData(squareData);
		AddMoveTrail(squareData);
	}

	/// <summary>
	/// �^�[���I��������
	/// </summary>
	/// <returns></returns>
	public override async UniTask OnEndTurn() {
		await base.OnEndTurn();
		if (_stamina <= 0) {
			// HP������
			RemoveHP(1);
			if (IsDead()) await CharacterUtility.DeadCharacter(this);

		} else {
			// �����x������
			RemoveStamina(_TURN_DECREASE_STAMINA);
			if (!IsDead()) AddHP(1);

		}
	}

	/// <summary>
	/// �t���A�I��������
	/// </summary>
	public override void OnEndFloor() {
		base.OnEndFloor();
		// �ړ��O�Ղ��N���A
		ClearMoveTrail();
	}

	/// <summary>
	/// �ړ��O�Ճ}�X���X�g�Ƀ}�X��ǉ�
	/// </summary>
	/// <param name="addSquare"></param>
	private void AddMoveTrail(MapSquareData addSquare) {
		if (_moveTrailSquareList.Exists(trailSquareID => trailSquareID == addSquare.ID)) return;

		while (_moveTrailSquareList.Count >= PLAYER_MOVE_TRAIL_COUNT) {
			MapSquareManager.instance.Get(_moveTrailSquareList[0])?.HideMark();
			_moveTrailSquareList.RemoveAt(0);
		}
		addSquare.ShowMark(Color.blue);
		_moveTrailSquareList.Add(addSquare.ID);
	}

	/// <summary>
	/// �ړ��O�Ճ}�X���N���A
	/// </summary>
	private void ClearMoveTrail() {
		if (IsEmpty(_moveTrailSquareList)) return;

		for (int i = 0, max = _moveTrailSquareList.Count; i < max; i++) {
			MapSquareManager.instance.Get(_moveTrailSquareList[i])?.HideMark();
		}
		_moveTrailSquareList.Clear();
	}

	/// <summary>
	/// �ړ��O�Ճ}�X���X�g�Ɏw��̃}�XID���܂܂�Ă��邩
	/// </summary>
	/// <param name="squareID"></param>
	/// <returns></returns>
	public bool ExistMoveTrail(int squareID) {
		if (IsEmpty(_moveTrailSquareList)) return false;

		return _moveTrailSquareList.Exists(trailSquareID => trailSquareID == squareID);
	}

	/// <summary>
	/// �����ڂ̈ړ�
	/// </summary>
	/// <param name="position"></param>
	public override void SetPosition(Vector3 position) {
		base.SetPosition(position);
		if (_moveObserver != null) _moveObserver.OnPlayerMove(position);

	}


}
