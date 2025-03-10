/**
 * @file UserDataHolder.cs
 * @brief ユーザーデータ保持
 * @author yao
 * @date 2025/2/4
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDataHolder {
	public static UserData currentData { get; private set; } = null;

	public static void SetCurrentData(UserData setData) {
		currentData = setData;
	}
}
