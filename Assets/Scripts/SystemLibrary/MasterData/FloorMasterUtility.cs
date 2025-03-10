/**
 * @file FloorMasterUtility.cs
 * @brief �t���A�}�X�^�[�f�[�^���s����
 * @author yao
 * @date 2025/2/4
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Entity_FloorData;

public class FloorMasterUtility {
	/// <summary>
	/// �t���A�}�X�^�[�f�[�^�擾
	/// </summary>
	/// <param name="floorCount"></param>
	/// <returns></returns>
	public static Param GetFloorMaster(int floorCount) {
		List<Param> floorMasterList = MasterDataManager.floorData[0];
		for (int i = 0, max = floorMasterList.Count; i < max; i++) {
			if (floorMasterList[i].floorCount != floorCount) continue;

			return floorMasterList[i];
		}
		return null;
	}
}
