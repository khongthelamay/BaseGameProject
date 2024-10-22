namespace Core
{
    public class ImmediatelyProjectile : Projectile
    {
        public override Projectile Spawn(Hero hero, Enemy targetEnemy)
        {
            return this;
        }

        public override Projectile Setup(Hero hero, Enemy targetEnemy)
        {
            ProjectileBehaviorGroup.StartBehavior(hero, targetEnemy);
            ProjectileBehaviorGroup.EndBehavior(hero, targetEnemy);
            return base.Setup(hero, targetEnemy);
        }
    }
}