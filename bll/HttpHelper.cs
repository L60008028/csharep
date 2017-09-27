﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Net;
using System.IO;
using System.Security.Cryptography;

namespace bll
{
    public class HttpHelper
    {
        private string host = "http://kltx.sms10000.com.cn/sdk/SMS";//短信
        private string voicehost = "http://i.huixun35.com/ClientApi?";//语音网页(高级接口)

        private string apiKey = "far*$3212@>{%^as";
 
        /// <summary>
        /// 接口地址
        /// </summary>
        public string Host
        {
            get { return host; }
            set { host = value; }
        }

         

        /// <summary>
        /// 修改电显示号码
        /// </summary>
        /// <param name="ucuid"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public string UpdateDisplayNum(string ucuid, string password, string telNum)
        {
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            string cmd = "updateDisplayNum";
            string key = GetMD5(cmd + timestamp + apiKey);
            string url = voicehost;
            string data = string.Format("cmd={0}&key={1}&timestamp={2}&ucuid={3}&password ={4}&telNum={5}", cmd, key, timestamp, ucuid, this.GetMD5(password),telNum);
            string re = this.SubmitPostByWebRequest(url, data);
       
            return re;
        }

        /// <summary>
        /// 修改电显示号码
        /// </summary>
        /// <param name="ucuid"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public string SetAddrUrl(string ucuid, string password, string callbackaddrl)
        {
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            string cmd = "setStatusUrl";
            string key = GetMD5(cmd + timestamp + apiKey);
            string url = voicehost;
            string data = string.Format("cmd={0}&key={1}&timestamp={2}&ucuid={3}&password ={4}&statusUrl={5}", cmd, key, timestamp, ucuid, password, callbackaddrl);
            string re = this.SubmitPostByWebRequest(url, data);

            return re;
        }


        

         

        

        

        /// <summary>
        /// 添加、删除坐席号码缓存
        /// </summary>
        /// <param name="mobile">手机号码</param>
        /// <param name="type">验证码</param>
        /// <returns></returns>
        public void ChangeCache(string mobile, string type)
        {
            string url = "http://202.104.149.166/HXVoiceCallback/ClearCache?";
            string data = string.Format("ucuid={0}&type={1}&mobile{2}","",type,mobile );
            string re = this.SubmitPostByWebRequest(url, data);
            //log.WriteInfo("SendSms,CheckoutCode=>host=" + url + ",data=" + data + ",result=" + re);
            //url += string.Format("cmd={0}&key={1}&timestamp={2}&checkCode={3}&mobile={4}", cmd, key, timestamp, checkCode,mobile);
            //string re = this.Submit(url);
            //HttpResult result = new HttpResult();
            //if (re.Equals("-1"))
            //{
            //    result.Result = -1;
            //    result.Msg = "请求失败!";
            //}
            //else
            //{

            //    re = "[" + re + "]";
            //    List<HttpResult> list = JSONSerialization.GetJsonList<HttpResult>(re);
            //    if (list != null && list.Count > 0)
            //    {
            //        result = list[0];
            //    }
            //    else
            //    {
            //        result.Result = -2;
            //        result.Msg = "JSON转换失败!";
            //    }
            //}
            //return result;
        }

 
 



 
         

        /// <summary>
        /// 取条数及权限信息
        /// </summary>
        /// <param name="uid">帐号</param>
        /// <param name="pwd">密码</param>
        /// <returns>100 # 已发送条数 # 还可发送条数#0不能发短信，1可以发短信#去电显示号码</returns>
        public string Getuserinfo(string uid, string pwd)
        {
            string re = "";
            string data = "";
            try
            {
                //data = string.Format("{0}cmd=getuserinfo&uid={1}&psw={2}", host, uid, GetMD5(pwd));
                data = string.Format("cmd=getuserinfo&uid={0}&psw={1}", uid, GetMD5(pwd));
                re = this.SubmitPostByWebRequest(host,data);
                //log.WriteInfo("SendSms,HttpSms.Getuserinfo=>host=" + host + ",data=" + data + ",result=" + re);
            }
            catch (Exception ex)
            {
              //  log.WriteLog("SendSms,HttpSms.Getuserinfo=>Exception:" + ex.Message + "url=" + data);
            }
            return re;
        }


