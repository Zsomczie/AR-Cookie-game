/**********
    Name: uiscript.cs
    Description: UI functionality in the AR cookie game

    Date created: March 2023
    Last edit:

    Author: Kalle
**********/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;//Needed for ARCameraManager
using TMPro; // TextMeshPro for text

public class KalleUIScript : MonoBehaviour
{
    // Reference to the ARCamera Manager
    [SerializeField] private ARCameraManager arcamera;

    // Reference to the ARTrackedImagemanger
    [SerializeField] private ARTrackedImageManager artim;

    // Reference to the ARFaceManager
    [SerializeField] private ARFaceManager arfm;
    private List<ARFace> faces = new List<ARFace>(); // List of ARFaces

    private bool cookiefound = false; // Have we a cookie?
    [SerializeField] private TMP_Text cookietext;   // Text which indicates whether we have a cookie?
    [SerializeField] private Button eatButton;  // Reference to the eat buggon

    // Subscribe to the trackedImagesChanged and facesChanged-events
    void OnEnable()
    {
        artim.trackedImagesChanged += OnChanged;
        arfm.facesChanged += OnFaceChanged;
    }
    void OnDisable()
    {
        artim.trackedImagesChanged -= OnChanged;
        arfm.facesChanged -= OnFaceChanged;
    }

    void OnFaceChanged(ARFacesChangedEventArgs eventArgs)
    {
        // Add tracked Face to our list of ARFaces
        foreach (var newFace in eventArgs.added)
        {
            faces.Add(newFace);
        }

        // Remove tracked Face from our list of ARFaces
        foreach (var lostFace in eventArgs.removed)
        {
            faces.Remove(lostFace);
        }
    }

    void OnChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        // In this event, when cookie has been found by camera, we have gained a cookie!
        foreach (var newImage in eventArgs.added)
        {
            cookiefound = true;
        }
    }

    private void Update()
    {
        // Change UI cookietext depending on whether we have a cookie or not
        if (cookiefound&&cookietext.text!= "Cookie eaten :)")
        {
            cookietext.text = "Cookie!";
            cookietext.color = Color.green;
        }
        else if (!cookiefound&&cookietext.text!= "Cookie eaten :)")
        {
            cookietext.text = "No cookie :<";
            cookietext.color = Color.red;
        }

        // Iterate faces on our faces list... 
        // As our app only detects 1 face at this time, we could simply use faces[0]
        foreach (var face in faces)
        {
            // The face.vertice is a position relative to the position of the face.transform
            // face.vertice[15] is near the mouth: https://user-images.githubusercontent.com/7452527/53465316-4a282000-3a02-11e9-8e85-0006e3100da0.png
            Vector3 pos = face.transform.TransformPoint(face.vertices[14]);

            // After using TransformPoint to find the vertice position in World Space,
            // we need to convert it relative to the screen position
            Vector3 screenpos = arcamera.GetComponent<Camera>().WorldToScreenPoint(pos);

            // Move eat button to position
            eatButton.transform.position = screenpos;
        }
    }

    // This button will switch between the front and the back cameras
    public void CameraSwitchButton(TMP_Text buttontext)
    {
         if (arcamera.currentFacingDirection == CameraFacingDirection.World&&cookiefound )
        {
            artim.enabled = false;  // Disable Image Tracking when using rear camera
            arfm.enabled = true;    // Enable Face Tracking when using rear camera
            arcamera.requestedFacingDirection = CameraFacingDirection.User;
            buttontext.text = "R"; // Change button text
            eatButton.gameObject.SetActive(true);
        }
        // If camera is facing the world, request to be facing user
        else if (arcamera.currentFacingDirection == CameraFacingDirection.World )
        {
            artim.enabled = false;  // Disable Image Tracking when using rear camera
            arfm.enabled = true;    // Enable Face Tracking when using rear camera
            arcamera.requestedFacingDirection = CameraFacingDirection.User;
            buttontext.text = "R"; // Change button text
        }

        // If camera not facing the world, make it face the world
        else if (arcamera.currentFacingDirection == CameraFacingDirection.User)
        {
            artim.enabled = true; // Enable Image Tracking when using rear camera
            arfm.enabled = false;// Disable Face Tracking when using rear camera
            eatButton.gameObject.SetActive(false);
            arcamera.requestedFacingDirection = CameraFacingDirection.World;
            buttontext.text = "F"; // Change button text

        }


    }
    public void eatCookie() 
    {
        cookietext.text = "Cookie eaten :)";
        eatButton.gameObject.SetActive(false);
    }
}
