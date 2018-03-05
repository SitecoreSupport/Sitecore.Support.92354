
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.shell.Applications.Dialogs.Diff;
using Sitecore.Data;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Text.Diff.View;
using Sitecore.Web.UI.Sheer;
using Sitecore.Web.UI.WebControls;
using System;
using Sitecore;
using Sitecore.Web;

namespace Sitecore.Support.shell.Applications.Dialogs.Diff

{

  public class DiffForm : BaseForm
  {
    /// <summary></summary>
    protected Button Cancel;

    /// <summary></summary>
    protected GridPanel Grid;

    /// <summary></summary>
    protected Button OK;

    /// <summary></summary>
    protected Combobox Version1;

    /// <summary></summary>
    protected Combobox Version2;

    /// <summary>
    /// Raises the load event.
    /// </summary>
    /// <param name="e">The <see cref="T:System.EventArgs" /> instance containing the event data.</param>
    /// <remarks>
    /// This method notifies the server control that it should perform actions common to each HTTP
    /// request for the page it is associated with, such as setting up a database query. At this
    /// stage in the page lifecycle, server controls in the hierarchy are created and initialized,
    /// view state is restored, and form controls reflect client-side data. Use the IsPostBack
    /// property to determine whether the page is being loaded in response to a client postback,
    /// or if it is being loaded and accessed for the first time.
    /// </remarks>
    protected override void OnLoad(EventArgs e)
    {
      Assert.ArgumentNotNull(e, "e");
      base.OnLoad(e);
      if (!Context.ClientPage.IsEvent)
      {
        Context.ClientPage.ServerProperties["id"] = WebUtil.GetQueryString("id");
        Context.ClientPage.ServerProperties["language"] = WebUtil.GetQueryString("la", Language.Current.Name);
      }
      this.Version1.OnChange += new EventHandler(this.OnUpdate);
      this.Version2.OnChange += new EventHandler(this.OnUpdate);
      this.OK.OnClick += new EventHandler(DiffForm.OnOK);
      this.Cancel.OnClick += new EventHandler(DiffForm.OnCancel);
    }

    /// <summary>
    /// Raises the pre render event.
    /// </summary>
    /// <param name="e">The <see cref="T:System.EventArgs" /> instance containing the event data.</param>
    /// <remarks>
    /// This method notifies the server control to perform any necessary prerendering steps prior to
    /// saving view state and rendering content.
    /// </remarks>
    protected override void OnPreRender(EventArgs e)
    {
      Assert.ArgumentNotNull(e, "e");
      base.OnPreRender(e);
      if (Context.ClientPage.IsEvent)
      {
        return;
      }
      string queryString = WebUtil.GetQueryString("id");
      string queryString2 = WebUtil.GetQueryString("la", Language.Current.Name);
      string queryString3 = WebUtil.GetQueryString("vs", "0");
      Item item = Context.ContentDatabase.GetItem(queryString, Language.Parse(queryString2), Sitecore.Data.Version.Parse(queryString3));
      if (item != null)
      {
        Sitecore.Data.Version[] versionNumbers = item.Versions.GetVersionNumbers();
        int number = item.Version.Number;
        int number2 = Sitecore.Data.Version.Invalid.Number;
        if (WebUtil.GetQueryString("wb") == "1")
        {
          for (int i = 1; i < versionNumbers.Length; i++)
          {
            if (versionNumbers[i].Number == number)
            {
              number2 = versionNumbers[i - 1].Number;
              break;
            }
          }
        }
        for (int j = versionNumbers.Length - 1; j >= 0; j--)
        {
          Sitecore.Data.Version version = versionNumbers[j];
          ListItem listItem = new ListItem();
          this.Version1.Controls.Add(listItem);
          listItem.ID = Control.GetUniqueID("ListItem");
          listItem.Header = version.Number.ToString();
          listItem.Value = version.Number.ToString();
          if (version.Number == number)
          {
            listItem.Selected = true;
          }
          listItem = new ListItem();
          this.Version2.Controls.Add(listItem);
          listItem.ID = Control.GetUniqueID("ListItem");
          listItem.Header = version.Number.ToString();
          listItem.Value = version.Number.ToString();
          if (version.Number == number2)
          {
            listItem.Selected = true;
          }
        }
      }
      ListItem selectedItem = this.Version1.SelectedItem;
      ListItem selectedItem2 = this.Version2.SelectedItem;
      if (selectedItem != null && selectedItem2 != null)
      {
        this.Compare(selectedItem.Value, selectedItem2.Value);
      }
      DiffForm.UpdateButtons();
    }

