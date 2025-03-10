/**
 * @file EnemyAIBase.cs
 * @brief �G�l�~�[AI�̊��
 * @author yao
 * @date 2025/1/21
 */

using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static ActionRangeManager;
using static ActionMasterUtility;

public abstract class EnemyAIBase {
	protected static System.Action<MoveAction> _AddMove = null;

	protected System.Func<CharacterBase> _GetSourceCharacter = null;

	/// <summary>
	/// �\��s��ID
	/// </summary>
	protected int _scheduleActionID = -1;

	public static void SetAddMoveCallback(System.Action<MoveAction> setProcess) {
		_AddMove = setProcess;
	}

	public EnemyAIBase(System.Func<CharacterBase> SetGetSourceProcess) {
		_GetSourceCharacter = SetGetSourceProcess;
	}

	public abstract void ThinkAction();

	/// <summary>
	/// �\��s���̎��s
	/// </summary>
	/// <returns></returns>
	public async UniTask ExecuteScheduleAction() {
		if (_scheduleActionID < 0) return;
		// �\��s���̎g�p�۔���
		var actionMaster = GetActionMaster(_scheduleActionID);
		if (actionMaster == null) return;

		ActionRangeBase range = GetRange(actionMaster.rangeType);
		CharacterBase sourceCharacter = _GetSourceCharacter();
		eDirectionEight dir = eDirectionEight.Invalid;
		if (!range.CanUse(sourceCharacter, ref dir)) return;
		// �\��s���̎��s
		if (dir != eDirectionEight.Invalid) sourceCharacter.SetDirection(dir);

		await ActionManager.ExecuteAction(sourceCharacter, _scheduleActionID);
	}

	/// <summary>
	/// �\��s���̐ݒ�
	/// </summary>
	/// <param name="setID"></param>
	protected void SetScheduleAction(int setID) {
		_scheduleActionID = setID;
	}

	/// <summary>
	/// �\��s���̃N���A
	/// </summary>
	public void ResetScheduleAction() {
		_scheduleActionID = -1;
	}

}
