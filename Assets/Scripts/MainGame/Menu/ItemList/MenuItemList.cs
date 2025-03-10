/**
 * @file MenuListItem.cs
 * @brief ���X�g���ڂ̊��N���X
 * @author yao
 * @date 2025/3/6
 */

using System.Collections.Generic;
using Cysharp.Threading.Tasks;

using static CommonModule;

public class MenuItemList : MenuList {

	public override async UniTask Initialize() {
		await base.Initialize();
		// �R�[���o�b�N�̐ݒ�
		var itemListFormat = new MenuListCallbackFortmat();
		itemListFormat.OnDecide = CloseItemList;
		itemListFormat.OnCancel = CloseItemList;
		SetCallbackFortmat( itemListFormat );
	}

	/// <summary>
	/// �A�C�e�����X�g�̓��͎�t���I������
	/// </summary>
	/// <param name="currentItem"></param>
	/// <returns></returns>
	private async UniTask<bool> CloseItemList( MenuListItem currentItem ) {
		await UniTask.CompletedTask;
		return false;
	}

	/// <summary>
	/// ���X�g���ڂ̐���
	/// </summary>
	/// <param name="itemList"></param>
	/// <returns></returns>
	public async UniTask Setup( List<int> itemList, MenuListCallbackFortmat callbackFormat ) {
		SetCallbackFortmat( callbackFormat );

		await SetIndex( -1 );
		RemoveAllItem();
		if (IsEmpty( itemList )) return;
		// ���ڂ̐���
		bool existItem = false;
		for (int i = 0, max = itemList.Count; i < max; i++) {
			if (itemList[i] < 0) break;
			// ���ڗL���̔���
			if (!existItem) existItem = true;
			// ���ڂ̐���
			var addItem = AddListItem() as MenuItemListItem;
			addItem.Setup( itemList[i] );
		}
		if (existItem) await SetIndex( 0 );

	}

}
