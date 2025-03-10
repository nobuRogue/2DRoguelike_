/**
 * @file ItemManager.cs
 * @brief �A�C�e���Ǘ�
 * @author yao
 * @date 2025/3/4
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CommonModule;

public class ItemManager : MonoBehaviour {
	[SerializeField]
	private ItemObject _itemObjectOrigin = null;

	[SerializeField]
	private Transform _useObjectRoot = null;
	[SerializeField]
	private Transform _unuseRoot = null;

	public static ItemManager instance { get; private set; } = null;
	// �g�p���̃A�C�e�����X�g
	private List<ItemBase> _useList = null;
	// ���g�p��Ԃ̃A�C�e�����X�g
	private List<List<ItemBase>> _unuseList = null;

	// �g�p���̃I�u�W�F�N�g���X�g
	private List<ItemObject> _useObjectList = null;
	// ���g�p��Ԃ̃I�u�W�F�N�g���X�g
	private List<ItemObject> _unuseObjectList = null;

	private readonly int _ITEM_MAX = 256;

	public void Initialize() {
		instance = this;
		ItemBase.SetGetObjectCallback(GetItemObject);
		// �A�C�e���f�[�^���̏�����
		_useList = new List<ItemBase>(_ITEM_MAX);
		int categoryMax = (int)eItemCategory.Max;
		_unuseList = new List<List<ItemBase>>(categoryMax);
		for (int i = 0; i < categoryMax; i++) {
			_unuseList.Add(new List<ItemBase>(_ITEM_MAX));
			for (int j = 0; j < _ITEM_MAX; j++) {
				// ���g�p��Ԃ̃A�C�e���ǉ�
				_unuseList[i].Add(CreateCategoryItem((eItemCategory)i));
			}
		}
		// �A�C�e���I�u�W�F�N�g���̏�����
		_useObjectList = new List<ItemObject>(_ITEM_MAX);
		_unuseObjectList = new List<ItemObject>(_ITEM_MAX);
		for (int i = 0; i < _ITEM_MAX; i++) {
			// ���g�p��Ԃ̃A�C�e���I�u�W�F�N�g�ǉ�
			_unuseObjectList.Add(Instantiate(_itemObjectOrigin, _unuseRoot));
		}
	}

	/// <summary>
	/// �A�C�e���J�e�S���ɕR�Â����N���X�𐶐�
	/// </summary>
	/// <param name="category"></param>
	/// <returns></returns>
	private ItemBase CreateCategoryItem(eItemCategory category) {
		switch (category) {
			case eItemCategory.Potion:
			return new ItemPotion();
			case eItemCategory.Food:
			return new ItemFood();
			case eItemCategory.Wand:
			return new ItemWand();
		}
		return null;
	}

	/// <summary>
	/// �A�C�e���I�u�W�F�N�g�擾
	/// </summary>
	/// <param name="ID"></param>
	/// <returns></returns>
	private ItemObject GetItemObject(int ID) {
		if (!IsEnableIndex(_useObjectList, ID)) return null;

		return _useObjectList[ID];
	}

	/// <summary>
	/// �A�C�e���f�[�^�̎擾
	/// </summary>
	/// <param name="ID"></param>
	/// <returns></returns>
	public ItemBase GetItemData(int ID) {
		if (!IsEnableIndex(_useList, ID)) return null;

		return _useList[ID];
	}

	/// <summary>
	/// �������A�C�e���̐���
	/// </summary>
	/// <param name="masterID"></param>
	/// <param name="square"></param>
	/// <returns></returns>
	public ItemBase UseFloorItem(int masterID, MapSquareData square) {
		// �g�p�\�ȃC���X�^���X�擾
		var itemMaster = ItemMasterUtility.GetItemMaster(masterID);
		if (itemMaster == null) return null;
		// �f�[�^�̐���
		int useID = UseItemData(itemMaster.category);
		// �I�u�W�F�N�g�̐���
		UseItemObject(useID);

		ItemBase useItem = GetItemData(useID);
		useItem.Setup(useID, masterID, square);
		return useItem;
	}

	/// <summary>
	/// �A�C�e���f�[�^�̎g�p��
	/// </summary>
	/// <param name="categoryIndex"></param>
	/// <returns></returns>
	private int UseItemData(int categoryIndex) {
		ItemBase useItem = GetUsableItemData(categoryIndex);
		// �g�p�\��ID���擾���Ďg�p���X�g�ɒǉ�
		int useID = -1;
		for (int i = 0, max = _useList.Count; i < max; i++) {
			if (_useList[i] != null) continue;

			_useList[i] = useItem;
			useID = i;
			break;
		}
		if (useID < 0) {
			useID = _useList.Count;
			_useList.Add(useItem);
		}
		return useID;
	}

	/// <summary>
	/// �A�C�e���I�u�W�F�N�g�̎g�p��
	/// </summary>
	/// <param name="useID"></param>
	public void UseItemObject(int useID) {
		ItemObject useObject = null;
		if (IsEmpty(_unuseObjectList)) {
			// ���g�p���X�g����Ȃ̂Ő���
			useObject = Instantiate(_itemObjectOrigin, _useObjectRoot);
		} else {
			// ���g�p������̂Ŏg�p
			useObject = _unuseObjectList[0];
			_unuseObjectList.RemoveAt(0);
			useObject.transform.SetParent(_useObjectRoot);
		}
		// �d�l�I�u�W�F�N�g���X�g�ւ̒ǉ�
		while (!IsEnableIndex(_useObjectList, useID)) _useObjectList.Add(null);

		_useObjectList[useID] = useObject;
	}

	/// <summary>
	/// �A�C�e���̖��g�p��
	/// </summary>
	/// <param name="ID"></param>
	public void UnuseItemData(int ID) {
		if (!IsEnableIndex(_useList, ID) || _useList[ID] == null) return;
		// �f�[�^�̖��g�p��
		ItemBase unuseItem = _useList[ID];
		_useList[ID] = null;
		unuseItem.Teardown();
		_unuseList[(int)unuseItem.GetCategory()].Add(unuseItem);
		// �I�u�W�F�N�g�̖��g�p��
		UnuseItemObject(ID);
	}

	/// <summary>
	/// �A�C�e���I�u�W�F�N�g�̖��g�p��
	/// </summary>
	/// <param name="ID"></param>
	public void UnuseItemObject(int ID) {
		if (!IsEnableIndex(_useObjectList, ID) || _useObjectList[ID] == null) return;
		// �I�u�W�F�N�g�̖��g�p��
		ItemObject unuseObject = _useObjectList[ID];
		_useObjectList[ID] = null;
		unuseObject.Teardown();
		_unuseObjectList.Add(unuseObject);
		unuseObject.transform.SetParent(_useObjectRoot);
	}

	/// <summary>
	/// �g�p�\�ȃA�C�e���f�[�^�̃C���X�^���X��n��
	/// </summary>
	/// <param name="category"></param>
	/// <returns></returns>
	private ItemBase GetUsableItemData(int categoryIndex) {
		List<ItemBase> targetList = _unuseList[categoryIndex];
		// ���g�p��Ԃ̃C���X�^���X��������ΐ������ĕԂ�
		if (IsEmpty(targetList)) return CreateCategoryItem((eItemCategory)categoryIndex);
		// ���g�p��Ԃ̃��X�g����1�Ԃ�
		ItemBase result = targetList[0];
		targetList.RemoveAt(0);
		return result;
	}

	/// <summary>
	/// �S�Ă̎g�p���A�C�e���Ɏw��̏��������s
	/// </summary>
	/// <param name="action"></param>
	public void ExecuteAllItem(System.Action<ItemBase> action) {
		if (action == null || IsEmpty(_useList)) return;

		for (int i = 0, max = _useList.Count; i < max; i++) {
			if (_useList[i] == null) continue;

			action(_useList[i]);
		}
	}

}
