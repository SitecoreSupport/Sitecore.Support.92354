using Sitecore.Text.Diff;
using System.Collections;
using System.Text;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Collections;
using Sitecore.Data.Fields;
using Sitecore.Globalization;
using System.Web.UI;
using Sitecore.Web.UI.WebControls;
using System.Web.UI.WebControls;



namespace Sitecore.Support.Text.Diff.View
{
  public class TwoCoumnsDiffView : global::Sitecore.Text.Diff.View.TwoCoumnsDiffView
  {

    public override int Compare(System.Web.UI.Control parent, Item item1, Item item2, string click)
    {
      Assert.ArgumentNotNull(parent, "parent");
      Assert.ArgumentNotNull(item1, "item1");
      Assert.ArgumentNotNull(item2, "item2");
      Assert.ArgumentNotNull(click, "click");
      int num = 0;
      Item item3 = item1.Versions.GetLatestVersion(Context.Language) ?? item1;
      FieldCollection fields = item1.Fields;
      fields.ReadAll();
      fields.Sort();
      string a = null;
      GridPanel gridPanel = null;
      foreach (Field field in fields)
      {
        if (this.ShowField(field))
        {
          Field field2 = item3.Fields[field.Name];
          if (a != field.Section)
          {
            Section section = new Section();
            section.Class = "scSection";
            parent.Controls.Add(section);
            section.Header = field.SectionDisplayName;
            section.ID = Sitecore.Web.UI.HtmlControls.Control.GetUniqueID("S");
            gridPanel = new GridPanel();
            section.Controls.Add(gridPanel);
            gridPanel.Columns = 2;
            gridPanel.Width = new Unit(100.0, UnitType.Percentage);
            gridPanel.Fixed = true;
            a = field.Section;
          }
          string text = base.GetValue(item1[field.Name]);
          string text2 = base.GetValue(item2[field.Name]);
          string @class = (text == text2) ? "scUnchangedFieldLabel" : "scChangedFieldLabel";
          if (field.IsBlobField)
          {
            if (text != text2)
            {
              text = "<span style=\"color:blue\">" + Translate.Text("Changed") + "</span>";
              text2 = "<span style=\"color:blue\">" + Translate.Text("Changed") + "</span>";
            }
            else
            {
              text = Translate.Text("Unable to compare binary fields.");
              text2 = Translate.Text("Unable to compare binary fields.");
            }
          }
          else
          {
            this.Compare(ref text, ref text2);
          }
          text = ((text.Length == 0) ? "&#160;" : text);
          text2 = ((text2.Length == 0) ? "&#160;" : text2);
          Border border = new Border();
          gridPanel.Controls.Add(border);
          gridPanel.SetExtensibleProperty(border, "valign", "top");
          gridPanel.SetExtensibleProperty(border, "width", "50%");
          border.Class = @class;
          border.Controls.Add(new LiteralControl(field2.DisplayName + ":"));
          border = new Border();
          gridPanel.Controls.Add(border);
          gridPanel.SetExtensibleProperty(border, "valign", "top");
          gridPanel.SetExtensibleProperty(border, "width", "50%");
          border.Class = @class;
          border.Controls.Add(new LiteralControl(field2.DisplayName + ":"));
          border = new Border();
          gridPanel.Controls.Add(border);
          gridPanel.SetExtensibleProperty(border, "valign", "top");
          border.Class = "scField";
          if (field.Type == "checkbox")
          {
            text = "<input type=\"checkbox\" disabled" + ((text == "1") ? " checked" : string.Empty) + " />";
          }
          border.Controls.Add(new LiteralControl(text));
          border = new Border();
          gridPanel.Controls.Add(border);
          gridPanel.SetExtensibleProperty(border, "valign", "top");
          border.Class = "scField";
          if (field.Type == "checkbox")
          {
            text2 = "<input type=\"checkbox\" disabled" + ((text2 == "1") ? " checked" : string.Empty) + " />";
          }
          border.Controls.Add(new LiteralControl(text2));
          num++;
        }
      }
      return num;
    }

    protected virtual void Compare(ref string value1, ref string value2)
    {

      DiffEngine diffEngine = new DiffEngine();
      value1 = System.Net.WebUtility.HtmlEncode(value1);
      value2 = System.Net.WebUtility.HtmlEncode(value2);
      DiffListHtml source = new DiffListHtml(value1);
      DiffListHtml destination = new DiffListHtml(value2);
      diffEngine.ProcessDiff(source, destination, DiffEngineLevel.SlowPerfect);
      ArrayList arrayList = diffEngine.DiffReport();
      StringBuilder stringBuilder = new StringBuilder();
      StringBuilder stringBuilder2 = new StringBuilder();
      for (int i = 0; i < arrayList.Count; i++)
      {
        DiffResultSpan diffResultSpan = arrayList[i] as DiffResultSpan;
        if (diffResultSpan != null)
        {
          switch (diffResultSpan.Status)
          {
            case DiffResultSpanStatus.NoChange:
              base.Append(stringBuilder, value1, diffResultSpan.SourceIndex, diffResultSpan.Length);
              base.Append(stringBuilder2, value2, diffResultSpan.DestIndex, diffResultSpan.Length);
              break;
            case DiffResultSpanStatus.Replace:
              base.Append(stringBuilder, value1, diffResultSpan.SourceIndex, diffResultSpan.Length, "#2694c0;font-weight:600;background-color:#d0ebf6;padding:2px");
              base.Append(stringBuilder2, value2, diffResultSpan.DestIndex, diffResultSpan.Length, "#2694c0;font-weight:600;background-color:#d0ebf6;padding:2px");
              break;
            case DiffResultSpanStatus.DeleteSource:
              base.Append(stringBuilder, value1, diffResultSpan.SourceIndex, diffResultSpan.Length, "#2694c0;font-weight:600;background-color:#d0ebf6;padding:2px");
              base.Append(stringBuilder2, value2, diffResultSpan.DestIndex, diffResultSpan.Length, "#2694c0;font-weight:600;background-color:#d0ebf6;padding:2px");
              break;
            case DiffResultSpanStatus.AddDestination:
              base.Append(stringBuilder, value1, diffResultSpan.SourceIndex, diffResultSpan.Length, "#2694c0;font-weight:600;background-color:#d0ebf6;padding:2px");
              base.Append(stringBuilder2, value2, diffResultSpan.DestIndex, diffResultSpan.Length, "#2694c0;font-weight:600;background-color:#d0ebf6;padding:2px");
              break;
          }
        }
      }
      value1 = stringBuilder.ToString();
      value2 = stringBuilder2.ToString();
    }


  }
}