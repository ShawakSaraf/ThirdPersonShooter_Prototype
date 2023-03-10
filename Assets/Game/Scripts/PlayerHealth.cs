using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class PlayerHealth : MonoBehaviour
{
    [Range(0, 100)] [SerializeField] float playerHP = 100f;
    float maxHP;

    [SerializeField] RawImage healthBar;
    LevelLoader levelLoader;
    DeathHandler deathHandler;
    Color healthColor;
    VolumeProfile profile;
    // Renderer renderer;
    public ClampedFloatParameter intensity;

    void Start()
    {
        levelLoader = GetComponent<LevelLoader>();
        maxHP = playerHP;
        healthBar = GetComponent<RawImage>();
    }

    void Update()
    {
        var currHP = playerHP/maxHP;

        healthColor = Color.Lerp(Color.red, Color.green , currHP);
        if(playerHP < maxHP)
        {
            // healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, decHPBar, lerpSpeed * Time.deltaTime);
            // if(playerHP <= maxHP * 1/2)
        }
        Shader.SetGlobalFloat("_Health", currHP*0.65f);
    }
    
    public void DecreasePlayerHP(float playerHPDamage)
    {
        // playerHP -= playerHPDamage;
        // if(playerHP <= 0f)
        {
            DeathHandler deathHandler = FindObjectOfType<DeathHandler>();
            // deathHandler.ShowDeathScreen();
        }
    }

    public void EnableHealthBar()
    {
        // healthBar.enabled = true;
    }
}
