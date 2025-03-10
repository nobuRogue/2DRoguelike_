/**
 * @file MenuRogueLog.cs
 * @brief ログ表示メニュー
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

	// 1画面に表示されるログの最大数
	private int _DEFAULT_LOG_COUNT = 4;
	// 使用中のログオブジェクト
	private List<RogueLog> _useList = null;
	// 未使用のログオブジェクト
	private List<RogueLog> _unuseList = null;
	// 待機ログの初期化数
	private int _DEFAULT_STANDBY_LOG_COUNT = 128;
	// 待機中のログメッセージ
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
	/// ログの追加
	/// </summary>
	/// <param name="logMessage"></param>
	public void AddLog(string logMessage) {
		_standbyLogList.Add(logMessage);
	}

	private async UniTask ShowLogTask() {
		while (true) {
			// 待機中のログメッセージがあり、使用可能なログオブジェクトがあるか判定
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
	/// ログオブジェクトを表示する
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
	/// ログオブジェクトを未使用状態にする
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
	/// 表示されているログと待機中ログのクリア
	/// </summary>
	public void ClearLog() {
		// 待機中ログのクリア
		_standbyLogList.Clear();
		// 表示中ログオブジェクトのクリア
		for (int i = _useList.Count - 1; i >= 0; i--) {
			UnuseLogObject(_useList[i]);
		}
	}

}
