/**
 * @file ActionMasterUtility.cs
 * @brief 行動マスターデータ実行処理
 * @author yao
 * @date 2025/2/20
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionMasterUtility {
	public static Entity_ActionData.Param GetActionMaster(int ID) {
		List<Entity_ActionData.Param> actionMasterList = MasterDataManager.actionData[0];
		for (int i = 0, max = actionMasterList.Count; i < max; i++) {
			if (actionMasterList[i].ID != ID) continue;

			return actionMasterList[i];
		}
		return null;
	}

}
