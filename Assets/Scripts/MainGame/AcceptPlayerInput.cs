/**
 * @file AcceptPlayerInput.cs
 * @brief �v���C���[�̓��͎�t
 * @author yao
 * @date 2025/1/21
 */

using Cysharp.Threading.Tasks;
using UnityEngine;

using static ItemUtility;
using static MenuList;
using static MapSquareUtility;
using static CharacterUtility;
using static UnityEngine.Input;

public class AcceptPlayerInput {
	private System.Action<MoveAction> _AddMove = null;

	MenuListCallbackFortmat _itemListFormat = null;

	public void Initialize( System.Action<MoveAction> setProcess ) {
		_AddMove = setProcess;

		// �R�[���o�b�N�̐ݒ�
		_itemListFormat = new MenuListCallbackFortmat();
		_itemListFormat.OnDecide = DecideItemList;
		_itemListFormat.OnCancel = CloseItemList;
		_itemListFormat.OnAfterAccept = AcceptSortItemList;
	}

	/// <summary>
	/// �v���C���[���͂̎�t
	/// </summary>
	/// <returns></returns>
	public async UniTask AcceptInput() {
		while (true) {
			if (AcceptMove()) break;

			if (await AcceptAttack()) break;

			if (await AcceptItemList()) break;

			await AcceptDirChange();
			await UniTask.DelayFrame( 1 );
		}
	}

	/// <summary>
	/// �ړ��̎�t
	/// </summary>
	/// <returns>�ړ��������ۂ�</returns>
	public bool AcceptMove() {
		// 8�����̓��͂��󂯕t����
		eDirectionEight inputDir = AcceptDirInput();
		if (!inputDir.IsSlant() && GetKey( KeyCode.LeftAlt )) inputDir = eDirectionEight.Invalid;

		if (inputDir == eDirectionEight.Invalid) return false;
		// �ړ��ۂ̔���
		PlayerCharacter player = GetPlayer();
		if (player == null) return false;

		player.SetDirection( inputDir );
		int playerX = player.positionX, playerY = player.positionY;
		MapSquareData playerSquare = MapSquareManager.instance.Get( playerX, playerY );
		MapSquareData moveSquare = GetToDirSquare( playerX, playerY, inputDir );
		if (!CanMove( playerX, playerY, moveSquare, inputDir )) return false;
		// �󂯕t�������͂ɉ����Ĉړ�
		MoveAction moveAction = new MoveAction();
		var moveData = new ChebyshevMoveData( playerSquare.ID, moveSquare.ID, inputDir );
		moveAction.ProcessData( player, moveData );
		_AddMove( moveAction );
		return true;
	}

	private eDirectionEight AcceptDirInput() {
		if (GetKey( KeyCode.UpArrow )) {
			if (GetKey( KeyCode.RightArrow )) {
				return eDirectionEight.UpRight;
			} else if (GetKey( KeyCode.LeftArrow )) {
				return eDirectionEight.UpLeft;
			} else {
				return eDirectionEight.Up;
			}
		} else if (GetKey( KeyCode.DownArrow )) {
			if (GetKey( KeyCode.RightArrow )) {
				return eDirectionEight.DownRight;
			} else if (GetKey( KeyCode.LeftArrow )) {
				return eDirectionEight.DownLeft;
			} else {
				return eDirectionEight.Down;
			}
		} else {
			if (GetKey( KeyCode.RightArrow )) {
				return eDirectionEight.Right;
			} else if (GetKey( KeyCode.LeftArrow )) {
				return eDirectionEight.Left;
			}
		}
		return eDirectionEight.Invalid;
	}

	/// <summary>
	/// �ʏ�U�����͎�t�A���ʏ���
	/// </summary>
	/// <returns></returns>
	private async UniTask<bool> AcceptAttack() {
		if (!GetKeyDown( KeyCode.Z )) return false;

		await ActionManager.ExecuteAction( GetPlayer(), GameConst.NORMAL_ATTACK_ACTION_ID );
		return true;
	}

	/// <summary>
	/// �����ύX��t
	/// </summary>
	/// <returns></returns>
	private async UniTask AcceptDirChange() {
		MapSquareData forwardSquare = null;
		if (GetKeyDown( KeyCode.LeftShift )) ChangeDirToEnemy( ref forwardSquare );

		while (GetKey( KeyCode.LeftShift )) {
			// 8�����̓��͂��󂯕t����
			eDirectionEight inputDir = AcceptDirInput();
			// ������ς���
			ChangePlayerDir( inputDir, ref forwardSquare );
			await UniTask.DelayFrame( 1 );
		}
		// �������Ă�����̃}�X�̐F����
		forwardSquare?.HideMark();
	}

