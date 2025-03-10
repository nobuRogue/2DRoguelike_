/**
 * @file PartManager.cs
 * @brief パート管理
 * @author yao
 * @date 2025/1/9
 */

using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CommonModule;

public class PartManager : SystemObject {
	[SerializeField]
	private PartBase[] _partOriginArray = null;

	public static PartManager instance { get; private set; } = null;

	private PartBase[] _partList = null;
	private PartBase _currentPart = null;

	public override async UniTask Initialize() {
		instance = this;
		// パートオブジェクトの生成、初期化
		int partMax = (int)eGamePart.Max;
		_partList = new PartBase[partMax];

		List<UniTask> taskList = new List<UniTask>(partMax);
		for (int i = 0; i < partMax; i++) {
			_partList[i] = Instantiate(_partOriginArray[i], transform);
			taskList.Add(_partList[i].Initialize());
		}
		// 各パートの初期化待ちをする
		await WaitTask(taskList);
		for (int i = 0; i < partMax; i++) {
			_partList[i]?.gameObject.SetActive(false);
		}
	}

	/// <summary>
	/// パートの遷移
	/// </summary>
	/// <param name="nextPart"></param>
	/// <returns></returns>
	public async UniTask TransitionPart(eGamePart nextPart) {
		if (_currentPart != null) {
			await _currentPart.Teardown();
			_currentPart.gameObject.SetActive(false);
		}

		_currentPart = _partList[(int)nextPart];
		_currentPart.gameObject.SetActive(true);
		await _currentPart.Setup();
		UniTask task = _currentPart.Execute();
	}

}
