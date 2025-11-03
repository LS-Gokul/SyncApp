using System;
using System.Linq;
using System.Text.Json;

namespace LSSyncApp.Controllers
{
    public class CustomerAPI
    {
        public static ODBCSyncParam _OdbcSyncParam = new ODBCSyncParam();
        private int iiSuccess, iiCmd;
        private string isReturn;

        public AuditLogVar CustAPI(ODBCSyncParam osp, IProgress<int> progress) 
        {
            _OdbcSyncParam = osp;
            iiCmd = _OdbcSyncParam.odbcGlobalVar.giCmd;
            _OdbcSyncParam._auditLogVar.LogId = _OdbcSyncParam.isLogTime;
            _OdbcSyncParam._auditLogVar.StartTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//Set Audit Log Var
            
            if (iiCmd == 0)
            {
                if (progress != null) progress.Report(1);//Progress Bar
            }
            string lsRet = APISync(progress);
            if (iiCmd == 0)
            {
                if (progress != null) progress.Report(100);//Progress Bar
            }
            if (lsRet.Contains("Failed"))
            {
                _OdbcSyncParam._auditLogVar.Object = "Sync";//Set Audit Log Var
                _OdbcSyncParam._auditLogVar.ChildObject = "ODBC-Sync";//Set Audit Log Var
                _OdbcSyncParam._auditLogVar.Sequence = 1;//Set Audit Log Var
                _OdbcSyncParam._auditLogVar.ObjectFromTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//Set Audit Log Var
                _OdbcSyncParam._auditLogVar.EndTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//Set Audit Log Var
                _OdbcSyncParam._auditLogVar.LogDetails = lsRet;//Set Audit Log Var
                _OdbcSyncParam._auditLogVar.Status = "Failed";//Set Audit Log Var
                _OdbcSyncParam.setStatusLog("", "", 2);
            }

            return _OdbcSyncParam._auditLogVar;
        }

