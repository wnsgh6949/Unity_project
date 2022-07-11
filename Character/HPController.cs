using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class HPController : MonoBehaviour
{   
    UIHealTimer _healTimer;
    UIHealth _uiHealth;
    Material _bodyMaterial;
    Color originalColor;
    Animator _animator;

    public int currentHealth;
    public int maxHealth = 100;
    
    // Damage()
    public bool isInvincible = false;
    public bool isCovered = false;
    public float blockChance = 0.2f;
    Text ui;
    public GameObject uiPrefab;
    public event System.Action OnDamaged;
    public event System.Action OnDeath;

    //Die()
    public GameObject deathEffect;

    // Heal()
    public int healAmount = 20;
    public float healCoolDown = 5f;
    float nextHeal;

    public bool isPlayer = false;
    
    //LootSpawner _lootSpawner;

    void Start()
    {
        _healTimer = GetComponent<UIHealTimer>();
        _uiHealth = GetComponent<UIHealth>();
        //_lootSpawner = GetComponent<LootSpawner>();
        //_bodyMaterial = transform.GetChild(0).GetComponent<Renderer>().material;
        //originalColor = _bodyMaterial.color;
        _animator = GetComponent<Animator>();

        currentHealth = maxHealth;
    }

    public void Damage(int amount)
    {
        if(isInvincible)
        {
            return;
        }

        if(isCovered && amount < 100 && Random.value < blockChance) // chance to block incoming damage
        {
            foreach(Canvas c in FindObjectsOfType<Canvas>())
            {
                if(c.renderMode == RenderMode.WorldSpace)
                {
                    ui = Instantiate(uiPrefab, c.transform).GetComponent<Text>();
                    ui.text = "Block!";
                    ui.transform.position = transform.position + Vector3.up + Vector3.right;
                    break;
                }
            }
            return;
        }

        if(OnDamaged != null)
        {
            OnDamaged();
            AudioManager.instance.PlaySound("Impact", transform.position);
        }

        currentHealth -= amount;
        //StartCoroutine(DamageEffect());
        StartCoroutine(_uiHealth.SetValue(currentHealth / (float)maxHealth));

        if(currentHealth <= 0 && OnDeath != null)
        {
            OnDeath();
            OnDeath = null;
            Die();
        }   
    }

    [ContextMenu("Self Destruct")]
    public void Die()
    {
        AudioManager.instance.PlaySound("Death", transform.position);
        _animator.Rebind();
        _animator.Update(0f);
        _animator.SetTrigger("Die");
        StopAllCoroutines();
        //if(_lootSpawner != null) _lootSpawner.SpawnLoot();
        Component[] comps = GetComponents<Component>();
        foreach(Component c in comps)
        {
            if(c != this && !(c is Transform) && c != _animator)
            {
                Destroy(c);
            }
        }
        if(_healTimer != null)
        {
            Destroy(_healTimer.ui.gameObject);
        }
        Destroy(_uiHealth.ui.gameObject);
        Destroy(gameObject, 2);
        Destroy(this);
    }

    public void Heal()
    {
        if(Time.time > nextHeal && currentHealth < maxHealth)
        {
            if((currentHealth += healAmount) > maxHealth) // heal but currentHealth can't exceed maxHealth
            {
                ResetHealth();
            }
            StartCoroutine(_uiHealth.SetValue(currentHealth / (float)maxHealth));
            foreach(Canvas c in FindObjectsOfType<Canvas>())
            {
                if(c.renderMode == RenderMode.WorldSpace)
                {
                    ui = Instantiate(uiPrefab, c.transform).GetComponent<Text>();
                    ui.text = "+";
                    ui.color = Color.green;
                    ui.transform.position = transform.position + new Vector3(Random.Range(0.5f, 1), Random.Range(0.5f, 1), Random.Range(-0.25f, 0.25f));
                    break;
                }
            }
            nextHeal = Time.time + healCoolDown;

            StartCoroutine(_healTimer.CoolDown());
        }
    }

    public void Regenerate()
    {
        if(currentHealth < maxHealth)
        {
            currentHealth++;
            StartCoroutine(_uiHealth.SetValue(currentHealth / (float)maxHealth));
            foreach(Canvas c in FindObjectsOfType<Canvas>())
            {
                if(c.renderMode == RenderMode.WorldSpace)
                {
                    ui = Instantiate(uiPrefab, c.transform).GetComponent<Text>();
                    ui.text = "+";
                    ui.color = Color.green;
                    ui.transform.position = transform.position + new Vector3(Random.Range(0.5f, 1), Random.Range(0.5f, 1), Random.Range(-0.25f, 0.25f));
                    break;
                }
            }
        }
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        StartCoroutine(_uiHealth.SetValue(currentHealth / (float)maxHealth));
    }

    IEnumerator DamageEffect()
    {
        _bodyMaterial.color = Color.red;
        yield return new WaitForSeconds(0.05f);
        _bodyMaterial.color = originalColor;
    }
}
