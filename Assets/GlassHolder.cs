using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class GlassHolder : MonoBehaviour
{
    [SerializeField] private MeshRenderer fruitSliceMesh;

    [SerializeField] private MeshRenderer juiceMesh;

    [SerializeField] private GameObject holder;

    [SerializeField] private Animator animator;

    public void InitGlass(HexagonTypes type)
    {
        // var tmpMaterial = fruitSliceMesh.material;
        //
        // tmpMaterial = ResourceSystem.ReturnVisualData().Glass3DInfo[type];
        //
        // fruitSliceMesh.material = tmpMaterial;
        //
        // AudioManager.Instance.Play(AudioManager.AudioEnums.SkillHammer);
        //
        // MaterialPropertyBlock mbp = new MaterialPropertyBlock();
        //
        // mbp.SetColor("_BaseColor", ResourceSystem.ReturnVisualData().GlassInfos[type].color);
        //
        // juiceMesh.SetPropertyBlock(mbp);
        //
        // StartCoroutine(EndCor(type));
    }

    private IEnumerator EndCor(HexagonTypes type)
    {
        yield return new WaitForSeconds(1.5f);

        if (JuiceTargetUIController.Instance.ReturnJuiceTargetUIElement(type) == null)
        {
            animator.enabled = false;

            yield return new WaitForSeconds(.3f);

          //  holder.transform.DOMoveX(holder.transform.position.x + 5, 1f);
            holder.transform.DOScale(Vector3.zero ,.5f).SetEase(Ease.OutBack);
            Vector3 offset = new Vector3(0,2.2f,-1.15f);
            GoldPanel.Instance.ActivateGoldAnim(holder.transform.position+offset,2);
            AudioManager.Instance.Play(AudioManager.AudioEnums.Swish);
            yield return new WaitForSeconds(1f);


            Destroy(holder.gameObject);

            Destroy(this.gameObject);
        }
        else
        {
            Transform target = JuiceTargetUIController.Instance.ReturnJuiceTargetUIElement(type).glasRef;


            holder.transform.parent = target.parent;

            animator.enabled = false;

            holder.transform.DOLocalRotateQuaternion(target.transform.localRotation, .4f);

            holder.transform.DOLocalMove(target.transform.position, .4f);

            holder.transform.DOScale(Vector3.one * 8, .4f);

            yield return new WaitForSeconds(.4f);


            Destroy(holder.gameObject);

            Destroy(this.gameObject);
        }
    }
}