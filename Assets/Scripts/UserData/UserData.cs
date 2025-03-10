/**
 * @file UserData.cs
 * @brief ���[�U�[�����f�[�_
 * @author yao
 * @date 2025/2/4
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserData {
	/// <summary>
	/// ���݂̊K��
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
