/**
 * @file ActionRangeManager.cs
 * @brief s“®‚ÌË’ö‚ÌŠÇ—
 * @author yao
 * @date 2025/2/18
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CommonModule;

public class ActionRangeManager {
	private static List<ActionRangeBase> _actionRangeList = null;

	public static void Initialize() {
		_actionRangeList = new List<ActionRangeBase>();
		_actionRangeList.Add(new ActionRange00_DirForward());
	}

	/// <summary>
	/// Ë’ö‚Ìæ“¾
	/// </summary>
	/// <param name="rangeType"></param>
	/// <returns></returns>
	public static ActionRangeBase GetRange(int rangeType) {
		if (!IsEnableIndex(_actionRangeList, rangeType)) return null;

		return _actionRangeList[rangeType];
	}

}
