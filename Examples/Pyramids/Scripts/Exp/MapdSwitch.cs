using UnityEngine;

public class MapdSwitch : MonoBehaviour
{
    public Material onMaterial;
    public Material offMaterial;
    public GameObject myButton;
    public string indexButton;
    GameObject m_Area;
    MapdArea m_AreaComponent;

    void Start()
    {
        m_Area = gameObject.transform.parent.gameObject;
        m_AreaComponent = m_Area.GetComponent<MapdArea>();
        ResetSwitch();
    }

    public void ResetSwitch()
    {
        tag = "wall";
        myButton.GetComponent<Renderer>().material = offMaterial;
    }


    public void EnableSwitch()
    {
        tag = "switch_" + indexButton;
        myButton.GetComponent<Renderer>().material = onMaterial;
    }
}
