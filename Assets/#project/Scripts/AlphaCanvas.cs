using UnityEngine;
using System.Collections;
//[RequireComponent(typeof(Renderer))]


public class AlphaCanvas : MonoBehaviour {

    public static string GENERIC_PAINTING_LAYER_NAME = "painting";

    public string paintingLayer;
    public float distanceMax = 1f;
    public Camera viveCamera;
    public Transform targetCanvas;
    public Transform targetScene;
    private bool _activated = false;
   
	// Use this for initialization
	void Start () {
        if (targetCanvas == null)
            targetCanvas = transform.parent;
        targetScene.gameObject.layer = LayerMask.NameToLayer(paintingLayer);
    }
	
	// Update is called once per frame
	void Update () {

        float distance = GetDistanceNormal();

        if (distance > distanceMax){
            ChangeVisibility(false);
        }
        else{
            float ratio = distance / distanceMax;
            ChangeVisibility(true);
            if(IsFrontOfCanvas())
                ChangeAlphaTexture(ratio);
            else
                ChangeAlphaTexture(1f);

        }
        
       
	}


    private float GetDistance() {
        Vector3 point1 = viveCamera.transform.position;
        Vector3 point2 = transform.position;
        float distance = Vector3.Distance(point1, point2);
        return distance;
    }

    private float GetDistanceNormal() {
        Vector3 realDistance = viveCamera.transform.position - transform.position;
        Vector3 normDistance = Vector3.Project(realDistance, transform.forward);
        return Vector3.Magnitude(normDistance);
        /*Vector3 point1 = Vector3.Project(viveCamera.transform.position, transform.forward);
        Vector3 point2 = transform.position;
        float distance = Vector3.Distance(point1, point2);
        return distance;*/
    }

    private float GetDistanceOnWall()
    {
        Vector3 realDistance = viveCamera.transform.position - transform.position;
        Vector3 projectedDistance = Vector3.ProjectOnPlane(realDistance, transform.forward);
        return Vector3.Magnitude(projectedDistance);
    }

    private bool IsFrontOfCanvas(){

        Vector3 min = targetCanvas.GetComponent<MeshFilter>().mesh.bounds.min;
        Vector3 max = targetCanvas.GetComponent<MeshFilter>().mesh.bounds.max;
        //print(Vector3.Distance(min, max));
        print(GetDistanceOnWall());
        if (GetDistanceOnWall() <= Vector3.Distance(min, max))
            return true;
        return false;
    }

    private float ComputeRatio(){
        Vector3 point1 = viveCamera.transform.position;
        Vector3 point2 = transform.position;
        float distance = Vector3.Distance(point1, point2);
        distance = Mathf.Min(distance, distanceMax);
        return distance / distanceMax;  
    }

    private void ChangeAlphaTexture(float ratio){
        Material material = targetCanvas.GetComponent<Renderer>().material;
        Color color = material.color;
        color.a = ratio;
        material.color = color;

    }

    private void ChangeVisibility(bool actived) {
        if (_activated == actived)
            return;
        _activated = actived;
        if (actived)
        {
            targetScene.gameObject.layer = LayerMask.NameToLayer(GENERIC_PAINTING_LAYER_NAME);
        }
        else
        {
            targetScene.gameObject.layer = LayerMask.NameToLayer(paintingLayer);
        }

}
}
