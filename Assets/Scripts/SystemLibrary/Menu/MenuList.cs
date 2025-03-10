/**
 * @file MenuList.cs
 * @brief リストメニューの基底
 * @author yao
 * @date 2025/3/6
 */
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

using static CommonModule;

public class MenuList : MenuBase {
	/// <summary>
	/// リスト項目のオリジナル
	/// </summary>
	[SerializeField]
	private MenuListItem _itemOrigin = null;
	/// <summary>
	/// 項目を並べるルートオブジェクト
	/// </summary>
	[SerializeField]
	private Transform _contentRoot = null;
	/// <summary>
	/// 未使用状態の項目のルートオブジェクト
	/// </summary>
	[SerializeField]
	private Transform _unuseRoot = null;

	/// <summary>
	/// リストメニューのコールバック集クラス
	/// </summary>
	protected class MenuListCallbackFortmat {
		// 決定された際の処理
		public System.Func<MenuListItem, UniTask<bool>> OnDecide = null;
		// キャンセルされた際の処理
		public System.Func<MenuListItem, UniTask<bool>> OnCancel = null;
		// カーソルが移動した際の処理
		public System.Func<MenuListItem, MenuListItem, UniTask<bool>> OnMoveCursor = null;
	}
	private MenuListCallbackFortmat _currentFormat = null;

	private int _currentIndex = -1;
	private bool _isContinue = false;

	private List<MenuListItem> _useList = null;
	private List<MenuListItem> _unuseList = null;

	public override async UniTask Initialize() {
		await base.Initialize();
		_useList = new List<MenuListItem>();
		_unuseList = new List<MenuListItem>();
	}

	/// <summary>
	/// コールバック用クラスの設定
	/// </summary>
	/// <param name="setFormat"></param>
	protected void SetCallbackFortmat(MenuListCallbackFortmat setFormat) {
		_currentFormat = setFormat;
	}

	/// <summary>
	/// リスト項目の生成
	/// </summary>
	/// <returns></returns>
	protected MenuListItem AddListItem() {
		MenuListItem addItem;
		if (IsEmpty(_unuseList)) {
			// 未使用リストが空なので生成
			addItem = Instantiate(_itemOrigin, _contentRoot);
		} else {
			// 未使用リストから取得
			addItem = _unuseList[0];
			_unuseList.RemoveAt(0);
			addItem.transform.SetParent(_contentRoot);
		}
		addItem.Deselect();
		_useList.Add(addItem);
		return addItem;
	}

	/// <summary>
	/// インデクス指定のリスト項目削除
	/// </summary>
	/// <param name="itemIndex"></param>
	protected void RemoveListItem(int itemIndex) {
		if (!IsEnableIndex(_useList, itemIndex)) return;
		// 使用リストから取り除く
		MenuListItem removeItem = _useList[itemIndex];
		_useList.RemoveAt(itemIndex);
		// 未使用リストへ追加
		_unuseList.Add(removeItem);
		removeItem.transform.SetParent(_unuseRoot);
		removeItem.Deselect();
	}

	/// <summary>
	/// 全てのリスト項目削除
	/// </summary>
	protected void RemoveAllItem() {
		while (!IsEmpty(_useList)) RemoveListItem(0);

	}

	/// <summary>
	/// リストの入力受付タスク
	/// </summary>
	/// <returns></returns>
	public async UniTask AcceptInput() {
		_isContinue = true;
		while (_isContinue) {
			// カーソルの移動受付
			await AcceptMoveCursor();
			if (!_isContinue) break;
			// 決定の入力受付
			await AcceptDecide();
			if (!_isContinue) break;
			// キャンセルの入力受付
			await AcceptCancel();
			if (!_isContinue) break;

			await UniTask.DelayFrame(1);
		}
	}

	/// <summary>
	/// カーソル移動入力の受付処理
	/// </summary>
	/// <returns></returns>
	private async UniTask AcceptMoveCursor() {
		// 四方向の入力受付
		eDirectionFour inputDir = GetDirInput();
		if (inputDir == eDirectionFour.Invalid) return;
		// 入力に応じたインデクスの変更
		int moveIndex = _currentIndex;
		switch (inputDir) {
			case eDirectionFour.Up:
			moveIndex--;
			break;
			case eDirectionFour.Right:
			break;
			case eDirectionFour.Down:
			moveIndex++;
			break;
			case eDirectionFour.Left:
			break;
		}
		// 移動後のインデクスがリスト項目数に収まるように修正
		if (moveIndex >= _useList.Count) moveIndex -= _useList.Count;

		if (moveIndex < 0) moveIndex += _useList.Count;
		// カーソル移動時の処理
		await SetIndex(moveIndex);
	}

	/// <summary>
	/// 四方向の入力受付
	/// </summary>
	/// <returns></returns>
	private eDirectionFour GetDirInput() {
		if (Input.GetKeyDown(KeyCode.UpArrow)) {
			return eDirectionFour.Up;
		} else if (Input.GetKeyDown(KeyCode.RightArrow)) {
			return eDirectionFour.Right;
		} else if (Input.GetKeyDown(KeyCode.DownArrow)) {
			return eDirectionFour.Down;
		} else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
			return eDirectionFour.Left;
		}
		return eDirectionFour.Invalid;
	}

	/// <summary>
	/// 選択中項目の変更
	/// </summary>
	/// <param name="setIndex"></param>
	/// <returns></returns>
	protected async UniTask SetIndex(int setIndex) {
		if (_currentIndex == setIndex) return;
		// 現在の項目を未選択状態にする
		MenuListItem prevItem;
		if (IsEnableIndex(_useList, _currentIndex)) {
			prevItem = _useList[_currentIndex];
			prevItem.Deselect();
		} else {
			prevItem = null;
		}
		_currentIndex = setIndex;
		if (!IsEnableIndex(_useList, _currentIndex)) return;
		// 移動後の項目を選択状態にする
		MenuListItem currentItem = _useList[_currentIndex];
		currentItem.Select();
		// カーソル移動コールバックの実行
		if (_currentFormat == null ||
			_currentFormat.OnMoveCursor == null) return;

		_isContinue = await _currentFormat.OnMoveCursor(currentItem, prevItem);
	}

	/// <summary>
	/// 決定入力の受付処理
	/// </summary>
	/// <returns></returns>
	private async UniTask AcceptDecide() {
		if (!Input.GetKeyDown(KeyCode.Z)) return;

		if (_currentFormat == null ||
			_currentFormat.OnDecide == null) return;

		MenuListItem currentItem = IsEnableIndex(_useList, _currentIndex) ? _useList[_currentIndex] : null;
		_isContinue = await _currentFormat.OnDecide(currentItem);
	}

	/// <summary>
	/// キャンセル入力の受付処理
	/// </summary>
	/// <returns></returns>
	private async UniTask AcceptCancel() {
		if (!Input.GetKeyDown(KeyCode.X)) return;

		if (_currentFormat == null ||
			_currentFormat.OnCancel == null) return;

		MenuListItem currentItem = IsEnableIndex(_useList, _currentIndex) ? _useList[_currentIndex] : null;
		_isContinue = await _currentFormat.OnCancel(currentItem);
	}

}
