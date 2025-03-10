/**
 * @file MenuRogueLog.cs
 * @brief ���O�\�����j���[
 * @author yao
 * @date 2025/1/21
 */

using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CommonModule;

public class MenuRogueLog : MenuBase {
	public static MenuRogueLog instance { get; private set; } = null;

	[SerializeField]
	private RogueLog _originLogObject = null;

	[SerializeField]
	private Transform _unuseRoot = null;

	[SerializeField]
	private Transform _useRoot = null;

	// 1��ʂɕ\������郍�O�̍ő吔
	private int _DEFAULT_LOG_COUNT = 4;
	// �g�p���̃��O�I�u�W�F�N�g
	private List<RogueLog> _useList = null;
	// ���g�p�̃��O�I�u�W�F�N�g
	private List<RogueLog> _unuseList = null;
	// �ҋ@���O�̏�������
	private int _DEFAULT_STANDBY_LOG_COUNT = 128;
	// �ҋ@���̃��O���b�Z�[�W
	private List<string> _standbyLogList = null;

	public override async UniTask Initialize() {
		instance = this;
		await base.Initialize();
		_standbyLogList = new List<string>(_DEFAULT_STANDBY_LOG_COUNT);
		_useList = new List<RogueLog>(_DEFAULT_LOG_COUNT);
		_unuseList = new List<RogueLog>(_DEFAULT_LOG_COUNT);
		for (int i = 0; i < _DEFAULT_LOG_COUNT; i++) {
			RogueLog createObject = Instantiate(_originLogObject, _unuseRoot);
			_unuseList.Add(createObject);
		}
		UniTask task = ShowLogTask();
	}

	/// <summary>
	/// ���O�̒ǉ�
	/// </summary>
	/// <param name="logMessage"></param>
	public void AddLog(string logMessage) {
		_standbyLogList.Add(logMessage);
	}

	private async UniTask ShowLogTask() {
		while (true) {
			// �ҋ@���̃��O���b�Z�[�W������A�g�p�\�ȃ��O�I�u�W�F�N�g�����邩����
			while (IsEmpty(_standbyLogList) || IsEmpty(_unuseList)) await UniTask.DelayFrame(1);

			string showMessage = _standbyLogList[0];
			_standbyLogList.RemoveAt(0);
			if (UseLogObject(showMessage) == null) continue;

			int logCount = _useList.Count;
			List<UniTask> taskList = new List<UniTask>(logCount);
			for (int i = 0; i < logCount; i++) {
				taskList.Add(_useList[i].FlowLog());
			}
			await WaitTask(taskList);
			while (_useList.Count >= _DEFAULT_LOG_COUNT) UnuseLogObject(_useList[0]);

		}
	}

	/// <summary>
	/// ���O�I�u�W�F�N�g��\������
	/// </summary>
	/// <param name="logMessage"></param>
	/// <returns></returns>
	private RogueLog UseLogObject(string logMessage) {
		if (IsEmpty(_unuseList)) return null;

		RogueLog useLogObject = _unuseList[0];
		_unuseList.RemoveAt(0);
		useLogObject.Setup(logMessage);
		_useList.Add(useLogObject);
		useLogObject.transform.SetParent(_useRoot);
		useLogObject.transform.localPosition = Vector3.zero;
		return useLogObject;
	}

	/// <summary>
	/// ���O�I�u�W�F�N�g�𖢎g�p��Ԃɂ���
	/// </summary>
	/// <param name="unuseLog"></param>
	private void UnuseLogObject(RogueLog unuseLog) {
		if (unuseLog == null) return;

		unuseLog.Teardown();
		_useList.Remove(unuseLog);
		_unuseList.Add(unuseLog);
		unuseLog.transform.SetParent(_unuseRoot);
	}

	/// <summary>
	/// �\������Ă��郍�O�Ƒҋ@�����O�̃N���A
	/// </summary>
	public void ClearLog() {
		// �ҋ@�����O�̃N���A
		_standbyLogList.Clear();
		// �\�������O�I�u�W�F�N�g�̃N���A
		for (int i = _useList.Count - 1; i >= 0; i--) {
			UnuseLogObject(_useList[i]);
		}
	}

}
