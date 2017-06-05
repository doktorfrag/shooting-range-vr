using UnityEngine;

public class Pedestal : MonoBehaviour
{
    //serialized variables
    [SerializeField]
    private GameObject _projectilePrefab;

    //public variables
    public float levitationSpeed = 0.25f;
    public float maxY = 1.4f;
    public float minY = 1.25f;


    //private variables
    private GameObject _projectile;
    private Projectile _projectileScript;
    private int _direction = 1;

    //private variable accessors

    void Update()
    {
        //put projectile on pedestal
        if (GameController.Instance.PedestalLoaded == false)
        {
            GameController.Instance.PedestalLoaded = true;
            _projectile = Instantiate(_projectilePrefab) as GameObject;
            _projectileScript = _projectile.GetComponent<Projectile>();
            _projectileScript.ProjectileGravity = false;
            _projectile.transform.position = transform.TransformPoint(Vector3.up * 0.6f);
            _projectile.transform.rotation = transform.rotation;
        }

        if (_projectile)
        {
            //move projectile up and down
            _projectile.transform.Translate(0, _direction * levitationSpeed * Time.deltaTime, 0);
            if (_projectile.transform.position.y > maxY || _projectile.transform.position.y < minY)
            {
                _direction = -_direction;
            }
        }
    }

    //void InstantiateProjectile()
    //{
    //    //instantiate projectile and place at top of pedestal
    //    _projectile = Instantiate(_projectilePrefab) as GameObject;
    //    _projectileScript = _projectile.GetComponent<Projectile>();
    //    _projectileScript.ProjectileGravity = false;
    //    _projectile.transform.position = transform.TransformPoint(Vector3.up * 0.6f);
    //    _projectile.transform.rotation = transform.rotation;

    //}
}
