//Structure goes like this: 

    //Login: 
    // 1) User writes email address in email section
    // 2) Entries from db are retrieved. Matching between email addresses in the db & the entered email starts. 
        //if found, 
            //We check the boolean value of Entry.
                //if false, 
                    //information of the user are printed as an entry, Entry is set to true, timestamp of entry is saved, a button that allows a sign out is displayed. * 
                        //if that sign out button is pressed: 
                            //we set the value of that specific user's boolean entry to false, a thank you for attending msg appears.
                //else if true
                    //Entry is set to false, a "Thank you for attending" msg appears, timestamp of exit is saved.
        //Else if not found 
            //Display msg: No user with email found 


    //User Sign up:
    //1) User is asked to input their information in specific input fields. (WITH A UNIQUE EMAIL THAT HASEN'T BEEN REGISTERED)
    //2) The values of the entered feilds (name, email, & image) are sent to the database, with atomatically setting a timestamp and automatically setting Entry to true.


using SimpleFirebaseUnity;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Proyecto26; 
using LitJson;
using System.IO; 

public class DBController : MonoBehaviour
{
    
    int usersCount = 0; //number of users in the db
    private Firebase firebase = Firebase.CreateNew("https://attendancesystem-motf.firebaseio.com/attendees");   // reference to firebase database
    FirebaseQueue firebaseQueue = new FirebaseQueue(true, 3, 1f);  
    User tempget = new User();  //temporary retrieved users

    ////////////////////////////////////////////////////////////////////////
    //Needed variables

    bool entry; //Logged in information
    string retmail; //returned email
    string retname; //returned name
    string retimage; //returned image
    string timeOfInOut; //timestamp
    int wantedUser; //user index in database
    bool emailFound; 
    bool signInFailed;
    public bool pictureTaken;

    ////////////////////////////////////////////////////////////////////////
    //Gameobject references
    public GameObject panel; 

    ////////////////////////////////////////////////////////////////////////
    //Text fields & input fields 

    public Text ErrorMsg; 
    public Text EmailMismatch;
    public InputField emailInput;
    public Text name;
    public Text loggedEmail; 
    public Text timeOfLogin; 
    public Text thankYouText; 
    public InputField addedName; 
    public InputField addedEmail;
    public Text notUniqueError; 
    public Text noEmailEntered; 
    public Text signInWait; 
    public Text loginWait; 
    public RawImage userImage;
    ////////////////////////////////////////////////////////////////////////
    //Canavases

    public Canvas emailInsertCanvas; 
    public Canvas InformationCanvas; 
    public Canvas thankYouCanvas; 
    public Canvas addUserCanvas;
    public Canvas welcomeCanvas; 

    ////////////////////////////////////////////////////////////////////////
    //picture info

    byte[] bytes;
    private bool camAvailable; 
    private WebCamTexture deviceCam;
    Texture defaultBackground; 
    public RawImage background; 
    public AspectRatioFitter fit; 
    float scaleY; 
    public Camera realLifeCam; 

    ////////////////////////////////////////////////////////////////////////

    void Start()
    {
        //Camera Capture code

        defaultBackground = background.texture;
        WebCamDevice[] devices = WebCamTexture.devices; 

        if(devices.Length == 0 )
        {
            camAvailable = false;
            return; 

        }
        
        for(int i=0; i < devices.Length ; i++)
        {
            if(devices[i].isFrontFacing)
            {
                deviceCam = new WebCamTexture(devices[i].name, Screen.width, Screen.height); 
            }
        }

        deviceCam.Play();
        background.texture = deviceCam; 

        camAvailable = true;


        //Get List Count
        GetCount();

    }

    void Update()
    {
        if(!camAvailable)
        {
            return; 
        }
        
        float ratio = (float)deviceCam.width / (float)deviceCam.height;
        fit.aspectRatio = ratio; 

        if(deviceCam.videoVerticallyMirrored)
        {
            scaleY = -1; 
        }
        else
        {
            scaleY = 1; 
        }

        background.rectTransform.localScale = new Vector3(1f, scaleY, 1f);
        int orient = -deviceCam.videoRotationAngle; 
        background.rectTransform.localEulerAngles = new Vector3(0,0, orient);

    }

