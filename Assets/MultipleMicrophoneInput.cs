using UnityEngine;
using System.Collections;

[RequireComponent (typeof (AudioSource))]

public class MultipleMicrophoneInput : MonoBehaviour 
{
	//A boolean that flags whether there's a connected microphone
	private bool micConnected = false;
	
	//The maximum and minimum available recording frequencies
	private int[] minFreqs;
	private int[] maxFreqs;
	
	//A handle to the attached AudioSource
	private AudioSource goAudioSource;
	
	//The current microphone
	private int currentMic = 0;
	
	//The selected microphone
	private int selectedMic = 0;
	
	//An integer that stores the number of connected microphones
	private	int numMics;
	
	//A boolean that flags whether the audio capture is active or not
	private bool recActive = false;
	
	//Use this for initialization
	void Start() 
	{
		//An integer that stores the number of connected microphones
		numMics = Microphone.devices.Length;
		
		//Check if there is at least one microphone connected
		if(numMics <= 0)
		{
			//Throw a warning message at the console if there isn't
			Debug.LogWarning("No microphone connected!");
		}
		else //At least one microphone is present
		{
			//Set 'micConnected' to true
			micConnected = true;
			
			//Initialize the minFreqs and maxFreqs array to hold the same number of integers as there are microphones
			minFreqs = new int[numMics];
			maxFreqs = new int[numMics];
			
			//Get the recording capabilities of each microphone
			for(int i=0; i < numMics; i++)
			{
				Microphone.GetDeviceCaps(Microphone.devices[i], out minFreqs[i], out maxFreqs[i]);
				
				//According to the documentation, if both minimum and maximum frequencies are zero, the microphone supports any recording frequency...
				if(minFreqs[i] == 0 && maxFreqs[i] == 0)
				{
					//...meaning 44100 Hz can be used as the recording sampling rate for the current microphone
					maxFreqs[i] = 44100;
				}
			}
			
			//Get the attached AudioSource component
			goAudioSource = this.GetComponent<AudioSource>();
		}
	}
	
	void OnGUI() 
	{
		//If there is at least as single microphone connected
		if(micConnected)
		{
			//For the current selected microphone, check if the audio is being captured
			recActive = Microphone.IsRecording(Microphone.devices[currentMic]);
			
			//If the audio from the current microphone isn't being recorded
			if(recActive == false)
			{
				//Case the 'Record' button gets pressed
				if(GUI.Button(new Rect(Screen.width/2-100, Screen.height/2-25, 200, 50), "Record"))
				{
					//Start recording and store the audio captured from the selected microphone at the AudioClip in the AudioSource
					goAudioSource.clip = Microphone.Start(Microphone.devices[currentMic], true, 20, maxFreqs[currentMic]);
				}
			}
			else //Recording is in progress
			{
				//Case the 'Stop and Play' button gets pressed
				if(GUI.Button(new Rect(Screen.width/2-100, Screen.height/2-25, 200, 50), "Stop and Play!"))
				{
					Microphone.End(Microphone.devices[currentMic]); //Stop the audio recording
					goAudioSource.Play(); //Playback the recorded audio
				}
				
				GUI.Label(new Rect(Screen.width/2-100, Screen.height/2+25, 200, 50), "Recording in progress...");
			}
			
			//Disable the mic SelectionGrid if a recording is in progress
			GUI.enabled = !recActive;
			
			//Render the SelectionGrid listing all the microphones and save the selected one at 'selectedMic'
			selectedMic = GUI.SelectionGrid(new Rect(Screen.width/2-210, Screen.height/2+50,420,50), currentMic, Microphone.devices, 1);
			
			//If the selected microphone isn't the current microphone
			if(selectedMic != currentMic)
			{
				//Assign the value of currentMic to selectedMic
				currentMic = selectedMic;
			}
			
		}
		else // No microphone
		{
			//Print a red "No microphone connected!" message at the center of the screen
			GUI.contentColor = Color.red;
			GUI.Label(new Rect(Screen.width/2-100, Screen.height/2-25, 200, 50), "No microphone connected!");
		}
	}
}