#if UNITY_EDITOR
#if !CT_RTV && !CT_RADIO && !CT_BWF && !CT_TR && !CT_OC && !CT_DJ && !CT_TPS && !CT_TB && !CT_TPB && !CT_SKY3D
using UnityEngine;
using UnityEditor;

namespace Crosstales.FB.EditorTask
{
    /// <summary>Reminds the customer to visit our other assets.</summary>
    [InitializeOnLoad]
    public static class ReminderCT
    {
        private const int numberOfDays = 11;
        private const int maxDays = numberOfDays * 2;
        private const string KEY_CT_REMINDER_DATE = Util.Constants.KEY_PREFIX + "CT_REMINDER_DATE";
        private const string KEY_CT_REMINDER_COUNT = Util.Constants.KEY_PREFIX + "CT_REMINDER_COUNT";

        #region Constructor

        static ReminderCT()
        {
            string lastDate = EditorPrefs.GetString(KEY_CT_REMINDER_DATE);
            int count = EditorPrefs.GetInt(KEY_CT_REMINDER_COUNT) + 1;
            string date = System.DateTime.Now.ToString("yyyyMMdd"); // every day
            //string date = System.DateTime.Now.ToString("yyyyMMddHHmm"); // every minute (for tests)

            if (maxDays <= count && !date.Equals(lastDate))
            {
                //if (count % 1 == 0) // for testing only
                if (count % numberOfDays == 0) // && EditorConfig.CT_REMINDER_CHECK)
                {
                    int option = EditorUtility.DisplayDialogComplex(Util.Constants.ASSET_NAME + " - Our other assets",
                                "Thank you for using '" + Util.Constants.ASSET_NAME + "'!" + System.Environment.NewLine + System.Environment.NewLine + "Please take a look at our other assets.",
                                "Yes, show me!",
                                "Not right now",
                                "Don't ask again!");

                    switch (option)
                    {
                        case 0:
                            Application.OpenURL(Common.Util.BaseConstants.ASSET_CT_URL);
                            //EditorConfig.CT_REMINDER_CHECK = false;
                            count = 9999;

                            Debug.LogWarning("<color=red>" + Common.Util.BaseHelper.CreateString("❤", 400) + "</color>");
                            Debug.LogWarning("<b>+++ Thank you for visiting our assets! +++</b>");
                            Debug.LogWarning("<color=red>" + Common.Util.BaseHelper.CreateString("❤", 400) + "</color>");
                            break;
                        case 1:
                            // do nothing!
                            break;
                        default:
                            //EditorConfig.CT_REMINDER_CHECK = false;
                            count = 9999;
                            break;
                    }

                    //EditorConfig.Save();
                }

                EditorPrefs.SetString(KEY_CT_REMINDER_DATE, date);
                EditorPrefs.SetInt(KEY_CT_REMINDER_COUNT, count);
            }
        }

        #endregion

    }
}
#endif
#endif
// © 2018-2019 crosstales LLC (https://www.crosstales.com)