using System;
using Manager;
using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Core
{
    [Serializable]
    public abstract class Ability : ScriptableObject
    {
        [GUIColor("@TW.Utility.Extension.AColorExtension.GetColorById((int)$value, 64)")]
        [Flags]
        public enum Group
        {
            NormalAttack = 1 << 0,
            Cooldown = 1 << 1,
            Randomness = 1 << 2,
            Passive = 1 << 3,
            Ultimate = 1 << 4,
        }
        private BattleManager BattleManagerCache { get; set; }
        public BattleManager BattleManager => BattleManagerCache ??= BattleManager.Instance;
        [field: SerializeField, InlineButton("SetupGroup",SdfIconType.ArrowRepeat, "")] public Group AbilityGroup {get; protected set;}

        [field: PreviewField(98, ObjectFieldAlignment.Left), HorizontalGroup(nameof(Ability), 100), HideLabel]
        [field: HideIf("@this is NormalAttackAbility")]
        [field: SerializeField] public Sprite Icon {get; set;}
        [field: VerticalGroup(nameof(Ability) + "/Info")]
        [field: HideIf("@this is NormalAttackAbility")]
        [field: SerializeField] public int LevelUnlock {get; set;}
        [field: VerticalGroup(nameof(Ability) + "/Info")]
        [field: HideIf("@this is NormalAttackAbility")]
        [field: SerializeField] public string Name {get; set;}
        [field: VerticalGroup(nameof(Ability) + "/Info"), TextArea]
        [field: HideIf("@this is NormalAttackAbility")]
        [field: SerializeField] public string Description {get; set;}

        public Hero Owner {get; private set;}
        
        protected virtual void SetupGroup()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif

        }
        
        public virtual Ability WithOwnerHero(Hero owner)
        {
            Owner = owner;
            return this;
        }
        public virtual Ability ResetAbility()
        {
            return this;
        }
        public bool IsUnlocked(int level) => level >= LevelUnlock;
        
        public abstract void OnEnterBattleField();
        public abstract void OnExitBattleField();
        public Ability Create()
        {
            return Instantiate(this);
        }
        public Ability Initialize(Hero owner)
        {
            return Create().WithOwnerHero(owner);
        }
    }
}