using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core
{
    [Serializable]
    public abstract class ProjectileBehavior
    {
        public abstract void StartBehavior(Hero hero, Enemy enemy);
        public abstract void EndBehavior(Hero hero, Enemy enemy);
    }
    [Serializable, HideLabel]
    public class ProjectileBehaviorGroup
    {
        [field: SerializeReference] public ProjectileBehavior[] ProjectileBehaviors { get; set; }
        
        public void StartBehavior(Hero hero, Enemy enemy)
        {
            foreach (ProjectileBehavior behavior in ProjectileBehaviors)
            {
                behavior.StartBehavior(hero, enemy);
            }
        }

        public void EndBehavior(Hero hero, Enemy enemy)
        {
            foreach (ProjectileBehavior behavior in ProjectileBehaviors)
            {
                behavior.EndBehavior(hero, enemy);
            }
        }
    }
}