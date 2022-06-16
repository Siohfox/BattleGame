using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BG.Entity
{
    public class Entity : MonoBehaviour
    {
        [SerializeField] protected GameObject shieldPrefab;
        protected GameObject basicHitFX;
        protected AudioClip shieldEquipClip;

        protected int entityMaxHealth;
        protected int entityCurrentHealth;
    }
}
