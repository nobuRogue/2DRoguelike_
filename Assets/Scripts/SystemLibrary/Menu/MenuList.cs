/**
 * @file MenuList.cs
 * @brief ���X�g���j���[�̊��
 * @author yao
 * @date 2025/3/6
 */
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

using static CommonModule;

public class MenuList : MenuBase {
	/// <summary>
	/// ���X�g���ڂ̃I���W�i��
	/// </summary>
	[SerializeField]
	private MenuListItem _itemOrigin = null;
	/// <summary>
	/// ���ڂ���ׂ郋�[�g�I�u�W�F�N�g
	/// </summary>
	[SerializeField]
	private Transform _contentRoot = null;
	/// <summary>
	/// ���g�p��Ԃ̍��ڂ̃��[�g�I�u�W�F�N�g
	/// </summary>
	[SerializeField]
	private Transform _unuseRoot = null;

	/// <summary>
	/// ���X�g���j���[�̃R�[���o�b�N�W�N���X
	/// </summary>
	public class MenuListCallbackFortmat {
		// ���肳�ꂽ�ۂ̏���
		public System.Func<MenuListItem, UniTask<bool>> OnDecide = null;
		// �L�����Z�����ꂽ�ۂ̏���
		public System.Func<MenuListItem, UniTask<bool>> OnCancel = null;
		// �J�[�\�����ړ������ۂ̏���
		public System.Func<MenuListItem, MenuListItem, UniTask<bool>> OnMoveCursor = null;
		public System.Func<MenuListItem, UniTask<bool>> OnAfterAccept = null;
	}
	private MenuListCallbackFortmat _currentFormat = null;

	private int _currentIndex = -1;

	private List<MenuListItem> _useList = null;
	private List<MenuListItem> _unuseList = null;

	public override async UniTask Initialize() {
		await base.Initialize();
		_useList = new List<MenuListItem>();
		_unuseList = new List<MenuListItem>();
	}

	/// <summary>
	/// �R�[���o�b�N�p�N���X�̐ݒ�
	/// </summary>
	/// <param name="setFormat"></param>
	protected void SetCallbackFortmat( MenuListCallbackFortmat setFormat ) {
		_currentFormat = setFormat;
	}

	/// <summary>
	/// ���X�g���ڂ̐���
	/// </summary>
	/// <returns></returns>
	protected MenuListItem AddListItem() {
		MenuListItem addItem;
		if (IsEmpty( _unuseList )) {
			// ���g�p���X�g����Ȃ̂Ő���
			addItem = Instantiate( _itemOrigin, _contentRoot );
		} else {
			// ���g�p���X�g����擾
			addItem = _unuseList[0];
			_unuseList.RemoveAt( 0 );
			addItem.transform.SetParent( _contentRoot );
		}
		addItem.Deselect();
		_useList.Add( addItem );
		return addItem;
	}

	/// <summary>
	/// �C���f�N�X�w��̃��X�g���ڍ폜
	/// </summary>
	/// <param name="itemIndex"></param>
	protected void RemoveListItem( int itemIndex ) {
		if (!IsEnableIndex( _useList, itemIndex )) return;
		// �g�p���X�g�����菜��
		MenuListItem removeItem = _useList[itemIndex];
		_useList.RemoveAt( itemIndex );
		// ���g�p���X�g�֒ǉ�
		_unuseList.Add( removeItem );
		removeItem.transform.SetParent( _unuseRoot );
		removeItem.Deselect();
	}

	/// <summary>
	/// �S�Ẵ��X�g���ڍ폜
	/// </summary>
	protected void RemoveAllItem() {
		while (!IsEmpty( _useList )) RemoveListItem( 0 );

	}

	/// <summary>
	/// ���X�g�̓��͎�t�^�X�N
	/// </summary>
	/// <returns></returns>
	public async UniTask AcceptInput() {
		while (true) {
			// �J�[�\���̈ړ���t
			if (await AcceptMoveCursor()) break;
			// ����̓��͎�t
			if (await AcceptDecide()) break;
			// �L�����Z���̓��͎�t
			if (await AcceptCancel()) break;
			// ���R��t
			if (await AcceptFree()) break;

			await UniTask.DelayFrame( 1 );
		}
	}

