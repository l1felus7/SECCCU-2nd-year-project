﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SECCCU
{
    public partial class Form1 : Form
    {
        MonitorSystem monitorSystem = new MonitorSystem();

        public Form1()
        {
            InitializeComponent();
            uiProgrammeComboBox.Items.Add("*");

            foreach (string programmeTitle in monitorSystem.Database.GetProgrammeTitles())
            {
                uiProgrammeComboBox.Items.Add(programmeTitle);
                uiProgrammePicker.Items.Add(programmeTitle);
            }
        }


        private void uiScanCardButton_click(object sender, EventArgs e)
        {
            Report textReport = new Report();
            uiScanCardButton.Enabled = false;
            Reader reader = new Reader();
            string cardNumber = reader.ScanCard();
            uiCardNumberLabel.Text = cardNumber;

            string[] dataFromDatabase = monitorSystem.Database.LogCardSwipe(cardNumber);
            MessageBox.Show(dataFromDatabase[0], dataFromDatabase[1]);
            if (dataFromDatabase[2] != null)
            {
                textReport.SendSignInText(dataFromDatabase[2], dataFromDatabase[0]);
            }

            uiScanCardButton.Enabled = true;
        }

        private void uiCheckLoginStatusButton_Click(object sender, EventArgs e)
        {
            if (uiStudentIDTextBox.Text != "")
            {
                string userID = uiStudentIDTextBox.Text.ToUpper();
                string[] responseFromDatabase = monitorSystem.Database.DidUserSwipeInCurrentLecture(userID);
                if (responseFromDatabase[0] == "")
                {
                    MessageBox.Show(
                        $"{uiStudentIDTextBox.Text} is not a valid student id", "Error");
                }
                else if (responseFromDatabase[2] == "")
                {
                    MessageBox.Show(
                        $"{responseFromDatabase[0]} {responseFromDatabase[1]} is not marked as attended", "Log In Status");
                }
                else
                {
                    MessageBox.Show($"{responseFromDatabase[0]} {responseFromDatabase[1]} is marked as attended in {responseFromDatabase[2]}", "Log In Status");

                }
            }
        }

        private void uiProgrammeComboBox_indexChanged(object sender, EventArgs e)
        {
            uiModuleComboBox.Items.Clear();
            uiModuleComboBox.Items.Add("*");
            foreach (string module in monitorSystem.Database.GetModules(uiProgrammeComboBox.Text))
            {
                uiModuleComboBox.Items.Add(module);
            }
        }

        private void uiEmailReportButton_Click(object sender, EventArgs e)
        {
            string email = uiEmailTextBox.Text;
            if (RegexUtils.IsValidEmail(email))
            {
                Report report = new Report();

                if (report.SendReport(email))
                {
                    MessageBox.Show("E-mail sent", "Success");
                }
                else
                {
                    MessageBox.Show("E-mail not sent", "Error");
                }
            }
            else
            {
                MessageBox.Show("Invalid e-mail address", "Error");
            }
        }

        private void uiCreateReportButton_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            foreach (string singleLog in monitorSystem.Database.GetReport(uiProgrammeComboBox.Text, uiModuleComboBox.Text, uiDateFromPicker.Value.ToString("yyyy-MM-dd"), uiDateToPicker.Value.ToString("yyyy-MM-dd"), uiStudentIDTextBox.Text))
            {
                listBox1.Items.Add(singleLog);
            }

            if (listBox1.Items.Count == 0)
            {
                listBox1.Items.Add("No classes found. Please expand search criteria");
            }
            else
            {
                var report = new Report();
                uiAttendanceAvgLabel.Text = $"Attendance: {report.GetAttendancePercentage()}%";

            }


        }

        private void uiAddStudentButton_Click(object sender, EventArgs e)
        {

            if (uiFirstNameTextBox.Text.Length > 2 && uiSurnameTextBox.Text.Length > 2 &&
                RegexUtils.ValidatePhoneNumber(uiPhoneNumberTextBox.Text) && uiProgrammePicker.Text != "" )
            {
                string studentID = monitorSystem.Database.AddStudent(uiFirstNameTextBox.Text, uiSurnameTextBox.Text,
                    uiPhoneNumberTextBox.Text,
                    uiProgrammePicker.Text);


                if (studentID != "False")
                {
                    MessageBox.Show($"Student Added\nID Number: {studentID} ", "Success");
                    uiFirstNameTextBox.Clear();
                    uiSurnameTextBox.Clear();
                    uiPhoneNumberTextBox.Clear();
                    uiProgrammePicker.SelectedIndex = -1;
                    
                }
                else
                {
                    MessageBox.Show($"Student not added ", "Error");

                }
            }

            else
            {
                MessageBox.Show("Invalid Details. Please try again", "Error");
            }
        }
    }
}