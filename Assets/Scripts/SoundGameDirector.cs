﻿using UnityEngine;
using System.Collections;

//<summary>
// Main game controller for "Sound Game" scene
//</summary>

namespace WordTree
{
	public class SoundGameDirector : MonoBehaviour
	{
		private GestureManager gestureManager;
		//<summary>
		// called on start
		//</summary>
		void Start()
		{
			//Scale graphics to screen size
			Utilities.setCameraViewForScreen();
			//make reference to existing gestureManager 
			GestureManager gestureManager = GameObject.
				FindGameObjectWithTag(Constants.Tags.TAG_GESTURE_MANAGER).GetComponent<GestureManager>();
			if (gestureManager != null) 
			{
				// create sound blanks, letters, and word object
				LoadSoundGameWord(ProgressManager.currentWord);
				// subscribe buttons to gestures
				GameObject[] buttons = GameObject.FindGameObjectsWithTag(Constants.Tags.TAG_BUTTON);
				foreach (GameObject button in buttons) 
				{
					button.AddComponent<GestureManager>().AddAndSubscribeToGestures(button);
				}
				// sound out word
				GameObject[] tar = GameObject.FindGameObjectsWithTag(Constants.Tags.TAG_TARGET_LETTER);
				GameObject audioManager = GameObject.Find("AudioManager");
				if (audioManager != null) 
				{
					audioManager.GetComponent<AudioManager>().SpellOutWord(tar);
					// start pulsing sound blanks
					GameObject[] mov = GameObject.FindGameObjectsWithTag(Constants.Tags.TAG_MOVABLE_BLANK);
					foreach (GameObject go in mov) 
					{
						go.GetComponent<PulseBehavior>().StartPulsing(go, (tar.Length + 1) * AudioManager.clipLength);
					}
				} 
				else 
				{
					Debug.LogWarning("Cannot find auio manager");
					
				} 
			}
			else 
			{
				Debug.LogError("Cannot find gesture manager");
			}	
		}

		//<summary>
		// create all objects necessary for sound game
		// including sound blanks, letters, jars, and word object
		//</summary>
		void LoadSoundGameWord(string word)
		{
			// get properties of current word in game
			WordProperties prop = WordProperties.GetWordProperties(word);
			if (prop != null) 
			{
				// phonemes in word
				string[] phonemes = prop.Phonemes(); 
				// scale of word object
				float objScale = prop.ObjScale(); 
				// create sound blanks that are scrambled
				BlankCreation.CreateScrambledBlanks(word, phonemes, "Circle", "MovableBlank", "SoundGame");
				// create word object
				WordCreation.CreateWord(word, phonemes, "TargetLetter", "SoundGame");
				CreateWordImage(word, objScale);
				// create jars that "hold" each letter
			}
			else 
			{
				Debug.LogWarning("Cannot find word properties");
			}
			CreateJars();
			// make letters black color
			GameObject[] tar = GameObject.FindGameObjectsWithTag(Constants.Tags.TAG_TARGET_LETTER);
			foreach (GameObject go in tar)
			{
				go.GetComponent<SpriteRenderer>().color = Color.black;
			}
		}

		//<summary>
		// create word object
		//</summary>
		void CreateWordImage(string word, float scale)
		{
			float y = 3; // y-position of object
			// instantiate object
			ObjectProperties Obj = ObjectProperties.CreateInstance(word, "WordObject", new Vector3(0, y, 0), 
				new Vector3(scale, scale, 1), ProgressManager.currentLevel + "/" + word, "Words/" + word);
			ObjectProperties.InstantiateObject(Obj);
		}

		//<summary>
		// create jars
		//</summary>
		void CreateJars()
		{
			// find letters
			GameObject[] gos = GameObject.FindGameObjectsWithTag(Constants.Tags.TAG_TARGET_LETTER);
			foreach (GameObject go in gos)
			{
				// get position of letter
				Vector3 letterPosn = new Vector3(go.transform.position.x, go.transform.position.y, 1.5f);
				// instantiate jar behind letter
				ObjectProperties jar = ObjectProperties.CreateInstance("Jar", "Jar", letterPosn, 
					new Vector3(.45f, .45f, 1), "Jar", null);
				ObjectProperties.InstantiateObject(jar);
			}
		}

		//<summary>
		//Play ping sound when word is incorrect
		//</summary>
		// TODO: make animation more kid-friendly
		public static void TryAgainAnimation()
		{	
			// find red X object which has sound attached
			GameObject tryAgain = GameObject.Find("TryAgain");
			// play "ping" sound
			if (tryAgain != null) 
			{
				tryAgain.AddComponent<AudioSource>().clip = Resources.Load("Audio/IncorrectSound") as AudioClip;
				if (tryAgain.GetComponent<AudioSource>().clip != null) 
				{
					tryAgain.GetComponent<AudioSource>().Play();
				} 
				else
				{
					Debug.LogWarning("Tried to find TryAgain audio source but it couldn't be found");
				}
			} 
			else 
			{
				Debug.LogWarning("Tried to find TryAgain GameObject but it couldn't be found!");
			}
		}

		//<summary>
		// Animation played when user gets word right
		// Word object wiggles to a cheerful sound
		//</summary>
		public static void CelebratoryAnimation(float delayTime)
		{	
			float time = .3f; // time to complete one wiggle
			GameObject go = GameObject.FindGameObjectWithTag(Constants.Tags.TAG_WORD_OBJECT);
			// wiggle object sideways a few times
			Debug.Log("Wiggling " + go.name);
			LeanTween.rotateAround(go, Vector3.forward, 80f, time).setDelay(delayTime);
			LeanTween.rotateAround(go, Vector3.back, 160f, time).setDelay(delayTime + time);
			LeanTween.rotateAround(go, Vector3.forward, 160f, time).setDelay(delayTime + 2 * time);
			LeanTween.rotateAround(go, Vector3.back, 160f, time).setDelay(delayTime + 3 * time);
			LeanTween.rotateAround(go, Vector3.forward, 80f, time).setDelay(delayTime + 4 * time);
			// play cheerful sound
			Debug.Log("Playing clip for congrats");
			AudioSource audio = go.AddComponent<AudioSource>();
			audio.clip = Resources.Load("Audio/CongratsSound") as AudioClip;
			audio.PlayDelayed(delayTime);
		}

		//<summary>
		//called once per frame
		//</summary>
		void Update()
		{
			// if user presses escape or 'back' button on android, exit program
			if (Input.GetKeyDown(KeyCode.Escape))
				Application.Quit();
		}

	}
}
