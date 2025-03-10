/**
 * @file EnemyCharacter.cs
 * @brief エネミーキャラクター
 * @author yao
 * @date 2025/1/21
 */

using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : CharacterBase {

	private EnemyAIBase _currentAI = null;

	public override void Setup(int setID, MapSquareData squareData, int masterID) {
		base.Setup(setID, squareData, masterID);
		_currentAI = new EnemyAI00_Normal(() => this);
	}

	public override bool IsPlayer() {
		return false;
	}

	/// <summary>
	/// 行動を思考する、移動の内部処理を行う
	/// </summary>
	public override void ThinkAction() {
		_currentAI.ThinkAction();
	}

	/// <summary>
	/// 予定行動の実行
	/// </summary>
	/// <returns></returns>
	public override async UniTask ExecuteScheduleAction() {
		await _currentAI.ExecuteScheduleAction();
		ResetScheduleAction();
	}

	/// <summary>
	/// 予定行動のクリア
	/// </summary>
	public override void ResetScheduleAction() {
		_currentAI.ResetScheduleAction();
	}

}
