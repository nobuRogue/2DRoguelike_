/**
 * @file RoomData.cs
 * @brief 1ïîâÆÇÃèÓïÒ
 * @author yao
 * @date 2025/1/21
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static GameConst;

public class RoomData {
	public int roomID { get; private set; } = -1;
	public List<int> squareIDList { get; private set; } = null;
	public RoomData() {
		squareIDList = new List<int>(MAX_ROOM_SIZE * MAX_ROOM_SIZE);
	}

	/// <summary>
	/// IDÇÃê›íË
	/// </summary>
	/// <param name="setID"></param>
	public void SetRoomID(int setID) {
		roomID = setID;
		for (int i = 0, max = squareIDList.Count; i < max; i++) {
			MapSquareManager.instance.Get(squareIDList[i]).SetRoomID(roomID);
		}
	}

	/// <summary>
	/// É}ÉXÇÃí«â¡
	/// </summary>
	/// <param name="squareID"></param>
	public void AddSquare(int squareID) {
		squareIDList.Add(squareID);
	}

	public void Teardown() {
		roomID = -1;
		for (int i = 0, max = squareIDList.Count; i < max; i++) {
			MapSquareManager.instance.Get(squareIDList[i]).SetRoomID(-1);
		}
		squareIDList.Clear();
	}

}
