﻿using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LindyCircle.Pages
{
    public partial class practices : Page
    {
        protected void Page_Load(object sender, EventArgs e) {

        }

        protected void gvPractices_DataBound(object sender, EventArgs e) {
            lblMessage.Text = string.Empty;
            if (gvPractices.Rows.Count > 0) {
                var rentalTotal = 0M;
                var attendanceRevenueTotal = 0M;
                var miscExpenseTotal = 0M;
                var miscRevenueTotal = 0M;
                var totalAttendance = 0;
                foreach (GridViewRow row in gvPractices.Rows) {
                    if (row.RowType == DataControlRowType.DataRow) {
                        rentalTotal += decimal.Parse(row.Cells[4].Text);
                        attendanceRevenueTotal += decimal.Parse(row.Cells[5].Text);
                        miscExpenseTotal += decimal.Parse(row.Cells[6].Text);
                        miscRevenueTotal += decimal.Parse(row.Cells[7].Text);
                        totalAttendance += int.Parse(row.Cells[8].Text);
                    }
                }
                gvPractices.FooterRow.Cells[4].Text = rentalTotal.ToString("#,##0.00");
                gvPractices.FooterRow.Cells[5].Text = attendanceRevenueTotal.ToString("#,##0.00");
                gvPractices.FooterRow.Cells[6].Text = miscExpenseTotal.ToString("#,##0.00");
                gvPractices.FooterRow.Cells[7].Text = miscRevenueTotal.ToString("#,##0.00");
                gvPractices.FooterRow.Cells[8].Text = totalAttendance.ToString();
            }
        }

        protected void btnClear_Click(object sender, EventArgs e) {
            lblMessage.Text = string.Empty;
            txtTopic.Text = string.Empty;
            txtStartDate.Text = string.Empty;
            txtEndDate.Text = string.Empty;
        }

        protected void gvPractices_RowDeleting(object sender, GridViewDeleteEventArgs e) {
            var practiceID = (int)e.Keys[0];
            using (var db = new LindyCircleContext()) {
                var practice = db.Practices.Single(t => t.PracticeID == practiceID);
                if (practice.Attendances.Count > 0) {
                    lblMessage.Text = "This practice has attendees and cannot be deleted.";
                    e.Cancel = true;
                }
            }
        }
    }
}