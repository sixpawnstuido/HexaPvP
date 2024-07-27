using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class TrailVFX : MonoBehaviour
{

    [SerializeField] private ParticleSystem trailVFX;
    
    [SerializeField] private List<AnimationCurve> trailMotionCurveList;
    
    [SerializeField] private Color32 blueColor;
    [SerializeField] private Color32 redColor;
    

    public void TrailMotion(PlayerType playerType)
    {
        float time = 0;
        var avatarElement = PvPController.Instance.ReturnAvatarElement(playerType);
        var avatarTarget = avatarElement.HeartImage;
        trailVFX.transform.localPosition = Vector3.zero;
        ChangeColor(playerType);
        var animationCurve = trailMotionCurveList[Random.Range(0, trailMotionCurveList.Count)];
        trailVFX.transform.DOMove(avatarTarget.transform.position, .45f)
            .OnUpdate(() =>
            {
                time += Time.deltaTime * (1 / .45f);
                float xOffset = animationCurve.Evaluate(time);
                trailVFX.transform.position = new Vector3(trailVFX.transform.position.x - xOffset, trailVFX.transform.position.y, trailVFX.transform.position.z);
            })
            .OnComplete(() =>
            {
                avatarElement.TrailArrivedState();
                DOVirtual.DelayedCall(0.5f, () => gameObject.SetActive(false));
            });
    }



    public void ChangeColor(PlayerType playerType)
    {
        var mainModule = trailVFX.main;
        Color newColor = playerType==PlayerType.OPPONENT ? redColor : blueColor;
        mainModule.startColor = newColor;
    }
}
