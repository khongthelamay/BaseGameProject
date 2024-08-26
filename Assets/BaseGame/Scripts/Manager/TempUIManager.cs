using System.Collections;
using System.Collections.Generic;
using TW.Utility.DesignPattern;
using UnityEngine;

public class TempUIManager : Singleton<TempUIManager>
{
    [field: SerializeField] public ModalHeroInteract ModalHeroInteract {get; private set;}
    
    public void ShowModalHeroInteract(FieldSlot fieldSlot)
    {
        ModalHeroInteract.gameObject.SetActive(true);
        ModalHeroInteract.SetFieldSlot(fieldSlot);
    }
    
}
