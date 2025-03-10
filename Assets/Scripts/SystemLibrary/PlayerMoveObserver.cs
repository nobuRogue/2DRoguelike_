/**
 * @file PlayerMoveObserver.cs
 * @brief プレイヤー移動用オブザーバ
 * @author yao
 * @date 2025/1/21
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface PlayerMoveObserver {
	void OnPlayerMove(Vector3 playerPosition);
}
