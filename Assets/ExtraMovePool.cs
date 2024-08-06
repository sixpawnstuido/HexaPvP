using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraMovePool : MonoBehaviour
{
    [SerializeField] private ExtraMove particlePrefab;
    [SerializeField] private int poolSize = 20; 

    private List<ExtraMove> poolList = new List<ExtraMove>();

    public float extraMoveTargetY;
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

    public ExtraMove GetParticle()
    {
        foreach (var particle in poolList)
        {
            if (!particle.gameObject.activeInHierarchy)
            {
                if(particle.extraMovePool is null) particle.extraMovePool = this;
                particle.gameObject.SetActive(true);
                return particle;
            }
        }

        var newParticle = Instantiate(particlePrefab,transform);
        poolList.Add(newParticle);
        return newParticle;
    }
}
