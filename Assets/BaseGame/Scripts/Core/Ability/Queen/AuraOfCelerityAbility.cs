namespace Core
{
    public class AuraOfCelerityAbility : PassiveAbility
    {
        public AuraOfCelerityAbility(Hero owner, int levelUnlock) : base(owner, levelUnlock)
        {
            
        }

        public override void OnEnterBattleField()
        {
            BattleManager.ChangeGlobalBuff(GlobalBuff.Type.AttackDamage, 20);
        }

        public override void OnExitBattleField()
        {
            BattleManager.ChangeGlobalBuff(GlobalBuff.Type.AttackDamage, -20);
        }
    }
}