using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SwipeTrial : MonoBehaviour
{

    public Text score;
    public Text floatPrefab;
    public Canvas cv;

    float time = 220;

    public AudioClip swipeAudio;
    public AudioClip swipeboom;
    public AudioClip swipegestureaudio;

    AudioSource audio_score;
    public float volume;
    public bool played = false;

    public GameObject applePrefab;
    public GameObject halfApplePrefab;
    public GameObject quartApplePrefab;
    public GameObject obstaclePrefab;
    bool swipeCooldown = true;
    GameObject gObj = null;

    Vector3 lastPosition;
    Vector3 deltaPosition;

    void Start()
    {
        audio_score = GetComponent<AudioSource>();
    }



    public void playswipeaudio()
    {
        if (!played)
        {
            audio_score.PlayOneShot(swipeAudio, volume);
            played = false;
        }
    }

    public void playswipeboom()
    {
        if (!played)
        {
            audio_score.PlayOneShot(swipeboom, volume);
            played = false;
        }
    }

    public void playswipegestureaudio()
    {
        if (!played)
        {
            audio_score.PlayOneShot(swipegestureaudio, volume);
            played = false;
        }
    }



    Ray GenerateMouseRay(Vector3 touchPos)
    {
        Vector3 mousePosFar = new Vector3(touchPos.x, touchPos.y, Camera.main.farClipPlane);
        Vector3 mousePosNear = new Vector3(touchPos.x, touchPos.y, Camera.main.nearClipPlane);
        Vector3 mousePosF = Camera.main.ScreenToWorldPoint(mousePosFar);
        Vector3 mousePosN = Camera.main.ScreenToWorldPoint(mousePosNear);

        Ray mr = new Ray(mousePosN, mousePosF - mousePosN);
        return mr;
    }

    void CoolDown()
    {
        swipeCooldown = true;
    }


    // Update is called once per frame
    void Update()
    {
        time -= Time.deltaTime;
        if (((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            || Input.GetMouseButtonDown(0)))
        {
            Plane objPlane = new Plane(Camera.main.transform.forward * -1, this.transform.position);
            Ray mRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            float rayDistance;
            if (objPlane.Raycast(mRay, out rayDistance))
                lastPosition = mRay.GetPoint(rayDistance);
            playswipegestureaudio();
        }
        else if (((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
            || Input.GetMouseButton(0)))
        {
            Plane objPlane = new Plane(Camera.main.transform.forward * -1, this.transform.position);
            Ray mRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            float rayDistance;
            if (objPlane.Raycast(mRay, out rayDistance))
                this.transform.position = mRay.GetPoint(rayDistance);

            Ray mouseRay = GenerateMouseRay(Input.mousePosition);
            RaycastHit hit;
            deltaPosition = this.transform.position - lastPosition;
            lastPosition = this.transform.position;

            if (Physics.Raycast(mouseRay.origin, mouseRay.direction, out hit) && swipeCooldown)
            {
                gObj = hit.transform.gameObject;
                swipeCooldown = false;
                Invoke("CoolDown", 0.5f);
                playswipeaudio();
                if (gObj.tag == "whole")
                {
                    GameObject h1 = (GameObject)Instantiate(halfApplePrefab, gObj.transform.position, gObj.transform.rotation);
                    h1.transform.rotation *= Quaternion.Euler(90, 0, 90);
                    h1.transform.Translate(0, 0, -5);
                    h1.GetComponent<Rigidbody>().velocity = gObj.GetComponent<Rigidbody>().velocity;
                    h1.GetComponent<Rigidbody>().AddTorque(deltaPosition * 1000);
                    h1.GetComponent<Rigidbody>().AddTorque(deltaPosition * 500);
                    GameObject h2 = (GameObject)Instantiate(halfApplePrefab, gObj.transform.position, gObj.transform.rotation);
                    h2.transform.rotation *= Quaternion.Euler(-90, -90, 0);
                    h2.GetComponent<Rigidbody>().velocity = gObj.GetComponent<Rigidbody>().velocity;
                    h2.GetComponent<Rigidbody>().AddTorque(deltaPosition * 1000);
                    h2.GetComponent<Rigidbody>().AddTorque(deltaPosition * 500);

                    Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, gObj.transform.position);
                    Text UI_Text = (Text)Instantiate(floatPrefab, gObj.transform.position, Quaternion.identity);
                    UI_Text.text = "50";
                    score.text = int.Parse(score.text) + int.Parse(UI_Text.text) + "";
                    UI_Text.gameObject.transform.SetParent(cv.transform);
                    UI_Text.gameObject.GetComponent<RectTransform>().anchoredPosition = screenPoint - cv.GetComponent<RectTransform>().sizeDelta / 2f;

                    Destroy(gObj);
                    playswipeaudio();
                }
                else if (gObj.tag == "half")
                {
                    GameObject h1 = (GameObject)Instantiate(quartApplePrefab, gObj.transform.position, gObj.transform.rotation);
                    h1.transform.rotation *= Quaternion.Euler(-90, 0, -90);
                    h1.GetComponent<Rigidbody>().velocity = gObj.GetComponent<Rigidbody>().velocity;
                    h1.GetComponent<Rigidbody>().AddTorque(deltaPosition * 1000);
                    h1.GetComponent<Rigidbody>().AddTorque(deltaPosition * 500);
                    //h1.transform.Translate (0, 0, -5);
                    GameObject h2 = (GameObject)Instantiate(quartApplePrefab, gObj.transform.position, gObj.transform.rotation);
                    h2.transform.rotation *= Quaternion.Euler(0, -180, 90);
                    h2.GetComponent<Rigidbody>().velocity = gObj.GetComponent<Rigidbody>().velocity;
                    h2.GetComponent<Rigidbody>().AddTorque(deltaPosition * 1000);
                    h2.GetComponent<Rigidbody>().AddTorque(deltaPosition * 500);

                    Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, gObj.transform.position);
                    Text UI_Text = (Text)Instantiate(floatPrefab, gObj.transform.position, Quaternion.identity);
                    //UI_Text.text = "50";
                    score.text = int.Parse(score.text) + int.Parse(UI_Text.text) + ""; //to calculate the total 
                    UI_Text.gameObject.transform.SetParent(cv.transform);
                    UI_Text.gameObject.GetComponent<RectTransform>().anchoredPosition = screenPoint - cv.GetComponent<RectTransform>().sizeDelta / 2f;

                    Destroy(gObj);
                    playswipeaudio();
                }
                else if (gObj.tag == "obstacle")
                {

                    Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, gObj.transform.position);
                    Text UI_Text = (Text)Instantiate(floatPrefab, gObj.transform.position, Quaternion.identity);
                    UI_Text.text = "-50";
                    score.text = int.Parse(score.text) + int.Parse(UI_Text.text) + ""; //to calculate the total 
                    UI_Text.gameObject.transform.SetParent(cv.transform);
                    UI_Text.gameObject.GetComponent<RectTransform>().anchoredPosition = screenPoint - cv.GetComponent<RectTransform>().sizeDelta / 2f;

                    Destroy(gObj);
                    playswipeboom();
                }

            }

        }
    }
}
