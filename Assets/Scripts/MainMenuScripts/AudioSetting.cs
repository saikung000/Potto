using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioSetting : MonoBehaviour {
	
	public AudioSource[] BGMAudioSource;
	public AudioSource[] SFXAudioSource;

	public Slider BGMLevelSlider;
	public Slider BGMEnable;

	public Slider SFXLevelSlider;
	public Slider SFXEnable;

	public Text BGMLevelText;
	public Text BGMEnableText;

	public Text SFXLevelText;
	public Text SFXEnableText;

	private bool EnableSFX;
	private bool EnableBGM;

	private void Start(){
		EnableSFX = true;
		EnableBGM = true;
	}

	private void SetVolumeSFX(float value){

		for (int i = 0; BGMAudioSource.Length > i; i++) {

			SFXAudioSource [i].volume = value;

		}

	}
	private void SetVolumeBGM(float value){

		for (int i = 0; BGMAudioSource.Length > i; i++) {

			BGMAudioSource [i].volume = value;

		}

	}


	private void ApplyVolume(){
		
	}

	public void UseSliderLevelSFX(){
		if (EnableSFX = true) {
			SetVolumeSFX (SFXLevelSlider.value);
		}

	}

	public void UseSliderLevelBGM(){
		if (EnableBGM = true) {
			SetVolumeBGM (BGMLevelSlider.value);
		}
	}

	public void UseEnableSFX(){
		if (SFXEnable.value == 0) {
			
			SFXEnableText.text = "OFF";
			EnableSFX = false;

			SetVolumeSFX (0);
		} else {
			
			SFXEnableText.text = "On";
			EnableSFX = true;

			SetVolumeSFX (SFXLevelSlider.value);
		}
	}

	public void UseEnableBGM(){
		if (BGMEnable.value == 0) {
			
			BGMEnableText.text = "Off";
			EnableBGM = false;

			SetVolumeBGM (0);
		} else {
			
			BGMEnableText.text = "On";
			EnableBGM = true;

			SetVolumeBGM (BGMLevelSlider.value);
		}
	}
}
