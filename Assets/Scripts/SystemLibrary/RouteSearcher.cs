/**
 * @file RouteSearcher.cs
 * @brief �o�H�T���N���X
 * @author yao
 * @date 2025/1/16
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static GameConst;
using static CommonModule;

public class RouteSearcher {

	private abstract class DistanceNode {

		public int distance { get; private set; } = -1;
		public int squareID { get; private set; } = -1;

		public DistanceNode(int setDistance, int setSquareID) {
			distance = setDistance;
			squareID = setSquareID;
		}
		/// <summary>
		/// �S�[������̋������X�R�A�Ƃ��ĕԂ�
		/// </summary>
		/// <param name="goalX"></param>
		/// <param name="goalY"></param>
		/// <returns></returns>
		public abstract int GetScore(int goalX, int goalY);
	}

	#region Manhattan

	private class DistanceNodeManhattan : DistanceNode {
		public eDirectionFour dir;
		public DistanceNodeManhattan prevNode = null;
		public DistanceNodeManhattan(eDirectionFour setDir, DistanceNodeManhattan setPrevNode, int setDistance, int setSquareID) : base(setDistance, setSquareID) {
			dir = setDir;
			prevNode = setPrevNode;
		}

		/// <summary>
		/// �S�[������̋������X�R�A�Ƃ��ĕԂ�
		/// </summary>
		/// <param name="goalX"></param>
		/// <param name="goalY"></param>
		/// <returns></returns>
		public override int GetScore(int goalX, int goalY) {
			MapSquareData square = MapSquareManager.instance.Get(squareID);
			int diffX = Mathf.Abs(square.positionX - goalX);
			int diffY = Mathf.Abs(square.positionY - goalY);
			return diffX + diffY;
		}
	}

	private class DistanceNodeTableManhattan {
		public DistanceNodeManhattan goalNode = null;
		public List<DistanceNodeManhattan> nodeList = null;
		public DistanceNodeTableManhattan() {
			nodeList = new List<DistanceNodeManhattan>(MAP_SQUARE_HEIGHT_COUNT * MAP_SQUARE_WIDTH_COUNT);
		}

		public void Clear() {
			goalNode = null;
			nodeList.Clear();
		}
	}

	private static DistanceNodeTableManhattan _nodeTableManhattan = null;
	private static List<DistanceNodeManhattan> _manhattanOpenList = null;

	/// <summary>
	/// 4�����̌o�H�T��
	/// </summary>
	/// <param name="startSquareID"></param>
	/// <param name="goalSquareID"></param>
	/// <param name="CanPass"></param>
	/// <returns></returns>
	public static List<ManhattanMoveData> RouteSearch(int startSquareID, int goalSquareID,
		System.Func<MapSquareData, eDirectionFour, int, bool> CanPass) {
		// �S�[���m�[�h��������܂Ńm�[�h���J���Ă���
		OpenNodeToGoalManhattan(startSquareID, goalSquareID, CanPass);
		// �S�[���m�[�h����X�^�[�g�܂ők���Čo�H�𐶐�
		return CreateRouteManhattan();
	}

	private static void OpenNodeToGoalManhattan(int startSquareID, int goalSquareID,
		System.Func<MapSquareData, eDirectionFour, int, bool> CanPass) {
		if (_nodeTableManhattan == null) {
			_nodeTableManhattan = new DistanceNodeTableManhattan();
		} else {
			_nodeTableManhattan.Clear();
		}
		InitializeList(ref _manhattanOpenList, MAP_SQUARE_HEIGHT_COUNT * MAP_SQUARE_WIDTH_COUNT);
		// �X�^�[�g�}�X�̃m�[�h�𐶐����ăI�[�v�����X�g�ɉ�����
		_manhattanOpenList.Add(new DistanceNodeManhattan(eDirectionFour.Invalid, null, 0, startSquareID));
		// �S�[���}�X�̈ʒu���擾���Ă���
		MapSquareData goalSquare = MapSquareManager.instance.Get(goalSquareID);
		int goalX = goalSquare.positionX, goalY = goalSquare.positionY;
		while (_nodeTableManhattan.goalNode == null) {
			// �X�R�A�ŏ��̃m�[�h���擾
			var minScoreNode = GetMinScoreNodeManhattan(goalX, goalY);
			if (minScoreNode == null) break;
			// �X�R�A�ŏ��̃m�[�h�̎��͂��I�[�v������
			OpenNodeAroundManhattan(minScoreNode, goalSquareID, CanPass);
			_manhattanOpenList.Remove(minScoreNode);
		}
	}

	/// <summary>
	/// �ŏ��X�R�A�̃m�[�h���擾
	/// </summary>
	/// <param name="goalX"></param>
	/// <param name="goalY"></param>
	/// <returns></returns>
	private static DistanceNodeManhattan GetMinScoreNodeManhattan(int goalX, int goalY) {
		if (IsEmpty(_manhattanOpenList)) return null;

		DistanceNodeManhattan minScoreNode = null;
		int minScore = -1;
		for (int i = 0, max = _manhattanOpenList.Count; i < max; i++) {
			DistanceNodeManhattan currentNode = _manhattanOpenList[i];
			if (currentNode == null) continue;

			int currentScore = currentNode.GetScore(goalX, goalY);
			if (minScoreNode == null || minScore > currentScore) {
				minScoreNode = currentNode;
				minScore = currentScore;
			}
		}
		return minScoreNode;
	}

	/// <summary>
	/// ��m�[�h�̎���4�}�X���I�[�v������
	/// </summary>
	/// <param name="baseNode"></param>
	/// <param name="goalSquareID"></param>
	/// <param name="CanPass"></param>
	private static void OpenNodeAroundManhattan(DistanceNodeManhattan baseNode, int goalSquareID,
		System.Func<MapSquareData, eDirectionFour, int, bool> CanPass) {
		if (baseNode == null) return;

		MapSquareData baseSquare = MapSquareManager.instance.Get(baseNode.squareID);
		int baseX = baseSquare.positionX, baseY = baseSquare.positionY;
		// ����4�}�X���I�[�v������
		for (int i = (int)eDirectionFour.Up, max = (int)eDirectionFour.Max; i < max; i++) {
			var dir = (eDirectionFour)i;
			MapSquareData openSquare = MapSquareUtility.GetToDirSquare(baseX, baseY, dir);
			if (openSquare == null) continue;
			// ����1�x�I�[�v�����ꂽ�m�[�h�Ȃ珈�����Ȃ�
			if (_nodeTableManhattan.nodeList.Exists(node => node.squareID == openSquare.ID)) continue;
			// �ʍs�۔���
			int distance = baseNode.distance + 1;
			if (!CanPass(openSquare, dir, distance)) continue;

			DistanceNodeManhattan addNode = new DistanceNodeManhattan(dir, baseNode, distance, openSquare.ID);
			_nodeTableManhattan.nodeList.Add(addNode);
			_manhattanOpenList.Add(addNode);
			// �S�[������
			if (openSquare.ID != goalSquareID) continue;

			_nodeTableManhattan.goalNode = addNode;
			return;
		}
	}

	/// <summary>
	/// �o�H����
	/// </summary>
	/// <returns></returns>
	private static List<ManhattanMoveData> CreateRouteManhattan() {
		if (_nodeTableManhattan == null || _nodeTableManhattan.goalNode == null) return null;

		int routeCount = _nodeTableManhattan.goalNode.distance;
		List<ManhattanMoveData> result = new List<ManhattanMoveData>(routeCount);
		for (int i = 0; i < routeCount; i++) {
			result.Add(null);
		}
		// �S�[������k���Čo�H����
		DistanceNodeManhattan currentNode = _nodeTableManhattan.goalNode;
		for (int i = routeCount - 1; i >= 0; i--) {
			var moveData = new ManhattanMoveData(currentNode.prevNode.squareID, currentNode.squareID, currentNode.dir);
			result[i] = moveData;
			currentNode = currentNode.prevNode;
		}
		return result;
	}

	#endregion

	private class DistanceNodeChebyshev : DistanceNode {
		public eDirectionEight dir;
		public DistanceNodeChebyshev prevNode = null;
		public DistanceNodeChebyshev(eDirectionEight setDir, DistanceNodeChebyshev setPrevNode, int setDistance, int setSquareID) : base(setDistance, setSquareID) {
			dir = setDir;
			prevNode = setPrevNode;
		}

		/// <summary>
		/// �S�[������̋������X�R�A�Ƃ��ĕԂ�
		/// </summary>
		/// <param name="goalX"></param>
		/// <param name="goalY"></param>
		/// <returns></returns>
		public override int GetScore(int goalX, int goalY) {
			MapSquareData square = MapSquareManager.instance.Get(squareID);
			int diffX = Mathf.Abs(square.positionX - goalX);
			int diffY = Mathf.Abs(square.positionY - goalY);
			int score = Mathf.Max(diffX * MAP_SQUARE_WIDTH_COUNT, diffY * MAP_SQUARE_HEIGHT_COUNT);
			score += Mathf.Min(diffX, diffY);
			return score;
		}
	}

	private class DistanceNodeTableChebyshev {
		public DistanceNodeChebyshev goalNode = null;
		public List<DistanceNodeChebyshev> nodeList = null;
		public DistanceNodeTableChebyshev() {
			nodeList = new List<DistanceNodeChebyshev>(MAP_SQUARE_HEIGHT_COUNT * MAP_SQUARE_WIDTH_COUNT);
		}

		public void Clear() {
			goalNode = null;
			nodeList.Clear();
		}
	}

	private static DistanceNodeTableChebyshev _nodeTableChebyshev = null;
	private static List<DistanceNodeChebyshev> _chebyshevOpenList = null;

	/// <summary>
	/// 8�����̌o�H�T��
	/// </summary>
	/// <param name="startSquareID"></param>
	/// <param name="goalSquareID"></param>
	/// <param name="CanPass"></param>
	/// <returns></returns>
	public static List<ChebyshevMoveData> RouteSearch(int startSquareID, int goalSquareID,
		System.Func<MapSquareData, MapSquareData, eDirectionEight, int, bool> CanPass) {
		// �S�[���m�[�h��������܂Ńm�[�h���J���Ă���
		OpenNodeToGoalChebyshev(startSquareID, goalSquareID, CanPass);
		// �S�[���m�[�h����X�^�[�g�܂ők���Čo�H�𐶐�
		return CreateRouteChebyshev();
	}

	private static void OpenNodeToGoalChebyshev(int startSquareID, int goalSquareID,
	System.Func<MapSquareData, MapSquareData, eDirectionEight, int, bool> CanPass) {
		if (_nodeTableChebyshev == null) {
			_nodeTableChebyshev = new DistanceNodeTableChebyshev();
		} else {
			_nodeTableChebyshev.Clear();
		}
		InitializeList(ref _chebyshevOpenList, MAP_SQUARE_HEIGHT_COUNT * MAP_SQUARE_WIDTH_COUNT);
		// �X�^�[�g�}�X�̃m�[�h�𐶐����ăI�[�v�����X�g�ɉ�����
		_chebyshevOpenList.Add(new DistanceNodeChebyshev(eDirectionEight.Invalid, null, 0, startSquareID));
		// �S�[���}�X�̈ʒu���擾���Ă���
		MapSquareData goalSquare = MapSquareManager.instance.Get(goalSquareID);
		int goalX = goalSquare.positionX, goalY = goalSquare.positionY;
		while (_nodeTableChebyshev.goalNode == null) {
			// �X�R�A�ŏ��̃m�[�h���擾
			var minScoreNode = GetMinScoreNodeChebyshev(goalX, goalY);
			if (minScoreNode == null) break;
			// �X�R�A�ŏ��̃m�[�h�̎��͂��I�[�v������
			OpenNodeAroundChebyshev(minScoreNode, goalSquareID, CanPass);
			_chebyshevOpenList.Remove(minScoreNode);
		}
	}

	/// <summary>
	/// �ŏ��X�R�A�̃m�[�h���擾
	/// </summary>
	/// <param name="goalX"></param>
	/// <param name="goalY"></param>
	/// <returns></returns>
	private static DistanceNodeChebyshev GetMinScoreNodeChebyshev(int goalX, int goalY) {
		if (IsEmpty(_chebyshevOpenList)) return null;

		DistanceNodeChebyshev minScoreNode = null;
		int minScore = -1;
		for (int i = 0, max = _chebyshevOpenList.Count; i < max; i++) {
			DistanceNodeChebyshev currentNode = _chebyshevOpenList[i];
			if (currentNode == null) continue;

			int currentScore = currentNode.GetScore(goalX, goalY);
			if (minScoreNode == null || minScore > currentScore) {
				minScoreNode = currentNode;
				minScore = currentScore;
			}
		}
		return minScoreNode;
	}

	/// <summary>
	/// ��m�[�h�̎���8�}�X���I�[�v������
	/// </summary>
	/// <param name="baseNode"></param>
	/// <param name="goalSquareID"></param>
	/// <param name="CanPass"></param>
	private static void OpenNodeAroundChebyshev(DistanceNodeChebyshev baseNode, int goalSquareID,
		System.Func<MapSquareData, MapSquareData, eDirectionEight, int, bool> CanPass) {
		if (baseNode == null) return;

		MapSquareData baseSquare = MapSquareManager.instance.Get(baseNode.squareID);
		// ����4�}�X���I�[�v������
		for (int i = (int)eDirectionEight.Up, max = (int)eDirectionEight.Max; i < max; i++) {
			var dir = (eDirectionEight)i;
			MapSquareData openSquare = MapSquareUtility.GetToDirSquare(baseSquare, dir);
			if (openSquare == null) continue;
			// ����1�x�I�[�v�����ꂽ�m�[�h�Ȃ珈�����Ȃ�
			if (_nodeTableChebyshev.nodeList.Exists(node => node.squareID == openSquare.ID)) continue;
			// �ʍs�۔���
			int distance = baseNode.distance + 1;
			if (!CanPass(baseSquare, openSquare, dir, distance)) continue;

			DistanceNodeChebyshev addNode = new DistanceNodeChebyshev(dir, baseNode, distance, openSquare.ID);
			_nodeTableChebyshev.nodeList.Add(addNode);
			_chebyshevOpenList.Add(addNode);
			// �S�[������
			if (openSquare.ID != goalSquareID) continue;

			_nodeTableChebyshev.goalNode = addNode;
			return;
		}
	}

	/// <summary>
	/// �o�H����
	/// </summary>
	/// <returns></returns>
	private static List<ChebyshevMoveData> CreateRouteChebyshev() {
		if (_nodeTableChebyshev == null || _nodeTableChebyshev.goalNode == null) return null;

		int routeCount = _nodeTableChebyshev.goalNode.distance;
		List<ChebyshevMoveData> result = new List<ChebyshevMoveData>(routeCount);
		for (int i = 0; i < routeCount; i++) {
			result.Add(null);
		}
		// �S�[������k���Čo�H����
		DistanceNodeChebyshev currentNode = _nodeTableChebyshev.goalNode;
		for (int i = routeCount - 1; i >= 0; i--) {
			var moveData = new ChebyshevMoveData(currentNode.prevNode.squareID, currentNode.squareID, currentNode.dir);
			result[i] = moveData;
			currentNode = currentNode.prevNode;
		}
		return result;
	}

}