    ////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////

    //Button clicks actions: 
    //Open Login Page
    public void LoginPage() 
    {
        welcomeCanvas.GetComponent<Canvas>().enabled = false;
        emailInsertCanvas.GetComponent<Canvas>().enabled = true; 

    }

    ////////////////////////////////////////////////////////////////////////
    //Submit login email
    public void Login() 
    {
        if(string.IsNullOrWhiteSpace(emailInput.text))
        {
            ErrorMsg.GetComponent<Text>().enabled = true;

        }
        else
        {
            ErrorMsg.GetComponent<Text>().enabled = false;
            StartCoroutine(Getdata(emailInput.text));

        }
    }

    ////////////////////////////////////////////////////////////////////////
    //Open sifnUp page
    public void addUser()  
    {
        welcomeCanvas.GetComponent<Canvas>().enabled = false;
        notUniqueError.GetComponent<Text>().enabled = false;
        noEmailEntered.GetComponent<Text>().enabled = false;
        addUserCanvas.GetComponent<Canvas>().enabled = true;

    }

    ////////////////////////////////////////////////////////////////////////
    //Capture Button
    public void Capture()
    {
        StartCoroutine(TakePicture());
    }
    
    ////////////////////////////////////////////////////////////////////////
    //Submit new user
    public void SubmitNewUser() 
    {
        StartCoroutine(userSignUp());

    }

    ////////////////////////////////////////////////////////////////////////
    //The back button on the information screen
    public void LoginBackButton() 
    {
        emailFound = false; 
        entry = false;
        InformationCanvas.GetComponent<Canvas>().enabled = false; 
        BackToWelcome();

    }

    ////////////////////////////////////////////////////////////////////////
    //The exit button on the information screen
    public void exit()  
    {
        StartCoroutine(OnExit());

    }

    ////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////
    //Picture capture

    IEnumerator TakePicture()
    {
        panel.GetComponent<Animator>().SetTrigger("Capture");
        yield return new WaitForSeconds(3.5f); 

        yield return new WaitForEndOfFrame();

        Texture2D photo = new Texture2D(deviceCam.width, deviceCam.height);
        photo.SetPixels(deviceCam.GetPixels());
        photo.Apply();
        
        bytes = photo.EncodeToPNG(); 
        retimage = Convert.ToBase64String(bytes); 

        Debug.Log("picture captured");
        pictureTaken = true;
    
    }

    ////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////
    //Sign up

    IEnumerator userSignUp()
    {   
        signInFailed = false;

        if(string.IsNullOrWhiteSpace(addedEmail.text))      //check if there is an email inserted
        {
            //enter email
            signInFailed = true;
            noEmailEntered.GetComponent<Text>().enabled = true;
            addedEmail.text = null; 

        }

        if(!signInFailed)
        {
            signInWait.GetComponent<Text>().enabled = true;
            for(int i = 0; i<usersCount; i++)
            {
                Getmail(i.ToString());
                yield return new WaitForSeconds(1f);

                if(retmail == addedEmail.text)
                {
                    //email already registered.
                    signInFailed = true;
                    notUniqueError.GetComponent<Text>().enabled = true;
                    addedEmail.text = null; 
                    signInWait.GetComponent<Text>().enabled = false;

                    break;
                } 
            }

        }

        if(!signInFailed)
        {
            retname = addedName.text; 
            retmail = addedEmail.text; 
            wantedUser = usersCount;

            NewUser(retname, retmail, retimage);
            yield return new WaitForSeconds(1f); 
            signInWait.GetComponent<Text>().enabled = false;
            addUserCanvas.GetComponent<Canvas>().enabled = false;
            InformationCanvas.GetComponent<Canvas>().enabled = true; 

            wantedUser = usersCount;
            Debug.Log ("Wanted user " + wantedUser);

            timeOfInOut = DateTime.Now.ToShortTimeString(); 

            name.text = "Welcome " + addedName.text +"!"; 
            timeOfLogin.text ="Entry time: " + timeOfInOut;
            loggedEmail.text = "Email: " + addedEmail.text;
            bytes = Convert.FromBase64String(retimage);

            Texture2D decodedTexture = new Texture2D(1920,1080); 

            decodedTexture.LoadImage(bytes); //decode back to a texture to show 
            decodedTexture.Apply();
            userImage.texture = decodedTexture; 
         
            
        }
    }

