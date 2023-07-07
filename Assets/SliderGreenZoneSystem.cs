using UnityEngine;

public class SliderGreenZoneSystem : MonoBehaviour
{
    public bool sliderInGreenZone = false;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("uh");
        if (other.gameObject.tag == "FreezerSlider")
        {
            this.sliderInGreenZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "FreezerSlider")
        {
            this.sliderInGreenZone = false;
        }
    }
}
