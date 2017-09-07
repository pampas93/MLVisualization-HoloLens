using ChartAndGraph;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ReadCSV : MonoBehaviour
{

    public static ReadCSV instance;

    [HideInInspector]
    public Dictionary<string, float[]> data_set_1;

    [HideInInspector]
    public Dictionary<string, float[]> data_set_2;

    [HideInInspector]
    public string[] titles_1;

    [HideInInspector]
    public string[] titles_2;

    [SerializeField]
    public Material barMaterial;

    [SerializeField]
    TextAsset linearRegression;

    [SerializeField]
    TextAsset decisionTree;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {

        //data_set_1 = new Dictionary<string, float[]>();
        //data_set_2 = new Dictionary<string, float[]>();

        //ReadCsvData(linearRegression, data_set_1, titles_1);
        //ReadCsvData(decisionTree, data_set_2, titles_2);

        //if (data_set_1 != null)
        //{
        //    currentDataUsed = "data_set_1";
        //    AddIntoGraph();
        //}
    }

    public void DataEntry()
    {
        data_set_1 = new Dictionary<string, float[]>();
        data_set_2 = new Dictionary<string, float[]>();

        titles_1 = ReadCsvData(linearRegression, data_set_1, titles_1);
        titles_2 = ReadCsvData(decisionTree, data_set_2, titles_2);
    }

    string[] ReadCsvData(TextAsset readFrom, Dictionary<string, float[]> feadInto, string[] titles)
    {
        var count = 0;
        var readTitles = false;

        TextAsset csvData = readFrom;

        //TextAsset csvData = Resources.Load("SampleData") as TextAsset;
        var lines = csvData.text.Split("\n"[0]);
        foreach (string line in lines)
        {
            var values = line.Split(',');
            count = values.Length;
            break;
        }

        titles = new string[count - 1];

        var line2 = csvData.text.Split("\n"[0]);
        foreach (string line in line2)
        {
            var values = line.Split(',');
            if (!readTitles)                    //if readTitles = false, I capture the titles first (since first line of csv always has titles)
            {
                for (int i = 1; i < values.Length; i++)
                {
                    titles[i - 1] = values[i];
                }
                readTitles = true;
            }
            else             //Capture store into dictionary
            {
                float[] tempValues = new float[count - 1];
                for (int i = 1; i < values.Length; i++)
                {
                    var temp = 0.0f;
                    try
                    {
                        temp = (float)Convert.ToDouble(values[i]);
                    }
                    catch (Exception)
                    {
                        temp = 0.0f;
                    }
                    tempValues[i - 1] = temp;
                }
                feadInto.Add(values[0], tempValues);
            }
        }

        return titles;
    }

    public void AddIntoGraph(string mode)       //Plotting new Graph with different mode
    {
        Dictionary<string, float[]> dictionaryUsed;
        string[] titles;

        switch (mode)
        {
            case "Linear Regression": dictionaryUsed = data_set_1;
                titles = titles_1;
                break;
            case "Decision Tree": dictionaryUsed = data_set_2;
                titles = titles_2;
                break;
            default:
                dictionaryUsed = data_set_1;
                titles = titles_1;
                break;
        }

        BarChart barChart = GetComponent<BarChart>();

        if (barChart != null)
        {
            barChart.DataSource.ClearCategories();
            barChart.DataSource.ClearGroups();

            barChart.DataSource.AutomaticMaxValue = true;           //Set Max value to Auto

            var title = titles[0]; 
            barChart.DataSource.AddGroup(title);
            foreach (var item in dictionaryUsed)
            {
                ChartDynamicMaterial mat = new ChartDynamicMaterial(barMaterial, Color.grey, Color.clear);
                barChart.DataSource.AddCategory(item.Key, mat);
                var val = Convert.ToDouble(item.Value[0]);
                barChart.DataSource.SetValue(item.Key, title, val);
            }
        }

    }

    public void PlotNewGraph(string title, string mode)      //Plotting new graph with same data_set but different property
    {
        var titleIndex = -1;

        Dictionary<string, float[]> dictionaryUsed;
        string[] titles;

        switch (mode)
        {
            case "Linear Regression":
                dictionaryUsed = data_set_1;
                titles = titles_1;
                break;
            case "Decision Tree":
                dictionaryUsed = data_set_2;
                titles = titles_2;
                break;
            default:
                dictionaryUsed = data_set_1;
                titles = titles_1;
                break;
        }

        for (int i = 0; i < titles.Length; i++)         //Finding index of title in titles[]
        {
            if (titles[i] == title)
                titleIndex = i;
        }

        if (titleIndex != -1)
        {
            //Plot new graph with title attribute

            Debug.Log("Plotting new graph");
            BarChart barChart = GetComponent<BarChart>();

            if (barChart != null)
            {
                barChart.DataSource.ClearCategories();
                barChart.DataSource.ClearGroups();
                barChart.DataSource.AutomaticMaxValue = true;           //Set Max value to Auto
                barChart.DataSource.AddGroup(title);

                foreach (var item in dictionaryUsed)
                {
                    ChartDynamicMaterial mat = new ChartDynamicMaterial(barMaterial, Color.grey, Color.clear);
                    barChart.DataSource.AddCategory(item.Key, mat);
                    var val = Convert.ToDouble(item.Value[titleIndex]);
                    barChart.DataSource.SetValue(item.Key, title, val);
                }
            }
        }

    }
}