        /// <summary>
        /// 获取申请显示号码的验证码
        /// </summary>
        /// <param name="uid">帐号</param>
        /// <param name="pwd">密码</param>
        /// <param name="code">固话有分机的，申请时填写分机号，如：075588250860a817</param>
        /// <param name="type">1短信发送,2语音发送</param>
        /// <returns></returns>
        public string GetCheckcode(string uid, string pwd, string code, string type)
        {
            string re = "";
            try
            {
               // string url = host;
               string  data= string.Format("cmd=checkcode&uid={0}&psw={1}&mobiles={2}&type={3}", uid, this.GetMD5(pwd), code, type);
                re = this.SubmitPostByWebRequest(host,data);
              // log.WriteInfo("SendSms,GetCheckcode=>host=" + host + ",data=" + data + ",result=" + re);
            }
            catch (Exception ex)
            {
                //log.WriteInfo("SendSms,HttpSms.GetCheckcode()=>Exception:" + ex.Message);
            }
            return re;
        }

        /// <summary>
        /// 设置去电显示号码
        /// </summary>
        /// <param name="uid">帐号</param>
        /// <param name="pwd">密码</param>
        /// <param name="num">显示号码</param>
        /// <param name="checkcode">验证码</param>
        /// <returns></returns>
        public string SetDiaplayNum(string uid, string pwd, string num, string checkcode)
        {
            string re = "";
            try
            {
               // string url = host;
               string data = string.Format("cmd=setdiaplaynum&uid={0}&psw={1}&mobiles={2}&checkcode={3}", uid, this.GetMD5(pwd), num, checkcode);
                re = this.SubmitPostByWebRequest(host,data);
                //log.WriteInfo("SendSms,SetDiaplayNum=>host=" + host + ",data=" + data + ",result=" + re);
            }
            catch (Exception ex)
            {
               // log.WriteInfo("SendSms,HttpSms.SetDiaplayNum()=>Exception:" + ex.Message);
            }
            return re;
        }


        /// <summary>
        ///  上传文件
        /// </summary>
        /// <param name="uid">帐号</param>
        /// <param name="pwd">密码</param>
        /// <param name="file">文件名</param>
        /// <param name="data">参数</param>
        /// <returns>100#201311110803</returns>
        public   string HttpUploadFile(string uid,string pwd, string file, NameValueCollection data)
        {
            string re = "-1";
            Encoding encoding = Encoding.GetEncoding("GBK");
            try
            {
                string url =host;
                url += string.Format("cmd=uploadvoicefile&uid={0}&psw={1}", uid, this.GetMD5(pwd));
                string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
                byte[] boundarybytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
                byte[] endbytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");

                //1.HttpWebRequest
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "multipart/form-data; boundary=" + boundary;
                request.Method = "POST";
                request.KeepAlive = true;
                request.Credentials = CredentialCache.DefaultCredentials;
                System.Net.ServicePointManager.DefaultConnectionLimit = 200;
                using (Stream stream = request.GetRequestStream())
                {
                    //1.1 key/value
                    string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
                    if (data != null)
                    {
                        foreach (string key in data.Keys)
                        {
                            stream.Write(boundarybytes, 0, boundarybytes.Length);
                            string formitem = string.Format(formdataTemplate, key, data[key]);
                            byte[] formitembytes = encoding.GetBytes(formitem);
                            stream.Write(formitembytes, 0, formitembytes.Length);
                        }
                    }

                    //1.2 file
                    string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: application/octet-stream\r\n\r\n";
                    byte[] buffer = new byte[4096];
                    int bytesRead = 0;
                    stream.Write(boundarybytes, 0, boundarybytes.Length);
                    string header = string.Format(headerTemplate, "file", Path.GetFileName(file));
                    byte[] headerbytes = encoding.GetBytes(header);
                    stream.Write(headerbytes, 0, headerbytes.Length);
                    using (FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
                    {
                        while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            stream.Write(buffer, 0, bytesRead);
                        }
                    }


                    //1.3 form end
                    stream.Write(endbytes, 0, endbytes.Length);
                }

                //2.WebResponse
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                using (StreamReader stream = new StreamReader(response.GetResponseStream()))
                {
                    re = stream.ReadToEnd();
                }
                //log.WriteInfo("SendSms,HttpSms.HttpUploadFile(上传文件)=>url:"+url+",Result:" + re);
            }
            catch (Exception ex)
            {
               // log.WriteLog("SendSms,HttpSms.HttpUploadFile()=>Exception:" + ex.Message);
            }
            finally
            {
                GC.Collect();
            }
            return re;
        }



