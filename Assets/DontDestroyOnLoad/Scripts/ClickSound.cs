using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ClickSound : MonoBehaviour
{
    public AudioClip sound;

	private Button button = null;
	private Button Button
	{
		get { if(!button) button = GetComponent<Button>(); return button; }
	}

	public AudioSource source;

	private void Start()
	{
		source.playOnAwake = false;
		Button.onClick.AddListener(() => PlaySound());
	}

	private void PlaySound()
	{
		source.clip = sound;
		source.Play();
	}
}
