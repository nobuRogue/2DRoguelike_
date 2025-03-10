/**
 * @file MapSquareManager.cs
 * @brief マスの管理
 * @author yao
 * @date 2025/1/9
 */
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

using static CommonModule;
using static GameConst;

public class MapSquareManager : MonoBehaviour {
	[SerializeField]
	private MapSquareObject _squareObjectOrigin = null; // マスオブジェクトのオリジナル

	[SerializeField]
	private Transform _squareObjectRoot = null;     // マスオブジェクトの親

	public static MapSquareManager instance { get; private set; } = null;

	private List<MapSquareData> _squareDataList = null;
	private List<MapSquareObject> _squareObjectList = null;

	private List<RoomData> _roomList = null;


	/// <summary>
	/// 初期化
	/// </summary>
	public void Initialize() {
		instance = this;
		MapSquareData.SetGetObjectCallback(GetSquareObject);
		int squareCount = MAP_SQUARE_HEIGHT_COUNT * MAP_SQUARE_WIDTH_COUNT;
		_squareDataList = new List<MapSquareData>(squareCount);
		_squareObjectList = new List<MapSquareObject>(squareCount);
		// マスの生成
		for (int i = 0; i < squareCount; i++) {
			// オブジェクト生成
			MapSquareObject createObject = Instantiate(_squareObjectOrigin, _squareObjectRoot);
			_squareObjectList.Add(createObject);
			// データ生成
			MapSquareData createSquare = new MapSquareData();
			int x, y;
			GetSquarePosition(i, out x, out y);
			createSquare.Setup(i, x, y);
			_squareDataList.Add(createSquare);
			createSquare.SetTerrain(eTerrain.Wall);
		}
		_roomList = new List<RoomData>(AREA_DEVIDE_COUNT + 1);
	}

	private MapSquareObject GetSquareObject(int ID) {
		if (!IsEnableIndex(_squareObjectList, ID)) return null;

		return _squareObjectList[ID];
	}

	/// <summary>
	/// IDを2次元座標に変換
	/// </summary>
	/// <param name="ID"></param>
	/// <returns></returns>
	public void GetSquarePosition(int ID, out int x, out int y) {
		x = ID % MAP_SQUARE_WIDTH_COUNT;
		y = ID / MAP_SQUARE_WIDTH_COUNT;
	}

	/// <summary>
	/// 2次元座標をIDに変換
	/// </summary>
	/// <param name="ID"></param>
	/// <returns></returns>
	public int GetID(int x, int y) {
		if (x < 0 || x >= MAP_SQUARE_WIDTH_COUNT ||
			y < 0 || y >= MAP_SQUARE_HEIGHT_COUNT) return -1;

		return y * MAP_SQUARE_WIDTH_COUNT + x;
	}

	public void ExecuteAllSquare(System.Action<MapSquareData> action) {
		if (action == null || IsEmpty(_squareDataList)) return;

		for (int i = 0, max = _squareDataList.Count; i < max; i++) {
			action(_squareDataList[i]);
		}
	}

	public MapSquareData Get(int ID) {
		if (!IsEnableIndex(_squareDataList, ID)) return null;

		return _squareDataList[ID];
	}

	public MapSquareData Get(int x, int y) {
		return Get(GetID(x, y));
	}

	public void AddRoom(RoomData addRoom) {
		int roomID = _roomList.Count;
		addRoom.SetRoomID(roomID);
		_roomList.Add(addRoom);
	}

	public void RemoveAllRoom() {
		for (int i = 0, max = _roomList.Count; i < max; i++) {
			_roomList[i]?.Teardown();
		}
		_roomList.Clear();
	}

	public RoomData GetRandomRoom() {
		if (IsEmpty(_roomList)) return null;

		return _roomList[UnityEngine.Random.Range(0, _roomList.Count)];
	}

	public RoomData GetRoom(int ID) {
		if (!IsEnableIndex(_roomList, ID)) return null;

		return _roomList[ID];
	}

}
