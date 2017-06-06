using System;
using VRTK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Projectile : MonoBehaviour
{
    //serialized variables
    [SerializeField]
    private GameObject _explosionPrefab;

    //public variables
    public float explosionRadius = 1.5f;
    public float explosivePower = 20.0f;

    //private variables
    private GameObject _explosion;
    private bool _targetHit;
    private bool _targetNearHit;
    private int _projectileNumber;
    private List<string> _projectileData = new List<string>();

    //private variable accessors
    private bool _projectileThrown = false;
    public bool ProjectileFired
    {
        get
        {
            return _projectileThrown;
        }

        set
        {
            _projectileThrown = value;
        }
    }

    private bool _projectileGravity;
    public bool ProjectileGravity
    {
        get
        {
            Collider coll = GetComponent<Collider>();
            _projectileGravity = coll.attachedRigidbody.useGravity;
            return _projectileGravity;
        }

        set
        {
            Collider coll = GetComponent<Collider>();
            coll.attachedRigidbody.useGravity = value;
        }
    }

    //methods
    private void Awake()
    {
        //make sure VRTK scripts are attached
        if (GetComponent<VRTK_InteractableObject>() == null)

        {
            Debug.LogError("Projectile must have the VRTK_InteractableObject script attached to it");
            return;
        }

        //create event handlers for VR controllers
        GetComponent<VRTK_InteractableObject>().InteractableObjectGrabbed += new InteractableObjectEventHandler(ObjectGrabbed);
        GetComponent<VRTK_InteractableObject>().InteractableObjectUngrabbed += new InteractableObjectEventHandler(ObjectThrown);
    }

    private void Update()
    {
        //track trajectory data from moment it leaves
        //the hand to the moment it explodes
        if (_projectileThrown == true)
        {
            //grab projectile data
            string projectileNumber = _projectileNumber.ToString();
            double dateReturn = Math.Round((DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds);
            string timestamp = dateReturn.ToString();
            string projectileX = transform.position.x.ToString();
            string projectileY = transform.position.y.ToString();
            string projectileZ = transform.position.z.ToString();
            string projectilePosition = projectileNumber + "," + timestamp + "," + projectileX + "," + projectileY + "," + projectileZ;

            //save projectile data as strong to list on each update
            _projectileData.Add(projectilePosition);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if projectile touches target collider, mark as hit
        if (collision.gameObject.name == "Target" || collision.gameObject.name == "Target(Clone)")
        {
            GameController.Instance.TargetsHit = 1;
            _targetHit = true;
        }

        //if (collision.gameObject.name == "Floor")
        //{
        //    Debug.Log("Landed on floor");
        //}
    }

    public void Shoot()
    {
        StartCoroutine(Explode());
    }

    private void ObjectGrabbed(object sender, InteractableObjectEventArgs e)
    {
        //once object is grabbed from pedestal, turn gravity on
        ProjectileGravity = true;
    }

    private void ObjectThrown(object sender, InteractableObjectEventArgs e)
    {
        //once projectile has been thrown or dropped,
        //update GameContoller private variable accessors,
        //load new projectile on pedestal, and start explosion sequence
        _projectileThrown = true;
        GameController.Instance.ShotsFired = 1;
        GameController.Instance.PedestalLoaded = false;
        _projectileNumber = GameController.Instance.ShotsFired;
        StartCoroutine(Explode());
    }

    private IEnumerator Explode()
    {
        //wait for projectile to blow up
        yield return new WaitForSeconds(3);

        //destroy projectile and explosion
        Vector3 explosionPos = transform.position;
        _explosion = Instantiate(_explosionPrefab) as GameObject;
        _explosion.transform.position = explosionPos;
        AudioSource audio = GetComponent<AudioSource>();
        audio.Play();
        Destroy(gameObject);
        Destroy(_explosion, 4.0f);

        //see what explosion influences
        Collider[] colliders = Physics.OverlapSphere(explosionPos, explosionRadius);
        foreach (Collider hit in colliders)
        {
            //if has rigidbody, is tagret, and not projectile
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            Target target = hit.GetComponent<Target>();
            if (rb != null && rb.tag != "Projectile")
            {
                //blow it up
                float upwardsModifier = UnityEngine.Random.Range(2.0f, 4.0f);
                rb.AddExplosionForce(explosivePower, explosionPos, explosionRadius, upwardsModifier, ForceMode.Impulse);
                if (target)
                {
                    //tell target to blow itself up
                    target.ExplodeTarget();
                    _targetNearHit = true;
                }
            }
        }
        //grab data on whether projectile hits target
        //or explodes within 1.5 of target, and write to file
        _projectileData.Add("Target hit: "+_targetHit.ToString());
        _projectileData.Add("Target near hit: " + _targetNearHit.ToString());
        GameController.Instance.WriteData = _projectileData;
    }
}
