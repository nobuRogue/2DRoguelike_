/**
 * @file ManhattanMoveData.cs
 * @brief 4•ûŒü‚Ì1•à•ª‚ÌˆÚ“®ƒf[ƒ^
 * @author yao
 * @date 2025/1/16
 */

public class ManhattanMoveData {
	public int sourceSquareID = -1;
	public int targetSquareID = -1;
	public eDirectionFour dir = eDirectionFour.Invalid;

	public ManhattanMoveData(int setSourceID, int setTargetID, eDirectionFour setDir) {
		sourceSquareID = setSourceID;
		targetSquareID = setTargetID;
		dir = setDir;
	}
}
