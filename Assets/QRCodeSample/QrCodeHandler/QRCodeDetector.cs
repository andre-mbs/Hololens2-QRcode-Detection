using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Microsoft.MixedReality.QR;

using Microsoft.MixedReality.Toolkit.UI;
using QRTracking;
#if WINDOWS_UWP
using Windows.Perception.Spatial;
#endif
namespace QRCodeTracking
{
    public class QRCodeDetector: QRCodeListener
    {     
        public string[] ExpectedQRCodeText;
        public GameObject[] qrCodePrefab;
        public UnityEvent onQRCode;
        private Microsoft.MixedReality.QR.QRCode the_qrcode;
        private Pose QRPose;
        private GameObject qrCodeObject = null;
        private GameObject[] qrCodeObjects = new GameObject[3];

#if WINDOWS_UWP
        private SpatialCoordinateSystem CoordinateSystem = null;
#endif

        // Use this for initialization
        public override void Start()
        {
            //Debug.Log("QRCodeDetector waiting for "+ ExpectedQRCodeText);
            base.Start();
        }

        public override void HandleQRCodeAdded(Microsoft.MixedReality.QR.QRCode qrCode)
        {
            // new QR code detected


            //if (qrCode.Data == ExpectedQRCodeText[0])
            //{                              
            //    the_qrcode = qrCode;
            //    onQRCode.Invoke();
            //}

            the_qrcode = qrCode;
            onQRCode.Invoke();
        }

        private bool GetPoseFromSpatialNode(System.Guid nodeId, out Pose pose)
        {
            
            bool found = false;
            pose = Pose.identity;

#if WINDOWS_UWP
                
                CoordinateSystem = Windows.Perception.Spatial.Preview.SpatialGraphInteropPreview.CreateCoordinateSystemForNode(nodeId);
                

                if (CoordinateSystem != null)
                {
                    info.text += "\ngot coordinate";
                    Quaternion rotation = Quaternion.identity;
                    Vector3 translation = new Vector3(0.0f, 0.0f, 0.0f);

                    SpatialCoordinateSystem rootSpatialCoordinateSystem = (SpatialCoordinateSystem)System.Runtime.InteropServices.Marshal.GetObjectForIUnknown(UnityEngine.XR.WSA.WorldManager.GetNativeISpatialCoordinateSystemPtr());

                    // Get the relative transform from the unity origin
                    System.Numerics.Matrix4x4? relativePose = CoordinateSystem.TryGetTransformTo(rootSpatialCoordinateSystem);

                    if (relativePose != null)
                    {
                        info.text += "\n got relative pose";
                        System.Numerics.Vector3 scale;
                        System.Numerics.Quaternion rotation1;
                        System.Numerics.Vector3 translation1;
       
                        System.Numerics.Matrix4x4 newMatrix = relativePose.Value;

                        // Platform coordinates are all right handed and unity uses left handed matrices. so we convert the matrix
                        // from rhs-rhs to lhs-lhs 
                        // Convert from right to left coordinate system
                        newMatrix.M13 = -newMatrix.M13;
                        newMatrix.M23 = -newMatrix.M23;
                        newMatrix.M43 = -newMatrix.M43;

                        newMatrix.M31 = -newMatrix.M31;
                        newMatrix.M32 = -newMatrix.M32;
                        newMatrix.M34 = -newMatrix.M34;

                        System.Numerics.Matrix4x4.Decompose(newMatrix, out scale, out rotation1, out translation1);
                        translation = new Vector3(translation1.X, translation1.Y, translation1.Z);
                        rotation = new Quaternion(rotation1.X, rotation1.Y, rotation1.Z, rotation1.W);
                        pose = new Pose(translation, rotation);
                        found = true;
                      

                        // can be used later using gameObject.transform.SetPositionAndRotation(pose.position, pose.rotation);
                        //Debug.Log("Id= " + id + " QRPose = " +  pose.position.ToString("F7") + " QRRot = "  +  pose.rotation.ToString("F7"));
                    } else {
                          info.text += "\nrelative pos NULL";
                    }
                } else {
                  info.text += "\ncannot retrieve coordinate";
                }
                
#endif
            return found;
            
        }

        void Update()
        {
            base.Update();
            if ((the_qrcode != null) && (qrCodePrefab[0] != null) && (qrCodePrefab[1] != null) && (qrCodePrefab[2] != null))
            {  
                
                var found  = GetPoseFromSpatialNode(the_qrcode.SpatialGraphNodeId, out QRPose);
                if (found)
                {
                    if(the_qrcode.Data == ExpectedQRCodeText[0] && qrCodeObjects[0] == null)
                    {
                        //Quaternion new_rot = QRPose.rotation;
                        qrCodeObjects[0] = Instantiate(qrCodePrefab[0], QRPose.position, QRPose.rotation);
                        qrCodeObjects[0].transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                        qrCodeObjects[0].transform.Rotate(new Vector3(90f, 90f, 90f));
                        info.text += " QRPose = " + QRPose.position.ToString("F7") + " QRRot = " + QRPose.rotation.ToString("F7");
                    }
                    else if (the_qrcode.Data == ExpectedQRCodeText[1] && qrCodeObjects[1] == null)
                    {
                        //Quaternion new_rot = QRPose.rotation;
                        qrCodeObjects[1] = Instantiate(qrCodePrefab[1], QRPose.position, QRPose.rotation);
                        qrCodeObjects[1].transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                        qrCodeObjects[1].transform.Rotate(new Vector3(90f, 90f, 90f));
                        info.text += " QRPose = " + QRPose.position.ToString("F7") + " QRRot = " + QRPose.rotation.ToString("F7");
                    }
                    else if (the_qrcode.Data == ExpectedQRCodeText[2] && qrCodeObjects[2] == null)
                    {
                        //Quaternion new_rot = QRPose.rotation;
                        qrCodeObjects[2] = Instantiate(qrCodePrefab[2], QRPose.position, QRPose.rotation);
                        qrCodeObjects[2].transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                        qrCodeObjects[2].transform.Rotate(new Vector3(90f, 90f, 90f));
                        info.text += " QRPose = " + QRPose.position.ToString("F7") + " QRRot = " + QRPose.rotation.ToString("F7");
                    }

                }
                
            }
        }

    }


}
