﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LindyCircle.Pages
{
    public partial class attendance : Page
    {
        protected void Page_Load(object sender, EventArgs e) {
            RegisterPostBackControls();
            if (!IsPostBack) {
                ddlPaymentTypes.Items.Add(new ListItem("None", "0"));
                ddlPaymentTypes.Items.Add(new ListItem("Door price", "1"));
                ddlPaymentTypes.Items.Add(new ListItem("Punch card", "2"));
                ddlPaymentTypes.Items.Add(new ListItem("Other", "3"));
                ddlPaymentTypes.SelectedIndex = 2;
                txtPaymentAmount.Text = "0.00";
            }
        }

        protected void txtPracticeDate_TextChanged(object sender, EventArgs e) {
            lblSaveStatus.Text = string.Empty;
            DateTime practiceDate;
            if (DateTime.TryParse(txtPracticeDate.Text, out practiceDate)) {
                calPracticeDate.SelectedDate = practiceDate;
                GetPracticeID(practiceDate);
            }
        }

        protected void calPracticeDate_SelectionChanged(object sender, EventArgs e) {
            lblSaveStatus.Text = string.Empty;
            txtPracticeDate.Text = calPracticeDate.SelectedDate.ToShortDateString();
            GetPracticeID(calPracticeDate.SelectedDate);
        }

        protected void GetPracticeID(DateTime practiceDate) {
            using (var db = new LindyCircleContext()) {
                var practice = db.Practices.FirstOrDefault(t => t.PracticeDate == practiceDate);
                if (practice != null) {
                    txtPracticeID.Text = practice.PracticeID.ToString();
                    txtRentalCost.Text = practice.PracticeCost.ToString("0.00");
                    txtMiscExpense.Text = practice.MiscExpense.ToString("0.00");
                    txtMiscRevenue.Text = practice.MiscRevenue.ToString("0.00");
                    panNewPractice.Visible = false;
                    panPracticeDetails.Visible = true;
                }
                else {
                    panPracticeDetails.Visible = false;
                    lblNewPractice.Text = "Would you like to add a new practice dated " + calPracticeDate.SelectedDate.ToShortDateString() + "?";
                    txtNewRentalCost.Text = db.Defaults.Single(t => t.DefaultName.Equals("Rental Cost")).DefaultValue.ToString("0.00");
                    txtNewMiscExpense.Text = "0.00";
                    txtNewMiscRevenue.Text = "0.00";
                    panNewPractice.Visible = true;
                }
            }
        }

        protected void ddlPaymentTypes_SelectedIndexChanged(object sender, EventArgs e) {
            SetDefaultPaymentAmount();
        }

        protected void btnYes_Click(object sender, EventArgs e) {
            using (var db = new LindyCircleContext()) {
                var nextPracticeNumber = db.Practices.Max(t => t.PracticeNumber) + 1;
                var newPractice = new Practice();
                newPractice.PracticeNumber = nextPracticeNumber;
                newPractice.PracticeDate = calPracticeDate.SelectedDate;
                if (string.IsNullOrEmpty(txtNewRentalCost.Text)) newPractice.PracticeCost = 0M;
                else newPractice.PracticeCost = decimal.Parse(txtNewRentalCost.Text);
                if (string.IsNullOrEmpty(txtNewMiscExpense.Text)) newPractice.MiscExpense = 0M;
                else newPractice.MiscExpense = decimal.Parse(txtNewMiscExpense.Text);
                if (string.IsNullOrEmpty(txtNewMiscRevenue.Text)) newPractice.MiscRevenue = 0M;
                else newPractice.MiscRevenue = decimal.Parse(txtNewMiscRevenue.Text);
                db.Practices.Add(newPractice);
                db.SaveChanges();
                txtPracticeID.Text = db.Practices.Max(t => t.PracticeID).ToString();
                panNewPractice.Visible = false;
                txtRentalCost.Text = newPractice.PracticeCost.ToString("#,##0.00");
                txtMiscExpense.Text = newPractice.MiscExpense.ToString("#,##0.00");
                txtMiscRevenue.Text = newPractice.MiscRevenue.ToString("#,##0.00");
                panPracticeDetails.Visible = true;
            }
        }

        protected void btnNo_Click(object sender, EventArgs e) {
            panNewPractice.Visible = false;
            txtPracticeDate.Text = string.Empty;
        }

        protected void ddlMembers_SelectedIndexChanged(object sender, EventArgs e) {
            SetDefaultPaymentType();
        }

        protected void ddlMembers_DataBound(object sender, EventArgs e) {
            SetDefaultPaymentType();
        }

        protected void SetDefaultPaymentType() {
            var selectedMemberID = int.Parse(ddlMembers.SelectedValue);
            using (var db = new LindyCircleContext()) {
                var unusedPunches = db.Members.First(t => t.MemberID == selectedMemberID).RemainingPunches;
                lblUnusedPunches.Text = unusedPunches.ToString();
                if (unusedPunches >= 1)
                    ddlPaymentTypes.SelectedIndex = 2;
                else ddlPaymentTypes.SelectedIndex = 1;
            }
            SetDefaultPaymentAmount();
        }

        protected void SetDefaultPaymentAmount() {
            if (ddlPaymentTypes.SelectedValue.Equals("1")) {
                using (var db = new LindyCircleContext())
                    txtPaymentAmount.Text = db.Defaults.Single(t => t.DefaultName.Equals("Door price")).DefaultValue.ToString("0.00");
            }
            else txtPaymentAmount.Text = "0.00";
        }

        protected void btnAddToPractice_Click(object sender, EventArgs e) {
            var attendance = new Attendance();
            attendance.MemberID = int.Parse(ddlMembers.SelectedValue);
            attendance.PracticeID = int.Parse(txtPracticeID.Text);
            attendance.PaymentType = int.Parse(ddlPaymentTypes.SelectedValue);
            if (string.IsNullOrEmpty(txtPaymentAmount.Text)) attendance.PaymentAmount = 0M;
            else attendance.PaymentAmount = decimal.Parse(txtPaymentAmount.Text);
            using (var db = new LindyCircleContext()) {
                db.Attendances.Add(attendance);
                db.SaveChanges();
                gvAttendance.DataBind();
                ddlMembers.DataBind();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e) {
            using (var db = new LindyCircleContext()) {
                var practiceID = int.Parse(txtPracticeID.Text);
                var practice = db.Practices.First(t => t.PracticeID == practiceID);
                if (string.IsNullOrEmpty(txtRentalCost.Text)) practice.PracticeCost = 0M;
                else practice.PracticeCost = decimal.Parse(txtRentalCost.Text);
                if (string.IsNullOrEmpty(txtMiscExpense.Text)) practice.MiscExpense = 0M;
                else practice.MiscExpense = decimal.Parse(txtMiscExpense.Text);
                if (string.IsNullOrEmpty(txtMiscRevenue.Text)) practice.MiscRevenue = 0M;
                else practice.MiscRevenue = decimal.Parse(txtMiscRevenue.Text);
                db.SaveChanges();
                lblSaveStatus.Text = "Practice expenses & revenues updated.";
            }
        }

        protected void btnRemove_Command(object sender, CommandEventArgs e) {
            var attendanceID = int.Parse(e.CommandArgument.ToString());
            using (var db = new LindyCircleContext()) {
                var attendance = db.Attendances.FirstOrDefault(t => t.AttendanceID == attendanceID);
                if (attendance != null) {
                    db.Attendances.Remove(attendance);
                    db.SaveChanges();
                    gvAttendance.DataBind();
                    ddlMembers.DataBind();
                }
            }
        }

        protected void RegisterPostBackControls() {
            foreach (GridViewRow row in gvAttendance.Rows) {
                LinkButton lb = row.FindControl("btnRemove") as LinkButton;
                ScriptManager.GetCurrent(this).RegisterAsyncPostBackControl(lb);
            }
        }
    }
}