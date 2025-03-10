/**
 * @file PartBase.cs
 * @brief ゲームパートの基底
 * @author yao
 * @date 2025/1/9
 */

using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PartBase : MonoBehaviour {
	/// <summary>
	/// 初期化
	/// </summary>
	/// <returns></returns>
	public abstract UniTask Initialize();
	/// <summary>
	/// 実行前の準備
	/// </summary>
	/// <returns></returns>
	public abstract UniTask Setup();
	/// <summary>
	/// 実行
	/// </summary>
	/// <returns></returns>
	public abstract UniTask Execute();
	/// <summary>
	/// 片付け
	/// </summary>
	/// <returns></returns>
	public abstract UniTask Teardown();
}
