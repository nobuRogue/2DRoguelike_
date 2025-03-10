/**
 * @file MenuEnding.cs
 * @brief �Q�[���I�����j���[
 * @author yao
 * @date 2025/2/13
 */

using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuEnding : MenuBase {
	public override async UniTask Open() {
		await base.Open();
		await FadeManager.instance.FadeIn();
		while (true) {
			if (Input.GetKeyDown(KeyCode.Z)) break;

			await UniTask.DelayFrame(1);
		}
		await FadeManager.instance.FadeOut();
	}
}
