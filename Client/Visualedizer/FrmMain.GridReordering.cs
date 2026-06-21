using System.ComponentModel;

namespace Ledqualizer
{
    public partial class FrmMain
    {
        private enum GridReorderDragKind
        {
            Scene,
            Collection,
            DeviceBlock,
            DeviceStrip
        }

        [Serializable]
        private sealed class GridReorderDragInfo
        {
            public GridReorderDragInfo(GridReorderDragKind kind, string sourceGridName, string sourceId, string parentDeviceId)
            {
                Kind = kind;
                SourceGridName = sourceGridName;
                SourceId = sourceId;
                ParentDeviceId = parentDeviceId;
            }

            public GridReorderDragKind Kind { get; }
            public string SourceGridName { get; }
            public string SourceId { get; }
            public string ParentDeviceId { get; }
        }

        private sealed class DeviceRowBlock
        {
            public DeviceRowBlock(DeviceGridRow root)
            {
                Root = root;
            }

            public DeviceGridRow Root { get; }
            public List<DeviceGridRow> Strips { get; } = new();
        }

        private DataGridView? reorderDragGrid;
        private int reorderDragRowIndex = -1;
        private Point reorderDragStartPoint = Point.Empty;

        private void InitializeGridReordering()
        {
            ConfigureGridReordering(dgvDevices);
            ConfigureGridReordering(dgvScenes);
            ConfigureGridReordering(dgvCollections);
        }

        private void ConfigureGridReordering(DataGridView grid)
        {
            grid.AllowDrop = true;
            grid.MouseDown += GridReorder_MouseDown;
            grid.MouseMove += GridReorder_MouseMove;
            grid.MouseUp += GridReorder_MouseUp;
            grid.DragEnter += GridReorder_DragEnter;
            grid.DragOver += GridReorder_DragOver;
            grid.DragDrop += GridReorder_DragDrop;
        }

        private void GridReorder_MouseDown(object? sender, MouseEventArgs e)
        {
            if (sender is not DataGridView grid || e.Button != MouseButtons.Left)
            {
                ClearPendingGridReorderDrag();
                return;
            }

            DataGridView.HitTestInfo hit = grid.HitTest(e.X, e.Y);
            if (hit.RowIndex < 0)
            {
                ClearPendingGridReorderDrag();
                return;
            }

            reorderDragGrid = grid;
            reorderDragRowIndex = hit.RowIndex;
            reorderDragStartPoint = e.Location;
        }

        private void GridReorder_MouseMove(object? sender, MouseEventArgs e)
        {
            if (sender is not DataGridView grid
                || e.Button != MouseButtons.Left
                || !ReferenceEquals(reorderDragGrid, grid)
                || reorderDragRowIndex < 0)
            {
                return;
            }

            Size dragSize = SystemInformation.DragSize;
            Rectangle dragBounds = new(
                reorderDragStartPoint.X - (dragSize.Width / 2),
                reorderDragStartPoint.Y - (dragSize.Height / 2),
                dragSize.Width,
                dragSize.Height);
            if (dragBounds.Contains(e.Location))
            {
                return;
            }

            GridReorderDragInfo? dragInfo = CreateGridReorderDragInfo(grid, reorderDragRowIndex);
            if (dragInfo == null)
            {
                ClearPendingGridReorderDrag();
                return;
            }

            EndInlineStripNameEdit(commit: true);
            grid.EndEdit();

            var data = new DataObject();
            data.SetData(typeof(GridReorderDragInfo), dragInfo);

            try
            {
                grid.DoDragDrop(data, DragDropEffects.Move);
            }
            finally
            {
                ClearPendingGridReorderDrag();
            }
        }

        private void GridReorder_MouseUp(object? sender, MouseEventArgs e)
        {
            ClearPendingGridReorderDrag();
        }

        private void GridReorder_DragEnter(object? sender, DragEventArgs e)
        {
            if (sender is not DataGridView grid)
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            e.Effect = GetGridReorderEffect(grid, e);
        }

        private void GridReorder_DragOver(object? sender, DragEventArgs e)
        {
            if (sender is not DataGridView grid)
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            e.Effect = GetGridReorderEffect(grid, e);
        }

