using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Forms
{
    public enum Keys { None = 0, Back = 8, Tab = 9, Return = 13, Enter = 13, Escape = 27, Delete = 46 }

    public class PreviewKeyDownEventArgs : EventArgs
    {
        public PreviewKeyDownEventArgs(Keys keyData) { KeyData = keyData; }
        public Keys KeyData { get; }
        public Keys KeyCode => KeyData;
        public bool IsInputKey { get; set; }
    }
    public delegate void PreviewKeyDownEventHandler(object sender, PreviewKeyDownEventArgs e);

    public class KeyEventArgs : EventArgs
    {
        public KeyEventArgs(Keys keyData) { KeyData = keyData; }
        public Keys KeyData { get; }
        public Keys KeyCode => KeyData;
        public bool Handled { get; set; }
        public bool SuppressKeyPress { get; set; }
    }
    public delegate void KeyEventHandler(object sender, KeyEventArgs e);

    // ------------------------------------------------------------------ ListView
    public class ColumnHeader
    {
        public string Text { get; set; } = "";
        public string Name { get; set; } = "";
        public int Width { get; set; } = 60;
    }

    public class ListViewItem
    {
        public ListViewItem() { SubItems = new ListViewSubItemCollection(this); }
        public ListViewItem(string text) : this() { Text = text; }
        public string Text { get => SubItems.Count > 0 ? SubItems[0].Text : ""; set { if (SubItems.Count == 0) SubItems.Add(value); else SubItems[0].Text = value ?? ""; } }
        public string Name { get; set; } = "";
        public object Tag { get; set; }
        public bool Selected { get; set; }
        public bool Checked { get; set; }
        public ListViewSubItemCollection SubItems { get; }

        public class ListViewSubItem
        {
            public ListViewSubItem(string text) { Text = text ?? ""; }
            public string Text { get; set; }
        }

        public class ListViewSubItemCollection : IEnumerable<ListViewSubItem>
        {
            private readonly ListViewItem _owner;
            private readonly List<ListViewSubItem> _items = new List<ListViewSubItem>();
            internal ListViewSubItemCollection(ListViewItem owner) { _owner = owner; }
            public int Count => _items.Count;
            public ListViewSubItem this[int i] => _items[i];
            public ListViewSubItem Add(string text)
            {
                var it = new ListViewSubItem(text); _items.Add(it); return it;
            }
            public void Clear() => _items.Clear();
            public IEnumerator<ListViewSubItem> GetEnumerator() => _items.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();
        }
    }

    public class ListView : Control
    {
        public ListView() { Items = new ListViewItemCollection(this); }
        public List<ColumnHeader> Columns { get; } = new List<ColumnHeader>();
        public ListViewItemCollection Items { get; }
        public bool MultiSelect { get; set; } = true;
        public bool CheckBoxes { get; set; }
        public bool FullRowSelect { get; set; }
        public List<ListViewItem> SelectedItems => Items.Where(i => i.Selected).ToList();
        public List<ListViewItem> CheckedItems => Items.Where(i => i.Checked).ToList();

        /// <summary>Selects one item (single click in the view).</summary>
        public void Select(ListViewItem item, bool addToSelection = false)
        {
            if (!addToSelection || !MultiSelect) foreach (var i in Items) i.Selected = false;
            if (item != null) item.Selected = true;
            SelectedIndexChanged?.Invoke(this, EventArgs.Empty);
            NotifyStateChanged();
        }
        public event EventHandler SelectedIndexChanged;

        public class ListViewItemCollection : IEnumerable<ListViewItem>
        {
            private readonly ListView _owner;
            private readonly List<ListViewItem> _items = new List<ListViewItem>();
            internal ListViewItemCollection(ListView owner) { _owner = owner; }
            public int Count => _items.Count;
            public ListViewItem this[int i] => _items[i];
            public ListViewItem Add(ListViewItem item) { _items.Add(item); _owner.NotifyStateChanged(); return item; }
            public ListViewItem Add(string text) => Add(new ListViewItem(text));
            public void Remove(ListViewItem item) { _items.Remove(item); _owner.NotifyStateChanged(); }
            public void Clear() { _items.Clear(); _owner.NotifyStateChanged(); }
            public IEnumerator<ListViewItem> GetEnumerator() => _items.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();
        }
    }

    // ------------------------------------------------------------------ DataGridView
    public class DataGridViewColumn
    {
        public string Name { get; set; } = "";
        public string HeaderText { get; set; } = "";
        public string ToolTipText { get; set; } = "";
        public bool ReadOnly { get; set; }
        public int Width { get; set; } = 100;
        public bool Visible { get; set; } = true;
    }
    public class DataGridViewTextBoxColumn : DataGridViewColumn { }

    public class DataGridViewCell
    {
        public object Value { get; set; }
        public DataGridViewRow OwningRow { get; internal set; }
        public DataGridViewColumn OwningColumn { get; internal set; }
        public string ToolTipText { get; set; } = "";
    }

    public class DataGridViewCellCollection : IEnumerable<DataGridViewCell>
    {
        private readonly DataGridViewRow _row;
        internal readonly List<DataGridViewCell> Items = new List<DataGridViewCell>();
        internal DataGridViewCellCollection(DataGridViewRow row) { _row = row; }
        public int Count => Items.Count;
        public DataGridViewCell this[int index] => Items[index];
        public DataGridViewCell this[string columnName]
        {
            get
            {
                var grid = _row.DataGridView ?? throw new ArgumentException("Column named " + columnName + " cannot be found.");
                int i = grid.Columns.IndexOf(columnName);
                if (i < 0) throw new ArgumentException("Column named " + columnName + " cannot be found.", nameof(columnName));
                return Items[i];
            }
        }
        public IEnumerator<DataGridViewCell> GetEnumerator() => Items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();
    }

    public class DataGridViewRow
    {
        public DataGridViewRow() { Cells = new DataGridViewCellCollection(this); }
        public DataGridViewCellCollection Cells { get; }
        public DataGridView DataGridView { get; internal set; }
        public int Index => DataGridView?.Rows.IndexOf(this) ?? -1;
        public bool IsNewRow => false;
        public bool Selected { get; set; }
        public object Tag { get; set; }
    }

    public class DataGridViewColumnCollection : IEnumerable<DataGridViewColumn>
    {
        private readonly DataGridView _grid;
        private readonly List<DataGridViewColumn> _items = new List<DataGridViewColumn>();
        internal DataGridViewColumnCollection(DataGridView grid) { _grid = grid; }
        public int Count => _items.Count;
        public DataGridViewColumn this[int i] => _items[i];
        public DataGridViewColumn this[string name] => _items.FirstOrDefault(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase));
        public int IndexOf(string name) => _items.FindIndex(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase));
        public int Add(DataGridViewColumn column) { _items.Add(column); _grid.OnColumnAdded(column); return _items.Count - 1; }
        public int Add(string columnName, string headerText) => Add(new DataGridViewTextBoxColumn { Name = columnName, HeaderText = headerText });
        public void AddRange(params DataGridViewColumn[] columns) { foreach (var c in columns) Add(c); }
        public void Clear() => _items.Clear();
        public IEnumerator<DataGridViewColumn> GetEnumerator() => _items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();
    }

    public class DataGridViewRowCollection : IEnumerable<DataGridViewRow>, IEnumerable
    {
        private readonly DataGridView _grid;
        private readonly List<DataGridViewRow> _items = new List<DataGridViewRow>();
        internal DataGridViewRowCollection(DataGridView grid) { _grid = grid; }
        public int Count => _items.Count;
        public DataGridViewRow this[int i] => _items[i];
        public int IndexOf(DataGridViewRow row) => _items.IndexOf(row);

        /// <summary>Adds a row with the given cell values (like WinForms) and raises RowsAdded.</summary>
        public int Add(params object[] values)
        {
            var row = new DataGridViewRow { DataGridView = _grid };
            for (int i = 0; i < _grid.Columns.Count; i++)
                row.Cells.Items.Add(new DataGridViewCell { Value = values != null && i < values.Length ? values[i] : null, OwningRow = row, OwningColumn = _grid.Columns[i] });
            _items.Add(row);
            _grid.OnRowsAdded(new DataGridViewRowsAddedEventArgs(_items.Count - 1, 1));
            return _items.Count - 1;
        }
        public void RemoveAt(int index)
        {
            _items.RemoveAt(index);
            _grid.OnRowsRemoved(new DataGridViewRowsRemovedEventArgs(index, 1));
        }
        public void Remove(DataGridViewRow row) { int i = _items.IndexOf(row); if (i >= 0) RemoveAt(i); }
        public void Clear()
        {
            int n = _items.Count; _items.Clear();
            if (n > 0) _grid.OnRowsRemoved(new DataGridViewRowsRemovedEventArgs(0, n));
        }
        internal void EnsureCells(DataGridViewColumn column)
        {
            foreach (var r in _items) r.Cells.Items.Add(new DataGridViewCell { OwningRow = r, OwningColumn = column });
        }
        public IEnumerator<DataGridViewRow> GetEnumerator() => _items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();
    }

    public class DataGridViewCellEventArgs : EventArgs
    {
        public DataGridViewCellEventArgs(int columnIndex, int rowIndex) { ColumnIndex = columnIndex; RowIndex = rowIndex; }
        public int ColumnIndex { get; }
        public int RowIndex { get; }
    }
    public class DataGridViewRowsAddedEventArgs : EventArgs
    {
        public DataGridViewRowsAddedEventArgs(int rowIndex, int rowCount) { RowIndex = rowIndex; RowCount = rowCount; }
        public int RowIndex { get; }
        public int RowCount { get; }
    }
    public class DataGridViewRowsRemovedEventArgs : EventArgs
    {
        public DataGridViewRowsRemovedEventArgs(int rowIndex, int rowCount) { RowIndex = rowIndex; RowCount = rowCount; }
        public int RowIndex { get; }
        public int RowCount { get; }
    }
    public delegate void DataGridViewCellEventHandler(object sender, DataGridViewCellEventArgs e);
    public delegate void DataGridViewRowsAddedEventHandler(object sender, DataGridViewRowsAddedEventArgs e);
    public delegate void DataGridViewRowsRemovedEventHandler(object sender, DataGridViewRowsRemovedEventArgs e);

    /// <summary>Headless DataGridView: rows of cells plus the editing events the recovered code listens to.</summary>
    public class DataGridView : Control
    {
        public DataGridView() { Columns = new DataGridViewColumnCollection(this); Rows = new DataGridViewRowCollection(this); }
        public DataGridViewColumnCollection Columns { get; }
        public DataGridViewRowCollection Rows { get; }
        public bool ReadOnly { get; set; }
        public bool AllowUserToAddRows { get; set; } = true;
        public bool AllowUserToDeleteRows { get; set; } = true;

        public event DataGridViewCellEventHandler CellEndEdit;
        public event DataGridViewRowsAddedEventHandler RowsAdded;
        public event DataGridViewRowsRemovedEventHandler RowsRemoved;

        internal void OnColumnAdded(DataGridViewColumn c) => Rows.EnsureCells(c);
        internal void OnRowsAdded(DataGridViewRowsAddedEventArgs e) { RowsAdded?.Invoke(this, e); NotifyStateChanged(); }
        internal void OnRowsRemoved(DataGridViewRowsRemovedEventArgs e) { RowsRemoved?.Invoke(this, e); NotifyStateChanged(); }

        /// <summary>The view calls this after the user edited a cell. Empty text becomes null, as in WinForms.</summary>
        public void EndEdit(int rowIndex, int columnIndex, string text)
        {
            Rows[rowIndex].Cells[columnIndex].Value = string.IsNullOrEmpty(text) ? null : text;
            CellEndEdit?.Invoke(this, new DataGridViewCellEventArgs(columnIndex, rowIndex));
            NotifyStateChanged();
        }
    }
}
