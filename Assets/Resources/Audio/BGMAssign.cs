/**
 * @file BGMAssign.cs
 * @brief BGMファイルの割り当て
 * @author yao
 * @date 2025/2/25
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BGMAssign : ScriptableObject {
	public AudioClip[] bgmArray = null;
}
