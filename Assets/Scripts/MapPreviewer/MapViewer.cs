using UnityEngine;

public class MapViewer : MonoBehaviour
{
    public GameObject mapRoot;
    void Update(){
        // if M pressed then show
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (mapRoot != null)
            {
                mapRoot.SetActive(!mapRoot.activeSelf);
            }
        }
        else{
            if(mapRoot!=null && mapRoot.activeSelf)
            {
                mapRoot.SetActive(false);
            }
        }
    }
}