using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialMetaAnimation : MonoBehaviour
{
    public GameObject holder;

    public Image blackBG;

    public GameObject stackTutReal;
    public GameObject stackTutDummy;

    public GameObject mergeTutReal;
    public GameObject mergeTutDummy;

    public GameObject circleDummy;

    public Image circle;

    public List<GameObject> fruits;
    public List<Vector3> scales;

    public GameObject circleTarget;

    public GameObject tapToContinueText;

    private void OnEnable()
    {
        StartCoroutine(AnimationStartCor());
    }

    private IEnumerator AnimationStartCor()
    {
        for (int i = 0; i < fruits.Count; i++)
        {
            scales.Add(fruits[i].transform.localScale);
            fruits[i].transform.localScale = Vector3.zero;
        }

        yield return new WaitForSeconds(.5f);
        blackBG.DOFade(.84f, .3f);

        yield return new WaitForSeconds(.3f);

        stackTutReal.gameObject.SetActive(true);
        stackTutReal.transform.DOMove(stackTutDummy.transform.position, .3f);

        yield return new WaitForSeconds(2);

        tapToContinueText.gameObject.SetActive(true);
        tapToContinueText.transform.localScale = Vector3.zero;
        tapToContinueText.transform.DOScale(Vector3.one * 2.15f, .3f).SetEase(Ease.OutBack);

        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Mouse0));

        tapToContinueText.gameObject.SetActive(false);
        tapToContinueText.transform.localScale = Vector3.zero;

        stackTutReal.SetActive(false);

        mergeTutReal.SetActive(true);
        mergeTutReal.transform.DOMove(mergeTutDummy.transform.position, .3f);

        yield return new WaitForSeconds(3);

        circle.DOFillAmount(1, .3f);
        yield return new WaitForSeconds(.3f);

        for (int i = 0; i < fruits.Count; i++)
        {
            Vector3 tmpScale = scales[i];
            fruits[i].transform.DOScale(tmpScale, .2f).SetEase(Ease.OutBack);

            yield return new WaitForSeconds(.05f);
        }
        yield return new WaitForSeconds(1);

        tapToContinueText.gameObject.SetActive(true);
        tapToContinueText.transform.localScale = Vector3.zero;
        tapToContinueText.transform.DOScale(Vector3.one * 2.15f, .3f).SetEase(Ease.OutBack);

        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Mouse0));

        circleDummy.SetActive(true);

        circleDummy.transform.DOMove(circleTarget.transform.position, .7f).OnComplete(delegate { circleTarget.gameObject.SetActive(true); circleDummy.SetActive(false); });
        circleDummy.transform.DOScale(Vector3.one * 1.01f, .7f);

        holder.SetActive(false);
    }
}
