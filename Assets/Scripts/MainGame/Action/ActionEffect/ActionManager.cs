/**
 * @file ActionManager.cs
 * @brief 行動の管理
 * @author yao
 * @date 2025/2/18
 */

using Cysharp.Threading.Tasks;
using System.Collections.Generic;

using static CommonModule;

public class ActionManager {
	private static List<ActionEffectBase> _actionEffectList = null;

	public static void Initialize() {
		_actionEffectList = new List<ActionEffectBase>();
		_actionEffectList.Add( new ActionEffect000_Attack() );
		_actionEffectList.Add( new ActionEffect001_RecoveryHP() );
		_actionEffectList.Add( new ActionEffect002_RecoveryStamina() );
	}

	/// <summary>
	/// アクション実行
	/// </summary>
	/// <param name="sourceCharacter"></param>
	/// <param name="actionID"></param>
	/// <returns></returns>
	public static async UniTask ExecuteAction( CharacterBase sourceCharacter, int actionID ) {
		Entity_ActionData.Param actionMaster = ActionMasterUtility.GetActionMaster( actionID );
		if (actionMaster == null) return;

		ActionRangeBase range = ActionRangeManager.GetRange( actionMaster.rangeType );
		if (range == null) return;

		range.Setup( sourceCharacter );
		await ExecuteActionEffect( actionMaster.effectID, sourceCharacter, range );
	}

	/// <summary>
	/// アクション効果実行
	/// </summary>
	/// <param name="effectID"></param>
	/// <param name="sourceCharacter"></param>
	/// <param name="range"></param>
	/// <returns></returns>
	private static async UniTask ExecuteActionEffect( int effectID, CharacterBase sourceCharacter, ActionRangeBase range ) {
		Entity_ActionEffectData.Param effectMaster = ActionMasterUtility.GetActionEffectMaster( effectID );
		if (effectMaster == null) return;

		if (!IsEnableIndex( _actionEffectList, effectID )) return;

		await _actionEffectList[effectID].Execute( sourceCharacter, effectMaster, range );
		_actionEffectList[effectID].TearDown();
	}

}
