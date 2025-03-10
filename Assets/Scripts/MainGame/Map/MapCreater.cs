/**
 * @file MapCreater.cs
 * @brief ランダムマップ生成
 * @author yao
 * @date 2025/1/14
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static GameConst;
using static CommonModule;

public class MapCreater {

	private class AreaData {
		public int startX = -1;
		public int startY = -1;
		public int width = -1;
		public int height = -1;

		public AreaData(int setX, int setY, int setWidth, int setHeight) {
			startX = setX;
			startY = setY;
			width = setWidth;
			height = setHeight;
		}
	}

	private static List<AreaData> _areaList = null;
	private static List<int> _devideLineList = null;

	public static void CreateMap() {
		// 部屋情報クリア
		MapSquareManager.instance.RemoveAllRoom();
		// 壁で埋める
		_devideLineList = new List<int>(MAP_SQUARE_WIDTH_COUNT * MAP_SQUARE_HEIGHT_COUNT);
		MapSquareManager.instance.ExecuteAllSquare(SetWall);
		// 最初のエリアを作る
		_areaList = new List<AreaData>(AREA_DEVIDE_COUNT + 1);
		_areaList.Add(new AreaData(2, 2, MAP_SQUARE_WIDTH_COUNT - 4, MAP_SQUARE_HEIGHT_COUNT - 4));
		// エリアを分割する
		DevideArea();
		// 部屋を置く
		CreateRoom();
		// 部屋を繋げる
		ConnectRoom();
		// 階段を置く
		CreateStair();
	}

	private static void SetWall(MapSquareData square) {
		square?.SetTerrain(eTerrain.Wall);
		int x = square.positionX, y = square.positionY;
		if (x == 0 || x == MAP_SQUARE_WIDTH_COUNT - 1 ||
			y == 0 || y == MAP_SQUARE_HEIGHT_COUNT - 1) return;

		if (x != 1 && x != MAP_SQUARE_WIDTH_COUNT - 2 &&
			y != 1 && y != MAP_SQUARE_HEIGHT_COUNT - 2) return;

		square.SetTerrain(eTerrain.Wall);
		_devideLineList.Add(square.ID);
	}

	private static void DevideArea() {
		for (int i = 0; i < AREA_DEVIDE_COUNT; i++) {
			// 幅最大のエリアを取得
			AreaData maxSizeArea = GetMaxSizeArea(out int maxSize, out bool isVertical);
			if (maxSizeArea == null || maxSize < (MIN_ROOM_SIZE + 2) * 2 + 1) break;
			// 取得したエリアを分割
			DevideArea(maxSizeArea, isVertical);
		}
	}

	/// <summary>
	/// 最大サイズのエリアを取得
	/// </summary>
	/// <param name="maxSize">エリアのサイズ</param>
	/// <param name="isVertical">縦か横か</param>
	/// <returns></returns>
	private static AreaData GetMaxSizeArea(out int maxSize, out bool isVertical) {
		maxSize = -1;
		isVertical = false;
		AreaData result = null;
		for (int i = 0, max = _areaList.Count; i < max; i++) {
			AreaData area = _areaList[i];
			if (area.width > maxSize) {
				maxSize = area.width;
				isVertical = false;
				result = area;
			}

			if (area.height > maxSize) {
				maxSize = area.height;
				isVertical = true;
				result = area;
			}
		}
		return result;
	}

	/// <summary>
	/// 指定したエリアを分割する
	/// </summary>
	/// <param name="devideArea"></param>
	/// <param name="isVertical"></param>
	private static void DevideArea(AreaData devideArea, bool isVertical) {
		if (isVertical) {
			// 水平に分割する処理
			DevideAreaVertical(devideArea);
		} else {
			// 垂直に分割する処理
			DevideAreaHorizontal(devideArea);
		}
	}

	/// <summary>
	/// 水平方向のエリア分割処理
	/// </summary>
	/// <param name="devideArea"></param>
	private static void DevideAreaVertical(AreaData devideArea) {
		// 分割位置の決定
		int randomMax = devideArea.height - (MIN_ROOM_SIZE + 2) * 2;
		int devidePos = Random.Range(0, randomMax);
		devidePos += MIN_ROOM_SIZE + 2 + devideArea.startY;
		// 新しいエリアの生成
		int newAreaHeight = devideArea.startY + devideArea.height - devidePos - 1;
		_areaList.Add(new AreaData(devideArea.startX, devidePos + 1, devideArea.width, newAreaHeight));
		// 既存エリアの修正
		devideArea.height = devidePos - devideArea.startY;
		// 分割線マスの追加
		for (int x = 0, max = devideArea.width; x < max; x++) {
			MapSquareData square = MapSquareManager.instance.Get(devideArea.startX + x, devidePos);
			_devideLineList.Add(square.ID);
		}
	}

	/// <summary>
	/// 垂直方向のエリア分割処理
	/// </summary>
	/// <param name="devideArea"></param>
	private static void DevideAreaHorizontal(AreaData devideArea) {
		// 分割位置の決定
		int randomMax = devideArea.width - (MIN_ROOM_SIZE + 2) * 2;
		int devidePos = Random.Range(0, randomMax);
		devidePos += MIN_ROOM_SIZE + 2 + devideArea.startX;
		// 新しいエリアの生成
		int newAreaWidth = devideArea.startX + devideArea.width - devidePos - 1;
		_areaList.Add(new AreaData(devidePos + 1, devideArea.startY, newAreaWidth, devideArea.height));
		// 既存エリアの修正
		devideArea.width = devidePos - devideArea.startX;
		// 分割線マスの追加
		for (int y = 0, max = devideArea.height; y < max; y++) {
			MapSquareData square = MapSquareManager.instance.Get(devidePos, devideArea.startY + y);
			_devideLineList.Add(square.ID);
		}
	}

	/// <summary>
	/// 部屋の生成
	/// </summary>
	private static void CreateRoom() {
		for (int i = 0, max = _areaList.Count; i < max; i++) {
			CreateRoom(_areaList[i]);
		}
	}

	/// <summary>
	/// エリア指定の部屋配置
	/// </summary>
	/// <param name="area"></param>
	private static void CreateRoom(AreaData area) {
		if (area == null) return;
		// 部屋のサイズ決定
		int roomWidth = Random.Range(MIN_ROOM_SIZE, area.width - 1);
		int roomHeight = Random.Range(MIN_ROOM_SIZE, area.height - 1);
		// 部屋の生成位置決定
		int xRandomRange = area.width - roomWidth - 1;
		int yRandomRange = area.height - roomHeight - 1;
		int startX = area.startX + Random.Range(0, xRandomRange) + 1;
		int startY = area.startY + Random.Range(0, yRandomRange) + 1;
		// 部屋の生成（地形の変更）
		RoomData createRoom = new RoomData();
		for (int y = 0; y < roomHeight; y++) {
			for (int x = 0; x < roomWidth; x++) {
				MapSquareData roomSquare = MapSquareManager.instance.Get(startX + x, startY + y);
				if (roomSquare == null) continue;

				roomSquare.SetTerrain(eTerrain.Room);
				createRoom.AddSquare(roomSquare.ID);
			}
		}
		MapSquareManager.instance.AddRoom(createRoom);
	}

	/// <summary>
	/// 全部屋を通路で連結
	/// </summary>
	private static void ConnectRoom() {
		// 掘削方向をランダムに決定
		eDirectionFour dir = (eDirectionFour)Random.Range(0, (int)eDirectionFour.Max);
		for (int i = 0, max = _areaList.Count - 1; i < max; i++) {
			// エリア1を分割線まで掘る
			AreaData area1 = _areaList[i];
			MapSquareData startSquare = DigToDevideLine(area1, dir);
			// エリア2を分割線まで掘る
			dir = (eDirectionFour)Random.Range(0, (int)eDirectionFour.Max);
			AreaData area2 = _areaList[i + 1];
			MapSquareData goalSquare = DigToDevideLine(area2, dir);
			// 分割線内で繋げる
			List<ManhattanMoveData> route = RouteSearcher.RouteSearch(startSquare.ID, goalSquare.ID, IsDevideLine);
			DigRoute(route);

			int dirIndex = (int)dir + Random.Range(1, (int)eDirectionFour.Max);
			if (dirIndex >= (int)eDirectionFour.Max) dirIndex -= (int)eDirectionFour.Max;

			dir = (eDirectionFour)dirIndex;
		}
	}

	/// <summary>
	/// 経路の通りに通路として掘る
	/// </summary>
	/// <param name="route"></param>
	private static void DigRoute(List<ManhattanMoveData> route) {
		for (int i = 0, max = route.Count; i < max; i++) {
			MapSquareManager.instance.Get(route[i].targetSquareID)?.SetTerrain(eTerrain.Passage);
		}
	}

	private static bool IsDevideLine(MapSquareData square, eDirectionFour dir, int distance) {
		return _devideLineList.Exists(squareID => square.ID == squareID);
	}

	/// <summary>
	/// 部屋からエリア分割線まで掘る
	/// </summary>
	/// <param name="area"></param>
	/// <param name="dir"></param>
	/// <returns></returns>
	private static MapSquareData DigToDevideLine(AreaData area, eDirectionFour dir) {
		// 掘削開始マスの決定
		eDirectionFour reverseDir = dir.ReverseDir();
		List<MapSquareData> targetList = new List<MapSquareData>(16);
		int startX = area.startX;
		int startY = area.startY;
		for (int y = 0, yMax = area.height; y < yMax; y++) {
			for (int x = 0, xMax = area.width; x < xMax; x++) {
				// 壁地形でかつ、掘削方向と反対のマスが部屋地形のマスを集約
				MapSquareData square = MapSquareManager.instance.Get(startX + x, startY + y);
				if (square == null || square.terrain != eTerrain.Wall) continue;

				MapSquareData toDirSquare = MapSquareUtility.GetToDirSquare(square.positionX, square.positionY, reverseDir);
				if (toDirSquare == null || toDirSquare.terrain != eTerrain.Room) continue;

				targetList.Add(square);
			}
		}
		if (IsEmpty(targetList)) return null;
		// 分割線まで掘る
		MapSquareData currentSquare = targetList[Random.Range(0, targetList.Count)];
		while (true) {
			currentSquare.SetTerrain(eTerrain.Passage);
			// 分割線リストに現在のマスが含まれていたら終了
			if (_devideLineList.Exists(squareID => squareID == currentSquare.ID)) break;

			currentSquare = MapSquareUtility.GetToDirSquare(currentSquare.positionX, currentSquare.positionY, dir);
		}
		return currentSquare;
	}

	private static void CreateStair() {
		// ランダムな部屋の取得
		RoomData targetRoom = MapSquareManager.instance.GetRandomRoom();
		if (targetRoom == null) return;
		// 部屋のランダムなマスを取得して階段にする
		List<int> squareList = targetRoom.squareIDList;
		int targetSquareID = squareList[Random.Range(0, squareList.Count)];
		MapSquareManager.instance.Get(targetSquareID)?.SetTerrain(eTerrain.Stair);
	}

}
