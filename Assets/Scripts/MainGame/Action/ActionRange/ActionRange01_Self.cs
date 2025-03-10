/**
 * @file ActionRange01_Self.cs
 * @brief é©êg
 * @author yao
 * @date 2025/3/10
 */

using static CommonModule;

public class ActionRange01_Self : ActionRangeBase {
	public override void Setup( CharacterBase sourceCharacter ) {
		InitializeList( ref targetList );
		targetList.Add( sourceCharacter.ID );
	}

}
