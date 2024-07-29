using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinusTextPool : MonoBehaviour
{
    [SerializeField] private MinusText particlePrefab;
    [SerializeField] private int poolSize = 20; 

    private List<MinusText> poolList = new List<MinusText>();

    private void Awake()
    {
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

    public MinusText GetParticle()
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
