/**
 * @file TerrainSpriteAssignor.cs
 * @brief 地形に応じたスプライト割り当て
 * @author yao
 * @date 2025/1/9
 */

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

using static CommonModule;

public class TerrainSpriteAssignor {
	// 地形スプライト画像のパス
	private static readonly string _MAP_SPRITE_PATH = "Design/Sprites/Map/";
	// 地形スプライト画像のファイル名
	private static readonly string[][] _MAP_SPRITE_NAME_LIST = new string[][] {
		new string[]{ "rogue_map_sand_floor","rogue_map_sand_wall","rogue_map_sand_stair"},
		new string[]{ "rogue_map_snow_floor", "rogue_map_snow_wall", "rogue_map_snow_stair"},
		new string[]{ "rogue_map_urban_floor", "rogue_map_urban_wall", "rogue_map_urban_stair"}};

	private static List<List<Sprite[]>> _terrainSpriteList = null;
	private static int _floorTypeIndex = -1;

	public static void Initialize() {
		int mapTypeMax = _MAP_SPRITE_NAME_LIST.Length;
		int terrainSpriteMax = _MAP_SPRITE_NAME_LIST[0].Length;
		_terrainSpriteList = new List<List<Sprite[]>>(mapTypeMax);
		for (int mapType = 0; mapType < mapTypeMax; mapType++) {
			_terrainSpriteList.Add(new List<Sprite[]>(terrainSpriteMax));
			for (int i = 0; i < terrainSpriteMax; i++) {
				Sprite[] loadSprite = Resources.LoadAll<Sprite>(_MAP_SPRITE_PATH + _MAP_SPRITE_NAME_LIST[mapType][i]);
				_terrainSpriteList[mapType].Add(loadSprite);
			}
		}
	}

	/// <summary>
	/// マップチップタイプの設定
	/// </summary>
	/// <param name="setIndex"></param>
	public static void SetFloorSpriteTypeIndex(int setIndex) {
		_floorTypeIndex = setIndex;
	}

	/// <summary>
	/// 地形スプライト画像の取得
	/// </summary>
	/// <param name="terrain"></param>
	/// <returns></returns>
	public static Sprite GetTerrainSprite(eTerrain terrain, int spriteIndex = -1) {
		if (!IsEnableIndex(_terrainSpriteList, _floorTypeIndex)) return null;

		Sprite[] spriteList = _terrainSpriteList[_floorTypeIndex][GetSpriteIndex(terrain)];
		if (!IsEnableIndex(spriteList, spriteIndex)) spriteIndex = Random.Range(0, spriteList.Length);

		return spriteList[spriteIndex];
	}

	private static int GetSpriteIndex(eTerrain terrain) {
		switch (terrain) {
			case eTerrain.Passage:
			case eTerrain.Room:
			return 0;
			case eTerrain.Wall:
			return 1;
			case eTerrain.Stair:
			return 2;
			default:
			return 0;
		}
	}

}
