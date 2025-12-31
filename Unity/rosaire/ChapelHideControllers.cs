
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ChapelHideControllers : MonoBehaviour
{
    [Header("���� XR Rays")]
    public XRRayInteractor leftRay;
    public XRRayInteractor rightRay;

    [Header("���� ��Ʈ�ѷ� ��")]
    public GameObject leftControllerModel;
    public GameObject rightControllerModel;

    
    public void OnClickHide()
    {
      
        if (leftRay) leftRay.gameObject.SetActive(false);
        if (rightRay) rightRay.gameObject.SetActive(false);

       
        if (leftControllerModel) leftControllerModel.SetActive(false);
        if (rightControllerModel) rightControllerModel.SetActive(false);

        Debug.Log("[ChapelHideControllers] ������ ���� �������� �� ��Ʈ�ѷ�/���� ����");
    }
}
