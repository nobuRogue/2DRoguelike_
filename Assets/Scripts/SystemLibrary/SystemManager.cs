/**
 * @file SystemManager.cs
 * @brief システムオブジェクト管理
 * @author yao
 * @date 2025/1/9
 */

using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemManager : MonoBehaviour {
	[SerializeField]
	private SystemObject[] _systemObjectList = null;

	void Start() {
		UniTask task = Initialize();
	}

	private async UniTask Initialize() {
		for (int i = 0, max = _systemObjectList.Length; i < max; i++) {
			SystemObject origin = _systemObjectList[i];
			if (origin == null) continue;

			SystemObject createObj = Instantiate(origin, transform);
			await createObj.Initialize();
		}
		UniTask task = PartManager.instance.TransitionPart(eGamePart.Standby);
	}

}
