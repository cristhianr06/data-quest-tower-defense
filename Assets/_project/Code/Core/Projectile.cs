using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float amoutDamage = 10f;
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;
        Debug.Log("Enemy");
        if (other.TryGetComponent<IDamage>(out var damage))
        {
            Debug.Log("Damage");
            damage.TakeDamage(amoutDamage);
        }
    }
}
