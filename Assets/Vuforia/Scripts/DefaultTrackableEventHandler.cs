using HoloToolkit.Unity.InputModule;
using UnityEngine;

namespace Vuforia
{
    /// <summary>
    /// A custom handler that implements the ITrackableEventHandler interface.
    /// </summary>
    public class DefaultTrackableEventHandler : MonoBehaviour, ITrackableEventHandler
    {
        private ParticleSystem particles;
        private AudioSource audioSource;
        private AudioClip vumarkDetect;

        #region PRIVATE_MEMBER_VARIABLES
        private TrackableBehaviour mTrackableBehaviour;
        #endregion // PRIVATE_MEMBER_VARIABLES

        #region UNTIY_MONOBEHAVIOUR_METHODS
        void Start()
        {
            mTrackableBehaviour = GetComponent<TrackableBehaviour>();
            if (mTrackableBehaviour)
            {
                mTrackableBehaviour.RegisterTrackableEventHandler(this);
            }

            particles = GameObject.Find("inputburst_ps").GetComponent<ParticleSystem>();
            audioSource = GameObject.Find("VuMark").GetComponent<AudioSource>();  // Audio Ausgabe 
            vumarkDetect = Resources.Load("Sounds/vumark_detect") as AudioClip;
        }
        #endregion // UNTIY_MONOBEHAVIOUR_METHODS

        #region PUBLIC_METHODS
        /// <summary>
        /// Implementation of the ITrackableEventHandler function called when the
        /// tracking state changes.
        /// </summary>
        public void OnTrackableStateChanged(
                                        TrackableBehaviour.Status previousStatus,
                                        TrackableBehaviour.Status newStatus)
        {
            if (newStatus == TrackableBehaviour.Status.DETECTED ||
                newStatus == TrackableBehaviour.Status.TRACKED ||
                newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
            {
                OnTrackingFound();
            }
            else
            {
                OnTrackingLost();
            }
        }
        #endregion // PUBLIC_METHODS

        #region PRIVATE_METHODS
        private void OnTrackingFound()
        {
            Renderer[] rendererComponents = GetComponentsInChildren<Renderer>(true);
            Collider[] colliderComponents = GetComponentsInChildren<Collider>(true);

            // Enable rendering:
            foreach (Renderer component in rendererComponents)
            {
                component.enabled = true;
            }

            // Enable colliders:
            foreach (Collider component in colliderComponents)
            {
                component.enabled = true;
            }

            audioSource.PlayOneShot(vumarkDetect);  // Ton wiedergeben
            particles.transform.position = GazeManager.Instance.HitPosition;  // Click-Position ermitteln
            particles.Emit(60);  // Partikel-Animation anzeigen

            Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " found");
        }
        private void OnTrackingLost()
        {
            Renderer[] rendererComponents = GetComponentsInChildren<Renderer>(true);
            Collider[] colliderComponents = GetComponentsInChildren<Collider>(true);

            // Disable rendering:
            foreach (Renderer component in rendererComponents)
            {
                component.enabled = false;
            }

            // Disable colliders:
            foreach (Collider component in colliderComponents)
            {
                component.enabled = false;
            }

            Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " lost");
        }
        #endregion // PRIVATE_METHODS
    }
}