        /// <summary>
        /// 修改密码，返回值：100 修改成功;101修改失败。108 密码太简单; 
        /// </summary>
        /// <param name="uid">帐号</param>
        /// <param name="oldPwd">旧密码</param>
        /// <param name="newPwd">新密码</param>
        /// <returns></returns>
        public string ModifyPassWord(string uid,string oldPwd,string newPwd)
        {
            string re = "";
            try
            {
                string url = host;
                url += string.Format("cmd=modifpsw&uid={0}&psw={1}&newpsw={2}", uid, this.GetMD5(oldPwd), newPwd);
                re = this.Submit(url);
                //log.WriteInfo("SendSms,ModifyPassWord=>url=" + url + ",result=" + re);
            }catch(Exception ex)
            {
               // log.WriteInfo("SendSms,HttpSms.ModifyPassWord()=>Exception:" + ex.Message);
            }
            return re;
        }
        /// <summary>
        /// 发语音短信
        /// </summary>
        /// <param name="uid">帐号</param>
        /// <param name="pwd">密码</param>
        /// <param name="reviceMobiles">接收号码以“,”号分隔</param>
        /// <param name="msgid">消息编号</param>
        /// <param name="content">短信内容</param>       
        /// <returns></returns>
        public string SendVoice(string uid, string pwd, string reviceMobiles, int msgid, string content)
        {
            string re = "";
            try
            {

                string data = string.Format("cmd=sendvoice&uid={0}&psw={1}&mobiles={2}&msgid={3}&msg={4}", uid, GetMD5(pwd), reviceMobiles, msgid, this.UrlEncoder(content));
              re = this.SubmitPostByWebRequest(host, data);
              // re = this.Submit(url);
              //log.WriteInfo("SendSms,SendVoice=>host=" + host + ",data=" + data + ",result=" + re);
                 
            }
            catch (Exception ex)
            {
              //  log.WriteInfo("SendSms,HttpSms.SendVoice()=>Exception:" + ex.Message);
  
            }
            return re;
        }
       

 



        /// <summary>
        /// 发语音文件_投票
        /// </summary>
        /// <param name="uid">帐号</param>
        /// <param name="pwd">密码</param>
        /// <param name="reviceMobiles">接收号码以“,”号分隔</param>
        /// <param name="msgid">消息编号</param>
        /// <param name="filename">文件名</param>       
        /// <returns></returns>
        public string SendVoiceVoteFile(string uid, string pwd, string reviceMobiles, int msgid, string filename)
        {
            string re = "";
            try
            {

                string data =string.Format("cmd=sendvote&uid={0}&psw={1}&mobiles={2}&msgid={3}&filename={4}", uid, GetMD5(pwd), reviceMobiles, msgid, filename);
                re = this.SubmitPostByWebRequest(host, data);
                //re = this.Submit(url);
               //log.WriteInfo("SendSms,SendVoiceVoteFile=>host=" + host + ",data=" + data + ",result=" + re);

            }
            catch (Exception ex)
            {
              //  log.WriteInfo("SendSms,HttpSms.SendVoiceFile()=>Exception:" + ex.Message);
            }
            return re;
        }

