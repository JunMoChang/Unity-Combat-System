using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Enemy
{
    public class EnemyController : MonoBehaviour
    {
        [NonSerialized] public bool isMoving;
        public float sightRadius = 10;
        public float speed = 4.5f;
        public float step = 0.1f;
        public float maxHealth;
        private float currentHealth;
        [NonSerialized] public bool isHit;
        public Transform healthBar;
        public Image healthBarImage;
        private Camera mainCamera;
        [NonSerialized] public Animator animator;

        void Awake()
        {
            mainCamera = Camera.main;
            animator = GetComponent<Animator>();
            
            currentHealth = maxHealth;
        }
        
        void LateUpdate()
        {
            UpdateHeathBar();
        }
        
        public void TakeDamage(float damage)
        {
            Debug.Log("TakeDamage");
            currentHealth -= damage;
            currentHealth = Mathf.Max(0, currentHealth);
            
            isHit = true;
            Debug.Log(isHit);
            UpdateHeathBar();
            
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private void UpdateHeathBar()
        {     
            healthBarImage.fillAmount = currentHealth / maxHealth;
            
            if (currentHealth / maxHealth > 0.5f)
            {
                healthBarImage.color = Color.green;
            }
            else if (currentHealth / maxHealth < 0.2f)
            {
                healthBarImage.color = Color.red;
            }
            healthBar.LookAt(healthBar.position + mainCamera.transform.rotation * Vector3.forward,
                mainCamera.transform.rotation * Vector3.up);
        }
        
        private void Die()
        {
            Debug.Log("敌人死亡");
            Destroy(gameObject);
        }

        public void MoveToTarget(Transform targetTransform)
        {
            if (isHit) return;
            
            transform.position = Vector3.MoveTowards(
                transform.position, targetTransform.position, speed * Time.deltaTime);
            transform.LookAt(targetTransform.position);
        }
        
        public void BeginCoroutine(IEnumerator coroutine)
        {
            StartCoroutine(coroutine);
        }
    }
}