        private async void GridReorder_DragDrop(object? sender, DragEventArgs e)
        {
            if (sender is not DataGridView grid || !TryGetGridReorderDragInfo(e.Data, out GridReorderDragInfo? dragInfo))
            {
                return;
            }

            if (dragInfo == null)
            {
                return;
            }

            Point clientPoint = grid.PointToClient(new Point(e.X, e.Y));
            switch (dragInfo.Kind)
            {
                case GridReorderDragKind.Scene:
                    if (grid == dgvScenes && MoveSceneRow(dragInfo.SourceId, GetLinearInsertionIndex(grid, clientPoint)))
                    {
                        RefreshSceneAssignmentOptions();
                        sceneGridBindingSource.ResetBindings(false);
                    }
                    break;
                case GridReorderDragKind.Collection:
                    if (grid == dgvCollections && MoveCollectionRow(dragInfo.SourceId, GetLinearInsertionIndex(grid, clientPoint)))
                    {
                        collectionGridBindingSource.ResetBindings(false);
                    }
                    break;
                case GridReorderDragKind.DeviceBlock:
                    if (grid == dgvDevices
                        && TryGetDeviceBlockInsertionIndex(clientPoint, dragInfo.SourceId, out int deviceTargetIndex)
                        && MoveDeviceBlock(dragInfo.SourceId, deviceTargetIndex))
                    {
                        await ReconcileDeviceRunsAsync();
                    }
                    break;
                case GridReorderDragKind.DeviceStrip:
                    if (grid == dgvDevices
                        && TryGetDeviceStripInsertionIndex(clientPoint, dragInfo.ParentDeviceId, dragInfo.SourceId, out int stripTargetIndex)
                        && MoveDeviceStrip(dragInfo.ParentDeviceId, dragInfo.SourceId, stripTargetIndex))
                    {
                        await ReconcileDeviceRunsAsync();
                    }
                    break;
            }
        }

        private GridReorderDragInfo? CreateGridReorderDragInfo(DataGridView grid, int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= grid.Rows.Count)
            {
                return null;
            }

            return grid.Rows[rowIndex].DataBoundItem switch
            {
                SceneGridRow row when ReferenceEquals(grid, dgvScenes) => new GridReorderDragInfo(GridReorderDragKind.Scene, grid.Name, row.Id, string.Empty),
                CollectionGridRow row when ReferenceEquals(grid, dgvCollections) => new GridReorderDragInfo(GridReorderDragKind.Collection, grid.Name, row.Id, string.Empty),
                DeviceGridRow row when ReferenceEquals(grid, dgvDevices) && row.Kind == DeviceRowKind.Device
                    => new GridReorderDragInfo(GridReorderDragKind.DeviceBlock, grid.Name, row.Id, row.Id),
                DeviceGridRow row when ReferenceEquals(grid, dgvDevices) && row.Kind == DeviceRowKind.Strip
                    => new GridReorderDragInfo(GridReorderDragKind.DeviceStrip, grid.Name, row.Id, row.ParentDeviceId),
                _ => null
            };
        }

        private DragDropEffects GetGridReorderEffect(DataGridView grid, DragEventArgs e)
        {
            if (!TryGetGridReorderDragInfo(e.Data, out GridReorderDragInfo? dragInfo)
                || dragInfo == null
                || !string.Equals(dragInfo.SourceGridName, grid.Name, StringComparison.Ordinal))
            {
                return DragDropEffects.None;
            }

            Point clientPoint = grid.PointToClient(new Point(e.X, e.Y));
            return dragInfo.Kind switch
            {
                GridReorderDragKind.Scene when ReferenceEquals(grid, dgvScenes) => CanMoveLinearRow(sceneRows.Select(row => row.Id).ToList(), dragInfo.SourceId, GetLinearInsertionIndex(grid, clientPoint))
                    ? DragDropEffects.Move
                    : DragDropEffects.None,
                GridReorderDragKind.Collection when ReferenceEquals(grid, dgvCollections) => CanMoveLinearRow(collectionRows.Select(row => row.Id).ToList(), dragInfo.SourceId, GetLinearInsertionIndex(grid, clientPoint))
                    ? DragDropEffects.Move
                    : DragDropEffects.None,
                GridReorderDragKind.DeviceBlock when ReferenceEquals(grid, dgvDevices) && TryGetDeviceBlockInsertionIndex(clientPoint, dragInfo.SourceId, out _)
                    => DragDropEffects.Move,
                GridReorderDragKind.DeviceStrip when ReferenceEquals(grid, dgvDevices) && TryGetDeviceStripInsertionIndex(clientPoint, dragInfo.ParentDeviceId, dragInfo.SourceId, out _)
                    => DragDropEffects.Move,
                _ => DragDropEffects.None
            };
        }

        private static bool TryGetGridReorderDragInfo(IDataObject? data, out GridReorderDragInfo? dragInfo)
        {
            dragInfo = data?.GetData(typeof(GridReorderDragInfo)) as GridReorderDragInfo;
            return dragInfo != null;
        }

