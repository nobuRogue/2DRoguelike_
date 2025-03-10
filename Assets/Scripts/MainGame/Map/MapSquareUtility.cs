/**
 * @file MapSquareUtility.cs
 * @brief マス関連実行処理
 * @author yao
 * @date 2025/1/21
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CommonModule;

public class MapSquareUtility {

	/// <summary>
	/// 指定方向のマス取得
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="dir"></param>
	/// <returns></returns>
	public static MapSquareData GetToDirSquare(int x, int y, eDirectionFour dir) {
		ToVectorPos(ref x, ref y, dir);
		return MapSquareManager.instance.Get(x, y);
	}

	/// <summary>
	/// 指定座標マスからの指定方向のマス取得
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="dir"></param>
	/// <returns></returns>
	public static MapSquareData GetToDirSquare(int x, int y, eDirectionEight dir) {
		ToVectorPos(ref x, ref y, dir);
		return MapSquareManager.instance.Get(x, y);
	}

	/// <summary>
	/// 指定マスからの指定方向のマス取得
	/// </summary>
	/// <param name="baseSquare"></param>
	/// <param name="dir"></param>
	/// <returns></returns>
	public static MapSquareData GetToDirSquare(MapSquareData baseSquare, eDirectionEight dir) {
		if (baseSquare == null) return null;

		return GetToDirSquare(baseSquare.positionX, baseSquare.positionY, dir);
	}

	/// <summary>
	/// 移動可否の判定
	/// </summary>
	/// <param name="startX"></param>
	/// <param name="startY"></param>
	/// <param name="moveSquare"></param>
	/// <param name="dir"></param>
	/// <returns></returns>
	public static bool CanMove(int startX, int startY, MapSquareData moveSquare, eDirectionEight dir) {
		return CanMoveTerrain(startX, startY, moveSquare, dir) && !moveSquare.existCharacter;
	}

	public static bool CanMoveTerrain(int startX, int startY, MapSquareData moveSquare, eDirectionEight dir) {
		if (moveSquare == null ||
			moveSquare.terrain == eTerrain.Wall) return false;
		// 斜め移動か否か
		if (!dir.IsSlant()) return true;
		// 斜め移動なら、方向を分割し各方向のマスをチェック
		eDirectionFour[] separateDir = dir.Separate();
		for (int i = 0, max = separateDir.Length; i < max; i++) {
			MapSquareData checkSquare = GetToDirSquare(startX, startY, separateDir[i]);
			if (checkSquare == null ||
				checkSquare.terrain == eTerrain.Wall) return false;

		}
		return true;
	}

	/// <summary>
	/// 攻撃可能なマスか
	/// </summary>
	/// <param name="startX"></param>
	/// <param name="startY"></param>
	/// <param name="attackSquare"></param>
	/// <param name="dir"></param>
	/// <returns></returns>
	public static bool CanAttack(int startX, int startY, MapSquareData attackSquare, eDirectionEight dir) {
		if (attackSquare == null ||
			attackSquare.terrain == eTerrain.Wall) return false;
		// 攻撃方向が斜めではないので攻撃先が壁でない時点で攻撃可能
		if (!dir.IsSlant()) return true;
		// 斜め方向なら、方向を分割し各方向のマスをチェック
		eDirectionFour[] separateDir = dir.Separate();
		for (int i = 0, max = separateDir.Length; i < max; i++) {
			MapSquareData checkSquare = GetToDirSquare(startX, startY, separateDir[i]);
			if (checkSquare == null ||
				checkSquare.terrain == eTerrain.Wall) return false;

		}
		// 斜め方向で分割したマスがどちらも壁ではないので攻撃可能
		return true;
	}

	public static void GetVisibleArea(ref List<int> visibleArea, MapSquareData sourceSquare) {
		InitializeList(ref visibleArea);
		if (sourceSquare == null) return;
		// 周囲8マスを取得
		GetChebyshevAroundSquare(ref visibleArea, sourceSquare);
		visibleArea.Add(sourceSquare.ID);
		// 周囲8マスか自身のマスに部屋があれば取得
		List<int> aroundRoomList = new List<int>(visibleArea.Count);
		PlayerCharacter player = CharacterManager.instance.GetPlayer();
		for (int i = 0, max = visibleArea.Count; i < max; i++) {
			MapSquareData targetSquare = MapSquareManager.instance.Get(visibleArea[i]);
			if (targetSquare == null ||
				targetSquare.roomID < 0) continue;

			if (aroundRoomList.Exists(roomID => roomID == targetSquare.roomID)) continue;

			aroundRoomList.Add(targetSquare.roomID);
		}
		// 隣接している部屋の全マス取得
		for (int i = 0, max = aroundRoomList.Count; i < max; i++) {
			RoomData roomData = MapSquareManager.instance.GetRoom(aroundRoomList[i]);
			if (roomData == null) continue;

			MeargeList(ref visibleArea, roomData.squareIDList);
		}
	}

	/// <summary>
	/// 等チェビシェフ距離のマスを全て取得
	/// </summary>
	/// <param name="result"></param>
	/// <param name="sourceSquare"></param>
	/// <param name="distance"></param>
	public static void GetChebyshevAroundSquare(ref List<int> result, MapSquareData sourceSquare, int distance = 1) {
		InitializeList(ref result, distance * 8);
		if (sourceSquare == null) return;

		if (distance == 0) {
			result.Add(sourceSquare.ID);
			return;
		}

		int countMax = distance * 2;
		int sourceX = sourceSquare.positionX;
		int sourceY = sourceSquare.positionY;
		for (int count = 0; count < countMax; count++) {
			MapSquareData targetSquare = MapSquareManager.instance.Get(sourceX - distance + count, sourceY - distance);
			if (targetSquare != null) result.Add(targetSquare.ID);

			targetSquare = MapSquareManager.instance.Get(sourceX + distance, sourceY - distance + count);
			if (targetSquare != null) result.Add(targetSquare.ID);

			targetSquare = MapSquareManager.instance.Get(sourceX + distance - count, sourceY + distance);
			if (targetSquare != null) result.Add(targetSquare.ID);

			targetSquare = MapSquareManager.instance.Get(sourceX - distance, sourceY + distance - count);
			if (targetSquare != null) result.Add(targetSquare.ID);

		}
	}

	/// <summary>
	/// ID指定のマスの取得
	/// </summary>
	/// <param name="ID"></param>
	/// <returns></returns>
	public static MapSquareData GetSquare(int ID) {
		return MapSquareManager.instance.Get(ID);
	}

	/// <summary>
	/// 座標指定のマス取得
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	public static MapSquareData GetSquare(int x, int y) {
		return MapSquareManager.instance.Get(x, y);
	}

	/// <summary>
	/// キャラの居るマス取得
	/// </summary>
	/// <param name="character"></param>
	/// <returns></returns>
	public static MapSquareData GetCharacterSquare(CharacterBase character) {
		if (character == null) return null;

		return GetSquare(character.positionX, character.positionY);
	}

}
