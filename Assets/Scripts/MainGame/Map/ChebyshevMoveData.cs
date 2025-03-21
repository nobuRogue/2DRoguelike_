/**
 * @file ChebyshevMoveData.cs
 * @brief 8方向の1歩分の移動データ
 * @author yao
 * @date 2025/1/23
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChebyshevMoveData {
	public int sourceSquareID = -1;
	public int targetSquareID = -1;
	public eDirectionEight dir = eDirectionEight.Invalid;

	public ChebyshevMoveData(int setSourceID, int setTargetID, eDirectionEight setDir) {
		sourceSquareID = setSourceID;
		targetSquareID = setTargetID;
		dir = setDir;
	}
}
