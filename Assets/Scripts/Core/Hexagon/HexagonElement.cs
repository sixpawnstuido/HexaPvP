using DG.Tweening;
using FIMSpace.BonesStimulation;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class HexagonElement : MonoBehaviour
{
    public HexagonTypes hexagonType;

    [SerializeField] private GameObject _hexagonMesh;

    private BonesStimulator[] _boneStimulators;

    public ParticleSystem fruitSmashVFX;

    [FormerlySerializedAs("_slices")] public List<Rigidbody> slices;

    private Collider _hexagonElementCollider;

    [SerializeField] private SpriteRenderer icon;
    private void Awake()
    {
        _boneStimulators = GetComponentsInChildren<BonesStimulator>();
        _hexagonElementCollider = GetComponent<Collider>();
    }

    private void Start()
    {
        StimulatorAmount(0f);
        SqueezAmount(0);
    }

    public void StimulatorAmount(float stimulatorAmount)
    {
        for (int i = 0; i < _boneStimulators.Length; i++)
        {
            _boneStimulators[i].StimulatorAmount = stimulatorAmount;
        }
    }

    public void RotationAmount(float rotationAmount)
    {
        for (int i = 0; i < _boneStimulators.Length; i++)
        {
            _boneStimulators[i].RotationSpaceMuscles = rotationAmount;
        }
    }

    public void MovementMuscleAmount(float movementMuscleAmount)
    {
        for (int i = 0; i < _boneStimulators.Length; i++)
        {
            _boneStimulators[i].MovementMuscles = movementMuscleAmount;
        }
    }

    public void SqueezAmount(float squeezAmount, float tweenTime = 0f)
    {
        for (int i = 0; i < _boneStimulators.Length; i++)
        {
            _boneStimulators[i].SqueezingAmount = squeezAmount;
        }
    }

    public void ClearSlotHintState()
    {
        StartCoroutine(ClearSlotHintStateCor());

        IEnumerator ClearSlotHintStateCor()
        {
            _hexagonMesh.SetActive(false);
            var fruitVFX = Instantiate(fruitSmashVFX,transform.position,fruitSmashVFX.transform.rotation);
            fruitVFX.gameObject.SetActive(true);
            slices[0].transform.parent.gameObject.SetActive(true);
            slices = slices.OrderBy(g => g.transform.localPosition.x).ToList();
            for (int i = 0; i < slices.Count; i++)
            {
                slices[i].gameObject.SetActive(true);
              //  slices[i].AddForce(i==0 ? Vector3.left*50: Vector3.right*50);
                slices[i].transform
                    .DOLocalMoveX(i==0 ? slices[i].transform.localPosition.x-.1f:slices[i].transform.localPosition.x+.1f,.1f)
                    .SetEase(Ease.Flash);
            }
            yield return new WaitForSeconds(0.6f);
            for (int i = 0; i <slices.Count ; i++)
            {
                slices[i].transform.DOScale(Vector3.zero, .2f)
                    .SetEase(Ease.Flash);
            }
            yield return new WaitForSeconds(0.3f);
            Destroy(gameObject);
        }
    }
}