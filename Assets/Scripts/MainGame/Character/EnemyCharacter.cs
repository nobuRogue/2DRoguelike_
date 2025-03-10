/**
 * @file EnemyCharacter.cs
 * @brief �G�l�~�[�L�����N�^�[
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
	/// �s�����v�l����A�ړ��̓����������s��
	/// </summary>
	public override void ThinkAction() {
		_currentAI.ThinkAction();
	}

	/// <summary>
	/// �\��s���̎��s
	/// </summary>
	/// <returns></returns>
	public override async UniTask ExecuteScheduleAction() {
		await _currentAI.ExecuteScheduleAction();
		ResetScheduleAction();
	}

	/// <summary>
	/// �\��s���̃N���A
	/// </summary>
	public override void ResetScheduleAction() {
		_currentAI.ResetScheduleAction();
	}

}