	/// <summary>
	/// �J�[�\���ړ����͂̎�t����
	/// </summary>
	/// <returns></returns>
	private async UniTask<bool> AcceptMoveCursor() {
		// �l�����̓��͎�t
		eDirectionFour inputDir = GetDirInput();
		if (inputDir == eDirectionFour.Invalid) return false;
		// ���͂ɉ������C���f�N�X�̕ύX
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
		// �ړ���̃C���f�N�X�����X�g���ڐ��Ɏ��܂�悤�ɏC��
		if (moveIndex >= _useList.Count) moveIndex -= _useList.Count;

		if (moveIndex < 0) moveIndex += _useList.Count;
		// �J�[�\���ړ����̏���
		return await SetIndex( moveIndex );
	}

	/// <summary>
	/// �l�����̓��͎�t
	/// </summary>
	/// <returns></returns>
	private eDirectionFour GetDirInput() {
		if (Input.GetKeyDown( KeyCode.UpArrow )) {
			return eDirectionFour.Up;
		} else if (Input.GetKeyDown( KeyCode.RightArrow )) {
			return eDirectionFour.Right;
		} else if (Input.GetKeyDown( KeyCode.DownArrow )) {
			return eDirectionFour.Down;
		} else if (Input.GetKeyDown( KeyCode.LeftArrow )) {
			return eDirectionFour.Left;
		}
		return eDirectionFour.Invalid;
	}

	/// <summary>
	/// �I�𒆍��ڂ̕ύX
	/// </summary>
	/// <param name="setIndex"></param>
	/// <returns></returns>
	protected async UniTask<bool> SetIndex( int setIndex ) {
		if (_currentIndex == setIndex) return false;
		// ���݂̍��ڂ𖢑I����Ԃɂ���
		MenuListItem prevItem;
		if (IsEnableIndex( _useList, _currentIndex )) {
			prevItem = _useList[_currentIndex];
			prevItem.Deselect();
		} else {
			prevItem = null;
		}
		_currentIndex = setIndex;
		if (!IsEnableIndex( _useList, _currentIndex )) return false;
		// �ړ���̍��ڂ�I����Ԃɂ���
		MenuListItem currentItem = _useList[_currentIndex];
		currentItem.Select();
		// �J�[�\���ړ��R�[���o�b�N�̎��s
		if (_currentFormat == null ||
			_currentFormat.OnMoveCursor == null) return false;

		return await _currentFormat.OnMoveCursor( currentItem, prevItem );
	}

	/// <summary>
	/// ������͂̎�t����
	/// </summary>
	/// <returns></returns>
	private async UniTask<bool> AcceptDecide() {
		if (!Input.GetKeyDown( KeyCode.Z )) return false;

		if (_currentFormat == null ||
			_currentFormat.OnDecide == null) return false;

		MenuListItem currentItem = IsEnableIndex( _useList, _currentIndex ) ? _useList[_currentIndex] : null;
		return await _currentFormat.OnDecide( currentItem );
	}

	/// <summary>
	/// �L�����Z�����͂̎�t����
	/// </summary>
	/// <returns></returns>
	private async UniTask<bool> AcceptCancel() {
		if (!Input.GetKeyDown( KeyCode.X )) return false;

		if (_currentFormat == null ||
			_currentFormat.OnCancel == null) return false;

		MenuListItem currentItem = IsEnableIndex( _useList, _currentIndex ) ? _useList[_currentIndex] : null;
		return await _currentFormat.OnCancel( currentItem );
	}

	/// <summary>
	/// �L�����Z�����͂̎�t����
	/// </summary>
	/// <returns></returns>
	private async UniTask<bool> AcceptFree() {
		if (_currentFormat == null ||
			_currentFormat.OnAfterAccept == null) return false;

		MenuListItem currentItem = IsEnableIndex( _useList, _currentIndex ) ? _useList[_currentIndex] : null;
		return await _currentFormat.OnAfterAccept( currentItem );
	}

}
