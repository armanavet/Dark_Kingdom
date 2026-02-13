using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Portal : MonoBehaviour
{
    [SerializeField] public Tile SpawnTile;
    [SerializeField] int id;
    [Header("Set Portal Layer")]
    [SerializeField] string portalLayerName = "Portal";
    [Header("Set Portal Visual Parameters")]
    [SerializeField] Gradient emissionColor;
    [SerializeField] ParticleSystem orbParticlesL, orbParticlesR, fireParticles;
    //[SerializeField] public AudioSource gateAudio, screamAudio, orbLAudio, orbRAudio, fireAudio;
    //It needs a sound data for clips
    [SerializeField] Light gateLight;
    [SerializeField] Renderer gateRenderer, gateEffectRenderer;

    [HideInInspector] public List<GameObject> path;
    
    Material gateMaterial, gateEffectMaterial;
    GameObject gateEffectObj;
    float gateLightMaxIntencity = 5f;
    //gateAudioMaxVolume = 0.3f,
    //fireAudioMaxVolume = 0.6f;
    bool hellGateOn;
    public int ID => id;

    #region Singleton 
    private static Portal _instance;
    public static Portal Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<Portal>();
            }

            return _instance;
        }
    }
    private void Awake()
    {
        _instance = this;
        gateEffectObj = gateEffectRenderer.gameObject;
        gateEffectMaterial = gateEffectRenderer.material;

        gateMaterial = gateRenderer.material;

        gateEffectObj.SetActive(false);
        gateLight.gameObject.SetActive(false);

        gateEffectMaterial.SetFloat("_Alpha", 0);
        gateMaterial.SetColor("_EmissionColor", emissionColor.Evaluate(0));
        gateLight.intensity = 0;
    }
    #endregion
    private void Start()
    {
        if (WaveManager.Instance.portalMode == PortalMode.BossMode)
        {
            gameObject.layer = LayerMask.NameToLayer(portalLayerName);
        }

        //gateEffectObj = gateEffectRenderer.gameObject;
        //gateEffectMaterial = gateEffectRenderer.material;

        //gateMaterial = gateRenderer.material;

        //gateEffectObj.SetActive(false);
        //gateLight.gameObject.SetActive(false);

        //gateEffectMaterial.SetFloat("_Alpha", 0);
        //gateMaterial.SetColor("_EmissionColor", emissionColor.Evaluate(0));
        //gateLight.intensity = 0;
    }
    
    public IEnumerator ActivateVisual()
    {

        float transitionTimer = 0;
        float rand1 = Random.Range(0.2f, 0.8f);
        float rand2 = Random.Range(0.2f, 0.8f);

        while (transitionTimer < 1f)
        {
            transitionTimer += Time.deltaTime * 0.5f;

            if (transitionTimer >= rand1 && !orbParticlesL.isPlaying)
            {
                orbParticlesL.Play();
                //orbLAudio.Play();
            }

            if (transitionTimer >= rand2 && !orbParticlesR.isPlaying)
            {
                orbParticlesR.Play();
                //orbRAudio.Play();
            }

            gateMaterial.SetColor("_EmissionColor", emissionColor.Evaluate(transitionTimer));

            yield return null;
        }

        gateMaterial.SetColor("_EmissionColor", emissionColor.Evaluate(1f));

        yield return StartCoroutine(ActivateGate());
    }

    public IEnumerator ActivateGate()
    {
        float transitionTimer = 0;

        gateEffectObj.SetActive(true);
        gateLight.gameObject.SetActive(true);

        fireParticles.Play();
        //fireAudio.volume = fireAudioMaxVolume;
        //fireAudio.Play();
        //screamAudio.Play();
        //gateAudio.Play();

        while (transitionTimer < 1f)
        {
            transitionTimer += Time.deltaTime * 0.21f;

            gateEffectMaterial.SetFloat("_Alpha", 1f - transitionTimer * 0.75f);

            gateLight.intensity = transitionTimer * gateLightMaxIntencity;
            //gateAudio.volume = transitionTimer * gateAudioMaxVolume;

            yield return null;
        }

        gateEffectMaterial.SetFloat("_Alpha", 0f);
        gateLight.intensity = gateLightMaxIntencity;
        //gateAudio.volume = gateAudioMaxVolume;

        //yield return new WaitForSeconds(1f);
    }

    public IEnumerator DeactivateVisual()
    {
        float transitionTimer = 1;

        orbParticlesL.Stop();
        orbParticlesR.Stop();


        while (transitionTimer > 0)
        {
            transitionTimer -= Time.deltaTime * 0.3f;

            gateMaterial.SetColor("_EmissionColor", emissionColor.Evaluate(transitionTimer));

            gateEffectMaterial.SetFloat("_Alpha", 1f - transitionTimer);
            gateLight.intensity = transitionTimer * gateLightMaxIntencity;
            //gateAudio.volume = transitionTimer * gateAudioMaxVolume;
            //fireAudio.volume = transitionTimer * fireAudioMaxVolume;
            yield return null;
        }

        fireParticles.Stop();
        gateMaterial.SetColor("_EmissionColor", emissionColor.Evaluate(0f));
        gateEffectObj.SetActive(false);
        gateLight.gameObject.SetActive(false);
        //gateAudio.Stop();
        //fireAudio.Stop();
        //WaveManager.Instance.isPortalOff = true;
        yield break;
    }
}