        /// <summary>
        /// 定时语音文件_投票
        /// </summary>
        /// <param name="uid">帐号</param>
        /// <param name="pwd">密码</param>
        /// <param name="reviceMobiles">接收号码</param>
        /// <param name="msgid">msgid</param>
        /// <param name="filename">语音文件名</param>
        /// <param name="senddate">发送日期yyyy-mm-dd</param>
        /// <param name="sendtime">发送时间HH:MM:SS</param>
        /// <returns></returns>
        public string TSendVoiceVoteFile(string uid, string pwd, string reviceMobiles, int msgid, string filename, string senddate, string sendtime)
        {
            string re = "";
            string data = "";
            try
            {
                data = string.Format("cmd=sendvote&uid={0}&psw={1}&mobiles={2}&msgid={3}&filename={4}&senddate={5}&sendtime={6}", uid, this.GetMD5(pwd), reviceMobiles, msgid, filename, senddate, sendtime);
                

                re = this.SubmitPostByWebRequest(host, data);
               // url = host + data;
               // re = this.Submit(url);
               // log.WriteInfo("SendSms,TSendVoiceVoteFile=>url=host=" + host + ",data=" + data + ",result=" + re);

            }
            catch (Exception ex)
            {
              //  log.WriteLog("SendSms,HttpSms.TSendVoiceFile=>Exception:" + ex.Message + ",url=" + data);

            }
            return re;
        }




        /// <summary>
        /// 发语音文件
        /// </summary>
        /// <param name="uid">帐号</param>
        /// <param name="pwd">密码</param>
        /// <param name="reviceMobiles">接收号码以“,”号分隔</param>
        /// <param name="msgid">消息编号</param>
        /// <param name="filename">文件名</param>       
        /// <returns></returns>
        public string SendVoiceFile(string uid, string pwd, string reviceMobiles, int msgid, string filename)
        {
            string re = "";
            try
            {
               
                string data = string.Format("cmd=sendvoice&uid={0}&psw={1}&mobiles={2}&msgid={3}&filename={4}", uid, GetMD5(pwd), reviceMobiles, msgid, filename);
                 re = this.SubmitPostByWebRequest(host, data);
               // re = this.Submit(url);
               //  log.WriteInfo("SendSms,SendVoiceFile=>host=" + host + ",data=" + data + ",result=" + re);

            }
            catch (Exception ex)
            {
                //log.WriteInfo("SendSms,HttpSms.SendVoiceFile()=>Exception:" + ex.Message);

            }
            return re;
        }

        /// <summary>
        /// 定时语音文件
        /// </summary>
        /// <param name="uid">帐号</param>
        /// <param name="pwd">密码</param>
        /// <param name="reviceMobiles">接收号码</param>
        /// <param name="msgid">msgid</param>
        /// <param name="content">语音文件名</param>
        /// <param name="senddate">发送日期yyyy-mm-dd</param>
        /// <param name="sendtime">发送时间HH:MM:SS</param>
        /// <returns></returns>
        public string TSendVoiceFile(string uid, string pwd, string reviceMobiles, int msgid, string content, string senddate, string sendtime)
        {
            string re = "";
            string url = "";
            try
            {
                string data = string.Format("cmd=sendvoice&uid={0}&psw={1}&mobiles={2}&msgid={3}&filename={4}&senddate={5}&sendtime={6}", uid, this.GetMD5(pwd), reviceMobiles, msgid, this.UrlEncoder(content), senddate, sendtime);


                re = this.SubmitPostByWebRequest(host, data);
                // url = host + data;
                //re = this.Submit(url);
               // log.WriteInfo("SendSms,TSendVoiceFile=>host=" + host + ",data=" + data + ",result=" + re);

            }
            catch (Exception ex)
            {
                //log.WriteLog("SendSms,HttpSms.TSendVoiceFile=>Exception:" + ex.Message + ",url=" + url);

            }
            return re;
        }



