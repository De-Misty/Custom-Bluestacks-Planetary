using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000109 RID: 265
	public class ExtensionPopupControl : UserControl, IComponentConnector
	{
		// Token: 0x06000B04 RID: 2820 RVA: 0x00008F64 File Offset: 0x00007164
		public ExtensionPopupControl()
		{
			this.InitializeComponent();
		}

		// Token: 0x1400000A RID: 10
		// (add) Token: 0x06000B05 RID: 2821 RVA: 0x0003E660 File Offset: 0x0003C860
		// (remove) Token: 0x06000B06 RID: 2822 RVA: 0x0003E698 File Offset: 0x0003C898
		public event EventHandler DownloadClicked;

		// Token: 0x06000B07 RID: 2823 RVA: 0x0003E6D0 File Offset: 0x0003C8D0
		internal void LoadExtensionPopupFromFolder(string folderPath)
		{
			if (!Path.IsPathRooted(folderPath))
			{
				folderPath = Path.Combine(CustomPictureBox.AssetsDir, folderPath);
			}
			if (Directory.Exists(folderPath))
			{
				try
				{
					string text = Path.Combine(folderPath, "extensionPopup.json");
					if (File.Exists(text))
					{
						ExtensionPopupControl.ExtensionPopupContext extensionPopupContext = ExtensionPopupControl.ExtensionPopupContext.ReadJson(JObject.Parse(File.ReadAllText(text)));
						this.slideShow.ImagesFolderPath = folderPath;
						this.ApplyContext(extensionPopupContext);
					}
				}
				catch (Exception ex)
				{
					Logger.Error("Error while trying to read extensionpopup.json from " + folderPath + "." + ex.ToString());
				}
			}
		}

		// Token: 0x06000B08 RID: 2824 RVA: 0x0003E764 File Offset: 0x0003C964
		private void ApplyContext(ExtensionPopupControl.ExtensionPopupContext context)
		{
			BlueStacksUIBinding.Bind(this.mTitle, context.Title);
			BlueStacksUIBinding.Bind(this.mSubTitle, context.SubTitle);
			BlueStacksUIBinding.Bind(this.mTagLine, context.TagLine, "");
			BlueStacksUIBinding.Bind(this.mDescription, context.Description, "");
			if (context.features != null && context.features.Any<string>())
			{
				BlueStacksUIBinding.Bind(this.mFeaturesText, context.FeaturesText, "");
				using (IEnumerator<string> enumerator = context.features.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string text = enumerator.Current;
						TextBlock textBlock = new TextBlock
						{
							FontSize = 13.0
						};
						BlueStacksUIBinding.BindColor(textBlock, Control.ForegroundProperty, "SettingsWindowTabMenuItemForeground");
						textBlock.Margin = new Thickness(7.0, 0.0, 0.0, 5.0);
						textBlock.TextWrapping = TextWrapping.Wrap;
						BlueStacksUIBinding.Bind(textBlock, text, "");
						TextBlock textBlock2 = new TextBlock
						{
							Text = "•"
						};
						BlueStacksUIBinding.BindColor(textBlock2, Control.ForegroundProperty, "SettingsWindowTabMenuItemForeground");
						textBlock2.FontSize = 13.0;
						textBlock2.FontWeight = FontWeights.Bold;
						textBlock2.Margin = new Thickness(0.0, 0.0, 0.0, 5.0);
						BulletDecorator bulletDecorator = new BulletDecorator
						{
							Bullet = textBlock2,
							Child = textBlock
						};
						this.mFeaturesStack.Children.Add(bulletDecorator);
					}
					goto IL_01B7;
				}
			}
			this.mFeaturesText.Text = "";
			IL_01B7:
			if (context.ExtensionDetails != null && context.ExtensionDetails.Any<KeyValuePair<string, string>>())
			{
				BlueStacksUIBinding.Bind(this.mDetailsText, context.ExtensionDetailText, "");
				using (IEnumerator<KeyValuePair<string, string>> enumerator2 = context.ExtensionDetails.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						KeyValuePair<string, string> keyValuePair = enumerator2.Current;
						Grid grid = new Grid();
						grid.ColumnDefinitions.Add(new ColumnDefinition
						{
							Width = new GridLength(1.0, GridUnitType.Star)
						});
						grid.ColumnDefinitions.Add(new ColumnDefinition
						{
							Width = new GridLength(1.6, GridUnitType.Star)
						});
						TextBlock textBlock3 = new TextBlock
						{
							FontSize = 13.0
						};
						BlueStacksUIBinding.BindColor(textBlock3, Control.ForegroundProperty, "SettingsWindowTabMenuItemForeground");
						textBlock3.Margin = new Thickness(0.0, 0.0, 0.0, 5.0);
						textBlock3.TextWrapping = TextWrapping.Wrap;
						BlueStacksUIBinding.Bind(textBlock3, keyValuePair.Key, "");
						grid.Children.Add(textBlock3);
						Grid.SetColumn(textBlock3, 0);
						TextBlock textBlock4 = new TextBlock
						{
							FontSize = 13.0
						};
						BlueStacksUIBinding.BindColor(textBlock4, Control.ForegroundProperty, "SettingsWindowTabMenuItemForeground");
						textBlock4.Margin = new Thickness(7.0, 0.0, 0.0, 5.0);
						textBlock4.TextWrapping = TextWrapping.Wrap;
						BlueStacksUIBinding.Bind(textBlock4, keyValuePair.Value, "");
						grid.Children.Add(textBlock4);
						Grid.SetColumn(textBlock4, 1);
						this.mDetailsStack.Children.Add(grid);
					}
					return;
				}
			}
			this.mDetailsText.Text = "";
		}

		// Token: 0x06000B09 RID: 2825 RVA: 0x00008DB9 File Offset: 0x00006FB9
		private void CloseBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			BlueStacksUIUtils.CloseContainerWindow(this);
		}

		// Token: 0x06000B0A RID: 2826 RVA: 0x00008F72 File Offset: 0x00007172
		private void mDownloadButton_Click(object sender, RoutedEventArgs e)
		{
			Logger.Info("Clicked DownloadNow Button");
			EventHandler downloadClicked = this.DownloadClicked;
			if (downloadClicked == null)
			{
				return;
			}
			downloadClicked(sender, e);
		}

		// Token: 0x06000B0B RID: 2827 RVA: 0x00008F90 File Offset: 0x00007190
		private void DetailsStack_Initialized(object sender, EventArgs e)
		{
			this.mDetailsStack = sender as StackPanel;
		}

		// Token: 0x06000B0C RID: 2828 RVA: 0x00008F9E File Offset: 0x0000719E
		private void DetailsText_Initialized(object sender, EventArgs e)
		{
			this.mDetailsText = sender as TextBlock;
		}

		// Token: 0x06000B0D RID: 2829 RVA: 0x00008FAC File Offset: 0x000071AC
		private void TagLine_Initialized(object sender, EventArgs e)
		{
			this.mTagLine = sender as TextBlock;
		}

		// Token: 0x06000B0E RID: 2830 RVA: 0x00008FBA File Offset: 0x000071BA
		private void Description_Initialized(object sender, EventArgs e)
		{
			this.mDescription = sender as TextBlock;
		}

		// Token: 0x06000B0F RID: 2831 RVA: 0x00008FC8 File Offset: 0x000071C8
		private void FeaturesText_Initialized(object sender, EventArgs e)
		{
			this.mFeaturesText = sender as TextBlock;
		}

		// Token: 0x06000B10 RID: 2832 RVA: 0x00008FD6 File Offset: 0x000071D6
		private void FeaturesStack_Initialized(object sender, EventArgs e)
		{
			this.mFeaturesStack = sender as StackPanel;
		}

		// Token: 0x06000B11 RID: 2833 RVA: 0x0003EB4C File Offset: 0x0003CD4C
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/extensionpopupcontrol.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06000B12 RID: 2834 RVA: 0x00004028 File Offset: 0x00002228
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		internal Delegate _CreateDelegate(Type delegateType, string handler)
		{
			return Delegate.CreateDelegate(delegateType, this, handler);
		}

		// Token: 0x06000B13 RID: 2835 RVA: 0x0003EB7C File Offset: 0x0003CD7C
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
		[SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
		[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 1:
				this.mTitle = (Label)target;
				return;
			case 2:
				this.mSubTitle = (Label)target;
				return;
			case 3:
				this.mDownloadButton = (CustomButton)target;
				this.mDownloadButton.Click += this.mDownloadButton_Click;
				return;
			case 4:
				this.mCloseBtn = (CustomPictureBox)target;
				this.mCloseBtn.MouseLeftButtonUp += this.CloseBtn_MouseLeftButtonUp;
				return;
			case 5:
				((TextBlock)target).Initialized += this.TagLine_Initialized;
				return;
			case 6:
				((TextBlock)target).Initialized += this.Description_Initialized;
				return;
			case 7:
				((StackPanel)target).Initialized += this.FeaturesStack_Initialized;
				return;
			case 8:
				((TextBlock)target).Initialized += this.FeaturesText_Initialized;
				return;
			case 9:
				((StackPanel)target).Initialized += this.DetailsStack_Initialized;
				return;
			case 10:
				((TextBlock)target).Initialized += this.DetailsText_Initialized;
				return;
			case 11:
				this.slideShow = (SlideShowControl)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x04000696 RID: 1686
		private StackPanel mFeaturesStack;

		// Token: 0x04000697 RID: 1687
		private StackPanel mDetailsStack;

		// Token: 0x04000698 RID: 1688
		private TextBlock mDetailsText;

		// Token: 0x04000699 RID: 1689
		private TextBlock mTagLine;

		// Token: 0x0400069A RID: 1690
		private TextBlock mDescription;

		// Token: 0x0400069B RID: 1691
		private TextBlock mFeaturesText;

		// Token: 0x0400069D RID: 1693
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Label mTitle;

		// Token: 0x0400069E RID: 1694
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Label mSubTitle;

		// Token: 0x0400069F RID: 1695
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomButton mDownloadButton;

		// Token: 0x040006A0 RID: 1696
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mCloseBtn;

		// Token: 0x040006A1 RID: 1697
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal SlideShowControl slideShow;

		// Token: 0x040006A2 RID: 1698
		private bool _contentLoaded;

		// Token: 0x0200010A RID: 266
		[JsonObject(MemberSerialization.OptIn)]
		internal class ExtensionPopupContext
		{
			// Token: 0x06000B14 RID: 2836 RVA: 0x00008FE4 File Offset: 0x000071E4
			public static ExtensionPopupControl.ExtensionPopupContext ReadJson(JObject input)
			{
				ExtensionPopupControl.ExtensionPopupContext extensionPopupContext = input.ToObject<ExtensionPopupControl.ExtensionPopupContext>();
				extensionPopupContext.features = input["features"].ToIenumerableString();
				extensionPopupContext.ExtensionDetails = input["ExtensionDetails"].ToStringStringEnumerableKvp();
				return extensionPopupContext;
			}

			// Token: 0x06000B15 RID: 2837 RVA: 0x0003ECC8 File Offset: 0x0003CEC8
			public void WriteJson(JObject writer)
			{
				writer.Add("Title", this.Title);
				writer.Add("SubTitle", this.SubTitle);
				writer.Add("TagLine", this.TagLine);
				writer.Add("Description", this.Description);
				writer.Add("FeaturesText", this.FeaturesText);
				writer.Add("features", new JArray { this.features.ToList<string>() });
				writer.Add("ExtensionDetailText", this.ExtensionDetailText);
				writer.Add("ExtensionDetails");
				foreach (KeyValuePair<string, string> keyValuePair in this.ExtensionDetails)
				{
					writer.Add(keyValuePair.Key, keyValuePair.Value);
				}
			}

			// Token: 0x040006A3 RID: 1699
			[JsonProperty("Title", NullValueHandling = NullValueHandling.Ignore)]
			internal string Title;

			// Token: 0x040006A4 RID: 1700
			[JsonProperty("SubTitle", NullValueHandling = NullValueHandling.Ignore)]
			internal string SubTitle;

			// Token: 0x040006A5 RID: 1701
			[JsonProperty("TagLine", NullValueHandling = NullValueHandling.Ignore)]
			internal string TagLine;

			// Token: 0x040006A6 RID: 1702
			[JsonProperty("Description", NullValueHandling = NullValueHandling.Ignore)]
			internal string Description;

			// Token: 0x040006A7 RID: 1703
			[JsonProperty("FeaturesText", NullValueHandling = NullValueHandling.Ignore)]
			internal string FeaturesText;

			// Token: 0x040006A8 RID: 1704
			[JsonProperty("dummyFeatures", NullValueHandling = NullValueHandling.Ignore)]
			internal IEnumerable<string> features;

			// Token: 0x040006A9 RID: 1705
			[JsonProperty("ExtensionDetailText", NullValueHandling = NullValueHandling.Ignore)]
			internal string ExtensionDetailText;

			// Token: 0x040006AA RID: 1706
			[JsonProperty("dummyExtensionDetails", NullValueHandling = NullValueHandling.Ignore)]
			internal IEnumerable<KeyValuePair<string, string>> ExtensionDetails;
		}
	}
}
