using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x0200011E RID: 286
	public class DeviceProfileControl : UserControl, IComponentConnector
	{
		// Token: 0x06000BBA RID: 3002 RVA: 0x000413D0 File Offset: 0x0003F5D0
		public DeviceProfileControl(MainWindow window)
		{
			this.InitializeComponent();
			this.ParentWindow = window;
			this.Init();
		}

		// Token: 0x06000BBB RID: 3003 RVA: 0x00041424 File Offset: 0x0003F624
		public void Init()
		{
			base.Visibility = Visibility.Hidden;
			base.IsVisibleChanged += this.DeviceProfileControl_IsVisibleChanged;
			this.mManufacturerTextBox.TextChanged += this.MManufacturerTextBox_TextChanged;
			this.mModelNumberTextBox.TextChanged += this.MManufacturerTextBox_TextChanged;
			this.mBrandTextBox.TextChanged += this.MManufacturerTextBox_TextChanged;
			if (PromotionObject.Instance.IsRootAccessEnabled || FeatureManager.Instance.IsCustomUIForNCSoft)
			{
				this.mRootAccessGrid.Visibility = Visibility.Visible;
				MainWindow parentWindow = this.ParentWindow;
				this.mCurrentRootAccessStatus = DeviceProfileControl.GetRootAccessStatusFromAndroid((parentWindow != null) ? parentWindow.mVmName : null);
				this.mEnableRootAccessCheckBox.IsChecked = new bool?(this.mCurrentRootAccessStatus);
			}
			this.mScrollBar.ScrollChanged += BluestacksUIColor.ScrollBarScrollChanged;
			this.mGettingProfilesFromCloud = false;
		}

		// Token: 0x06000BBC RID: 3004 RVA: 0x00041504 File Offset: 0x0003F704
		private static bool GetRootAccessStatusFromAndroid(string vmname)
		{
			bool flag;
			try
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string> { { "d", "bst.config.bindmount" } };
				JObject jobject = JObject.Parse(HTTPUtils.SendRequestToGuest("getprop", dictionary, vmname, 0, null, false, 1, 0, "bgp64"));
				if (string.Equals(jobject["result"].ToString(), "ok", StringComparison.InvariantCulture))
				{
					if (string.Equals(jobject["value"].ToString(), "1", StringComparison.InvariantCulture))
					{
						flag = true;
					}
					else
					{
						flag = false;
					}
				}
				else
				{
					flag = false;
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in Getting root status from android: " + ex.ToString());
				flag = false;
			}
			return flag;
		}

		// Token: 0x06000BBD RID: 3005 RVA: 0x000415B4 File Offset: 0x0003F7B4
		private void ChangeLoadingGridVisibility(bool state)
		{
			base.Dispatcher.Invoke(new Action(delegate
			{
				if (state)
				{
					this.mProfileLoader.Visibility = Visibility.Visible;
					this.mNoInternetWarning.Visibility = Visibility.Collapsed;
					this.mChildGrid.Visibility = Visibility.Collapsed;
					this.mMobileOperatorGrid.Visibility = Visibility.Collapsed;
					this.mTryAgainBtnGrid.Visibility = Visibility.Collapsed;
					return;
				}
				this.mProfileLoader.Visibility = Visibility.Collapsed;
				this.mNoInternetWarning.Visibility = Visibility.Collapsed;
				this.mChildGrid.Visibility = Visibility.Visible;
				if (RegistryManager.Instance.IsCacodeValid)
				{
					this.mMobileOperatorGrid.Visibility = Visibility.Visible;
				}
				this.mTryAgainBtnGrid.Visibility = Visibility.Collapsed;
			}), new object[0]);
		}

		// Token: 0x06000BBE RID: 3006 RVA: 0x000415F4 File Offset: 0x0003F7F4
		private void ChangeNoInternetGridVisibility(bool state)
		{
			base.Dispatcher.Invoke(new Action(delegate
			{
				if (state)
				{
					this.mProfileLoader.Visibility = Visibility.Collapsed;
					this.mNoInternetWarning.Visibility = Visibility.Visible;
					this.mChildGrid.Visibility = Visibility.Collapsed;
					this.mMobileOperatorGrid.Visibility = Visibility.Collapsed;
					this.mTryAgainBtnGrid.Visibility = Visibility.Visible;
					return;
				}
				this.mProfileLoader.Visibility = Visibility.Visible;
				this.mNoInternetWarning.Visibility = Visibility.Collapsed;
				this.mChildGrid.Visibility = Visibility.Collapsed;
				this.mMobileOperatorGrid.Visibility = Visibility.Collapsed;
				this.mTryAgainBtnGrid.Visibility = Visibility.Collapsed;
			}), new object[0]);
		}

		// Token: 0x06000BBF RID: 3007 RVA: 0x00041634 File Offset: 0x0003F834
		private void DeviceProfileControl_IsVisibleChanged(object _1, DependencyPropertyChangedEventArgs _2)
		{
			if (base.IsVisible)
			{
				if (!this.mGettingProfilesFromCloud)
				{
					this.mGettingProfilesFromCloud = true;
					this.ChangeLoadingGridVisibility(true);
					this.ChangeNoInternetGridVisibility(false);
					this.GetPreDefinedProfilesFromCloud();
					return;
				}
			}
			else
			{
				this.mSaveChangesBtn.IsEnabled = false;
				this.mEnableRootAccessCheckBox.IsChecked = new bool?(this.mCurrentRootAccessStatus);
			}
		}

		// Token: 0x06000BC0 RID: 3008 RVA: 0x00041690 File Offset: 0x0003F890
		private void SetUIAccordingToCurrentDeviceProfile()
		{
			this.mPredefinedProfilesComboBox.SelectionChanged -= this.mPredefinedProfilesComboBox_SelectionChanged;
			this.mMobileOperatorsCombobox.SelectionChanged -= this.MobileOperatorsCombobox_SelectionChanged;
			if (this.mCurrentDeviceProfileObject == null)
			{
				this.mPredefinedProfilesComboBox.Visibility = Visibility.Visible;
				this.mCustomProfileGrid.Visibility = Visibility.Collapsed;
			}
			else
			{
				JToken jtoken = this.mCurrentDeviceProfileObject["pcode"];
				if (string.Equals((jtoken != null) ? jtoken.ToString() : null, "custom", StringComparison.InvariantCulture))
				{
					this.mPredefinedProfilesComboBox.Visibility = Visibility.Collapsed;
					this.mCustomProfileGrid.Visibility = Visibility.Visible;
					this.mModelNumberTextBox.Text = this.mCurrentDeviceProfileObject["model"].ToString();
					this.mBrandTextBox.Text = this.mCurrentDeviceProfileObject["brand"].ToString();
					this.mManufacturerTextBox.Text = this.mCurrentDeviceProfileObject["manufacturer"].ToString();
					this.mCustomProfile.IsChecked = new bool?(true);
					this.mPredefinedProfilesComboBox.SelectedItem = null;
				}
				else
				{
					this.mPredefinedProfilesComboBox.Visibility = Visibility.Visible;
					this.mCustomProfileGrid.Visibility = Visibility.Collapsed;
					Dictionary<string, ComboBoxItem> dictionary = this.mDeviceProfileComboBoxItems;
					JToken jtoken2 = this.mCurrentDeviceProfileObject["pcode"];
					if (dictionary.ContainsKey((jtoken2 != null) ? jtoken2.ToString() : null))
					{
						this.mPredefinedProfilesComboBox.SelectedItem = this.mDeviceProfileComboBoxItems[this.mCurrentDeviceProfileObject["pcode"].ToString()];
					}
					this.mChooseProfile.IsChecked = new bool?(true);
					this.mModelNumberTextBox.Text = string.Empty;
					this.mBrandTextBox.Text = string.Empty;
					this.mManufacturerTextBox.Text = string.Empty;
				}
				Dictionary<string, ComboBoxItem> dictionary2 = this.mMobileOperatorComboboxItems;
				JToken jtoken3 = this.mCurrentDeviceProfileObject["caSelector"];
				if (dictionary2.ContainsKey((jtoken3 != null) ? jtoken3.ToString() : null))
				{
					this.mMobileOperatorsCombobox.SelectedItem = this.mMobileOperatorComboboxItems[this.mCurrentDeviceProfileObject["caSelector"].ToString()];
				}
			}
			this.mMobileOperatorsCombobox.SelectionChanged += this.MobileOperatorsCombobox_SelectionChanged;
			this.mPredefinedProfilesComboBox.SelectionChanged += this.mPredefinedProfilesComboBox_SelectionChanged;
			this.ChangeLoadingGridVisibility(false);
		}

		// Token: 0x06000BC1 RID: 3009 RVA: 0x0000967A File Offset: 0x0000787A
		private void GetPreDefinedProfilesFromCloud()
		{
			new Thread(delegate
			{
				try
				{
					this.GetCurrentDeviceProfileFromAndroid(this.ParentWindow.mVmName);
					if (this.mPreDefinedProfilesList.Count == 0 || this.mMobileOperatorsList.Count == 0)
					{
						string text = string.Format(CultureInfo.InvariantCulture, "{0}/{1}/{2}", new object[]
						{
							RegistryManager.Instance.Host,
							"bs4",
							"get_device_profile_list"
						});
						Dictionary<string, string> commonPOSTData = WebHelper.GetCommonPOSTData();
						commonPOSTData.Add("ca_code", Utils.GetValueInBootParams("caCode", this.ParentWindow.mVmName, "", "bgp64"));
						JObject jobject = JObject.Parse(BstHttpClient.Post(text, commonPOSTData, null, false, this.ParentWindow.mVmName, 0, 1, 0, false, "bgp64"));
						if (jobject != null && (bool)jobject["success"])
						{
							if (!JsonExtensions.IsNullOrEmptyBrackets(jobject["device_profile_list"].ToString()))
							{
								foreach (JObject jobject2 in jobject["device_profile_list"].ToArray<JToken>())
								{
									this.mPreDefinedProfilesList[jobject2["pcode"].ToString()] = jobject2["display_name"].ToString();
								}
							}
							if (jobject.ContainsKey("ca_selector_list") && !JsonExtensions.IsNullOrEmptyBrackets(jobject["ca_selector_list"].ToString()))
							{
								foreach (JObject jobject3 in jobject["ca_selector_list"].ToArray<JToken>())
								{
									this.mMobileOperatorsList[jobject3["ca_selector"].ToString()] = jobject3["display_name"].ToString();
								}
							}
							this.AddPreDefinedProfilesinComboBox();
						}
					}
					else
					{
						this.AddPreDefinedProfilesinComboBox();
					}
				}
				catch (Exception ex)
				{
					Logger.Error("Error while getting device profile from cloud : " + ex.ToString());
					this.ChangeNoInternetGridVisibility(true);
				}
			})
			{
				IsBackground = true
			}.Start();
		}

		// Token: 0x06000BC2 RID: 3010 RVA: 0x00009699 File Offset: 0x00007899
		internal void GetCurrentDeviceProfileFromAndroid(string vmName)
		{
			if (string.Equals(VmCmdHandler.SendRequest("currentdeviceprofile", null, vmName, out this.mCurrentDeviceProfileObject), "ok", StringComparison.InvariantCulture))
			{
				this.mCurrentDeviceProfileObject.Remove("result");
			}
		}

		// Token: 0x06000BC3 RID: 3011 RVA: 0x000096CB File Offset: 0x000078CB
		private void AddPreDefinedProfilesinComboBox()
		{
			base.Dispatcher.Invoke(new Action(delegate
			{
				foreach (string text in this.mPreDefinedProfilesList.Keys)
				{
					ComboBoxItem comboBoxItem = new ComboBoxItem
					{
						Content = this.mPreDefinedProfilesList[text]
					};
					this.mPredefinedProfilesComboBox.Items.Add(comboBoxItem);
					if (this.mDeviceProfileComboBoxItems.ContainsKey(text))
					{
						this.mDeviceProfileComboBoxItems[text] = comboBoxItem;
					}
					else
					{
						this.mDeviceProfileComboBoxItems.Add(text, comboBoxItem);
					}
				}
				foreach (string text2 in this.mMobileOperatorsList.Keys)
				{
					ComboBoxItem comboBoxItem2 = new ComboBoxItem
					{
						Content = this.mMobileOperatorsList[text2]
					};
					this.mMobileOperatorsCombobox.Items.Add(comboBoxItem2);
					this.mMobileOperatorComboboxItems[text2] = comboBoxItem2;
				}
				this.SetUIAccordingToCurrentDeviceProfile();
			}), new object[0]);
		}

		// Token: 0x06000BC4 RID: 3012 RVA: 0x000418EC File Offset: 0x0003FAEC
		private void Profile_Checked(object sender, RoutedEventArgs e)
		{
			if (this.mChooseProfile.IsChecked.Value)
			{
				this.mPredefinedProfilesComboBox.Visibility = Visibility.Visible;
				this.mCustomProfileGrid.Visibility = Visibility.Collapsed;
				JToken jtoken = this.mCurrentDeviceProfileObject["pcode"];
				this.mIsProfileChanged = string.Equals((jtoken != null) ? jtoken.ToString() : null, "custom", StringComparison.InvariantCulture);
				return;
			}
			if (this.mCustomProfile.IsChecked.Value)
			{
				this.mPredefinedProfilesComboBox.Visibility = Visibility.Collapsed;
				this.mCustomProfileGrid.Visibility = Visibility.Visible;
				JToken jtoken2 = this.mCurrentDeviceProfileObject["pcode"];
				this.mIsProfileChanged = !string.Equals((jtoken2 != null) ? jtoken2.ToString() : null, "custom", StringComparison.InvariantCulture);
			}
		}

		// Token: 0x06000BC5 RID: 3013 RVA: 0x000419BC File Offset: 0x0003FBBC
		private void mPredefinedProfilesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			string text;
			JObject changedDeviceProfileObject = this.GetChangedDeviceProfileObject(out text);
			UIElement uielement = this.mSaveChangesBtn;
			bool flag2;
			if (JToken.DeepEquals(this.mCurrentDeviceProfileObject, changedDeviceProfileObject))
			{
				bool? isChecked = this.mEnableRootAccessCheckBox.IsChecked;
				bool flag = this.mCurrentRootAccessStatus;
				flag2 = !((isChecked.GetValueOrDefault() == flag) & (isChecked != null));
			}
			else
			{
				flag2 = true;
			}
			uielement.IsEnabled = flag2;
		}

		// Token: 0x06000BC6 RID: 3014 RVA: 0x000419BC File Offset: 0x0003FBBC
		private void MManufacturerTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			string text;
			JObject changedDeviceProfileObject = this.GetChangedDeviceProfileObject(out text);
			UIElement uielement = this.mSaveChangesBtn;
			bool flag2;
			if (JToken.DeepEquals(this.mCurrentDeviceProfileObject, changedDeviceProfileObject))
			{
				bool? isChecked = this.mEnableRootAccessCheckBox.IsChecked;
				bool flag = this.mCurrentRootAccessStatus;
				flag2 = !((isChecked.GetValueOrDefault() == flag) & (isChecked != null));
			}
			else
			{
				flag2 = true;
			}
			uielement.IsEnabled = flag2;
		}

		// Token: 0x06000BC7 RID: 3015 RVA: 0x00041A18 File Offset: 0x0003FC18
		private JObject GetChangedDeviceProfileObject(out string jsonString)
		{
			jsonString = "{";
			JObject jobject = new JObject();
			string text = this.mCurrentDeviceProfileObject["pcode"].ToString();
			string text2 = this.mCurrentDeviceProfileObject["caSelector"].ToString();
			if (this.mChooseProfile.IsChecked.Value)
			{
				if (this.mPredefinedProfilesComboBox.SelectedItem != null)
				{
					string selectedDeviceProfile = (this.mPredefinedProfilesComboBox.SelectedItem as ComboBoxItem).Content.ToString();
					text = this.mPreDefinedProfilesList.FirstOrDefault((KeyValuePair<string, string> x) => x.Value == selectedDeviceProfile).Key;
				}
				jsonString += string.Format(CultureInfo.InvariantCulture, "\"createcustomprofile\":\"{0}\",", new object[] { "false" });
				jsonString += string.Format(CultureInfo.InvariantCulture, "\"pcode\":\"{0}\",", new object[] { text });
				jobject["pcode"] = text;
			}
			else
			{
				jsonString += string.Format(CultureInfo.InvariantCulture, "\"createcustomprofile\":\"{0}\",", new object[] { "true" });
				jsonString += string.Format(CultureInfo.InvariantCulture, "\"model\":\"{0}\",", new object[] { this.mModelNumberTextBox.Text });
				jsonString += string.Format(CultureInfo.InvariantCulture, "\"brand\":\"{0}\",", new object[] { this.mBrandTextBox.Text });
				jsonString += string.Format(CultureInfo.InvariantCulture, "\"manufacturer\":\"{0}\",", new object[] { this.mManufacturerTextBox.Text });
				jobject["pcode"] = "custom";
				jobject["model"] = this.mModelNumberTextBox.Text;
				jobject["brand"] = this.mBrandTextBox.Text;
				jobject["manufacturer"] = this.mManufacturerTextBox.Text;
			}
			if (this.mMobileOperatorsCombobox.SelectedItem != null)
			{
				string selectedMobileOperator = (this.mMobileOperatorsCombobox.SelectedItem as ComboBoxItem).Content.ToString();
				if (!string.IsNullOrEmpty(selectedMobileOperator))
				{
					text2 = this.mMobileOperatorsList.FirstOrDefault((KeyValuePair<string, string> x) => x.Value == selectedMobileOperator).Key;
				}
			}
			jsonString += string.Format(CultureInfo.InvariantCulture, "\"caSelector\":\"{0}\"", new object[] { text2 });
			jsonString += "}";
			jobject.Add("caSelector", text2);
			return jobject;
		}

		// Token: 0x06000BC8 RID: 3016 RVA: 0x00041CE4 File Offset: 0x0003FEE4
		private void SaveChangesBtn_Click(object sender, RoutedEventArgs e)
		{
			this.mSaveChangesBtn.IsEnabled = false;
			this.mIsProfileChanged = false;
			string text;
			JObject changedDeviceProfileObject = this.GetChangedDeviceProfileObject(out text);
			this.SendDeviceProfileChangeToGuest(text, changedDeviceProfileObject);
			bool? isChecked = this.mEnableRootAccessCheckBox.IsChecked;
			bool flag = this.mCurrentRootAccessStatus;
			if (!((isChecked.GetValueOrDefault() == flag) & (isChecked != null)))
			{
				string res = null;
				new Thread(delegate
				{
					try
					{
						if (!this.mCurrentRootAccessStatus)
						{
							res = HTTPUtils.SendRequestToGuest("bindmount", null, this.ParentWindow.mVmName, 0, null, false, 1, 0, "bgp64");
						}
						else
						{
							res = HTTPUtils.SendRequestToGuest("unbindmount", null, this.ParentWindow.mVmName, 0, null, false, 1, 0, "bgp64");
						}
						if (string.Equals(JObject.Parse(res)["result"].ToString(), "ok", StringComparison.InvariantCulture))
						{
							this.AddToastPopup(LocaleStrings.GetLocalizedString("STRING_CHANGES_SAVED", ""));
							this.mCurrentRootAccessStatus = !this.mCurrentRootAccessStatus;
							this.SendStatsOfRootAccessStatusAsync("success", this.mCurrentRootAccessStatus);
							if (SecurityMetrics.SecurityMetricsInstanceList.ContainsKey(this.ParentWindow.mVmName) && this.mCurrentRootAccessStatus)
							{
								SecurityMetrics.SecurityMetricsInstanceList[this.ParentWindow.mVmName].AddSecurityBreach(SecurityBreach.DEVICE_ROOTED, string.Empty);
							}
						}
						else
						{
							this.AddToastPopup(LocaleStrings.GetLocalizedString("STRING_ROOT_ACCESS_FAILURE", ""));
							this.Dispatcher.Invoke(new Action(delegate
							{
								this.mEnableRootAccessCheckBox.IsChecked = new bool?(this.mCurrentRootAccessStatus);
							}), new object[0]);
							this.SendStatsOfRootAccessStatusAsync("failed", this.mCurrentRootAccessStatus);
						}
						ClientStats.SendMiscellaneousStatsAsync("Setting-save", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "Advanced-Settings", "", null, this.ParentWindow.mVmName, null, null);
					}
					catch (Exception ex)
					{
						Logger.Error("Exception in sending mount unmount request to Android: " + ex.ToString());
					}
				})
				{
					IsBackground = true
				}.Start();
			}
		}

		// Token: 0x06000BC9 RID: 3017 RVA: 0x00041D6C File Offset: 0x0003FF6C
		private void AddToastPopup(string message)
		{
			base.Dispatcher.Invoke(new Action(delegate
			{
				if (this.mToastPopup == null)
				{
					this.mToastPopup = new CustomToastPopupControl(this);
				}
				Thickness thickness = new Thickness(0.0, 0.0, 0.0, 50.0);
				this.mToastPopup.Init(this, message, null, null, HorizontalAlignment.Center, VerticalAlignment.Bottom, new Thickness?(thickness), 12, null, null);
				this.mToastPopup.ShowPopup(1.3);
			}), new object[0]);
		}

		// Token: 0x06000BCA RID: 3018 RVA: 0x00041DAC File Offset: 0x0003FFAC
		private static void SendStatsOfDeviceProfileChangeAsync(string successString, JObject newDeviceProfile, JObject oldDeviceProfile)
		{
			ClientStats.SendMiscellaneousStatsAsync("DeviceProfileChangeStats", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, successString, JsonConvert.SerializeObject(newDeviceProfile), JsonConvert.SerializeObject(oldDeviceProfile), RegistryManager.Instance.Version, "DeviceProfileSetting", null);
		}

		// Token: 0x06000BCB RID: 3019 RVA: 0x00041DF4 File Offset: 0x0003FFF4
		private void SendStatsOfRootAccessStatusAsync(string successString, bool rootedstatus)
		{
			string text = (rootedstatus ? "Rooted" : "Unrooted");
			ClientStats.SendMiscellaneousStatsAsync("DeviceRootingStats", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, successString, text, this.ParentWindow.mVmName, null, null);
		}

		// Token: 0x06000BCC RID: 3020 RVA: 0x000096EB File Offset: 0x000078EB
		private void TryAgainBtn_Click(object sender, RoutedEventArgs e)
		{
			this.ChangeNoInternetGridVisibility(false);
			this.ChangeLoadingGridVisibility(true);
			this.GetPreDefinedProfilesFromCloud();
		}

		// Token: 0x06000BCD RID: 3021 RVA: 0x000419BC File Offset: 0x0003FBBC
		private void mEnableRootAccessCheckBox_Click(object sender, RoutedEventArgs e)
		{
			string text;
			JObject changedDeviceProfileObject = this.GetChangedDeviceProfileObject(out text);
			UIElement uielement = this.mSaveChangesBtn;
			bool flag2;
			if (JToken.DeepEquals(this.mCurrentDeviceProfileObject, changedDeviceProfileObject))
			{
				bool? isChecked = this.mEnableRootAccessCheckBox.IsChecked;
				bool flag = this.mCurrentRootAccessStatus;
				flag2 = !((isChecked.GetValueOrDefault() == flag) & (isChecked != null));
			}
			else
			{
				flag2 = true;
			}
			uielement.IsEnabled = flag2;
		}

		// Token: 0x06000BCE RID: 3022 RVA: 0x00041E48 File Offset: 0x00040048
		private void SendDeviceProfileChangeToGuest(string json, JObject changedDeviceProfileObject)
		{
			if (Utils.CheckIfDeviceProfileChanged(this.mCurrentDeviceProfileObject, changedDeviceProfileObject))
			{
				string command = string.Format(CultureInfo.InvariantCulture, "{0} {1}", new object[] { "changeDeviceProfile", json });
				Logger.Info("Command for device profile change: " + command);
				new Thread(delegate
				{
					try
					{
						string text = VmCmdHandler.RunCommand(command, this.ParentWindow.mVmName);
						Logger.Info("Result for device profile change command: " + text);
						if (string.Equals(text, "ok", StringComparison.InvariantCulture))
						{
							this.AddToastPopup(LocaleStrings.GetLocalizedString("STRING_CHANGES_SAVED", ""));
							DeviceProfileControl.SendStatsOfDeviceProfileChangeAsync("success", changedDeviceProfileObject, this.mCurrentDeviceProfileObject);
							this.mCurrentDeviceProfileObject = changedDeviceProfileObject;
							Utils.UpdateValueInBootParams("pcode", changedDeviceProfileObject["pcode"].ToString(), this.ParentWindow.mVmName, false, "bgp64");
							Utils.UpdateValueInBootParams("caSelector", changedDeviceProfileObject["caSelector"].ToString(), this.ParentWindow.mVmName, false, "bgp64");
							if (SecurityMetrics.SecurityMetricsInstanceList.ContainsKey(this.ParentWindow.mVmName))
							{
								SecurityMetrics.SecurityMetricsInstanceList[this.ParentWindow.mVmName].AddSecurityBreach(SecurityBreach.DEVICE_PROFILE_CHANGED, string.Empty);
							}
						}
						else
						{
							this.AddToastPopup(LocaleStrings.GetLocalizedString("STRING_SWITCH_PROFILE_FAILED", ""));
							DeviceProfileControl.SendStatsOfDeviceProfileChangeAsync("failed", changedDeviceProfileObject, this.mCurrentDeviceProfileObject);
						}
						this.Dispatcher.Invoke(new Action(delegate
						{
							this.SetUIAccordingToCurrentDeviceProfile();
						}), new object[0]);
					}
					catch (Exception ex)
					{
						Logger.Error("Exception in change to predefined Pcode call to android: " + ex.ToString());
					}
				})
				{
					IsBackground = true
				}.Start();
			}
		}

		// Token: 0x06000BCF RID: 3023 RVA: 0x000419BC File Offset: 0x0003FBBC
		private void MobileOperatorsCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			string text;
			JObject changedDeviceProfileObject = this.GetChangedDeviceProfileObject(out text);
			UIElement uielement = this.mSaveChangesBtn;
			bool flag2;
			if (JToken.DeepEquals(this.mCurrentDeviceProfileObject, changedDeviceProfileObject))
			{
				bool? isChecked = this.mEnableRootAccessCheckBox.IsChecked;
				bool flag = this.mCurrentRootAccessStatus;
				flag2 = !((isChecked.GetValueOrDefault() == flag) & (isChecked != null));
			}
			else
			{
				flag2 = true;
			}
			uielement.IsEnabled = flag2;
		}

		// Token: 0x06000BD0 RID: 3024 RVA: 0x00009701 File Offset: 0x00007901
		public bool IsDirty()
		{
			return this.mSaveChangesBtn.IsEnabled || this.mIsProfileChanged;
		}

		// Token: 0x06000BD1 RID: 3025 RVA: 0x00041EE4 File Offset: 0x000400E4
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/settingswindows/deviceprofilecontrol.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06000BD2 RID: 3026 RVA: 0x00041F14 File Offset: 0x00040114
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
				this.mScrollBar = (ScrollViewer)target;
				return;
			case 2:
				this.mProfileLoader = (Border)target;
				return;
			case 3:
				this.mNoInternetWarning = (Border)target;
				return;
			case 4:
				this.mChildGrid = (Grid)target;
				return;
			case 5:
				this.mChooseProfile = (CustomRadioButton)target;
				this.mChooseProfile.Checked += this.Profile_Checked;
				return;
			case 6:
				this.mPredefinedProfilesComboBox = (CustomComboBox)target;
				this.mPredefinedProfilesComboBox.SelectionChanged += this.mPredefinedProfilesComboBox_SelectionChanged;
				return;
			case 7:
				this.mCustomProfile = (CustomRadioButton)target;
				this.mCustomProfile.Checked += this.Profile_Checked;
				return;
			case 8:
				this.mCustomProfileGrid = (Grid)target;
				return;
			case 9:
				this.mManufacturerTextBox = (CustomTextBox)target;
				return;
			case 10:
				this.mBrandTextBox = (CustomTextBox)target;
				return;
			case 11:
				this.mModelNumberTextBox = (CustomTextBox)target;
				return;
			case 12:
				this.mTryAgainBtnGrid = (Grid)target;
				return;
			case 13:
				((CustomButton)target).Click += this.TryAgainBtn_Click;
				return;
			case 14:
				this.mMobileOperatorGrid = (Grid)target;
				return;
			case 15:
				this.mMobileOpertorText = (TextBlock)target;
				return;
			case 16:
				this.mMobileNetworkSetupText = (TextBlock)target;
				return;
			case 17:
				this.mMobileOperatorsCombobox = (CustomComboBox)target;
				this.mMobileOperatorsCombobox.SelectionChanged += this.MobileOperatorsCombobox_SelectionChanged;
				return;
			case 18:
				this.mRootAccessGrid = (Grid)target;
				return;
			case 19:
				this.mEnableRootAccessCheckBox = (CustomCheckbox)target;
				this.mEnableRootAccessCheckBox.Click += this.mEnableRootAccessCheckBox_Click;
				return;
			case 20:
				this.mInfoIcon = (CustomPictureBox)target;
				return;
			case 21:
				this.mSaveChangesBtn = (CustomButton)target;
				this.mSaveChangesBtn.Click += this.SaveChangesBtn_Click;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x04000715 RID: 1813
		private JObject mCurrentDeviceProfileObject;

		// Token: 0x04000716 RID: 1814
		private Dictionary<string, string> mPreDefinedProfilesList = new Dictionary<string, string>();

		// Token: 0x04000717 RID: 1815
		private Dictionary<string, ComboBoxItem> mDeviceProfileComboBoxItems = new Dictionary<string, ComboBoxItem>();

		// Token: 0x04000718 RID: 1816
		private Dictionary<string, string> mMobileOperatorsList = new Dictionary<string, string>();

		// Token: 0x04000719 RID: 1817
		private Dictionary<string, ComboBoxItem> mMobileOperatorComboboxItems = new Dictionary<string, ComboBoxItem>();

		// Token: 0x0400071A RID: 1818
		private CustomToastPopupControl mToastPopup;

		// Token: 0x0400071B RID: 1819
		private MainWindow ParentWindow;

		// Token: 0x0400071C RID: 1820
		private bool mGettingProfilesFromCloud;

		// Token: 0x0400071D RID: 1821
		private bool mCurrentRootAccessStatus;

		// Token: 0x0400071E RID: 1822
		private bool mIsProfileChanged;

		// Token: 0x0400071F RID: 1823
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal ScrollViewer mScrollBar;

		// Token: 0x04000720 RID: 1824
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mProfileLoader;

		// Token: 0x04000721 RID: 1825
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mNoInternetWarning;

		// Token: 0x04000722 RID: 1826
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mChildGrid;

		// Token: 0x04000723 RID: 1827
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomRadioButton mChooseProfile;

		// Token: 0x04000724 RID: 1828
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomComboBox mPredefinedProfilesComboBox;

		// Token: 0x04000725 RID: 1829
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomRadioButton mCustomProfile;

		// Token: 0x04000726 RID: 1830
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mCustomProfileGrid;

		// Token: 0x04000727 RID: 1831
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomTextBox mManufacturerTextBox;

		// Token: 0x04000728 RID: 1832
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomTextBox mBrandTextBox;

		// Token: 0x04000729 RID: 1833
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomTextBox mModelNumberTextBox;

		// Token: 0x0400072A RID: 1834
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mTryAgainBtnGrid;

		// Token: 0x0400072B RID: 1835
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mMobileOperatorGrid;

		// Token: 0x0400072C RID: 1836
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mMobileOpertorText;

		// Token: 0x0400072D RID: 1837
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mMobileNetworkSetupText;

		// Token: 0x0400072E RID: 1838
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomComboBox mMobileOperatorsCombobox;

		// Token: 0x0400072F RID: 1839
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mRootAccessGrid;

		// Token: 0x04000730 RID: 1840
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomCheckbox mEnableRootAccessCheckBox;

		// Token: 0x04000731 RID: 1841
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mInfoIcon;

		// Token: 0x04000732 RID: 1842
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomButton mSaveChangesBtn;

		// Token: 0x04000733 RID: 1843
		private bool _contentLoaded;
	}
}