        /// <summary>
        /// 发送IVR语音文件
        /// </summary>
        /// <param name="uid">帐号</param>
        /// <param name="pwd">密码</param>
        /// <param name="reviceMobiles">接收号码</param>
        /// <param name="msgid">msgid</param>
        /// <param name="filename">语音文件名</param>       
        /// <param name="timecheck">发送时间段0不限，1 9:00-12:00</param>
        /// <param name="datecheck">timing字段，0不限，1 周一至周五，2周一至周六</param>
        /// <param name="batchnum">批次</param>
        /// <param name="callType">0自动转，1按键转</param>
        /// <returns></returns>
        public string SendVoiceIVRFile(string uid, string pwd, string reviceMobiles, int msgid, string filename, int timecheck,int datecheck, string batchnum,int callType,string speed)
        {
            string re = "";
            string url = "";
            try
            {
                string data = string.Format("cmd=sendvoicecall&uid={0}&psw={1}&mobiles={2}&msgid={3}&filename={4}&timecheck={5}&batchnum={6}&callType={7}&datecheck={8}&speed={9}", uid, this.GetMD5(pwd), reviceMobiles, msgid, this.UrlEncoder(filename), timecheck, batchnum, callType,datecheck,int.Parse(speed));
                re = this.SubmitPostByWebRequest(host, data);
               // log.WriteInfo("SendSms,SendVoiceIVRFile=>host=" + host + ",data=" + data + ",result=" + re);

            }
            catch (Exception ex)
            {
             //   log.WriteLog("SendSms,HttpSms.SendVoiceIVRFile=>Exception:" + ex.Message + ",url=" + url);

            }
            return re;
        }


        /// <summary>
        /// 发送IVR文本
        /// </summary>
        /// <param name="uid">帐号</param>
        /// <param name="pwd">密码</param>
        /// <param name="reviceMobiles">接收号码</param>
        /// <param name="msgid">msgid</param>
        /// <param name="msg">内容</param>
        /// <param name="timecheck">发送时间段0不限，1 9:00-12:00</param>
        /// <param name="datecheck">timing字段，0不限，1 周一至周五，2周一至周六</param>
        /// <param name="batchnum">批次</param>
        /// <param name="callType">0自动转，1按键转</param>
        /// <returns></returns>
        public string SendVoiceIVRText(string uid, string pwd, string reviceMobiles, int msgid, string msg, int timecheck,int datecheck, string batchnum,int callType)
        {
            string re = "";
            string url = "";
            try
            {
                string data = string.Format("cmd=sendvoicecall&uid={0}&psw={1}&mobiles={2}&msgid={3}&msg={4}&timecheck={5}&batchnum={6}&callType={7}&datecheck={8}", uid, this.GetMD5(pwd), reviceMobiles, msgid, this.UrlEncoder(msg), timecheck, batchnum, callType, datecheck);
                re = this.SubmitPostByWebRequest(host, data);
 
                //log.WriteInfo("SendSms,SendVoiceIVRFile=>host=" + host + ",data=" + data + ",result=" + re);

            }
            catch (Exception ex)
            {
               // log.WriteLog("SendSms,HttpSms.SendVoiceIVRText=>Exception:" + ex.Message + ",url=" + url);

            }
            return re;
        }




        /// <summary>
        /// 定时语音
        /// </summary>
        /// <param name="uid">帐号</param>
        /// <param name="pwd">密码</param>
        /// <param name="reviceMobiles">接收号码</param>
        /// <param name="msgid">msgid</param>
        /// <param name="content">内容</param>
        /// <param name="senddate">发送日期yyyy-mm-dd</param>
        /// <param name="sendtime">发送时间HH:MM:SS</param>
        /// <returns></returns>
        public string TSendVoice(string uid, string pwd, string reviceMobiles, int msgid, string content, string senddate, string sendtime)
        {
            string re = "";
            string url="";
            try
            {
                string data = string.Format("cmd=sendvoice&uid={0}&psw={1}&mobiles={2}&msgid={3}&msg={4}&senddate={5}&sendtime={6}", uid, this.GetMD5(pwd), reviceMobiles, msgid, this.UrlEncoder(content), senddate, sendtime);


               // re = this.SubmitPostByWebRequest(posthost, data);
                 url = host + data;
                 re = this.Submit(url);
               //  log.WriteInfo("SendSms,TSendVoice=>host=" + host + ",data=" + data + ",result=" + re);

            }
            catch (Exception ex)
            {
              //  log.WriteLog("SendSms,HttpSms.SendVoice=>Exception:" + ex.Message + ",url=" + url);

            }
            return re;
        }




