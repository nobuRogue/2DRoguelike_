/**
 * @file ExpansionMethod.cs
 * @brief 拡張メソッドクラス
 * @author yao
 * @date 2025/1/14
 */

public static class ExpansionMethod {

	/// <summary>
	/// 反対方向を取得
	/// </summary>
	/// <param name="dir"></param>
	/// <returns></returns>
	public static eDirectionFour ReverseDir(this eDirectionFour dir) {
		int result = (int)dir + 2;
		if (result >= (int)eDirectionFour.Max) result -= (int)eDirectionFour.Max;

		return (eDirectionFour)result;
	}

	/// <summary>
	/// 斜め方向か否か
	/// </summary>
	/// <param name="dir"></param>
	/// <returns></returns>
	public static bool IsSlant(this eDirectionEight dir) {
		switch (dir) {
			case eDirectionEight.UpRight:
			case eDirectionEight.DownRight:
			case eDirectionEight.DownLeft:
			case eDirectionEight.UpLeft:
			return true;
		}
		return false;
	}

	/// <summary>
	/// 斜め方向を2方向に分割
	/// </summary>
	/// <param name="dir"></param>
	/// <returns></returns>
	public static eDirectionFour[] Separate(this eDirectionEight dir) {
		eDirectionFour[] result = new eDirectionFour[2];
		switch (dir) {
			case eDirectionEight.UpRight:
			result[0] = eDirectionFour.Up;
			result[1] = eDirectionFour.Right;
			break;
			case eDirectionEight.DownRight:
			result[0] = eDirectionFour.Down;
			result[1] = eDirectionFour.Right;
			break;
			case eDirectionEight.DownLeft:
			result[0] = eDirectionFour.Down;
			result[1] = eDirectionFour.Left;
			break;
			case eDirectionEight.UpLeft:
			result[0] = eDirectionFour.Up;
			result[1] = eDirectionFour.Left;
			break;
		}
		return result;
	}

	public static eFloorEndReason GetFloorEndReaosn(this eDungeonEndReason reaosn) {
		switch (reaosn) {
			case eDungeonEndReason.Dead:
			return eFloorEndReason.Dead;
			case eDungeonEndReason.Clear:
			return eFloorEndReason.Stair;
		}
		return eFloorEndReason.Invalid;
	}

	/// <summary>
	/// 整数を8方向の向きに変換
	/// </summary>
	/// <param name="index"></param>
	/// <returns></returns>
	public static eDirectionEight ToDirEight(this int index) {
		int maxIndex = (int)eDirectionEight.Max;
		while (index < 0) index += maxIndex;

		while (index >= maxIndex) index -= maxIndex;

		return (eDirectionEight)index;
	}

	/// <summary>
	/// 整数をマスターメッセージに変換
	/// </summary>
	/// <param name="messageID"></param>
	/// <returns></returns>
	public static string ToMessage(this int messageID) {
		return MessageMasterUtility.GetMessage(messageID);
	}

}
