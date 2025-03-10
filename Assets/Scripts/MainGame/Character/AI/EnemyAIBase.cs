/**
 * @file EnemyAIBase.cs
 * @brief エネミーAIの基底
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
	/// 予定行動ID
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
	/// 予定行動の実行
	/// </summary>
	/// <returns></returns>
	public async UniTask ExecuteScheduleAction() {
		if (_scheduleActionID < 0) return;
		// 予定行動の使用可否判定
		var actionMaster = GetActionMaster(_scheduleActionID);
		if (actionMaster == null) return;

		ActionRangeBase range = GetRange(actionMaster.rangeType);
		CharacterBase sourceCharacter = _GetSourceCharacter();
		eDirectionEight dir = eDirectionEight.Invalid;
		if (!range.CanUse(sourceCharacter, ref dir)) return;
		// 予定行動の実行
		if (dir != eDirectionEight.Invalid) sourceCharacter.SetDirection(dir);

		await ActionManager.ExecuteAction(sourceCharacter, _scheduleActionID);
	}

	/// <summary>
	/// 予定行動の設定
	/// </summary>
	/// <param name="setID"></param>
	protected void SetScheduleAction(int setID) {
		_scheduleActionID = setID;
	}

	/// <summary>
	/// 予定行動のクリア
	/// </summary>
	public void ResetScheduleAction() {
		_scheduleActionID = -1;
	}

}
