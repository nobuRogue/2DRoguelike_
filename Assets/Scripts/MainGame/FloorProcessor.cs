/**
 * @file FloorProcessor.cs
 * @brief フロア実行処理
 * @author yao
 * @date 2025/1/21
 */


using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;

using static CommonModule;
using static GameConst;

public class FloorProcessor {
	private TurnProcessor _turnProcessor = null;

	private eFloorEndReason _endReason = eFloorEndReason.Invalid;

	public void Initialize(System.Action<eDungeonEndReason> SetEndDungeon) {
		_turnProcessor = new TurnProcessor();
		_turnProcessor.Initialize(EndFloor, SetEndDungeon);
	}

	public async UniTask<eFloorEndReason> Execute() {
		// フロアの生成
		await SetupFloor();
		while (_endReason == eFloorEndReason.Invalid) {
			await _turnProcessor.Execute();
		}
		// フロアの破棄
		await TeardownFloor();
		OnEndFloor();
		return _endReason;
	}

	/// <summary>
	/// フロア準備
	/// </summary>
	private async UniTask SetupFloor() {
		// 現在のフロアマスターデータからマップチップインデックス取得
		Entity_FloorData.Param floorMaster = FloorMasterUtility.GetFloorMaster(UserDataHolder.currentData.floorCount);
		int floorTypeIndex = floorMaster == null ? 0 : floorMaster.spriteIndex;
		TerrainSpriteAssignor.SetFloorSpriteTypeIndex(floorTypeIndex);
		// フロアの生成
		MapCreater.CreateMap();
		// プレイヤーの配置
		SetPlayer();
		// 部屋マスを集約
		var roomSquareList = new List<MapSquareData>(MAP_SQUARE_HEIGHT_COUNT * MAP_SQUARE_WIDTH_COUNT);
		MapSquareManager.instance.ExecuteAllSquare(square => {
			if (square.existCharacter ||
				square.terrain != eTerrain.Room) return;

			roomSquareList.Add(square);
		});
		// エネミーの配置
		SpawnEnemy(3, roomSquareList);
		// アイテムの配置
		CreateFloorItem(4, roomSquareList);
		_endReason = eFloorEndReason.Invalid;
		await FadeManager.instance.FadeIn();
	}

	/// <summary>
	/// プレイヤーの配置
	/// </summary>
	private void SetPlayer() {
		PlayerCharacter player = CharacterManager.instance.GetPlayer();
		if (player == null) return;
		// ランダムな部屋マスを取得
		RoomData roomData = MapSquareManager.instance.GetRandomRoom();
		if (roomData == null) return;

		List<int> roomSquareList = roomData.squareIDList;
		int playerSquareID = roomSquareList[Random.Range(0, roomSquareList.Count)];
		MapSquareData playerSquare = MapSquareManager.instance.Get(playerSquareID);
		player.SetSquare(playerSquare);
	}

	/// <summary>
	/// エネミーの生成
	/// </summary>
	/// <param name="spawnCount"></param>
	private void SpawnEnemy(int spawnCount, List<MapSquareData> candidateSquareList) {
		for (int i = 0; i < spawnCount; i++) {
			if (IsEmpty(candidateSquareList)) return;

			MapSquareData enemySquare = candidateSquareList[Random.Range(0, candidateSquareList.Count)];
			CharacterManager.instance.UseEnemy(enemySquare, 1);
			candidateSquareList.Remove(enemySquare);
		}
	}

	/// <summary>
	/// 床落ちアイテムの生成
	/// </summary>
	/// <param name="createCount"></param>
	/// <param name="candidateSquareList"></param>
	private void CreateFloorItem(int createCount, List<MapSquareData> candidateSquareList) {
		for (int i = 0; i < createCount; i++) {
			if (IsEmpty(candidateSquareList)) return;
			// 候補マスの内ランダムな一つにアイテム生成
			int randomIndex = Random.Range(0, candidateSquareList.Count);
			MapSquareData itemSquare = candidateSquareList[randomIndex];
			ItemUtility.CreateFloorItem(0, itemSquare);
			candidateSquareList.Remove(itemSquare);
		}
		for (int i = 0; i < createCount; i++) {
			if (IsEmpty(candidateSquareList)) return;
			// 候補マスの内ランダムな一つにアイテム生成
			int randomIndex = Random.Range(0, candidateSquareList.Count);
			MapSquareData itemSquare = candidateSquareList[randomIndex];
			ItemUtility.CreateFloorItem(1, itemSquare);
			candidateSquareList.Remove(itemSquare);
		}
		for (int i = 0; i < createCount; i++) {
			if (IsEmpty(candidateSquareList)) return;
			// 候補マスの内ランダムな一つにアイテム生成
			int randomIndex = Random.Range(0, candidateSquareList.Count);
			MapSquareData itemSquare = candidateSquareList[randomIndex];
			ItemUtility.CreateFloorItem(2, itemSquare);
			candidateSquareList.Remove(itemSquare);
		}
	}

	private async UniTask TeardownFloor() {
		await FadeManager.instance.FadeOut();
		// エネミーの全削除
		CharacterUtility.ExecuteAllCharacter(character => {
			if (character.IsPlayer()) return;

			CharacterManager.instance.UnuseEnemy(character as EnemyCharacter);
		});
		// 床落ちアイテムの全削除
		ItemUtility.ExecuteAllItem(itemData => {
			if (itemData.positionX < 0 || itemData.positionY < 0) return;

			ItemUtility.UnuseItem(itemData.ID);
		});
		// キャラクターのフロア終了時処理
		CharacterUtility.ExecuteAllCharacter(character => character.OnEndFloor());
	}

	/// <summary>
	/// フロアを終了させる
	/// </summary
	/// <param name="endReason"></param>
	private void EndFloor(eFloorEndReason endReason) {
		_endReason = endReason;
	}

	/// <summary>
	/// フロア終了時処理
	/// </summary>
	private void OnEndFloor() {
		switch (_endReason) {
			case eFloorEndReason.Dead:
			break;
			case eFloorEndReason.Stair:
			UserData userData = UserDataHolder.currentData;
			userData.SetFloorCount(userData.floorCount + 1);
			break;
		}
	}

}
