/**
 * @file MoveAction.cs
 * @brief 移動アクション
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
	/// 内部的な移動処理
	/// </summary>
	public void ProcessData(CharacterBase moveCharacter, ChebyshevMoveData moveData) {
		_moveCharacterID = moveCharacter.ID;
		_moveData = moveData;

		moveCharacter.SetSquareData(MapSquareManager.instance.Get(moveData.targetSquareID));
	}

	/// <summary>
	/// 見た目上の移動処理
	/// </summary>
	/// <param name="duration">移動にかかる秒数</param>
	/// <returns></returns>
	public async UniTask ProcessObject(float duration) {
		CharacterBase moveCharacter = CharacterManager.instance.Get(_moveCharacterID);
		MapSquareData startSquare = MapSquareManager.instance.Get(_moveData.sourceSquareID);
		Vector3 startPos = startSquare.GetCharacterRoot().position;

		MapSquareData goalSquare = MapSquareManager.instance.Get(_moveData.targetSquareID);
		Vector3 goalPos = goalSquare.GetCharacterRoot().position;
		// 歩行アニメーションの再生
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
		// 移動後の処理
		AfterMoveProcess(moveCharacter, goalSquare);
	}

	/// <summary>
	/// 移動後の処理
	/// </summary>
	/// <param name="moveCharacter"></param>
	/// <param name="goalSquare"></param>
	private void AfterMoveProcess(CharacterBase moveCharacter, MapSquareData goalSquare) {
		// プレイヤーなら階段によるフロア終了判定
		if (!moveCharacter.IsPlayer()) return;
		// マスにアイテムがあるなら拾得処理
		ProcessAddItem(moveCharacter, goalSquare);
		// 階段処理
		ProcessStair(goalSquare);
	}

	/// <summary>
	/// 移動先のアイテム拾得処理
	/// </summary>
	/// <param name="moveCharacter"></param>
	/// <param name="goalSquare"></param>
	private void ProcessAddItem(CharacterBase moveCharacter, MapSquareData goalSquare) {
		// 移動先にアイテムが無ければ終了
		if (goalSquare.itemID < 0) return;
		// キャラクターが拾えなければ終了
		ItemBase addItem = ItemUtility.GetItemData(goalSquare.itemID);
		if (!moveCharacter.CanAddItem()) {
			// 拾えないログを表示
			string cannotLogMessage = string.Format(_CANNOT_ADD_ITEM_LOG_ID.ToMessage(), addItem.GetItemName());
			MenuRogueLog.instance.AddLog(cannotLogMessage);
			return;
		}
		// キャラクターのアイテムに追加
		addItem.AddCharcter(moveCharacter);
		// 拾ったログを表示
		string logMessage = string.Format(_ADD_ITEM_LOG_ID.ToMessage(), addItem.GetItemName());
		MenuRogueLog.instance.AddLog(logMessage);
	}

	/// <summary>
	/// 移動先が階段だった時のフロア移動処理
	/// </summary>
	/// <param name="goalSquare"></param>
	private void ProcessStair(MapSquareData goalSquare) {
		if (goalSquare.terrain != eTerrain.Stair) return;
		// 次のフロアがあるならフロア移動
		var floorMaster = FloorMasterUtility.GetFloorMaster(UserDataHolder.currentData.floorCount + 1);
		if (floorMaster == null) {
			// ゲームクリア
			_EndDungeon(eDungeonEndReason.Clear);
		} else {
			// 次の階層へ移動
			_EndFloor(eFloorEndReason.Stair);
		}
	}

}
