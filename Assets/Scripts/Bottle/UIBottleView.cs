using UnityEngine;
using UnityEngine.UI;
using DoozyUI;

public class UIBottleView : MonoBehaviour
{
    [SerializeField]
    private UIElement inGameUI;
    [SerializeField]
    private Image waterImage;
    [SerializeField]
    private Text waterText;

    [SerializeField]
    private Slider powerSlider;
    [SerializeField]
    private Text powerText;

    [SerializeField]
    private RectTransform angleBar;

	[SerializeField]
	private ParticleSystem waterParticle;

    public void SetWaterBar(int water)
    {
        waterImage.fillAmount = (float)water / 100;
        waterText.text = water + "%";
    }
    public void SetPowerBar(float power, float max, float min)
    {
        powerSlider.value = ((float)power - min) / (max - min);
        powerText.text = ((((float)power - min) / (max - min)) * 100f).ToString("00");
    }
    public void SetAngleBar(float angle)
    {
        angleBar.eulerAngles = new Vector3(0, 0, angle);
    }
    public void SetInGameUI(bool show) {
		if (show)
			inGameUI.Show (false);
		else
			inGameUI.Hide (false);
    }

	public void PlayWaterParticle(bool play)
	{
		if (play)
			waterParticle.Play ();
		else
			waterParticle.Stop ();
	}
}