        /// <summary>
        /// 发短信
        /// </summary>
        /// <param name="uid">帐号</param>
        /// <param name="pwd">密码</param>
        /// <param name="reviceMobiles">接收号码以“,”号分隔</param>
        /// <param name="msgid">消息编号</param>
        /// <param name="content">短信内容</param>       
        /// <returns></returns>
        public string Send(string uid, string pwd, string reviceMobiles, int msgid, string content)
        {
            string re = "";
            string data="";
            try
            {

               // string url = host;
                data =string.Format("cmd=send&uid={0}&psw={1}&mobiles={2}&msgid={3}&msg={4}", uid, GetMD5(pwd), reviceMobiles, msgid, this.UrlEncoder(content));
               re = this.SubmitPostByWebRequest(host, data);
                
             // re = this.Submit(url);
              //  log.WriteInfo("SendSms,Send=>host=" + host +",data="+data+ ",result=" + re);
               
            }
            catch (Exception ex)
            {
              //  log.WriteLog("SendSms,HttpSms.Send=>Exception:" + ex.Message + ",url=" + data);
                 
            }
            return re;
        }

        /// <summary>
        /// 定时短信
        /// </summary>
        /// <param name="uid">帐号</param>
        /// <param name="pwd">密码</param>
        /// <param name="reviceMobiles">接收号码</param>
        /// <param name="msgid">msgid</param>
        /// <param name="content">内容</param>
        /// <param name="senddate">发送日期yyyy-mm-dd</param>
        /// <param name="sendtime">发送时间HH:MM:SS</param>
        /// <returns></returns>
        public string SendTimer(string uid, string pwd, string reviceMobiles, int msgid, string content, string senddate, string sendtime)
        {
            string re = "";
            string url = "";
            try
            {

                string data = string.Format("cmd=tsend&uid={0}&psw={1}&mobiles={2}&msgid={3}&msg={4}&senddate={5}&sendtime={6}", uid, this.GetMD5(pwd), reviceMobiles, msgid, this.UrlEncoder(content), senddate, sendtime);
                 re = SubmitPostByWebRequest(host, data);
                 // url = host + data;
                 //re = this.Submit(url);

                // log.WriteInfo("SendSms,SendTimer=>host=" +host+",data="+ data + ",result=" + re);
               
            }
            catch (Exception ex)
            {
               // log.WriteLog("SendSms,HttpSms.SendTimer=>Exception:" + ex.Message + ",url=" + url);
            }
            return re;
        }

        /// <summary>
        /// 接收短信
        /// </summary>
        /// <returns></returns>
        public string RevSMS(string uid, string pwd)
        {
            string revsms = "";
            string url = "";
            try
            {
                url = string.Format("{0}cmd=getmo&uid={1}&psw={2}", host, uid, GetMD5(pwd));
                revsms = Submit(url);
            }
            catch (Exception ex)
            {
               // log.WriteLog("SendSms,HttpSms.RevSMS=>Exception:" + ex.Message + "url=" + url);
            }
            return revsms;

        }

        /// <summary>
        /// 取条数
        /// </summary>
        /// <param name="uid">帐号</param>
        /// <param name="pwd">密码</param>
        /// <returns>100 # 已发送条数 # 还可发送条数</returns>
        public string GetNum(string uid, string pwd)
        {
            string re = "";
            string data="";
            try
            {
                //data = string.Format("{0}cmd=getnum&uid={1}&psw={2}", host, uid.Trim(), GetMD5(pwd));
                data = string.Format("cmd=getnum&uid={0}&psw={1}",uid.Trim(), GetMD5(pwd));
                this.SubmitPostByWebRequest(host,data);
                re = Submit(data);
                //log.WriteInfo("SendSms,HttpSms.GetNum=>host=" +host+",data="+ data + ",result=" + re);
            }
            catch (Exception ex)
            {
              //  log.WriteLog("SendSms,HttpSms.GetNum=>Exception:" + ex.Message + "url=" + data);
            }
            return re;
        }