    private void NewUser(string name, string email, string picture)
    {
        Dictionary<string, object> US = new Dictionary<string, object>();
        US.Add("name", name);
        US.Add("email", email);
        US.Add("image", picture);
        US.Add("timeStamp", "0");    //posts the timestamp 
        US.Add("login", true);
        firebaseQueue.AddQueueSet(firebase.Child(usersCount.ToString()), US, FirebaseParam.Empty.PrintSilent());
        firebaseQueue.AddQueueSetTimeStamp(firebase.Child (usersCount.ToString(), true), "timeStamp");

        Debug.Log("userCount " + usersCount);
         
        
        Debug.Log("wanted user: " + wantedUser);
        
    }
    
    ////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////
    //Login

    IEnumerator Getdata(string email)       
    {
        Debug.Log("usersCount: " + usersCount);
        loginWait.GetComponent<Text>().enabled = true;
        EmailMismatch.GetComponent<Text>().enabled = false;
        ErrorMsg.GetComponent<Text>().enabled = false;
        GetCount(); 
        yield return new WaitForSeconds(1f); 

        for (int i = 0; i < usersCount ; i++)
        {

            Getmail(i.ToString());  //Gets the emails
            yield return new WaitForSeconds(2f);

            if (retmail == email && retmail != null )    //checks if the email "retmail" is equal to the email adress we are checking for && that emails exist in the db
            {
                emailFound = true; 
                Debug.Log("Successful retrieval");
                wantedUser = i;
                Debug.Log("wanted user index: " + wantedUser);
                break;
            }         
        }

        if(emailFound)
        {
            GetLogin(wantedUser.ToString());
            
            yield return new WaitForSeconds(2f);
            GetName(wantedUser.ToString()); 
            yield return new WaitForSeconds(2f);


            if(entry) //login is true
            {
                Debug.Log("Already signed in");
                emailInput.text = null;
                loginWait.GetComponent<Text>().enabled = false;

                exit(); 

            }
            else
            {
                
                GetImage(wantedUser.ToString());
                yield return new WaitForSeconds(2f);

                ChangeLogin(wantedUser);  
                UpdateTime(wantedUser);

                yield return new WaitForSeconds(1f);    //needed for name retrieval 
                loginWait.GetComponent<Text>().enabled = false;
                emailInput.text = null;
                emailInsertCanvas.GetComponent<Canvas>().enabled = false;

                InformationCanvas.GetComponent<Canvas>().enabled = true; 

                name.text = "Welcome " + retname +"!"; 
                timeOfLogin.text ="Entry time: " + timeOfInOut;
                loggedEmail.text = "Email: " + email;

                bytes = Convert.FromBase64String(retimage);

                Texture2D decodedTexture = new Texture2D(1920,1080); 

                decodedTexture.LoadImage(bytes); //decode back to a texture to show 
                decodedTexture.Apply();
                userImage.texture = decodedTexture; 

            }
        }

        else
        {
            loginWait.GetComponent<Text>().enabled = false;
            emailInput.text = null;

            EmailMismatch.GetComponent<Text>().enabled = true;
            emailInput.text = null;

            Debug.Log("email unavailable");
        }

        entry = false;
        emailFound = false;

        yield return new WaitForSeconds(2f);
        Debug.Log("END OF COROUTINE");

    }

    ////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////
    //Data retreival from database    

    private void GetCount()     //returns the number of entries in the database
    {
        RestClient.Get($"https://attendancesystem-motf.firebaseio.com/attendees.json").Then(response => {
            Debug.Log("Successful Count retrieval");

            JsonData jsonvale = JsonMapper.ToObject(response.Text);
            Debug.Log("Count: " + jsonvale.Count);
            usersCount = jsonvale.Count;

        });
    }
    ////////////////////////////////////////////////////////////////////////

