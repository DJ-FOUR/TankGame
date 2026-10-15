using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class MyShellExplosion : MonoBehaviour
{
    public float MaxLifeTime = 2f;
    public ParticleSystem ExplosionParticles;
    public AudioSource ExplosionSound;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject,MaxLifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        //±¨’®Ãÿ–ß
        ExplosionParticles.transform.parent = null;
        ExplosionParticles.Play();
        //±¨’®“Ù–ß
        ExplosionSound.transform.parent = null;
        ExplosionSound.Play();
        //œ˙ªŸ±¨’®“Ù–ß≤–¡Ù
        Destroy(ExplosionSound.gameObject, ExplosionSound.clip.length);
        //œ˙ªŸ±¨’®Ãÿ–ß≤–¡Ù
        Destroy(ExplosionParticles.gameObject, ExplosionParticles.main.duration);
        //œ˙ªŸ≈⁄µØ
        Destroy(gameObject);
    }

}