        private bool MoveSceneRow(string sceneId, int insertionIndex)
        {
            List<SceneGridRow> orderedRows = sceneRows.ToList();
            if (!MoveLinearItem(orderedRows, row => row.Id, sceneId, insertionIndex, out SceneGridRow? movedRow))
            {
                return false;
            }

            ApplyOrderedBindingList(sceneRows, orderedRows);
            SyncSceneOrderFromRows();
            SelectGridRowByItem(dgvScenes, movedRow);
            return true;
        }

        private bool MoveCollectionRow(string collectionId, int insertionIndex)
        {
            List<CollectionGridRow> orderedRows = collectionRows.ToList();
            if (!MoveLinearItem(orderedRows, row => row.Id, collectionId, insertionIndex, out CollectionGridRow? movedRow))
            {
                return false;
            }

            ApplyOrderedBindingList(collectionRows, orderedRows);
            SyncCollectionOrderFromRows();
            SelectGridRowByItem(dgvCollections, movedRow);
            return true;
        }

        private bool MoveDeviceBlock(string deviceId, int targetIndex)
        {
            List<DeviceRowBlock> blocks = BuildDeviceRowBlocks();
            int sourceIndex = blocks.FindIndex(block => string.Equals(block.Root.Id, deviceId, StringComparison.Ordinal));
            if (sourceIndex < 0 || sourceIndex == targetIndex)
            {
                return false;
            }

            DeviceRowBlock block = blocks[sourceIndex];
            blocks.RemoveAt(sourceIndex);
            targetIndex = Math.Max(0, Math.Min(targetIndex, blocks.Count));
            blocks.Insert(targetIndex, block);

            ApplyOrderedBindingList(deviceRows, blocks.SelectMany(FlattenDeviceRowBlock));
            SyncDeviceOrderFromRows();
            SelectGridRowByItem(dgvDevices, block.Root);
            return true;
        }

        private bool MoveDeviceStrip(string parentDeviceId, string stripRowId, int targetIndex)
        {
            List<DeviceRowBlock> blocks = BuildDeviceRowBlocks();
            DeviceRowBlock? block = blocks.FirstOrDefault(item => string.Equals(item.Root.Id, parentDeviceId, StringComparison.Ordinal));
            if (block == null)
            {
                return false;
            }

            int sourceIndex = block.Strips.FindIndex(row => string.Equals(row.Id, stripRowId, StringComparison.Ordinal));
            if (sourceIndex < 0 || sourceIndex == targetIndex)
            {
                return false;
            }

            DeviceGridRow stripRow = block.Strips[sourceIndex];
            block.Strips.RemoveAt(sourceIndex);
            targetIndex = Math.Max(0, Math.Min(targetIndex, block.Strips.Count));
            block.Strips.Insert(targetIndex, stripRow);

            ApplyOrderedBindingList(deviceRows, blocks.SelectMany(FlattenDeviceRowBlock));
            SyncDeviceOrderFromRows();
            SelectGridRowByItem(dgvDevices, stripRow);
            return true;
        }

        private List<DeviceRowBlock> BuildDeviceRowBlocks()
        {
            var blocks = new List<DeviceRowBlock>();
            DeviceRowBlock? currentBlock = null;

            foreach (DeviceGridRow row in deviceRows)
            {
                if (row.Kind == DeviceRowKind.Device)
                {
                    currentBlock = new DeviceRowBlock(row);
                    blocks.Add(currentBlock);
                    continue;
                }

                if (currentBlock != null && string.Equals(row.ParentDeviceId, currentBlock.Root.Id, StringComparison.Ordinal))
                {
                    currentBlock.Strips.Add(row);
                }
            }

            return blocks;
        }

        private static IEnumerable<DeviceGridRow> FlattenDeviceRowBlock(DeviceRowBlock block)
        {
            yield return block.Root;
            foreach (DeviceGridRow stripRow in block.Strips)
            {
                yield return stripRow;
            }
        }

