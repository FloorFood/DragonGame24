using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    /*How to use: find or make an approriate location in a script for a sound effect to be triggered.
    After doing so, type in the following to play a sound: AudioManager.instance.PlaySound("Name of sound");
    "Name of sound" should be switched out with the name of the sound/song you want to be played. Not the file name, but the name given to it in the sounds array inside the AudioManager
    object.

    If you want a sound effect to have a random pitch when it plays, you can use "AudioManager.instance.shouldRandomizePitch = true;" before calling the PlaySound function.
    If you want to play a sound from inside this script instead of in another script, you can leave out the "AudioManager.instance." part of the code. Same thing goes for changing shouldRandomizePitch.
    
    To fade out all music in 1 second put: 
        GameObject audioCancel = GameObject.FindWithTag("AudioCancel");
        audioCancel.GetComponent<AudioCancelScript>().CancelAudio();

    To quickly stop all music put:
    GameObject audioCancel = GameObject.FindWithTag("AudioCancel");
        audioCancel.GetComponent<AudioCancelScript>().CancelAudioFast();
    
     To trigger a fade:
     Add:
     using UnityEngine.Audio;
    Reference the AudioMixer "MainMixer" and the exposed parameter name "Music" by setting: 
     [SerializeField] AudioMixer audioMixer;
     [SerializeField] string Music;
    Call when fading (first f is how long the fade should take, second f is how loud on a scale of 0 to 1 the music should be after):
     StartCoroutine(FadeMixerGroup.StartFade(audioMixer, Music, f, f));
     */


    public Sound[] sounds;
    public static AudioManager instance;
    AudioSource[] allMyAudioSources;
    AudioSource audioSource;

    public bool shouldRandomizePitch;
    private bool firstTimeRunning;
    private bool hasEnteredSongLoop;
    private string whatSongIsGonnaPlayNow;
    private int audioCounter;

    public string currentScene;
    private string sceneName;
    private string lastScene;
    private string currentSong;

    /*[SerializeField] private AudioMixer mixer;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider voiceSlider;

    GameObject masterObject;
    GameObject sfxObject;
    GameObject musicObject;
    GameObject voiceObject;
    GameObject optionsholder;*/

    int sceneNumber;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = s.mix;
        }

        allMyAudioSources = GetComponents<AudioSource>();

        shouldRandomizePitch = false;
        firstTimeRunning = true;
        hasEnteredSongLoop = false;
        audioCounter = 0;

        Scene currentScene = SceneManager.GetActiveScene();
        sceneName = currentScene.name;

        if (sceneName == "TrustScene" || sceneName == "TutorialScene")
        {
            this.currentScene = "Game";
        }
        else
        {
            this.currentScene = "Error";
        }

        /*optionsholder = GameObject.FindWithTag("optionsholder");
        masterObject = optionsholder.transform.Find("MasterSlider").gameObject;
        musicObject = optionsholder.transform.Find("MusicSlider").gameObject;
        sfxObject = optionsholder.transform.Find("SFXSlider").gameObject;
        voiceObject = optionsholder.transform.Find("VoiceSlider").gameObject;

        masterSlider = masterObject.GetComponent<Slider>();
        musicSlider = musicObject.GetComponent<Slider>();
        sfxSlider = sfxObject.GetComponent<Slider>();
        voiceSlider = voiceObject.GetComponent<Slider>();

        Debug.Log("mainmenuScene");

        masterObject = GameObject.FindWithTag("Master");
        musicObject = GameObject.FindWithTag("Music");
        sfxObject = GameObject.FindWithTag("SFX");
        voiceObject = GameObject.FindWithTag("Voice");

        masterSlider = masterObject.GetComponent<Slider>();
        musicSlider = musicObject.GetComponent<Slider>();
        sfxSlider = sfxObject.GetComponent<Slider>();
        voiceSlider = voiceObject.GetComponent<Slider>();*/

        /*sfxSlider.onValueChanged.AddListener(HandleSFXSliderValueChanged);
        musicSlider.onValueChanged.AddListener(HandleMusicSliderValueChanged);
        masterSlider.onValueChanged.AddListener(HandleMasterSliderValueChanged);
        voiceSlider.onValueChanged.AddListener(HandleVoiceSliderValueChanged);*/
    }

   /* private void Start()
    {
        sfxSlider.value = PlayerPrefs.GetFloat("sfxParameter", sfxSlider.value);
        musicSlider.value = PlayerPrefs.GetFloat("musicParameter", musicSlider.value);
        masterSlider.value = PlayerPrefs.GetFloat("masterParameter", masterSlider.value);
        voiceSlider.value = PlayerPrefs.GetFloat("voiceParameter", voiceSlider.value);

    }*/

    /*private void OnDisable()
    {
        PlayerPrefs.SetFloat("sfxParameter", sfxSlider.value);
        PlayerPrefs.SetFloat("musicParameter", musicSlider.value);
        PlayerPrefs.SetFloat("masterParameter", masterSlider.value);
        PlayerPrefs.SetFloat("voiceParameter", voiceSlider.value);
    }*/

    public void Update()
    {
        //Scene currentScene = SceneManager.GetActiveScene();
        //sceneName = currentScene.name;

        //if (sceneName == "TrustScene" || sceneName == "TutorialScene")
        //{
        //    this.currentScene = "Game";
        //}
        //else if (sceneName == "NAME-OF-MENU-SCENE")
        //{
        //    this.currentScene = "Menu";
        //}
        //else if (sceneName == "NAME-OF-CUTSCENE-SCENE")
        //{
        //    this.currentScene = "Cutscene";
        //}
        //else
        //{
        //    this.currentScene = "Error";
        //}

        if (this.currentScene == "Game")
        {
            if (firstTimeRunning)
            {
                audioSource = allMyAudioSources[0];
                //Debug.Log("audioSource: " + audioSource.name);
                whatSongIsGonnaPlayNow = "MainThemeIntro";

                PlaySong(whatSongIsGonnaPlayNow);
                firstTimeRunning = false;
            }
            else if (!firstTimeRunning)
            {
                if (audioSource.isPlaying && audioCounter > 0)
                {
                    audioCounter = 0;
                }
                else if (!hasEnteredSongLoop && audioCounter > 1 && !audioSource.isPlaying)
                {
                    audioSource = allMyAudioSources[1];
                    //Debug.Log("audioSource: " + audioSource.name);
                    whatSongIsGonnaPlayNow = "MainThemeLoop";

                    PlaySong(whatSongIsGonnaPlayNow);
                    hasEnteredSongLoop = true;
                    audioCounter = 0;
                }
                else if (!audioSource.isPlaying && audioCounter < 2)
                {
                    audioCounter++;
                }
            }
        }
        else if (this.currentScene == "NoLongerGame")
        {
            StopSound(whatSongIsGonnaPlayNow);
            firstTimeRunning = true;
        }

        lastScene = this.currentScene;
    }

    public void PlaySound(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound '" + name + "' not found.");
            return;
        }

        if (shouldRandomizePitch)
        {
            s.source.pitch = Random.Range(0.90f, 1.1f);
            s.source.Play();
            shouldRandomizePitch = false;
        }
        else
        {
            s.source.pitch = 1f;
            s.source.Play();
        }

    }

    public void PlaySong(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Song '" + name + "' not found.");
            return;
        }
        s.source.pitch = 1f;
        s.source.Play();
    }

    

    public void StopSound(string sound)
    {
        Sound s = Array.Find(sounds, item => item.name == sound);
        if (s == null)
        {
            Debug.LogWarning("Sound '" + name + "' not found.");
            return;
        }
        s.source.volume = s.volume;
        s.source.pitch = s.pitch;

        s.source.Stop();
    }

    /*public void HandleSFXSliderValueChanged(float value)
    {
        if (value > 0)
        {
            mixer.SetFloat("SFX", Mathf.Log10(value) * 30);
        }
        else
        {
            mixer.SetFloat("SFX", -80);
        }
    }

    public void HandleMusicSliderValueChanged(float value)
    {
        if (value > 0)
        {
            mixer.SetFloat("Music", Mathf.Log10(value) * 30);
        }
        else
        {
            mixer.SetFloat("Music", -80);
        }
    }

    public void HandleMasterSliderValueChanged(float value)
    {
        if (value > 0)
        {
            mixer.SetFloat("Master", Mathf.Log10(value) * 30);
        }
        else
        {
            mixer.SetFloat("Master", -80);
        }
    }

    public void HandleVoiceSliderValueChanged(float value)
    {
        if (value > 0)
        {
            mixer.SetFloat("Voice", Mathf.Log10(value) * 30);
        }
        else
        {
            mixer.SetFloat("Voice", -80);
        }
    }*/
}