	/// <summary>
	/// �v���C���[�̌���������ς����ԏ���
	/// </summary>
	/// <param name="inputDir"></param>
	/// <param name="forwardSquare"></param>
	private void ChangePlayerDir( eDirectionEight inputDir, ref MapSquareData forwardSquare ) {
		if (inputDir == eDirectionEight.Invalid) return;
		// �������Ă�����̃}�X�̐F����
		forwardSquare?.HideMark();
		// �v���C���[�̌�����ς���
		PlayerCharacter player = GetPlayer();
		player.SetDirection( inputDir );
		// �v���C���[�������Ă����1�}�X���擾���ĐF�t����
		MapSquareData playerSquare = GetCharacterSquare( player );
		forwardSquare = GetToDirSquare( playerSquare, player.direction );
		forwardSquare?.ShowMark( Color.red );
	}

	/// <summary>
	/// ���͂̓G�Ƀv���C���[�̌�����ύX
	/// </summary>
	private void ChangeDirToEnemy( ref MapSquareData forwardSquare ) {
		PlayerCharacter player = GetPlayer();
		MapSquareData playerSquare = GetCharacterSquare( player );
		int startIndex = (int)player.direction + 1;
		for (int i = 0, max = (int)eDirectionEight.Max; i < max; i++) {
			var dir = (startIndex + i).ToDirEight();
			MapSquareData square = GetToDirSquare( playerSquare, dir );
			if (square == null ||
				!square.existCharacter) continue;

			ChangePlayerDir( dir, ref forwardSquare );
			return;
		}
		// �G��������Ȃ������̂Ńv���C���[�̌����̃}�X��F�ς�
		MapSquareData playerDirSquare = GetToDirSquare( playerSquare, player.direction );
		if (playerDirSquare == null) return;

		playerDirSquare.ShowMark( Color.red );
		forwardSquare = playerDirSquare;
	}

	private int _selectItemID = -1;

	private async UniTask<bool> AcceptItemList() {
		if (!GetKeyDown( KeyCode.C )) return false;

		_selectItemID = -1;
		var itemList = MenuManager.instance.Get<MenuItemList>();
		PlayerCharacter player = GetPlayer();
		await itemList.Setup( player.possessItemList, _itemListFormat );
		await itemList.Open();
		await itemList.AcceptInput();
		await itemList.Close();

		if (_selectItemID >= 0) {
			// �A�C�e������
			var itemMaster = ItemUtility.GetMasterDataFromID( _selectItemID );
			player.RemoveIDItem( _selectItemID );
			// �A�C�e���g�p����
			await ActionManager.ExecuteAction( player, itemMaster.actionID );
			return true;
		}
		return false;
	}

	/// <summary>
	/// �A�C�e�����X�g�̓��͎�t���I������
	/// </summary>
	/// <param name="currentItem"></param>
	/// <returns></returns>
	private async UniTask<bool> CloseItemList( MenuListItem currentItem ) {
		await UniTask.CompletedTask;
		return true;
	}

	/// <summary>
	/// �g�p����A�C�e������
	/// </summary>
	/// <param name="currentItem"></param>
	/// <returns></returns>
	private async UniTask<bool> DecideItemList( MenuListItem currentItem ) {
		MenuItemListItem itemListItem = currentItem as MenuItemListItem;
		_selectItemID = itemListItem.itemID;
		await UniTask.CompletedTask;
		return true;
	}

	/// <summary>
	/// �\�[�g�̎�t
	/// </summary>
	/// <param name="currentItem"></param>
	/// <returns></returns>
	private async UniTask<bool> AcceptSortItemList( MenuListItem currentItem ) {
		if (!GetKeyDown( KeyCode.V )) return false;

		PlayerCharacter player = GetPlayer();
		player.possessItemList.Sort( ItemSortMehtod );
		await MenuManager.instance.Get<MenuItemList>().Setup( player.possessItemList, _itemListFormat );
		return false;
	}

	public static int ItemSortMehtod( int idA, int idB ) {
		ItemBase dataA = GetItemData( idA ), dataB = GetItemData( idB );

		int aScore = dataA.masterID;
		int bScore = dataB.masterID;

		return aScore - bScore;
	}

}