        private bool TryGetDeviceBlockInsertionIndex(Point clientPoint, string sourceDeviceId, out int targetIndex)
        {
            targetIndex = -1;

            List<string> deviceIds = GetRootDeviceRows()
                .Select(row => row.Id)
                .ToList();
            int sourceIndex = deviceIds.FindIndex(id => string.Equals(id, sourceDeviceId, StringComparison.Ordinal));
            if (sourceIndex < 0)
            {
                return false;
            }

            int insertionIndex = GetLinearInsertionIndex(dgvDevices, clientPoint);
            if (insertionIndex < 0)
            {
                return false;
            }

            DataGridView.HitTestInfo hit = dgvDevices.HitTest(clientPoint.X, clientPoint.Y);
            if (hit.RowIndex >= 0 && dgvDevices.Rows[hit.RowIndex].DataBoundItem is DeviceGridRow targetRow)
            {
                string targetDeviceId = targetRow.Kind == DeviceRowKind.Device ? targetRow.Id : targetRow.ParentDeviceId;
                int targetDeviceIndex = deviceIds.FindIndex(id => string.Equals(id, targetDeviceId, StringComparison.Ordinal));
                if (targetDeviceIndex < 0 || !TryGetDeviceBlockRange(targetDeviceId, out int blockStart, out int blockEnd))
                {
                    return false;
                }

                Rectangle topRowBounds = dgvDevices.GetRowDisplayRectangle(blockStart, false);
                Rectangle bottomRowBounds = dgvDevices.GetRowDisplayRectangle(blockEnd, false);
                int midpoint = topRowBounds.Top + ((bottomRowBounds.Bottom - topRowBounds.Top) / 2);
                insertionIndex = clientPoint.Y < midpoint ? targetDeviceIndex : targetDeviceIndex + 1;
            }
            else
            {
                insertionIndex = clientPoint.Y <= GetGridTop(dgvDevices) ? 0 : deviceIds.Count;
            }

            targetIndex = NormalizeMoveTargetIndex(sourceIndex, insertionIndex, deviceIds.Count);
            return targetIndex != sourceIndex;
        }

        private bool TryGetDeviceStripInsertionIndex(Point clientPoint, string parentDeviceId, string stripRowId, out int targetIndex)
        {
            targetIndex = -1;

            List<DeviceGridRow> stripRows = GetStripRows(parentDeviceId);
            int sourceIndex = stripRows.FindIndex(row => string.Equals(row.Id, stripRowId, StringComparison.Ordinal));
            if (sourceIndex < 0 || stripRows.Count <= 1)
            {
                return false;
            }

            DataGridView.HitTestInfo hit = dgvDevices.HitTest(clientPoint.X, clientPoint.Y);
            if (hit.RowIndex < 0 || dgvDevices.Rows[hit.RowIndex].DataBoundItem is not DeviceGridRow targetRow)
            {
                return false;
            }

            int insertionIndex;
            if (targetRow.Kind == DeviceRowKind.Device)
            {
                if (!string.Equals(targetRow.Id, parentDeviceId, StringComparison.Ordinal))
                {
                    return false;
                }

                insertionIndex = 0;
            }
            else
            {
                if (!string.Equals(targetRow.ParentDeviceId, parentDeviceId, StringComparison.Ordinal))
                {
                    return false;
                }

                int targetStripIndex = stripRows.FindIndex(row => string.Equals(row.Id, targetRow.Id, StringComparison.Ordinal));
                if (targetStripIndex < 0)
                {
                    return false;
                }

                Rectangle rowBounds = dgvDevices.GetRowDisplayRectangle(hit.RowIndex, false);
                insertionIndex = clientPoint.Y < rowBounds.Top + (rowBounds.Height / 2)
                    ? targetStripIndex
                    : targetStripIndex + 1;
            }

            targetIndex = NormalizeMoveTargetIndex(sourceIndex, insertionIndex, stripRows.Count);
            return targetIndex != sourceIndex;
        }

        private bool TryGetDeviceBlockRange(string deviceId, out int startIndex, out int endIndex)
        {
            startIndex = -1;
            endIndex = -1;

            for (int index = 0; index < deviceRows.Count; index++)
            {
                DeviceGridRow row = deviceRows[index];
                if (row.Kind != DeviceRowKind.Device || !string.Equals(row.Id, deviceId, StringComparison.Ordinal))
                {
                    continue;
                }

                startIndex = index;
                endIndex = index;
                for (int stripIndex = index + 1; stripIndex < deviceRows.Count; stripIndex++)
                {
                    DeviceGridRow nextRow = deviceRows[stripIndex];
                    if (nextRow.Kind == DeviceRowKind.Device)
                    {
                        break;
                    }

                    if (string.Equals(nextRow.ParentDeviceId, deviceId, StringComparison.Ordinal))
                    {
                        endIndex = stripIndex;
                    }
                }

                return true;
            }

            return false;
        }

