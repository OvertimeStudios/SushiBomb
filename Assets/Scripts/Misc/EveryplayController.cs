using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EveryplayController : MonoBehaviour 
{
	private static bool isReady;
	private static bool videoFinished;

	public static bool IsReady
	{
		get { return isReady && Everyplay.IsSupported() && Everyplay.IsRecordingSupported(); }
	}

	#region singleton
	private static EveryplayController instance;
	public static EveryplayController Instance
	{
		get { return instance; }
	}
	#endregion
	
	void Awake()
	{
		if(instance != null && instance != this)
		{
			Destroy(gameObject);
			return;
		}
		else
		{
			instance = this;
		}
		
		DontDestroyOnLoad (gameObject);
	}

	void OnEnable()
	{
		GameController.OnGameStarted += StartRecording;
		Chest.OnClosed += StopRecording;
		GameController.OnGameOver += StopRecording;

		Everyplay.ReadyForRecording += OnReadyForRecording;
	}

	void OnDisable()
	{
		GameController.OnGameStarted -= StartRecording;
		Chest.OnClosed += StopRecording;
		GameController.OnGameOver -= StopRecording;

		Everyplay.ReadyForRecording -= OnReadyForRecording;
	}

	void Start()
	{
		if(instance != null) return;

		isReady = false;
		videoFinished = false;
	}

	private void OnReadyForRecording(bool enabled) 
	{
		Debug.Log("OnReadyForRecording? " + enabled);

		isReady = enabled;
	}

	public static void StartRecording()
	{
		Debug.Log("Start Recording? " + IsReady);
		if(!Everyplay.IsRecording() && IsReady)
		{
			Everyplay.StartRecording();
			videoFinished = false;
		}
	}

	public static void StopRecording()
	{
		Debug.Log("Stop Recording");
		if(Everyplay.IsRecording())
		{
			Everyplay.StopRecording();
			videoFinished = true;
		}
	}

	public static void PauseRecording()
	{
		if(!Everyplay.IsPaused())
			Everyplay.PauseRecording();
	}

	public static void ResumeRecording()
	{
		if(Everyplay.IsPaused())
			Everyplay.ResumeRecording();
	}

	public static void OpenShareOptions()
	{
		OpenShareOptions(null);
	}

	public static void OpenShareOptions(Dictionary<string, object> metadata)
	{
		if(!videoFinished) return;

		if(metadata != null)
			Everyplay.SetMetadata(metadata);

		Everyplay.ShowSharingModal();
	}

	public static void OpenEveryplay()
	{
		Everyplay.ShowWithPath("/feed/game");
	}

	public static void PlayLastRecording()
	{
		PlayLastRecording(null);
	}

	public static void PlayLastRecording(Dictionary<string, object> metadata)
	{
		Debug.Log("Play Last Recording: " + videoFinished);

		if(!videoFinished) return;

		if(metadata != null)
			Everyplay.SetMetadata(metadata);

		Everyplay.PlayLastRecording();
	}
}
