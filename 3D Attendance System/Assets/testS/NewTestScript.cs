using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

[TestFixture]
public class NewTestScript
{
    private DBController control;

    [SetUp]
    public void Setup()
    {
        GameObject gameGameObject = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/Controller"));
        control = gameGameObject.GetComponent<DBController>();
    }

    [TearDown]
    public void Teardown()
    {
        Object.Destroy(control.gameObject);
    }

    [UnityTest]
    public IEnumerator SwitchEntryToTrue()
    {
        control.entry = false;
        control.ChangeLogin(); 

        Assert.True(control.entry);


        yield return null; 
    }

    [UnityTest]
    public IEnumerator SwitchEntryToFalse()
    {
        control.entry = true;
        control.ChangeLogin(); 

        Assert.False(control.entry);


        yield return null; 
    }

    [UnityTest]
    public IEnumerator ImageSavedAsString()
    {
        control.TakePicture(); 
        Assert.NotNull(control.retimage); 
        yield return null; 
    }
}
