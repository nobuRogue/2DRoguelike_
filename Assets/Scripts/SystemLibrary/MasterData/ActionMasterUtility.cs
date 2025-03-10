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
	/// <summary>
	/// アクションマスター取得
	/// </summary>
	/// <param name="ID"></param>
	/// <returns></returns>
	public static Entity_ActionData.Param GetActionMaster( int ID ) {
		List<Entity_ActionData.Param> actionMasterList = MasterDataManager.actionData[0];
		for (int i = 0, max = actionMasterList.Count; i < max; i++) {
			if (actionMasterList[i].ID != ID) continue;

			return actionMasterList[i];
		}
		return null;
	}

	/// <summary>
	/// 効果マスターデータ取得
	/// </summary>
	/// <param name="ID"></param>
	/// <returns></returns>
	public static Entity_ActionEffectData.Param GetActionEffectMaster( int ID ) {
		List<Entity_ActionEffectData.Param> actionMasterList = MasterDataManager.actionEffectData[0];
		for (int i = 0, max = actionMasterList.Count; i < max; i++) {
			if (actionMasterList[i].ID != ID) continue;

			return actionMasterList[i];
		}
		return null;
	}

}
