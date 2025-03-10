/**
 * @file ExpansionMethod.cs
 * @brief �g�����\�b�h�N���X
 * @author yao
 * @date 2025/1/14
 */

public static class ExpansionMethod {

	/// <summary>
	/// ���Ε������擾
	/// </summary>
	/// <param name="dir"></param>
	/// <returns></returns>
	public static eDirectionFour ReverseDir(this eDirectionFour dir) {
		int result = (int)dir + 2;
		if (result >= (int)eDirectionFour.Max) result -= (int)eDirectionFour.Max;

		return (eDirectionFour)result;
	}

	/// <summary>
	/// �΂ߕ������ۂ�
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
	/// �΂ߕ�����2�����ɕ���
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
	/// ������8�����̌����ɕϊ�
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
	/// �������}�X�^�[���b�Z�[�W�ɕϊ�
	/// </summary>
	/// <param name="messageID"></param>
	/// <returns></returns>
	public static string ToMessage(this int messageID) {
		return MessageMasterUtility.GetMessage(messageID);
	}

}
