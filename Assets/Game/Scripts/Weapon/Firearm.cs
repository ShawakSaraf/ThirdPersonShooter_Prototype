using System;
using System.Collections;
using System.Collections.Generic;
using EnemyBehavior;
using UnityEngine;
using UnityEngine.AI;

public class Firearm : MonoBehaviour
{
    public static Firearm I { get; private set;}
    [Header("Camera Shake")]
    [Range(0 , 1)] [SerializeField] float aimShakeApm = .5f;
    [Range(0 , 1)] [SerializeField] float aimShakeFreq = .2f;
    [Range(0 , 1)] [SerializeField] float ShakeDuration = .2f;
    [Range(0 , 10000)] [SerializeField] float m_HitForce = 1000;
    
    [SerializeField] Camera mainCam;
    [SerializeField] Transform rayOrigin;
    
    [SerializeField] int gunDamage = 3;

    [Range(0 , 1)] [SerializeField] float m_TimeBetweenShots = 0.1f;

    [SerializeField] AmmoType ammoType;
    [SerializeField] ParticleSystem muzzleFlash;
    AudioSource audioSource;
    Animator ammoTxtAnim;
    EnemyAI enemyAI2;
    WeaponZoom weaponZoom;
    Reticle m_reticle;
    Ammo ammo;
    WeaponSwitcher weaponSwitcher;
    TPSController tPSController;
    CustomCinemachineCam _customCM;
    IKTarget iKTarget;
    WaitForSeconds m_coroitineLerpWait = new WaitForSeconds(1/90);
    WaitForSeconds m_ReloadWait = new WaitForSeconds(1);

    [SerializeField] Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();
    [SerializeField] List<Pool> pools;

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    [SerializeField] AmmoSlot[] ammoSlots;

    [System.Serializable]
    class AmmoSlot { public AmmoType ammoType; }

    bool isReloading = false;
    bool isAiming = false;
    RaycastHit hit;

    // bool isTimeSlow = false;

    public float m_currAmmoPercent {get; private set;} = 1;
    int m_lastClip = 0;
    int m_maxAmmo;

    void Awake()
    {
        I = this;
        AssignComponents();
    }

    void Start()
    {
        m_maxAmmo = ammo.GetCurAmmo(ammoType);
        StartCoroutine(UpdateAmmoGaugeUI());
    }

    void AssignComponents()
    {
        audioSource     = GetComponent<AudioSource>();
        weaponSwitcher  = GetComponentInParent<WeaponSwitcher>();
        iKTarget        = FindObjectOfType<IKTarget>();
        tPSController   = TPSController.I;
        _customCM       = FindObjectOfType<CustomCinemachineCam>();
        weaponZoom      = FindObjectOfType<WeaponZoom>();
        ammo            = FindObjectOfType<Ammo>();
        m_reticle       = FindObjectOfType<Reticle>();
    }
    void Update()
    {
        isAiming = tPSController.GetIsAiming;
        ManualReaload();
    }

    public void InitiateShoot()
    {
        if(ammo.GetCurAmmo(ammoType) > 0)
        {
            ammo.ReduceCurAmmo(ammoType);
            if(ammo.GetCurAmmo(ammoType) < m_maxAmmo) { m_lastClip++; }

            StartCoroutine(ExecuteShoot());

            // StartCoroutine(Shoot(ammo.GetCurrentAmmo(ammoType)));
        }
    }
    IEnumerator ExecuteShoot()
    {
        Shoot();
        yield return new WaitForSeconds(m_TimeBetweenShots);
        muzzleFlash.gameObject.SetActive(false);
        // animator.SetBool("Shoot", false);
    }

    void Shoot()
    {
        Vector2 screenCenterPoint = new Vector2(Screen.width/2f, Screen.height/2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        // bool rayHit = Physics.Raycast(ray, out RaycastHit hit, 999f); // raycast from Camera
        Vector3 rayDir = (iKTarget.transform.position - rayOrigin.position).normalized; // raycast from Pistol
        bool rayHit = Physics.SphereCast(ray, 0.01f, out hit, 999f);

        if(!isReloading) 
        {
            muzzleFlash.gameObject.SetActive(true);
            _customCM.CamShake(aimShakeApm, aimShakeFreq, ShakeDuration);
            // bool enemyIsDead = hit.transform.GetComponentInParent<FSM.FiniteStateMachine>().m_isDead;
            if (rayHit && hit.collider.tag == "Enemy" /* && !enemyIsDead */)
            {
                // ShowHitVFX(hit);

                Rigidbody hitRB = hit.collider.GetComponent<Rigidbody>();
                hitRB.AddForceAtPosition(transform.forward * m_HitForce, hit.point);
                PassHitInfo(hit.collider);
            }
        }
        else { return; }
    }

    void ShowHitVFX(RaycastHit hit)
    {
        if (hit.collider != null)
        {
            Queue<GameObject> hitVFXPool = new Queue<GameObject>();
            foreach(Pool pool in pools)
            {
                GameObject obj = Instantiate(pool.prefab, hit.point, Quaternion.LookRotation(hit.normal));
                hitVFXPool.Enqueue(obj);
                // poolDictionary.Add(pool.tag , hitVFXPool);
            }
            hitVFXPool.Dequeue();
        }
    }

    void PassHitInfo(Collider collider)
    {
        m_reticle.AmmoGuagePop(); // OPTIMIZE this, reduse the engine calls
        collider.GetComponentInParent<EnemyHealth>().TakeDamage(gunDamage, collider);
        

        // // need to find a bettr way, this'll only work with ray cast
        // EnemyAI enemyAI = collider.GetComponentInParent<EnemyAI>(); // OPTIMIZE this, reduse the engine calls
        // enemyAI.ChaseTargetIfShot(hit);
    }

        
    void ManualReaload()
    {
        if (Input.GetKeyDown(KeyCode.R) && ammo.GetCurAmmo(ammoType) < m_maxAmmo)
        {
            StartCoroutine( Reload() );
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        // weaponSwitcher.SetCanSwitch(isReloading);
        m_reticle.SetReticlAlpha();
        yield return m_ReloadWait;
        int currentAmmo = ammo.GetCurAmmo(ammoType);
        if (currentAmmo > 0)
        {
            ammo.ReloadAmmo(ammoType, m_lastClip);
        }

        if (currentAmmo <= 0)
        {
            ammo.ReloadAmmo(ammoType, m_lastClip);
        }

        m_lastClip = 0;
        isReloading = false;

        m_reticle.SetReticlColor();
    }

    IEnumerator UpdateAmmoGaugeUI()
    {
        float currV = default;
        while (true)
        {
            m_currAmmoPercent = Mathf.SmoothDamp(
                m_currAmmoPercent, (float)ammo.GetCurAmmo(ammoType) / (float)m_maxAmmo,
                ref currV , 0.1f);

            Shader.SetGlobalFloat("_AmmoLeft", m_currAmmoPercent/* *0.75f */);
            yield return m_coroitineLerpWait;
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (iKTarget == null) return;

        Vector3 rayDir = (iKTarget.transform.position - rayOrigin.position).normalized; // raycast from Pistol
        bool rayHit = Physics.Raycast(rayOrigin.position, rayDir, out RaycastHit hit, 999f);
        
        Gizmos.color = Color.red;
        Gizmos.DrawLine(rayOrigin.position, hit.point);
        Gizmos.color = Color.yellow;
        // Gizmos.DrawSphere(hit.point, 0.1f);
    }
#endif

    void OnDisable()
    {
        isReloading = false;
    }
}
