/**
 * @file PartManager.cs
 * @brief �p�[�g�Ǘ�
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
		// �p�[�g�I�u�W�F�N�g�̐����A������
		int partMax = (int)eGamePart.Max;
		_partList = new PartBase[partMax];

		List<UniTask> taskList = new List<UniTask>(partMax);
		for (int i = 0; i < partMax; i++) {
			_partList[i] = Instantiate(_partOriginArray[i], transform);
			taskList.Add(_partList[i].Initialize());
		}
		// �e�p�[�g�̏������҂�������
		await WaitTask(taskList);
		for (int i = 0; i < partMax; i++) {
			_partList[i]?.gameObject.SetActive(false);
		}
	}

	/// <summary>
	/// �p�[�g�̑J��
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