        /// <summary>
        /// 获取发送状态
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public string GetStatus(string uid, string pwd)
        {
            string re = "";
            string url = "";
            try
            {
                 url = string.Format("{0}cmd=getstatus&uid={1}&psw={2}", host, uid, GetMD5(pwd));

                re = Submit(url);
                //log.WriteInfo("SendSms,HttpSms.GetStatus=>url=" + url + ",result=" + re);
            }
            catch (Exception ex)
            {
              //  log.WriteLog("SendSms,HttpSms.GetStatus=>Exception:" + ex.Message + "url=" + url);
            }
            return re;
        }

        /// <summary>
        /// GET提交
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string Submit(string url)
        {
            string re = "-1";
            try
            {

                WebRequest request = HttpWebRequest.Create(url);//请求对像
                request.Timeout = 20000;
                request.Method = "GET";
               //request.Headers.Add("Host","http://kltx.sms10000.com.cn");
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;//响应对像
                Stream stream = response.GetResponseStream();//流对像
                StreamReader reader = new StreamReader(stream, Encoding.GetEncoding("GBK"));//读流对像
                re = reader.ReadToEnd();//读流
                reader.Close();
            }
            catch (Exception ex)
            {
                //log.WriteLog("SendSms,HttpSms.Submit=>Exception:"+ex.Message+"url=" + url);
            }
            return re;
        }

        /// <summary>
        /// post提交
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postvalues"></param>
        /// <returns></returns>
        public string SubmitPostByWebRequest(string url, string postvalues)
        {
            string stringResponse = string.Empty;
            try
            {
                string _method = "post";

                string sUserAgent = "Mozilla/4.0(compatible;MSIE 7.0;Windows NT 5.2;.NET CLR 1.1.4322;.NET CLR 2.0.50727)";
                string sContentType = "application/x-www-form-urlencoded";
                //string sRequestEncoding = "ascii";
                //string sRequestEncoding = "GBK";

                byte[] data = Encoding.GetEncoding("GBK").GetBytes(postvalues);


                WebRequest webRequest = WebRequest.Create(url);
                HttpWebRequest httpRequest = webRequest as HttpWebRequest;
                if (httpRequest == null)
                {
                    return "-1";
                }

                httpRequest.UserAgent = sUserAgent;
                if (_method.ToLower() == "post")
                {
                    httpRequest.ContentType = sContentType;
                    httpRequest.Accept = "*/*";
                    httpRequest.Method = _method;
                    httpRequest.ContentLength = data.Length;
                    httpRequest.KeepAlive = false;
                   // httpRequest.Timeout = 2000;
                    httpRequest.ServicePoint.Expect100Continue = false;//关闭Expect:100-Continue握手
                    System.Net.ServicePointManager.DefaultConnectionLimit = 200;
                    Stream requestStream = httpRequest.GetRequestStream();
                    requestStream.Write(data, 0, data.Length);
                    requestStream.Flush();
                    requestStream.Close();

                }
                else
                {
                    httpRequest.Method = _method;
                }
                Stream responseStream;
                try
                {
                    HttpWebResponse response = (HttpWebResponse)httpRequest.GetResponse();
                    Console.WriteLine("status:" + response.StatusCode);
                    responseStream = response.GetResponseStream();
                }
                catch (WebException e)
                {
                    //   throw new ApplicationException(string.Format("POST操作发生异常: {0}", e.Message));
                    return "-1";
                }



                using (StreamReader responseReader = new StreamReader(responseStream, Encoding.GetEncoding("GBK")))
                {
                    stringResponse = responseReader.ReadToEnd();
                }

                responseStream.Close();


            }
            catch (Exception ex)
            {
                stringResponse = "-1";
            }
            finally
            {
                GC.Collect();
            }

            return stringResponse;

        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="str">要加密的字符串</param>
        /// <returns></returns>
        private string GetMD5(string str)
        {
            MD5 md5 = MD5.Create();
            byte[] data = md5.ComputeHash(Encoding.Default.GetBytes(str));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString("x2").ToUpper());
            }
            return sb.ToString(); ;
        }


        /// <summary>
        /// 中文编码 HttpUtility
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string UrlEncoder(string content)
        {
            string re = "";
            if (!string.IsNullOrEmpty(content))
            {
                re = System.Web.HttpUtility.UrlEncode(content, Encoding.GetEncoding("GBK"));
            }
            return re;

        }

    }//end
}//end