    private void Getmail(string id)     
    {
        RestClient.Get($"https://attendancesystem-motf.firebaseio.com/attendees/"+id+".json").Then(response => {

            JsonData jsonvale = JsonMapper.ToObject(response.Text);
            retmail = (string) jsonvale["email"];
            Debug.Log("retmail: " + retmail);

        });
    }
    ////////////////////////////////////////////////////////////////////////

    private void GetLogin(string id)    //gets login status.
    {
        RestClient.Get($"https://attendancesystem-motf.firebaseio.com/attendees/"+id+".json").Then(response => {
            
            JsonData jsonvale = JsonMapper.ToObject(response.Text);
            Debug.Log("Logged in: " + jsonvale["login"]);
            entry = (bool) jsonvale["login"];

        });
    }
    ////////////////////////////////////////////////////////////////////////

    private void GetName(string id)
    {
            RestClient.Get($"https://attendancesystem-motf.firebaseio.com/attendees/"+id+".json").Then(response => {
            JsonData jsonvale = JsonMapper.ToObject(response.Text);
            Debug.Log(jsonvale["name"]);
            retname = (string) jsonvale["name"];
        
        });
    }
    ////////////////////////////////////////////////////////////////////////

    private void GetImage(string id)
    {
            RestClient.Get($"https://attendancesystem-motf.firebaseio.com/attendees/"+id+".json").Then(response => {
            JsonData jsonvale = JsonMapper.ToObject(response.Text);
            Debug.Log(jsonvale["image"]);
            retimage = (string) jsonvale["image"];
        
        });
    }

    ////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////
    //Data uploaded to database

    private void UpdateTime(int id)
    {
        timeOfInOut = DateTime.Now.ToShortTimeString(); 
        firebaseQueue.AddQueueSetTimeStamp(firebase.Child (id.ToString(), true), "timeStamp");

    }

    private void ChangeLogin(int id)
    {

        firebaseQueue.AddQueueUpdate(firebase.Child (id.ToString(), true), "{\"login\": true}");
        Debug.Log("changed to true successfuly");

    }
    ////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////
    //Exit 

    public void BackToWelcome()
    {
        addedName.text = null; 
        addedEmail.text = null; 
        emailInput.text = null;        
        welcomeCanvas.GetComponent<Canvas>().enabled = true;
        addUserCanvas.GetComponent<Canvas>().enabled = false; 
        InformationCanvas.GetComponent<Canvas>().enabled = false;
        emailInsertCanvas.GetComponent<Canvas>().enabled = false; 

    }

    IEnumerator OnExit()
    {        
        firebaseQueue.ForceClearQueue();
        InformationCanvas.GetComponent<Canvas>().enabled = false;
        emailInsertCanvas.GetComponent<Canvas>().enabled = false;

        thankYouCanvas.GetComponent<Canvas>().enabled = true;

        thankYouText.text = "Thank you for attending, " + retname +"!"; 

        firebaseQueue.AddQueueUpdate(firebase.Child (wantedUser.ToString(), true), "{\"login\": false}");

        yield return new WaitForSeconds (1f); 
        firebaseQueue.ForceClearQueue();
        emailFound = false;
        entry = false;

        yield return new WaitForSeconds(3f); 

        thankYouCanvas.GetComponent<Canvas>().enabled = false; 
        welcomeCanvas.GetComponent<Canvas>().enabled = true;

        Debug.Log("Exit done");

    }

    ////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////
    //User Class

    public class User // user class
    {
        public string userID;
        public string name;
        public string email;
        public string picture; 
        public string timeStamp;
        public bool login;

        public User()
        {

        }

        public User(string userId, string name, string email,string picture, string timeStamp, bool login) // create new user 
        {
            this.userID = userId;
            this.name = name;
            this.email = email;
            this.picture = picture;
            this.timeStamp = timeStamp;
            this.login = login;
        }
    }
}