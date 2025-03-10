/**
 * @file AcceptPlayerInput.cs
 * @brief プレイヤーの入力受付
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

		// コールバックの設定
		_itemListFormat = new MenuListCallbackFortmat();
		_itemListFormat.OnDecide = DecideItemList;
		_itemListFormat.OnCancel = CloseItemList;
		_itemListFormat.OnAfterAccept = AcceptSortItemList;
	}

	/// <summary>
	/// プレイヤー入力の受付
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
	/// 移動の受付
	/// </summary>
	/// <returns>移動したか否か</returns>
	public bool AcceptMove() {
		// 8方向の入力を受け付ける
		eDirectionEight inputDir = AcceptDirInput();
		if (!inputDir.IsSlant() && GetKey( KeyCode.LeftAlt )) inputDir = eDirectionEight.Invalid;

		if (inputDir == eDirectionEight.Invalid) return false;
		// 移動可否の判定
		PlayerCharacter player = GetPlayer();
		if (player == null) return false;

		player.SetDirection( inputDir );
		int playerX = player.positionX, playerY = player.positionY;
		MapSquareData playerSquare = MapSquareManager.instance.Get( playerX, playerY );
		MapSquareData moveSquare = GetToDirSquare( playerX, playerY, inputDir );
		if (!CanMove( playerX, playerY, moveSquare, inputDir )) return false;
		// 受け付けた入力に応じて移動
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
	/// 通常攻撃入力受付、効果処理
	/// </summary>
	/// <returns></returns>
	private async UniTask<bool> AcceptAttack() {
		if (!GetKeyDown( KeyCode.Z )) return false;

		await ActionManager.ExecuteAction( GetPlayer(), GameConst.NORMAL_ATTACK_ACTION_ID );
		return true;
	}

	/// <summary>
	/// 向き変更受付
	/// </summary>
	/// <returns></returns>
	private async UniTask AcceptDirChange() {
		MapSquareData forwardSquare = null;
		if (GetKeyDown( KeyCode.LeftShift )) ChangeDirToEnemy( ref forwardSquare );

		while (GetKey( KeyCode.LeftShift )) {
			// 8方向の入力を受け付ける
			eDirectionEight inputDir = AcceptDirInput();
			// 向きを変える
			ChangePlayerDir( inputDir, ref forwardSquare );
			await UniTask.DelayFrame( 1 );
		}
		// 今向いている方のマスの色消す
		forwardSquare?.HideMark();
	}

	/// <summary>
	/// プレイヤーの向きだけを変える状態処理
	/// </summary>
	/// <param name="inputDir"></param>
	/// <param name="forwardSquare"></param>
	private void ChangePlayerDir( eDirectionEight inputDir, ref MapSquareData forwardSquare ) {
		if (inputDir == eDirectionEight.Invalid) return;
		// 今向いている方のマスの色消す
		forwardSquare?.HideMark();
		// プレイヤーの向きを変える
		PlayerCharacter player = GetPlayer();
		player.SetDirection( inputDir );
		// プレイヤーが向いている先1マスを取得して色付ける
		MapSquareData playerSquare = GetCharacterSquare( player );
		forwardSquare = GetToDirSquare( playerSquare, player.direction );
		forwardSquare?.ShowMark( Color.red );
	}

	/// <summary>
	/// 周囲の敵にプレイヤーの向きを変更
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
		// 敵が見つからなかったのでプレイヤーの向きのマスを色変え
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
			// アイテム消費
			var itemMaster = ItemUtility.GetMasterDataFromID( _selectItemID );
			player.RemoveIDItem( _selectItemID );
			// アイテム使用効果
			await ActionManager.ExecuteAction( player, itemMaster.actionID );
			return true;
		}
		return false;
	}

	/// <summary>
	/// アイテムリストの入力受付を終了する
	/// </summary>
	/// <param name="currentItem"></param>
	/// <returns></returns>
	private async UniTask<bool> CloseItemList( MenuListItem currentItem ) {
		await UniTask.CompletedTask;
		return true;
	}

	/// <summary>
	/// 使用するアイテム決定
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
	/// ソートの受付
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
