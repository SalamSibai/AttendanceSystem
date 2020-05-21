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

public class DBController : MonoBehaviour
{
    
    int usersCount = 0; //number of users in the db
    private Firebase firebase = Firebase.CreateNew("https://attendancesystem-motf.firebaseio.com/attendees");   // reference to firebase database
    FirebaseQueue firebaseQueue = new FirebaseQueue(true, 3, 1f);  
    User tempget = new User();  //temporary retrieved users

    ////////////////////////////////////////////////////////////////////////

    bool entry; //Logged in information
    string retmail; //returned email
    string retname; //returned name
    string timeOfInOut; 
    int wantedUser; 
    bool emailFound; 
    bool signInFailed;


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
    ////////////////////////////////////////////////////////////////////////
    //Canavases

    public Canvas emailInsertCanvas; 
    public Canvas InformationCanvas; 
    public Canvas thankYouCanvas; 
    public Canvas addUserCanvas;
    public Canvas welcomeCanvas; 

    
    void Start()
    {
        Debug.Log("start");
        GetCount();
    }

    public void LoginPage()
    {
        //hide welcome canvas
        welcomeCanvas.GetComponent<Canvas>().enabled = false;
        //show login canvas
        emailInsertCanvas.GetComponent<Canvas>().enabled = true; 

    }

    public void BackToWelcome()
    {
        Debug.Log("back to front page");
        welcomeCanvas.GetComponent<Canvas>().enabled = true;
        addUserCanvas.GetComponent<Canvas>().enabled = false; 
        InformationCanvas.GetComponent<Canvas>().enabled = false;
        emailInsertCanvas.GetComponent<Canvas>().enabled = false; 

    }
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
    public void addUser()
    {
        //emailInsertCanvas.GetComponent<Canvas>().enabled = false; 
        //welcome canvas
        welcomeCanvas.GetComponent<Canvas>().enabled = false;
        addUserCanvas.GetComponent<Canvas>().enabled = true;

    }

    public void SubmitNewUser()
    {
        StartCoroutine(userSignUp());

    }

