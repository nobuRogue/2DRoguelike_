/**
 * @file TurnProcessor.cs
 * @brief �^�[�����s����
 * @author yao
 * @date 2025/1/21
 */
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

using static CommonModule;
using static GameConst;

public class TurnProcessor {
	// �v���C���[�̓��͎�t����
	AcceptPlayerInput _acceptPlayerInput = null;
	// �^�[���p���t���O
	private bool _isContinueTurn = false;

	private List<MoveAction> _moveActionList = null;
	private List<UniTask> _moveTaskList = null;

	private System.Action<eFloorEndReason> _EndFloor = null;
	private System.Action<eDungeonEndReason> _EndDungeon = null;

	public void Initialize(
		System.Action<eFloorEndReason> SetEndFloor,
		System.Action<eDungeonEndReason> SetEndDungeon) {
		_acceptPlayerInput = new AcceptPlayerInput();
		_acceptPlayerInput.Initialize(moveAction => _moveActionList.Add(moveAction));
		EnemyAIBase.SetAddMoveCallback(moveAction => _moveActionList.Add(moveAction));

		_moveActionList = new List<MoveAction>(FLOOR_ENEMY_MAX + 1);
		_moveTaskList = new List<UniTask>(FLOOR_ENEMY_MAX + 1);

		_EndFloor = SetEndFloor;
		_EndDungeon = SetEndDungeon;
		// �t���A�I���A�_���W�����I��������K�v�ȃN���X�ɓn��
		MoveAction.SetEndCallback(EndFloor, EndDungeon);
		ActionEffectBase.SetEndCallback(EndDungeon);
		CharacterUtility.SetEndDungeonCallback(EndDungeon);
	}

	public async UniTask Execute() {
		_isContinueTurn = true;
		// �v���C���[�̓��͎�t
		await AcceptPlayerInput();
		// �S�L�����N�^�[�̈ړ�
		await MoveAllCharacter();
		// �S�G�l�~�[�̍s��
		await ActionAllEnemy();
		// �^�[���I��������
		await OnEndTurn();
	}

	/// <summary>
	/// �S�L�����N�^�[�̈ړ�
	/// </summary>
	/// <returns></returns>
	private async UniTask MoveAllCharacter() {
		// �S�ẴG�l�~�[�ɍs�����v�l�A�ړ��̓���������������
		CharacterUtility.ExecuteAllCharacter(character => character?.ThinkAction());
		// �����ڂ̈ړ�����
		for (int i = 0, max = _moveActionList.Count; i < max; i++) {
			_moveTaskList.Add(_moveActionList[i].ProcessObject(MOVE_DURATION));
		}
		await WaitTask(_moveTaskList);
		_moveTaskList.Clear();
		_moveActionList.Clear();
	}

	/// <summary>
	/// �S�G�l�~�[�̍s��
	/// </summary>
	/// <returns></returns>
	private async UniTask ActionAllEnemy() {
		// �s��������G�l�~�[�����Ԃɍs��������
		await CharacterUtility.ExecuteTaskAllCharacter(ExecuteScheduleAction);
	}

	/// <summary>
	/// �S�L�����N�^�[�̃^�[���I�����������s��
	/// </summary>
	/// <returns></returns>
	private async UniTask OnEndTurn() {
		await CharacterUtility.ExecuteTaskAllCharacter(OnEndTurnCharacter);
	}

	/// <summary>
	/// �L�����N�^�[�̃^�[���I�����������s��
	/// </summary>
	/// <param name="character"></param>
	/// <returns></returns>
	private async UniTask OnEndTurnCharacter(CharacterBase character) {
		await character.OnEndTurn();
	}

	/// <summary>
	/// �L�����N�^�[���\�肵�Ă���s�������s������
	/// </summary>
	/// <param name="character"></param>
	/// <returns></returns>
	private async UniTask ExecuteScheduleAction(CharacterBase character) {
		if (_isContinueTurn) {
			// �\�肳��Ă���s�����s��
			await character.ExecuteScheduleAction();
		} else {
			// �^�[���I���Ȃ�\��s�����N���A����
			character.ResetScheduleAction();
		}
	}

	/// <summary>
	/// �v���C���[�̓��͎�t
	/// </summary>
	/// <returns></returns>
	private async UniTask AcceptPlayerInput() {
		// �p���ړ������邩�m�F
		if (_acceptPlayerInput.AcceptMove()) return;
		// �S�ẴL�����N�^�[��ҋ@�A�j���[�V�����ɂ���
		CharacterUtility.ExecuteAllCharacter(character => character.SetAnimation(eCharacterAnimation.Wait));
		await _acceptPlayerInput.AcceptInput();
	}

	private void EndTurn() {
		_isContinueTurn = false;
	}

	private void EndFloor(eFloorEndReason endReason) {
		_EndFloor(endReason);
		EndTurn();
	}

	private void EndDungeon(eDungeonEndReason endReason) {
		_EndDungeon(endReason);
		EndFloor(endReason.GetFloorEndReaosn());
	}

}
