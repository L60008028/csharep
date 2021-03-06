﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;
using model;
using System.Data.SqlClient;

namespace dal
{
    public class SmsMobileDAL
    {

        LocalParams lp;
        public SmsMobileDAL()
        {
            lp = new LocalParams();
            MySqlHelper.ConnectionstringLocalTransaction = lp.SqlConnStr;
        }

        private SmsMobileModel FillData(SqlDataReader sdr)
        {
            SmsMobileModel m = new SmsMobileModel();
            m.mobile = sdr.GetString(0);
            m.smsCount = sdr.GetInt32(1);
            m.sendTime = sdr.GetDateTime(2);
            return m;
        }


        public bool UpdateUnknowReportStatus(int id, string eprId)
        {
            try
            {
                string sql = string.Format("update TBL_SMS_MOBILES set [RESULT]=NULL where Id={0} and EprId='{1}'", id, eprId);

                int re = SqlHelper.ExecuteNonQuery(lp.SqlConnStr, CommandType.Text, sql, null);
                if (re > 0)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {

                MyDelegateFunc.WriteFmLog("SmsMobileDAL=>UpdateUnknowReportStatus:Exception:" + ex.ToString());
            }
            return false;

        }

        public bool UpdateUnknowReportStatus(List<string> list)
        {

            try
            {
                List<string> failList = SqlHelper.BatchExec(lp.SqlConnStr, list);
                if (failList == null || failList.Count == 0)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {

                MyDelegateFunc.WriteFmLog("SmsMobileDAL=>UpdateUnknowReportStatus:Exception:" + ex.ToString());
            }
            return false;

        }

        public int GetUnknowCount(string eprid, string userid, string y, string m, string d)
        {
            int result = 0;
            try
            {
                DateTime dt = DateTime.Now;
                string begintime = y + "-" + m + "-" + d;//开始时间
                string endtime = begintime + " 23:59:59";//结果时间
                object[] obj = { eprid, begintime, endtime, userid };
                string sql = string.Format("select count(1) from TBL_SMS_MOBILES where EprId={0} and [SENDTIME] between '{1}' and '{2}'  and UserId='{3}' and [RESULT]=NULL and [STATUS]=3 ", obj);
                object re = SqlHelper.ExecuteScalar(lp.SqlConnStr, CommandType.Text, sql, null);
                if (re != null)
                {
                    int.TryParse(re.ToString(), out result);
                }
                MyDelegateFunc.WriteFmLog("SmsMobileDAL=>GetUnknowList:SQL:" + sql + ",耗时：" + (DateTime.Now - dt).TotalMilliseconds + "毫秒");
            }
            catch (Exception ex)
            {

                MyDelegateFunc.WriteFmLog("SmsMobileDAL=>GetUnknowCount:Exception:" + ex.ToString());
            }
            return result;
        }

        public List<SmsMobileModel> GetUnknowList(string eprid, string userid, string y, string m, string d)
        {
            List<SmsMobileModel> list = new List<SmsMobileModel>();
            try
            {
                DateTime dt = DateTime.Now;
                string begintime = y + "-" + m + "-" + d;//开始时间
                string endtime = begintime + " 23:59:59";//结果时间
                object[] obj = { begintime, endtime, eprid, userid };
                string sql = string.Format("select Id, [CLIENTMSGID] from TBL_SMS_MOBILES where    SendTime SENDTIME '{0}' and '{1}' and EprId={2}  and UserId='{3}' and [RESULT]=1 and [STATUS]=3 order by Id ", obj);
                using (MySqlDataReader sdr = MySqlHelper.ExecuteReader(MySqlHelper.ConnectionstringLocalTransaction, CommandType.Text, sql, null))
                {
                    while (sdr.Read())
                    {
                        SmsMobileModel smm = new SmsMobileModel();
                        smm.id = sdr.GetInt32(0);
                        smm.clientMsgId = sdr.GetString(1);
                        list.Add(smm);
                    }
                }
                MyDelegateFunc.WriteFmLog("SmsMobileDAL=>GetUnknowList:SQL:" + sql + ",耗时：" + (DateTime.Now - dt).TotalMilliseconds + "毫秒");
            }
            catch (Exception ex)
            {

                MyDelegateFunc.WriteFmLog("SmsMobileDAL=>GetUnknowList:Exception:" + ex.ToString());
            }

            return list;
        }
        public Dictionary<int, int> GetSumSMScountByDay(int y, int m, int d)
        {
            DateTime dt = DateTime.Now;
            string begintime = y + "-" + m + "-" + d;//开始时间
            string endtime = begintime + " 23:59:59";//结果时间
            string sql = string.Format("Select EprId,Sum(MSGCOUNT) From TBL_SMS_MOBILES Where SendTime between '{0}' and '{1}'  and [STATUS]=3 and ([RESULT]=1 or [RESULT] is null or [RESULT]='' ) and EPRID>0 and [GATEWAY]<50000000  group by EprId", begintime, endtime);
            //string sql = "Select EprId,Sum(SmsCount) From t_smsmobile_t   group by EprId";

            try
            {
                Dictionary<int, int> re = new Dictionary<int, int>();
                using (SqlDataReader sdr = SqlHelper.ExecuteReader(lp.SqlConnStr, CommandType.Text, sql, null))
                {
                    while (sdr.Read())
                    {
                        int eprId = sdr.GetInt32(0);
                        int count = sdr.GetInt32(1);
                        //("getSumSMScount==>eprId=" + eprId + ",count=" + count);
                        re.Add(eprId, count);

                    }
                }
                MyDelegateFunc.WriteFmLog("SmsMobileDAL=>GetSumSMScountByDay:SQL:" + sql + ",耗时：" + (DateTime.Now - dt).TotalMilliseconds + "毫秒");
                return re;
            }
            catch (Exception ex)
            {
                MyDelegateFunc.WriteFmLog("SmsMobileDAL=>GetSumSMScountByDay:ExceptionSQL:" + sql);
                MyDelegateFunc.WriteFmLog("SmsMobileDAL=>GetSumSMScountByDay:Exception:" + ex.ToString());
            }

            return null;
        }

        /// <summary>
        /// 取企业发送量统计
        /// </summary>
        /// <param name="eprIds">企业ID LIST</param>
        /// <param name="y">年</param>
        /// <param name="m">月</param>
        /// <param name="d">日</param>
        /// <returns></returns>
        public Dictionary<int, MobileTypeCountModel> GetSumSMScountByDay(List<int> eprIds, int y, int m, int d)
        {
            string sql = "";
            DateTime dt = DateTime.Now;
            // MobileType:1 联通  2移动 3电信  4 未知
            try
            {
                string begintime = y + "-" + m + "-" + d;//开始时间
                string endtime = begintime + " 23:59:59";//结果时间
                Dictionary<int, MobileTypeCountModel> re = new Dictionary<int, MobileTypeCountModel>();
                //foreach (int eprid in eprIds)
                //{
                //sql = string.Format("select Sum(tmp.ct),Sum(tmp.cm) from (select IFNULL((case when MobileType<3 then Sum(SmsCount) end),0) as 'ct',IFNULL((case when MobileType=3 then Sum(SmsCount) end),0) as 'cm' From t_smsmobile_t where  SendTime between '{0}' and '{1}' and EprId='{2}' and SubmitStatus='1' and ReportStatus > -1 and GatewayNum<40001 Group by MobileType) as tmp;", begintime,endtime, eprid);
                //StringBuilder sb = new StringBuilder();
                //sb.Append("select EprId ,Sum(tmp.ct),Sum(tmp.cm) from ( select EprId, IFNULL((case when MobileType<3 then Sum(SmsCount) end),0) as 'ct',IFNULL((case when MobileType=3 then Sum(SmsCount) end),0) as 'cm' From t_smsmobile_t sms");
                //sb.Append(" INNER JOIN t_eprinfo epr on sms.EprId=epr.Id");
                //sb.Append(string.Format(" where  sms.SendTime between '{0}' and '{1}'  and sms.SubmitStatus='1' and sms.ReportStatus > -1 and sms.GatewayNum<40001 Group by sms.MobileType,sms.EprId) as tmp GROUP BY EprId", begintime, endtime));
                //sql = sb.ToString();

                sql = string.Format("select EprId ,Sum(tmp.ct),Sum(tmp.cm) from (select EprId, ISNULL((case when ISNULL(MobileType,1)<3 then Sum([MSGCOUNT]) end),0) as 'ct',ISNULL((case when MobileType=3 then Sum([MSGCOUNT]) end),0) as 'cm' From [TBL_SMS_MOBILES] WHERE SENDTIME BETWEEN '{0}' AND  '{1}'  AND EPRID>0  AND [STATUS]=3 AND ISNULL(RESULT,1)<>0 and RESEND<2 AND [GATEWAY]<50000000 GROUP BY EPRID,MobileType) as tmp GROUP BY EprId", begintime, endtime);
                using (SqlDataReader sdr = SqlHelper.ExecuteReader(lp.SqlConnStr, CommandType.Text, sql, null))
                {
                    while (sdr.Read())
                    {
                        MobileTypeCountModel mtcm = new MobileTypeCountModel();
                        int eprid = sdr.GetInt32(0);
                        if (!sdr.IsDBNull(1))
                        {
                            mtcm.CTcount = sdr.GetInt32(1);
                        }
                        else
                        {
                            mtcm.CTcount = 0;
                        }
                        if (!sdr.IsDBNull(2))
                        {
                            mtcm.CMcount = sdr.GetInt32(2);
                        }
                        else
                        {
                            mtcm.CMcount = 0;
                        }
                        re.Add(eprid, mtcm);
                    }
                }
                //}
                MyDelegateFunc.WriteFmLog("SmsMobileDAL=>GetSumSMScountByDay:LastSQL:" + sql + "\n耗时：" + (DateTime.Now - dt).TotalMilliseconds + "毫秒");
                return re;
            }
            catch (Exception ex)
            {
                MyDelegateFunc.WriteFmLog("SmsMobileDAL=>GetSumSMScountByDay:ExceptionSQL:" + sql);
                MyDelegateFunc.WriteFmLog("SmsMobileDAL=>GetSumSMScountByDay:Exception:" + ex);
            }

            return null;
        }




        /// <summary>
        /// 取企业月发送量统计
        /// </summary>
        /// <param name="eprIds">企业ID LIST</param>
        /// <param name="begintime">开始时间</param>
        /// <param name="endtime">结束</param>
        /// <returns></returns>
        public Dictionary<int, MobileTypeCountModel> GetSumSMScountByMomth(List<int> eprIds, string begintime, string endtime)
        {
            string sql = "";
            DateTime dt = DateTime.Now;
            try
            {


                Dictionary<int, MobileTypeCountModel> re = new Dictionary<int, MobileTypeCountModel>();
                foreach (int eprid in eprIds)
                {
                   // sql = string.Format("select Sum(tmp.ct),Sum(tmp.cm) from (select IFNULL((case when MobileType<3 then Sum(SmsCount) end),0) as 'ct',IFNULL((case when MobileType=3 then Sum(SmsCount) end),0) as 'cm' From t_smsmobile_t where  SendTime between '{0}' and '{1}' and EprId={2} and SubmitStatus='1' and ReportStatus > -1 and GatewayNum<40001 Group by MobileType) as tmp;", begintime, endtime, eprid);
                   // sql = string.Format("  select EprId ,Sum(tmp.ct),Sum(tmp.cm) from (select EprId, ISNULL((case when MobileType<3 then Sum([MSGCOUNT]) end),0) as 'ct',ISNULL((case when MobileType=3 then Sum([MSGCOUNT]) end),0) as 'cm' From [TBL_SMS_MOBILES] WHERE SENDTIME BETWEEN '{0}' AND  '{1}'  AND EPRID={2} AND [GATEWAY]<50000000 GROUP BY EPRID,MobileType) as tmp GROUP BY EprId",begintime, endtime, eprid);
                    sql = string.Format("select EprId ,Sum(tmp.ct),Sum(tmp.cm) from (select EprId, ISNULL((case when ISNULL(MobileType,1)<3 then Sum([MSGCOUNT]) end),0) as 'ct',ISNULL((case when MobileType=3 then Sum([MSGCOUNT]) end),0) as 'cm' From [TBL_SMS_MOBILES] WHERE SENDTIME BETWEEN '{0}' AND  '{1}'  AND EPRID={2}  AND [STATUS]=3 AND ISNULL(RESULT,1)<>0 AND [GATEWAY]<50000000 GROUP BY EPRID,MobileType) as tmp GROUP BY EprId", begintime, endtime, eprid);
                    using (SqlDataReader sdr = SqlHelper.ExecuteReader(lp.SqlConnStr, CommandType.Text, sql, null))
                    {
                        if (sdr.Read())
                        {
                            MobileTypeCountModel mtcm = new MobileTypeCountModel();
                            if (!sdr.IsDBNull(0))
                            {
                                mtcm.CTcount = sdr.GetInt32(0);
                            }
                            else
                            {
                                mtcm.CTcount = 0;
                            }
                            if (!sdr.IsDBNull(1))
                            {
                                mtcm.CMcount = sdr.GetInt32(1);
                            }
                            else
                            {
                                mtcm.CMcount = 0;
                            }
                            mtcm.TotalCount = mtcm.CTcount + mtcm.CMcount;
                            re.Add(eprid, mtcm);
                        }
                    }
                }
                MyDelegateFunc.WriteFmLog("SmsMobileDAL=>GetSumSMScountByDay:LastSQL:" + sql + ",耗时：" + (DateTime.Now - dt).TotalMilliseconds + "毫秒");
                return re;
            }
            catch (Exception ex)
            {
                MyDelegateFunc.WriteFmLog("SmsMobileDAL=>GetSumSMScountByDay2:ExceptionSQL:" + sql);
                MyDelegateFunc.WriteFmLog("SmsMobileDAL=>GetSumSMScountByDay2:Exception:" + ex.ToString());
            }

            return null;
        }




        /// <summary>
        /// 取企业发送量统计
        /// </summary>
        /// <param name="eprIds">企业ID LIST</param>
        /// <param name="begintime">开始时间</param>
        /// <param name="endtime">结束时间</param>
        /// <returns></returns>
        public Dictionary<int, MobileTypeCountModel> GetSumSMScountByDay(List<int> eprIds, string begintime, string endtime)
        {
            string sql = "";
            DateTime dt = DateTime.Now;
            try
            {
                string erids = "";
                if (eprIds != null && eprIds.Count > 0)
                {
                    erids = String.Join(",", eprIds);
                    sql = string.Format("select EprId ,Sum(tmp.ct),Sum(tmp.cm) from (select EprId, ISNULL((case when ISNULL(MobileType,1)<3 then Sum([MSGCOUNT]) end),0) as 'ct',ISNULL((case when MobileType=3 then Sum([MSGCOUNT]) end),0) as 'cm' From [TBL_SMS_MOBILES] WHERE SENDTIME BETWEEN '{0}' AND  '{1}'  AND EPRID IN({2})  AND [STATUS]=3 AND ISNULL(RESULT,1)<>0 and RESEND<2 AND [GATEWAY]<50000000 GROUP BY EPRID,MobileType) as tmp GROUP BY EprId", begintime, endtime, erids);
                }
                else
                {
                    sql = string.Format("select EprId ,Sum(tmp.ct),Sum(tmp.cm) from (select EprId, ISNULL((case when ISNULL(MobileType,1)<3 then Sum([MSGCOUNT]) end),0) as 'ct',ISNULL((case when MobileType=3 then Sum([MSGCOUNT]) end),0) as 'cm' From [TBL_SMS_MOBILES] WHERE SENDTIME BETWEEN '{0}' AND  '{1}'  AND EPRID>0  AND [STATUS]=3 AND ISNULL(RESULT,1)<>0 and RESEND<2 AND [GATEWAY]<50000000 GROUP BY EPRID,MobileType) as tmp GROUP BY EprId", begintime, endtime);
                }

                Dictionary<int, MobileTypeCountModel> re = new Dictionary<int, MobileTypeCountModel>();
                // foreach (int eprid in eprIds)
                // {
                //sql = string.Format("select Sum(tmp.ct),Sum(tmp.cm) from (select IFNULL((case when MobileType<3 then Sum(SmsCount) end),0) as 'ct',IFNULL((case when MobileType=3 then Sum(SmsCount) end),0) as 'cm' From t_smsmobile_t where  SendTime between '{0}' and '{1}' and EprId={2} and SubmitStatus='1' and ReportStatus > -1 and GatewayNum<40001 Group by MobileType) as tmp;", begintime, endtime, eprid);
               // StringBuilder sb = new StringBuilder();
                //sb.Append("select EprId ,Sum(tmp.ct),Sum(tmp.cm) from ( select EprId, IFNULL((case when MobileType<3 then Sum(SmsCount) end),0) as 'ct',IFNULL((case when MobileType=3 then Sum(SmsCount) end),0) as 'cm' From t_smsmobile_t sms");
                //sb.Append(" INNER JOIN t_eprinfo epr on sms.EprId=epr.Id");
                //sb.Append(string.Format(" where  sms.SendTime between '{0}' and '{1}'  and sms.SubmitStatus='1' and sms.ReportStatus > -1 and sms.GatewayNum<40001 Group by sms.MobileType,sms.EprId) as tmp GROUP BY EprId", begintime, endtime));
                //sql = sb.ToString();

               
                using (SqlDataReader sdr = SqlHelper.ExecuteReader(lp.SqlConnStr, CommandType.Text, sql, null))
                {
                    while (sdr.Read())
                    {
                        MobileTypeCountModel mtcm = new MobileTypeCountModel();
                        int eprid = sdr.GetInt32(0);
                        if (!sdr.IsDBNull(1))
                        {
                            mtcm.CTcount = sdr.GetInt32(1);
                        }
                        else
                        {
                            mtcm.CTcount = 0;
                        }
                        if (!sdr.IsDBNull(2))
                        {
                            mtcm.CMcount = sdr.GetInt32(2);
                        }
                        else
                        {
                            mtcm.CMcount = 0;
                        }
                        re.Add(eprid, mtcm);
                    }
                }
                //}
                MyDelegateFunc.WriteFmLog("SmsMobileDAL=>GetSumSMScountByDay:SQL:" + sql + ",耗时：" + (DateTime.Now - dt).TotalMilliseconds + "毫秒");
                return re;
            }
            catch (Exception ex)
            {
                MyDelegateFunc.WriteFmLog("SmsMobileDAL=>GetSumSMScountByDay2:ExceptionSQL:" + sql);
                MyDelegateFunc.WriteFmLog("SmsMobileDAL=>GetSumSMScountByDay2:Exception:" + ex.ToString());
            }

            return null;
        }






        public List<SmsMobileModel> GetListByYYMMDD(int eprId, int yy, int mm, int dd)
        {
            string sql = "";
            DateTime dt = DateTime.Now;
            try
            {
                string begintime = yy + "-" + mm + "-" + dd;//开始时间
                string endtime = begintime + " 23:59:59";//结果时间
                List<SmsMobileModel> list = new List<SmsMobileModel>();
                sql = "Select Mobile,SmsCount,SendTime from t_smsmobile_t where EprId='" + eprId + "' and  SendTime between '" + begintime + "' and '" + endtime + "'   and SubmitStatus='1' and ReportStatus<>-1 and GatewayNum<40001 ";
                // string sql = "Select Mobile,SmsCount,SendTime from t_smsmobile_t where EprId='" + eprId + "' ";
                using (SqlDataReader sdr = SqlHelper.ExecuteReader(lp.SqlConnStr, CommandType.Text, sql, null))
                {
                    while (sdr.Read())
                    {
                        list.Add(FillData(sdr));
                    }
                }
                MyDelegateFunc.WriteFmLog("SmsMobileDAL=>GetListByYYMMDD:SQL:" + sql + ",耗时：" + (DateTime.Now - dt).TotalMilliseconds + "毫秒");
                return list;

            }
            catch (Exception ex)
            {
                MyDelegateFunc.WriteFmLog("SmsMobileDAL=>GetListByYYMMDD:ExceptionSQL:" + sql);
                MyDelegateFunc.WriteFmLog("SmsMobileDAL=>GetListByYYMMDD:Exception:" + ex.ToString());
            }

            return null;
        }




    }//end
}//end
