using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Text.Diff;
using System.Collections;
using System.Text;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Collections;
using Sitecore.Data.Fields;
using Sitecore.Globalization;
using System.Web.UI;
using Sitecore.Web.UI.WebControls;
using System.Web.UI.WebControls;
using Sitecore.Resources;


namespace Sitecore.Support.Text.Diff.View
{
  public class OneColumnDiffView : global::Sitecore.Text.Diff.View.OneColumnDiffView
  {

    public override int Compare(System.Web.UI.Control parent, Item item1, Item item2, string click)
    {
      Assert.ArgumentNotNull(parent, "parent");
      Assert.ArgumentNotNull(item1, "item1");
      Assert.ArgumentNotNull(item2, "item2");
      Assert.ArgumentNotNull(click, "click");
      int num = 0;
      Section section = null;
      Item item3 = item1.Versions.GetLatestVersion(Context.Language) ?? item1;
      FieldCollection fields = item1.Fields;
      fields.ReadAll();
      fields.Sort();
      string a = null;
      foreach (Field field in fields)
      {
        Field field2 = item3.Fields[field.Name];
        if (this.ShowField(field))
        {
          if (a != field.Section)
          {
            section = new Section
            {
              Class = "scSection"
            };
            parent.Controls.Add(section);
            section.Header = field.SectionDisplayName;
            section.ID = Sitecore.Web.UI.HtmlControls.Control.GetUniqueID("S");
            a = field.Section;
          }
          string value = base.GetValue(item1[field.Name]);
          string value2 = base.GetValue(item2[field.Name]);
          string @class = (value == value2) ? "scUnchangedFieldLabel" : "scChangedFieldLabel";
          string text;
          if (field.IsBlobField)
          {
            if (value != value2)
            {
              text = "<span style=\"color:#DC291E\">" + Translate.Text("Changed") + "</span>";
            }
            else
            {
              text = Translate.Text("Unable to compare binary fields.");
            }
          }
          else
          {
            text = this.Compare(value, value2);
          }
          text = ((text.Length == 0) ? "&#160;" : text);
          Border border = new Border();
          section.Controls.Add(border);
          border.Class = @class;
          border.Controls.Add(new LiteralControl(field2.DisplayName + ":"));
          GridPanel gridPanel = new GridPanel();
          section.Controls.Add(gridPanel);
          gridPanel.Columns = 2;
          gridPanel.Width = new Unit(100.0, UnitType.Percentage);
          border = new Border();
          gridPanel.Controls.Add(border);
          border.Class = "scField";
          if (field.Type == "checkbox")
          {
            text = "<input type=\"checkbox\" disabled" + ((text == "1") ? " checked" : string.Empty) + " />";
          }
          border.Controls.Add(new LiteralControl(text));
          Border border2 = new Border();
          gridPanel.Controls.Add(border2);
          if (click.Length > 0)
          {
            gridPanel.SetExtensibleProperty(border2, "width", "16px");
            gridPanel.SetExtensibleProperty(border2, "valign", "top");
            border2.ToolTip = Translate.Text("Translate");
            border2.Click = string.Concat(new object[]
            {
                            click,
                            "(\"",
                            field.ID,
                            "\")"
            });
            ImageBuilder imageBuilder = new ImageBuilder
            {
              Src = "Office/16x16/arrow_right.png",
              Width = 16,
              Height = 16,
              Margin = "4px 0px 0px 0px",
              Alt = border2.ToolTip
            };
            border2.Controls.Add(new LiteralControl(imageBuilder.ToString()));
          }
          num++;
        }
      }
      return num;
    }


    protected string Compare(string value1, string value2)
    {
      DiffEngine diffEngine = new DiffEngine();

      #region Modified code
      // Remove "StringUtil.RemoveTags()" and encode the html to output XML content in the coloumn view. 
      value1 = System.Net.WebUtility.HtmlEncode(value1);
      value2 = System.Net.WebUtility.HtmlEncode(value2);
      #endregion

      DiffListHtml source = new DiffListHtml(value1);
      DiffListHtml destination = new DiffListHtml(value2);
      diffEngine.ProcessDiff(source, destination, DiffEngineLevel.SlowPerfect);
      ArrayList arrayList = diffEngine.DiffReport();
      StringBuilder stringBuilder = new StringBuilder();
      for (int i = 0; i < arrayList.Count; i++)
      {
        DiffResultSpan diffResultSpan = arrayList[i] as DiffResultSpan;
        if (diffResultSpan != null)
        {
          switch (diffResultSpan.Status)
          {
            case DiffResultSpanStatus.NoChange:
              base.Append(stringBuilder, value1, diffResultSpan.SourceIndex, diffResultSpan.Length);
              break;
            case DiffResultSpanStatus.Replace:
              base.Append(stringBuilder, value1, diffResultSpan.SourceIndex, diffResultSpan.Length, "green");
              base.Append(stringBuilder, value2, diffResultSpan.DestIndex, diffResultSpan.Length, "red; text-decoration:line-through;font-weight:600");
              break;
            case DiffResultSpanStatus.DeleteSource:
              base.Append(stringBuilder, value1, diffResultSpan.SourceIndex, diffResultSpan.Length, "#2694c0;font-weight:600;background-color:#d0ebf6;padding:2px");
              break;
            case DiffResultSpanStatus.AddDestination:
              base.Append(stringBuilder, value2, diffResultSpan.DestIndex, diffResultSpan.Length, "red; text-decoration:line-through;font-weight:600");
              break;
          }
        }
      }
      return stringBuilder.ToString();
    }

  }
}