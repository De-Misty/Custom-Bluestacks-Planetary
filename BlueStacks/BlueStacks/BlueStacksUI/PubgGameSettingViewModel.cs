using System;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000055 RID: 85
	public class PubgGameSettingViewModel : OtherAppGameSetting
	{
		// Token: 0x170001A0 RID: 416
		// (get) Token: 0x0600046F RID: 1135 RVA: 0x00004E7E File Offset: 0x0000307E
		// (set) Token: 0x06000470 RID: 1136 RVA: 0x00004E86 File Offset: 0x00003086
		public InGameResolution InGameResolution
		{
			get
			{
				return this.mInGameResolution;
			}
			set
			{
				base.SetProperty<InGameResolution>(ref this.mInGameResolution, value, null);
			}
		}

		// Token: 0x170001A1 RID: 417
		// (get) Token: 0x06000471 RID: 1137 RVA: 0x00004E97 File Offset: 0x00003097
		// (set) Token: 0x06000472 RID: 1138 RVA: 0x00004E9F File Offset: 0x0000309F
		public GraphicsQuality GraphicsQuality
		{
			get
			{
				return this.mGraphicsQuality;
			}
			set
			{
				base.SetProperty<GraphicsQuality>(ref this.mGraphicsQuality, value, null);
			}
		}

		// Token: 0x06000473 RID: 1139 RVA: 0x00004E28 File Offset: 0x00003028
		public PubgGameSettingViewModel(MainWindow parentWindow, string appName, string packageName)
			: base(parentWindow, appName, packageName)
		{
		}

		// Token: 0x06000474 RID: 1140 RVA: 0x0001C3E8 File Offset: 0x0001A5E8
		public override void Init()
		{
			base.Init();
			if (string.IsNullOrEmpty(RegistryManager.Instance.Guest[base.ParentWindow.mVmName].GamingResolutionPubg) || string.Equals(RegistryManager.Instance.Guest[base.ParentWindow.mVmName].GamingResolutionPubg, "1", StringComparison.InvariantCultureIgnoreCase))
			{
				this.InGameResolution = InGameResolution.HD_720p;
			}
			else if (string.Equals(RegistryManager.Instance.Guest[base.ParentWindow.mVmName].GamingResolutionPubg, "1.5", StringComparison.InvariantCultureIgnoreCase))
			{
				this.InGameResolution = InGameResolution.FHD_1080p;
			}
			else if (string.Equals(RegistryManager.Instance.Guest[base.ParentWindow.mVmName].GamingResolutionPubg, "2", StringComparison.InvariantCultureIgnoreCase))
			{
				this.InGameResolution = InGameResolution.QHD_1440p;
			}
			if (string.IsNullOrEmpty(RegistryManager.Instance.Guest[base.ParentWindow.mVmName].DisplayQualityPubg) || string.Equals(RegistryManager.Instance.Guest[base.ParentWindow.mVmName].DisplayQualityPubg, "-1", StringComparison.InvariantCultureIgnoreCase))
			{
				this.GraphicsQuality = GraphicsQuality.Auto;
				return;
			}
			if (string.Equals(RegistryManager.Instance.Guest[base.ParentWindow.mVmName].DisplayQualityPubg, "0", StringComparison.InvariantCultureIgnoreCase))
			{
				this.GraphicsQuality = GraphicsQuality.Smooth;
				return;
			}
			if (string.Equals(RegistryManager.Instance.Guest[base.ParentWindow.mVmName].DisplayQualityPubg, "1", StringComparison.InvariantCultureIgnoreCase))
			{
				this.GraphicsQuality = GraphicsQuality.Balanced;
				return;
			}
			if (string.Equals(RegistryManager.Instance.Guest[base.ParentWindow.mVmName].DisplayQualityPubg, "2", StringComparison.InvariantCultureIgnoreCase))
			{
				this.GraphicsQuality = GraphicsQuality.HD;
			}
		}

		// Token: 0x06000475 RID: 1141 RVA: 0x00004EB0 File Offset: 0x000030B0
		public override void LockOriginal()
		{
			base.LockOriginal();
			this.mOldInGameResolution = this.InGameResolution;
			this.mOldGraphicsQuality = this.GraphicsQuality;
		}

		// Token: 0x06000476 RID: 1142 RVA: 0x00004ED0 File Offset: 0x000030D0
		public override bool HasChanged()
		{
			return base.HasChanged() || this.InGameResolution != this.mOldInGameResolution || this.GraphicsQuality != this.mOldGraphicsQuality;
		}

		// Token: 0x06000477 RID: 1143 RVA: 0x0001C5B4 File Offset: 0x0001A7B4
		public override bool Save(bool restartReq)
		{
			restartReq = base.Save(restartReq);
			if (this.InGameResolution != this.mOldInGameResolution)
			{
				restartReq = true;
				switch (this.InGameResolution)
				{
				case InGameResolution.HD_720p:
					RegistryManager.Instance.Guest[base.ParentWindow.mVmName].GamingResolutionPubg = "1";
					GameSettingViewModel.SendGameSettingsStat("pubg_res_720");
					break;
				case InGameResolution.FHD_1080p:
					RegistryManager.Instance.Guest[base.ParentWindow.mVmName].GamingResolutionPubg = "1.5";
					GameSettingViewModel.SendGameSettingsStat("pubg_res_1080");
					break;
				case InGameResolution.QHD_1440p:
					RegistryManager.Instance.Guest[base.ParentWindow.mVmName].GamingResolutionPubg = "2";
					GameSettingViewModel.SendGameSettingsStat("pubg_res_1440");
					break;
				}
			}
			if (this.GraphicsQuality != this.mOldGraphicsQuality)
			{
				restartReq = true;
				switch (this.GraphicsQuality)
				{
				case GraphicsQuality.Auto:
					RegistryManager.Instance.Guest[base.ParentWindow.mVmName].DisplayQualityPubg = "-1";
					GameSettingViewModel.SendGameSettingsStat("pubg_gfx_auto");
					break;
				case GraphicsQuality.Smooth:
					RegistryManager.Instance.Guest[base.ParentWindow.mVmName].DisplayQualityPubg = "0";
					GameSettingViewModel.SendGameSettingsStat("pubg_gfx_smooth");
					break;
				case GraphicsQuality.Balanced:
					RegistryManager.Instance.Guest[base.ParentWindow.mVmName].DisplayQualityPubg = "1";
					GameSettingViewModel.SendGameSettingsStat("pubg_gfx_balanced");
					break;
				case GraphicsQuality.HD:
					RegistryManager.Instance.Guest[base.ParentWindow.mVmName].DisplayQualityPubg = "2";
					GameSettingViewModel.SendGameSettingsStat("pubg_gfx_hd");
					break;
				}
			}
			return restartReq;
		}

		// Token: 0x04000270 RID: 624
		private InGameResolution mOldInGameResolution;

		// Token: 0x04000271 RID: 625
		private GraphicsQuality mOldGraphicsQuality;

		// Token: 0x04000272 RID: 626
		private InGameResolution mInGameResolution;

		// Token: 0x04000273 RID: 627
		private GraphicsQuality mGraphicsQuality;
	}
}
