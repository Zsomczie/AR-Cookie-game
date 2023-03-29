using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    [SerializeField] private ARCameraManager arcamera;
    [SerializeField] private ARTrackedImageManager artim;
    [SerializeField] private TMP_Text cookietext;
    [SerializeField] private ARFaceManager faceManager;
    [SerializeField] private Button eatButton;
    private List<ARFace> faces = new List<ARFace>();
    private bool cookiefound = false;
    [SerializeField]private TMP_Text EatButtonText;
    public void CameraSwitchButton(TMP_Text Buttontext) 
    {
        if (arcamera.currentFacingDirection==CameraFacingDirection.World)
        {
            eatButton.enabled = true;
            artim.enabled = false;
            arcamera.requestedFacingDirection = CameraFacingDirection.User;
            Buttontext.text = "R";
            
        }
        else if (arcamera.currentFacingDirection == CameraFacingDirection.User)
        {
            artim.enabled = true;
            arcamera.requestedFacingDirection = CameraFacingDirection.World;
            Buttontext.text = "F";
            eatButton.enabled = false;
        }
    }
     public void onFaceChanged(ARFacesChangedEventArgs eventArgs)
    {
        foreach (var newFace in eventArgs.added)
        {
            faces.Add(newFace);
        }
        foreach (var lostFace in eventArgs.removed)
        {
            faces.Remove(lostFace);
        }
    }
    private void Start()
    {
    }
    void OnEnable()
    {
        artim.trackedImagesChanged += onChanged;
        faceManager.facesChanged -= onFaceChanged;
    }

    void OnDisable() 
    {
        artim.trackedImagesChanged -= onChanged;
        faceManager.facesChanged += onFaceChanged;
    }
    

    private void Update()
    {
        if (cookiefound)
        {
            cookietext.text = "cookie";
            cookietext.color = Color.green;
        }
        else
        {
            cookietext.text = "no cookie";
            cookietext.color = Color.red;
        }
        foreach (var face in faces)
        {
            Vector3 pos = face.transform.TransformPoint(face.vertices[14]);
            cookietext.text = pos.ToString();
            Vector3 screenpos = arcamera.GetComponent<Camera>().WorldToScreenPoint(pos);
            EatButtonText.text = screenpos.ToString();
            eatButton.transform.position = screenpos;
        }
    }
     public void onChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var newImage in eventArgs.added)
        {
            cookiefound=true;
        }
    }
   
}
