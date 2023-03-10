using UnityEngine;

public class Recallable : MonoBehaviour
{
    [Range(0, 1)] [SerializeField] float damage = 0.1f;
    [SerializeField] Vector3 m_PointLightOffset;

    public string hitObject;
    // Start is called before the first frame update
    void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            hitObject = collision.gameObject.name;
            print("EnemyHit: " + collision.gameObject.name);
            collision.gameObject.GetComponentInParent<EnemyHealth>().TakeDamage(damage, collision.collider);
        }
    }

    public Vector3 GetPointLightOffset => m_PointLightOffset;

    public void LightPop()
    {
            
    }
}
