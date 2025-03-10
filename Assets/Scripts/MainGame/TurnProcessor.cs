/**
 * @file TurnProcessor.cs
 * @brief ターン実行処理
 * @author yao
 * @date 2025/1/21
 */
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

using static CommonModule;
using static GameConst;

public class TurnProcessor {
	// プレイヤーの入力受付処理
	AcceptPlayerInput _acceptPlayerInput = null;
	// ターン継続フラグ
	private bool _isContinueTurn = false;

	private List<MoveAction> _moveActionList = null;
	private List<UniTask> _moveTaskList = null;

	private System.Action<eFloorEndReason> _EndFloor = null;
	private System.Action<eDungeonEndReason> _EndDungeon = null;

	public void Initialize(
		System.Action<eFloorEndReason> SetEndFloor,
		System.Action<eDungeonEndReason> SetEndDungeon) {
		_acceptPlayerInput = new AcceptPlayerInput();
		_acceptPlayerInput.SetAddMoveActionCallback(moveAction => _moveActionList.Add(moveAction));
		EnemyAIBase.SetAddMoveCallback(moveAction => _moveActionList.Add(moveAction));

		_moveActionList = new List<MoveAction>(FLOOR_ENEMY_MAX + 1);
		_moveTaskList = new List<UniTask>(FLOOR_ENEMY_MAX + 1);

		_EndFloor = SetEndFloor;
		_EndDungeon = SetEndDungeon;
		// フロア終了、ダンジョン終了処理を必要なクラスに渡す
		MoveAction.SetEndCallback(EndFloor, EndDungeon);
		ActionEffectBase.SetEndCallback(EndDungeon);
		CharacterUtility.SetEndDungeonCallback(EndDungeon);
	}

	public async UniTask Execute() {
		_isContinueTurn = true;
		// プレイヤーの入力受付
		await AcceptPlayerInput();
		// 全キャラクターの移動
		await MoveAllCharacter();
		// 全エネミーの行動
		await ActionAllEnemy();
		// ターン終了時処理
		await OnEndTurn();
	}

	/// <summary>
	/// 全キャラクターの移動
	/// </summary>
	/// <returns></returns>
	private async UniTask MoveAllCharacter() {
		// 全てのエネミーに行動を思考、移動の内部処理をさせる
		CharacterUtility.ExecuteAllCharacter(character => character?.ThinkAction());
		// 見た目の移動処理
		for (int i = 0, max = _moveActionList.Count; i < max; i++) {
			_moveTaskList.Add(_moveActionList[i].ProcessObject(MOVE_DURATION));
		}
		await WaitTask(_moveTaskList);
		_moveTaskList.Clear();
		_moveActionList.Clear();
	}

	/// <summary>
	/// 全エネミーの行動
	/// </summary>
	/// <returns></returns>
	private async UniTask ActionAllEnemy() {
		// 行動をするエネミーを順番に行動させる
		await CharacterUtility.ExecuteTaskAllCharacter(ExecuteScheduleAction);
	}

	/// <summary>
	/// 全キャラクターのターン終了時処理を行う
	/// </summary>
	/// <returns></returns>
	private async UniTask OnEndTurn() {
		await CharacterUtility.ExecuteTaskAllCharacter(OnEndTurnCharacter);
	}

	/// <summary>
	/// キャラクターのターン終了時処理を行う
	/// </summary>
	/// <param name="character"></param>
	/// <returns></returns>
	private async UniTask OnEndTurnCharacter(CharacterBase character) {
		await character.OnEndTurn();
	}

	/// <summary>
	/// キャラクターが予定している行動を実行させる
	/// </summary>
	/// <param name="character"></param>
	/// <returns></returns>
	private async UniTask ExecuteScheduleAction(CharacterBase character) {
		if (_isContinueTurn) {
			// 予定されている行動を行う
			await character.ExecuteScheduleAction();
		} else {
			// ターン終了なら予定行動をクリアする
			character.ResetScheduleAction();
		}
	}

	/// <summary>
	/// プレイヤーの入力受付
	/// </summary>
	/// <returns></returns>
	private async UniTask AcceptPlayerInput() {
		// 継続移動があるか確認
		if (_acceptPlayerInput.AcceptMove()) return;
		// 全てのキャラクターを待機アニメーションにする
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
