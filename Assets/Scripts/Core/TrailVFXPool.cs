using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailVFXPool : MonoBehaviour
{
    public static TrailVFXPool Instance;
    
    [SerializeField] private TrailVFX particlePrefab;
    [SerializeField] private int poolSize = 20; 

    private List<TrailVFX> poolList = new List<TrailVFX>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        FillPool();
    }

    private void FillPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            var particle = Instantiate(particlePrefab,transform);
            particle.gameObject.SetActive(false);
            poolList.Add(particle);
        }
    }

    public TrailVFX GetParticle()
    {
        foreach (var particle in poolList)
        {
            if (!particle.gameObject.activeInHierarchy)
            {
                particle.gameObject.SetActive(true);
                return particle;
            }
        }

        var newParticle = Instantiate(particlePrefab,transform);
        poolList.Add(newParticle);
        return newParticle;
    }
}
