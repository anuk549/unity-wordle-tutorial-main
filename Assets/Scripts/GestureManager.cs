﻿using UnityEngine;
using System;
using System.Collections;
using TouchScript.Gestures;
using TouchScript.Behaviors;
using TouchScript.Hit;

//<summary>
// Manage gesture events
// Governs what happens when certain gestures are used (tap, press, release, pan/drag)
//</summary>
namespace WordTree
{
	public class GestureManager : MonoBehaviour
	{
		//create rectangle for screen boundaries
		private Rect cameraRect;
		//create reference for most recent object
		private GameObject recentObj;

		//<summary>
		// Called on start, used to initialize stuff
		//</summary>
		public void Start()
		{
			// store camera parameters for adjusting screen size
			//convert camera parameters to world view for calculations
			Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(Vector3.zero);
			Vector3 topRight = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, 
				                   Camera.main.pixelHeight));
			//creates rectangle to limit where objects can go on the screen
			this.cameraRect = new Rect(bottomLeft.x, bottomLeft.y, topRight.x - bottomLeft.x, 
				topRight.y - bottomLeft.y);
		}

		//<summary>
		// subscribes an object to all relevant gestures, according to its tag
		//</summary>
		public void AddAndSubscribeToGestures(GameObject go)
		{
			if (go.tag == Constants.Tags.TAG_LEVEL_ICON || go.tag == Constants.Tags.TAG_WORD_OBJECT
					|| go.tag == Constants.Tags.TAG_BUTTON || go.tag == Constants.Tags.TAG_KID 
					|| go.tag == Constants.Tags.TAG_LOCK) 
			{
				// add tap gesture component
				TapGesture tg = go.AddComponent<TapGesture>();
				if (tg != null) 
				{
					// subscribe to tap events
					tg.Tapped += tappedHandler;
					Debug.Log(go.name + " subscribed to tap events");
				} 
				else 
				{
					Debug.LogError("Cannot find tap gesture");
				}

			}
			if (go.tag == Constants.Tags.TAG_MOVABLE_LETTER || go.tag == Constants.Tags.TAG_MOVABLE_BLANK) 
			{
				// add pan gesture component
				TransformGesture pg = go.AddComponent<TransformGesture>();
				pg.CombineTouchesInterval = 0.2f;
				if (pg != null) 
				{
					// subscribe to pan events
					pg.TransformStarted += panStartedHandler;
					pg.Transformed += pannedHandler;
					pg.TransformCompleted += panCompleteHandler;
					Debug.Log(go.name + " subscribed to pan events");
				} 
				else 
				{
					Debug.LogError("Cannot find transform gesture");
				}
				// add press gesture component
				PressGesture prg = go.AddComponent<PressGesture>();
				if (prg != null) 
				{
					// subscribe to press events
					prg.Pressed += pressedHandler;
					Debug.Log(go.name + " subscribed to press events");
				} 
				else 
				{
					Debug.LogError("Cannot find press gesture");
				}
				// add release gesture component
				ReleaseGesture rg = go.AddComponent<ReleaseGesture>();
				if (rg != null) 
				{
					// subscribe to release events
					rg.Released += releasedHandler;
					Debug.Log(go.name + " subscribed to release events");
				} 
				else 
				{
					Debug.LogError("Cannot find release gesture");
				}
				// add transformer component so object automatically moves on drag
				go.AddComponent<Transformer>();
			}
			if (go.tag == Constants.Tags.TAG_TARGET_LETTER || go.tag == Constants.Tags.TAG_TARGET_BLANK) 
			{
				// add press gesture component
				PressGesture prg = go.AddComponent<PressGesture>();
				if (prg != null) 
				{
					// subscribe to press events
					prg.Pressed += pressedHandler;
					Debug.Log(go.name + " subscribed to press events");
				}
				else 
				{
					Debug.LogError("Cannot find press gesture");
				}
			}
		}

		//<summary>
		// enable all gesture events
		//</summary>
		public void EnableGestures(GameObject go)
		{
			// enable tap events
			TapGesture tg = go.GetComponent<TapGesture>();
			if (tg != null) 
			{
				tg.enabled = true;
			}
			// enable pan events
			TransformGesture pg = go.GetComponent<TransformGesture>();
			if (pg != null)
			{
				pg.enabled = true;
			}
			// enable press events
			PressGesture prg = go.GetComponent<PressGesture>();
			if (prg != null) 
			{
				prg.enabled = true;
			}
			// enable release events
			ReleaseGesture rg = go.GetComponent<ReleaseGesture>();
			if (rg != null) 
			{
				rg.enabled = true;
			}
			Debug.Log("Enabled gestures for " + go.name);
		}

		//<summary>
		// disable all gesture events
		//</summary>
		public void DisableGestures(GameObject go)
		{
			// disable tap events
			TapGesture tg = go.GetComponent<TapGesture>();
			if (tg != null) 
			{
				tg.enabled = false;
			}
			// disable pan events
			TransformGesture pg = go.GetComponent<TransformGesture>();
			if (pg != null) 
			{
				pg.enabled = false;
			}
			// disable press events
			PressGesture prg = go.GetComponent<PressGesture>();
			if (prg != null)
			{
				prg.enabled = false;
			}
			// disable release events
			ReleaseGesture rg = go.GetComponent<ReleaseGesture>();
			if (rg != null) 
			{
				rg.enabled = false;
			}
			Debug.Log("Disabled gestures for " + go.name);
		}

		//<summary>
		// Handle all tap events. Trigger actions in response.
		//</summary>
		private void tappedHandler(object sender, EventArgs e)
		{
			// get the gesture that was sent to us
			// this gesture will tell us what object was touched
			TapGesture gesture = sender as TapGesture;
			TouchHit hit;
			GameObject go = gesture.gameObject;
			// get info about where the hit object was located when the gesture was
			// recognized - i.e., where on the object (in screen dimensions) did
			// the tap occur?
			if (gesture.GetTargetHitResult(out hit)) 
			{ 
				// want the info as a 2D point 
				Debug.Log("TAP on " + gesture.gameObject.name + " at " + hit.Point);
			}
			// if kid is tapped - stop pulsing kid, make kid bounce up and down, make kid speak
			if (go.tag == Constants.Tags.TAG_KID) 
			{
				go.GetComponent<PulseBehavior>().StopPulsing(go);
				BounceKid(go);
				go.AddComponent<AudioSource>().clip = Resources.Load("Audio/KidSpeaking/Intro") as AudioClip;
				if (go.GetComponent<AudioSource>().clip != null) 
				{
					go.GetComponent<AudioSource>().Play();
				} 
				else 
				{
					Debug.LogWarning("Cannot load audio file");
				}
				// keep track of which kid was tapped on (boy or girl)
				ProgressManager.chosenKid = go.name;
				// go to next scene with the word tree
				StartCoroutine(LoadLevel("2. Word Tree", 2.5f));
			}
			// if a levelIcon is tapped on - make kid "shrink into" the levelIcon 
			if (go.tag == Constants.Tags.TAG_LEVEL_ICON)
			{
				ShrinkKid(new Vector3(go.transform.position.x, go.transform.position.y, -2));
				// keep track of what level Icon was tapped: stores the name of the current level
				ProgressManager.currentLevel = go.name.Substring(0, go.name.Length - 1);
				// go to next scene
				StartCoroutine(LoadLevel("3. Choose Object", 1f));
			}
			// For each level (category of words, i.e. Animals or Transportation), there are three different modes (games for the user to play):
			// 1. Learn Spelling
			// 2. Spelling Game
			// 3. Sound Game
			// The name of each level icon has either the number 1, 2, or 3 appended to it. The number corresponds to the mode.
			// If level icon is tapped - keep track of what mode it is
			if (go.name.Substring(go.name.Length - 1).Equals("1"))
			{
				ProgressManager.currentMode = 1;
			}
			if (go.name.Substring(go.name.Length - 1).Equals("2"))
			{
				ProgressManager.currentMode = 2;
			}
			if (go.name.Substring(go.name.Length - 1).Equals("3"))
			{
				ProgressManager.currentMode = 3;
			}
			// if a word object is tapped on in the Choose Object scene, load the appropriate scene
			if (go.tag == "WordObject" && Application.loadedLevelName == "3. Choose Object") 
			{
				// if the mode is 1, go to Learn Spelling scene
				if (ProgressManager.currentMode == 1)
				{
					Application.LoadLevel("4. Learn Spelling");
				}
				// if the mode is 2, go to Spelling Game scene
				if (ProgressManager.currentMode == 2) 
				{
					Application.LoadLevel("5. Spelling Game");
				}
				// if the mode is 3, go to Sound Game scene
				if (ProgressManager.currentMode == 3)
				{
					Application.LoadLevel("6. Sound Game");
				}
				// keep track of the name of the word object that was tapped on (the current word)
				ProgressManager.currentWord = gesture.gameObject.name;
			}
			// play word's sound when tapped
			if (go.tag == "WordObject" && Application.loadedLevelName != "3. Choose Object") 
			{
				go.GetComponent<AudioSource>().Play();
			}
			// if home button is tapped, go back to the intro scene
			if (go.name == "HomeButton") 
			{
				Application.LoadLevel("1. Intro");
			}
			// if tree button is tapped, shrink kid into tree icon and go back to Word Tree scene
			if (go.name == "TreeButton") 
			{
				ShrinkKid(go.transform.position);
				StartCoroutine(LoadLevel("2. Word Tree", 1f));
			}
			// if back button is tapped, go to Choose Object scene
			if (go.name == "BackButton") 
			{
				Application.LoadLevel("3. Choose Object");
			}
			// if sound button is tapped, play word's sound
			if (go.name == "SoundButton") 
			{
				GameObject word = GameObject.FindGameObjectWithTag(Constants.Tags.TAG_WORD_OBJECT);
				if (word != null) 
				{
					//create audio source
					AudioSource audioSource = gameObject.AddComponent<AudioSource>();
					//load audio file for word
					string file = "Audio" + "/Words/" + word.transform.name;
					audioSource.clip = Resources.Load(file) as AudioClip;
					if (audioSource.clip != null) 
					{
						//play sound clip 
						audioSource.Play();
					} 
					else 
					{
						Debug.LogWarning("Cannot find audio file");
					}
				} 
				else 
				{
					Debug.LogWarning("Cannot find word object");
				}
			}
			// if hint button is tapped, show a hint
			if (go.name == "HintButton") 
			{
				// if scene is Spelling Game, show a letter hint
				if (Application.loadedLevelName == "5. Spelling Game")
				{
					CollisionManager.ShowLetterHint();
				}
				// if scene is Sound Game, show a sound hint
				if (Application.loadedLevelName == "6. Sound Game") 
				{
					CollisionManager.ShowSoundHint();
				}
			}
			// if the closed lock icon is tapped, unlock all levels of the word tree
			if (go.name == "LockClosed") 
			{
				ProgressManager.UnlockAllLevels();
				ProgressManager.lockStatus = "LockOpen"; // change status of lock to open
				// move closed lock icon to behind the background and disable touch gestures
				LeanTween.moveZ(go, 3f, .01f);
				go.GetComponent<GestureManager>().DisableGestures(go);
				// move open lock icon in front of background and subscribe to touch gestures
				GameObject lockOpen = GameObject.Find("LockOpen");
				LeanTween.moveZ(lockOpen, -2f, .01f);
				lockOpen.AddComponent<GestureManager>().AddAndSubscribeToGestures(lockOpen);
			}
			// if the open lock icon is tapped, remove all levels not yet completed from word tree
			if (go.name == "LockOpen") 
			{
				ProgressManager.RelockLevels();
				ProgressManager.lockStatus = "LockClosed"; // change status of lock to closed
				// move open lock icon to behind background and disable touch gestures
				LeanTween.moveZ(go, 3f, .01f);
				go.GetComponent<GestureManager>().DisableGestures(go);
				// move closed lock icon in front of background and subscribe to touch gestures
				GameObject lockClosed = GameObject.Find("LockClosed");
				LeanTween.moveZ(lockClosed, -2f, .01f);
				lockClosed.AddComponent<GestureManager>().AddAndSubscribeToGestures(lockClosed);
			}
			// if any button is tapped, darken the button briefly to indicate to user that 
			// tap gesture has been registered
			if (go.tag == Constants.Tags.TAG_BUTTON) 
			{
				LeanTween.color(go, Color.grey, .01f);
				LeanTween.color(go, Color.white, .01f).setDelay(.2f);
			}
		}

		//<summary>
		// Handle press events
		//</summary>
		private void pressedHandler(object sender, EventArgs e)
		{
			// get the gesture that was sent to us, which will tell us which object was pressed
			PressGesture gesture = sender as PressGesture;
			// get info about where the hit object was located when the gesture was recognized
			TouchHit hit;
			if (gesture.GetTargetHitResult(out hit)) 
			{
				// want the info as a 2D point
				Debug.Log("PRESS on " + gesture.gameObject.name + " at " + hit.Point);
			}

			// play audio clip attached to object when pressed
			PlaySound(gesture.gameObject);
			// If the loaded scene is Spelling Game, re-enable collisions
			// when a letter is first pressed after resetting incorrect letters
			if (Application.loadedLevelName == "5. Spelling Game")
			{
				CollisionManager.EnableCollisions(gesture.gameObject, "TargetBlank");
			}
			// if the loaded scene is the sound game, re-enable collisions
			// when a sound blank is first pressed after resetting incorrect blanks
			if (Application.loadedLevelName == "6. Sound Game") 
			{
				CollisionManager.EnableCollisions(gesture.gameObject, "TargetLetter");
			}
		}

		//<summary>
		// Handles release events
		//</summary>
		private void releasedHandler(object sender, EventArgs e)
		{
			Debug.Log("PRESS COMPLETE");

		}

		//<summary>
		// Handle pan start events
		//</summary>
		private void panStartedHandler(object sender, EventArgs e)
		{
			// get the gesture that was sent to us, which will tell us which object was pressed
			TransformGesture gesture = sender as TransformGesture;
			// get info about where the hit object was located when the gesture was recognized
			TouchHit hit;
			if (gesture.GetTargetHitResult(out hit)) 
			{
				// want the info as a 2D point
				//ITouchHit2D hit2d = (ITouchHit2D)hit; 
				Debug.Log("PAN STARTED on " + gesture.gameObject.name + " at " + hit.Point);
			}
		}

		//<summary>
		// Handle pan / drag events
		//</summary>
		private void pannedHandler(object sender, EventArgs e)
		{
			// get the gesture that was sent to us, which will tell us which object was pressed
			TransformGesture gesture = sender as TransformGesture;
			// get info about where the hit object was located when the gesture was recognized
			TouchHit hit;
			if (gesture.GetTargetHitResult(out hit)) 
			{
				// want the info as a 2D point
				Debug.Log("PAN on " + gesture.gameObject.name + " at " + hit.Point);
				//saving reference to last dragged object
				//will check if last dragged object is within screen in Update function 
				this.recentObj = gesture.gameObject;
			}
		}

		//<summary>
		// Handle pan complete events
		//</summary>
		private void panCompleteHandler(object sender, EventArgs e)
		{
			Debug.Log("PAN COMPLETE");      
		}

		//<summary>
		// Wait for kid to shrink before going to next level
		//</summary>
		IEnumerator LoadLevel(string level, float delayTime)
		{
			yield return new WaitForSeconds(delayTime);
			Application.LoadLevel(level);
		}

		//<summary>
		// Play animation for kid spiraling into level icon with sound ("Whee!")
		//</summary>
		void ShrinkKid(Vector3 posn)
		{
			GameObject kid = GameObject.FindGameObjectWithTag(Constants.Tags.TAG_KID);
			// Rotate kid 360 degrees
			LeanTween.rotateAround(kid, Vector3.forward, 360f, 1f);
			// Shrink kid
			LeanTween.scale(kid, new Vector3(.1f, .1f, 1f), 1f);
			// Move kid to the position of the level icon
			LeanTween.move(kid, posn, 1f);
			// Fade out kid
			LeanTween.alpha(kid, 0f, .1f).setDelay(.9f);
			// if the current scene is Word Tree
			if (Application.loadedLevelName == "2. Word Tree") 
			{
				// Load audio onto kid
				kid.AddComponent<AudioSource>().clip = Resources.Load("Audio/TumbleSound") as AudioClip;
				// Play audio clip attached to kid if it exists
				if (kid.GetComponent<AudioSource>().clip != null)
				{
					kid.GetComponent<AudioSource>().Play();
				}
			}
		}

		//<summary>
		// Play animation for kid bouncing up and down
		//</summary>
		void BounceKid(GameObject kid)
		{
			float time = .5f; // time to complete one bounce
			// Move kid up and then down along y-axis twice
			LeanTween.moveY(kid, 0, time);
			LeanTween.moveY(kid, -1.5f, time).setDelay(time);
			LeanTween.moveY(kid, 0, time).setDelay(2 * time);
			LeanTween.moveY(kid, -1.5f, time).setDelay(3 * time);
		}

		//<summary>
		// Play sound attached to object
		//</summary>
		public void PlaySound(GameObject go)
		{ 
			//find word in scene
			GameObject word = GameObject.FindGameObjectWithTag(Constants.Tags.TAG_WORD_OBJECT);
			//get phonemes of the word 
			WordProperties prop = WordProperties.GetWordProperties(word.transform.name);
			// phonemes in word
			string[] phonemes = prop.Phonemes(); 
			//if phoeme contains the current letter
			//play the phoneme for that letter 
			foreach (string sound in phonemes)
			{
				if (sound.Contains(go.transform.name))
				{
					AudioSource audioSource = gameObject.AddComponent<AudioSource>();
					string file = "Audio" + "/Phonemes/" + sound;
					Debug.Log(file);
					audioSource.clip = Resources.Load(file) as AudioClip;
					if (audioSource.clip != null) 
					{
						//play sound clip
						audioSource.Play();  
					}
					else 
					{
						Debug.LogWarning("Cannot find audio file");
					}
				}
			}
		}

		/// <summary>
		/// On destroy, disable some stuff
		/// </summary>
		private void OnDestroy ()
		{
			// unsubscribe from gesture events
			foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>())
			{
				TapGesture tg = go.GetComponent<TapGesture>();
				//unsubscribte from tap events
				if (tg != null) 
				{
					tg.Tapped -= tappedHandler;
				}
				TransformGesture pg = go.GetComponent<TransformGesture>();
				if (pg != null) 
				{
					// unsubscribe from pan events
					pg.TransformStarted -= panStartedHandler;
					pg.Transformed -= pannedHandler;
					pg.TransformCompleted -= panCompleteHandler;
				}
				PressGesture prg = go.GetComponent<PressGesture>();
				// unsubscribe from press events
				if (prg != null) 
				{
					prg.Pressed -= pressedHandler;
				}
				ReleaseGesture rg = go.GetComponent<ReleaseGesture>();
				// unsubscribe to release events
				if (rg != null)
				{
					rg.Released -= releasedHandler;
				}
			}	
		}
			
		//<summary>
		// Update is called once per frame
		//</summary>
		void Update()
		{
			//changes transform.position of most recently hit gameObject
			//restricts the position of gameObject to rectangle
			//checks if recentObj is null
			if (this.recentObj != null)
			{
				//checks if object position is within boundaries
				if (this.recentObj.transform.position.x
				    <= this.cameraRect.xMin ||
				    this.recentObj.transform.position.x
				    >= this.cameraRect.xMax ||
				    this.recentObj.transform.position.y
				    <= this.cameraRect.yMin ||
				    this.recentObj.transform.position.y
				    >= this.cameraRect.yMax)
				{
					//restricts object's position to rectangle with screen boundaries
					this.recentObj.transform.position = new Vector3(Mathf.Clamp
						(this.recentObj.transform.position.x, this.cameraRect.xMin, this.cameraRect.xMax),
							Mathf.Clamp(this.recentObj.transform.position.y, this.cameraRect.yMin,
								this.cameraRect.yMax), this.recentObj.transform.position.z);
				}
			}
		}
			
	}
}
