using UnityEngine;

public class Actor_Pistol : InterfaceBase_IItem
{
    [Header("Shoot Options")]
    public Transform FirePoint;
    public GameObject Bullet;
    public float BulletSpeed = 100f;
    public float BulletDamage = 1f;

    [Header("Audio Options")]
    public AudioClip ShootSound;
    [Range(0f, 1f)] public float ShootVolume = 1f;
    public AudioClip ReloadSound;
    [Range(0f, 1f)] public float ReloadVolume = 1f;

    [Header("Ammo Options")]
    public int MagazineSize = 7;
    public int CurrentAmmo = 7;
    public int ReserveAmmo = 21;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;
        ShootSound?.LoadAudioData();
        ReloadSound?.LoadAudioData();
    }

    public override void OnEquip(GameObject itemHolder)
    {
        base.OnEquip(itemHolder);
        UpdateAmmoUI();
    }

    public override void OnUnEquip(GameObject sender)
    {
        base.OnUnEquip(sender);

        if (AmmoUI.Instance != null)
        {
            AmmoUI.Instance.HideAmmo();
        }
    }

    public override void OnUse()
    {
        Fire();
    }

    public override void OnReload()
    {
        Reload();
    }

    void Fire()
    {
        if (CurrentAmmo <= 0)
        {
            Debug.Log("[Pistol] No ammo. Reload!");
            return;
        }

        CurrentAmmo--;
        UpdateAmmoUI();
        PlayShootSound();

        Vector3 pos = FirePoint.position;
        Quaternion dir = FirePoint.rotation;

        GameObject bulletClone = Instantiate(Bullet, pos, dir);
        bulletClone.GetComponent<Actor_Bullet>().SetDamage(BulletDamage);

        Rigidbody rb = bulletClone.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(FirePoint.forward * BulletSpeed, ForceMode.VelocityChange);
        }

        Destroy(bulletClone, 2f);
    }

    void PlayShootSound()
    {
        if (ShootSound == null || audioSource == null) return;

        audioSource.PlayOneShot(ShootSound, ShootVolume);
    }

    void Reload()
    {
        if (CurrentAmmo >= MagazineSize) return;
        if (ReserveAmmo <= 0) return;

        int needAmmo = MagazineSize - CurrentAmmo;
        int reloadAmmo = Mathf.Min(needAmmo, ReserveAmmo);

        CurrentAmmo += reloadAmmo;
        ReserveAmmo -= reloadAmmo;

        UpdateAmmoUI();
        PlayReloadSound();

        Debug.Log($"[Pistol] Reloaded: {CurrentAmmo}/{ReserveAmmo}");
    }

    void PlayReloadSound()
    {
        if (ReloadSound == null || audioSource == null) return;

        audioSource.PlayOneShot(ReloadSound, ReloadVolume);
    }

    void UpdateAmmoUI()
    {
        if (AmmoUI.Instance != null)
        {
            AmmoUI.Instance.ShowAmmo(CurrentAmmo, ReserveAmmo);
        }
    }

    public void AddReserveAmmo(int amount)
    {
        ReserveAmmo += amount;
        UpdateAmmoUI();

        Debug.Log($"[Pistol] Ammo picked up: +{amount}. ReserveAmmo={ReserveAmmo}");
    }
}
