using ChartAndGraph;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA.Input;

public class BarObjectManager2 : MonoBehaviour
{

    GameObject CurrentlyLookingAt = null;
    bool isGrowing = false;

    GameObject ClickedBar = null;

    GestureRecognizer tapRecognizer;

    //[SerializeField]
    //TextMesh text1;

    //[SerializeField]
    //TextMesh text2;

    [SerializeField]
    GameObject DetailCube;

    [SerializeField]
    GameObject NameCube;

    [SerializeField]
    GameObject FeatureCube;

    [SerializeField]
    Material SelectedMaterial;

    private Material defaultMaterial;

    [SerializeField]
    TextMesh Mode;

    [SerializeField]
    TextMesh Property;

    public static BarObjectManager2 instance;

    string currentMode;
    string currentProperty;

    private void Awake()
    {
        instance = this;
    }

    public void ChangeMode()
    {
        RemoveBarDetails();


        if (currentMode == "Linear Regression")
        {
            if(ReadCSV.instance.data_set_2 != null)
            {
                //First unhighlight the previous CurrentMode and currentProperty
                HighlightFeatureCube(defaultMaterial);

                currentMode = "Decision Tree";
                Mode.text = currentMode;
                currentProperty = ReadCSV.instance.titles_2[0];
                Property.text = currentProperty;
                ReadCSV.instance.AddIntoGraph(currentMode);

                //Then Highlight new feature
                HighlightFeatureCube(SelectedMaterial);
            }
            
        }else
        {
            if (ReadCSV.instance.data_set_1 != null)
            {
                //First unhighlight the previous CurrentMode and currentProperty
                HighlightFeatureCube(defaultMaterial);

                currentMode = "Linear Regression";
                Mode.text = currentMode;
                currentProperty = ReadCSV.instance.titles_1[0];
                Property.text = currentProperty;
                ReadCSV.instance.AddIntoGraph(currentMode);

                //Then Highlight new feature
                HighlightFeatureCube(SelectedMaterial);
            }   
        }

    }

    // Use this for initialization
    void Start()
    {
        ReadCSV.instance.DataEntry();           //Main function to feed data into dictionary

        currentMode = "Linear Regression";
        Mode.text = currentMode;
        currentProperty = ReadCSV.instance.titles_1[0];
        Property.text = currentProperty;

        if(ReadCSV.instance.data_set_1 != null)
        {
            ReadCSV.instance.AddIntoGraph(currentMode);
        }

        //Building the Features table
        BuildFeatures("Linear Regression", ReadCSV.instance.titles_1);
        BuildFeatures("Decision Tree", ReadCSV.instance.titles_2);

        //Highlight the feature
        HighlightFeatureCube(SelectedMaterial);

        tapRecognizer = new GestureRecognizer();
        tapRecognizer.TappedEvent += (source, tapCount, ray) =>
        {
            //text2.text = "Clicked";
            if (CurrentlyLookingAt != null)
            {
                if (CurrentlyLookingAt.tag == "BarTag")
                {
                    //A bar is clicked now!
                    if (CurrentlyLookingAt != ClickedBar)    //Clicked Bar for first time    //Show the details menu
                    {
                        ClickedBar = CurrentlyLookingAt;
                        BarOnClick(ClickedBar);
                    }
                    else                                    //Clicked the same bar //So hide the details menu
                    {
                        //Hide the details
                        ClickedBar = null;
                        GameObject BarDetails = GameObject.Find("BarDetails2");

                        RemoveBarDetails();

                    }
                    ////Just to Debug
                    if (ClickedBar != null)
                    {
                        var barData = ClickedBar.GetComponent<InternalItemEvents>().UserData as BarChart.BarObject;
                        //text2.text = barData.Category + " Clicked";
                    }
                    else
                    {
                        //text2.text = "Nothing clicked";
                    }
                }
                else if (CurrentlyLookingAt.tag == "FeatureTag")
                {
                    //Before changing the currentMode and currentProperty, need to set the materail back to unselected i.e. defaultMaterial
                    HighlightFeatureCube(defaultMaterial);

                    //Now, can change graph and highlight selected feature
                    GameObject feature = CurrentlyLookingAt;
                    string title = feature.name;
                    string model = feature.transform.parent.name;

                    Mode.text = currentMode = model;
                    Property.text = currentProperty = title;

                    ReadCSV.instance.PlotNewGraph(currentProperty, currentMode);

                    //Highlighting the current feature of current mode graph which is plotted. on Feature table
                    HighlightFeatureCube(SelectedMaterial);

                }   
                //else if (CurrentlyLookingAt.tag == "DetailTag")                                       // Disabling this, for now.       
                //{
                //    //A Detail cube is clicked now
                //    GameObject detail = CurrentlyLookingAt;
                //    string title = detail.name;

                //    currentProperty = title;
                //    Property.text = currentProperty;

                //    ReadCSV.instance.PlotNewGraph(currentProperty, currentMode);

                //}

            }

        };

        tapRecognizer.StartCapturingGestures();
    }

    void HighlightFeatureCube(Material changeTo)
    {
        string temp = currentMode + "/" + currentProperty;
        GameObject selected = GameObject.Find(temp);
        selected.GetComponent<Renderer>().material = changeTo;
    }

    void RemoveBarDetails()
    {
        GameObject BarDetails = GameObject.Find("BarDetails2");

        //Make sure BarDetails has no children
        for (int i = 0; i < BarDetails.transform.childCount; i++)
        {
            GameObject temp = BarDetails.transform.GetChild(i).gameObject;
            Destroy(temp);
        }
    }

