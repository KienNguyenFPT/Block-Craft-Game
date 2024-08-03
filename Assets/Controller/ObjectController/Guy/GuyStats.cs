using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuyStats : MonoBehaviour
{
    public string userName = "Rai Yugi";
    public int level;
    public int maxHealth;
    public int currentHealth;
    public int maxMana;
    public int currentMana;
    public int exp;
    public int respawnTime = 3;
    public int currentExp = 0;

    public TextMeshProUGUI userNameText;
    public TextMeshProUGUI levelText;

    private TextMeshProUGUI healthText;
    private TextMeshProUGUI manaText;
    private TextMeshProUGUI expText;
    public Slider healthSlider;
    public Slider manaSlider;
    public Slider expSlider;
    public Slider diedCountdownSlider;
    private Animator animator;
    public bool IsDied = false;
    private SpawnManager spawnManager;
    public float regenRate = 3f;
    public GuyInventory GuyInventory;


    void Start()
    {
        if (healthSlider != null)
        {
            healthText = healthSlider.GetComponentInChildren<TextMeshProUGUI>();
        }

        if (manaSlider != null)
        {
            manaText = manaSlider.GetComponentInChildren<TextMeshProUGUI>();
        }

        if (expSlider != null)
        {
            expText = expSlider.GetComponentInChildren<TextMeshProUGUI>();
        }
        userName = StaticLoadData.userName;

        if (StaticLoadData.IsLoadData)
        {
            gameObject.SetActive(false);
            level = StaticLoadData.level;
            currentExp = StaticLoadData.currentExp;
            currentHealth = StaticLoadData.currentHealth;
            currentMana = StaticLoadData.currentMana;
            GuyInventory = GetComponent<GuyInventory>();
            foreach (var weaponToSave in StaticLoadData.weapons)
            {
                Weapon weapon = new Weapon(weaponToSave);
                GuyInventory.AddWeapon(weapon);
            }

            foreach (var itemToSave in StaticLoadData.inventory)
            {
                Item item = new Item(itemToSave.itemName, itemToSave.itemType, itemToSave.icon, itemToSave.amount, itemToSave.describe, itemToSave.prefab);
                GuyInventory.AddItem(item);
            }
            Vector3 pos = StaticLoadData.playerPosition;
            pos.y += 2f;
            transform.position = pos;
            gameObject.SetActive(true);
        }

        userNameText.text = userName;
        levelText.text = "Lv. " + level;
        StartStats(level);
        StartCoroutine(RegenerateHealthMana());
        spawnManager = FindObjectOfType<SpawnManager>();
        animator = GetComponent<Animator>();
    }

    public void SetGuyStats(string userName, int level, int currentHealth, int currentMana, int currentExp)
    {
        this.userName = userName;
        this.level = level;
        this.currentHealth = currentHealth;
        this.currentMana = currentMana;
        this.currentExp = currentExp;
    }

    private void Update()
    {
        if (healthSlider.value != currentHealth)
        {
            healthSlider.value = currentHealth;
        }

        if (manaSlider.value != currentMana)
        {
            manaSlider.value = currentMana;
        }

        if (expSlider.value != currentExp)
        {
            expSlider.value = currentExp;
        }
        UpdateStats(level);
    }

    public void StartStats(int level)
    {
        maxHealth = CalculateMaxHealth(level);
        maxMana = CalculateMaxMana(level);
        exp = CalculateLevelExp(level);
        currentHealth = maxHealth;
        currentMana = maxMana;
        UpdateUI();
    }

    public void UpdateStats(int newLevel)
    {
        level = newLevel;
        maxHealth = CalculateMaxHealth(level);
        maxMana = CalculateMaxMana(level);
        exp = CalculateLevelExp(level);
        UpdateUI();
    }

    private int CalculateMaxHealth(int level)
    {
        return 1000 + level * 500;
    }

    private int CalculateMaxMana(int level)
    {
        return 50 + level * 10;
    }

    private int CalculateLevelExp(int level)
    {
        return level * level * 1000;
    }

    private void UpdateUI()
    {
        if (healthText != null)
        {
            healthText.text = "Health: " + currentHealth + "/" + maxHealth;
        }

        if (manaText != null)
        {
            manaText.text = "Mana: " + currentMana + "/" + maxMana;
        }

        if (expText != null)
        {
            expText.text = "Exp: " + currentExp + "/" + exp;
        }

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        if (manaSlider != null)
        {
            manaSlider.maxValue = maxMana;
            manaSlider.value = currentMana;
        }

        if (expSlider != null)
        {
            expSlider.maxValue = exp;
            expSlider.value = currentExp;
        }
    }

    private IEnumerator RegenerateHealthMana()
    {
        while (true)
        {
            yield return new WaitForSeconds(regenRate);

            if (currentHealth < maxHealth)
            {
                int healthRegen = (int)(regenRate * level);
                currentHealth += healthRegen;
                if (currentHealth > maxHealth)
                {
                    currentHealth = maxHealth;
                }
            }

            if (currentMana < maxMana)
            {
                int manaRegen = (int)(regenRate * level / 2);
                currentMana += manaRegen;
                if (currentMana > maxMana)
                {
                    currentMana = maxMana;
                }
            }

            UpdateUI();
        }
    }


    public void GainExperience(int amount)
    {
        currentExp += amount;

        if (currentExp >= exp)
        {
            LevelUp();
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth = Mathf.Max(0, currentHealth - amount);

        UpdateUI();
    }

    private void FixedUpdate()
    {

        if (currentHealth <= 0 && !IsDied)
        {
            animator.SetTrigger("Died");
            IsDied = true;
            if (level > 1 && currentExp == 0)
            {
                --level;
                int temp = CalculateLevelExp(level);
                currentExp = Mathf.Max(0, temp - Mathf.FloorToInt(temp * 0.1f));
            }
            else
            {
                currentExp = Mathf.Max(0, currentExp - Mathf.FloorToInt(currentExp * 0.1f));
            }
            Invoke(nameof(Respawn), respawnTime * level);
        }
    }
    private void Respawn()
    {
        gameObject.SetActive(false);
        IsDied = false;

        currentHealth = CalculateMaxHealth(level);
        currentMana = CalculateMaxMana(level);
        UpdateUI();
        UpdateLevelUI();

        Vector3 spawnPosition = spawnManager.GetSpawnPosition();
        transform.position = spawnPosition;
        gameObject.SetActive(true);
    }

    private void UpdateRespawnUI(float timeRemaining)
    {
        diedCountdownSlider.GetComponentInChildren<TextMeshProUGUI>().text = Mathf.CeilToInt(timeRemaining).ToString();
        diedCountdownSlider.value = timeRemaining;
    }
    private void LevelUp()
    {
        level++;
        currentExp -= exp;
        exp = CalculateNextLevelExp(level);
        UpdateLevelUI();
    }

    private int CalculateNextLevelExp(int level)
    {
        return CalculateLevelExp(level);
    }

    private void UpdateLevelUI()
    {
        if (levelText != null)
        {
            levelText.text = "Level: " + level;
        }
    }
}
