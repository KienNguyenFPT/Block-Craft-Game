using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPCStats : MonoBehaviour
{
    public float maxHealth = 100;
    private float currentHealth;
    public Slider healthSlider;
    private TextMeshProUGUI healthText;
    public Transform healthBarPosition;
    public float respawnTime = 5f;
    public int expAmount = 5;
    private Vector3 initialPosition;
    public NPCController npcController;

    public float wanderRadius = 20f;
    public float wanderTimer = 20f;
    public int attackPoint = 10;
    public float attackDistance = 2f;
    public float attackSpeed = 1f;
    public float attackCooldown = 5f;
    public GameObject[] dropItems;
    public GameObject spotLight;
    public int minDropCount = 1;
    public int maxDropCount = 5;
    public float dropRadius = 5f;
    private Animator animator;
    public bool isDied = false;
    void Start()
    {
        if (healthSlider != null)
        {
            healthText = healthSlider.GetComponentInChildren<TextMeshProUGUI>();
        }
        currentHealth = maxHealth;
        initialPosition = transform.position;
        animator = GetComponent<Animator>();
        npcController = GetComponent<NPCController>();
        UpdateUI();
    }

    void Update()
    {
        if (healthBarPosition != null && healthSlider != null)
        {
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(healthBarPosition.position);
            healthSlider.transform.position = screenPosition;

            if (screenPosition.z < 0)
            {
                screenPosition.z = 0;
                healthSlider.transform.position = screenPosition;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        gameObject.transform.Find("Health").gameObject.SetActive(true);
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;
        UpdateUI();

        if (currentHealth == 0)
        {
            isDied = true;
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                GuyStats playerStats = player.GetComponent<GuyStats>();
                if (playerStats != null)
                {
                    playerStats.GainExperience(expAmount);
                }
                else
                {
                    Debug.LogWarning("GuyStats component not found on the player.");
                }
            }
            else
            {
                Debug.LogWarning("Player not found.");
            }
            Die();
        }
    }

    private void Die()
    {
        DropItems();
        if (gameObject.CompareTag("NPCAttack"))
        {
            animator.SetTrigger("Died");
        }
        else
        {
            gameObject.SetActive(false);
        }
        Invoke(nameof(Respawn), respawnTime);
    }
    void DropItems()
    {
        int dropCount = Random.Range(minDropCount, maxDropCount + 1);

        for (int i = 0; i < dropCount; i++)
        {
            GameObject item = dropItems[Random.Range(0, dropItems.Length)];
            Vector3 randomPosition = transform.position + Random.insideUnitSphere * dropRadius;
            float pos = transform.position.y + 1f;
            if (pos > 1)
            {
                randomPosition.y = pos;
            }
            else
            {
                GameObject player = GameObject.FindWithTag("Player");
                if (player != null)
                {
                    Vector3 playerPosition = player.transform.position;
                    randomPosition.y = player.transform.position.y + 1f;
                    Debug.Log("Player Position: " + playerPosition);
                }
            }
            item.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f); 
            GameObject instantiatedItem = Instantiate(item, randomPosition, Quaternion.identity);
            GameObject instantiatedSpotLight = Instantiate(spotLight, randomPosition, Quaternion.identity);
            instantiatedSpotLight.transform.SetParent(instantiatedItem.transform);
        }
    }

    private void Respawn()
    {
        gameObject.SetActive(false);
        currentHealth = maxHealth;
        transform.position = initialPosition;
        isDied = false;
        gameObject.SetActive(true);
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        if (healthText != null)
        {
            healthText.text = currentHealth + "/" + maxHealth;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon"))
        {
            Transform weaponRoot = other.transform.root;

            GuyAction player = weaponRoot.GetComponent<GuyAction>();
            if (player != null && player.currentWeapon != null && player.getIsSlashing())
            {
                float damage = player.currentWeapon.attackPower * player.GetComponent<GuyStats>().level;
                TakeDamage(damage);
            }
        }
    }

    private void OnTriggerExit()
    {
        Invoke(nameof(hideHealthBar), 5f);
    }

    private void hideHealthBar()
    {
        gameObject.transform.Find("Health").gameObject.SetActive(false);
    }
}