        private static int GetLinearInsertionIndex(DataGridView grid, Point clientPoint)
        {
            if (grid.Rows.Count == 0)
            {
                return 0;
            }

            DataGridView.HitTestInfo hit = grid.HitTest(clientPoint.X, clientPoint.Y);
            if (hit.RowIndex < 0)
            {
                return clientPoint.Y <= GetGridTop(grid) ? 0 : grid.Rows.Count;
            }

            Rectangle rowBounds = grid.GetRowDisplayRectangle(hit.RowIndex, false);
            return clientPoint.Y < rowBounds.Top + (rowBounds.Height / 2)
                ? hit.RowIndex
                : hit.RowIndex + 1;
        }

        private static int GetGridTop(DataGridView grid)
        {
            return grid.Rows.Count == 0 ? 0 : grid.GetRowDisplayRectangle(0, false).Top;
        }

        private static bool MoveLinearItem<T>(List<T> items, Func<T, string> idSelector, string sourceId, int insertionIndex, out T? movedItem)
            where T : class
        {
            movedItem = null;

            int sourceIndex = items.FindIndex(item => string.Equals(idSelector(item), sourceId, StringComparison.Ordinal));
            if (sourceIndex < 0)
            {
                return false;
            }

            int targetIndex = NormalizeMoveTargetIndex(sourceIndex, insertionIndex, items.Count);
            if (targetIndex == sourceIndex)
            {
                return false;
            }

            movedItem = items[sourceIndex];
            items.RemoveAt(sourceIndex);
            targetIndex = Math.Max(0, Math.Min(targetIndex, items.Count));
            items.Insert(targetIndex, movedItem);
            return true;
        }

        private static bool CanMoveLinearRow(IReadOnlyList<string> ids, string sourceId, int insertionIndex)
        {
            int sourceIndex = -1;
            for (int index = 0; index < ids.Count; index++)
            {
                if (string.Equals(ids[index], sourceId, StringComparison.Ordinal))
                {
                    sourceIndex = index;
                    break;
                }
            }

            return sourceIndex >= 0 && NormalizeMoveTargetIndex(sourceIndex, insertionIndex, ids.Count) != sourceIndex;
        }

        private static int NormalizeMoveTargetIndex(int sourceIndex, int insertionIndex, int itemCount)
        {
            insertionIndex = Math.Max(0, Math.Min(insertionIndex, itemCount));
            if (insertionIndex > sourceIndex)
            {
                insertionIndex--;
            }

            return insertionIndex;
        }

        private static void ApplyOrderedBindingList<T>(BindingList<T> list, IEnumerable<T> orderedItems)
        {
            List<T> materialized = orderedItems.ToList();
            list.Clear();
            foreach (T item in materialized)
            {
                list.Add(item);
            }
        }

        private void SyncSceneOrderFromRows()
        {
            Dictionary<string, SceneConfig> scenesById = appConfig.Scenes
                .ToDictionary(scene => scene.Id, StringComparer.Ordinal);
            appConfig.Scenes = sceneRows
                .Where(row => scenesById.ContainsKey(row.Id))
                .Select(row => scenesById[row.Id])
                .ToList();
        }

        private void SyncCollectionOrderFromRows()
        {
            Dictionary<string, ConfigurationCollection> collectionsById = appConfig.Collections
                .ToDictionary(collection => collection.Id, StringComparer.Ordinal);
            appConfig.Collections = collectionRows
                .Where(row => collectionsById.ContainsKey(row.Id))
                .Select(row => collectionsById[row.Id])
                .ToList();
        }

        private void SyncDeviceOrderFromRows()
        {
            appConfig.Devices = GetRootDeviceRows()
                .Select(BuildDeviceConfigFromRows)
                .ToList();
        }

        private static void SelectGridRowByItem(DataGridView grid, object? item)
        {
            if (item == null)
            {
                return;
            }

            int firstVisibleColumnIndex = GetFirstVisibleColumnIndex(grid);
            grid.ClearSelection();

            foreach (DataGridViewRow row in grid.Rows)
            {
                if (!ReferenceEquals(row.DataBoundItem, item))
                {
                    continue;
                }

                row.Selected = true;
                if (row.Cells.Count > firstVisibleColumnIndex)
                {
                    grid.CurrentCell = row.Cells[firstVisibleColumnIndex];
                }

                break;
            }
        }

        private static int GetFirstVisibleColumnIndex(DataGridView grid)
        {
            DataGridViewColumn? firstVisibleColumn = grid.Columns
                .Cast<DataGridViewColumn>()
                .Where(column => column.Visible)
                .OrderBy(column => column.DisplayIndex)
                .FirstOrDefault();
            return firstVisibleColumn?.Index ?? 0;
        }

        private void ClearPendingGridReorderDrag()
        {
            reorderDragGrid = null;
            reorderDragRowIndex = -1;
            reorderDragStartPoint = Point.Empty;
        }
    }
}
