﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using model;
using Quartz;
using bll;

namespace Business_Bill
{
    public partial class FmMain : Form
    {
        public FmMain()
        {
            InitializeComponent();
        }

        LocalParams lp;
        List<IScheduler> schList = null;
        private void FmMain_Load(object sender, EventArgs e)
        {
            schList = new List<IScheduler>();
            MyDelegateFunc.WriteFmLogEvent = this.WriteLog;
            this.timer1.Enabled = true;
            log4net.Config.XmlConfigurator.Configure();
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="txt"></param>
        private void WriteLog(string txt)
        {
            try
            {
                if (rtbLog.InvokeRequired)
                {

                    rtbLog.Invoke(new Action<string>(x => WriteLog(x)), new object[] { txt });

                }
                else
                {
                    if (rtbLog.TextLength > 214748364)
                    {
                        rtbLog.Clear();
                    }
                    rtbLog.AppendText("[" + DateTime.Now.ToString() + "]==>" + txt + Environment.NewLine);
                    rtbLog.SelectionStart = rtbLog.Text.Length;
                    rtbLog.ScrollToCaret();
                }
            }
            catch (Exception)
            {

            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                this.toolStripStatusLabelTimeNow.Text = DateTime.Now.ToString();
            }
            catch (Exception ex)
            {

            }
        }

        //启动
        private void toolStripBtnStart_Click(object sender, EventArgs e)
        {
            lp = new LocalParams();
            if(lp==null)
            {
                MessageBox.Show("LocalParams为空。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if(string.IsNullOrEmpty(lp.CronExpression1) || string.IsNullOrEmpty(lp.CronExpression2))
            {
                MessageBox.Show("请设置QZ运行表达示。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrEmpty(lp.SqlConnStr))
            {
                MessageBox.Show("请设置数据库连接字符串。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!SQLbll.IsConn(lp.SqlConnStr))
            {
                MessageBox.Show("数据库连接字符串无效。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            CreateJob cjob = new CreateJob();
            if (!cjob.CheckCronExpression(lp.CronExpression1))
            {
                MessageBox.Show(string.Format("表达试[{0}]无效", lp.CronExpression1), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!cjob.CheckCronExpression(lp.CronExpression2))
            {
                MessageBox.Show(string.Format("表达试[{0}]无效", lp.CronExpression2), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            /*
            if (!cjob.CheckCronExpression(lp.CronExpressionTaocan))
            {
                MessageBox.Show(string.Format("表达试[{0}]无效", lp.CronExpressionTaocan), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!cjob.CheckCronExpression(lp.CronExpressionTaocanClose))
            {
                MessageBox.Show(string.Format("表达试[{0}]无效", lp.CronExpressionTaocanClose), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
             * */

            // [秒] [分] [小时] [日] [月] [周] [年],日和周必须互斥,不能都指明特定的数字或*,必须有一个是?; *号就是每的意思;？代表不确定; - 表示区间; ,表示多个值
            // 0 58 9 23 * ?,每年每月23号9点58分0秒执行
            // 0 0/1 * * * ?,第分钟执行一次
            // 0 0 * * * ?,每小时一次
            // 0 15 10 15 * ? 每月15日上午10:15触发   
            // 0 15 10 L * ?  每月最后一天的10点15分触发    
            string cron = "0 28 15 * * ?";
            IScheduler sched = cjob.CreateSched(lp.CronExpression1, "trigger1", "group1", typeof(CreateBillJob));
            if (sched != null)
            {
                schList.Add(sched);
                sched.Start();

            }
            else
            {
                MessageBox.Show(string.Format("表达试[{0}] CreateBillJob初始化失败!", lp.CronExpression1), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            IScheduler sched2 = cjob.CreateSched(lp.CronExpression2, "trigger2", "group2", typeof(CreateEmptyBillJob));
            if (sched2 != null)
            {
                schList.Add(sched2);
                sched2.Start();
            }
            else
            {
                MessageBox.Show("提示", string.Format("表达试[{0}] CreateBillJob初始化失败!", lp.CronExpression2), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            /*
            IScheduler sched3 = cjob.CreateSched(lp.CronExpressionTaocan, "trigger3", "group3", typeof(CreateTaoCanBillJob));
            if (sched3 != null)
            {
                schList.Add(sched3);
                sched3.Start();
            }
            else
            {
                MessageBox.Show("提示", string.Format("表达试[{0}] CreateTaoCanBillJob初始化失败!", lp.CronExpressionTaocan), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
             */

            /*
            IScheduler sched4 = cjob.CreateSched(lp.CronExpressionTaocanClose, "trigger4", "group4", typeof(CreateTaoCanCloseBillJob));
            if (sched4 != null)
            {
                schList.Add(sched4);
                sched4.Start();
            }
            else
            {
                MessageBox.Show("提示", string.Format("表达试[{0}] CreateTaoCanCloseBillJob初始化失败!", lp.CronExpressionTaocanClose), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
           */

            this.toolStripBtnStart.Enabled = false;
            this.toolStripStatusLabel2.Text = DateTime.Now.ToString("yyyy/MM/dd H:mm:ss", System.Globalization.DateTimeFormatInfo.InvariantInfo);// string.Format("{0:yyyy\\/MM\\/dd HH:mm:ss}", DateTime.Now);//2005/11/5 14:23:20 这种格式更适合老外的格式;
        }

        //停止
        private void toolStripBtnStop_Click(object sender, EventArgs e)
        {
            try
            {
                this.toolStripBtnStart.Enabled = true;
                if (schList != null && schList.Count > 0)
                {
                    foreach (IScheduler sch in schList)
                    {
                        sch.Shutdown(true);
                    }
                }
                schList = new List<IScheduler>();
                this.WriteLog("任务停止....");
                this.toolStripStatusLabel1.Text = "已停止";
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        private void toolStripBtnSet_Click(object sender, EventArgs e)
        {
            try
            {
                FmSet fs = new FmSet();
                fs.StartPosition = FormStartPosition.CenterScreen;
                fs.ShowInTaskbar = false;
                fs.ShowDialog();
            }
            catch (Exception ex)
            {

            }
        }

        private void FmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                DialogResult dr = MessageBox.Show("确定要关闭么？", "关闭提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (dr == System.Windows.Forms.DialogResult.OK)
                {
                    if (schList != null && schList.Count > 0)
                    {
                        foreach (IScheduler sch in schList)
                        {
                            try
                            {
                                sch.Shutdown(true);
                            }
                            catch (Exception) { }
                        }
                    }
                }
                else
                {
                    e.Cancel = true;
                }
            }
            catch (Exception)
            {
 
            }
        }

        private void toolStripBtnReprotStatus_Click(object sender, EventArgs e)
        {
            FmSuccessRate fsr = new FmSuccessRate();

            fsr.Show();
        }

        private void toolStripBtnManualCreateBill_Click(object sender, EventArgs e)
        {
            FmManualCreateBill fmcbill = new FmManualCreateBill();
            fmcbill.Show();
        }

        private void toolStripBtnTaocan_Click(object sender, EventArgs e)
        {
            FmManualCreateTaocanBill ftb = new FmManualCreateTaocanBill();
            ftb.Show();
        }




    }//end
}//end
