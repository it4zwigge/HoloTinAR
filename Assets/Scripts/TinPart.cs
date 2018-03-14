using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class TinPart : MonoBehaviour, IFocusable, IInputClickHandler
{
    #region Klassenvariablen
    [HideInInspector]  // im Unity verstecken
    public bool IsResetable;  // Teil ist platziert kann also resettet werden

    [SerializeField]        // private Variable in Unity anzeigen
    [Tooltip("Legen Sie hier die Reihenfolge des Einlegens fest! (0 = beliebig)")]  // Tooltip im Unity anzeigen
    private int PartOrder;  // TODO: Typ anpassen

    private AudioSource audioSource;      // zum Abspielen des Fehlertons notwendig
    private AudioClip wrongSound;
    private bool soundIsRunning = false;  // Hilfsvariable um mehrmaliges gleichzeitiges Abspielen des Fehlertons verhindert wird
    private float redTime = 0f;           // Variable zum Steuern der Abspieldauer des Fehlertons

    private ParticleSystem particles;     // Animation am Cursor

    private bool gazing;                  // im Focus oder nicht
    private Renderer partInfo;            // Infotext mit Teilname
    #endregion Klassenvariablen

    // Use this for initialization
    void Start()
    {
        audioSource = GameObject.Find("VuMark").GetComponent<AudioSource>();          // allgemeine Audioquelle
        wrongSound = Resources.Load("Sounds/wrong") as AudioClip;                     // Fehlerton laden
        particles = GameObject.Find("inputburst_ps").GetComponent<ParticleSystem>();  // Partikeleffekt laden

        //GameObject partInfoObj = Instantiate(Resources.Load("Prefabs/PartInfo"), this.transform) as GameObject;
        ////partInfoObj.name = name + "Info";

        //BoxCollider boxCollider = GetComponent<BoxCollider>();
        //partInfoObj.transform.position = new Vector3(boxCollider.transform.position.x,
        //                                             boxCollider.transform.position.y,
        //                                             boxCollider.transform.position.z);

        //partInfoObj.GetComponent<TextMesh>().text = name;
        //partInfo = partInfoObj.GetComponent<Renderer>();  // Infotext mit Teilname

        partInfo = GameObject.Find(name + "Info").GetComponent<Renderer>();
        SetTransparancy(ref partInfo, 0.0f);  // Infotext ausblenden
    }

    // Update is called once per frame
    void Update()
    {
        #region Falsch angeklickte Teile behandeln
        // WENN das Teil rot ist
        if (GetComponentInChildren<Renderer>().material.color == Color.red)
            redTime += Time.deltaTime;  // Zeit starten (deltaTime = die Zeit in sec die es gedauert hat den letzten Frame zu rendern)

        if (redTime >= 1)  // nach etwa 1 sec
        {
            // rotes Teil wieder normal einfärben
            SetMaterialColor(Color.white);
            redTime = 0f;            // Zeit zurücksetzen
            soundIsRunning = false;  // neuen Warnton ermöglichen
        }
        #endregion Falsch angeklickte Teile behandeln

        #region Infotext Ein- oder Ausblenden
        float desiredAlpha = 0.0f;  // Zielwert (0 für Ausblenden, 1 für Einblenden)
        // ist das Teil focusiert UND noch NICHT platziert DANN Text einblenden
        if (gazing && !IsResetable)
            desiredAlpha = 1.0f;

        // Eventuell Alphawert verändern bis der Zielwert erreicht ist
        if (partInfo.material.color.a != desiredAlpha)
            FadeInOrOut(ref partInfo, desiredAlpha);
        #endregion Infotext Ein- oder Ausblenden
    }

    #region Schnittstellenmember
    // IFocusable
    public void OnFocusEnter()
    {
        gazing = true;
    }
    public void OnFocusExit()
    {
        gazing = false;
    }
    // IInputClickHandler
    public void OnInputClicked(InputClickedEventData eventData)
    {
        if (PartOrder == AllParts.PartToPlace ||  // ist das Teile an Reihe ODER
            PartOrder == 0)                       // ist die Reihenfolge egal (z.B. Schottteil)
        {
            particles.transform.position = GazeManager.Instance.HitPosition;  // Click-Position ermitteln
            particles.Emit(60);  // Partikel-Animation anzeigen
            MoveInPosition();
        }
        else if (!IsResetable)  // falsches Teil angeklickt
        {
            SetMaterialColor(Color.red);
            PlayWrongSound();
        }
    }
    #endregion Schnittstellenmember

    #region Helper
    private void MoveInPosition()
    {
        GetComponent<Animator>().Play(name + "Animation");
        IsResetable = true;
        if (PartOrder != 0)
            AllParts.PartToPlace++;  // Platzieren des nächsten Teils ermöglichen
    }
    public void MoveBack()
    {
        GetComponent<Animator>().Play(name + "Back");
        IsResetable = false;
    }
    private void PlayWrongSound()
    {
        if (!soundIsRunning)  // Ton nicht mehrfach aufrufen
        {
            soundIsRunning = true;
            audioSource.PlayOneShot(wrongSound);  // Ton wiedergeben  
        }
    }
    private void SetTransparancy(ref Renderer renderer, float alpha)
    {
        renderer.material.color = new Color(renderer.material.color.r,
                                            renderer.material.color.g,
                                            renderer.material.color.b,
                                            alpha);  // Alphawert für Transparenz (RGBA)
    }
    private void FadeInOrOut(ref Renderer renderer, float desiredAlpha)
    {
        float currentAlpha = renderer.material.color.a;

        if (currentAlpha < desiredAlpha)
            currentAlpha += 0.02f;
        else
            currentAlpha -= 0.02f;

        SetTransparancy(ref renderer, currentAlpha);
    }
    private void SetMaterialColor(Color color)
    {
        Renderer[] renderer = GetComponentsInChildren<Renderer>();
        foreach (Renderer render in renderer)
        {
            if (render.gameObject.name.Contains("Info"))
                continue;
            render.material.color = color;
        }
    }
    #endregion Helper
}
