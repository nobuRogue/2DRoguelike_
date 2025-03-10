/**
 * @file UserData.cs
 * @brief ユーザーが持つデーダ
 * @author yao
 * @date 2025/2/4
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserData {
	/// <summary>
	/// 現在の階数
	/// </summary>
	public int floorCount { get; private set; } = -1;

	public UserData() {
		SetFloorCount(1);
	}

	public void SetFloorCount(int setCount) {
		floorCount = setCount;
		MenuManager.instance.Get<MenuPlayerStatus>().SetFloorCount(floorCount);
	}
}
