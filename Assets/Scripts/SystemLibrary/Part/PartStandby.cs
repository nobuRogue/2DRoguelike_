/**
 * @file PartStandby.cs
 * @brief アプリの準備パート
 * @author yao
 * @date 2025/1/9
 */

using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartStandby : PartBase {
	public override async UniTask Initialize() {
		await UniTask.CompletedTask;
	}

	public override async UniTask Setup() {
		await UniTask.CompletedTask;
	}

	public override async UniTask Execute() {
		MasterDataManager.LoadAllData();
		UniTask task = PartManager.instance.TransitionPart(eGamePart.Title);
		await UniTask.CompletedTask;
	}

	public override async UniTask Teardown() {
		await UniTask.CompletedTask;
	}
}