    void BuildFeatures(string emptyParent, string[] titles)
    {
        GameObject Features = GameObject.Find(emptyParent);

        for (int i = 0; i < Features.transform.childCount; i++)
        {
            GameObject temp = Features.transform.GetChild(i).gameObject;
            Destroy(temp);
        }

        var sizeDiff = 1.00f;
        var gap = -1.2f;
        var distance = 0.0f;

        for (int i = 0; i < titles.Length; i++)
        {
            var title = titles[i];

            //Create each new Detail Cube
            GameObject x = Instantiate(FeatureCube, Features.transform);   //Instantiating a prefab "FeatureCube" and made Features as transform Parent; That is why, transform.scale = (1,1,1) and transform.position = (0,0,0) of FeatureCube
            var t = x.GetComponentInChildren<TextMesh>();
            t.text = title;
            x.name = title;

            Vector3 n = new Vector3(0, distance, 0f);       //Making sure each detail cube is below by 1.2f 
            x.transform.localPosition = n;
            distance = distance + gap;

            x.transform.localScale = x.transform.localScale * sizeDiff;
            sizeDiff -= 0.05f;

            defaultMaterial = FeatureCube.GetComponent<Renderer>().sharedMaterial;      //.material was giving error. I think, when accessing prefab's material, need to use sharedMaterial.
        }

    }

    void BarOnClick(GameObject bar)
    {
        var barData = bar.GetComponent<InternalItemEvents>().UserData as BarChart.BarObject;
        var category = barData.Category;

        float[] values;
        string[] titles;

        switch (currentMode)
        {
            case "Linear Regression":
                values = ReadCSV.instance.data_set_1[category];
                titles = ReadCSV.instance.titles_1;
                break;
            case "Decision Tree":
                values = ReadCSV.instance.data_set_2[category];
                titles = ReadCSV.instance.titles_2;
                break;
            default:
                values = ReadCSV.instance.data_set_1[category];
                titles = ReadCSV.instance.titles_1;
                break;
        }

        var gap = -1.2f;
        var distance = 0.0f;

        GameObject BarDetails = GameObject.Find("BarDetails2");  //The empty object inside which all detail prefabs go;

        //Make sure BarDetails has no children
        for (int i = 0; i < BarDetails.transform.childCount; i++)
        {
            GameObject temp = BarDetails.transform.GetChild(i).gameObject;
            Destroy(temp);
        }

        //Showing the Name of Category
        GameObject name = Instantiate(NameCube, BarDetails.transform);
        var tName = name.GetComponentInChildren<TextMesh>();
        tName.text = category;

        var sizeDiff = 1.00f;

        for (int i = 0; i < titles.Length; i++)
        {
            var title = titles[i];

            //Create each new Detail Cube
            GameObject x = Instantiate(DetailCube, BarDetails.transform);   //Instantiating a prefab "DetailsCube" and made BarDetails as transform Parent; That is why, transform.scale = (1,1,1) and transform.position = (0,0,0) of DetailsCube
            var t = x.GetComponentInChildren<TextMesh>();
            t.text = title + " : " + Convert.ToString(values[i]);
            x.name = title;

            Vector3 n = new Vector3(0, distance, 0f);       //Making sure each detail cube is below by 1.2f 
            x.transform.localPosition = n;
            distance = distance + gap;

            x.transform.localScale = x.transform.localScale * sizeDiff;
            sizeDiff -= 0.05f;
        }

        Debug.Log(values);
    }

    void Update()
    {

        var headPosition = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward;

        Debug.DrawRay(headPosition, gazeDirection * 100f, Color.green);

        RaycastHit hitInfo;
        if (Physics.Raycast(headPosition, gazeDirection, out hitInfo))
        {
            if (hitInfo.collider.gameObject.tag == "BarTag")
            {

                GameObject bar = hitInfo.collider.gameObject;

                if (CurrentlyLookingAt != bar)
                {
                    LookingAt(bar);
                }

                CurrentlyLookingAt = bar;

            }
            else if (hitInfo.collider.gameObject.tag == "DetailTag")
            {
                LookingAway(CurrentlyLookingAt);

                GameObject detailCube = hitInfo.collider.gameObject;
                CurrentlyLookingAt = detailCube;
            }
            else if (hitInfo.collider.gameObject.tag == "FeatureTag")
            {
                LookingAway(CurrentlyLookingAt);

                GameObject featureCube = hitInfo.collider.gameObject;
                CurrentlyLookingAt = featureCube;
            }
            else
            {
                LookingAway(CurrentlyLookingAt);
                CurrentlyLookingAt = null;
            }
        }
        else
        {
            LookingAway(CurrentlyLookingAt);
            CurrentlyLookingAt = null;
        }


        if (CurrentlyLookingAt != null)
        {
            //Debug.Log("Looking at " + CurrentlyLookingAt.name);
            //text1.text = "Looking at " + CurrentlyLookingAt.name;
        }
        else
        {
            //Debug.Log("Looking at null");
            //text1.text = "Looking at null";
        }

    }

    void LookingAt(GameObject bar)
    {
        if (!isGrowing)
        {
            var growEffect = bar.GetComponent<ChartItemGrowEffect>();
            growEffect.Grow();
            var materialEffect = bar.GetComponent<ChartMaterialController>();
            materialEffect.TriggerOn();

        }


    }

    void LookingAway(GameObject bar)
    {
        if (bar != null && bar.tag == "BarTag")
        {
            var growEffect = bar.GetComponent<ChartItemGrowEffect>();
            growEffect.Shrink();
            var materialEffect = bar.GetComponent<ChartMaterialController>();
            materialEffect.TriggerOff();

        }

    }
}
