using System;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020001C4 RID: 452
	internal class MainWindowsStaticComponents
	{
		// Token: 0x14000015 RID: 21
		// (add) Token: 0x060011E0 RID: 4576 RVA: 0x0006FC34 File Offset: 0x0006DE34
		// (remove) Token: 0x060011E1 RID: 4577 RVA: 0x0006FC6C File Offset: 0x0006DE6C
		internal event EventHandler ShowAllUninstallButtons;

		// Token: 0x14000016 RID: 22
		// (add) Token: 0x060011E2 RID: 4578 RVA: 0x0006FCA4 File Offset: 0x0006DEA4
		// (remove) Token: 0x060011E3 RID: 4579 RVA: 0x0006FCDC File Offset: 0x0006DEDC
		internal event EventHandler HideAllUninstallButtons;

		// Token: 0x14000017 RID: 23
		// (add) Token: 0x060011E4 RID: 4580 RVA: 0x0006FD14 File Offset: 0x0006DF14
		// (remove) Token: 0x060011E5 RID: 4581 RVA: 0x0006FD4C File Offset: 0x0006DF4C
		internal event Action PlayAllGifs;

		// Token: 0x14000018 RID: 24
		// (add) Token: 0x060011E6 RID: 4582 RVA: 0x0006FD84 File Offset: 0x0006DF84
		// (remove) Token: 0x060011E7 RID: 4583 RVA: 0x0006FDBC File Offset: 0x0006DFBC
		internal event Action PauseAllGifs;

		// Token: 0x060011E8 RID: 4584 RVA: 0x0000CAFC File Offset: 0x0000ACFC
		internal void ShowUninstallButtons(bool isShow)
		{
			this.IsDeleteButtonVisible = isShow;
			if (isShow)
			{
				EventHandler showAllUninstallButtons = this.ShowAllUninstallButtons;
				if (showAllUninstallButtons == null)
				{
					return;
				}
				showAllUninstallButtons(null, new EventArgs());
				return;
			}
			else
			{
				EventHandler hideAllUninstallButtons = this.HideAllUninstallButtons;
				if (hideAllUninstallButtons == null)
				{
					return;
				}
				hideAllUninstallButtons(null, new EventArgs());
				return;
			}
		}

		// Token: 0x060011E9 RID: 4585 RVA: 0x0000CB35 File Offset: 0x0000AD35
		internal void PlayPauseGifs(bool isPlay)
		{
			if (isPlay)
			{
				Action playAllGifs = this.PlayAllGifs;
				if (playAllGifs == null)
				{
					return;
				}
				playAllGifs();
				return;
			}
			else
			{
				Action pauseAllGifs = this.PauseAllGifs;
				if (pauseAllGifs == null)
				{
					return;
				}
				pauseAllGifs();
				return;
			}
		}

		// Token: 0x04000BB6 RID: 2998
		internal AppTabButton mSelectedTabButton;

		// Token: 0x04000BB7 RID: 2999
		internal bool mPreviousSelectedTabWeb;

		// Token: 0x04000BB8 RID: 3000
		internal HomeAppTabButton mSelectedHomeAppTabButton;

		// Token: 0x04000BB9 RID: 3001
		internal bool IsDeleteButtonVisible;

		// Token: 0x04000BBA RID: 3002
		internal IntPtr mLastMappableWindowHandle = IntPtr.Zero;
	}
}
