/**
 * @file MenuBase.cs
 * @brief ƒƒjƒ…[‚ÌŠî’ê
 * @author yao
 * @date 2025/2/6
 */

using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MenuBase : MonoBehaviour {
	[SerializeField]
	private GameObject _menuRoot = null;

	public virtual async UniTask Initialize() {
		await UniTask.CompletedTask;
	}

	public virtual async UniTask Open() {
		_menuRoot.SetActive(true);
		await UniTask.CompletedTask;
	}

	public virtual async UniTask Close() {
		_menuRoot.SetActive(false);
		await UniTask.CompletedTask;
	}
}