        private string APISync(IProgress<int> progress)
        {
            try
            {
            int liApiCnt;
            /*****************************************************/
            /////1. Get the List of API
            /*****************************************************/
            _OdbcSyncParam.odbcGlobalVar._MasterConfig.GetCustomerApiList(_OdbcSyncParam.odbcGlobalVar, out iiSuccess, out isReturn);
            if (iiSuccess == 0)
            {
                return isReturn;
            }
            JsonElement ljeAPIList = _OdbcSyncParam.odbcGlobalVar.createJsonElement(isReturn);
            liApiCnt = ljeAPIList.EnumerateArray().Count();
            if(liApiCnt == 0) 
            {
                return "No APIs Assigned";
            }

                /*****************************************************/
                /////2. Loop
                /*****************************************************/
                for (int i = 0; i < liApiCnt; i++)
                {
                    string lsTableCode, lsTableType, lsTblName, lsDelCol, lsModTimeCol;
                    string lsReqMethod, lsReqUrl, lsReqBody, lsReqHead, lsReqTokenType, lsReqAuth, lsParamFromTime, lsParamToTime;
                    string lsResDataType, lsResCode, lsResHead, lsResRoot, lsResColList;
                    string lsDepTabCode1, lsDepRoot1, lsDepcolList1;
                    string lsDepTabCode2, lsDepRoot2, lsDepcolList2;
                    string lsDepTabCode3, lsDepRoot3, lsDepcolList3;
                    string lsDepTabCode4, lsDepRoot4, lsDepcolList4;
                    string lsDepTabCode5, lsDepRoot5, lsDepcolList5;

                    //Get Input from API & assign it into a variable
                    lsTableCode = ljeAPIList[i].GetProperty("tableCode").ToString();
                    lsTableType = ljeAPIList[i].GetProperty("tableType").ToString();
                    lsTblName = ljeAPIList[i].GetProperty("tblName").ToString();
                    lsReqMethod = ljeAPIList[i].GetProperty("reqMethod").ToString();
                    lsReqUrl = ljeAPIList[i].GetProperty("reqUrl").ToString();
                    lsReqBody = ljeAPIList[i].GetProperty("reqBody").ToString();
                    lsReqHead = ljeAPIList[i].GetProperty("reqHead").ToString();
                    lsReqTokenType = ljeAPIList[i].GetProperty("reqTokenType").ToString();
                    lsReqAuth = ljeAPIList[i].GetProperty("reqAuth").ToString();
                    lsParamFromTime = ljeAPIList[i].GetProperty("paramFromTime").ToString();
                    lsParamToTime = ljeAPIList[i].GetProperty("paramToTime").ToString();
                    lsResDataType = ljeAPIList[i].GetProperty("resDataType").ToString();
                    lsResCode = ljeAPIList[i].GetProperty("resCode").ToString();
                    lsResHead = ljeAPIList[i].GetProperty("resHead").ToString();
                    lsResRoot = ljeAPIList[i].GetProperty("resRoot").ToString();
                    lsResColList = ljeAPIList[i].GetProperty("resColList").ToString();
                    lsDelCol = ljeAPIList[i].GetProperty("delCol").ToString();
                    lsModTimeCol = ljeAPIList[i].GetProperty("modTimeCol").ToString();
                    lsDepTabCode1 = ljeAPIList[i].GetProperty("depTabCode1").ToString();
                    lsDepRoot1 = ljeAPIList[i].GetProperty("depRoot1").ToString();
                    lsDepcolList1 = ljeAPIList[i].GetProperty("depcolList1").ToString();
                    lsDepTabCode2 = ljeAPIList[i].GetProperty("depTabCode2").ToString();
                    lsDepRoot2 = ljeAPIList[i].GetProperty("depRoot2").ToString();
                    lsDepcolList2 = ljeAPIList[i].GetProperty("depcolList2").ToString();
                    lsDepTabCode3 = ljeAPIList[i].GetProperty("depTabCode3").ToString();
                    lsDepRoot3 = ljeAPIList[i].GetProperty("depRoot3").ToString();
                    lsDepcolList3 = ljeAPIList[i].GetProperty("depcolList3").ToString();
                    lsDepTabCode4 = ljeAPIList[i].GetProperty("depTabCode4").ToString();
                    lsDepRoot4 = ljeAPIList[i].GetProperty("depRoot4").ToString();
                    lsDepcolList4 = ljeAPIList[i].GetProperty("depcolList4").ToString();
                    lsDepTabCode5 = ljeAPIList[i].GetProperty("depTabCode5").ToString();
                    lsDepRoot5 = ljeAPIList[i].GetProperty("depRoot5").ToString();
                    lsDepcolList5 = ljeAPIList[i].GetProperty("depcolList5").ToString();

                    /*****************************************************/
                    /////3. Check the Key Parameters Like Method, Authentication, Headers, Body and other Params
                    /*****************************************************/
                    if(lsReqMethod == null || lsReqMethod == "")
                    {
                        _OdbcSyncParam.setStatusLog("status", "Request Method Should not be Blank", 1);
                        _OdbcSyncParam.auditLogReset(2);
                    }



                    /*****************************************************/
                    /////4. Call the API & fetch the data
                    /*****************************************************/
                    if(lsReqMethod == "Get")
                    {

                    }
                    else if(lsReqMethod == "Post")
                    {

                    }

                    /*****************************************************/
                        /////5. If Status code is not Success then go to next loop
                        /*****************************************************/




                        /*****************************************************/
                        /////6. Construct the data format into Data table
                        /*****************************************************/





                        /*****************************************************/
                        /////7. Delete if the table Delete column name exists
                        /*****************************************************/




                        /*****************************************************/
                        /////8. Check Primary key and Construct Merge / do Bulk Delete
                        /*****************************************************/




                        /*****************************************************/
                        /////9. Insert Data
                        /*****************************************************/




                        /*****************************************************/
                        /////11. Check for Dependant Table 1 if Exists Repeat the step 6 - 9.
                        /*****************************************************/




                        /*****************************************************/
                        /////12. Check for Dependant Table 2 if Exists Repeat the step 6 - 9.
                        /*****************************************************/




                        /*****************************************************/
                        /////13. Check for Dependant Table 3 if Exists Repeat the step 6 - 9.
                        /*****************************************************/



                        /*****************************************************/
                        /////14. Check for Dependant Table 4 if Exists Repeat the step 6 - 9.
                        /*****************************************************/



                        /*****************************************************/
                        /////15. Check for Dependant Table 5 if Exists Repeat the step 6 - 9.
                        /*****************************************************/

                }
            }
            /*****************************************************/
            /////16. End Loop
            /*****************************************************/
            catch (Exception Ex)
            {
                return "Failed - " + Ex.Message;
            }
            return "";
        }

    }
}