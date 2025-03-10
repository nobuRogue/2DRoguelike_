/**
 * @file SystemObject.cs
 * @brief 
 * @author yao
 * @date 2025/1/9
 */
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SystemObject : MonoBehaviour {
	public abstract UniTask Initialize();
}
