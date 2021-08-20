using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
#if UNITY_EDITOR
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
#endif

public class GoogleSheetsWrapper : ScriptableObject
{
#if UNITY_EDITOR
    static string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
#endif
    public string ApplicationName = "Google API wrapper";
    public string spreadsheetId = "123456";// set it to your spread sheet id


    public void ImportGoogleSheet()
    {
#if UNITY_EDITOR
        UserCredential credential;
        using (var stream =
               new FileStream(Application.dataPath + "/GoogleSheetsAPI/" + "credentials.json", FileMode.Open, FileAccess.Read)) //you have to put the file to this path. The credentials.json you can download from the Google Cloud console.
        {
            // The file token.json stores the user's access and refresh tokens, and is created
            // automatically when the authorization flow completes for the first time.
            string credPath = Application.dataPath + "/GoogleSheetsAPI/token.json";
            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromStream(stream).Secrets,
                Scopes,
                "user",
                CancellationToken.None,
                new FileDataStore(credPath, true)).Result;
            Debug.Log("Credential file has been saved to: " + credPath);
        }

        // Create Google Sheets API service.
        var service = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName,
        });

        //getting a document with sheets
        List<string> ranges = new List<string>();
        bool includeGridData = true;
        SpreadsheetsResource.GetRequest request = service.Spreadsheets.Get(spreadsheetId);
        request.Ranges = ranges;
        request.IncludeGridData = includeGridData;
        Google.Apis.Sheets.v4.Data.Spreadsheet spreadSheet = request.Execute();

        foreach (var i in spreadSheet.Sheets)
        {
            for (int c = 0; c < i.Data.Count; c++)
            {
                var rowData = i.Data[c].RowData;
                Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();

                for (int j = 1; j < rowData.Count; j++)//for my reason, I want to get data from each second row.
                {
                    if (rowData[j].Values != null && rowData[j].Values.Count > 0)
                    {
                        if ((rowData[j].Values[0] != null && rowData[j].Values[0].FormattedValue != null) && (rowData[j].Values[1] != null && rowData[j].Values[1].FormattedValue != null))
                        {
                            keyValuePairs.Add(rowData[j].Values[0].FormattedValue, rowData[j].Values[1].FormattedValue);
                        }
                    }
                }
            }
            //You got a sample dicitonary "keyValuePairs". For my reason I transfer the data from this dictionary to my scriptable object.
            //for example
            //LevelsDataContainer textData = ScriptableObject.CreateInstance<LevelsDataContainer>();
            //textData.Info = new LevelsDataContainer.InfoDictionary();
            //foreach (var e in keyValuePairs)
            //{
            //    textData.Info.Add(e.Key, e.Value);
            //}
            //string path = Path.Combine(LevelsDataPath, i.Properties.Title + ".asset");
            //AssetDatabase.CreateAsset(textData, path);
            //AssetDatabase.SaveAssets();
        }
#endif
    }

    [MenuItem("Assets/Create/CreateGoogleSheetImproter")]
    public static void CreateGoogleAPISheetsSO()
    {
#if UNITY_EDITOR
        GoogleSheetsWrapper inst = ScriptableObject.CreateInstance<GoogleSheetsWrapper>();
        string path = Application.dataPath + "/Data/GoogleSheetImporter.asset";//for example
        AssetDatabase.CreateAsset(inst, path);
        AssetDatabase.SaveAssets();
        Selection.activeObject = inst;
#endif
    }
}
