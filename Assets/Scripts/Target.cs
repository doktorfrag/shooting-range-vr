using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

[RequireComponent(typeof(AudioSource))]
public class Target : MonoBehaviour {
    //serialized variables
    [SerializeField]
    private GameObject _targetExplosionPrefab;
    private List<string> _targetData = new List<string>();

    //private variables
    private GameObject _explosion;

    //methods
    private void Start()
    {
        double dateReturn = Math.Round((DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds);
        string timestamp = dateReturn.ToString();
        string targetX = transform.position.x.ToString();
        string targetY = transform.position.y.ToString();
        string targetZ = transform.position.z.ToString();
        string targetPosition = timestamp + "," + targetX + "," + targetY + "," + targetZ;
        _targetData.Add("----------");
        _targetData.Add("Target: " + targetPosition);
        GameController.Instance.WriteData = _targetData;
    }
    public void ExplodeTarget()
    {
        StartCoroutine(Explode());
    }

    private IEnumerator Explode()
    {
        //destroy target and explosion
        yield return new WaitForSeconds(2);
        _explosion = Instantiate(_targetExplosionPrefab) as GameObject;
        _explosion.transform.position = transform.position;
        Destroy(gameObject);
        Destroy(_explosion, 4.0f);
        AudioSource audio = _explosion.GetComponent<AudioSource>();
        audio.Play();
        GameController.Instance.TargetDestroyed = true;
    }
}

