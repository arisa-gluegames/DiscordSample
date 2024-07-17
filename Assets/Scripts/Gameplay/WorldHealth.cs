using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldHealth : MonoBehaviour
{
    [SerializeField]
    protected Image healthBar;
    [SerializeField]
    protected TextMeshProUGUI healthText;

    protected virtual void OnEnable()
    {
        HealthManager.OnHealthUpdate += UpdateHealthBar;
    }

    protected virtual void OnDisable()
    {
        HealthManager.OnHealthUpdate -= UpdateHealthBar;
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Enemy>(out Enemy enemy))
        {
            enemy.DestroyEnemy();
            HealthManager.Instance.Health -= enemy.Damage;
        }
    }

    protected void UpdateHealthBar(float health, float max)
    {
        healthBar.fillAmount = health / max;
        healthText.text = $"{health}/{max}";
    }


}
