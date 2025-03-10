/**
 * @file MapSquareUtility.cs
 * @brief �}�X�֘A���s����
 * @author yao
 * @date 2025/1/21
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CommonModule;

public class MapSquareUtility {

	/// <summary>
	/// �w������̃}�X�擾
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="dir"></param>
	/// <returns></returns>
	public static MapSquareData GetToDirSquare(int x, int y, eDirectionFour dir) {
		ToVectorPos(ref x, ref y, dir);
		return MapSquareManager.instance.Get(x, y);
	}

	/// <summary>
	/// �w����W�}�X����̎w������̃}�X�擾
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="dir"></param>
	/// <returns></returns>
	public static MapSquareData GetToDirSquare(int x, int y, eDirectionEight dir) {
		ToVectorPos(ref x, ref y, dir);
		return MapSquareManager.instance.Get(x, y);
	}

	/// <summary>
	/// �w��}�X����̎w������̃}�X�擾
	/// </summary>
	/// <param name="baseSquare"></param>
	/// <param name="dir"></param>
	/// <returns></returns>
	public static MapSquareData GetToDirSquare(MapSquareData baseSquare, eDirectionEight dir) {
		if (baseSquare == null) return null;

		return GetToDirSquare(baseSquare.positionX, baseSquare.positionY, dir);
	}

	/// <summary>
	/// �ړ��ۂ̔���
	/// </summary>
	/// <param name="startX"></param>
	/// <param name="startY"></param>
	/// <param name="moveSquare"></param>
	/// <param name="dir"></param>
	/// <returns></returns>
	public static bool CanMove(int startX, int startY, MapSquareData moveSquare, eDirectionEight dir) {
		return CanMoveTerrain(startX, startY, moveSquare, dir) && !moveSquare.existCharacter;
	}

	public static bool CanMoveTerrain(int startX, int startY, MapSquareData moveSquare, eDirectionEight dir) {
		if (moveSquare == null ||
			moveSquare.terrain == eTerrain.Wall) return false;
		// �΂߈ړ����ۂ�
		if (!dir.IsSlant()) return true;
		// �΂߈ړ��Ȃ�A�����𕪊����e�����̃}�X���`�F�b�N
		eDirectionFour[] separateDir = dir.Separate();
		for (int i = 0, max = separateDir.Length; i < max; i++) {
			MapSquareData checkSquare = GetToDirSquare(startX, startY, separateDir[i]);
			if (checkSquare == null ||
				checkSquare.terrain == eTerrain.Wall) return false;

		}
		return true;
	}

	/// <summary>
	/// �U���\�ȃ}�X��
	/// </summary>
	/// <param name="startX"></param>
	/// <param name="startY"></param>
	/// <param name="attackSquare"></param>
	/// <param name="dir"></param>
	/// <returns></returns>
	public static bool CanAttack(int startX, int startY, MapSquareData attackSquare, eDirectionEight dir) {
		if (attackSquare == null ||
			attackSquare.terrain == eTerrain.Wall) return false;
		// �U���������΂߂ł͂Ȃ��̂ōU���悪�ǂłȂ����_�ōU���\
		if (!dir.IsSlant()) return true;
		// �΂ߕ����Ȃ�A�����𕪊����e�����̃}�X���`�F�b�N
		eDirectionFour[] separateDir = dir.Separate();
		for (int i = 0, max = separateDir.Length; i < max; i++) {
			MapSquareData checkSquare = GetToDirSquare(startX, startY, separateDir[i]);
			if (checkSquare == null ||
				checkSquare.terrain == eTerrain.Wall) return false;

		}
		// �΂ߕ����ŕ��������}�X���ǂ�����ǂł͂Ȃ��̂ōU���\
		return true;
	}

	public static void GetVisibleArea(ref List<int> visibleArea, MapSquareData sourceSquare) {
		InitializeList(ref visibleArea);
		if (sourceSquare == null) return;
		// ����8�}�X���擾
		GetChebyshevAroundSquare(ref visibleArea, sourceSquare);
		visibleArea.Add(sourceSquare.ID);
		// ����8�}�X�����g�̃}�X�ɕ���������Ύ擾
		List<int> aroundRoomList = new List<int>(visibleArea.Count);
		PlayerCharacter player = CharacterManager.instance.GetPlayer();
		for (int i = 0, max = visibleArea.Count; i < max; i++) {
			MapSquareData targetSquare = MapSquareManager.instance.Get(visibleArea[i]);
			if (targetSquare == null ||
				targetSquare.roomID < 0) continue;

			if (aroundRoomList.Exists(roomID => roomID == targetSquare.roomID)) continue;

			aroundRoomList.Add(targetSquare.roomID);
		}
		// �אڂ��Ă��镔���̑S�}�X�擾
		for (int i = 0, max = aroundRoomList.Count; i < max; i++) {
			RoomData roomData = MapSquareManager.instance.GetRoom(aroundRoomList[i]);
			if (roomData == null) continue;

			MeargeList(ref visibleArea, roomData.squareIDList);
		}
	}

	/// <summary>
	/// ���`�F�r�V�F�t�����̃}�X��S�Ď擾
	/// </summary>
	/// <param name="result"></param>
	/// <param name="sourceSquare"></param>
	/// <param name="distance"></param>
	public static void GetChebyshevAroundSquare(ref List<int> result, MapSquareData sourceSquare, int distance = 1) {
		InitializeList(ref result, distance * 8);
		if (sourceSquare == null) return;

		if (distance == 0) {
			result.Add(sourceSquare.ID);
			return;
		}

		int countMax = distance * 2;
		int sourceX = sourceSquare.positionX;
		int sourceY = sourceSquare.positionY;
		for (int count = 0; count < countMax; count++) {
			MapSquareData targetSquare = MapSquareManager.instance.Get(sourceX - distance + count, sourceY - distance);
			if (targetSquare != null) result.Add(targetSquare.ID);

			targetSquare = MapSquareManager.instance.Get(sourceX + distance, sourceY - distance + count);
			if (targetSquare != null) result.Add(targetSquare.ID);

			targetSquare = MapSquareManager.instance.Get(sourceX + distance - count, sourceY + distance);
			if (targetSquare != null) result.Add(targetSquare.ID);

			targetSquare = MapSquareManager.instance.Get(sourceX - distance, sourceY + distance - count);
			if (targetSquare != null) result.Add(targetSquare.ID);

		}
	}

	/// <summary>
	/// ID�w��̃}�X�̎擾
	/// </summary>
	/// <param name="ID"></param>
	/// <returns></returns>
	public static MapSquareData GetSquare(int ID) {
		return MapSquareManager.instance.Get(ID);
	}

	/// <summary>
	/// ���W�w��̃}�X�擾
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	public static MapSquareData GetSquare(int x, int y) {
		return MapSquareManager.instance.Get(x, y);
	}

	/// <summary>
	/// �L�����̋���}�X�擾
	/// </summary>
	/// <param name="character"></param>
	/// <returns></returns>
	public static MapSquareData GetCharacterSquare(CharacterBase character) {
		if (character == null) return null;

		return GetSquare(character.positionX, character.positionY);
	}

}
