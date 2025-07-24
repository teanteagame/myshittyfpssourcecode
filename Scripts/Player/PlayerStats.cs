using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public float healthPoint = 100;
    public float maxHealthPoint = 100;
    public float healthRecoverRate = 5;
    public float healthRegenDelay = 7;

    public float stamina = 100;
    public float maxStamina = 100;
    public float staminaRecoverRate = 5;
    public float staminaRegenDelay = 2;

    public float hunger = 100;
    public float maxHunger = 100;
    public float hungerDrainRate = 2;
    public float saturatingTime = 3;

    public float oxygen = 100;
    public float maxOxygen = 100;
    public float oxygenRecoverRate = 5;
    public float oxygenDrainRate = 3;

    private bool isTakingDamage;
    private bool isDrainingStamina;
    private bool isSaturating;
    private bool isHoldingBreath;

    private float damageTimer, staminaTimer, hungerTimer, starvationTimer, breathTimer;

    public bool isDead;

    private PlayerEffect effect;

    private void Start()
    {
        effect = GetComponent<PlayerEffect>();
    }

    private void Update()
    {
        UpdateStats();
    }

    void UpdateStats()
    {
        if (isTakingDamage)
        {
            damageTimer += Time.deltaTime;
            if(damageTimer >= healthRegenDelay)
            {
                isTakingDamage = false;
                damageTimer = 0;
            }
        }
        else
        {
            if (hunger > 0) healthPoint += Time.deltaTime * healthRegenDelay;

            if(healthPoint >= maxHealthPoint)
            {
                healthPoint = maxHealthPoint;
            }
        }

        if (isDrainingStamina) 
        {
            staminaTimer += Time.deltaTime;
            if(staminaTimer >= staminaRegenDelay)
            {
                isDrainingStamina = false;
                staminaTimer = 0;
            }
        }
        else
        {
            if (hunger > 0) stamina += Time.deltaTime * staminaRecoverRate;
            if(stamina >= maxStamina)
            {
                stamina = maxStamina;
            }
        }

        if (isSaturating)
        {
            hungerTimer += Time.deltaTime;
            if (hungerTimer >= saturatingTime) 
            {
                isSaturating = false;
                hungerTimer = 0;
            }
        }
        else
        {
            hunger -= Time.deltaTime * hungerDrainRate;
            if(hunger <= 0)
            {
                hunger = 0;

                starvationTimer += Time.deltaTime;
                if(starvationTimer >= 10)
                {
                    ApplyDamage(10, true);
                    starvationTimer = 0;
                }
            }
        }

        if (isHoldingBreath) 
        {
            oxygen -= Time.deltaTime * oxygenDrainRate;
            if(oxygen <= 0)
            {
                oxygen = 0;

                breathTimer += Time.deltaTime;
                if(breathTimer > 3)
                {
                    ApplyDamage(20, true);
                    breathTimer = 0;
                }
            }
        }
        else
        {
            oxygen += Time.deltaTime * oxygenRecoverRate;
            if (oxygen >= maxOxygen) oxygen = maxOxygen;
        }
    }

    public void DrainStamina(float amount)
    {
        stamina -= amount;

        if (stamina <= 0)
        {
            stamina = 0;
        }

        isDrainingStamina = true;
        staminaTimer = 0;
    }

    public void ApplyDamage(float damage, bool piercing = false)
    {
        if (piercing)
        {
             healthPoint -= damage;
        }
        else
        {
           
        }

        if(healthPoint <= 0)
        {
            healthPoint = 0;
            isDead = true;
        }

        if (Random.value > 0.75f) effect.PlayerMouthAudio("Player Hurt 0" + Random.Range(1, 4));
        isTakingDamage = true;
        damageTimer = 0;
    }

    public void EatingCalories(float amount)
    {
        hunger += amount;

        if(hunger >= maxHunger)
        {
            hunger = maxHunger;
        }

        isSaturating = true;
        hungerTimer = 0;
    }

    public void HoldBreath(bool isHolding)
    {      
        if (isHolding)
        {
            isHoldingBreath = true;
        }
        else
        {
            isHoldingBreath = false;
            breathTimer = 0;
        }
    }
}
