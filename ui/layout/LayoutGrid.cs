using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LansUILib.ui
{
    public enum LayoutGridType
	{
		Columns,
		Rows
	}
    public class LayoutGrid : Layout
    {
		

		LayoutGridType layoutType;
        int columnsOrRows;
		int[] padding = new int[4];
		int spacing = 0;

		bool[] fitSizeToChildren;
		bool[] expandChildren;

        public LayoutGrid(int columnsOrRows, bool[] fitSizeToChildren, bool[] expandChildren, LayoutGridType layoutType = LayoutGridType.Columns, int paddingLeft = 0, int paddingTop = 0, int paddingRight = 0, int paddingBottom = 0, int spacing = 0)
		{
			this.layoutType = layoutType;

            this.columnsOrRows = columnsOrRows;
			padding[0] = paddingLeft;
			padding[1] = paddingTop;
			padding[2] = paddingRight;
			padding[3] = paddingBottom;
			this.spacing = spacing;
			this.fitSizeToChildren = fitSizeToChildren;
			this.expandChildren = expandChildren;

        }

		public void SetColumnsOrRowsCount(int count)
		{
			this.columnsOrRows = count;
        }

		public void SetPadding(int paddingLeft = 0, int paddingTop = 0, int paddingRight = 0, int paddingBottom = 0, int spacing = 0)
		{
            padding[0] = paddingLeft;
            padding[1] = paddingTop;
            padding[2] = paddingRight;
            padding[3] = paddingBottom;
            this.spacing = spacing;
        }

		protected int[][][] GetColumnAndRowSizes()
		{
            var childCount = component.Children.Count();

            int columns = columnsOrRows;
            int rows = (childCount + columnsOrRows - 1) / columnsOrRows;
            if (layoutType == LayoutGridType.Rows)
            {
                var temp = columns;
                columns = rows;
                rows = temp;
            }


            int[] columnWidth = new int[columns];
            int[] columnFlex = new int[columns];
            int[] rowHeight = new int[rows];
            int[] rowFlex = new int[rows];
            int[][] prefChildSize = new int[component.Children.Count()][];

            int index = 0;
            foreach (var c in component.Children)
            {
                var prefSize = c.GetLayout().GetPrefferedSize();
                var cColumn = index % columns;
                var cRow = index / columns;

                columnWidth[cColumn] = Math.Max(columnWidth[cColumn], prefSize[0]);
                columnFlex[cColumn] = Math.Max(columnFlex[cColumn], c.GetLayout().Flex);
                rowHeight[cRow] = Math.Max(rowHeight[cRow], prefSize[1]);
                rowFlex[cRow] = Math.Max(rowFlex[cRow], c.GetLayout().Flex);
                prefChildSize[index] = prefSize;
                index++;
            }

            return new int[][][] { new int[][] { columnWidth, rowHeight, columnFlex, rowFlex }, prefChildSize };
        }

		public override void Update()
        {
            base.Update();

            var columnAndRows = GetColumnAndRowSizes();


            int[] columnWidth = columnAndRows[0][0];
            int[] rowHeight = columnAndRows[0][1];
            int[] columnFlex = columnAndRows[0][2];
            int[] rowFlex = columnAndRows[0][3];
            int[][] childPrefSize = columnAndRows[1];

            var newWidth = columnWidth.Sum() + (columnWidth.Count() - 1) * spacing + padding[0] + padding[2];
            var newHeight = rowHeight.Sum() + (rowHeight.Count() - 1) * spacing + padding[1] + padding[3];
            if (!this.component.Parent.GetLayout().ControlsChildren())
            {
                if (fitSizeToChildren[0])
                {
                    var diff = newWidth - this.Width;
                    this.X -= (int)(diff * component.GetPivot(0));
                    this.Width = newWidth;
                }
                if (fitSizeToChildren[1])
                {
                    var diff = newHeight - this.Height;
                    this.Y -= (int)(diff * component.GetPivot(1));
                    this.Height = newHeight;
                }
            }

            var diffX = Math.Max(this.Width-newWidth,0);
            var diffY = Math.Max(this.Height-newHeight,0);
            var flexX = Math.Max(1,columnFlex.Sum());
            var flexY = Math.Max(1, rowFlex.Sum());



            int cx = padding[0] + this.X;
            int cy = padding[1] + this.Y;

            var index = 0;
            foreach (var c in component.Children)
            {
                var cColumn = index % columnWidth.Length;
                var cRow = index / columnWidth.Length;

                if (expandChildren[0])
                {
					c.GetLayout().Width = columnWidth[cColumn] + (int) (diffX * ((float)columnFlex[cColumn]/ flexX));
                }
                else
                {
                    c.GetLayout().Width = childPrefSize[index][0];
                }
				if(expandChildren[1])
				{
                    c.GetLayout().Height = rowHeight[cRow] + (int)(diffY * ((float)rowFlex[cRow] / flexY));
                }
                else
                {
                    c.GetLayout().Height = childPrefSize[index][1];
                }

                c.GetLayout().X = cx;
                c.GetLayout().Y = cy;

				if (cColumn == columnWidth.Length - 1)
				{
					cx = padding[0] + this.X;
					cy += c.GetLayout().Height + spacing;
				}
				else
				{
					cx += c.GetLayout().Width + spacing;
				}

                index++;
            }
            
            this.Refresh();
        }

		public override bool ControlsChildren()
		{
			return true;
		}

        public override int[] GetPrefferedSize()
        {
            var columnAndRows = GetColumnAndRowSizes();
            int[] columnWidth = columnAndRows[0][0];
            int[] rowHeight = columnAndRows[0][1];

            var newWidth = 0;
            if (fitSizeToChildren[0])
            {
                newWidth = columnWidth.Sum() + (columnWidth.Count() - 1) * spacing + padding[0] + padding[2];
            }
            var newHeight = 0;
            if (fitSizeToChildren[1])
            {
                newHeight = rowHeight.Sum() + (rowHeight.Count() - 1) * spacing + padding[1] + padding[3];
            }

            return new int[] { newWidth, newHeight };
        }
    }
}
