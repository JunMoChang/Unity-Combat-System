using System;
using System.Collections.Generic;
using Scriptable;
using UnityEngine;

namespace WeaponConfiguration.AbstractWeapon
{
    public abstract class BaseWeapon : MonoBehaviour
    {
        public ItemScriptableObject weaponSo;
        
        [NonSerialized] public bool isGet;
        
        private readonly Dictionary<Type, IWeaponComponent> components = new ();
        
        protected void AddWeaponComponent<T>(T component)where T : IWeaponComponent
        {
            components[typeof(T)] = component;
        }

        public T GetWeaponComponent<T>()where T : class, IWeaponComponent
        {
            if(components.TryGetValue(typeof(T), out var component))
            {
                return component as T;
            }
            return null; 
        }

        public void CreateWeapon()
        {
            GameObject weapon = Instantiate(weaponSo.prefab);

            BaseWeapon baseWeapon = weapon.AddComponent<BaseWeapon>();
            baseWeapon.weaponSo = weaponSo;
        }
    }
}