using Microsoft.Win32.TaskScheduler;
using System;
using System.Data;

namespace LSSyncApp.Functions
{
    public class Scheduler
    {
        GlobalVariable gblVar = new GlobalVariable();
        public static string isSqlQuery, isReturn;

        public string init(GlobalVariable gbl, IProgress<int> progress)
        {
            gblVar = gbl;
            try
            {
                int rCnt, liDays, liInterval;
                DateTime ldStartDate;
                string lsParam, lsTaskName, lsPath;

                gblVar._MasterConfig.GetSchedulerList(gblVar, out int liSuccess, out isReturn);

                progress.Report(1);
                if (liSuccess == 1 && isReturn != "" && isReturn != null)
                {
                    Forms.Schedulers _Scheduler = new Forms.Schedulers(gblVar, isReturn);
                    _Scheduler.ShowDialog();
                    DataTable _SchedulerData = _Scheduler._SchedulerData;

                    rCnt = _SchedulerData.Rows.Count;
                    int i = 0;
                    foreach (DataRow dr in _SchedulerData.Rows)
                    {
                        //liDays = int.Parse(ljeSyncSch[i].GetProperty("syncDays").ToString());
                        //liInterval = int.Parse(ljeSyncSch[i].GetProperty("syncInterval").ToString());
                        //ldStartDate = DateTime.Parse(ljeSyncSch[i].GetProperty("startTime").ToString());
                        //lsParam = ljeSyncSch[i].GetProperty("syncParam").ToString();

                        string lsNewInterval = dr.ItemArray[6].ToString();

                        liDays = int.Parse(dr.ItemArray[2].ToString());
                        liInterval = int.Parse(dr.ItemArray[3].ToString());
                        ldStartDate = DateTime.Parse(dr.ItemArray[1].ToString());
                        lsParam = dr.ItemArray[4].ToString();

                        lsTaskName = "LS_" + lsParam.Replace("-", "") + "_" + gblVar.firmCode;
                        lsParam += "~" + gblVar.firmCode;
                        lsPath = gblVar.gsApplPath + "LSEngine.exe";

                        if(lsNewInterval != "" && lsNewInterval != null)
                        {
                            //"30 Minutes", "1 Hour", "2 Hours", "5 Hours", "24 Hours"
                            liInterval = int.Parse(((lsNewInterval == "30 Minutes" ? 0.5 :
                                    lsNewInterval == "1 Hour" ? 1 :
                                    lsNewInterval == "2 Hours" ? 2 :
                                    lsNewInterval == "5 Hours" ? 5 :
                                    lsNewInterval == "24 Hours" ? 24 : 1) * 60).ToString());
                        }
                        else
                        {
                            if (liInterval == 0)
                            {
                                liInterval = 24 * 60;
                            }
                        }
                        
                        nextSch(lsTaskName, lsPath, liInterval, lsParam, ldStartDate);
                        progress.Report(i + 1 * (100 / rCnt));
                        i++;
                    }
                }
                progress.Report(100);
                return "Success";
            }
            catch(Exception e)
                {
                if(e.Message.Contains("Access is denied"))
                {
                    return "Failed" + Environment.NewLine + e.Message + Environment.NewLine + "Please run the application as Administratior and create scheduler once again!!!";
                }
                else
                {
                    return "Failed" + Environment.NewLine + e.Message;
                }
            }
        }

        private void nextSch(string asTaskName, string asPath, int aiInterval, string asParam, DateTime adStartDate)
        {
            var file = asPath;
            TaskService ts = new TaskService();
            TaskDefinition td = ts.NewTask();
            //td.Principal.LogonType = TaskLogonType.S4U;
            //td.Principal.UserId = Environment.UserName;
            Trigger trigger = new DailyTrigger();
            trigger.Repetition.Interval = TimeSpan.FromMinutes(aiInterval);
            trigger.StartBoundary = adStartDate;
            td.Triggers.Add(trigger);
            td.Actions.Add(new ExecAction(file, asParam, gblVar.gsApplPath));
            ts.RootFolder.RegisterTaskDefinition(asTaskName, td);

            //TaskService task = new TaskService();
            //Task taskdDef = task.FindTask(lsTaskName);
            /*
            SchedulerResponse response = WindowTaskScheduler
                .Configure()
                .CreateTask(lsTaskName, lsPath + " " + lsParam)
                .RunDaily()
                .RunEveryXMinutes(liInterval)
                .RunDurationFor(new TimeSpan(18, 0, 0))
                //.SetStartDate(new DateTime(2015, 8, 8))
                .SetStartDate(ldStartDate)
                .SetStartTime(new TimeSpan(0, 0, 0))
                .Execute();*/
            //progress.Report(int.Parse((Math.Ceiling(decimal.Parse((i + 1 * (100/rCnt)).ToString()))).ToString()));
        }
    }
}