    IEnumerator userSignUp()
    {   
        signInFailed = false;
        notUniqueError.GetComponent<Text>().enabled = false;
        noEmailEntered.GetComponent<Text>().enabled = false;

        if(string.IsNullOrWhiteSpace(addedEmail.text))      //check if there is an email inserted
        {
            //enter email
            signInFailed = true;
            noEmailEntered.GetComponent<Text>().enabled = true;
            addedEmail.text = null; 

        }

        if(!signInFailed)
        {
            for(int i = 0; i<usersCount; i++)
            {
                Getmail(i.ToString());
                signInWait.GetComponent<Text>().enabled = true;
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
            signInWait.GetComponent<Text>().enabled = false;

            NewUser(addedName.text, addedEmail.text);
            yield return new WaitForSeconds(1f); 
            addUserCanvas.GetComponent<Canvas>().enabled = false;
            InformationCanvas.GetComponent<Canvas>().enabled = true; 

            wantedUser = usersCount;
            timeOfInOut = DateTime.Now.ToShortTimeString(); 

            name.text = "Welcome " + addedName.text +"!"; 
            timeOfLogin.text ="Entry time: " + timeOfInOut;
            loggedEmail.text = "Email: " + addedEmail.text;
        }
    }

    public void LoginBackButton() //The back button on the information screen
    {
        emailFound = false; 
        entry = false;
        InformationCanvas.GetComponent<Canvas>().enabled = false; 
        BackToWelcome();

    }

    public void exit()
    {
        StartCoroutine(OnExit());

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

    private void NewUser(string name, string email)
    {
        Dictionary<string, object> US = new Dictionary<string, object>();
        US.Add("name", name);
        US.Add("email", email);
        US.Add("timeStamp", "0");    //posts the timestamp 
        US.Add("login", true);
        firebaseQueue.AddQueueSet(firebase.Child(usersCount.ToString()), US, FirebaseParam.Empty.PrintSilent());
        firebaseQueue.AddQueueSetTimeStamp(firebase.Child (usersCount.ToString(), true), "timeStamp");
        
    }
    


    IEnumerator Getdata(string email)       
    {
        loginWait.GetComponent<Text>().enabled = true;

        EmailMismatch.GetComponent<Text>().enabled = false;
        ErrorMsg.GetComponent<Text>().enabled = false;

        // yield return new WaitForSeconds(0.2f);

        // //emailInsertCanvas.GetComponent<Canvas>().enabled = false;    

        for (int i = 0; i < usersCount; i++)
        {

            Debug.Log("checking");
            Getmail(i.ToString());  //Gets the emails
            yield return new WaitForSeconds(1f);

            if (retmail == email && retmail != null )    //checks if the email "retmail" is equal to the email adress we are checking for && that emails exist in the db
            {
                emailFound = true; 
                Debug.Log("Successful retrieval");
                wantedUser = i;
                break;

            }         
        }


        if(emailFound)
        {
            GetLogin(wantedUser.ToString());
            GetName(wantedUser.ToString());
            yield return new WaitForSeconds(1f);

            if(entry) //login is true
            {
                Debug.Log("Already signed in");
                emailInput.text = null;
                loginWait.GetComponent<Text>().enabled = false;


                exit(); 

            }
            else
            {
                ChangeLogin(wantedUser);  
                UpdateTime(wantedUser);

                yield return new WaitForSeconds(1f);    //needed for name retrieval 
                loginWait.GetComponent<Text>().enabled = false;
                emailInput.text = null;
                emailInsertCanvas.GetComponent<Canvas>().enabled = false;


                InformationCanvas.GetComponent<Canvas>().enabled = true; 
                name.text = "Welcome " + retname +"!"; 
                timeOfLogin.text ="Entry time: " + timeOfInOut;
                loggedEmail.text = "Email: " + retmail;

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
        
    private void Getmail(string id)     
    {
        RestClient.Get($"https://attendancesystem-motf.firebaseio.com/attendees/"+id+".json").Then(response => {

            JsonData jsonvale = JsonMapper.ToObject(response.Text);
            retmail = (string) jsonvale["email"];

        });
    }

    private void GetLogin(string id)    //gets login status.
    {
        RestClient.Get($"https://attendancesystem-motf.firebaseio.com/attendees/"+id+".json").Then(response => {
            
            JsonData jsonvale = JsonMapper.ToObject(response.Text);
            Debug.Log("Logged in: " + jsonvale["login"]);
            entry = (bool) jsonvale["login"];

        });
    }

    private void GetName(string id)
    {
            RestClient.Get($"https://attendancesystem-motf.firebaseio.com/attendees/"+id+".json").Then(response => {
            JsonData jsonvale = JsonMapper.ToObject(response.Text);
            Debug.Log(jsonvale["name"]);
            retname = (string) jsonvale["name"];
        
        });
    }

    private void UpdateTime(int id)
    {
        timeOfInOut = DateTime.Now.ToShortTimeString(); 
        firebaseQueue.AddQueueSetTimeStamp(firebase.Child (id.ToString(), true), "timeStamp");

    }

    private void ChangeLogin( int id)
    {

        // if(entry)
        // {
        //     firebaseQueue.AddQueueUpdate(firebase.Child (id.ToString(), true), "{\"login\": false}");
        //     Debug.Log("changed to false successfuly");
        // }

        // else
        // {
            firebaseQueue.AddQueueUpdate(firebase.Child (id.ToString(), true), "{\"login\": true}");
            Debug.Log("changed to true successfuly");
        //}

        Debug.Log("Logged in: " + !entry);

    }

    private void GetCount()     //returns the number of entries in the database
    {
        RestClient.Get($"https://attendancesystem-motf.firebaseio.com/attendees.json").Then(response => {
            Debug.Log("Successful Count retrieval");

            JsonData jsonvale = JsonMapper.ToObject(response.Text);
            Debug.Log("Count: " + jsonvale.Count);
            usersCount = jsonvale.Count;

        });
    }

    public class User // user class
    {
        public string userID;
        public string name;
        public string email;
        public string timeStamp;
        public bool login;

        public User()
        {

        }

        public User(string userId, string name, string email, string timeStamp, bool login) // create new user 
        {
            this.userID = userId;
            this.name = name;
            this.email = email;
            this.timeStamp = timeStamp;
            this.login = login;

        }
    }
}