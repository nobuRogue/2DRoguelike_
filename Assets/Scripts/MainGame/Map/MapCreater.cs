/**
 * @file MapCreater.cs
 * @brief �����_���}�b�v����
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
		// �������N���A
		MapSquareManager.instance.RemoveAllRoom();
		// �ǂŖ��߂�
		_devideLineList = new List<int>(MAP_SQUARE_WIDTH_COUNT * MAP_SQUARE_HEIGHT_COUNT);
		MapSquareManager.instance.ExecuteAllSquare(SetWall);
		// �ŏ��̃G���A�����
		_areaList = new List<AreaData>(AREA_DEVIDE_COUNT + 1);
		_areaList.Add(new AreaData(2, 2, MAP_SQUARE_WIDTH_COUNT - 4, MAP_SQUARE_HEIGHT_COUNT - 4));
		// �G���A�𕪊�����
		DevideArea();
		// ������u��
		CreateRoom();
		// �������q����
		ConnectRoom();
		// �K�i��u��
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
			// ���ő�̃G���A���擾
			AreaData maxSizeArea = GetMaxSizeArea(out int maxSize, out bool isVertical);
			if (maxSizeArea == null || maxSize < (MIN_ROOM_SIZE + 2) * 2 + 1) break;
			// �擾�����G���A�𕪊�
			DevideArea(maxSizeArea, isVertical);
		}
	}

	/// <summary>
	/// �ő�T�C�Y�̃G���A���擾
	/// </summary>
	/// <param name="maxSize">�G���A�̃T�C�Y</param>
	/// <param name="isVertical">�c������</param>
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
	/// �w�肵���G���A�𕪊�����
	/// </summary>
	/// <param name="devideArea"></param>
	/// <param name="isVertical"></param>
	private static void DevideArea(AreaData devideArea, bool isVertical) {
		if (isVertical) {
			// �����ɕ������鏈��
			DevideAreaVertical(devideArea);
		} else {
			// �����ɕ������鏈��
			DevideAreaHorizontal(devideArea);
		}
	}

	/// <summary>
	/// ���������̃G���A��������
	/// </summary>
	/// <param name="devideArea"></param>
	private static void DevideAreaVertical(AreaData devideArea) {
		// �����ʒu�̌���
		int randomMax = devideArea.height - (MIN_ROOM_SIZE + 2) * 2;
		int devidePos = Random.Range(0, randomMax);
		devidePos += MIN_ROOM_SIZE + 2 + devideArea.startY;
		// �V�����G���A�̐���
		int newAreaHeight = devideArea.startY + devideArea.height - devidePos - 1;
		_areaList.Add(new AreaData(devideArea.startX, devidePos + 1, devideArea.width, newAreaHeight));
		// �����G���A�̏C��
		devideArea.height = devidePos - devideArea.startY;
		// �������}�X�̒ǉ�
		for (int x = 0, max = devideArea.width; x < max; x++) {
			MapSquareData square = MapSquareManager.instance.Get(devideArea.startX + x, devidePos);
			_devideLineList.Add(square.ID);
		}
	}

	/// <summary>
	/// ���������̃G���A��������
	/// </summary>
	/// <param name="devideArea"></param>
	private static void DevideAreaHorizontal(AreaData devideArea) {
		// �����ʒu�̌���
		int randomMax = devideArea.width - (MIN_ROOM_SIZE + 2) * 2;
		int devidePos = Random.Range(0, randomMax);
		devidePos += MIN_ROOM_SIZE + 2 + devideArea.startX;
		// �V�����G���A�̐���
		int newAreaWidth = devideArea.startX + devideArea.width - devidePos - 1;
		_areaList.Add(new AreaData(devidePos + 1, devideArea.startY, newAreaWidth, devideArea.height));
		// �����G���A�̏C��
		devideArea.width = devidePos - devideArea.startX;
		// �������}�X�̒ǉ�
		for (int y = 0, max = devideArea.height; y < max; y++) {
			MapSquareData square = MapSquareManager.instance.Get(devidePos, devideArea.startY + y);
			_devideLineList.Add(square.ID);
		}
	}

	/// <summary>
	/// �����̐���
	/// </summary>
	private static void CreateRoom() {
		for (int i = 0, max = _areaList.Count; i < max; i++) {
			CreateRoom(_areaList[i]);
		}
	}

	/// <summary>
	/// �G���A�w��̕����z�u
	/// </summary>
	/// <param name="area"></param>
	private static void CreateRoom(AreaData area) {
		if (area == null) return;
		// �����̃T�C�Y����
		int roomWidth = Random.Range(MIN_ROOM_SIZE, area.width - 1);
		int roomHeight = Random.Range(MIN_ROOM_SIZE, area.height - 1);
		// �����̐����ʒu����
		int xRandomRange = area.width - roomWidth - 1;
		int yRandomRange = area.height - roomHeight - 1;
		int startX = area.startX + Random.Range(0, xRandomRange) + 1;
		int startY = area.startY + Random.Range(0, yRandomRange) + 1;
		// �����̐����i�n�`�̕ύX�j
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
	/// �S������ʘH�ŘA��
	/// </summary>
	private static void ConnectRoom() {
		// �@������������_���Ɍ���
		eDirectionFour dir = (eDirectionFour)Random.Range(0, (int)eDirectionFour.Max);
		for (int i = 0, max = _areaList.Count - 1; i < max; i++) {
			// �G���A1�𕪊����܂Ō@��
			AreaData area1 = _areaList[i];
			MapSquareData startSquare = DigToDevideLine(area1, dir);
			// �G���A2�𕪊����܂Ō@��
			dir = (eDirectionFour)Random.Range(0, (int)eDirectionFour.Max);
			AreaData area2 = _areaList[i + 1];
			MapSquareData goalSquare = DigToDevideLine(area2, dir);
			// ���������Ōq����
			List<ManhattanMoveData> route = RouteSearcher.RouteSearch(startSquare.ID, goalSquare.ID, IsDevideLine);
			DigRoute(route);

			int dirIndex = (int)dir + Random.Range(1, (int)eDirectionFour.Max);
			if (dirIndex >= (int)eDirectionFour.Max) dirIndex -= (int)eDirectionFour.Max;

			dir = (eDirectionFour)dirIndex;
		}
	}

	/// <summary>
	/// �o�H�̒ʂ�ɒʘH�Ƃ��Č@��
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
	/// ��������G���A�������܂Ō@��
	/// </summary>
	/// <param name="area"></param>
	/// <param name="dir"></param>
	/// <returns></returns>
	private static MapSquareData DigToDevideLine(AreaData area, eDirectionFour dir) {
		// �@��J�n�}�X�̌���
		eDirectionFour reverseDir = dir.ReverseDir();
		List<MapSquareData> targetList = new List<MapSquareData>(16);
		int startX = area.startX;
		int startY = area.startY;
		for (int y = 0, yMax = area.height; y < yMax; y++) {
			for (int x = 0, xMax = area.width; x < xMax; x++) {
				// �ǒn�`�ł��A�@������Ɣ��΂̃}�X�������n�`�̃}�X���W��
				MapSquareData square = MapSquareManager.instance.Get(startX + x, startY + y);
				if (square == null || square.terrain != eTerrain.Wall) continue;

				MapSquareData toDirSquare = MapSquareUtility.GetToDirSquare(square.positionX, square.positionY, reverseDir);
				if (toDirSquare == null || toDirSquare.terrain != eTerrain.Room) continue;

				targetList.Add(square);
			}
		}
		if (IsEmpty(targetList)) return null;
		// �������܂Ō@��
		MapSquareData currentSquare = targetList[Random.Range(0, targetList.Count)];
		while (true) {
			currentSquare.SetTerrain(eTerrain.Passage);
			// ���������X�g�Ɍ��݂̃}�X���܂܂�Ă�����I��
			if (_devideLineList.Exists(squareID => squareID == currentSquare.ID)) break;

			currentSquare = MapSquareUtility.GetToDirSquare(currentSquare.positionX, currentSquare.positionY, dir);
		}
		return currentSquare;
	}

	private static void CreateStair() {
		// �����_���ȕ����̎擾
		RoomData targetRoom = MapSquareManager.instance.GetRandomRoom();
		if (targetRoom == null) return;
		// �����̃����_���ȃ}�X���擾���ĊK�i�ɂ���
		List<int> squareList = targetRoom.squareIDList;
		int targetSquareID = squareList[Random.Range(0, squareList.Count)];
		MapSquareManager.instance.Get(targetSquareID)?.SetTerrain(eTerrain.Stair);
	}

}
