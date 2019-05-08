﻿using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LindyCircle.Pages
{
    public partial class punchcards : Page
    {
        decimal punchCardCost = 0M;
        
        protected void Page_Init(object sender, EventArgs e) {
            using (var db = new LindyCircleContext())
                punchCardCost = db.Defaults.Single(t => t.DefaultName.Equals("Punch card price")).DefaultValue;
        }

        protected void Page_Load(object sender, EventArgs e) {
            txtPurchaseDate.Text = DateTime.Now.ToShortDateString();
            txtAmount.Text = punchCardCost.ToString("0.00");
            RegisterPostBackControls();
        }

        protected void btnPurchase_Click(object sender, EventArgs e) {
            var punchCard = new PunchCard();
            punchCard.MemberID = int.Parse(ddlMembers.SelectedValue);
            punchCard.PurchaseDate = DateTime.Parse(txtPurchaseDate.Text);
            punchCard.PurchaseAmount = decimal.Parse(txtAmount.Text);
            using (var db = new LindyCircleContext()) {
                db.PunchCards.Add(punchCard);
                db.SaveChanges();
                txtPurchaseDate.Text = DateTime.Now.ToShortDateString();
                txtAmount.Text = punchCardCost.ToString("0.00");
                gvPunchCards.DataBind();
            }
        }

        protected void RegisterPostBackControls() {
            foreach (GridViewRow row in gvPunchCards.Rows) {
                LinkButton lb = row.FindControl("btnDelete") as LinkButton;
                ScriptManager.GetCurrent(this).RegisterAsyncPostBackControl(lb);
            }
        }

        protected void btnDelete_Command(object sender, CommandEventArgs e) {
            var punchCardID = int.Parse(e.CommandArgument.ToString());
            using (var db = new LindyCircleContext()) {
                var punchCard = db.PunchCards.FirstOrDefault(t => t.PunchCardID == punchCardID);
                if (punchCard != null) {
                    db.PunchCards.Remove(punchCard);
                    db.SaveChanges();
                    gvPunchCards.DataBind();
                }
            }
        }

        protected void ddlMembers_DataBound(object sender, EventArgs e) {
            gvPunchCards.DataBind();
        }
    }
}