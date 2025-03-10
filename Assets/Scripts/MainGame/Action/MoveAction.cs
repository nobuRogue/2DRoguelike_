/**
 * @file MoveAction.cs
 * @brief �ړ��A�N�V����
 * @author yao
 * @date 2025/1/21
 */

using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class MoveAction {
	private static readonly int _CANNOT_ADD_ITEM_LOG_ID = 1000;
	private static readonly int _ADD_ITEM_LOG_ID = 1001;

	private static Action<eFloorEndReason> _EndFloor = null;
	private static Action<eDungeonEndReason> _EndDungeon = null;


	public static void SetEndCallback(Action<eFloorEndReason> setFloorProcess, Action<eDungeonEndReason> setDungeonProcess) {
		_EndFloor = setFloorProcess;
		_EndDungeon = setDungeonProcess;
	}

	private int _moveCharacterID = -1;
	private ChebyshevMoveData _moveData = null;

	/// <summary>
	/// �����I�Ȉړ�����
	/// </summary>
	public void ProcessData(CharacterBase moveCharacter, ChebyshevMoveData moveData) {
		_moveCharacterID = moveCharacter.ID;
		_moveData = moveData;

		moveCharacter.SetSquareData(MapSquareManager.instance.Get(moveData.targetSquareID));
	}

	/// <summary>
	/// �����ڏ�̈ړ�����
	/// </summary>
	/// <param name="duration">�ړ��ɂ�����b��</param>
	/// <returns></returns>
	public async UniTask ProcessObject(float duration) {
		CharacterBase moveCharacter = CharacterManager.instance.Get(_moveCharacterID);
		MapSquareData startSquare = MapSquareManager.instance.Get(_moveData.sourceSquareID);
		Vector3 startPos = startSquare.GetCharacterRoot().position;

		MapSquareData goalSquare = MapSquareManager.instance.Get(_moveData.targetSquareID);
		Vector3 goalPos = goalSquare.GetCharacterRoot().position;
		// ���s�A�j���[�V�����̍Đ�
		moveCharacter.SetAnimation(eCharacterAnimation.Walk);
		float elapsedTime = 0.0f;
		while (elapsedTime < duration) {
			elapsedTime += Time.deltaTime;
			float t = elapsedTime / duration;
			Vector3 setPos = Vector3.Lerp(startPos, goalPos, t);
			moveCharacter.SetPosition(setPos);
			await UniTask.DelayFrame(1);
		}
		moveCharacter.SetPosition(goalPos);
		_moveCharacterID = -1;
		_moveData = null;
		// �ړ���̏���
		AfterMoveProcess(moveCharacter, goalSquare);
	}

	/// <summary>
	/// �ړ���̏���
	/// </summary>
	/// <param name="moveCharacter"></param>
	/// <param name="goalSquare"></param>
	private void AfterMoveProcess(CharacterBase moveCharacter, MapSquareData goalSquare) {
		// �v���C���[�Ȃ�K�i�ɂ��t���A�I������
		if (!moveCharacter.IsPlayer()) return;
		// �}�X�ɃA�C�e��������Ȃ�E������
		ProcessAddItem(moveCharacter, goalSquare);
		// �K�i����
		ProcessStair(goalSquare);
	}

	/// <summary>
	/// �ړ���̃A�C�e���E������
	/// </summary>
	/// <param name="moveCharacter"></param>
	/// <param name="goalSquare"></param>
	private void ProcessAddItem(CharacterBase moveCharacter, MapSquareData goalSquare) {
		// �ړ���ɃA�C�e����������ΏI��
		if (goalSquare.itemID < 0) return;
		// �L�����N�^�[���E���Ȃ���ΏI��
		ItemBase addItem = ItemUtility.GetItemData(goalSquare.itemID);
		if (!moveCharacter.CanAddItem()) {
			// �E���Ȃ����O��\��
			string cannotLogMessage = string.Format(_CANNOT_ADD_ITEM_LOG_ID.ToMessage(), addItem.GetItemName());
			MenuRogueLog.instance.AddLog(cannotLogMessage);
			return;
		}
		// �L�����N�^�[�̃A�C�e���ɒǉ�
		addItem.AddCharcter(moveCharacter);
		// �E�������O��\��
		string logMessage = string.Format(_ADD_ITEM_LOG_ID.ToMessage(), addItem.GetItemName());
		MenuRogueLog.instance.AddLog(logMessage);
	}

	/// <summary>
	/// �ړ��悪�K�i���������̃t���A�ړ�����
	/// </summary>
	/// <param name="goalSquare"></param>
	private void ProcessStair(MapSquareData goalSquare) {
		if (goalSquare.terrain != eTerrain.Stair) return;
		// ���̃t���A������Ȃ�t���A�ړ�
		var floorMaster = FloorMasterUtility.GetFloorMaster(UserDataHolder.currentData.floorCount + 1);
		if (floorMaster == null) {
			// �Q�[���N���A
			_EndDungeon(eDungeonEndReason.Clear);
		} else {
			// ���̊K�w�ֈړ�
			_EndFloor(eFloorEndReason.Stair);
		}
	}

}
