using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000177 RID: 375
	public class GroupByGrid : DataGridView
	{
		// Token: 0x06000F0A RID: 3850 RVA: 0x0000B132 File Offset: 0x00009332
		protected override void OnCellFormatting(DataGridViewCellFormattingEventArgs args)
		{
			base.OnCellFormatting(args);
			if (args != null)
			{
				if (args.RowIndex == 0)
				{
					return;
				}
				if (this.IsRepeatedCellValue(args.RowIndex, args.ColumnIndex))
				{
					args.Value = string.Empty;
					args.FormattingApplied = true;
				}
			}
		}

		// Token: 0x06000F0B RID: 3851 RVA: 0x0005FD54 File Offset: 0x0005DF54
		private bool IsRepeatedCellValue(int rowIndex, int colIndex)
		{
			DataGridViewCell dataGridViewCell = base.Rows[rowIndex].Cells[colIndex];
			DataGridViewCell dataGridViewCell2 = base.Rows[rowIndex - 1].Cells[colIndex];
			return this.ColumnsToBeGrouped.Contains(colIndex) && (dataGridViewCell.Value == dataGridViewCell2.Value || (dataGridViewCell.Value != null && dataGridViewCell2.Value != null && dataGridViewCell.Value.ToString() == dataGridViewCell2.Value.ToString()));
		}

		// Token: 0x06000F0C RID: 3852 RVA: 0x0005FDE4 File Offset: 0x0005DFE4
		protected override void OnCellPainting(DataGridViewCellPaintingEventArgs args)
		{
			base.OnCellPainting(args);
			if (args != null)
			{
				args.AdvancedBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.None;
				if (args.RowIndex < 1 || args.ColumnIndex < 0)
				{
					return;
				}
				if (this.IsRepeatedCellValue(args.RowIndex, args.ColumnIndex))
				{
					args.AdvancedBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None;
					return;
				}
				args.AdvancedBorderStyle.Top = base.AdvancedCellBorderStyle.Top;
			}
		}

		// Token: 0x040009EC RID: 2540
		internal List<int> ColumnsToBeGrouped = new List<int>();
	}
}
