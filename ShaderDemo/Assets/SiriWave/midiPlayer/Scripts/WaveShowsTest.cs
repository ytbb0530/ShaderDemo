using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class WaveShowsTest : MonoBehaviour
{
	public SiriWave waveTemplate;
	public List<BtnClicked> buttons = new List<BtnClicked> ();

    private waveshow show;
	private UnityMidiSynth midi;
	private Vector2 scrollPos;
    private List<string> files = new List<string>();
	private List<SiriWave> waves = new List<SiriWave>();
	private int soundIndex = 0;
	private float localPos = -1f;
	private float localStep = .1f;

	private Color[] waveColors = new Color[] {
		// warm color
		new Color(1, 0, 0),
		new Color(1, .5f, 0),
		new Color(.8f, .2f, .2f),
		new Color(1, 1, 0),

		new Color(0, 1, 0),
		new Color(.5f, .2f, 1),
		new Color(0, .8f, .8f),
		new Color(0, .5f, 1),
		// cold color
	};

    void Start()
    {
		show = GetComponent<waveshow> ();
		midi = GetComponent<UnityMidiSynth> ();

        string midis = Resources.Load<TextAsset>("midi.list").text;
        string[] _files = midis.Split(new string[] { "\r", "\n" },System.StringSplitOptions.RemoveEmptyEntries);
        foreach (var _f in _files)
        {
            files.Add(_f);
        }

		for(int i = 0; i < buttons.Count && i < files.Count; i++)
		{
			BtnClicked btn = buttons [i];
			btn.setText (files [i]);
		}

		waves.Clear ();
		waves.Add (waveTemplate);
		for(int i = 0; i < 10; i++)
		{
			GameObject waveGc = Instantiate (waveTemplate.gameObject);
			waveGc.transform.parent = transform;
			waveGc.transform.position = waveTemplate.transform.position;

			SiriWave wave = waveGc.GetComponent<SiriWave> ();
			wave.index = i + 1;
			wave.init ();
			waves.Add (wave);
		}
		waveTemplate.init ();

		soundIndex = 0;
		localPos = -1f;
    }

	public void setMidi(string file)
	{
		midi.StopAll();
		midi.Play(file);
	}

	public void stop()
	{
		midi.StopAll ();
	}

	void Update()
	{
        float[] d = show.GetDataSafe();
		showLine (d[soundIndex], soundIndex);
		soundIndex++;
		soundIndex = soundIndex >= d.Length ? 0 : soundIndex;

		if (localPos > 1f || localPos < -1f) {
			localStep = -localStep;
		}
		localPos += localStep;
    }

	private void showLine(float strongth, int cindex)
	{
		foreach(SiriWave wave in waves)
		{
			if (wave.inUse) {
				continue;
			}
			wave.show (strongth, waveColors[cindex], localPos / 100);
			break;
		}
	}

}