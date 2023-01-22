using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPrefsSaveExample : MonoBehaviour
{
	public Text text;
	private string originalText;

	private void Start()
	{
		originalText = text.text;
		GetText();
	}

    public void SaveText(InputField inputField)
	{
		PlayerPrefs.SetString("mySavedText", inputField.text);
		GetText();
	}

	public void GetText()
	{
		text.text = originalText + " " + PlayerPrefs.GetString("mySavedText", "#NOTSET#");
	}
}