    /// <summary>
    /// Shows the one column.
    /// </summary>
    protected void ShowOneColumn()
    {
      Registry.SetString("/Current_User/Diff/View", "OneColumn");
      DiffForm.UpdateButtons();
      this.Refresh();
    }

    /// <summary>
    /// Shows the two columns.
    /// </summary>
    protected void ShowTwoColumns()
    {
      Registry.SetString("/Current_User/Diff/View", "TwoColumn");
      DiffForm.UpdateButtons();
      this.Refresh();
    }

    /// <summary>
    /// Compares the specified version1.
    /// </summary>
    /// <param name="version1">The version1.</param>
    /// <param name="version2">The version2.</param>
    private void Compare(string version1, string version2)
    {
      Assert.ArgumentNotNull(version1, "version1");
      Assert.ArgumentNotNull(version2, "version2");
      string itemPath = Context.ClientPage.ServerProperties["id"] as string;
      string name = Context.ClientPage.ServerProperties["language"] as string;
      Item item = Context.ContentDatabase.Items[itemPath, Language.Parse(name), Sitecore.Data.Version.Parse(version1)];
      Item item2 = Context.ContentDatabase.Items[itemPath, Language.Parse(name), Sitecore.Data.Version.Parse(version2)];
      string @string = Registry.GetString("/Current_User/Diff/View", "OneColumn");
      DiffView diffView;
      if (@string == "OneColumn")
      {
        diffView = new OneColumnDiffView();
      }
      else
      {
        diffView = new TwoCoumnsDiffView();
      }
      diffView.Compare(this.Grid, item, item2, string.Empty);
    }

    /// <summary>
    /// Called when this instance has cancel.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="T:System.EventArgs" /> instance containing the event data.</param>
    private static void OnCancel(object sender, EventArgs e)
    {
      Assert.ArgumentNotNull(sender, "sender");
      Assert.ArgumentNotNull(e, "e");
      Context.ClientPage.ClientResponse.CloseWindow();
    }

    /// <summary>
    /// Called when this instance has OK.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="T:System.EventArgs" /> instance containing the event data.</param>
    private static void OnOK(object sender, EventArgs e)
    {
      Assert.ArgumentNotNull(sender, "sender");
      Assert.ArgumentNotNull(e, "e");
      Context.ClientPage.ClientResponse.CloseWindow();
    }

    /// <summary>
    /// Called when this instance has update.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="T:System.EventArgs" /> instance containing the event data.</param>
    private void OnUpdate(object sender, EventArgs e)
    {
      Assert.ArgumentNotNull(sender, "sender");
      Assert.ArgumentNotNull(e, "e");
      this.Refresh();
    }

    /// <summary>
    /// Refreshes this instance.
    /// </summary>
    private void Refresh()
    {
      ListItem selectedItem = this.Version1.SelectedItem;
      ListItem selectedItem2 = this.Version2.SelectedItem;
      if (selectedItem != null && selectedItem2 != null)
      {
        this.Grid.Controls.Clear();
        this.Compare(selectedItem.Value, selectedItem2.Value);
        Context.ClientPage.ClientResponse.SetOuterHtml("Grid", this.Grid);
      }
    }

    /// <summary>
    /// Updates the buttons.
    /// </summary>
    private static void UpdateButtons()
    {
      string @string = Registry.GetString("/Current_User/Diff/View", "OneColumn");
      Toolbutton toolbutton = Context.ClientPage.FindControl("OneColumn") as Toolbutton;
      Assert.IsNotNull(toolbutton, typeof(Toolbutton));
      Toolbutton toolbutton2 = Context.ClientPage.FindControl("TwoColumn") as Toolbutton;
      Assert.IsNotNull(toolbutton2, typeof(Toolbutton));
      toolbutton.Down = (@string == "OneColumn");
      toolbutton2.Down = !toolbutton.Down;
    }
  }
}